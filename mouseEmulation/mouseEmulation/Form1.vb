Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Nova.Mars.SDK

Public Class Form1
    ''' <summary>
    ''' 声明注册热键API函数
    ''' </summary>
    ''' <param name="hWnd"></param>
    ''' <param name="id"></param>
    ''' <param name="fsModifiers"></param>
    ''' <param name="vk"></param>
    ''' <returns></returns>
    Public Declare Function RegisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer,
                                                    ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    ''' <summary>
    ''' 声明注销热键API函数
    ''' </summary>
    ''' <param name="hWnd"></param>
    ''' <param name="id"></param>
    ''' <returns></returns>
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer) As Integer
    ''' <summary>
    ''' 热键消息ID，此值固定，不能修改
    ''' </summary>
    Public Const WM_HOTKEY As Short = &H312S
    ''' <summary>
    ''' ALT按键ID
    ''' </summary>
    Public Const MOD_ALT As Short = &H1S
    ''' <summary>
    ''' Ctrl
    ''' </summary>
    Public Const MOD_CONTROL As Short = &H2S
    ''' <summary>
    ''' Shift
    ''' </summary>
    Public Const MOD_SHIFT As Short = &H4S

    ''' <summary>
    ''' 鼠标模拟事件
    ''' </summary>
    ''' <param name="dwFlags"></param>
    ''' <param name="dX"></param>
    ''' <param name="dY"></param>
    ''' <param name="dwData"></param>
    ''' <param name="dwExtraInfo"></param>
    ''' <returns></returns>
    Private Declare Function mouse_event Lib "user32.dll" Alias "mouse_event" (ByVal dwFlags As MouseEvent, ByVal dX As Int32, ByVal dY As Int32, ByVal dwData As Int32, ByVal dwExtraInfo As Int32) As Boolean
    ''' <summary>
    ''' 鼠标操作
    ''' </summary>
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
    ''' <summary>
    ''' 显示鼠标指针
    ''' </summary>
    ''' <param name="bShow"></param>
    ''' <returns></returns>
    Declare Function ShowCursor Lib "user32.dll" (ByVal bShow As Int32) As Int32

    ''' <summary>
    ''' 接收线程
    ''' </summary>
    Dim workThread As Threading.Thread

    ''' <summary>
    ''' 创建窗体
    ''' </summary>
    ''' <param name="screenIndex"></param>
    Public Sub creatDialogThread(ByVal screenIndex As Integer)
        screenMain(screenIndex).playDialog = New FormPlay
        If InteractMode = 1 Or InteractMode = 2 Then
            screenMain(screenIndex).playDialog.touchPieceHeight = screenMain(screenIndex).ScanBoardHeight \ 4
            screenMain(screenIndex).playDialog.touchPieceWidth = screenMain(screenIndex).ScanBoardWidth \ 4
        End If
        If InteractMode = 3 Then
            screenMain(screenIndex).playDialog.touchPieceHeight = screenMain(screenIndex).ScanBoardHeight \ 2
            screenMain(screenIndex).playDialog.touchPieceWidth = screenMain(screenIndex).ScanBoardWidth \ 2
        End If

        screenMain(screenIndex).playDialog.setLocation(screenMain(screenIndex).x,
                                                       screenMain(screenIndex).y,
                                                       screenMain(screenIndex).width,
                                                       screenMain(screenIndex).height)
        screenMain(screenIndex).playDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 显示播放窗体
    ''' </summary>
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
        Me.Text = My.Application.Info.Title + "0930"
        ToolStripStatusLabel1.Text = $"{If(selectLanguageId = 0, "查询次数:未连接", "Update:Disconnect")}"
        Me.ToolStripStatusLabel2.Text = $"| {If(selectLanguageId = 0, "| 屏幕数：0 | 控制器数：0", "Screen Nums：0 | Control Nums：0")}"
        ComboBox2.Sorted = True


        '初始为断开连接模式
        offLinkCon()

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '普通模式[测试时注释]
        debugMode(False)
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        InteractMode = 1 '普通模式
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
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "屏幕", "Screen")}", 40, HorizontalAlignment.Left)
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "备注", "remark")}")
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "播放文件", "playing")}")

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

        '读取ini配置文件并初始化
        Dim tmp2 As New ClassIni
        '程序启动位置读取
        Dim x As Integer = CInt(tmp2.GetINI("SYS", "x", "", ".\setting.ini"))
        Dim y As Integer = CInt(tmp2.GetINI("SYS", "y", "", ".\setting.ini"))
        If x < 0 Then
            x = 0
        End If
        If y < 0 Then
            y = 0
        End If
        If x >= Screen.PrimaryScreen.Bounds.Width Then
            x = Screen.PrimaryScreen.Bounds.Width - 10
        End If
        If y >= Screen.PrimaryScreen.Bounds.Height Then
            y = Screen.PrimaryScreen.Bounds.Height - 10
        End If


        Me.Location = New Point(x, y)

        '屏幕灵敏度参数读取
        ScreenSensitivity = CInt(tmp2.GetINI("SYS", "Screensensitivity", "", ".\setting.ini"))
        '屏幕互动模式参数读取
        InteractMode = CInt(tmp2.GetINI("SYS", "InteractMode", "", ".\setting.ini"))
        '屏幕抗干扰参数读取
        ScreenAnti = CInt(tmp2.GetINI("SYS", "ScreenAnti", "", ".\setting.ini"))
        '屏幕抗干扰总数参数读取
        ScreenAntiS = CInt(tmp2.GetINI("SYS", "ScreenAntiS", "", ".\setting.ini"))
        '定时复位温度增量参数读取
        ResetTemp = CInt(tmp2.GetINI("SYS", "ResetTemp", "", ".\setting.ini"))
        '定时复位时间参数读取
        ResetTimeSec = CInt(tmp2.GetINI("SYS", "ResetTimeSec", "", ".\setting.ini"))
        '定时复位类型，0软件，1硬件
        resetType = CInt(tmp2.GetINI("SYS", "resetType", "0", ".\setting.ini"))

        ' If ResetTimeSec <= 0 Then
        Timer1.Enabled = True
        ' Else
        Timer1.Interval = 800
        ' End If


        ' 语言设置保存
        selectLanguageId = CInt(tmp2.GetINI("SYS", "selectLanguageId", "", ".\setting.ini"))


        '程序与诺瓦设备通信间隔参数读取
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
        '键盘获取密码
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

        Me.Text = $"{My.Application.Info.Title} [{If(selectLanguageId = 0, "调试模式", "Debug")}]"
    End Sub

    Private Sub 关于ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 关于ToolStripMenuItem.Click
        '启动关于窗口
        AboutBox1.ShowDialog()
    End Sub

    Private Sub 技术支持TToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 技术支持TToolStripMenuItem.Click
        '打开技术支持网址
        System.Diagnostics.Process.Start("http://www.csyes.com/service.html")
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '未关闭连接则屏蔽关闭按钮消息
        If ToolStripButton1.Text <> "连接控制器" And ToolStripButton1.Text <> "Connect Screen" Then
            ' e.Cancel = True
            ' Exit Sub

            ToolStripButton1_Click(Nothing, Nothing)
        End If

        Try
            '关闭记录文件
            recordDataFile.Close()
        Catch ex As Exception
        End Try
        '清空播放窗口
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
        '保存系统变量参数
        Dim tmp2 As New ClassIni
        tmp2.WriteINI("SYS", "x", Me.Location.X, ".\setting.ini")
        tmp2.WriteINI("SYS", "y", Me.Location.Y, ".\setting.ini")
        tmp2.WriteINI("SYS", "Time", checkTime, ".\setting.ini")
        tmp2.WriteINI("SYS", "Screensensitivity", ScreenSensitivity, ".\setting.ini")
        tmp2.WriteINI("SYS", "InteractMode", InteractMode, ".\setting.ini")
        tmp2.WriteINI("SYS", "ScreenAnti", ScreenAnti, ".\setting.ini")
        tmp2.WriteINI("SYS", "ScreenAntiS", ScreenAntiS, ".\setting.ini")
        tmp2.WriteINI("SYS", "ResetTemp", ResetTemp, ".\setting.ini")
        tmp2.WriteINI("SYS", "ResetTimeSec", ResetTimeSec, ".\setting.ini")
        tmp2.WriteINI("SYS", "selectLanguageId", selectLanguageId, ".\setting.ini")
        tmp2.WriteINI("SYS", "resetType", resetType, ".\setting.ini")



        '清空变量
        If mainClass Is Nothing Then
        Else
            mainClass.UnInitialize()
        End If
        If rootClass Is Nothing Then
        Else
            rootClass.UnInitialize()
        End If

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
        '打开控制器设置窗口
        tmpDialog.ShowDialog()
    End Sub

    Private Sub 版本检测ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 版本检测ToolStripMenuItem.Click
        Dim tmpDialog As New FormCheckVersions
        '打开版本检测窗口
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
        '诺瓦发送卡参数读取，初始化系统参数
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
            '刷新屏幕
            updateScreen()
            Me.ToolStripStatusLabel2.Text = $"| {If(selectLanguageId = 0, "屏幕数", "Screen Nums")}：{screenMain.Length} | {If(selectLanguageId = 0, "控制器数", "Control Nums")}：{senderArray.Length}"
        Catch ex As Exception
        End Try
        '绑定刷新数据事件
        AddHandler mainClass.RefreshHardwareStatusFinishEvent, AddressOf RefreshHardwareStatusFinishEvent
    End Sub

    ''' <summary>
    ''' 窗体的消息处理函数
    ''' </summary>
    ''' <param name="m"></param>
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
                    'If Me.Text.IndexOf($"调试模式") = -1 And Me.Text.IndexOf($"Debug") = -1 Then
                    'Exit Sub
                    'End If

                    ToolStripMenuItem5_Click(Nothing, Nothing)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub

    ''' <summary>
    ''' 读取打开文件历史 [废弃]
    ''' </summary>
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

    ''' <summary>
    ''' 保存开文件历史 [废弃]
    ''' </summary>
    ''' <param name="newFile"></param>
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
            putlog($"{If(selectLanguageId = 0, "序列化异常", "Serialized exception")} {ex.Message}")
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

    ''' <summary>
    ''' 添加文件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        '添加flash文件
        Dim tmp As New OpenFileDialog
        'tmp.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
        tmp.Filter = "swf|*.swf"
        tmp.Multiselect = True
        tmp.ShowDialog()
        '没添加文件，退出过程
        If tmp.FileNames.Length < 1 Then
            Exit Sub
        End If
        '添加到播放列表
        For i As Integer = 0 To tmp.FileNames.Length - 1
            If systeminfo.filesList.Item(tmp.SafeFileNames(i)) IsNot Nothing Then
                Continue For
            End If

            ComboBox2.Items.Add(tmp.SafeFileNames(i))
            systeminfo.filesList.Add(tmp.SafeFileNames(i), tmp.FileNames(i))
        Next

    End Sub

    ''' <summary>
    ''' 删除文件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        systeminfo.filesList.Remove(ComboBox2.Text)
        ComboBox2.Items.Remove(ComboBox2.Text)
    End Sub

    ''' <summary>
    ''' 调试模式
    ''' </summary>
    ''' <param name="Value"></param>
    Private Sub debugMode(Value As Boolean)

        版本检测ToolStripMenuItem.Visible = Value
        'ToolStripSeparator9.Visible = Value
        '记录数据ToolStripMenuItem.Visible = Value
        '灵敏度调节ToolStripMenuItem.Visible = Value*-
        '数据记录布局分割
        ToolStripSeparator3.Visible = Value

        '数据记录 按钮
        ToolStripButton2.Visible = Value
        '强制复位 按钮
        ToolStripButton3.Visible = Value
        '灵敏度调节
        'ToolStripButton4.Visible = Value
        '测试灵敏度对话框
        'ToolStripButton5.Visible = Value

        '测试电容值 按钮
        ToolStripMenuItem11.Visible = Value
        '测试电容值 按钮
        ToolStripMenuItem5.Visible = Value



    End Sub

    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub onLinkCon()
        '设置按键属性
        ToolStripMenuItem1.Enabled = True
        屏幕模式SToolStripMenuItem.Enabled = True
        屏幕设置ToolStripMenuItem.Enabled = False
        控制器设置ToolStripMenuItem.Enabled = False
        灵敏度调节ToolStripMenuItem.Enabled = False
        版本检测ToolStripMenuItem.Enabled = False
        '更新标示图标
        ToolStripButton1.Text = If(selectLanguageId = 0, "断开连接", "Disconnect Screen")
        ToolStripButton1.Image = My.Resources.disconnect
    End Sub

    ''' <summary>
    ''' 断开连接
    ''' </summary>
    Private Sub offLinkCon()
        '设置按键属性
        ToolStripMenuItem1.Enabled = False
        屏幕模式SToolStripMenuItem.Enabled = False
        屏幕设置ToolStripMenuItem.Enabled = True
        控制器设置ToolStripMenuItem.Enabled = True
        灵敏度调节ToolStripMenuItem.Enabled = True
        版本检测ToolStripMenuItem.Enabled = True
        '更新标示图标
        ToolStripButton1.Text = If(selectLanguageId = 0, "连接控制器", "Connect Screen")
        ToolStripButton1.Image = My.Resources.connect
    End Sub

    ''' <summary>
    ''' 连接-断开连接
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If ToolStripButton1.Text = "连接控制器" Or ToolStripButton1.Text = "Connect Screen" Then
            Dim pz As Boolean


            Dim screenNums As Integer = 0
            '统计屏幕数量
            For i As Integer = 0 To screenMain.Length - 1
                If screenMain(i).showFlage Then
                    screenNums += 1
                End If
            Next
            '不存在屏幕，退出
            If screenNums = 0 Then
                MsgBox($"{If(selectLanguageId = 0, "未设置屏幕", "Unset screen")}",
                       MsgBoxStyle.Information,
                       $"{If(selectLanguageId = 0, "连接", "Connect")}")
                Exit Sub
            End If

            '判断是否能连通
            For i As Integer = 0 To senderArray.Length - 1
                Dim ipStr As String = $"{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}"
                'ping 设备IP地址
                If My.Computer.Network.Ping(ipStr, 500) = False Then
                    MsgBox(ipStr & $"{If(selectLanguageId = 0, " 未能连通", " Failed to connect")}",
                           MsgBoxStyle.Information,
                           $"{If(selectLanguageId = 0, "连接", "Connect")}")
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
                '异常关闭连接
                For i As Integer = 0 To senderArray.Length - 1
                    Try
                        senderArray(i).cliSocket.Close()
                    Catch ex2 As Exception
                    End Try
                Next

                MsgBox($"{If(selectLanguageId = 0, "端口绑定失败，请稍后再连接", "The port binding failed, please connect later")}",
                       MsgBoxStyle.Information,
                       $"{If(selectLanguageId = 0, "连接", "Connect")}")

                Exit Sub
            End Try


            '建立一个线程文件
            workThread = New Threading.Thread(AddressOf communicWorkThread)
            '线程后台运行
            workThread.IsBackground = True
            '线程开始
            workThread.Start()

            onLinkCon()
            '如果硬件复位开启，重新设置。如果软件复位，开启定时器。
            If resetType Then
                '硬件复位功能打开
                pz = SetresetTemp(ResetTemp)
                'Delay()
                Thread.Sleep(1000)

                '硬件复位功能打开
                pz = SetResetTimeSec(ResetTimeSec)
                Thread.Sleep(1000)
            Else
                Timer1.Enabled = True '软件复位
            End If

        Else
            '硬件复位功能关闭

            workThread.Abort()
            Thread.Sleep(500)
            '关闭连接
            For i As Integer = 0 To senderArray.Length - 1
                Try
                    senderArray(i).cliSocket.Close()
                Catch ex As Exception
                End Try
            Next

            offLinkCon()


            '复位关闭。
            If resetType Then
                Dim mz As Boolean
                '硬件复位温度功能关闭
                mz = SetresetTemp(0)
                Thread.Sleep(1000)
                '硬件复位定时功能关闭
                mz = SetResetTimeSec(0)
                Thread.Sleep(1000)
            Else
                '断开
                Timer1.Enabled = False '软件复位

            End If
            ToolStripStatusLabel1.Text = $"{If(selectLanguageId = 0, "未连接", "Disconnect")}"
        End If
    End Sub

    ''' <summary>
    ''' 播放
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        '正则表达式
        Dim reg As New Regex("\d+")
        Dim m As Match = reg.Match(ComboBox1.Text)

        If m.Success Then
            Dim objFile As System.IO.File
            '文件为空，提示
#Disable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
            If Not objFile.Exists(systeminfo.filesList.Item(ComboBox2.Text)) Then
#Enable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
                MsgBox($"{If(selectLanguageId = 0, "文件不存在！请清空列表，重新加入文件！", "The file don't exist! Please clear list，Add the file！")}")
                ' Exit Sub
            End If
            '打开播放的内容
            screenMain(CInt(m.Value)).playDialog.play(systeminfo.filesList.Item(ComboBox2.Text))
            screenMain(CInt(m.Value)).filePath = ComboBox2.Text
            screenMain(CInt(m.Value)).playDialog.switchPlayMode("") '播放器打开
            For i As Integer = 0 To ListView1.Items.Count - 1
                If CInt(ListView1.Items(i).SubItems(0).Text) = CInt(m.Value) Then
                    ListView1.Items(i).SubItems(2).Text = ComboBox2.Text
                    Exit For
                End If
            Next

            ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)
        Else
            MsgBox($"{If(selectLanguageId = 0, "屏幕编号读取失败", "Screen number reads failed")}")
        End If
    End Sub

    ''' <summary>
    ''' 播放所有屏幕
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                Dim objFile As System.IO.File
#Disable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
                If Not objFile.Exists(systeminfo.filesList.Item(ComboBox2.Text)) Then
#Enable Warning BC42025 ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
                    MsgBox($"{If(selectLanguageId = 0, "文件不存在！", "The file don't exist! ")}")
                    ' Exit Sub
                End If
                screenMain(i).playDialog.play(systeminfo.filesList.Item(ComboBox2.Text))
                screenMain(i).filePath = ComboBox2.Text
                screenMain(i).playDialog.switchPlayMode("") '播放器打开
            End If
        Next

        For i As Integer = 0 To ListView1.Items.Count - 1
            ListView1.Items(i).SubItems(2).Text = ComboBox2.Text
        Next

        ListView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub

    ''' <summary>
    ''' 设置读取间隔
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripTextBox1_LostFocus(sender As Object, e As EventArgs) Handles ToolStripTextBox1.LostFocus
        Dim tmp As Integer
        Try
            tmp = CInt(ToolStripTextBox1.Text)
        Catch ex As Exception
            Exit Sub
        End Try

        If tmp > 1000 Then
            Exit Sub
        End If

        checkTime = tmp
    End Sub

    ''' <summary>
    ''' 显示当前每秒查询次数
    ''' </summary>
    ''' <param name="checknum"></param>
    Public Delegate Sub showChecknumCallback(ByVal checknum As Integer)
    Public Sub showChecknum(ByVal checknum As Integer)
        If Me.InvokeRequired Then
            Dim d As New showChecknumCallback(AddressOf showChecknum)
            Me.Invoke(d, New Object() {checknum})
        Else
            'putlog("1")
            ToolStripStatusLabel1.Text = $"{If(selectLanguageId = 0, "查询次数", "Update")}:{checknum}/s"
        End If

    End Sub

    ''' <summary>
    ''' 异常时断开连接并提示
    ''' </summary>
    ''' <param name="nums"></param>
    Public Delegate Sub showExceptionCallback(ByVal nums As Integer)
    Public Sub showException(ByVal nums As Integer)
        If Me.InvokeRequired Then
            Dim d As New showExceptionCallback(AddressOf showException)
            Me.Invoke(d, New Object() {nums})
        Else
            '关闭线程
            workThread.Abort()
            Thread.Sleep(500)

            For i As Integer = 0 To senderArray.Length - 1
                Try
                    senderArray(i).cliSocket.Close()
                Catch ex As Exception
                End Try
            Next

            offLinkCon()

            MsgBox($"{If(selectLanguageId = 0, "控制器已连续", "The controller has not returned data for")} {nums} {If(selectLanguageId = 0, "次未返回数据!", "consecutive times!")}",
                   MsgBoxStyle.Information,
                   Me.Text)
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

    ''' <summary>
    ''' 检测线程
    ''' </summary>
    Private Sub communicWorkThread()
        '上次运行时间
        Dim lastsec As Integer = -1
        ' Dim lastmillisec As Integer = -1
        '当前运行时间
        Dim nowsec As Integer = 0
        'Dim nowmillisec As Integer = 0
        '检测的次数
        Dim checknum As Integer = 0
        ' Dim checkTime As Integer
        '异常次数
        Dim exceptionNums As Integer = 0
        ' ReDim ReceiveData(16, 256)
        'ReDim ReceiveDataH(16, 256)

        Do
            '获取当前时间秒
            nowsec = Now().Second
            'nowmillisec = Now().Millisecond
            If lastsec = nowsec Then
                checknum += 1
                'putlog(checknum)
            Else
                exceptionNums = 0
                'putlog(checknum)
                'Dim k As Integer =
                'checkTime = (nowsec * 1000 + nowmillisec) - (lastsec * 1000 + lastsec)
                '显示读取频率到任务栏
                showChecknum(checknum)
                'ToolStripStatusLabel1.Text = $"查询次数:{checknum}/s"
                checknum = 0
                lastsec = nowsec
                'lastmillisec = Now().Millisecond
            End If
            '出现三次异常，进行提示，并且终止进程
            If exceptionNums > 3 Then
                showException(exceptionNums)
                Exit Sub
            End If

            'Dim asd As New Stopwatch
            'asd.Start()
            '发送到发送卡的字符
            Dim showstr As String = Nothing
            '轮询所有的屏幕设备
            For index As Integer = 0 To senderArray.Length - 1
                '未连接则跳过
                If senderArray(index).link = False Then
                    Continue For
                End If

                Try
                    '向发送卡 请求接收传感器数据数据
                    Dim bytes(1028 - 1) As Byte
                    Dim tmpstr As String = "55d50902"
                    Dim sendbytes(4 - 1) As Byte
                    For i As Integer = 0 To tmpstr.Length \ 2 - 1
                        sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                    Next i
                    '发送数据
                    Dim bytesSend As Integer = senderArray(index).cliSocket.Send(sendbytes)
                    '接收数据
                    Dim bytesRec As Integer = senderArray(index).cliSocket.Receive(bytes)

                Catch ex As Exception
                    '出现异常，显示什么异常，结束本次循环
                    exceptionNums += 1
                    putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 01:{ex.Message}")
                    Continue For
                End Try

                'Dim asd As New Stopwatch
                'asd.Start()
                '向发送卡 请求发送传感器数据数据
                Try
                    '向发送卡发送数据
                    Dim bytes(1028 - 1) As Byte
                    Dim tmpstr As String = "55d50905000000000400"
                    Dim sendbytes(10 - 1) As Byte
                    For i As Integer = 0 To tmpstr.Length \ 2 - 1
                        sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                    Next i
                    '发送到发送卡数据
                    Dim bytesSend As Integer = senderArray(index).cliSocket.Send(sendbytes)

                    '诺瓦每次只发送1K数据，16K数据分16次发送
                    For i As Integer = 0 To 16 - 1

                        Dim bytesRec As Integer = senderArray(index).cliSocket.Receive(bytes)
                        'TextBox5.Text = ""
                        '分析接收到的数据
                        For j As Integer = 4 To 1027 Step 32
                            '有效数据头
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
                            '接收卡数据分析 参数
                            Dim tempimax As Integer = 0
                            Dim bytemax(0) As Byte
                            Dim kmax As Integer = 0
                            Dim lmax As Integer = 0

                            ' ReceiveData(15, 0) = 12
                            TempS = (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 2) And &H80) +
                                    (bytes(j + 3 + 3) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                     (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80) +
                                      (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80) +
                                       (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) +
                                        (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                         (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 14) And &H80) +
                                          (bytes(j + 3 + 15) And &H80) + (bytes(j + 3 + 16) And &H80)
                            For k As Integer = 0 To 4 - 1
                                If InteractMode = 3 Then '四合一模式
                                    Dim tempB(0) As Byte
                                    Dim tx3 As Integer = 0
                                    Dim ty3 As Integer = 0
                                    Select Case k
                                        Case 0 '第1个线圈
                                            TempI = (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 2) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80)
                                            tx3 = 0
                                            ty3 = 0
                                            If TempI > 128 Then
                                                tempB(0) = ((bytes(j + 3 + 1) And &H7F) + (bytes(j + 3 + 2) And &H7F) +
                                                    (bytes(j + 3 + 5) And &H7F) + (bytes(j + 3 + 6) And &H7F)) / 4 + 128
                                            Else
                                                tempB(0) = ((bytes(j + 3 + 1) And &H7F) + (bytes(j + 3 + 2) And &H7F) +
                                                    (bytes(j + 3 + 5) And &H7F) + (bytes(j + 3 + 6) And &H7F)) / 4
                                            End If

                                        Case 1 '第2个线圈
                                            TempI = (bytes(j + 3 + 3) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80)
                                            tx3 = 1
                                            ty3 = 0
                                            If TempI > 128 Then
                                                tempB(0) = ((bytes(j + 3 + 3) And &H7F) + (bytes(j + 3 + 4) And &H7F) +
                                                    (bytes(j + 3 + 7) And &H7F) + (bytes(j + 3 + 8) And &H7F)) / 4 + 128
                                            Else
                                                tempB(0) = ((bytes(j + 3 + 3) And &H7F) + (bytes(j + 3 + 4) And &H7F) +
                                                    (bytes(j + 3 + 7) And &H7F) + (bytes(j + 3 + 8) And &H7F)) / 4
                                            End If
                                        Case 2 '第3个线圈
                                            TempI = (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) +
                                                    (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 14) And &H80)
                                            tx3 = 0
                                            ty3 = 1
                                            If TempI > 128 Then
                                                tempB(0) = ((bytes(j + 3 + 9) And &H7F) + (bytes(j + 3 + 10) And &H7F) +
                                                    (bytes(j + 3 + 13) And &H7F) + (bytes(j + 3 + 14) And &H7F)) / 4 + 128
                                            Else
                                                tempB(0) = ((bytes(j + 3 + 9) And &H7F) + (bytes(j + 3 + 10) And &H7F) +
                                                   (bytes(j + 3 + 13) And &H7F) + (bytes(j + 3 + 14) And &H7F)) / 4
                                            End If
                                        Case 3 '第4个线圈
                                            TempI = (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 15) And &H80) + (bytes(j + 3 + 16) And &H80)
                                            tx3 = 1
                                            ty3 = 1
                                            If TempI > 128 Then
                                                tempB(0) = ((bytes(j + 3 + 11) And &H7F) + (bytes(j + 3 + 12) And &H7F) +
                                                    (bytes(j + 3 + 15) And &H7F) + (bytes(j + 3 + 16) And &H7F)) / 4 + 128
                                            Else
                                                tempB(0) = ((bytes(j + 3 + 11) And &H7F) + (bytes(j + 3 + 12) And &H7F) +
                                                    (bytes(j + 3 + 15) And &H7F) + (bytes(j + 3 + 16) And &H7F)) / 4
                                            End If
                                    End Select
                                    If (tempB(0) And &H80) <> &H80 Then
                                        If runMode = 1 Or runMode = 4 Then
                                            screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X / 2 + tx3, tmp.Y / 2 + ty3, tempB(0))
                                        End If
                                        screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y / 2 + ty3, tmp.X / 2 + tx3) = 0
                                        Continue For
                                    End If
                                    If screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y / 2 + ty3, tmp.X / 2 + tx3) Then
                                        screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y / 2 + ty3, tmp.X / 2 + tx3) = &H80

                                        Continue For
                                    End If
                                    screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y / 2 + ty3, tmp.X / 2 + tx3) = &H80

                                    screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X / 2 + tx3, tmp.Y / 2 + ty3, tempB(0))
                                    'Dim tmp3 As New ClassIni
                                    'If tmp3.GetINI("Screen", "0-1-2", "0", ".\InteractionNum.ini") Then

                                    'End If

                                    'tmp3.WriteINI("SYS", "x", Me.Location.X, ".\InteractionNum.ini")




                                    '有感应信号记录数据
                                    ' If TempI > 128 And recordDataFlage Then 'And (TempI2 > 128)
                                    'tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                    '读取数据段
                                    'For m As Integer = 4 To 27
                                    'tmpstr = tmpstr & bytes(j + m) & " "
                                    'Next
                                    'recordDataFile.WriteLine("InteractMode = 3")
                                    'recordDataFile.WriteLine($"k={tempB(0)}")
                                    'recordDataFile.WriteLine($"tempi={TempI}")
                                    'recordDataFile.WriteLine($"ReceiveData={ReceiveData(i, Int(j / 32))}-ReceiveDataH-{ReceiveData(i, Int(j / 32))}")

                                    'recordDataFile.WriteLine(tmpstr)




                                    'End If


                                End If
                                If InteractMode = 1 Or InteractMode = 2 Then '普通模式 和 踩踏模式

                                    For l As Integer = 0 To 4 - 1
                                        'screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = bytes(index + k * 4 + l) And &H80

                                        If InteractMode = 1 Then '互动 普通模式
                                            TempI = (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 2) And &H80) + (bytes(j + 3 + 3) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) + (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 14) And &H80) + (bytes(j + 3 + 15) And &H80) + (bytes(j + 3 + 16) And &H80)
                                            ' If ReceiveData(i, Int(j / 32)) <> ReceiveDataH(i, Int(j / 32)) Then '刷新其他显示数据
                                            'If recordDataFlage = True Then recordDataFile.WriteLine($"序号-数据-={k * 4 + l}-{bytes(j + 4 + k * 4 + l)}-temp-{TempI}-ReceiveData-{ReceiveData(i, Int(j / 32)) }-ReceiveDataH-{ReceiveDataH(i, Int(j / 32)) }")
                                            'screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))
                                            'If k * 4 + l = 15 Then ReceiveDataH(i, Int(j / 32)) = ReceiveData(i, Int(j / 32))
                                            'Continue For
                                            'End If


                                            If (bytes(j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                                If runMode = 1 Or runMode = 4 Then
                                                    screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))
                                                End If
                                                screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = 0
                                                Continue For
                                            End If

                                            If screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                                screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                                Continue For
                                            End If

                                            screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                            screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))




                                        End If

                                        If InteractMode = 2 Then '互动 地砖1对1模式

                                            ' Dim TempI As Integer = 0
                                            Select Case k * 4 + l
                                                Case 0 '第1个线圈
                                                    TempI = (bytes(j + 3 + 1) And &H80) +
                                                    (bytes(j + 3 + 2) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80)
                                                Case 1 '第2个线圈
                                                    TempI = (bytes(j + 3 + 2) And &H80) +
                                                    (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 3) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 7) And &H80)

                                                Case 2 '第3个线圈
                                                    TempI = (bytes(j + 3 + 3) And &H80) +
                                                    (bytes(j + 3 + 2) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80)

                                                Case 3 '第4个线圈
                                                    TempI = (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 3) And &H80) +
                                                    (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80)

                                                Case 4 '第5个线圈
                                                    TempI = (bytes(j + 3 + 5) And &H80) +
                                                    (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 2) And &H80) +
                                                    (bytes(j + 3 + 6) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80)
                                                Case 5 '第6个线圈
                                                    TempI = (bytes(j + 3 + 6) And &H80) +
                                                    (bytes(j + 3 + 1) And &H80) + (bytes(j + 3 + 2) And &H80) + (bytes(j + 3 + 3) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 7) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) + (bytes(j + 3 + 11) And &H80)

                                                Case 6 '第7个线圈
                                                    TempI = (bytes(j + 3 + 7) And &H80) +
                                                    (bytes(j + 3 + 2) And &H80) + (bytes(j + 3 + 3) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 8) And &H80) +
                                                    (bytes(j + 3 + 10) And &H80) + (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80)
                                                Case 7 '第8个线圈
                                                    TempI = (bytes(j + 3 + 8) And &H80) +
                                                    (bytes(j + 3 + 3) And &H80) + (bytes(j + 3 + 4) And &H80) +
                                                    (bytes(j + 3 + 7) And &H80) +
                                                    (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80)

                                                Case 8 '第9个线圈
                                                    TempI = (bytes(j + 3 + 9) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80) +
                                                    (bytes(j + 3 + 10) And &H80) +
                                                    (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 14) And &H80)

                                                Case 9 '第10个线圈
                                                    TempI = (bytes(j + 3 + 10) And &H80) +
                                                    (bytes(j + 3 + 5) And &H80) + (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 7) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 11) And &H80) +
                                                    (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 14) And &H80) + (bytes(j + 3 + 15) And &H80)
                                                Case 10 '第11个线圈
                                                    TempI = (bytes(j + 3 + 11) And &H80) +
                                                    (bytes(j + 3 + 6) And &H80) + (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80) +
                                                    (bytes(j + 3 + 10) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 14) And &H80) + (bytes(j + 3 + 15) And &H80) + (bytes(j + 3 + 16) And &H80)
                                                Case 11 '第12个线圈
                                                    TempI = (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 7) And &H80) + (bytes(j + 3 + 8) And &H80) +
                                                    (bytes(j + 3 + 11) And &H80) +
                                                    (bytes(j + 3 + 15) And &H80) + (bytes(j + 3 + 16) And &H80)
                                                Case 12 '第13个线圈
                                                    TempI = (bytes(j + 3 + 13) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) +
                                                    (bytes(j + 3 + 14) And &H80)
                                                Case 13 '第14个线圈
                                                    TempI = (bytes(j + 3 + 14) And &H80) +
                                                    (bytes(j + 3 + 9) And &H80) + (bytes(j + 3 + 10) And &H80) + +(bytes(j + 3 + 11) And &H80) +
                                                    (bytes(j + 3 + 13) And &H80) + (bytes(j + 3 + 15) And &H80)
                                                Case 14 '第15个线圈
                                                    TempI = (bytes(j + 3 + 15) And &H80) +
                                                    (bytes(j + 3 + 10) And &H80) + (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 14) And &H80) + (bytes(j + 3 + 16) And &H80)
                                                Case 15 '第16个线圈
                                                    TempI = (bytes(j + 3 + 16) And &H80) +
                                                    (bytes(j + 3 + 11) And &H80) + (bytes(j + 3 + 12) And &H80) +
                                                    (bytes(j + 3 + 15) And &H80)
                                            End Select
                                            ' If TempI > 128 Then
                                            If TempI >= tempimax And (bytes(j + 4 + k * 4 + l) And &H80) And TempI >= 128 Then ' 感应两个以上线圈 进入触发模式
                                                tempimax = TempI
                                                bytemax(0) = bytes(j + 4 + k * 4 + l)
                                                kmax = k
                                                lmax = l
                                            End If
                                            ' If recordDataFlage = True Then recordDataFile.WriteLine($"序号-数据-={k * 4 + l}-{bytes(j + 4 + k * 4 + l)}-temp-{TempI}")
                                            If runMode = 1 Or runMode = 2 Or runMode = 3 Or runMode = 4 Then '刷新其他显示数据   And ReceiveData(i, Int(j / 32)) <> ReceiveDataH(i, Int(j / 32))

                                                ' screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))
                                                ' If k * 4 + l = 15 Then ReceiveDataH(i, Int(j / 32)) = ReceiveData(i, Int(j / 32))
                                                ' Continue For

                                                If (bytes(j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                                    If runMode = 1 Or runMode = 4 Then
                                                        screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))
                                                    End If
                                                    screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = 0
                                                    Continue For
                                                End If
                                                If screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                                    screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                                    Continue For
                                                End If
                                                screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                                screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + l, tmp.Y + k, bytes(j + 4 + k * 4 + l))




                                            End If


                                            'If recordDataFlage = True Then recordDataFile.WriteLine($"序号-数据-={k * 4 + l}-{bytes(j + 4 + k * 4 + l)}")
                                            If (k * 4 + l) = 15 And (runMode = 0) Then 'And (bytemax(0) And &H80)Or runMode = 4
                                                If recordDataFlage = True Then recordDataFile.WriteLine($"序号-数据-={k * 4 + l}-{bytes(j + 4 + k * 4 + l)}-temp-{TempI}")
                                                TempI = tempimax
                                                ScreenM2click = bytes(1) * 100000 + (bytes(2) * 256 + bytes(3)) * 100 + kmax * 4 + lmax '上一个数据流，点击感应线圈记录
                                                If recordDataFlage = True Then recordDataFile.WriteLine($"上次点击-数据-={ScreenM2click}")
                                                If (TempI > 128) And ScreenM2click <> ScreenM2clickH Then '上次点击数据记录
                                                    ScreenM2clickH = ScreenM2click
                                                    'ReceiveDataH = ReceiveData
                                                    screenMain(tmp.ScreenIndex).clickHistoryArray(tmp.Y + kmax, tmp.X + lmax) = &H80

                                                    screenMain(tmp.ScreenIndex).playDialog.MousesimulationClick(tmp.X + lmax, tmp.Y + kmax, bytemax(0))
                                                    TempI = 0
                                                    Continue For
                                                End If

                                            End If
                                            ' If k * 4 + l = 15 Then ReceiveDataH(i, Int(j / 32)) = ReceiveData(i, Int(j / 32))
                                            ' End If
                                            '有感应信号记录数据

                                            '有感应信号记录数据
                                            ' If (bytes(j + 4 + k * 4 + l) And &H80) And recordDataFlage Then 'And (TempI2 > 128)
                                            'tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                            '读取数据段
                                            ' For m As Integer = 4 To 27
                                            'tmpstr = tmpstr & bytes(j + m) & " "
                                            'Next
                                            'recordDataFile.WriteLine($"InteractMode={InteractMode}")
                                            'recordDataFile.WriteLine($"bytes={j + 4 + k * 4 + l}-{ k * 4 + l}-{bytes(j + 4 + k * 4 + l)｝")
                                            'recordDataFile.WriteLine($"TempI={TempI}")
                                            'recordDataFile.WriteLine(tmpstr)
                                            'End If


                                        End If


                                        'tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                        ''读取数据段
                                        'For m As Integer = 4 To 27
                                        '    tmpstr = tmpstr & bytes(j + m).ToString("X") & " "
                                        'Next

                                        'showstr = showstr & tmpstr & vbCrLf
                                    Next l
                                End If

                            Next k

                            If recordDataFlage Then 'And (TempI2 > 128) recordDataFlage 取消记录数据
                                tmpstr = $"{tmp.ScreenIndex}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                                '读取数据段
                                For m As Integer = 4 To 27
                                    tmpstr = tmpstr & bytes(j + m) & " "
                                Next
                                'recordDataFile.WriteLine(TempI2)
                                ' recordDataFile.WriteLine(TempI)
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
                    If recordDataFlage = True Then
                        recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-异常-exceptionNums-{exceptionNums}")
                        recordDataFile.WriteLine($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 02:{ex.Message}")

                        Dim ipStr As String = "192.168.11.2"
                        If My.Computer.Network.Ping(ipStr, 500) = False Then
                            ' MsgBox(ipStr & $"{If(selectLanguageId = 0, " 未能连通", " Failed to connect")}",
                            'MsgBoxStyle.Information,
                            '  $"{If(selectLanguageId = 0, "连接", "Connect")}")
                            recordDataFile.WriteLine(ipStr & "未能连通")
                            'Exit Su
                        Else
                            recordDataFile.WriteLine(ipStr & "已连通")
                        End If

                    End If
                    putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 02:{ex.Message}")
                    Continue For
                End Try
            Next

            Thread.Sleep(checkTime)
        Loop
    End Sub

    ''' <summary>
    ''' 点击
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem2_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem2.Click
        runMode = 0
        ToolStripMenuItem1.Text = ToolStripMenuItem2.Text
        ToolStripMenuItem1.Image = ToolStripMenuItem2.Image

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchPlayMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem8_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem8.Click
        ToolStripMenuItem2_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 测试
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem4_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem4.Click
        runMode = 1
        ToolStripMenuItem1.Text = ToolStripMenuItem4.Text
        ToolStripMenuItem1.Image = ToolStripMenuItem4.Image

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchTestMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem10_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem10.Click
        ToolStripMenuItem4_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 黑屏
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem6_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem6.Click
        runMode = 2
        ToolStripMenuItem1.Text = ToolStripMenuItem6.Text
        ToolStripMenuItem1.Image = ToolStripMenuItem6.Image

        For i As Integer = 0 To screenMain.Length - 1
            If screenMain(i).showFlage Then
                screenMain(i).playDialog.switchBlankScreenMode("")
            End If
        Next
    End Sub
    Private Sub ToolStripMenuItem12_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem12.Click
        ToolStripMenuItem6_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 忽略
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem7_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem7.Click
        runMode = 3
        ToolStripMenuItem1.Text = ToolStripMenuItem7.Text
        ToolStripMenuItem1.Image = ToolStripMenuItem7.Image
    End Sub
    Private Sub ToolStripMenuItem13_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem13.Click
        ToolStripMenuItem7_Click(Nothing, Nothing)
    End Sub

    ''' <summary>
    ''' 测试(电容)
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripMenuItem5_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem5.Click
        runMode = 4
        ToolStripMenuItem1.Text = ToolStripMenuItem5.Text
        ToolStripMenuItem1.Image = ToolStripMenuItem5.Image

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

    ''' <summary>
    ''' 清空播放列表
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        systeminfo.filesList.Clear()
        ComboBox2.Items.Clear()
    End Sub

    ''' <summary>
    ''' 记录数据
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If ToolStripButton2.Text = "记录数据" Or ToolStripButton2.Text = "Save Data" Then
            recordDataFile = New StreamWriter($"DEBUG{Format(Now(), "yyyyMMddHHmmss")}.txt", True)

            recordDataFlage = True

            ToolStripButton2.Text = If(selectLanguageId = 0, "停止记录", "Stop Save")
            ToolStripButton2.Image = My.Resources.disconnect
        Else
            recordDataFile.Close()

            recordDataFlage = False

            ToolStripButton2.Text = If(selectLanguageId = 0, "记录数据", "Save Data")
            ToolStripButton2.Image = My.Resources.connect
        End If
    End Sub

    Private Sub 灵敏度调节ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 灵敏度调节ToolStripMenuItem.Click
        Dim tmpDialog As New FormTouchSetting

        tmpDialog.RadioButton2.Visible = ToolStripSeparator3.Visible
        tmpDialog.GroupBox4.Visible = ToolStripSeparator3.Visible
        If InteractMode = 1 Then tmpDialog.RadioButton1.Checked = True
        If InteractMode = 2 Then tmpDialog.RadioButton2.Checked = True
        If InteractMode = 3 Then tmpDialog.RadioButton3.Checked = True
        If ScreenSensitivity > 9 Or ScreenSensitivity < 1 Then
            tmpDialog.NumericUpDown1.Value = 1
        Else
            tmpDialog.NumericUpDown1.Value = ScreenSensitivity
        End If
        If ScreenAntiS > 9 Or ScreenAntiS < 1 Then
            tmpDialog.NumericUpDown2.Value = 1
        Else
            tmpDialog.NumericUpDown2.Value = ScreenAntiS
        End If
        If resetType = 1 Then
            tmpDialog.RadioButton5.Checked = True

        Else
            tmpDialog.RadioButton4.Checked = True
        End If

        'tmpDialog.ShowDialog()

        If tmpDialog.ShowDialog() = DialogResult.OK Then
            For i As Integer = 0 To screenMain.Length - 1

                If screenMain(i).showFlage Then
                    screenMain(i).playDialog.closeDialog("")
                    screenMain(i).playDialog = Nothing
                End If
            Next
            updateScreen()
        End If
        Dim p As Integer
        p = resetType

    End Sub

    ''' <summary>
    ''' 切换为中文
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub 中文ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 中文ToolStripMenuItem.Click
        selectLanguageId = 0

        changeLanguage()
    End Sub

    ''' <summary>
    ''' 切换为英文
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub EnglishToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnglishToolStripMenuItem.Click
        selectLanguageId = 1

        changeLanguage()
    End Sub

    ''' <summary>
    ''' 更改语言 0:中文 1:English
    ''' </summary>
    Public Sub changeLanguage()
        Me.文件ToolStripMenuItem.Text = If(selectLanguageId = 0, "文件(&F)", "File(&F)")
        Me.新建ToolStripMenuItem.Text = If(selectLanguageId = 0, "新建(&N)", "New(&N)")
        Me.打开ToolStripMenuItem.Text = If(selectLanguageId = 0, "打开(&O)", "Open(&O)")
        Me.保存ToolStripMenuItem.Text = If(selectLanguageId = 0, "保存(&S)", "Save(&S)")
        Me.另存为ToolStripMenuItem.Text = If(selectLanguageId = 0, "另存为(&A)", "Save As(&A)")
        Me.最近的文件ToolStripMenuItem.Text = If(selectLanguageId = 0, "最近的文件(&R)", "Recent Files(&R)")
        Me.清空历史ToolStripMenuItem.Text = If(selectLanguageId = 0, "清除历史记录(&H)", "Clear History(&H)")
        Me.退出ToolStripMenuItem.Text = If(selectLanguageId = 0, "退出(&X)", "Exit(&X)")
        Me.控制CToolStripMenuItem.Text = If(selectLanguageId = 0, "控制(&C)", "Control(&C)")
        Me.屏幕模式SToolStripMenuItem.Text = If(selectLanguageId = 0, "屏幕模式(&S)", "Screen Mode(&S)")

        Me.ToolStripMenuItem8.Text = If(selectLanguageId = 0, "运行(&F1)", "Run(&F1)")
        Me.ToolStripMenuItem10.Text = If(selectLanguageId = 0, "测试(&F2)", "Test(&F2)")
        Me.ToolStripMenuItem12.Text = If(selectLanguageId = 0, "黑屏(&F3)", "Blank Screen(&F3)")
        Me.ToolStripMenuItem13.Text = If(selectLanguageId = 0, "忽略(&F4)", "Disabled(&F4)")
        Me.ToolStripMenuItem11.Text = If(selectLanguageId = 0, "测试(电容)(&F5)", "Test(Debug)(&F5)")
        Me.ToolStripMenuItem2.Text = Me.ToolStripMenuItem8.Text
        Me.ToolStripMenuItem4.Text = Me.ToolStripMenuItem10.Text
        Me.ToolStripMenuItem6.Text = Me.ToolStripMenuItem12.Text
        Me.ToolStripMenuItem7.Text = Me.ToolStripMenuItem13.Text
        Me.ToolStripMenuItem5.Text = Me.ToolStripMenuItem11.Text

        Select Case runMode
            Case 0
                Me.ToolStripMenuItem1.Text = Me.ToolStripMenuItem2.Text
            Case 1
                Me.ToolStripMenuItem1.Text = Me.ToolStripMenuItem4.Text
            Case 2
                Me.ToolStripMenuItem1.Text = Me.ToolStripMenuItem6.Text
            Case 3
                Me.ToolStripMenuItem1.Text = Me.ToolStripMenuItem7.Text
            Case 4
                Me.ToolStripMenuItem1.Text = Me.ToolStripMenuItem5.Text
        End Select

        Me.查询间隔ToolStripMenuItem.Text = If(selectLanguageId = 0, "查询间隔(ms)(&C)", "Update Interval(ms)(&C)")
        Me.工具TToolStripMenuItem.Text = If(selectLanguageId = 0, "工具(&T)", "Tool(&T)")
        Me.屏幕设置ToolStripMenuItem.Text = If(selectLanguageId = 0, "屏幕设置(&S)", "Screen Setting(&S)")
        Me.控制器设置ToolStripMenuItem.Text = If(selectLanguageId = 0, "控制器设置(&C)", "Control Setting(&C)")
        Me.灵敏度调节ToolStripMenuItem.Text = If(selectLanguageId = 0, "灵敏度调节(&T)", "Sensitivity Setting(&T)")
        Me.版本检测ToolStripMenuItem.Text = If(selectLanguageId = 0, "版本检测(&V)", "Version Check(&V)")
        Me.语言选择LToolStripMenuItem.Text = If(selectLanguageId = 0, "语言选择(&L)", "Language(&L)")
        Me.帮助HToolStripMenuItem.Text = If(selectLanguageId = 0, "帮助(&H)", "Help(&H)")
        Me.技术支持TToolStripMenuItem.Text = If(selectLanguageId = 0, "技术支持(&T)", "Technical Support(&T)")
        Me.关于ToolStripMenuItem.Text = If(selectLanguageId = 0, "关于 ME触摸地砖屏控制系统(&A)", "About ME触摸地砖屏控制系统(&A)")

        If Me.ToolStripButton1.Text = "连接控制器" Or Me.ToolStripButton1.Text = "Connect Screen" Then
            Me.ToolStripButton1.Text = If(selectLanguageId = 0, "连接控制器", "Connect Screen")
        Else
            Me.ToolStripButton1.Text = If(selectLanguageId = 0, "断开连接", "Disconnect Screen")
        End If

        If Me.ToolStripButton2.Text = "记录数据" Or Me.ToolStripButton2.Text = "Save Data" Then
            Me.ToolStripButton2.Text = If(selectLanguageId = 0, "记录数据", "Save Data")
        Else
            Me.ToolStripButton2.Text = If(selectLanguageId = 0, "停止记录", "Stop Save")
        End If

        Me.GroupBox1.Text = If(selectLanguageId = 0, "播放信息", "Play Message")
        Me.GroupBox2.Text = If(selectLanguageId = 0, "播放控制", "Play Control")
        Me.Button5.Text = If(selectLanguageId = 0, "清空列表", "Clear List")
        Me.Button4.Text = If(selectLanguageId = 0, "播放所有屏幕", "Play to all screens")
        Me.Label2.Text = If(selectLanguageId = 0, "文件", "File")
        Me.Button3.Text = If(selectLanguageId = 0, "删除文件", "Del File")
        Me.Button2.Text = If(selectLanguageId = 0, "播放", "Play")
        Me.Button1.Text = If(selectLanguageId = 0, "添加文件", "Add")
        Me.Label1.Text = If(selectLanguageId = 0, "屏幕", "Screen")

        Me.ListView1.Columns(0).Text = If(selectLanguageId = 0, "屏幕", "Screen")
        Me.ListView1.Columns(1).Text = If(selectLanguageId = 0, "备注", "remark")
        Me.ListView1.Columns(2).Text = If(selectLanguageId = 0, "播放文件", "playing")

        Me.ToolStripStatusLabel1.Text = $"{If(selectLanguageId = 0, "查询次数:未连接", "Update:Disconnect")}"
        Me.ToolStripStatusLabel2.Text = $"| {If(selectLanguageId = 0, "屏幕数", "Screen Nums")}：{screenMain.Length} | {If(selectLanguageId = 0, "控制器数", "Control Nums")}：{senderArray.Length}"
    End Sub

    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim sendByte(4 - 1) As Byte
        Dim sendstr As String = "aadb0101" '强制复位
        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        'For i As Integer = 0 To senderArray.Length - 1
        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        'Dim sendstr As String = "aadb0305"
        Dim sendByte(5 - 1) As Byte
        Dim sendstr As String = "aadb0305" '强制复位
        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(5) = ScreenSensitivity

        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
    End Sub

    Private Sub ToolStripButton5_Click(sender As Object, e As EventArgs) Handles ToolStripButton5.Click
        灵敏度调节ToolStripMenuItem_Click(Nothing, Nothing)
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        '硬复位则关闭定时器
        If resetType Or (ToolStripButton1.Text = "连接控制器" Or ToolStripButton1.Text = "Connect Screen") Then
            Timer1.Enabled = False
            Exit Sub
        End If

        Static runMin As Integer = 0
        Static runMin2 As Integer = 0
        'Static lastTemp As Integer = 0
        runMin += 1
        runMin2 += 1
        'ResetTemp
        'ResetTimeSec

        If ResetTimeSec Then
            If runMin >= ResetTimeSec Then
                runMin = 0
                '定时复位
                Dim sendByte(4 - 1) As Byte
                Dim sendstr As String = "aadb0101" '强制复位
                For i As Integer = 0 To sendByte.Length - 1
                    sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
                Next
                mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
            End If
        Else
            '温度复位
            '刷新数据
            runMin = 0
        End If
        If runMin2 > 30 Then
            runMin2 = 0
            If ResetTemp > 0 Then mainClass.BeginRefreshHardwareStatus()
        End If

    End Sub

    'Public Sub Delay(ByRef Interval As Double)  'Interval单位为毫秒
    '    Dim time As DateTime = DateTime.Now
    '    Dim Span As Double = Interval * 10000
    '    While ((DateTime.Now.Ticks - time.Ticks) < Span)
    '        Application.DoEvents()
    '    End While
    'End Sub

    ''' <summary>
    ''' 刷新事件
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RefreshHardwareStatusFinishEvent(sender As Object, e As RefreshResultEventArgs)
        'Static Dim senderArrayIndex As Integer = 0
        Static laseTemp As Double = 0

        If e.bFinishSucceed Then
            '刷新成功
            Dim tmp = ScanBoardTable.Item($"0-0-0")
            If tmp Is Nothing Then
                Exit Sub
            End If

            Dim qwe As Nova.Mars.SDK.ValueInfo
            If mainClass.GetScanBoardTemperature(tmp.ScreenIndex, tmp.linkIndex, qwe) = False Then
                Exit Sub
            End If

            If qwe.IsValid = False Then
                Exit Sub
            End If

            '未超过设定温度则退出
            If Math.Abs(qwe.Value - laseTemp) <= ResetTemp Then
                'laseTemp = qwe.Value
                Exit Sub
            End If
            laseTemp = qwe.Value

            Dim sendByte(4 - 1) As Byte
            Dim sendstr As String = "aadb0101" '强制复位
            For i As Integer = 0 To sendByte.Length - 1
                sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
            Next
            mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            '刷新失败
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs)
        mainClass.BeginRefreshHardwareStatus()
    End Sub
End Class
