Imports System.ComponentModel
Imports System.Threading
Imports DevComponents.DotNetBar
Imports Nova.LCT.GigabitSystem.Common
Imports Nova.Mars.SDK
Imports Wangk.Resource

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'AppSettingHelper.GetInstance.Logger.Info("程序启动")

#Region "重新启动Nova服务"
        'Try
        '    ''todo:重新启动Nova服务
        '    Dim tmpProcess = System.Diagnostics.Process.GetProcessesByName("MarsServerProvider")
        '    If tmpProcess.Length > 0 Then
        '        tmpProcess(0).Kill()
        '    End If
        '    Process.Start($".\Nova\Server\MarsServerProvider.exe")

        'Catch ex As Exception
        'End Try
#End Region

        '初始化配置
        AppSettingHelper.GetInstance.ToString()
        HttpServerHelper.UIMainForm = Me
        UIFormHelper.UIForm = Me

#Region "样式设置"
        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light

        '产品版本号
        Me.Text = $"{My.Application.Info.ProductName} V{AppSettingHelper.GetInstance.ProductVersion}"

        Me.KeyPreview = True

        DebugCheckBox.Visible = False

        LanguageComboBox.DropDownStyle = ComboBoxStyle.DropDownList
        For Each LANGStr In [Enum].GetNames(GetType(Wangk.Resource.MultiLanguage.LANG))
            LanguageComboBox.Items.Add(LANGStr)
        Next
        LanguageComboBox.SelectedIndex = AppSettingHelper.GetInstance.SelectLang

        ChangeControlsLanguage()

        RibbonBar1.Width = 256
        RibbonBar3.Width = 256
#End Region

#Region "显示模式切换"
        InteractCheckBox.Tag = InteractiveOptions.DISPLAYMODE.INTERACT
        TestCheckBox.Tag = InteractiveOptions.DISPLAYMODE.TEST
        BlackCheckBox.Tag = InteractiveOptions.DISPLAYMODE.BLACK
        DebugCheckBox.Tag = InteractiveOptions.DISPLAYMODE.DEBUG

        AddHandler InteractCheckBox.CheckedChanged, AddressOf DisplayModeChange
        AddHandler TestCheckBox.CheckedChanged, AddressOf DisplayModeChange
        AddHandler BlackCheckBox.CheckedChanged, AddressOf DisplayModeChange
        AddHandler DebugCheckBox.CheckedChanged, AddressOf DisplayModeChange
#End Region

#Region "注册全局快捷键"
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)
#End Region

        AutoRunCheckBox.Checked = AppSettingHelper.GetInstance.IsAutoRun

        'Wangk.Tools.ConsoleDebug.Open()
    End Sub

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        ShowNovaStarSenderList()

#Region "显示播放窗口/连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              '计算传感器位置
                                              DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLSensor()

                                              DisplayingSchemeProcessingHelper.ShowFormForALLDisplayingWindow()
                                              SensorDataProcessingHelper.StartAsync()

                                          End Sub)

        End Using
        Me.Activate()
#End Region

        ShowDisplayingProgram()

        '默认互动模式
        InteractCheckBox.Checked = True

        '更新接收卡状态
        Timer1.Interval = 2000
        Timer1.Start()

        '自启后最小化
        If AppSettingHelper.GetInstance.IsAutoRun Then
            Me.WindowState = FormWindowState.Minimized
        End If

    End Sub

