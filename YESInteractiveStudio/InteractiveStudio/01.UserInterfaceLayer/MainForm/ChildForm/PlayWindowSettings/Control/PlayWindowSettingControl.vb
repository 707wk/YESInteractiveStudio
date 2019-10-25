﻿Public Class PlayWindowSettingControl
    ''' <summary>
    ''' 播放窗口信息
    ''' </summary>
    Public DisplayingWindow As DisplayingWindow

    ''' <summary>
    ''' 播放文件列表
    ''' </summary>
    Public PlayFileItems As List(Of DisplayingFile)

    Private Sub PlayWindowSettingControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill

        TextBox1.Text = DisplayingWindow.Name
        NumericUpDown1.Value = DisplayingWindow.Location.X
        NumericUpDown2.Value = DisplayingWindow.Location.Y

        ComboBox1.Text = DisplayingWindow.Magnificine

        For Each itemID In DisplayingWindow.ScreenIDItems
            With AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(itemID)

                Dim tmpButton As New ScreenButton(itemID) With {
                    .ContextMenuStrip = ContextMenuStrip1
                }

                Panel2.Controls.Add(tmpButton)

            End With
        Next

        PlayFileItems = DisplayingWindow.PlayFileItems

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
