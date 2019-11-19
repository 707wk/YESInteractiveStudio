Imports System.ComponentModel

Public Class PlayWindowSettingsForm

    ''' <summary>
    ''' 是否必须保存
    ''' </summary>
    Public IsMustSave As Boolean
    ''' <summary>
    ''' 是否已保存
    ''' </summary>
    Private IsSave As Boolean = False

    Private Sub PlayWindowSettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        '设置使用标志
        For Each item In AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems
            item.IsUsed = False
        Next

        For Each screenID In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems
            AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(screenID).IsUsed = True
        Next

    End Sub

    Private Sub PlayWindowSettingsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Me.Panel1.Controls.Add(New PlayWindowSettingControl)

    End Sub

    Private Sub PlayWindowSettingsForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If IsMustSave AndAlso Not IsSave Then

            e.Cancel = True
            Exit Sub
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not IsWindowSettingDataCheckoutOk() Then
            Exit Sub
        End If

        Dim tmpPlayWindowSettingControl = CType(Panel1.Controls.Item(0), PlayWindowSettingControl)

        Dim tmpDisplayingWindow = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem
        With tmpDisplayingWindow
            '.Location.X = tmpPlayWindowSettingControl.NumericUpDown1.Value
            '.Location.Y = tmpPlayWindowSettingControl.NumericUpDown2.Value
            .Magnificine = Val(tmpPlayWindowSettingControl.ComboBox1.Text)
            .ScreenIDItems.Clear()

            For Each tmpScreenButton As ScreenButton In tmpPlayWindowSettingControl.Panel2.Controls
                .ScreenIDItems.Add(tmpScreenButton.ScreenId)

                AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(tmpScreenButton.ScreenId).LocationOfOriginal =
                        tmpPlayWindowSettingControl.Location
            Next

        End With

        IsSave = True
        Me.DialogResult = DialogResult.OK
        Me.Close()

    End Sub

#Region "窗口设置数据校验"
    ''' <summary>
    ''' 窗口设置数据校验
    ''' </summary>
    Private Function IsWindowSettingDataCheckoutOk() As Boolean
        Try

            Dim tmpPlayWindowSettingControl = CType(Panel1.Controls.Item(0), PlayWindowSettingControl)

            If tmpPlayWindowSettingControl.Panel2.Controls.Count = 0 Then
                Throw New Exception($"Window {tmpPlayWindowSettingControl} Screen cannot be empty")
            End If

        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   Button1.Text)
            Return False
        End Try

        Return True
    End Function
#End Region

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub

End Class