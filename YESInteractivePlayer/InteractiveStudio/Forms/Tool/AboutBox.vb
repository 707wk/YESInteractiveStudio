Public Class AboutBox
    Private Sub AboutBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = sysInfo.Language.GetLang("About")

        With My.Application.Info
            Label1.Text = $"V{ .Version.Major}.{ .Version.Minor}.{ .Version.Build}"
            Label2.Text = .Copyright
            Label3.Text = .CompanyName
            LinkLabel1.Text = "www.yes-led.com"
        End With
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("www.yes-led.com")
    End Sub
End Class