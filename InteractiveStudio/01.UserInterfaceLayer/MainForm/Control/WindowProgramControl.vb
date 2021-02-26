Imports Wangk.Resource

Public Class WindowProgramControl

    ''' <summary>
    ''' 播放窗口信息
    ''' </summary>
    Public DisplayingWindow As DisplayingWindow

    Private Sub WindowProgramControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill
        Me.Visible = True

        'ListView1.ContextMenuStrip = ContextMenuStrip1
        'ListView1.MultiSelect = False
        'ListView1.FullRowSelect = True

        With CheckBoxDataGridView1
            '.EditMode = DataGridViewEditMode.EditOnEnter
            .AllowDrop = False
            .ReadOnly = True
            '.ColumnHeadersDefaultCellStyle.Font = New Font(Me.Font.Name, Me.Font.Size, FontStyle.Bold)
            .RowHeadersWidth = 80
            .BorderStyle = BorderStyle.None
            .CellBorderStyle = DataGridViewCellBorderStyle.None

            .Columns.Add(New DataGridViewTextBoxColumn With {
                         .HeaderText = MultiLanguageHelper.Lang.GetS("File name"),
                         .Width = 180,
                         .SortMode = DataGridViewColumnSortMode.NotSortable
                         })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                         .HeaderText = MultiLanguageHelper.Lang.GetS("Path"),
                         .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                         .MinimumWidth = 300,
                         .SortMode = DataGridViewColumnSortMode.NotSortable
                         })
            .Columns.Add(New DataGridViewTextBoxColumn With {
                         .HeaderText = MultiLanguageHelper.Lang.GetS("Play Time"),
                         .Width = 80,
                         .SortMode = DataGridViewColumnSortMode.NotSortable
                         })
            .Columns(.Columns.Count - 1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            .Columns.Add(UIFormHelper.GetDataGridViewLinkColumn(MultiLanguageHelper.Lang.GetS("Operation"), UIFormHelper.NormalColor))
            .Columns(.Columns.Count - 1).SortMode = DataGridViewColumnSortMode.NotSortable
            .Columns.Add(UIFormHelper.GetDataGridViewLinkColumn("", UIFormHelper.NormalColor))
            .Columns(.Columns.Count - 1).SortMode = DataGridViewColumnSortMode.NotSortable
            .Columns.Add(UIFormHelper.GetDataGridViewLinkColumn("", UIFormHelper.ErrorColor))
            .Columns(.Columns.Count - 1).SortMode = DataGridViewColumnSortMode.NotSortable

        End With

        ToolStripButton2.Enabled = False

        ShowFileItems()

        If DisplayingWindow.IsAutoPlay AndAlso
            DisplayingWindow.PlayFileItems.Count > 0 Then

            ToolStripButton1.Enabled = False

            'With CheckBoxDataGridView1
            '    .Columns(.Columns.Count - 1).Visible = False
            '    .Columns(.Columns.Count - 2).Visible = False
            '    .Columns(.Columns.Count - 3).Visible = False
            'End With
            'ContextMenuStrip1.Enabled = False
            ToolStripButton4.Enabled = False

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
                CheckBoxDataGridView1.Rows.Add({
                                               False,
                                               IO.Path.GetFileName(tmpDisplayingFile.Path),
                                               tmpDisplayingFile.Path,
                                               tmpDisplayingFile.PlaySecond,
                                               MultiLanguageHelper.Lang.GetS("Play"),
                                               MultiLanguageHelper.Lang.GetS("Edit"),
                                               MultiLanguageHelper.Lang.GetS("Remove")
                                               })
            Next
#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub

#Region "添加文件"
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim TmpDialog As New OpenFileDialog With {
            .Filter = "DLL or Unity|*.dll;*.exe",
            .Multiselect = True
        }
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        '添加到播放列表
        For Each FileName In TmpDialog.FileNames

            CheckBoxDataGridView1.Rows.Add({
                                           False,
                                           IO.Path.GetFileName(FileName),
                                           FileName,
                                           60,
                                           MultiLanguageHelper.Lang.GetS("Play"),
                                           MultiLanguageHelper.Lang.GetS("Edit"),
                                           MultiLanguageHelper.Lang.GetS("Remove")
                                           })

            'ListView1.Items.Add(New ListViewItem({IO.Path.GetFileName(FileName),
            '                                     FileName,
            '                                     60}))

            DisplayingWindow.PlayFileItems.Add(
                New DisplayingFile With {
                .Path = FileName,
                .PlaySecond = 60
                })
        Next
    End Sub
#End Region

#Region "移除文件"
    'Private Sub RemoveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveToolStripMenuItem.Click
    '    If ListView1.SelectedItems.Count < 1 Then
    '        Exit Sub
    '    End If

    '    DisplayingWindow.PlayFileItems.RemoveAt(ListView1.SelectedItems(0).Index)
    '    ListView1.Items.Remove(ListView1.SelectedItems(0))

    'End Sub
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

        'With CheckBoxDataGridView1
        '    .Columns(.Columns.Count - 1).Visible = False
        '    .Columns(.Columns.Count - 2).Visible = False
        '    .Columns(.Columns.Count - 3).Visible = False
        'End With
        'ContextMenuStrip1.Enabled = False
        ToolStripButton4.Enabled = False

        ToolStripButton2.Enabled = True

        DisplayingWindow.PlayFileID = 0
        DisplayingWindow.IsAutoPlay = True

        DisplayingWindow.PlayWindowForm?.StartPlay(True)

    End Sub
#End Region

#Region "播放指定位置文件"
    ''' <summary>
    ''' 播放指定位置文件
    ''' </summary>
    Public Sub RemotePlayFile(fileID As Integer)
        If fileID < 0 Then Exit Sub
        If fileID >= CheckBoxDataGridView1.Rows.Count Then Exit Sub

        If AppSettingHelper.GetInstance.DisplayMode <> InteractiveOptions.DISPLAYMODE.INTERACT Then
            Exit Sub
        End If

        ToolStripButton1.Enabled = False

        'With CheckBoxDataGridView1
        '    .Columns(.Columns.Count - 1).Visible = False
        '    .Columns(.Columns.Count - 2).Visible = False
        '    .Columns(.Columns.Count - 3).Visible = False
        'End With
        'ContextMenuStrip1.Enabled = False
        ToolStripButton4.Enabled = False

        ToolStripButton2.Enabled = True

        DisplayingWindow.PlayFileID = fileID
        DisplayingWindow.IsAutoPlay = True

        DisplayingWindow.PlayWindowForm?.StartPlay(True)

    End Sub