#Region "显示节目列表"
    ''' <summary>
    ''' 显示节目列表
    ''' </summary>
    Private Sub ShowDisplayingProgram()
        TabControl1.TabPages.Clear()

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems
                Dim addTabPage = New TabPage(tmpDisplayingWindow.Name)
                addTabPage.Controls.Add(New WindowProgramControl With {
                                        .Visible = True,
                                        .DisplayingWindow = tmpDisplayingWindow
                                        })
                TabControl1.TabPages.Add(addTabPage)
            Next

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub
#End Region

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
#Region "注销全局快捷键"
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)
#End Region

        SensorDataProcessingHelper.StopAsync()
        DisplayingSchemeProcessingHelper.CloseFormForALLDisplayingWindow()

        AppSettingHelper.SaveToLocaltion()
    End Sub

#Region "显示模式切换处理"
    Private Sub DisplayModeChange(sender As Object, e As EventArgs)
        Dim tmpCheckBoxItem = CType(sender, DevComponents.DotNetBar.CheckBoxItem)

        If Not tmpCheckBoxItem.Checked Then
            Exit Sub
        End If

        AppSettingHelper.GetInstance.DisplayMode = tmpCheckBoxItem.Tag
        DisplayingSchemeProcessingHelper.ChangeDisplayModeForALLDisplayingWindow()

    End Sub
#End Region

#Region "按键消息处理"
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WindowsHotKeyHelper.WM_HOTKEY Then
            Select Case m.WParam.ToInt32
                Case 1
                    InteractCheckBox.Checked = True
                Case 2
                    BlackCheckBox.Checked = True
                    TestCheckBox.Checked = True
                Case 3
                    BlackCheckBox.Checked = True
                Case 4
                    If Not DebugCheckBox.Visible Then
                        Exit Select
                    End If

                    BlackCheckBox.Checked = True
                    DebugCheckBox.Checked = True
            End Select
        End If

        MyBase.WndProc(m)
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Static password As String = Nothing

        If DebugCheckBox.Visible Then
            Exit Sub
        End If

        password &= Convert.ToChar(e.KeyValue)

        If password.Length > 128 Then
            password = Microsoft.VisualBasic.Right(password, 32)
        End If

        If password.IndexOf("YESTECH") > -1 Then
            DebugCheckBox.Visible = True
            RibbonBar1.Width *= 1.33

        End If

    End Sub
#End Region

#Region "读取屏幕信息"
    Private Sub ReadScreenInformationButton_Click(sender As Object, e As EventArgs) Handles ReadScreenInformationButton.Click

        Dim tmpNovaStarScreenItems() As NovaStarScreen = Nothing
        Dim tmpNovaStarSenderItems() As NovaStarSender = Nothing

        Using tmpSerialPortSelectForm As New SerialPortSelectForm
            If tmpSerialPortSelectForm.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            '开始读取
            Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                .Text = MultiLanguageHelper.Lang.GetS("Reading screen information")
            }

                Dim tmpNovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl

                tmpBackgroundWorkDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
#Region "初始化"
                                                  Dim screenCount As Integer
                                                  Dim senderCount As Integer
                                                  tmpNovaMarsControl.Initialize(tmpSerialPortSelectForm.SelectedSerialPort, screenCount, senderCount)

                                                  If screenCount = 0 Then Throw New Exception(MultiLanguageHelper.Lang.GetS("No Screen found"))
                                                  If senderCount = 0 Then Throw New Exception(MultiLanguageHelper.Lang.GetS("No Sender found"))

                                                  ReDim tmpNovaStarScreenItems(screenCount - 1)
                                                  ReDim tmpNovaStarSenderItems(senderCount - 1)
                                                  For itemID = 0 To tmpNovaStarSenderItems.Count - 1
                                                      tmpNovaStarSenderItems(itemID) = New NovaStarSender
                                                  Next
#End Region

#Region "读取屏幕信息"
                                                  '读取屏幕信息
                                                  Dim tmpLEDScreenInfoList As List(Of Nova.Mars.SDK.LEDScreenInfo) = Nothing
                                                  If tmpNovaMarsControl.ReadLEDScreenInfo(tmpLEDScreenInfoList) <>
                                                  Nova.Mars.SDK.OperateResult.OK Then
                                                      Throw New Exception(MultiLanguageHelper.Lang.GetS("Failed to reading screen information"))
                                                  End If
#End Region

#Region "读取热备份信息"
                                                  Dim hotBackUpItems As List(Of SenderRedundancyInfo) = Nothing
                                                  '读取热备份信息
                                                  Using GetHotBackUpEvent As New AutoResetEvent(False)
                                                      tmpNovaMarsControl.GetHotBackUp(Sub(res As HotBackUpState, info As List(Of SenderRedundancyInfo))
                                                                                          If res = HotBackUpState.Successful Then
                                                                                              hotBackUpItems = info
                                                                                              'Throw New Exception($"{MultiLanguageHelper.Lang.GetS("Fail to Get HotBackUp")}")
                                                                                          End If

                                                                                          GetHotBackUpEvent.Set()
                                                                                      End Sub)
                                                      GetHotBackUpEvent.WaitOne()
                                                  End Using

                                                  If hotBackUpItems IsNot Nothing Then

                                                      '记录冗余信息
                                                      For Each item In hotBackUpItems
                                                          tmpNovaStarSenderItems(item.MasterSenderIndex).
                                                          HotBackUpSenderPortItems.
                                                          Add(item.MasterPortIndex,
                                                              New NovaStartSenderRedundancyInfo(item.MasterSenderIndex,
                                                                                                item.MasterPortIndex,
                                                                                                item.SlaveSenderIndex,
                                                                                                item.SlavePortIndex))
                                                      Next

                                                  End If

#End Region

#Region "遍历屏幕"
                                                  For itemID = 0 To tmpNovaStarScreenItems.Count - 1
                                                      tmpNovaStarScreenItems(itemID) = New NovaStarScreen

                                                      With tmpNovaStarScreenItems(itemID)
                                                          tmpNovaMarsControl.GetScreenLocation(itemID,
                                                                                               .LocationOfOriginal.X,
                                                                                               .LocationOfOriginal.Y,
                                                                                               .SizeOfOriginal.Width,
                                                                                               .SizeOfOriginal.Height)


#Region "读取接收卡信息"
                                                          For Each tmpScanBoard In tmpLEDScreenInfoList(itemID).ScanBoardInfoList
                                                              '接收卡留空则不添加
                                                              If tmpScanBoard.SenderIndex = &HFF Then Continue For

                                                              '连接位置
                                                              Dim tmpNovaStarScanBoard As New NovaStarScanBoard
                                                              With tmpNovaStarScanBoard
                                                                  .SenderID = tmpScanBoard.SenderIndex
                                                                  .PortID = tmpScanBoard.PortIndex
                                                                  .ConnectID = tmpScanBoard.ConnectIndex
                                                                  .LocationOfOriginal.X = tmpScanBoard.X
                                                                  .LocationOfOriginal.Y = tmpScanBoard.Y
                                                                  .SizeOfOriginal.Width = tmpScanBoard.Width
                                                                  .SizeOfOriginal.Height = tmpScanBoard.Height
                                                              End With

                                                              '箱体旋转角度
                                                              tmpNovaMarsControl.ReadCabinetRotateAngle(tmpNovaStarScanBoard.SenderID,
                                                                                                        tmpNovaStarScanBoard.PortID,
                                                                                                        tmpNovaStarScanBoard.ConnectID,
                                                                                                        tmpNovaStarScanBoard.BoxRotateAngle)

                                                              .NovaStarScanBoardItems.Add(tmpNovaStarScanBoard)

                                                          Next
#End Region

                                                      End With

                                                  Next
#End Region

#Region "读取发送卡IP"
                                                  AddHandler tmpNovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

                                                  For itemID = 0 To tmpNovaStarSenderItems.Count - 1
                                                      'tmpNovaStarSenderItems(itemID) = New NovaStarSender

                                                      SenderIPData = Nothing

                                                      tmpNovaMarsControl.GetEquipmentIP(itemID)
                                                      GetEquipmentIPDataEvent.WaitOne()

                                                      If SenderIPData Is Nothing Then
                                                          Throw New Exception($"{MultiLanguageHelper.Lang.GetS("Sender")} {itemID} {MultiLanguageHelper.Lang.GetS("no support for interactive")}")
                                                      End If

                                                      tmpNovaStarSenderItems(itemID).IpData = SenderIPData

                                                  Next

                                                  RemoveHandler tmpNovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
#End Region

                                              End Sub)

                If tmpBackgroundWorkDialog.Error IsNot Nothing Then
                    MsgBox(tmpBackgroundWorkDialog.Error.Message,
                           MsgBoxStyle.Information,
                           tmpBackgroundWorkDialog.Text)
                    Exit Sub
                End If

            End Using
        End Using

#Region "清空旧播放窗体/发送卡列表/屏幕列表"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Clear old screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                              DisplayingSchemeProcessingHelper.CloseFormForALLDisplayingWindow()
                                              AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems.Clear()
                                          End Sub)

        End Using
#End Region

        '复制新配置
        AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems = tmpNovaStarScreenItems
        AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems = tmpNovaStarSenderItems

        ''显示硬件设置窗体
        'Using tmpHardwareSettingsForm As New HardwareSettingsForm With {
        '    .NovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl
        '}
        '    tmpHardwareSettingsForm.ShowDialog()
        'End Using

        '显示播放窗口编辑窗体
        Using tmpPlayWindowSettingsForm As New PlayWindowSettingsForm With {
            .IsMustSave = True
        }
            tmpPlayWindowSettingsForm.ShowDialog()
        End Using

        ShowNovaStarSenderList()

#Region "显示播放窗口/连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              '计算传感器位置
                                              DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLScreenAndScanBoard()
                                              DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLSensor()
                                              DisplayingSchemeProcessingHelper.ComputeSizeForALLDisplayingWindow()

                                              DisplayingSchemeProcessingHelper.ShowFormForALLDisplayingWindow()
                                              SensorDataProcessingHelper.StartAsync()
                                          End Sub)

        End Using
#End Region

        ShowDisplayingProgram()

        '默认互动模式
        InteractCheckBox.Checked = True

    End Sub

#Region "获取发送卡IP"
    ''' <summary>
    ''' 读取到IP标志
    ''' </summary>
    Private ReadOnly GetEquipmentIPDataEvent As New AutoResetEvent(False)
    ''' <summary>
    ''' 读到的发送卡IP
    ''' </summary>
    Private SenderIPData As Byte()

    ''' <summary>
    ''' 获取IP通知
    ''' </summary>
    Private Sub GetEquipmentIPData(sender As Object, e As Nova.Mars.SDK.MarsEquipmentIPEventArgs)
        Static Dim senderArrayId As Integer = 0

        If e.IsExecResult Then
            SenderIPData = e.Data
        End If

        GetEquipmentIPDataEvent.Set()
    End Sub
