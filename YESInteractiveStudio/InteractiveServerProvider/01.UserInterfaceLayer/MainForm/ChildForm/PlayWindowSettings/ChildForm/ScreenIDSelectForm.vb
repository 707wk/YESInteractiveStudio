Public Class ScreenIDSelectForm

    Public SelectScreenIDItems As New List(Of Integer)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        For Each item In CheckedListBox1.CheckedItems
            Dim screenID As Integer = Val(item.ToString.Split(" ")(1)) - 1

            SelectScreenIDItems.Add(screenID)
            AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(screenID).IsUsed = True
        Next

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub ScreenIDSelectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For itemID = 0 To AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems.Count - 1

            If AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(itemID).IsUsed Then
                Continue For
            End If

            CheckedListBox1.Items.Add($"Screen {itemID + 1}")
        Next

        CheckedListBox1.CheckOnClick = True

    End Sub
End Class