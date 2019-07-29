Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light
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
End Class