#End Region

#End Region

#Region "播放窗口设置"
    Private Sub PlayWindowSettingsButton_Click(sender As Object, e As EventArgs) Handles PlayWindowSettingsButton.Click
        If AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems Is Nothing Then
            Exit Sub
        End If

#Region "清空旧播放窗体"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                              DisplayingSchemeProcessingHelper.CloseFormForALLDisplayingWindow()
                                          End Sub)

        End Using
#End Region

        Dim tmpDialog As New PlayWindowSettingsForm
        Dim isChanged As Boolean = tmpDialog.ShowDialog() = DialogResult.OK

#Region "显示播放窗口/连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              If isChanged Then
                                                  '计算传感器位置
                                                  DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLScreenAndScanBoard()
                                                  DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLSensor()
                                                  DisplayingSchemeProcessingHelper.ComputeSizeForALLDisplayingWindow()
                                              End If

                                              DisplayingSchemeProcessingHelper.ShowFormForALLDisplayingWindow()
                                              SensorDataProcessingHelper.StartAsync()
                                          End Sub)

        End Using
        Me.Activate()
#End Region

        ShowDisplayingProgram()

    End Sub
#End Region

#Region "精度设置"
    Private Sub AccuracyButton_Click(sender As Object, e As EventArgs) Handles AccuracyButton.Click
        Dim tmpDialog As New AccuracySettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "硬件设置"
    Private Sub HardwareButton_Click(sender As Object, e As EventArgs) Handles HardwareButton.Click
