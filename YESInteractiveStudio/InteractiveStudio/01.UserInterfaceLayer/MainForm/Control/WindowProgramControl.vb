Imports Wangk.Resource

Public Class WindowProgramControl

    ''' <summary>
    ''' 播放窗口信息
    ''' </summary>
    Public DisplayingWindow As DisplayingWindow

    Private Sub WindowProgramControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Me.Visible = True

        ListView1.ContextMenuStrip = ContextMenuStrip1
        ListView1.MultiSelect = False
        ListView1.FullRowSelect = True

        ToolStripButton2.Enabled = False

        ShowFileItems()

        If DisplayingWindow.IsAutoPlay AndAlso
            DisplayingWindow.PlayFileItems.Count > 0 Then

            ToolStripButton1.Enabled = False
            ContextMenuStrip1.Enabled = False
            ToolStripButton2.Enabled = True

        End If

        ChangeControlsLanguage()

    End Sub

    ''' <summary>
    ''' 显示文件列表
    ''' </summary>
    Private Sub ShowFileItems()
        Try
            For Each tmpDisplayingFile In DisplayingWindow.PlayFileItems
                ListView1.Items.Add(
                    New ListViewItem({IO.Path.GetFileName(tmpDisplayingFile.Path),
                    tmpDisplayingFile.Path,
                    tmpDisplayingFile.PlaySecond}))
            Next
        Catch ex As Exception

        End Try

    End Sub

#Region "添加文件"
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim TmpDialog As New OpenFileDialog With {
            .Filter = "Flash or DLL or Unity|*.swf;*.dll;*.exe",
            .Multiselect = True
        }
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        '添加到播放列表
        For Each FileName In TmpDialog.FileNames

            ListView1.Items.Add(New ListViewItem({IO.Path.GetFileName(FileName),
                                                 FileName,
                                                 60}))

            DisplayingWindow.PlayFileItems.Add(
                New DisplayingFile With {
                .Path = FileName,
                .PlaySecond = 60
                })
        Next
    End Sub
#End Region

#Region "移除文件"
    Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        DisplayingWindow.PlayFileItems.RemoveAt(ListView1.SelectedItems(0).Index)
        ListView1.Items.Remove(ListView1.SelectedItems(0))

    End Sub
#End Region

#Region "播放文件"
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If DisplayingWindow.PlayFileItems.Count = 0 Then
            Exit Sub
        End If

        If AppSettingHelper.GetInstance.DisplayMode <> InteractiveOptions.DISPLAYMODE.INTERACT Then
            Exit Sub
        End If

        ToolStripButton1.Enabled = False
        ContextMenuStrip1.Enabled = False
        ToolStripButton2.Enabled = True

        DisplayingWindow.PlayFileID = 0
        DisplayingWindow.IsAutoPlay = True

        DisplayingWindow.PlayWindowForm?.StartPlay(True)

    End Sub
#End Region

#Region "从选中文件开始播放"
    Friend Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        If DisplayingWindow.PlayFileItems.Count = 0 Then
            Exit Sub
        End If

        If AppSettingHelper.GetInstance.DisplayMode <> InteractiveOptions.DISPLAYMODE.INTERACT Then
            Exit Sub
        End If

        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        ToolStripButton1.Enabled = False
        ContextMenuStrip1.Enabled = False
        ToolStripButton2.Enabled = True

        DisplayingWindow.PlayFileID = ListView1.SelectedItems(0).Index
        DisplayingWindow.IsAutoPlay = True

        DisplayingWindow.PlayWindowForm?.StartPlay(True)

    End Sub
#End Region

#Region "停止播放"
    Friend Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        ToolStripButton1.Enabled = True
        ContextMenuStrip1.Enabled = True
        ToolStripButton2.Enabled = False

        DisplayingWindow.IsAutoPlay = False

        DisplayingWindow.PlayWindowForm?.StartPlay(False)

    End Sub
#End Region

#Region "编辑文件"
    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If
        Dim SelectedID = ListView1.SelectedItems(0).Index

        Using tmpDialog As New EditPlayFileForm With {
            .DisplayingFile = DisplayingWindow.PlayFileItems(SelectedID)
        }

            If tmpDialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            ListView1.Items(SelectedID).SubItems(2).Text = tmpDialog.DisplayingFile.PlaySecond

        End Using

    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.ColumnHeader2.Text = .GetS("Path")
            Me.GroupBox2.Text = .GetS("Interactive material list")
            Me.ColumnHeader1.Text = .GetS("File name")
            Me.ColumnHeader3.Text = .GetS("Play Time")
            Me.ToolStripButton3.Text = .GetS("Add file")
            Me.ToolStripButton1.Text = .GetS("Play the current program")
            Me.ToolStripButton2.Text = .GetS("Stop play")
            Me.PlayToolStripMenuItem.Text = .GetS("Play")
            Me.EditToolStripMenuItem.Text = .GetS("Edit")
            Me.RemoveToolStripMenuItem.Text = .GetS("Remove")
        End With
    End Sub
#End Region

End Class