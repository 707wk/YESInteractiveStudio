Imports Newtonsoft.Json
Imports YESInteractiveSDK

Public Class PlayWindow
    ''' <summary>
    ''' 播放窗口信息
    ''' </summary>
    Public DisplayingWindow As DisplayingWindow

#Region "关闭窗体"
    Public Delegate Sub CloseFormCallback()
    Public Sub CloseForm()
        If Me.InvokeRequired Then
            Me.Invoke(New CloseFormCallback(AddressOf CloseForm))
            Exit Sub
        End If

        IsClose = True
        DisplayingWindow.StartOfCompletedSensorDataEvent.Set()
        WorkThread.Join()
        WorkThread = Nothing

        Timer1.Stop()

        ClearPlayControl()

        Me.Close()

    End Sub
#End Region

#Region "隐藏/显示窗体"
    Public Delegate Sub HideFormCallback(value As Boolean)
    Public Sub HideForm(value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New HideFormCallback(AddressOf HideForm),
                      New Object() {value})
            Exit Sub
        End If

        If value Then
            Me.Size = New Size(1, 1)

        Else
            Me.Size = DisplayingWindow.SizeOfZoom

        End If

    End Sub
#End Region

#Region "切换显示模式"
    Public Delegate Sub DisplayModeChangeCallback()
    Public Sub DisplayModeChange()
        If Me.InvokeRequired Then
            Me.Invoke(New DisplayModeChangeCallback(AddressOf DisplayModeChange))
            Exit Sub
        End If

        Static LastDisplayMode As InteractiveOptions.DISPLAYMODE = InteractiveOptions.DISPLAYMODE.DEBUG

        ''todo:切换显示模式
        Select Case AppSettingHelper.GetInstance.DisplayMode
            Case InteractiveOptions.DISPLAYMODE.INTERACT
                If AppSettingHelper.GetInstance.DisplayMode = LastDisplayMode Then
                    Exit Sub
                End If
                StartPlay(DisplayingWindow.IsAutoPlay)

            Case InteractiveOptions.DISPLAYMODE.TEST
                StartPlay(False)
                ClearPlayControl()
                ChangeToTestMode()

            Case InteractiveOptions.DISPLAYMODE.BLACK
                StartPlay(False)
                ClearPlayControl()
                ChangeToBlackMode()

            Case InteractiveOptions.DISPLAYMODE.DEBUG
                StartPlay(False)
                ClearPlayControl()
                ChangeToDebugMode()

        End Select

        LastDisplayMode = AppSettingHelper.GetInstance.DisplayMode

    End Sub

#Region "切换成测试模式"
    ''' <summary>
    ''' 切换成测试模式
    ''' </summary>
    Private Sub ChangeToTestMode()
        Me.Refresh()
        DrawScanBoardRectangle()

    End Sub
#End Region

#Region "切换成黑屏模式"
    ''' <summary>
    ''' 切换成黑屏模式
    ''' </summary>
    Private Sub ChangeToBlackMode()
        Me.Refresh()

    End Sub
#End Region

#Region "切换成调试模式"
    ''' <summary>
    ''' 切换成调试模式
    ''' </summary>
    Private Sub ChangeToDebugMode()
        Me.Refresh()
        DrawScanBoardRectangle()

    End Sub
#End Region

