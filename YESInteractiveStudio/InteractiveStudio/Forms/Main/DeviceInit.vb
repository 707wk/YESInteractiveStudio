Imports System.ComponentModel
Imports System.Threading
Imports Nova.Mars.SDK

Public Class DeviceInit
#Region "窗体初始化/关闭"
    Private Sub DeviceInit_Load(sender As Object, e As EventArgs) Handles Me.Load
        'MsgBox($"{My.Application.Info.Title}.{[Enum].GetName(GetType(Wangk.Resource.MultiLanguage.LANG),
        '                                                     Wangk.Resource.MultiLanguage.LANG.EN)}.resources")
        'Dim TmpDialog As New AboutBox
        'TmpDialog.ShowDialog()
        'End

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
    End Sub

    Private Sub DeviceInit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '启动后台线程
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub DeviceInit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing

    End Sub

    '关闭窗体
    Public Delegate Sub CloseCallback(ByVal Value As Boolean)
    Public Overloads Sub Close(ByVal Value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New CloseCallback(AddressOf Close), New Object() {Value})
            Exit Sub
        End If

        Me.Close()
    End Sub
#End Region

#Region "后台线程"
    ''' <summary>
    ''' 后台线程
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        '判断诺瓦服务是否启动
        If System.Diagnostics.Process.GetProcessesByName("MarsServerProvider").Length = 0 Then
            Dim tmpProcessHwnd As Process = Process.Start($".\Nova\Server\MarsServerProvider.exe")
            ShowInfo($"{AppSetting.Language.GetS("Start Nova Serve")}：{If(tmpProcessHwnd.Handle, True, False)}")
            Thread.Sleep(5000)
        End If

        NovaInitialize(True)
    End Sub
#End Region

#Region "显示信息"
    ''' <summary>
    ''' 显示信息
    ''' </summary>
    Public Delegate Sub ShowInfoCallback(ByVal text As String)
    Public Sub ShowInfo(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New ShowInfoCallback(AddressOf ShowInfo), New Object() {text})
            Exit Sub
        End If

        ToolStripStatusLabel1.Text = text
        '有错误则提示并退出
        If text.IndexOf("ERROR") = -1 Then
            Exit Sub
        End If

        MsgBox($"{text}", MsgBoxStyle.Information, Me.Text)

        End
    End Sub
#End Region

#Region "获取ip通知"
    ''' <summary>
    ''' 获取ip通知
    ''' </summary>
    Private Sub GetEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        Static Dim senderArrayId As Integer = 0

        If e.IsExecResult Then
            AppSetting.SenderList(senderArrayId).IpDate = e.Data

            senderArrayId += 1
            If senderArrayId < AppSetting.SenderList.Length Then
                AppSetting.MainClass.GetEquipmentIP(senderArrayId)
            Else
                ShowInfo(AppSetting.Language.GetS("Loading Completed"))
                '移除事件
                RemoveHandler AppSetting.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.Close(True)
            End If
        Else
            '移除事件
            RemoveHandler AppSetting.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
            ShowInfo($"ERROR:{AppSetting.Language.GetS("Failed to Get IP data!Please check the equipment and reset it")}!")
        End If
    End Sub
#End Region

