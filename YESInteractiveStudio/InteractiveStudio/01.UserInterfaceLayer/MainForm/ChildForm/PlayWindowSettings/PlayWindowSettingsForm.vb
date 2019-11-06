Imports System.ComponentModel
Imports DevComponents.DotNetBar
Imports Wangk.Resource

Public Class PlayWindowSettingsForm
    ''' <summary>
    ''' 添加播放窗口按钮
    ''' </summary>
    Private AddWindowButton As ButtonItem

    ''' <summary>
    ''' 是否必须保存
    ''' </summary>
    Public IsMustSave As Boolean
    ''' <summary>
    ''' 是否已保存
    ''' </summary>
    Private IsSave As Boolean = False

    Private Sub PlayWindowSettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#Region "添加窗口按钮"
        AddWindowButton = New ButtonItem()
        With AddWindowButton
            .ButtonStyle = eButtonStyle.ImageAndText
            .Text = MultiLanguageHelper.Lang.GetS("Add Play Window")
            .Image = My.Resources.add_24px
            .CustomColorName = "AddPlayWindow"

            AddHandler .Click, AddressOf AddTabClick

        End With
        SuperTabControl1.ControlBox.SubItems.Add(AddWindowButton)
#End Region

        If AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Count = 0 Then
            AddTabClick(Nothing, Nothing)
        End If

        '设置使用标志
        For Each item In AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems
            item.IsUsed = False
        Next
        For Each item In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems
            For Each screenID In item.ScreenIDItems
                AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(screenID).IsUsed = True
            Next
        Next

        ChangeControlsLanguage()

    End Sub

#Region "添加窗口"
    Private Sub AddTabClick(ByVal sender As Object, ByVal e As System.EventArgs)
        Dim s As String = $"{MultiLanguageHelper.Lang.GetS("Window")} {SuperTabControl1.Tabs.Count + 1}"
        Dim tab As SuperTabItem = SuperTabControl1.CreateTab(s)

        tab.AttachedControl.Controls.Add(New PlayWindowSettingControl With {
                                         .DisplayingWindow = New DisplayingWindow With {.Name = s}
                                         })
        SuperTabControl1.SelectedTabIndex = SuperTabControl1.Tabs.Count - 1

    End Sub
#End Region

    Private Sub PlayWindowSettingsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        For Each item In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems
            Dim tab As SuperTabItem = SuperTabControl1.CreateTab(item.Name)

            tab.AttachedControl.Controls.Add(New PlayWindowSettingControl With {
                                             .DisplayingWindow = item
                                             })
        Next

        For pageID = 0 To SuperTabControl1.Tabs.Count - 1
            SuperTabControl1.SelectedTabIndex = pageID
        Next
        SuperTabControl1.SelectedTabIndex = 0

    End Sub

    Private Sub PlayWindowSettingsForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If (IsMustSave AndAlso Not IsSave) AndAlso
            Not IsWindowSettingDataCheckoutOk() Then

            e.Cancel = True
            Exit Sub
        End If

        RemoveHandler AddWindowButton.Click, AddressOf AddTabClick

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If Not IsWindowSettingDataCheckoutOk() Then
            Exit Sub
        End If

        '替换旧数据
        AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Clear()

        For Each tmpSuperTabItem As SuperTabItem In SuperTabControl1.Tabs
            Dim tmpPlayWindowSettingControl = CType(tmpSuperTabItem.AttachedControl.Controls.Item(0), PlayWindowSettingControl)

            Dim tmpDisplayingWindow As New DisplayingWindow
            With tmpDisplayingWindow
                .Name = tmpPlayWindowSettingControl.TextBox1.Text
                .Location.X = tmpPlayWindowSettingControl.NumericUpDown1.Value
                .Location.Y = tmpPlayWindowSettingControl.NumericUpDown2.Value
                .Magnificine = Val(tmpPlayWindowSettingControl.ComboBox1.Text)

                For Each tmpScreenButton As ScreenButton In tmpPlayWindowSettingControl.Panel2.Controls
                    .ScreenIDItems.Add(tmpScreenButton.ScreenId)

                    AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(tmpScreenButton.ScreenId).LocationOfOriginal =
                        tmpScreenButton.Location
                Next

                .PlayFileItems = tmpPlayWindowSettingControl.PlayFileItems

            End With

            AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Add(tmpDisplayingWindow)

        Next

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

            If SuperTabControl1.Tabs.Count = 0 Then
                Throw New Exception(MultiLanguageHelper.Lang.GetS("Window count cannot be 0"))
            End If

            For Each tmpSuperTabItem As SuperTabItem In SuperTabControl1.Tabs
                Dim tmpPlayWindowSettingControl = CType(tmpSuperTabItem.AttachedControl.Controls.Item(0), PlayWindowSettingControl)

                If tmpPlayWindowSettingControl.TextBox1.Text = "" Then
                    Throw New Exception(MultiLanguageHelper.Lang.GetS("Window name cannot be empty"))
                End If

                If tmpPlayWindowSettingControl.Panel2.Controls.Count = 0 Then
                    Throw New Exception($"{MultiLanguageHelper.Lang.GetS("Window")} {tmpPlayWindowSettingControl} {MultiLanguageHelper.Lang.GetS("Screen cannot be empty")}")
                End If

            Next
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

    Private Sub SuperTabControl1_TabItemClose(sender As Object, e As SuperTabStripTabItemCloseEventArgs) Handles SuperTabControl1.TabItemClose
        Dim tmpSuperTabItem = CType(e.Tab, SuperTabItem)
        Dim tmpPlayWindowSettingControl = CType(tmpSuperTabItem.AttachedControl.Controls.Item(0), PlayWindowSettingControl)
        tmpPlayWindowSettingControl.ToolStripButton4_Click(Nothing, Nothing)
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Text = .GetS("PlayWindowSettingsForm")
            Me.Button1.Text = .GetS("Save changes")
            Me.Button2.Text = .GetS("Cancel")
        End With
    End Sub
#End Region

End Class