#Region "停止互动"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                          End Sub)

        End Using
#End Region

        ''todo:选择串口
        Dim tmpSerialPortSelectForm As New SerialPortSelectForm
        If tmpSerialPortSelectForm.ShowDialog() = DialogResult.OK Then

            '开始读取
            Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Reading screen information")
                }

                Dim tmpNovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl
                Dim screenCount As Integer
                Dim senderCount As Integer

                tmpBackgroundWorkDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
#Region "初始化"
                                                  tmpNovaMarsControl.Initialize(tmpSerialPortSelectForm.SelectedSerialPort, screenCount, senderCount)

                                                  If screenCount = 0 Then Throw New Exception(MultiLanguageHelper.Lang.GetS("No Screen found"))
                                                  If senderCount = 0 Then Throw New Exception(MultiLanguageHelper.Lang.GetS("No Sender found"))
#End Region
                                              End Sub)

                If tmpBackgroundWorkDialog.Error Is Nothing Then
                    Dim tmpDialog As New HardwareSettingsForm
                    With tmpDialog
                        .NovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl
                        ReDim .NovaStarSenderItems(senderCount - 1)
                    End With

                    tmpDialog.ShowDialog()
                Else
                    MsgBox(tmpBackgroundWorkDialog.Error.Message,
                               MsgBoxStyle.Information,
                               tmpBackgroundWorkDialog.Text)
                End If

            End Using

        End If

