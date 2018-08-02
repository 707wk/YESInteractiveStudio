Public Class AboutBox
    Private Sub AboutBox_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        Me.Text = sysInfo.Language.GetS("About")

        With My.Application.Info
            Label1.Text = $"V{ .Version.Major}.{ .Version.Minor}.{ .Version.Build}"
            Label2.Text = .Copyright
            Label3.Text = .CompanyName
            LinkLabel1.Text = "www.yes-led.com"
        End With

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
#End Region
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Process.Start("www.yes-led.com")
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.Label1.Text = .GetS("Version")
            Me.Label2.Text = .GetS("Copyright")
            Me.Label3.Text = .GetS("CompanyName")
            Me.LinkLabel1.Text = .GetS("WebUrl")
        End With
    End Sub
#End Region
End Class