#Region "绘制接收卡边框"
    ''' <summary>
    ''' 绘制接收卡边框
    ''' </summary>
    Private Sub DrawScanBoardRectangle()

        Using tmpPen As New Pen(Color.Green)
            Dim tmpGraphics = Me.CreateGraphics

            For Each screenID In DisplayingWindow.ScreenIDItems
                For Each scanBoardItem In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems(screenID).NovaStarScanBoardItems
                    tmpGraphics.DrawRectangle(tmpPen,
                                          scanBoardItem.LocationInDisplayingWindow.X,
                                          scanBoardItem.LocationInDisplayingWindow.Y,
                                          scanBoardItem.SizeOfZoom.Width - 1,
                                          scanBoardItem.SizeOfZoom.Height - 1)
                Next
            Next
        End Using

    End Sub
#End Region

#End Region

#Region "开始/停止轮播"
    Public Delegate Sub StartPlayCallback(value As Boolean)
    Public Sub StartPlay(value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New StartPlayCallback(AddressOf StartPlay),
                      New Object() {value})
            Exit Sub
        End If

        Me.Refresh()

        If value AndAlso
            DisplayingWindow.PlayFileItems.Count > 0 Then

            Timer1.Interval = DisplayingWindow.PlayFileItems(DisplayingWindow.PlayFileID).PlaySecond * 1000
            Timer1.Start()
            PlayMedia()

        Else
            Timer1.Stop()
            ClearPlayControl()

        End If

    End Sub
#End Region

    Private Sub PlayWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        StrStringFormat.Alignment = StringAlignment.Center
        StrStringFormat.LineAlignment = StringAlignment.Center
    End Sub

    Private Sub PlayWindow_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'If DisplayingWindow.IsAutoPlay AndAlso
        '    DisplayingWindow.PlayFileItems.Count > 0 Then

        '    StartPlay(True)
        'End If

        ''todo:开启处理数据线程
        WorkThread = New Threading.Thread(AddressOf WorkFunction) With {
            .IsBackground = True
        }
        WorkThread.Start()

    End Sub

#Region "后台数据处理"
    ''' <summary>
    ''' 是否关闭窗体
    ''' </summary>
    Private IsClose As Boolean = False
    ''' <summary>
    ''' 工作线程
    ''' </summary>
    Private WorkThread As Threading.Thread
    ''' <summary>
    ''' 活动点列表
    ''' </summary>
    Private PointOfMergeItems As New List(Of PointOfMerge)
    ''' <summary>
    ''' 合并点列表
    ''' </summary>
    Private PointInfoItems As New List(Of PointInfo)

#Region "数据处理主函数"
    ''' <summary>
    ''' 数据处理主函数
    ''' </summary>
    Public Sub WorkFunction()
        Do

            DisplayingWindow.StartOfCompletedSensorDataEvent.WaitOne()
            If IsClose Then Exit Do

            Select Case AppSettingHelper.GetInstance.DisplayMode
                Case InteractiveOptions.DISPLAYMODE.INTERACT
                    WorkFunctionInInteractMode()

                Case InteractiveOptions.DISPLAYMODE.TEST
                    WorkFunctionInTestMode()

                Case InteractiveOptions.DISPLAYMODE.BLACK
                    '无操作

                Case InteractiveOptions.DISPLAYMODE.DEBUG
                    WorkFunctionInDebugMode()

            End Select

            SensorDataProcessingHelper.EndOfCompletedSensorDataEvent.Signal()

        Loop While Not IsClose

    End Sub
#End Region

#Region "互动模式时数据处理方式"
    ''' <summary>
    ''' 互动模式时数据处理方式
    ''' </summary>
    Private Sub WorkFunctionInInteractMode()
        '网格距离法
        'https://www.cnblogs.com/LBSer/p/4417127.html
        Dim distance2Pow = Math.Pow(AppSettingHelper.GetInstance.PositionaIAccuracy, 2)
        Dim ValidSensorMinimum = AppSettingHelper.GetInstance.ValidSensorMinimum

        PointOfMergeItems.Clear()
        For Each sensorItem In DisplayingWindow.ActiveSensorItems

            Dim isInclude As Boolean = False
            For Each pointOfMergeItem In PointOfMergeItems
                '判断是否在合并范围内
                If Math.Pow(pointOfMergeItem.X - sensorItem.LocationOfCenter.X, 2) + Math.Pow(pointOfMergeItem.Y - sensorItem.LocationOfCenter.Y, 2) <= distance2Pow Then

                    pointOfMergeItem.XSum += sensorItem.LocationOfCenter.X
                    pointOfMergeItem.YSum += sensorItem.LocationOfCenter.Y
                    pointOfMergeItem.SensorCount += 1

                    '有一个点被感应则标记为新点
                    If sensorItem.State = InteractiveOptions.SensorState.DOWN Then
                        pointOfMergeItem.IsNew = True
                    End If

                    isInclude = True
                    Exit For

                End If
            Next

            '没有被合并则添加为新聚合点
            If Not isInclude Then
                Dim addPointOfMerge = New PointOfMerge
                With addPointOfMerge
                    .X = sensorItem.LocationOfCenter.X
                    .Y = sensorItem.LocationOfCenter.Y
                    .XSum = .X
                    .YSum = .Y
                    .SensorCount = 1

                    .IsNew = (sensorItem.State = InteractiveOptions.SensorState.DOWN)
                End With

                PointOfMergeItems.Add(addPointOfMerge)
            End If

        Next

        '过滤及转换
        PointInfoItems.Clear()
        For Each item In PointOfMergeItems
            If item.SensorCount < ValidSensorMinimum Then
                Continue For
            End If

            item.X = item.XSum \ item.SensorCount
            item.Y = item.YSum \ item.SensorCount

            PointInfoItems.Add(New PointInfo With {
                               .ID = PointInfoItems.Count + 1,
                               .X = item.X,
                               .Y = item.Y,
                               .Old = If(item.IsNew, 0, 1)
                               })

        Next

        'Console.WriteLine(JsonConvert.SerializeObject(DisplayingWindow.ActiveSensorItems))
        'Console.WriteLine(JsonConvert.SerializeObject(PointInfoItems))
        PointActive(PointInfoItems)

    End Sub
#End Region

#Region "测试模式时数据处理方式"
    ''' <summary>
    ''' 测试模式时数据处理方式
    ''' </summary>
    Private Sub WorkFunctionInTestMode()
        ''todo:测试模式时数据处理方式
        Using tmpSolidBrush As New SolidBrush(Color.Green)
            Dim tmpGraphics = Me.CreateGraphics

            For Each sensorItem In DisplayingWindow.ActiveSensorItems
                tmpGraphics.FillRectangle(tmpSolidBrush,
                                      sensorItem.Location.X,
                                      sensorItem.Location.Y,
                                      sensorItem.Size,
                                      sensorItem.Size)
            Next
        End Using

    End Sub
