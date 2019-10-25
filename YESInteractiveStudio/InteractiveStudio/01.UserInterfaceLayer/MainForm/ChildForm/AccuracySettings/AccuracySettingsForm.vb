Imports System.ComponentModel

Public Class AccuracySettingsForm
    Private Sub AccuracySettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TrackBar2.Value = AppSettingHelper.Settings.PositionaIAccuracy
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        AppSettingHelper.Settings.PositionaIAccuracy = TrackBar2.Value

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar2.ValueChanged
        Label1.Text = TrackBar2.Value
    End Sub
End Class