Imports System.ComponentModel
Imports Nova.Mars.SDK
Imports Wangk.Resource

Public Class SerialPortSelectForm

    ''' <summary>
    ''' Nova连接变量
    ''' </summary>
    Private ReadOnly NovaMarsHardware As New MarsHardwareEnumerator
    ''' <summary>
    ''' Nova配置变量
    ''' </summary>
    Public NovaMarsControl As MarsControlSystem
    ''' <summary>
    ''' 选择的串口号
    ''' </summary>
    Public SelectedSerialPort As String

    Private Sub SerialPortSelectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        With ComboBox1
            .DropDownStyle = ComboBoxStyle.DropDownList
            .Sorted = True
        End With

        ConnectButton.Enabled = False
        Label2.BringToFront()

        ChangeControlsLanguage()

        Timer1.Interval = 2000

    End Sub

    Private Sub SerialPortSelectForm_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed

        NovaMarsControl?.UnInitialize()
        NovaMarsHardware?.UnInitialize()

    End Sub

    Private Sub SerialPortSelectForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Label2.Refresh()

        Timer1.Start()

        Label2.Hide()

    End Sub

    Private Async Sub SerialPortSelectForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Timer1.Stop()

        Await Task.Run(Sub()
                           Do While RefreshButton.Enabled <> True
                               Threading.Thread.Sleep(100)
                           Loop
                       End Sub)

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        RefreshButton_MouseUp(Nothing, Nothing)

        If ComboBox1.Items.Count > 0 Then
            Timer1.Stop()
        End If

    End Sub

    Private Sub RefreshButton_MouseUp(sender As Object, e As MouseEventArgs) Handles RefreshButton.MouseUp
        ComboBox1.Items.Clear()
        ConnectButton.Enabled = False
        RefreshButton.Enabled = False
        Try
            NovaMarsHardware?.UnInitialize()
            NovaMarsHardware?.Initialize()

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
        End Try

        For Each i001 As String In NovaMarsHardware.CommPortListOfCtrlSystem
            ComboBox1.Items.Add(i001)
        Next

        If ComboBox1.Items.Count > 0 Then
            ComboBox1.SelectedIndex = 0
            ConnectButton.Enabled = True
        End If

        RefreshButton.Enabled = True
    End Sub

    Private Sub ConnectButton_Click(sender As Object, e As EventArgs) Handles ConnectButton.Click

        '初始化控制系统
        NovaMarsControl = New MarsControlSystem(NovaMarsHardware)
        SelectedSerialPort = ComboBox1.Text

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