#End Region

#Region "调试模式时数据处理方式"
    ''' <summary>
    ''' 自绘字体格式
    ''' </summary>
    Private Shared ReadOnly StrStringFormat As New StringFormat()

    ''' <summary>
    ''' 调试模式时数据处理方式
    ''' </summary>
    Private Sub WorkFunctionInDebugMode()
        ''todo:调试模式时数据处理方式
        Using tmpSolidBrush As New SolidBrush(Color.Green)
            Dim tmpGraphics = Me.CreateGraphics

            For Each sensorItem In DisplayingWindow.ActiveSensorItems
                tmpGraphics.DrawString(sensorItem.Value,
                                       Me.Font,
                                       tmpSolidBrush,
                                       sensorItem.LocationOfCenter,
                                       StrStringFormat)
            Next
        End Using

    End Sub
#End Region

#End Region

#Region "点击事件"
    Public Delegate Sub PointActiveCallback(values As List(Of PointInfo))
    ''' <summary>
    ''' 点击事件
    ''' </summary>
    Public Sub PointActive(values As List(Of PointInfo))
        If Me.InvokeRequired Then
            Me.Invoke(New PointActiveCallback(AddressOf PointActive),
                      New Object() {values})
            Exit Sub
        End If

        PlayControl?.PointActive(values)

    End Sub
#End Region

#Region "定时切换节目"
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        ''todo:定时切换节目
        DisplayingWindow.PlayFileID = (DisplayingWindow.PlayFileID + 1) Mod DisplayingWindow.PlayFileItems.Count
        Timer1.Interval = DisplayingWindow.PlayFileItems(DisplayingWindow.PlayFileID).PlaySecond * 1000
        PlayMedia()

    End Sub
#End Region

#Region "播放文件"
    Private PlayControl As IPlayBaseControl

    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        'Me.TopMost = True

    End Sub

    Private Sub PlayMedia()
        ClearPlayControl()

        Dim path = DisplayingWindow.PlayFileItems(DisplayingWindow.PlayFileID).Path

        Select Case System.IO.Path.GetExtension(path).ToLower()
            Case ".swf"
                PlayControl = New PlayFlashControl()
            Case ".dll"
                PlayControl = New PlayDllControl()
            Case ".exe"
                PlayControl = New PlayUnityControl()
        End Select

        Try
            PlayControl.Init(Me.Controls, path)
        Catch ex As Exception
        End Try

    End Sub

    Private Sub ClearPlayControl()

        PlayControl?.Dispose()
        PlayControl = Nothing

    End Sub

#Region "Unity事件"
    Private Sub PlayWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        PlayControl?.FormActivated()
    End Sub

    Private Sub PlayWindow_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        PlayControl?.FormDeactivate()
    End Sub
#End Region

#End Region
End Class