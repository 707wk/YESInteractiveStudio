Imports Wangk.Resource

Public Class ScreenIDSelectForm

    Public SelectScreenIDItems As New List(Of Integer)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        For Each item In CheckedListBox1.CheckedItems
            Dim screenID As Integer = Val(item.ToString.Split(" ")(1)) - 1

            SelectScreenIDItems.Add(screenID)
            AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems(screenID).IsUsed = True
        Next

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub ScreenIDSelectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For itemID = 0 To AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems.Count - 1

            If AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems(itemID).IsUsed Then
                Continue For
            End If

            CheckedListBox1.Items.Add($"Screen {itemID + 1}")
        Next

        CheckedListBox1.CheckOnClick = True

        ChangeControlsLanguage()

    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Button1.Text = .GetS("OK")
            Me.Button2.Text = .GetS("Cancel")
            Me.Text = .GetS("ScreenIDSelectForm")
        End With
    End Sub
#End Region

End Class