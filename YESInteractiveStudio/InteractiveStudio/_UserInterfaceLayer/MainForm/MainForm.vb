Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light

        Dim assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim versionStr = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion
        Me.Text = $"{My.Application.Info.ProductName} V{versionStr}"
#End Region

    End Sub

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
End Class