#Region "读取屏幕信息"
    ''' <summary>
    ''' 读取屏幕信息
    ''' </summary>
    Public Delegate Sub novaInitializeCallback(ByVal text As String)
    Public Sub NovaInitialize(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New novaInitializeCallback(AddressOf NovaInitialize), New Object() {text})
            Exit Sub
        End If

#Region "连接Nova服务"
        ShowInfo(AppSetting.Language.GetS("Connect Nova Serve ..."))
        AppSetting.RootClass = New MarsHardwareEnumerator
        If Not AppSetting.RootClass.Initialize() Then
            ShowInfo($"ERROR:{AppSetting.Language.GetS("Failed to Connect Nova Serve")}")
            Exit Sub
        End If
#End Region

#Region "查找控制系统"
        For i001 As Integer = 0 To 20 - 1
            ShowInfo(AppSetting.Language.GetS("Searching Control System") & i001)

            If AppSetting.RootClass.CtrlSystemCount() < 1 Then
                If i001 = 20 - 1 Then
                    ShowInfo($"ERROR:{AppSetting.Language.GetS("No control system found")}")
                    Exit Sub
                End If

                Thread.Sleep(1000)
                Continue For
            End If

            Exit For
        Next
#End Region

        AppSetting.MainClass = New MarsControlSystem(AppSetting.RootClass)
        '绑定读取到ip事件
        AddHandler AppSetting.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        AppSetting.RootClass.GetComNameOfControlSystem(0, tmpstr)
        AppSetting.MainClass.Initialize(tmpstr, screenCount, senderCount)

        If senderCount = 0 Then
            ShowInfo($"ERROR:{AppSetting.Language.GetS("No controller found")}")
            Exit Sub
        End If

        ShowInfo(AppSetting.Language.GetS("Reading display screen information"))
        Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing
        If AppSetting.MainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
            ShowInfo($"ERROR:{AppSetting.Language.GetS("Failed to Reading display screen information")}")
            Exit Sub
        End If

        If LEDScreenInfoList Is Nothing OrElse
            LEDScreenInfoList.Count = 0 Then
            ShowInfo($"ERROR:{AppSetting.Language.GetS("No display screen found")}")
            Exit Sub
        End If

        ShowInfo(AppSetting.Language.GetS("Loading screen information"))
        AppSetting.ScanBoardTable = New Hashtable
        ReDim AppSetting.ScreenList(screenCount - 1)
        ReDim AppSetting.SenderList(senderCount - 1)
        For i As Integer = 0 To AppSetting.SenderList.Length - 1
            ReDim AppSetting.SenderList(i).TmpIpData(12 - 1)
        Next

#Region "遍历屏幕"
        '遍历屏幕
        For LEDScreenId As Integer = 0 To screenCount - 1
            With AppSetting.ScreenList(LEDScreenId)

                Dim x As Integer
                Dim y As Integer
                '获取起始位置 大小
                AppSetting.MainClass.GetScreenLocation(LEDScreenId,
                                            x,
                                            y,
                                            .DefSize.Width,
                                            .DefSize.Height)
                '屏幕单元宽度
                .DefScanBoardSize.Width = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Width
                '屏幕单元高度
                .DefScanBoardSize.Height = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Height

#Region "根据尺寸判断感应单元布局"
                '根据尺寸判断感应单元布局
                If .DefScanBoardSize.Width * 4 = .DefScanBoardSize.Height Then
                    '4 1
                    .SensorLayout.Height = 4
                    .SensorLayout.Width = 1
                ElseIf .DefScanBoardSize.Width = .DefScanBoardSize.Height * 4 Then
                    '1 4
                    .SensorLayout.Height = 1
                    .SensorLayout.Width = 4
                Else
                    '4 4
                    .SensorLayout.Height = 4
                    .SensorLayout.Width = 4
                End If
#End Region

                '创建上次点击状态缓存
                ReDim .SensorMap((.DefSize.Height \ .DefScanBoardSize.Height) * .SensorLayout.Height,
                                       (.DefSize.Width \ .DefScanBoardSize.Width) * .SensorLayout.Width)

#Region "存储接收卡信息"
                '存储接收卡信息
                .SenderList = New List(Of Integer)
                For Each i001 As ScanBoardMapRegion In LEDScreenInfoList(LEDScreenId).ScanBoardInfoList
                    '接收卡留空则不添加
                    If i001.SenderIndex = &HFF Then
                        Continue For
                    End If

                    AppSetting.ScreenList(LEDScreenId).SenderList.Add(i001.SenderIndex)

                    Dim tmpScanBoardInfo As New ScanBoardInfo With {
                        .ScreenId = LEDScreenId,'屏幕索引
                        .SenderId = i001.SenderIndex,'控制器索引
                        .PortId = i001.PortIndex,'网口索引
                        .ConnectId = i001.ConnectIndex'接收卡索引
                    }
                    '屏幕块位置
                    tmpScanBoardInfo.Location.X = (i001.X \ .DefScanBoardSize.Width) * .SensorLayout.Width
                    tmpScanBoardInfo.Location.Y = (i001.Y \ .DefScanBoardSize.Height) * .SensorLayout.Height

                    AppSetting.ScanBoardTable.Add($"{i001.SenderIndex}-{i001.PortIndex}-{i001.ConnectIndex}", tmpScanBoardInfo)
                Next
#End Region
            End With
        Next
#End Region

        AppSetting.MainClass.GetEquipmentIP(0)
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With AppSetting.Language
            Me.ToolStripStatusLabel1.Text = .GetS("Start ...")
        End With
    End Sub
#End Region
End Class