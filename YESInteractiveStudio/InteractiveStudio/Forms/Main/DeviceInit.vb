Imports System.ComponentModel
Imports System.Threading
Imports Nova.Mars.SDK

Public Class DeviceInit
#Region "窗体初始化/关闭"
    Private Sub DeviceInit_Load(sender As Object, e As EventArgs) Handles Me.Load
        Timer1.Interval = 500

        ChangeControlsLanguage()
    End Sub

    Private Sub DeviceInit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Timer1.Start()
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

#Region "显示信息"
    ''' <summary>
    ''' 显示信息
    ''' </summary>
    Public Delegate Sub ShowInfoCallback(ByVal text As String, ExitFlag As Boolean)
    Public Sub ShowInfo(ByVal text As String, Optional ExitFlag As Boolean = False)
        If Me.InvokeRequired Then
            Me.Invoke(New ShowInfoCallback(AddressOf ShowInfo), New Object() {text, ExitFlag})
            Exit Sub
        End If

        ToolStripStatusLabel1.Text = text

        If ExitFlag Then
            MsgBox(text, MsgBoxStyle.Information, Me.Text)
            End
        End If

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
                AppSetting.NovaMarsControl.GetEquipmentIP(senderArrayId)
            Else
                ShowInfo(AppSetting.Language.GetS("Loading Completed"))
                '移除事件
                RemoveHandler AppSetting.NovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.Close(True)
            End If
        Else
            '移除事件
            RemoveHandler AppSetting.NovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
            ShowInfo(AppSetting.Language.GetS("Failed to Get IP data!Please check the equipment and reset it"), True)
        End If
    End Sub
#End Region

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Stop()

        '判断Nova服务是否启动
        ShowInfo(AppSetting.Language.GetS("Start Nova Serve"))
        If System.Diagnostics.Process.GetProcessesByName("MarsServerProvider").Length = 0 Then
            Process.Start($".\Nova\Server\MarsServerProvider.exe")
        End If

#Region "连接Nova服务"
        ShowInfo(AppSetting.Language.GetS("Connect Nova Serve ..."))

        AppSetting.NovaMarsHardware = New MarsHardwareEnumerator
        For i001 = 0 To 10 - 1
            ShowInfo($"{AppSetting.Language.GetS("Connect Nova Serve ...")}{"".PadRight(i001, ".")}")

            Thread.Sleep(500)

            If AppSetting.NovaMarsHardware.Initialize() Then
                If AppSetting.NovaMarsHardware.CommPortListOfCtrlSystem.Count > 0 Then
                    Exit For
                End If

                AppSetting.NovaMarsHardware.UnInitialize()
            End If

            If i001 = 10 - 1 Then
                ShowInfo(AppSetting.Language.GetS("Failed to Connect Nova Serve"), True)
                Exit Sub
            End If

        Next
#End Region

#Region "查找控制系统"
        For i001 As Integer = 0 To 5 - 1
            ShowInfo(AppSetting.Language.GetS("Searching Control System") & "".PadRight(i001, "."))

            If AppSetting.NovaMarsHardware.CtrlSystemCount() < 1 Then
                If i001 = 20 - 1 Then
                    ShowInfo(AppSetting.Language.GetS("No control system found"), True)
                    Exit Sub
                End If

                Thread.Sleep(1000)
                Continue For
            End If

            Exit For
        Next
#End Region

        AppSetting.NovaMarsControl = New MarsControlSystem(AppSetting.NovaMarsHardware)
        '绑定读取到ip事件
        AddHandler AppSetting.NovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        AppSetting.NovaMarsHardware.GetComNameOfControlSystem(0, tmpstr)
        AppSetting.NovaMarsControl.Initialize(tmpstr, screenCount, senderCount)

        If senderCount = 0 Then
            ShowInfo(AppSetting.Language.GetS("No controller found"), True)
            Exit Sub
        End If

        ShowInfo(AppSetting.Language.GetS("Reading display screen information"))
        Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing
        If AppSetting.NovaMarsControl.ReadLEDScreenInfo(LEDScreenInfoList) Then
            ShowInfo(AppSetting.Language.GetS("Failed to Reading display screen information"), True)
            Exit Sub
        End If

        If LEDScreenInfoList Is Nothing OrElse
            LEDScreenInfoList.Count = 0 Then
            ShowInfo(AppSetting.Language.GetS("No display screen found"), True)
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
                AppSetting.NovaMarsControl.GetScreenLocation(LEDScreenId,
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

        AppSetting.NovaMarsControl.GetEquipmentIP(0)

    End Sub

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