#Region "开始互动"
        Using tmpDialog As New Wangk.Resource.UIWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Dispose screen information")
        }

            tmpDialog.Start(Sub()
                                Try
                                    tmpSerialPortSelectForm.Dispose()
#Disable Warning CA1031 ' Do not catch general exception types
                                Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
                                End Try

                                SensorDataProcessingHelper.StartAsync()
                            End Sub)

        End Using
#End Region

    End Sub
#End Region

#Region "开机自启"
    Private Sub AutoRunCheckBox_CheckedChanged(sender As Object, e As CheckBoxChangeEventArgs) Handles AutoRunCheckBox.CheckedChanged

        If AppSettingHelper.GetInstance.IsAutoRun = AutoRunCheckBox.Checked Then
            Exit Sub
        End If

        Try
            If AutoRunCheckBox.Checked Then

                Dim shortcutPath As String = $"{System.Environment.GetFolderPath(Environment.SpecialFolder.Startup) }\{My.Application.Info.ProductName}.lnk"
                Dim tmpWshShell = New IWshRuntimeLibrary.WshShell()
                Dim tmpIWshShortcut As IWshRuntimeLibrary.IWshShortcut = tmpWshShell.CreateShortcut(shortcutPath)
                With tmpIWshShortcut
                    .TargetPath = Application.ExecutablePath
                    .WorkingDirectory = IO.Path.GetDirectoryName(Application.ExecutablePath)
                    .WindowStyle = 1
                    .Description = My.Application.Info.ProductName
                    .IconLocation = .TargetPath
                    .Save()
                End With

            Else
                Dim shortcutPath As String = $"{System.Environment.GetFolderPath(Environment.SpecialFolder.Startup) }\{My.Application.Info.ProductName}.lnk"
                Try
                    IO.File.Delete(shortcutPath)
#Disable Warning CA1031 ' Do not catch general exception types
                Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
                End Try

            End If

            AppSettingHelper.GetInstance.IsAutoRun = AutoRunCheckBox.Checked
            AppSettingHelper.SaveToLocaltion()

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
            MsgBox(ex.ToString,
                   MsgBoxStyle.Information,
                   MultiLanguageHelper.Lang.GetS("Setup failed"))
#Enable Warning CA1031 ' Do not catch general exception types
        End Try

    End Sub
#End Region

#Region "切换语言"
    Private Sub LanguageComboBox_SelectedIndexChanged(sender As Object, e As EventArgs) Handles LanguageComboBox.SelectedIndexChanged
        Static oldValue = AppSettingHelper.GetInstance.SelectLang

        AppSettingHelper.GetInstance.SelectLang = LanguageComboBox.SelectedIndex

        If oldValue <> AppSettingHelper.GetInstance.SelectLang Then
            MsgBox(MultiLanguageHelper.Lang.GetS("Restart the program to enable the language changes to take effect"),
                   MsgBoxStyle.Information,
                   MultiLanguageHelper.Lang.GetS("Change language"))
        End If

        AppSettingHelper.SaveToLocaltion()
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.LabelItem3.Text = .GetS("Hide windows")
            Me.HideWindowsCheckBox.Text = .GetS("Hide windows")
            Me.InteractCheckBox.Text = .GetS("Interact") & "(F1)"
            Me.TestCheckBox.Text = .GetS("Test") & "(F2)"
            Me.BlackCheckBox.Text = .GetS("Black") & "(F3)"
            Me.DebugCheckBox.Text = .GetS("Debug") & "(F4)"
            Me.ReadScreenInformationButton.Text = .GetS("Read screen information")
            Me.PlayWindowSettingsButton.Text = .GetS("Play window settings")
            Me.LabelItem1.Text = .GetS("Language")
            Me.LabelItem2.Text = .GetS("Auto run")
            Me.AutoRunCheckBox.Text = .GetS("AutoRun")
            Me.AccuracyButton.Text = .GetS("Accuracy settings")
            Me.HardwareButton.Text = .GetS("Hardware settings")
            Me.MobileControlButton.Text = .GetS("Mobile control")
            Me.StartTab.Text = .GetS("Start")
            Me.SettingsTab.Text = .GetS("Settings")
            Me.ToolStripDropDownButton1.Text = .GetS("Connection abnormality")
            Me.FeedbackButton.Text = .GetS("Feedback")
        End With
    End Sub
