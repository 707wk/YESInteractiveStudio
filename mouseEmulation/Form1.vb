Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Nova.Mars.SDK

Public Class Form1
    '声明注册热键API函数
    Public Declare Function RegisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer,
                                                    ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    '声明注销热键API函数
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer) As Integer
    Public Const WM_HOTKEY As Short = &H312S '热键消息ID，此值固定，不能修改
    Public Const MOD_ALT As Short = &H1S  'ALT按键ID
    Public Const MOD_CONTROL As Short = &H2S  'Ctrl
    Public Const MOD_SHIFT As Short = &H4S  'Shift

    '鼠标模拟事件
    Private Declare Function mouse_event Lib "user32.dll" Alias "mouse_event" (ByVal dwFlags As MouseEvent, ByVal dX As Int32, ByVal dY As Int32, ByVal dwData As Int32, ByVal dwExtraInfo As Int32) As Boolean
    '鼠标操作
    Enum MouseEvent
        None
        AbsoluteLocation = &H8000
        LeftButtonDown = &H2
        LeftButtonUp = &H4
        Move = &H1
        MiddleButtonDown = &H20
        MiddleButtonUp = &H40
        RightButtonDown = &H8
        RightButtonUp = &H10
        Wheel = &H800
        WheelDelta = 120
        XButtonDown = &H100
        XButtonUp = &H200
    End Enum
    '显示鼠标指针
    Declare Function ShowCursor Lib "user32.dll" (ByVal bShow As Int32) As Int32

    '接收线程
    Dim workThread As Threading.Thread

    '创建窗体
    Public Sub creatDialogThread(ByVal screenIndex As Integer)
        screenMain(screenIndex).playDialog = New FormPlay
        screenMain(screenIndex).playDialog.touchPieceWidth = screenMain(screenIndex).ScanBoardWidth \ 4
        screenMain(screenIndex).playDialog.touchPieceHeight = screenMain(screenIndex).ScanBoardHeight \ 4

        screenMain(screenIndex).playDialog.setLocation(screenMain(screenIndex).x,
                                                       screenMain(screenIndex).y,
                                                       screenMain(screenIndex).width,
                                                       screenMain(screenIndex).height)
        screenMain(screenIndex).playDialog.ShowDialog()
    End Sub

    '显示播放窗体
    Private Sub updateScreen()
        '刷新屏幕下拉列表列表
        '刷新播放信息列表
        ComboBox1.Items.Clear()
        ListView1.Items.Clear()

        '显示标记的屏幕窗体
        Dim tmpCreatDialogThread As Threading.Thread
        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                ComboBox1.Items.Add($"{i} - {screenMain(i).remark}")

                Dim itm As ListViewItem = ListView1.Items.Add($"{i}")

                itm.SubItems.Add($"{screenMain(i).remark}")
                itm.SubItems.Add($"{screenMain(i).filePath}")

                If screenMain(i).playDialog Is Nothing Then
                    tmpCreatDialogThread = New Threading.Thread(AddressOf creatDialogThread)
                    tmpCreatDialogThread.SetApartmentState(ApartmentState.STA)
                    tmpCreatDialogThread.IsBackground = True
                    tmpCreatDialogThread.Start(i)
                End If
            Else
                screenMain(i).filePath = ""
                If screenMain(i).playDialog IsNot Nothing Then
                    screenMain(i).playDialog.closeDialog("")
                    screenMain(i).playDialog = Nothing
                End If
            End If
        Next
        '自适应宽度
        ListView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
        ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)

        '清空控制器连接标记
        For i As Integer = 0 To senderArray.Length - 1
            senderArray(i).link = False
        Next
        '判断哪些控制器要连接
        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                For Each j As Integer In screenMain(i).SenderList
                    senderArray(j).link = True
                Next
            End If
        Next
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True
        Me.Text = My.Application.Info.Title
        ToolStripStatusLabel1.Text = $"查询次数:未连接"
        Me.ToolStripStatusLabel2.Text = $"| 屏幕数：0 | 控制器数：0"
        ComboBox2.Sorted = True

        '初始为断开连接模式
        offLinkCon()

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '普通模式[测试时注释]
        debugMode(False)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '保留功能
        新建ToolStripMenuItem.Enabled = False
        打开ToolStripMenuItem.Enabled = False
        保存ToolStripMenuItem.Enabled = False
        另存为ToolStripMenuItem.Enabled = False
        最近的文件ToolStripMenuItem.Enabled = False
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '下拉列表只读
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList

        '设置屏幕播放列表格式
        'TreeView1.FullRowSelect = True
        'TreeView1.HideSelection = False
        'TreeView1.ShowNodeToolTips = True
        'TreeView1.ShowRootLines = False
        'TreeView1.ImageList = ImageList1

        '''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        ''测试数据
        'Dim qwe As New Random
        'For i As Integer = 0 To 16 - 1
        '    Dim tmp As New TreeNode($"屏幕{i}")
        '    tmp.ContextMenuStrip = ContextMenuStrip1
        '    tmp.ImageIndex = 0
        '    tmp.SelectedImageIndex = 0

        '    For j As Integer = 0 To (qwe.Next Mod 6)
        '        Dim asd As New TreeNode($"test{j}.swf - 12:34:56")
        '        asd.ContextMenuStrip = ContextMenuStrip2
        '        asd.ImageIndex = 1
        '        asd.SelectedImageIndex = 1
        '        asd.ToolTipText = $"{Application.StartupPath}\tese.swf"

        '        tmp.Nodes.Add(asd)
        '    Next

        '    TreeView1.Nodes.Add(tmp)
        'Next
        'TreeView1.ExpandAll()
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '设置播放信息列表格式
        ListView1.View = View.Details
        ListView1.GridLines = True
        ListView1.FullRowSelect = True
        ListView1.CheckBoxes = False
        ListView1.ShowItemToolTips = True
        ListView1.Clear()
        ListView1.Columns.Add("屏幕", 40, HorizontalAlignment.Left)
        ListView1.Columns.Add("备注")
        ListView1.Columns.Add("播放文件")
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '测试数据
        'For i As Integer = 0 To 32 - 1
        '    Dim itm As ListViewItem = ListView1.Items.Add($"{i}", 0)
        '    itm.ToolTipText = $"{Application.StartupPath}\test{i}.swf"
        '    itm.SubItems.Add($"test{i}.swf")
        'Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '自适应宽度
        ListView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)

        '读取ini配置文件
        Dim tmp2 As New ClassIni
        Dim x As Integer = CInt(tmp2.GetINI("SYS", "x", "", ".\setting.ini"))
        Dim y As Integer = CInt(tmp2.GetINI("SYS", "y", "", ".\setting.ini"))
        Me.Location = New Point(x, y)

        checkTime = 100
        Try
            checkTime = CInt(tmp2.GetINI("SYS", "Time", "", ".\setting.ini"))
        Catch ex As Exception
        End Try
        ToolStripTextBox1.Text = checkTime

        '清空日志文件
        Dim sw As StreamWriter = New StreamWriter（"log.txt", False)
        sw.Close（）

        '绑定设置到ip事件
        'AddHandler mainClass.GetEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)
        RegisterHotKey(Me.Handle.ToInt32, 5, 0, Keys.F5)
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
        debugMode(True)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        Me.Text = $"{My.Application.Info.Title} [调试模式]"
    End Sub

    Private Sub 关于ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于ToolStripMenuItem.Click
        AboutBox1.ShowDialog()
    End Sub

    Private Sub 技术支持TToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 技术支持TToolStripMenuItem.Click
        System.Diagnostics.Process.Start("http://www.csyes.com/service.html")
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '未关闭连接则屏蔽关闭按钮消息
        If ToolStripButton1.Text <> "连接控制器" Then
            e.Cancel = True
            Exit Sub
        End If

        Try
            recordDataFile.Close()
        Catch ex As Exception
        End Try

        Try
            For i As Integer = 0 To screenMain.Length - 1
                If screenMain(i).playDialog IsNot Nothing Then
                    screenMain(i).playDialog.closeDialog("")
                    screenMain(i).playDialog = Nothing
                End If
            Next
        Catch ex As Exception
        End Try


        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F5)

        '序列化
        Try
            'Dim systeminfo As New playInfoSave
            ReDim systeminfo.playList(screenMain.Length - 1)
            For i As Integer = 0 To screenMain.Length - 1
                'systeminfo.playList(i).filePath = screenMain(i).filePath
                systeminfo.playList(i).showFlage = screenMain(i).showFlage
                systeminfo.playList(i).remark = screenMain(i).remark
            Next

            Dim fStream As New FileStream("screen.ini", FileMode.Create)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            sfFormatter.Serialize(fStream, systeminfo)
            fStream.Close()
        Catch ex As Exception
            putlog(ex.Message)
            '不知道会不会引发异常，加个保险
        End Try

        Dim tmp2 As New ClassIni
        tmp2.WriteINI("SYS", "x", Me.Location.X, ".\setting.ini")
        tmp2.WriteINI("SYS", "y", Me.Location.Y, ".\setting.ini")
        tmp2.WriteINI("SYS", "Time", checkTime, ".\setting.ini")

        If mainClass Is Nothing Then
        Else
            mainClass.UnInitialize()
        End If
        If rootClass Is Nothing Then
        Else
            rootClass.UnInitialize()
        End If

    End Sub

    '添加文件
    Private Sub ToolStripButton7_Click(sender As Object, e As EventArgs)
        Dim tmpDialog As New FormAddFile
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 屏幕设置ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 屏幕设置ToolStripMenuItem.Click
        Dim tmpDialog As New FormScreenOption
        If tmpDialog.ShowDialog() = DialogResult.OK Then
            '刷新显示屏及播放信息列表
            updateScreen()
        End If
    End Sub

    'Dim senderArrayIndex As Integer = 0
    ''设置ip通知
    'Private Sub SendEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
    '    If e.IsExecResult Then
    '        senderArray(senderArrayIndex).ipDate = senderArray(senderArrayIndex).tmpIpData

    '        senderArrayIndex += 1
    '        If senderArrayIndex < senderArray.Length Then
    '            mainClass.SetEquipmentIP(senderArrayIndex, senderArray(senderArrayIndex).tmpIpData)
    '        Else
    '            MsgBox("ip设置成功")
    '        End If
    '    Else
    '        MsgBox($"控制器{senderArrayIndex}设置IP数据失败！请检查设备后，重新发送！")
    '    End If
    'End Sub

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
    'Private Sub TreeView1_MouseDown(sender As Object, e As MouseEventArgs)
    '    If e.Button = System.Windows.Forms.MouseButtons.Right Then
    '        Dim node As TreeNode = TreeView1.GetNodeAt(e.X, e.Y)
    '        If Not IsNothing(node) Then
    '            TreeView1.SelectedNode = node
    '        End If
    '    End If
    'End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim tmpDialog As New FormNovaInit
        tmpDialog.ShowDialog()

        'If systeminfo.playList Is Nothing Then
        '    '没有要显示的屏幕
        'Else
        '    '显示上次关闭前显示的屏幕
        '    For i As Integer = 0 To systeminfo.playList.Length
        '        '屏幕索引超过读取到的屏幕个数-1则不再添加
        '        If systeminfo.playList(i).screenIndex > screenMain.Length - 1 Then
        '            Exit For
        '        End If
        '        ComboBox1.Items.Add(systeminfo.playList(i).screenIndex)
        '    Next
        'End If

        '刷新文件列表
        If systeminfo.filesList Is Nothing Then
            '没有要显示的文件列表
            systeminfo.filesList = New Hashtable
        Else
            '显示上次关闭前显示的文件列表
            For Each i In systeminfo.filesList.Keys
                ComboBox2.Items.Add(i)
            Next
        End If

        ''刷新屏幕下拉列表列表
        ''刷新播放信息列表
        'ComboBox1.Items.Clear()
        'ListView1.Items.Clear()

        'For i As Integer = 0 To screenMain.Length - 1
        '    If screenMain(i).showFlage Then
        '        ComboBox1.Items.Add($"{i} - {screenMain(i).remark}")

        '        Dim itm As ListViewItem = ListView1.Items.Add($"{i}")

        '        itm.SubItems.Add($"{screenMain(i).remark}")
        '        itm.SubItems.Add($"{screenMain(i).filePath}")
        '    End If
        'Next
        ''自适应宽度
        'ListView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
        'ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)

        Try
            updateScreen()
            Me.ToolStripStatusLabel2.Text = $"| 屏幕数：{screenMain.Length} | 控制器数：{senderArray.Length}"
        Catch ex As Exception
        End Try
    End Sub

    '窗体的消息处理函数
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY And ToolStripButton1.Enabled = True Then '判断是否为热键消息
            'Me.Text = m.WParam.ToInt32
            Select Case m.WParam.ToInt32 '判断热键消息的注册ID
                Case 1
                    ToolStripMenuItem2_Click(Nothing, Nothing)
                Case 2
                    ToolStripMenuItem4_Click(Nothing, Nothing)
                Case 3
                    ToolStripMenuItem6_Click(Nothing, Nothing)
                Case 4
                    ToolStripMenuItem7_Click(Nothing, Nothing)
                Case 5
                    If Me.Text.IndexOf("调试模式") = -1 Then
                        Exit Sub
                    End If

                    ToolStripMenuItem5_Click(Nothing, Nothing)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub

    Private Sub getHistoryFilesList()
        Dim filesList() As String
        '创建菜单索引
        Dim menuList(10 - 1) As ToolStripMenuItem
        menuList(0) = ToolStripMenuItem15
        menuList(1) = ToolStripMenuItem16
        menuList(2) = ToolStripMenuItem17
        menuList(3) = ToolStripMenuItem18
        menuList(4) = ToolStripMenuItem19
        menuList(5) = ToolStripMenuItem20
        menuList(6) = ToolStripMenuItem21
        menuList(7) = ToolStripMenuItem22
        menuList(8) = ToolStripMenuItem23
        menuList(9) = ToolStripMenuItem24

        '隐藏菜单
        For i As Integer = 0 To 10 - 1
            menuList(i).Text = $"NULL"
            menuList(i).Visible = False
        Next

        '反序列化
        Try
            Dim fStream As New FileStream(“history.txt”, FileMode.Open)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            filesList = sfFormatter.Deserialize(fStream)
            fStream.Close()

            '显示历史文件菜单
            For i As Integer = 0 To filesList.Length - 1
                If i > 10 - 1 Then
                    Exit For
                End If

                menuList(i).Text = filesList(i)
                menuList(i).Visible = True
            Next
        Catch ex As Exception
            'history.txt不存在会引发异常，但没关系
        End Try

    End Sub

    Private Sub setHistoryFilesList(newFile As String)
        Dim filesList() As String
        '创建菜单索引
        Dim menuList(10 - 1) As ToolStripMenuItem
        menuList(0) = ToolStripMenuItem15
        menuList(1) = ToolStripMenuItem16
        menuList(2) = ToolStripMenuItem17
        menuList(3) = ToolStripMenuItem18
        menuList(4) = ToolStripMenuItem19
        menuList(5) = ToolStripMenuItem20
        menuList(6) = ToolStripMenuItem21
        menuList(7) = ToolStripMenuItem22
        menuList(8) = ToolStripMenuItem23
        menuList(9) = ToolStripMenuItem24

        '增加历史记录
        Dim tmpList As New List(Of String)
        For i As Integer = 0 To 10 - 1
            If menuList(10 - 1 - i).Text = $"NULL" Then
                Continue For
            End If

            tmpList.Add($"{menuList(10 - 1 - i).Text}")
        Next

        '删除已有的再添加
        tmpList.Remove(newFile)
        tmpList.Add(newFile)

        '倒序赋值
        ReDim filesList(tmpList.Count - 1)
        For i As Integer = 0 To tmpList.Count - 1
            filesList(tmpList.Count - 1 - i) = tmpList(i)
        Next

        '序列化
        Try
            Dim fStream As New FileStream(“history.txt”, FileMode.Create)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            sfFormatter.Serialize(fStream, filesList)
            fStream.Close()
        Catch ex As Exception
            putlog($"序列化异常 {ex.Message}")
            '不知道会不会引发异常，加个保险
        End Try

    End Sub

    Private Sub 清空历史ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 清空历史ToolStripMenuItem.Click
        Dim menuList(10 - 1) As ToolStripMenuItem
        menuList(0) = ToolStripMenuItem15
        menuList(1) = ToolStripMenuItem16
        menuList(2) = ToolStripMenuItem17
        menuList(3) = ToolStripMenuItem18
        menuList(4) = ToolStripMenuItem19
        menuList(5) = ToolStripMenuItem20
        menuList(6) = ToolStripMenuItem21
        menuList(7) = ToolStripMenuItem22
        menuList(8) = ToolStripMenuItem23
        menuList(9) = ToolStripMenuItem24

        '隐藏文件历史
        For i As Integer = 0 To 10 - 1
            menuList(i).Text = $"NULL"
            menuList(i).Visible = False
        Next
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    '历史记录
    'Private Sub ToolStripMenuItem15_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem15.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem16_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem16.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem17_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem17.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem18_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem18.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem19_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem19.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem20_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem20.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem21_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem21.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem22_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem22.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem23_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem23.Click

    'End Sub
    ''
    'Private Sub ToolStripMenuItem24_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem24.Click

    'End Sub
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

    '添加文件
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim tmp As New OpenFileDialog
        'tmp.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        tmp.Filter = "swf|*.swf"
        tmp.Multiselect = True
        tmp.ShowDialog()

        If tmp.FileNames.Length < 1 Then
            Exit Sub
        End If

        For i As Integer = 0 To tmp.FileNames.Length - 1
            If systeminfo.filesList.Item(tmp.SafeFileNames(i)) IsNot Nothing Then
                Continue For
            End If

            ComboBox2.Items.Add(tmp.SafeFileNames(i))
            systeminfo.filesList.Add(tmp.SafeFileNames(i), tmp.FileNames(i))
        Next

    End Sub

    '删除文件
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        systeminfo.filesList.Remove(ComboBox2.Text)
        ComboBox2.Items.Remove(ComboBox2.Text)
    End Sub

    '调试模式
    Private Sub debugMode(Value As Boolean)
        版本检测ToolStripMenuItem.Visible = Value
        'ToolStripSeparator9.Visible = Value
        '记录数据ToolStripMenuItem.Visible = Value
        灵敏度调节ToolStripMenuItem.Visible = Value
        ToolStripSeparator3.Visible = Value
        ToolStripButton2.Visible = Value
        ToolStripMenuItem11.Visible = Value
        ToolStripMenuItem5.Visible = Value
    End Sub

    '连接
    Private Sub onLinkCon()
        ToolStripMenuItem1.Enabled = True
        屏幕模式SToolStripMenuItem.Enabled = True
        屏幕设置ToolStripMenuItem.Enabled = False
        控制器设置ToolStripMenuItem.Enabled = False
        灵敏度调节ToolStripMenuItem.Enabled = False
        版本检测ToolStripMenuItem.Enabled = False

        ToolStripButton1.Text = "断开连接"
        ToolStripButton1.Image = My.Resources.disconnect
    End Sub

    '断开连接
    Private Sub offLinkCon()
        ToolStripMenuItem1.Enabled = False
        屏幕模式SToolStripMenuItem.Enabled = False
        屏幕设置ToolStripMenuItem.Enabled = True
        控制器设置ToolStripMenuItem.Enabled = True
        灵敏度调节ToolStripMenuItem.Enabled = True
        版本检测ToolStripMenuItem.Enabled = True

        ToolStripButton1.Text = "连接控制器"
        ToolStripButton1.Image = My.Resources.connect
    End Sub

    '连接-断开连接
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If ToolStripButton1.Text = "连接控制器" Then
            Dim screenNums As Integer = 0
            For i As Integer = 0 To screenMain.Length - 1
                If screenMain(i).showFlage Then
                    screenNums += 1
                End If
            Next
            If screenNums = 0 Then
                MsgBox("未设置屏幕", MsgBoxStyle.Information, "连接控制器")
                Exit Sub
            End If

            '判断是否能连通
            For i As Integer = 0 To senderArray.Length - 1
                Dim ipStr As String = $"{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}"

                If My.Computer.Network.Ping(ipStr, 500) = False Then
                    MsgBox(ipStr & " 未能连通", MsgBoxStyle.Information, "连接")
                    Exit Sub
                End If
            Next

            '建立与控制器的连接
            Try
                For i As Integer = 0 To senderArray.Length - 1
                    If senderArray(i).link = False Then
                        Continue For
                    End If

                    Dim ipStr As String = $"{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}"

                    senderArray(i).cliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    '发送超时
                    senderArray(i).cliSocket.SendTimeout = 1000
                    '接收超时
                    senderArray(i).cliSocket.ReceiveTimeout = 1000
                    '连接
                    senderArray(i).cliSocket.Connect(ipStr, 6000)
                Next
            Catch ex As Exception
                For i As Integer = 0 To senderArray.Length - 1
                    Try
                        senderArray(i).cliSocket.Close()
                    Catch ex2 As Exception
                    End Try
                Next

                MsgBox("端口绑定失败，请稍后再连接", MsgBoxStyle.Information, "连接")

                Exit Sub
            End Try

            workThread = New Threading.Thread(AddressOf communicWorkThread)
            workThread.IsBackground = True
            workThread.Start()

            onLinkCon()
        Else
            '断开
            workThread.Abort()
            Thread.Sleep(500)

            For i As Integer = 0 To senderArray.Length - 1
                Try
                    senderArray(i).cliSocket.Close()
                Catch ex As Exception
                End Try
            Next

            offLinkCon()
        End If
    End Sub

    '播放
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim reg As New Regex("\d+")
        Dim m As Match = reg.Match(ComboBox1.Text)

        If m.Success Then
            screenMain(CInt(m.Value)).playDialog.play(systeminfo.filesList.Item(ComboBox2.Text))
            screenMain(CInt(m.Value)).filePath = ComboBox2.Text

            For i As Integer = 0 To ListView1.Items.Count - 1
                If CInt(ListView1.Items(i).SubItems(0).Text) = CInt(m.Value) Then
                    ListView1.Items(i).SubItems(2).Text = ComboBox2.Text
                    Exit For
                End If
            Next

            ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)
        Else
            MsgBox("屏幕编号读取失败")
        End If
    End Sub

    '播放所有屏幕
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.play(systeminfo.filesList.Item(ComboBox2.Text))
                screenMain(i).filePath = ComboBox2.Text
            End If
        Next

        For i As Integer = 0 To ListView1.Items.Count - 1
            ListView1.Items(i).SubItems(2).Text = ComboBox2.Text
        Next

        ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub

    '设置读取间隔
    Private Sub ToolStripTextBox1_LostFocus(sender As Object, e As EventArgs) Handles ToolStripTextBox1.LostFocus
        Dim tmp As Integer
        Try
            tmp = CInt(ToolStripTextBox1.Text)
        Catch ex As Exception
            Exit Sub
        End Try

        If tmp > 10000 Then
            Exit Sub
        End If

        checkTime = tmp

    End Sub

    Public Delegate Sub showChecknumCallback(ByVal checknum As Integer)
    Public Sub showChecknum(ByVal checknum As Integer)
        If Me.InvokeRequired Then
            Dim d As New showChecknumCallback(AddressOf showChecknum)
            Me.Invoke(d, New Object() {checknum})
        Else
            'putlog("1")
            ToolStripStatusLabel1.Text = $"查询次数:{checknum}/s"
        End If

    End Sub

    Public Delegate Sub showExceptionCallback(ByVal nums As Integer)
    Public Sub showException(ByVal nums As Integer)
        If Me.InvokeRequired Then
            Dim d As New showExceptionCallback(AddressOf showException)
            Me.Invoke(d, New Object() {nums})
        Else
            workThread.Abort()
            Thread.Sleep(500)

            For i As Integer = 0 To senderArray.Length - 1
                Try
                    senderArray(i).cliSocket.Close()
                Catch ex As Exception
                End Try
            Next

            offLinkCon()

            MsgBox($"控制器已连续{nums}次未返回数据!", MsgBoxStyle.Information, Me.Text)
        End If

    End Sub

    ''移动鼠标点击区域
    'Private Sub getMouseClick(tmpScanBoard As ScanBoardInfo, tDataArray As Byte(), index As Integer)
    '    'Dim tmp As ScanBoardInfo = screenMain.ScanBoardTable.Item(key)

    '    'If runMode <> 0 And runMode <> 1 Then
    '    '    Exit Sub
    '    'End If
    '    'Dim qwe As String = Nothing
    '    'For i As Integer = 0 To 16 - 1
    '    '    qwe = qwe & tDataArray(index + i).ToString("X") & " "
    '    'Next
    '    'putlog(qwe)
    '    'putlog($"{key} {tmp.ConnectIndex}-{tmp.PortIndex}-{tmp.SenderIndex} {tmp.X} {tmp.Y}")

    '    BackgroundWorker1.WorkerReportsProgress = True

    '    Dim w As Integer = screenMain(tmpScanBoard.ScreenIndex).ScanBoardWidth / 4
    '    Dim h As Integer = screenMain(tmpScanBoard.ScreenIndex).ScanBoardHeight / 4

    '    For i As Integer = 0 To 4 - 1
    '        For j As Integer = 0 To 4 - 1
    '            If (tDataArray(index + i * 4 + j) And &H80) <> &H80 Then
    '                Continue For
    '            End If

    '            '点击
    '            Dim oldx As Integer = System.Windows.Forms.Control.MousePosition.X
    '            Dim oldy As Integer = System.Windows.Forms.Control.MousePosition.Y

    '            '隐藏鼠标指针
    '            ShowCursor(False)

    '            '移动鼠标然后点击
    '            mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move Or MouseEvent.LeftButtonDown Or MouseEvent.LeftButtonUp,
    '                            (screenMain(tmpScanBoard.ScreenIndex).x + tmpScanBoard.X + j * w + screenMain(tmpScanBoard.ScreenIndex).ScanBoardWidth / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Width,
    '                            (screenMain(tmpScanBoard.ScreenIndex).y + tmpScanBoard.Y + i * h + screenMain(tmpScanBoard.ScreenIndex).ScanBoardHeight / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
    '            '回原位
    '            mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move,
    '                            oldx * 65536 / Screen.PrimaryScreen.Bounds.Width,
    '                            oldy * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
    '            '显示鼠标指针
    '            ShowCursor(True)
    '            'End If
    '        Next
    '    Next
    'End Sub

    'Public Delegate Sub showRecCallback(ByVal text As String)
    'Public Sub showRec(ByVal text As String)
    '    If Me.InvokeRequired Then
    '        Dim d As New showRecCallback(AddressOf showRec)
    '        Me.Invoke(d, New Object() {text})
    '    Else
    '        'putlog("1")
    '        TextBox1.Text = text
    '    End If

    'End Sub

    '检测线程
    Private Sub communicWorkThread()
        Dim lastsec As Integer = -1
        Dim nowsec As Integer = 0
        Dim checknum As Integer = 0
        Dim exceptionNums As Integer = 0

        Do
            nowsec = Now().Second
            If lastsec = nowsec Then
                checknum += 1
                'putlog(checknum)
            Else
                exceptionNums = 0
                'putlog(checknum)
                showChecknum(checknum)
                'ToolStripStatusLabel1.Text = $"查询次数:{checknum}/s"
                checknum = 0
                lastsec = nowsec
            End If

            If exceptionNums > 3 Then
                showException(exceptionNums)
                Exit Sub
            End If

            'Dim asd As New Stopwatch
            'asd.Start()

            Dim showstr As String = Nothing
            For index As Integer = 0 To senderArray.Length - 1
                '未连接则跳过
                If senderArray(index).link = False Then
                    Continue For
                End If

                Try
                    Dim bytes(1028 - 1) As Byte
                    Dim tmpstr As String = "55d50902"
                    Dim sendbytes(4 - 1) As Byte
                    For i As Integer = 0 To tmpstr.Length \ 2 - 1
                        sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                    Next i

                    Dim bytesSend As Integer = senderArray(index).cliSocket.Send(sendbytes)
                    Dim bytesRec As Integer = senderArray(index).cliSocket.Receive(bytes)

                Catch ex As Exception
                    exceptionNums += 1
                    putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 01:{ex.Message}")
                    Continue For
                End Try

                'Dim asd As New Stopwatch
                'asd.Start()

                Try
                    Dim bytes(1028 - 1) As Byte
                    Dim tmpstr As String = "55d50905000000000400"
                    Dim sendbytes(10 - 1) As Byte
                    For i As Integer = 0 To tmpstr.Length \ 2 - 1
                        sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                    Next i
                    Dim bytesSend As Integer = senderArray(index).cliSocket.Send(sendbytes)

                    For i As Integer = 0 To 16 - 1
                        Dim bytesRec As Integer = senderArray(index).cliSocket.Receive(bytes)
                        'TextBox5.Text = ""

                        For j As Integer = 4 To 1027 Step 32
                            If bytes(j + 0) <> &H55 Then
                                Continue For
                            End If

                            If bytes(j + 1) > 4 Then
                                Continue For
                            End If

                            If runMode <> 0 And runMode <> 1 And runMode <> 4 Then
                                Exit For
                            End If

                            '查找接收卡位置[由像素改为索引]
                            Dim tmp = ScanBoardTable.Item($"{index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}")
                            If tmp Is Nothing Then
                                Continue For
                            End If

                            For k As Integer = 0 To 4 - 1
                                For l As Integer = 0 To 4 - 1
                                    'screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = bytes(index + k * 4 + l) And &H80
                                    If (bytes(j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                        If runMode = 1 Or runMode = 4 Then
                                            screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))
                                        End If
                                        screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = 0
                                        Continue For
                                    End If
                                    'putlog($"{tmp.Y }+{ k}, {tmp.X} +{ l}")

                                    'putlog($"{k},{l}")
                                    If screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                        screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                        'putlog($"{tmp.Y + k},{tmp.X + l}")
                                        Continue For
                                    End If

                                    'putlog($"{tmp.Y }+{ k}, {tmp.X} +{ l}")
                                    screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                    screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))

                                    'tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                    ''读取数据段
                                    'For m As Integer = 4 To 27
                                    '    tmpstr = tmpstr & bytes(j + m).ToString("X") & " "
                                    'Next

                                    'showstr = showstr & tmpstr & vbCrLf
                                Next
                            Next

                            If recordDataFlage Then
                                tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                '读取数据段
                                For m As Integer = 4 To 27
                                    tmpstr = tmpstr & bytes(j + m) & " "
                                Next

                                recordDataFile.WriteLine(tmpstr)
                            End If
                            'showstr = showstr & tmpstr & vbCrLf
                        Next
                    Next

                    'showRec(showstr)
                    'putlog($"{showstr}")
                    'BackgroundWorker1.ReportProgress(1, showstr)
                Catch ex As Exception
                    exceptionNums += 1
                    putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 02:{ex.Message}")
                    Continue For
                End Try
            Next

            Thread.Sleep(checkTime)
        Loop
    End Sub

    '点击
    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        runMode = 0
        ToolStripMenuItem1.Text = ToolStripMenuItem2.Text

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchPlayMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem8_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        ToolStripMenuItem2_Click(Nothing, Nothing)
    End Sub

    '测试
    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        runMode = 1

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchTestMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem10_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem10.Click
        ToolStripMenuItem4_Click(Nothing, Nothing)
    End Sub

    '黑屏
    Private Sub ToolStripMenuItem6_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem6.Click
        runMode = 2

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchBlankScreenMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem12_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        ToolStripMenuItem6_Click(Nothing, Nothing)
    End Sub

    '忽略
    Private Sub ToolStripMenuItem7_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        runMode = 3
    End Sub
    Private Sub ToolStripMenuItem13_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem13.Click
        ToolStripMenuItem7_Click(Nothing, Nothing)
    End Sub

    '测试(电容)
    Private Sub ToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click
        runMode = 4

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchTestModeWithValue("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem11_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem11.Click
        ToolStripMenuItem5_Click(Nothing, Nothing)
    End Sub

    Private Sub 退出ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 退出ToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        systeminfo.filesList.Clear()
        ComboBox2.Items.Clear()
    End Sub

    '记录数据
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If ToolStripButton2.Text = "记录数据" Then
            recordDataFile = New StreamWriter($"DEBUG{Format(Now(), "yyyyMMddHHmmss")}.txt", True)

            recordDataFlage = True

            ToolStripButton2.Text = "停止记录"
            ToolStripButton2.Image = My.Resources.disconnect
        Else
            recordDataFile.Close()

            recordDataFlage = False

            ToolStripButton2.Text = "记录数据"
            ToolStripButton2.Image = My.Resources.connect
        End If
    End Sub

    Private Sub 灵敏度调节ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 灵敏度调节ToolStripMenuItem.Click
        Dim tmpDialog As New FormTouchSetting
        tmpDialog.ShowDialog()
    End Sub
End Class
