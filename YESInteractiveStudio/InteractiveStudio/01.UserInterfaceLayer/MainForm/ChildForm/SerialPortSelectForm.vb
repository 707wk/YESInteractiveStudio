Imports Nova.Mars.SDK
Imports Wangk.Resource

Public Class SerialPortSelectForm

    ''' <summary>
    ''' Nova连接变量
    ''' </summary>
    Private NovaMarsHardware As New MarsHardwareEnumerator
    ''' <summary>
    ''' Nova配置变量
    ''' </summary>
    Public NovaMarsControl As MarsControlSystem
    ''' <summary>
    ''' 选择的串口号
    ''' </summary>
    Public selectedSerialPort As String

    Private Sub SerialPortSelectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With ComboBox1
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
        End With

        ConnectButton.Enabled = False
        Label2.BringToFront()

#Region "重新启动Nova服务"
        Try
            ''todo:重新启动Nova服务
            Dim tmpProcess = System.Diagnostics.Process.GetProcessesByName("MarsServerProvider")
            If tmpProcess.Length > 0 Then
                tmpProcess(0).Kill()
            End If
            Process.Start($".\Nova\Server\MarsServerProvider.exe")
        Catch ex As Exception
        End Try
#End Region

        ChangeControlsLanguage()

    End Sub

    Private Sub SerialPortSelectForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        NovaMarsControl?.UnInitialize()
        NovaMarsHardware?.UnInitialize()
    End Sub

    Private Sub SerialPortSelectForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Label2.Refresh()

        RefreshButton_Click()

        Label2.Hide()
    End Sub

    Private Sub RefreshButton_Click(Optional sender As Object = Nothing, Optional e As EventArgs = Nothing) Handles RefreshButton.Click

        ComboBox1.Items.Clear()
        ConnectButton.Enabled = False

        Try
            NovaMarsHardware?.UnInitialize()
            NovaMarsHardware?.Initialize()

        Catch ex As Exception
        End Try

        For Each i001 As String In NovaMarsHardware.CommPortListOfCtrlSystem
            ComboBox1.Items.Add(i001)
        Next

        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 0
            ConnectButton.Enabled = True
        End If
    End Sub

    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click

        '初始化控制系统
        NovaMarsControl = New MarsControlSystem(NovaMarsHardware)
        selectedSerialPort = ComboBox1.Text

        Me.DialogResult = DialogResult.OK

        Me.Close()
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Text = .GetS("SerialPortSelectForm")
            Me.ConnectButton.Text = .GetS("Connect to Control")
            Me.Label1.Text = .GetS("Serial Port")
            Me.Label2.Text = .GetS("Waiting...")
        End With
    End Sub
#End Region

End Class