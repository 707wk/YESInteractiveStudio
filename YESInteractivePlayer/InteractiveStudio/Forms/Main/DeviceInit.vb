Imports System.ComponentModel
Imports System.Threading
Imports Nova.Mars.SDK

Public Class DeviceInit
#Region "窗体初始化/关闭"
    Private Sub DeviceInit_Load(sender As Object, e As EventArgs) Handles Me.Load

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
            ShowInfo($"{sysInfo.Language.GetLang("启动Nova服务")}：{If(tmpProcessHwnd.Handle, True, False)}")
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
            sysInfo.SenderList(senderArrayId).IpDate = e.Data

            senderArrayId += 1
            If senderArrayId < sysInfo.SenderList.Length Then
                sysInfo.MainClass.GetEquipmentIP(senderArrayId)
            Else
                ShowInfo(sysInfo.Language.GetLang("加载完成"))
                '移除事件
                RemoveHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.Close(True)
            End If
        Else
            '移除事件
            RemoveHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
            ShowInfo($"ERROR:{sysInfo.Language.GetLang("获取设备IP失败！请检查设备后，重新启动程序")}")
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
        ShowInfo(sysInfo.Language.GetLang("连接Nova服务中"))
        sysInfo.RootClass = New MarsHardwareEnumerator
        If Not sysInfo.RootClass.Initialize() Then
            ShowInfo($"ERROR:{sysInfo.Language.GetLang("连接Nova服务失败")}")
            Exit Sub
        End If
#End Region

#Region "查找控制系统"
        ShowInfo(sysInfo.Language.GetLang("查找控制系统中"))
        For i001 As Integer = 0 To 6 - 1
            If sysInfo.RootClass.CtrlSystemCount() < 1 Then
                If i001 = 5 Then
                    ShowInfo($"ERROR:{sysInfo.Language.GetLang("未找到控制系统")}")
                    Exit Sub
                End If

                Thread.Sleep(1000)
                Continue For
            End If

            Exit For
        Next
#End Region

        sysInfo.MainClass = New MarsControlSystem(sysInfo.RootClass)
        '绑定读取到ip事件
        AddHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        sysInfo.RootClass.GetComNameOfControlSystem(0, tmpstr)
        sysInfo.MainClass.Initialize(tmpstr, screenCount, senderCount)

        If senderCount = 0 Then
            ShowInfo($"ERROR:{sysInfo.Language.GetLang("未找到控制器")}")
            Exit Sub
        End If

        ShowInfo(sysInfo.Language.GetLang("读取显示屏信息中"))
        Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing
        If sysInfo.MainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
            ShowInfo($"ERROR:{sysInfo.Language.GetLang("读显示屏信息失败")}")
            Exit Sub
        End If

        If LEDScreenInfoList Is Nothing OrElse
            LEDScreenInfoList.Count = 0 Then
            ShowInfo($"ERROR:{sysInfo.Language.GetLang("未找到显示屏")}")
            Exit Sub
        End If

        ShowInfo(sysInfo.Language.GetLang("载入屏幕信息中"))
        sysInfo.ScanBoardTable = New Hashtable
        ReDim sysInfo.ScreenList(screenCount - 1)
        ReDim sysInfo.SenderList(senderCount - 1)
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            ReDim sysInfo.SenderList(i).TmpIpData(12 - 1)
        Next

#Region "遍历屏幕"
        '遍历屏幕
        For LEDScreenId As Integer = 0 To screenCount - 1
            With sysInfo.ScreenList(LEDScreenId)

                Dim x As Integer
                Dim y As Integer
                '获取起始位置 大小
                sysInfo.MainClass.GetScreenLocation(LEDScreenId,
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
                ReDim .ClickHistoryMap((.DefSize.Height \ .DefScanBoardSize.Height) * .SensorLayout.Height,
                                       (.DefSize.Width \ .DefScanBoardSize.Width) * .SensorLayout.Width)

#Region "存储接收卡信息"
                '存储接收卡信息
                .SenderList = New List(Of Integer)
                For Each i001 As ScanBoardMapRegion In LEDScreenInfoList(LEDScreenId).ScanBoardInfoList
                    '接收卡留空则不添加
                    If i001.SenderIndex = &HFF Then
                        Continue For
                    End If

                    sysInfo.ScreenList(LEDScreenId).SenderList.Add(i001.SenderIndex)

                    Dim tmpScanBoardInfo As New ScanBoardInfo With {
                        .ScreenId = LEDScreenId,'屏幕索引
                        .SenderId = i001.SenderIndex,'控制器索引
                        .PortId = i001.PortIndex,'网口索引
                        .ConnectId = i001.ConnectIndex'接收卡索引
                    }
                    '屏幕块位置
                    tmpScanBoardInfo.Location.X = (i001.X \ .DefScanBoardSize.Width) * .SensorLayout.Width
                    tmpScanBoardInfo.Location.Y = (i001.Y \ .DefScanBoardSize.Height) * .SensorLayout.Height

                    sysInfo.ScanBoardTable.Add($"{i001.SenderIndex}-{i001.PortIndex}-{i001.ConnectIndex}", tmpScanBoardInfo)
                Next
#End Region
            End With
        Next
#End Region

        sysInfo.MainClass.GetEquipmentIP(0)
    End Sub
#End Region
End Class