#End Region

#Region "隐藏窗体"
    Private Sub HideWindowsCheckBox_CheckedChanged(sender As Object, e As CheckBoxChangeEventArgs) Handles HideWindowsCheckBox.CheckedChanged
        DisplayingSchemeProcessingHelper.HideFormForALLDisplayingWindow(HideWindowsCheckBox.Checked)
    End Sub
#End Region

#Region "显示接收卡列表"
    ''' <summary>
    ''' 显示接收卡列表
    ''' </summary>
    Private Sub ShowNovaStarSenderList()
        ToolStripDropDownButton1.DropDownItems.Clear()

        If AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems Is Nothing Then
            Exit Sub
        End If

        For Each tmpNovaStarSender In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
            ToolStripDropDownButton1.DropDownItems.Add(New ToolStripMenuItem(
                                                       $"{MultiLanguageHelper.Lang.GetS("Sender")}:{tmpNovaStarSender.IPAddress}",
                                                       My.Resources.usb_disconnect_32px)
                                                       )
        Next

    End Sub

    ''' <summary>
    ''' 显示接收卡状态
    ''' </summary>
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim stateOfOK As Boolean = True

        Try
            If AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems Is Nothing Then
                Exit Sub
            End If

            For NovaStarSenderID = 0 To AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems.Count - 1
                Dim tmpNovaStarSender = AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems(NovaStarSenderID)
                If tmpNovaStarSender.State = InteractiveOptions.SenderConnectState.OffLine Then
                    stateOfOK = False
                    ToolStripDropDownButton1.DropDownItems(NovaStarSenderID).Image = My.Resources.usb_disconnect_32px

                Else
                    ToolStripDropDownButton1.DropDownItems(NovaStarSenderID).Image = My.Resources.usb_connect_32px

                End If
            Next

            If stateOfOK Then
                ToolStripDropDownButton1.Image = InteractiveStudio.My.Resources.Resources.usb_connect_32px
                ToolStripDropDownButton1.Text = MultiLanguageHelper.Lang.GetS("Normal Connection")

            Else
                ToolStripDropDownButton1.Image = InteractiveStudio.My.Resources.Resources.usb_disconnect_32px
                ToolStripDropDownButton1.Text = MultiLanguageHelper.Lang.GetS("Connection abnormality")

            End If

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
        End Try

    End Sub

#End Region

#Region "手机控制"
    Private Sub MobileControlButton_Click(sender As Object, e As EventArgs) Handles MobileControlButton.Click
        Dim tmpDialog As New MobileControlForm
        tmpDialog.ShowDialog()
    End Sub

#Region "远程播放文件"
    Public Delegate Sub RemotePlayFileCallback(windowID As Integer, fileID As Integer)
    ''' <summary>
    ''' 远程播放文件
    ''' </summary>
    Friend Sub RemotePlayFile(windowID As Integer, fileID As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New RemotePlayFileCallback(AddressOf RemotePlayFile), New Object() {windowID, fileID})
            Exit Sub
        End If

        If AppSettingHelper.GetInstance.DisplayMode <> InteractiveOptions.DISPLAYMODE.INTERACT Then
            Exit Sub
        End If

        With AppSettingHelper.GetInstance.DisplayingScheme
            If windowID >= .DisplayingWindowItems.Count Then
                Exit Sub
            End If
            If fileID >= .DisplayingWindowItems(windowID).PlayFileItems.Count Then
                Exit Sub
            End If

            Dim tmpWindowProgramControl = CType(TabControl1.TabPages(windowID).Controls(0), WindowProgramControl)
            With tmpWindowProgramControl
                .ToolStripButton2_Click(Nothing, Nothing)
                .RemotePlayFile(fileID)
            End With
        End With

    End Sub

#End Region
#End Region

    Private Sub FeedbackButton_Click(sender As Object, e As EventArgs) Handles FeedbackButton.Click
        Process.Start("https://support.qq.com/products/285331")
    End Sub

    Private Sub MainForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        GetEquipmentIPDataEvent.Dispose()
    End Sub
End Class