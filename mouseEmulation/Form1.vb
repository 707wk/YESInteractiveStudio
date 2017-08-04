Imports System.ComponentModel
Imports System.IO

Public Class Form1
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Me.Text = My.Application.Info.Title

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '普通模式
        版本检测ToolStripMenuItem.Visible = False
        ToolStripSeparator9.Visible = False
        记录数据ToolStripMenuItem.Visible = False
        ToolStripMenuItem11.Visible = False
        ToolStripMenuItem5.Visible = False
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '设置屏幕播放列表格式
        TreeView1.FullRowSelect = True
        TreeView1.HideSelection = False
        TreeView1.ShowNodeToolTips = True
        TreeView1.ShowRootLines = False
        TreeView1.ImageList = ImageList1

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '测试数据
        Dim qwe As New Random
        For i As Integer = 0 To 16 - 1
            Dim tmp As New TreeNode($"屏幕{i}")
            tmp.ContextMenuStrip = ContextMenuStrip1
            tmp.ImageIndex = 0
            tmp.SelectedImageIndex = 0

            For j As Integer = 0 To (qwe.Next Mod 6)
                Dim asd As New TreeNode($"test{j}.swf - 12:34:56")
                asd.ContextMenuStrip = ContextMenuStrip2
                asd.ImageIndex = 1
                asd.SelectedImageIndex = 1
                asd.ToolTipText = $"{Application.StartupPath}\tese.swf"

                tmp.Nodes.Add(asd)
            Next

            TreeView1.Nodes.Add(tmp)
        Next
        TreeView1.ExpandAll()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '设置屏幕状态列表格式
        ListView2.View = View.Details
        ListView2.GridLines = True
        ListView2.FullRowSelect = True
        ListView2.CheckBoxes = False
        ListView2.ShowItemToolTips = True
        ListView2.Clear()
        ListView2.Columns.Add("屏幕", 40, HorizontalAlignment.Left)
        ListView2.Columns.Add("播放文件", 100, HorizontalAlignment.Left)
        ListView2.Columns.Add("播放时长", 60, HorizontalAlignment.Left)

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '测试数据
        For i As Integer = 0 To 32 - 1
            Dim itm As ListViewItem = ListView2.Items.Add($"{i}", 0)
            'itm.ToolTipText = $"{Application.StartupPath}\test{i}.swf"
            itm.SubItems.Add($"test{i}.swf")
            itm.SubItems.Add($"11:22:33")
        Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '读取ini配置文件
        Dim tmp2 As New ClassIni
        Dim x As Integer = CInt(tmp2.GetINI("SYS", "x", "", ".\setting.ini"))
        Dim y As Integer = CInt(tmp2.GetINI("SYS", "y", "", ".\setting.ini"))
        Me.Location = New Point(x, y)

        '清空日志文件
        Dim sw As StreamWriter = New StreamWriter（"log.txt", False)
        sw.Close（）
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Static password As String = Nothing

        password = password & Convert.ToChar(e.KeyValue)
        If password.Length > 128 Then
            password = Microsoft.VisualBasic.Right(password, 32)
        End If

        If password.IndexOf("YESTECH") = -1 Then
            Exit Sub
        End If

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '调试模式
        版本检测ToolStripMenuItem.Visible = True
        ToolStripSeparator9.Visible = True
        记录数据ToolStripMenuItem.Visible = True
        ToolStripMenuItem11.Visible = True
        ToolStripMenuItem5.Visible = True
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Me.Text = $"{Me.Text} [调试模式]"
    End Sub

    Private Sub 关于ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于ToolStripMenuItem.Click
        AboutBox1.ShowDialog()
    End Sub

    Private Sub 技术支持TToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 技术支持TToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://www.csyes.com/service.html")
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        Dim tmp2 As New ClassIni
        tmp2.WriteINI("SYS", "x", Me.Location.X, ".\setting.ini")
        tmp2.WriteINI("SYS", "y", Me.Location.Y, ".\setting.ini")
    End Sub

    '添加文件
    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs) Handles ToolStripButton7.Click
        Dim tmpDialog As New FormAddFile
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 屏幕设置ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 屏幕设置ToolStripMenuItem.Click
        Dim tmpDialog As New FormScreenOption
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 控制器设置ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 控制器设置ToolStripMenuItem.Click
        Dim tmpDialog As New FormControlOption
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 版本检测ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 版本检测ToolStripMenuItem.Click
        Dim tmpDialog As New FormCheckVersions
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 编辑ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 编辑ToolStripMenuItem.Click
        Dim tmpDialog As New FormEditFile
        tmpDialog.ShowDialog()
    End Sub

    '右键选中
    Private Sub TreeView1_MouseDown(sender As Object, e As MouseEventArgs) Handles TreeView1.MouseDown
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            Dim node As TreeNode = TreeView1.GetNodeAt(e.X, e.Y)
            If Not IsNothing(node) Then
                TreeView1.SelectedNode = node
            End If
        End If
    End Sub
End Class
