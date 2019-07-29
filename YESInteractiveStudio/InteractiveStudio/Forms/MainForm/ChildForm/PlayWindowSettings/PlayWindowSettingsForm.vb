Imports DevComponents.DotNetBar

Public Class PlayWindowSettingsForm
    ''' <summary>
    ''' 添加播放窗体按钮
    ''' </summary>
    Private AddWindowButton As ButtonItem

    Private Sub PlayWindowSettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#Region "添加窗体按钮"
        AddWindowButton = New ButtonItem()
        With AddWindowButton
            .ButtonStyle = eButtonStyle.ImageAndText
            .Text = "Add Play Window"
            .Image = My.Resources.add_24px
            .CustomColorName = "AddPlayWindow"

            AddHandler .Click, AddressOf AddTabClick

        End With
        SuperTabControl1.ControlBox.SubItems.Add(AddWindowButton)
#End Region

        If SuperTabControl1.Tabs.Count <= 0 Then
            AddTabClick(Nothing, Nothing)
        End If

    End Sub

#Region "添加窗体"
    Private Sub AddTabClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim s As String = $"Window {SuperTabControl1.Tabs.Count + 1}"
        Dim tab As SuperTabItem = SuperTabControl1.CreateTab(s)

        tab.AttachedControl.Controls.Add(New PlayWindowSettingControl)
    End Sub
#End Region

End Class