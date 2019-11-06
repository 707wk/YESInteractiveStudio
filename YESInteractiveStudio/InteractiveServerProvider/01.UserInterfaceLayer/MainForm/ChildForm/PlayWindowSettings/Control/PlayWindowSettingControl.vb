Public Class PlayWindowSettingControl

    Private Sub PlayWindowSettingControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill

        'NumericUpDown1.Value = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.Location.X
        'NumericUpDown2.Value = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.Location.Y

        ComboBox1.Text = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.Magnificine

        For Each itemID In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems
            With AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(itemID)

                Dim tmpButton As New ScreenButton(itemID) With {
                    .ContextMenuStrip = ContextMenuStrip1
                }

                Panel2.Controls.Add(tmpButton)

            End With
        Next

    End Sub

    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click
        Dim tmpActiveControl = Me.ActiveControl

        If tmpActiveControl.GetType.Name = NameOf(ScreenButton) Then

            Dim tmpScreenButton = CType(tmpActiveControl, ScreenButton)

            AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(tmpScreenButton.ScreenId).IsUsed = False

            Me.ActiveControl.Dispose()
        End If
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Using tmpDialog As New ScreenIDSelectForm
            If tmpDialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            For Each screenID In tmpDialog.SelectScreenIDItems
                Dim tmpButton As New ScreenButton(screenID) With {
                    .ContextMenuStrip = ContextMenuStrip1
                }

                Panel2.Controls.Add(tmpButton)
            Next

        End Using
    End Sub

    Friend Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click

        For Each item In Panel2.Controls
            Dim tmpScreenButton = CType(item, ScreenButton)

            AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(tmpScreenButton.ScreenId).IsUsed = False

        Next

        Panel2.Controls.Clear()
    End Sub
End Class
