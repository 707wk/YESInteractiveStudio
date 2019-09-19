Imports System.ComponentModel

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '初始化配置
        AppSettingHelper.Settings.ToString()

#Region "重新启动Nova服务"
        Dim tmpProcess = System.Diagnostics.Process.GetProcessesByName("MarsServerProvider")
        If tmpProcess.Length > 0 Then
            tmpProcess(0).Kill()
        End If
        Process.Start($".\Nova\Server\MarsServerProvider.exe")
#End Region

#Region "样式设置"
        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light

        '产品版本号
        Dim assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim versionStr = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion
        Me.Text = $"{My.Application.Info.ProductName} V{versionStr}"

        Me.KeyPreview = True
        DebugCheckBox.Visible = False
        RibbonBar1.RecalcLayout()

        '切换语言
        ChangeControlsLanguage()
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

        '默认互动模式
        InteractCheckBox.Checked = True
#End Region

#Region "注册全局快捷键"
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        WindowsHotKeyHelper.RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)
#End Region

    End Sub

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
#Region "注销全局快捷键"
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        WindowsHotKeyHelper.UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)
#End Region

        AppSettingHelper.SaveToLocaltion()
    End Sub

#Region "显示模式切换处理"
    Private Sub DisplayModeChange(sender As Object, e As EventArgs)
        Dim tmpCheckBoxItem = CType(sender, DevComponents.DotNetBar.CheckBoxItem)

        If Not tmpCheckBoxItem.Checked Then
            Exit Sub
        End If

        AppSettingHelper.Settings.DisplayMode = tmpCheckBoxItem.Tag
    End Sub
#End Region

#Region "按键消息处理"
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WindowsHotKeyHelper.WM_HOTKEY Then
            Select Case m.WParam.ToInt32
                Case 1
                    InteractCheckBox.Checked = True
                Case 2
                    TestCheckBox.Checked = True
                Case 3
                    BlackCheckBox.Checked = True
                Case 4
                    If Not DebugCheckBox.Visible Then
                        Exit Select
                    End If

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
        Dim tmpDialog As New ReadScreenInformationForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "播放窗体设置"
    Private Sub PlayWindowSettingsButton_Click(sender As Object, e As EventArgs) Handles PlayWindowSettingsButton.Click
        Dim tmpDialog As New PlayWindowSettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "控制器设置"
    Private Sub ControlButton_Click(sender As Object, e As EventArgs) Handles ControlButton.Click
        Dim tmpDialog As New ControlSettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "传感器设置"
    Private Sub SensorButton_Click(sender As Object, e As EventArgs) Handles SensorButton.Click
        Dim tmpDialog As New SensorSettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "精度设置"
    Private Sub AccuracyButton_Click(sender As Object, e As EventArgs) Handles AccuracyButton.Click
        Dim tmpDialog As New AccuracySettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "单片机设置"
    Private Sub MCUButton_Click(sender As Object, e As EventArgs) Handles MCUButton.Click
        Dim tmpDialog As New MCUSettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With Wangk.Resource.MultiLanguageHelper.Lang
            Me.Text = .GetS("Temp Change Over")
        End With
    End Sub
#End Region

End Class