Public Class ProgramEdit
    Dim WindowId As Integer
    Dim ProgramId As Integer

#Region "加载节目"
    ''' <summary>
    ''' 加载节目
    ''' </summary>
    Public Sub LoadProgram(ByVal WindowId As Integer, ByVal ProgramId As Integer)
        Me.WindowId = WindowId
        Me.ProgramId = ProgramId
        Me.ActiveControl = Me.ListView1

        With AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId)
            TextBox1.Text = .Remark

            ListView1.Items.Clear()
            For Each i001 As MediaInfo In .MediaList
                ListView1.Items.Add("")
                ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(i001.Path)
                ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(i001.PlayTime)
            Next

            UpdateID()
        End With

        TextBox2.Clear()
    End Sub
#End Region

    Private Sub ProgramEdit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
#Region "样式设置"
        With ListView1
            .View = View.Details
            .GridLines = True
            .LabelEdit = False
            .FullRowSelect = True
            .HideSelection = True
            .ShowItemToolTips = True
            .MultiSelect = False

            .Columns.Add(AppSetting.Language.GetS("ID"), 40)
            .Columns.Add(AppSetting.Language.GetS("File"), 400)
            .Columns.Add(AppSetting.Language.GetS("Play Time(s)"), 100)
            .ContextMenuStrip = MediaMenuStrip
        End With

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
#End Region
    End Sub

#Region "更新序号"
    ''' <summary>
    ''' 更新序号
    ''' </summary>
    Public Sub UpdateID()
        Dim ID As Integer = 1
        For Each i001 As ListViewItem In ListView1.Items
            i001.SubItems(0).Text = ID
            ID += 1
        Next
    End Sub
#End Region

#Region "添加文件"
    ''' <summary>
    ''' 添加文件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        Dim TmpDialog As New OpenFileDialog With {
            .Filter = "DLL or Unity|*.dll;*.exe",
            .Multiselect = True
        }
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        '添加到播放列表
        For i001 As Integer = 0 To TmpDialog.FileNames.Length - 1

            ListView1.Items.Add("")
            ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(TmpDialog.FileNames(i001))
            ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(60)

            AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList.Add(
                New MediaInfo With {
                .Path = TmpDialog.FileNames(i001),
                .PlayTime = 60
                })
        Next

        UpdateID()
    End Sub
#End Region

#Region "删除文件"
    ''' <summary>
    ''' 删除文件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub DeleteMediaToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteMediaToolStripMenuItem.Click
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        Dim mediaId As Integer = ListView1.SelectedItems(0).Index

        ListView1.Items.RemoveAt(mediaId)
        AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList.RemoveAt(mediaId)
        TextBox2.Clear()

        UpdateID()
    End Sub
#End Region

#Region "保存节目"
    Private Sub TextBox1_LostFocus(sender As Object, e As EventArgs) Handles TextBox1.LostFocus
        Dim TmpProgram As ProgramInfo = AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId)

        TmpProgram.Remark = TextBox1.Text

        AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId) = TmpProgram

        MDIParentMain.UpdateProgram(WindowId, ProgramId)
    End Sub
#End Region

#Region "编辑文件"
#Region "显示文件"
    Public Sub ShowMedia(ByVal MediaId As Integer)
        TextBox2.Text = MediaId + 1
        TextBox3.Text = AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList(MediaId).Path
        NumericUpDown1.Value = AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList(MediaId).PlayTime
    End Sub
#End Region

#Region "更新文件"
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim TmpDialog As New OpenFileDialog With {
            .Filter = "DLL or Unity|*.dll;*.exe",
            .Multiselect = False
        }
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        TextBox3.Text = TmpDialog.FileName
    End Sub
#End Region

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        ShowMedia(ListView1.SelectedItems(0).Index)
    End Sub

    ''' <summary>
    ''' 保存文件
    ''' </summary>
    Private Sub GroupBox2_Leave(sender As Object, e As EventArgs) Handles GroupBox2.Leave
        Dim mediaId As Integer = Val(TextBox2.Text)
        If mediaId = 0 Then
            Exit Sub
        End If

        mediaId -= 1

        Dim TmpMedia As MediaInfo = AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList(mediaId)

        TmpMedia.Path = TextBox3.Text
        TmpMedia.PlayTime = NumericUpDown1.Value

        AppSetting.Schedule.WindowList(WindowId).ProgramList(ProgramId).MediaList(mediaId) = TmpMedia

        ListView1.Items(mediaId).SubItems(1).Text = TextBox3.Text
        ListView1.Items(mediaId).SubItems(2).Text = NumericUpDown1.Value
    End Sub
#End Region

#Region "播放文件"
    Private Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        Dim mediaId As Integer = ListView1.SelectedItems(0).Index

        Dim TmpWindowInfo As WindowInfo = AppSetting.Schedule.WindowList(WindowId)

        With TmpWindowInfo
            If .ProgramList(ProgramId).MediaList.Count = 0 Then
                Exit Sub
            End If

            .PlayProgramInfo = .ProgramList(ProgramId)
            .PlayMediaId = mediaId
            .PlayMediaTime = 0

            If AppSetting.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT Then
                If .PlayDialog IsNot Nothing Then
                    .PlayDialog.Play(.PlayProgramInfo.MediaList(mediaId).Path)
                End If
            End If
        End With

        AppSetting.Schedule.WindowList(WindowId) = TmpWindowInfo
    End Sub
#End Region

#Region "播放节目"
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ListView1.Items.Count < 1 Then
            Exit Sub
        End If

        Dim TmpWindowInfo As WindowInfo = AppSetting.Schedule.WindowList(WindowId)

        With TmpWindowInfo
            If .ProgramList(ProgramId).MediaList.Count = 0 Then
                Exit Sub
            End If

            .PlayProgramInfo = .ProgramList(ProgramId)
            .PlayMediaId = 0
            .PlayMediaTime = 0

            If AppSetting.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT Then
                If .PlayDialog IsNot Nothing Then
                    .PlayDialog.Play(.PlayProgramInfo.MediaList(0).Path)
                End If
            End If
        End With

        AppSetting.Schedule.WindowList(WindowId) = TmpWindowInfo
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With AppSetting.Language
            Me.Label1.Text = .GetS("Remark")
            Me.GroupBox1.Text = .GetS("Media List")
            Me.ToolStrip1.Text = .GetS("ToolStrip1")
            Me.ToolStripButton1.Text = .GetS("Add Media")
            Me.GroupBox2.Text = .GetS("Media Setting")
            Me.Label4.Text = .GetS("Play Time")
            Me.Button1.Text = .GetS("Browse ...")
            Me.Label3.Text = .GetS("File")
            Me.Label2.Text = .GetS("ID")
            Me.PlayToolStripMenuItem.Text = .GetS("Play")
            Me.DeleteMediaToolStripMenuItem.Text = .GetS("Delete Media")
            Me.Label5.Text = .GetS("s")
            Me.Button2.Text = .GetS("Play")
        End With
    End Sub
#End Region
End Class