#End Region

    Private Sub CheckBoxDataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles CheckBoxDataGridView1.CellContentClick
        If e.RowIndex < 0 Then
            Exit Sub
        End If

        Select Case e.ColumnIndex
            Case 4
#Region "播放"
                RemotePlayFile(e.RowIndex)
#End Region

            Case 5
#Region "编辑"
                If Not ToolStripButton4.Enabled Then
                    UIFormHelper.ToastWarning(MultiLanguageHelper.Lang.GetS("Please stop playing"))
                    Exit Sub
                End If

                Dim SelectedID = e.RowIndex

                Using tmpDialog As New EditPlayFileForm With {
                    .DisplayingFile = DisplayingWindow.PlayFileItems(SelectedID)
                }

                    If tmpDialog.ShowDialog <> DialogResult.OK Then
                        Exit Sub
                    End If

                    CheckBoxDataGridView1.Rows(SelectedID).Cells(3).Value = tmpDialog.DisplayingFile.PlaySecond

                End Using
#End Region

            Case 6
#Region "移除"
                If Not ToolStripButton4.Enabled Then
                    UIFormHelper.ToastWarning(MultiLanguageHelper.Lang.GetS("Please stop playing"))
                    Exit Sub
                End If

                DisplayingWindow.PlayFileItems.RemoveAt(e.RowIndex)
                CheckBoxDataGridView1.Rows.RemoveAt(e.RowIndex)
#End Region

        End Select
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        '删除
        For rowID = CheckBoxDataGridView1.Rows.Count - 1 To 0 Step -1
            If CheckBoxDataGridView1.Rows(rowID).Cells(0).EditedFormattedValue Then
                DisplayingWindow.PlayFileItems.RemoveAt(rowID)
                CheckBoxDataGridView1.Rows.RemoveAt(rowID)
            End If
        Next
    End Sub

#Region "从选中文件开始播放"
    'Friend Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
    '    If DisplayingWindow.PlayFileItems.Count = 0 Then
    '        Exit Sub
    '    End If

    '    If AppSettingHelper.GetInstance.DisplayMode <> InteractiveOptions.DISPLAYMODE.INTERACT Then
    '        Exit Sub
    '    End If

    '    If ListView1.SelectedItems.Count < 1 Then
    '        Exit Sub
    '    End If

    '    ToolStripButton1.Enabled = False
    '    ContextMenuStrip1.Enabled = False
    '    ToolStripButton2.Enabled = True

    '    DisplayingWindow.PlayFileID = ListView1.SelectedItems(0).Index
    '    DisplayingWindow.IsAutoPlay = True

    '    DisplayingWindow.PlayWindowForm?.StartPlay(True)

    'End Sub
#End Region

#Region "停止播放"
    Friend Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        ToolStripButton1.Enabled = True

        'With CheckBoxDataGridView1
        '    .Columns(.Columns.Count - 1).Visible = True
        '    .Columns(.Columns.Count - 2).Visible = True
        '    .Columns(.Columns.Count - 3).Visible = True
        'End With
        'ContextMenuStrip1.Enabled = True
        ToolStripButton4.Enabled = True

        ToolStripButton2.Enabled = False

        DisplayingWindow.IsAutoPlay = False

        DisplayingWindow.PlayWindowForm?.StartPlay(False)

    End Sub
#End Region

#Region "编辑文件"
    'Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click
    '    If ListView1.SelectedItems.Count < 1 Then
    '        Exit Sub
    '    End If
    '    Dim SelectedID = ListView1.SelectedItems(0).Index

    '    Using tmpDialog As New EditPlayFileForm With {
    '        .DisplayingFile = DisplayingWindow.PlayFileItems(SelectedID)
    '    }

    '        If tmpDialog.ShowDialog <> DialogResult.OK Then
    '            Exit Sub
    '        End If

    '        ListView1.Items(SelectedID).SubItems(2).Text = tmpDialog.DisplayingFile.PlaySecond

    '    End Using

    'End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            'Me.ColumnHeader2.Text = .GetS("Path")
            Me.GroupBox2.Text = .GetS("Interactive material list")
            'Me.ColumnHeader1.Text = .GetS("File name")
            'Me.ColumnHeader3.Text = .GetS("Play Time")
            Me.ToolStripButton3.Text = .GetS("Add file")
            Me.ToolStripButton1.Text = .GetS("Play the current program")
            Me.ToolStripButton2.Text = .GetS("Stop play")
            Me.ToolStripButton4.Text = .GetS("Remove")
            'Me.PlayToolStripMenuItem.Text = .GetS("Play")
            'Me.EditToolStripMenuItem.Text = .GetS("Edit")
            'Me.RemoveToolStripMenuItem.Text = .GetS("Remove")
        End With
    End Sub
#End Region

End Class