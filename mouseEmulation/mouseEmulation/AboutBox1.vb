Public NotInheritable Class AboutBox1

    Private Sub AboutBox1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' 设置此窗体的标题。
        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Me.Text = $"{If(selectLanguageId = 0, "关于", "About")}"
        ' 初始化“关于”对话框显示的所有文字。
        ' TODO: 在项目的“应用程序”窗格中自定义此应用程序的程序集信息 
        '    属性对话框(在“项目”菜单下)。
        Me.LabelProductName.Text = My.Application.Info.Title
        Me.LabelVersion.Text = $"{If(selectLanguageId = 0, "版本", "Versions")} " & My.Application.Info.Version.ToString
        Me.LabelCopyright.Text = My.Application.Info.Copyright
        Me.LabelCompanyName.Text = My.Application.Info.CompanyName
        Me.TextBoxDescription.Text = My.Application.Info.Description
        Me.TextBoxDescription.Text = $"由 湖南新亚胜光电股份有限公司 研发" '{vbCrLf}与 湖南师范大学工程与设计学院 联合

        changeLanguage()
    End Sub

    Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
        Me.Close()
    End Sub

    ''' <summary>
    ''' 更改语言 0:中文 1:English
    ''' </summary>
    Private Sub changeLanguage()
        Me.OKButton.Text = $"{If(selectLanguageId = 0, "确定", "Ok")}(&O)"
    End Sub
End Class
