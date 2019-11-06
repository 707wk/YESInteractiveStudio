Imports System.ComponentModel
Imports Wangk.Resource

Public Class AccuracySettingsForm
    Private Sub AccuracySettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        TrackBar2.Value = AppSettingHelper.Settings.PositionaIAccuracy
        NumericUpDown1.Value = AppSettingHelper.Settings.ValidSensorMinimum

        ChangeControlsLanguage()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        AppSettingHelper.Settings.PositionaIAccuracy = TrackBar2.Value
        AppSettingHelper.Settings.ValidSensorMinimum = NumericUpDown1.Value

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

    Private Sub TrackBar2_ValueChanged(sender As Object, e As EventArgs) Handles TrackBar2.ValueChanged
        Label1.Text = TrackBar2.Value
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Button1.Text = .GetS("Save changes")
            Me.Button2.Text = .GetS("Cancel")
            Me.Label6.Text = .GetS("Low")
            Me.Label5.Text = .GetS("High")
            Me.Label4.Text = .GetS("PositionaI accuracy")
            Me.Label2.Text = .GetS("Valid Sensor Minimum")
            Me.Text = .GetS("AccuracySettingsForm")
        End With
    End Sub
#End Region

End Class