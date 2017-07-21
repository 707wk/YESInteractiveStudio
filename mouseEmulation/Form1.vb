Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
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

    Dim rootClass As MarsHardwareEnumerator
    Dim mainClass As MarsControlSystem
    Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If System.IO.File.Exists("setting.ini") = False Then
            MsgBox("未找到配置文件!", MsgBoxStyle.Information, "初始化")
            Application.Exit()
            Exit Sub
        End If

        '临时使用
        Me.Width = ListView5.Location.X + ListView5.Width + 20
        Me.Height = ListView5.Location.Y + ListView5.Height + 70

        '读取ini配置文件
        Dim tmp As New ClassIni
        checkTime = CInt(tmp.GetINI("SYS", "Time", "", ".\setting.ini"))

        '清空日志文件
        Dim sw As StreamWriter = New StreamWriter（"log.txt", False)
        sw.Close（）

        '显示配置
        Timer1.Interval = checkTime
        ToolStripTextBox1.Text = $"{checkTime}"

        ToolStripButton1.Enabled = False

        '设置控制系统列表格式
        ListView1.View = View.Details
        ListView1.GridLines = True
        ListView1.FullRowSelect = True
        ListView1.CheckBoxes = False
        ListView1.Clear()
        ListView1.Columns.Add("序号", 36, HorizontalAlignment.Left)
        ListView1.Columns.Add("串口", 54, HorizontalAlignment.Center)
        ListView1.Columns.Add("显示屏数", 72, HorizontalAlignment.Center)
        ListView1.Columns.Add("设备数", 54, HorizontalAlignment.Center)

        '设置显示屏列表格式
        ListView2.View = View.Details
        ListView2.GridLines = True
        ListView2.FullRowSelect = True
        ListView2.CheckBoxes = True
        ListView2.Clear()
        ListView2.Columns.Add("屏幕编号", 72, HorizontalAlignment.Left)
        ListView2.Columns.Add("信息", 200, HorizontalAlignment.Center)

        '设置接收卡列表格式
        ListView3.View = View.Details
        ListView3.GridLines = True
        ListView3.FullRowSelect = True
        ListView3.CheckBoxes = False
        ListView3.Clear()
        ListView3.Columns.Add("控制器索引", 72, HorizontalAlignment.Left)
        ListView3.Columns.Add("网口索引", 72, HorizontalAlignment.Center)
        ListView3.Columns.Add("连接序号", 72, HorizontalAlignment.Center)
        ListView3.Columns.Add("信息", 150, HorizontalAlignment.Center)

        '设置控制器列表格式
        ListView4.View = View.Details
        ListView4.GridLines = True
        ListView4.FullRowSelect = True
        ListView4.CheckBoxes = False
        ListView4.Clear()
        ListView4.Columns.Add("控制器索引", 72, HorizontalAlignment.Left)
        ListView4.Columns.Add("IP", 100, HorizontalAlignment.Center)

        '设置swf列表格式
        ListView5.View = View.Details
        ListView5.GridLines = True
        ListView5.FullRowSelect = True
        ListView5.CheckBoxes = False
        ListView5.Clear()
        ListView5.Columns.Add("FLASH文件", 200, HorizontalAlignment.Left)
        ListView5.Columns.Add("路径", 500, HorizontalAlignment.Left)

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)
        RegisterHotKey(Me.Handle.ToInt32, 5, 0, Keys.F5)
        RegisterHotKey(Me.Handle.ToInt32, 6, 0, Keys.F6)
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        'Dim tmp As New FormShowInfo
        'tmp.setInfo("加载中")
        'tmp.TopMost = True
        'tmp.Show()

        rootClass = New MarsHardwareEnumerator

        If rootClass.Initialize() Then
            'TextBox1.AppendText($"连接Nova服务成功{vbCrLf}")
        Else
            MsgBox($"连接Nova服务失败")
            'Thread.Sleep(1000)
            Application.Exit()
            Exit Sub
        End If

        Dim SystemCount As Integer = rootClass.CtrlSystemCount()
        If SystemCount Then
            'TextBox1.AppendText($"控制系统数:{rootClass.CtrlSystemCount()}{vbCrLf}")
        Else
            MsgBox($"未找到控制系统")
            'Thread.Sleep(1000)
            Application.Exit()
            Exit Sub
        End If

        mainClass = New MarsControlSystem(rootClass)

        AddHandler mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        'Dim screenCount As Integer
        'Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        'For i As Integer = 1 To SystemCount
        '    tmpstr = Nothing
        '    screenCount = 0
        '    senderCount = 0

        '    Dim itm As ListViewItem = ListView1.Items.Add(i.ToString, 0)

        rootClass.GetComNameOfControlSystem(0, tmpstr)
        '    itm.SubItems.Add(tmpstr)

        '    mainClass.Initialize(tmpstr, screenCount, senderCount)
        '    mainClass.UnInitialize()
        '    itm.SubItems.Add(screenCount)
        '    itm.SubItems.Add(senderCount)
        'Next

        mainClass.Initialize(tmpstr, vbNull, vbNull)

        'Dim sendByte As Byte()
        'ReDim sendByte(16 - 1)
        'For i As Integer = 0 To 16 - 1
        '    sendByte(i) = i + i * &H10
        'Next
        'MsgBox($"发送：{mainClass.SetScanBoardData(0, 0, 0, sendByte)}")

        'Thread.Sleep(1)

        'Dim recByte As Byte() = Nothing
        'Dim qwe = mainClass.GetScanBoardData(0, 0, 0, 16, recByte)
        'Dim asd As String = Nothing
        'For i As Integer = 0 To recByte.Length - 1
        '    asd = asd & $"{recByte(i).ToString("X")} "
        'Next

        'MsgBox($"{qwe} {asd}")

        ListView2.Items.Clear()

        If mainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
            MsgBox($"读显示屏信息失败")
            'Thread.Sleep(1000)
            Application.Exit()
            Exit Sub
        End If


        '获取到的显示屏 X 偏移
        Dim x As Integer
        '获取到的显示屏 Y 偏移
        Dim y As Integer
        '获取到的显示屏宽度
        Dim width As Integer
        '获取到的显示屏高度
        Dim height As Integer
        '获取到的接收卡个数
        'Dim scanBdCount As Integer

        For i As Integer = 0 To LEDScreenInfoList.Count - 1
            Dim itm As ListViewItem = ListView2.Items.Add($"{i}", 0)

            mainClass.GetScreenLocation(i, x, y, width, height)

            itm.SubItems.Add($"起始点:{x},{y} 宽:{width} 高:{height}")

            'mainClass.GetScanBoardCount(i, scanBdCount)
            'itm.SubItems.Add(scanBdCount)
        Next

        'tmp.setInfo($"加载完成")
        'Thread.Sleep(1000)
        'tmp.Close()

    End Sub

    '窗体的消息处理函数
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY And ToolStripButton1.Enabled = True Then '判断是否为热键消息
            'Me.Text = m.WParam.ToInt32
            Select Case m.WParam.ToInt32 '判断热键消息的注册ID
                Case 1
                    模拟点击ToolStripMenuItem_Click(Nothing, Nothing)
                Case 2
                    点击捕获鼠标F2ToolStripMenuItem_Click(Nothing, Nothing)
                Case 3
                    测试ToolStripMenuItem_Click(Nothing, Nothing)
                Case 4
                    测试显示电容F4ToolStripMenuItem_Click(Nothing, Nothing)
                Case 5
                    黑屏ToolStripMenuItem_Click(Nothing, Nothing)
                Case 6
                    忽略ToolStripMenuItem_Click(Nothing, Nothing)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F5)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F6)

        Try
            mainClass.UnInitialize()
        Catch ex As Exception
        End Try

        Try
            rootClass.UnInitialize()
        Catch ex As Exception
        End Try
    End Sub

    Public Delegate Sub showipCallback(ByVal recData As Byte())
    Public Sub showip(ByVal recData As Byte())
        If Me.InvokeRequired Then
            Dim d As New showipCallback(AddressOf showip)
            Me.Invoke(d, New Object() {recData})
        Else
            Dim itm As ListViewItem = ListView4.Items.Add($"{SenderIndexlist(getSenderIndex)}", 0)
            itm.SubItems.Add($"{recData(3)}.{recData(2)}.{recData(1)}.{recData(0)}{vbCrLf}")
        End If
    End Sub

    Public Delegate Sub showListView2Callback(ByVal text As String)
    Public Sub showListView2(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New showListView2Callback(AddressOf showListView2)
            Me.Invoke(d, New Object() {text})
        Else
            ToolStripButton1.Enabled = True
            ListView2.Enabled = True
        End If
    End Sub

    Private Sub GetEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        'TextBox1.AppendText($"{e.Data(3)}.{e.Data(2)}.{e.Data(1)}.{e.Data(0)}{vbCrLf}")
        'TextBox1.AppendText($"{e.Data(7)}.{e.Data(6)}.{e.Data(5)}.{e.Data(4)}{vbCrLf}")
        'TextBox1.AppendText($"{e.Data(11)}.{e.Data(10)}.{e.Data(9)}.{e.Data(8)}{vbCrLf}")
        If e.IsExecResult Then
            showip(e.Data)
            Dim qwe As Integer = SenderIndexlist(getSenderIndex)
            senderArray(senderArrayIndex).index = qwe 'SenderIndexlist(getSenderIndex)
            senderArray(senderArrayIndex).ipDate = e.Data
            senderArrayIndex += 1

            If getSenderIndex < SenderIndexlist.Count - 1 Then
                getSenderIndex += 1
                mainClass.GetEquipmentIP(SenderIndexlist(getSenderIndex))
            Else
                'For i As Integer = 0 To senderArray.Count - 1
                '    putlog($"索引:{senderArray(i).index}")
                '    putlog($"ip:{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}")
                '    putlog($"掩码:{senderArray(i).ipDate(7)}.{senderArray(i).ipDate(6)}.{senderArray(i).ipDate(5)}.{senderArray(i).ipDate(4)}")
                '    putlog($"网关:{senderArray(i).ipDate(11)}.{senderArray(i).ipDate(10)}.{senderArray(i).ipDate(9)}.{senderArray(i).ipDate(8)}")
                'Next

                showListView2("")
            End If
        End If
    End Sub

    ''显示控制器列表
    'Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
    '    If ListView1.SelectedItems.Count = 0 Then
    '        Exit Sub
    '    End If

    '    mainClass.Initialize(ListView1.SelectedItems(0).SubItems(1).Text, vbNull, vbNull)

    '    ListView1.Enabled = False
    '    ListView2.Items.Clear()

    '    If mainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
    '        Exit Sub
    '    End If

    '    '获取到的显示屏 X 偏移
    '    Dim x As Integer
    '    '获取到的显示屏 Y 偏移
    '    Dim y As Integer
    '    '获取到的显示屏宽度
    '    Dim width As Integer
    '    '获取到的显示屏高度
    '    Dim height As Integer
    '    '获取到的接收卡个数
    '    'Dim scanBdCount As Integer

    '    For i As Integer = 0 To LEDScreenInfoList.Count - 1
    '        Dim itm As ListViewItem = ListView2.Items.Add($"{i}", 0)

    '        mainClass.GetScreenLocation(i, x, y, width, height)

    '        itm.SubItems.Add($"起始点:{x},{y} 宽:{width} 高:{height}")

    '        'mainClass.GetScanBoardCount(i, scanBdCount)
    '        'itm.SubItems.Add(scanBdCount)
    '    Next

    '    ListView1.Enabled = True
    'End Sub

    'mmp的NovaMarsSDK
    Dim SenderIndexlist As HashSet(Of Byte)
    Dim getSenderIndex As Integer = 0
    Dim senderArrayIndex As Integer = 0
    '显示接收卡列表
    Private Sub ListView2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView2.SelectedIndexChanged
        If ListView2.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        SenderIndexlist = New HashSet(Of Byte)
        getSenderIndex = 0
        senderArrayIndex = 0

        Dim LEDScreenIndex As Integer = CInt(ListView2.SelectedItems(0).SubItems(0).Text)

        '屏幕索引
        screenMain.index = LEDScreenIndex
        '获取起始位置 大小
        mainClass.GetScreenLocation(LEDScreenIndex, screenMain.x, screenMain.y, screenMain.width, screenMain.height)
        '带载宽度
        screenMain.ScanBoardWidth = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Width
        '带载高度
        screenMain.ScanBoardHeight = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Height
        '接收卡索引表
        screenMain.ScanBoardTable = New Hashtable
        '显示区域
        If screenMain.showFlash IsNot Nothing Then
            screenMain.showFlash.Close()
        End If

        screenMain.showFlash = New FormPlayFlash

        screenMain.showFlash.touchPieceWidth = screenMain.ScanBoardWidth \ 4
        screenMain.showFlash.touchPieceHeight = screenMain.ScanBoardHeight \ 4

        'putlog($"screenMain{screenMain.index} {screenMain.x} {screenMain.y} {screenMain.width} {screenMain.height}")

        screenMain.showFlash.setLocation(screenMain.x, screenMain.y, screenMain.width, screenMain.height)
        screenMain.showFlash.Show()

        ListView2.Enabled = False
        ListView3.Items.Clear()
        'MsgBox(LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList.Count)
        For Each i In LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList
            SenderIndexlist.Add(i.SenderIndex)

            Dim itm As ListViewItem = ListView3.Items.Add($"{i.SenderIndex}", 0)
            itm.SubItems.Add($"{i.PortIndex}")
            itm.SubItems.Add($"{i.ConnectIndex}")
            itm.SubItems.Add($"{i.X},{i.Y} [{i.Width},{i.Height}]")

            Dim tmpScanBoardInfo As ScanBoardInfo
            tmpScanBoardInfo.SenderIndex = i.SenderIndex
            tmpScanBoardInfo.PortIndex = i.PortIndex
            tmpScanBoardInfo.ConnectIndex = i.ConnectIndex
            tmpScanBoardInfo.X = i.X
            tmpScanBoardInfo.Y = i.Y

            screenMain.ScanBoardTable.Add($"{i.SenderIndex}-{i.PortIndex}-{i.ConnectIndex}", tmpScanBoardInfo)
            'TextBox1.AppendText( 连接序号:{i.ConnectIndex} X偏移:{i.X} Y偏移:{i.Y} 带载宽度:{i.Width} 带载高度:{i.Height}{vbCrLf}")
        Next

        'For Each i In screenMain.ScanBoardTable.Keys
        '    Dim tmp As ScanBoardInfo = screenMain.ScanBoardTable.Item(i)
        '    putlog($"Key = {i}")
        '    putlog($"SenderIndex = {tmp.SenderIndex}")
        '    putlog($"PortIndex = {tmp.PortIndex}")
        '    putlog($"ConnectIndex = {tmp.ConnectIndex}")
        '    putlog($"X = {tmp.X}")
        '    putlog($"Y = {tmp.Y}")
        'Next

        '创建存储控制器及ip地址的数组
        ReDim senderArray(SenderIndexlist.Count - 1)
        ListView4.Items.Clear()

        'ControlPaint.DrawReversibleFrame(
        '    New Rectangle(screenMain.x,
        '                  screenMain.y,
        '                  screenMain.width,
        '                  screenMain.height),
        '    Color.DarkRed,
        '    FrameStyle.Thick)

        'Thread.Sleep(200)

        'ControlPaint.DrawReversibleFrame(
        '    New Rectangle(screenMain.x,
        '                  screenMain.y,
        '                  screenMain.width,
        '                  screenMain.height),
        '    Color.DarkRed,
        '    FrameStyle.Thick)

        mainClass.GetEquipmentIP(SenderIndexlist(getSenderIndex))
    End Sub

    '连接/断开连接
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If ToolStripButton1.Text = "连接" Then

            '写ini配置文件
            Dim tmp As New ClassIni
            tmp.WriteINI("SYS", "Time", ToolStripTextBox1.Text, ".\setting.ini")

            '读取ini配置文件
            checkTime = CInt(tmp.GetINI("SYS", "Time", "", ".\setting.ini"))
            Timer1.Interval = checkTime

            Try
                For i As Integer = 0 To senderArray.Count - 1
                    Dim ipStr As String = $"{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}"
                    'MsgBox(ipStr)
                    If My.Computer.Network.Ping(ipStr, 500) = False Then
                        For ji As Integer = 0 To senderArray.Count - 1
                            Try
                                senderArray(ji).cliSocket.Close()
                            Catch ex As Exception
                            End Try
                        Next

                        MsgBox(ipStr & " 未能连通", MsgBoxStyle.Information, "连接")
                        Exit Sub
                    End If

                    senderArray(i).cliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
                    '发送超时
                    senderArray(i).cliSocket.SendTimeout = 500
                    '接收超时
                    senderArray(i).cliSocket.ReceiveTimeout = 500
                    '连接
                    senderArray(i).cliSocket.Connect(ipStr, 6000)
                Next
            Catch ex As Exception
                For i As Integer = 0 To senderArray.Count - 1
                    senderArray(i).cliSocket.Close()
                Next

                MsgBox("端口绑定失败，请稍后再连接", MsgBoxStyle.Information, "连接")

                Exit Sub
            End Try

            Timer1.Start()
            ToolStripButton1.Text = "断开"
            ToolStripButton1.Image = My.Resources.disconnect
        Else
            '断开
            Try
                For i As Integer = 0 To senderArray.Count - 1
                    Try
                        senderArray(i).cliSocket.Close()
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception
            End Try

            Timer1.Stop()
            ToolStripButton1.Text = "连接"
            ToolStripButton1.Image = My.Resources.connect
        End If
    End Sub

    '移动鼠标点击区域
    Private Sub getMouseClick(tmpScanBoard As ScanBoardInfo, tDataArray As Byte(), index As Integer)
        'Dim tmp As ScanBoardInfo = screenMain.ScanBoardTable.Item(key)

        'If runMode <> 0 And runMode <> 1 Then
        '    Exit Sub
        'End If
        'Dim qwe As String = Nothing
        'For i As Integer = 0 To 16 - 1
        '    qwe = qwe & tDataArray(index + i).ToString("X") & " "
        'Next
        'putlog(qwe)
        'putlog($"{key} {tmp.ConnectIndex}-{tmp.PortIndex}-{tmp.SenderIndex} {tmp.X} {tmp.Y}")

        Dim w As Integer = screenMain.ScanBoardWidth / 4
        Dim h As Integer = screenMain.ScanBoardHeight / 4

        For i As Integer = 0 To 4 - 1
            For j As Integer = 0 To 4 - 1
                If (tDataArray(index + i * 4 + j) And &H80) <> &H80 Then
                    Continue For
                End If

                'If runMode <> 0 And runMode <> 1 Then
                '    '测试
                '    ControlPaint.FillReversibleRectangle(
                '    New Rectangle(screenMain.x + tmp.X + j * w,
                '                  screenMain.y + tmp.Y + i * h,
                '                  w, h),
                '            Color.DarkRed)
                '    ControlPaint.FillReversibleRectangle(
                '    New Rectangle(screenMain.x + tmp.X + j * w,
                '                  screenMain.y + tmp.Y + i * h,
                '                  w, h),
                '            Color.DarkRed)
                'Else
                '点击
                Dim oldx As Integer = System.Windows.Forms.Control.MousePosition.X
                Dim oldy As Integer = System.Windows.Forms.Control.MousePosition.Y

                '隐藏鼠标指针
                ShowCursor(False)

                'screenMain.showFlash.capacitance = dataArray(index + i * 4 + j) And &H7F
                '移动鼠标然后点击
                mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move Or MouseEvent.LeftButtonDown Or MouseEvent.LeftButtonUp,
                                (screenMain.x + tmpScanBoard.X + j * w + screenMain.ScanBoardWidth / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Width,
                                (screenMain.y + tmpScanBoard.Y + i * h + screenMain.ScanBoardHeight / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                '回原位
                mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move,
                                oldx * 65536 / Screen.PrimaryScreen.Bounds.Width,
                                oldy * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                '显示鼠标指针
                ShowCursor(True)
                'End If
            Next
        Next
    End Sub

    '查询状态
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Static lastsec As Integer = -1
        Dim nowsec As Integer = 0
        Static checknum As Integer = 0

        nowsec = Now().Second
        If lastsec = nowsec Then
            checknum += 1
            'putlog(checknum)
        Else
            ToolStripStatusLabel1.Text = $"查询次数:{checknum}/s"
            checknum = 0
            lastsec = nowsec
        End If

        Dim showstr As String = Nothing
        For index As Integer = 0 To senderArray.Count - 1
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
                putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 发送读取指令错误")
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

                For m As Integer = 0 To 16 - 1
                    Dim bytesRec As Integer = senderArray(index).cliSocket.Receive(bytes)
                    'TextBox5.Text = ""

                    For j As Integer = 4 To 1027 Step 32
                        If bytes(j + 0) <> &H55 Then
                            Continue For
                        End If

                        If runMode <> 0 And runMode <> 1 And runMode <> 2 And runMode <> 3 Then
                            Exit For
                        End If

                        Dim tmp = screenMain.ScanBoardTable.Item($"{senderArray(index).index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}")
                        If tmp Is Nothing Then
                            putlog($"{senderArray(index).index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))} nof found")
                            Continue For
                        End If

                        'putlog($"{senderArray(index).index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))} {tmp.X} {tmp.Y}")
                        If runMode <> 1 Then
                            screenMain.showFlash.MousesimulationClick(tmp.X, tmp.Y, bytes, j + 4)
                        Else 'If runMode = 1 Then
                            getMouseClick(tmp, bytes, j + 4)
                        End If

                        tmpstr = $"{senderArray(index).index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}:"
                        '读取数据段
                        For k As Integer = 4 To 27
                            tmpstr = tmpstr & bytes(j + k).ToString("X") & " "
                        Next

                        showstr = showstr & tmpstr & vbCrLf
                    Next

                Next
            Catch ex As Exception
                putlog($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)} 接收数据错误")
                Continue For
            End Try
        Next
        TextBox1.Text = showstr

    End Sub

    Private Sub 模拟点击ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 模拟点击ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 模拟点击ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 模拟点击ToolStripMenuItem.Image

        runMode = 0
        screenMain.showFlash.switchPlayMode()
    End Sub

    Private Sub 点击捕获鼠标F2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 点击捕获鼠标F2ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 点击捕获鼠标F2ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 点击捕获鼠标F2ToolStripMenuItem.Image

        screenMain.showFlash.switchPlayMode()
        runMode = 1
    End Sub

    Private Sub 测试ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测试ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 测试ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 测试ToolStripMenuItem.Image

        runMode = 2
        screenMain.showFlash.switchTestMode()
    End Sub

    Private Sub 测试显示电容F4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测试显示电容F4ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 测试显示电容F4ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 测试显示电容F4ToolStripMenuItem.Image

        runMode = 3
        screenMain.showFlash.switchTestModeWithValue()
    End Sub

    Private Sub 黑屏ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 黑屏ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 黑屏ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 黑屏ToolStripMenuItem.Image

        runMode = 4
        screenMain.showFlash.switchBlankScreenMode()
    End Sub

    Private Sub 忽略ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 忽略ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 忽略ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 忽略ToolStripMenuItem.Image

        runMode = 5
    End Sub

    'Private Sub ListView2_ItemMouseHover(sender As Object, e As ListViewItemMouseHoverEventArgs) Handles ListView2.ItemMouseHover
    '    'Me.Text = e.Item.Text
    '    Dim x As Integer = 0
    '    Dim y As Integer = 0
    '    Dim width As Integer = 0
    '    Dim height As Integer = 0
    '    '获取起始位置 大小
    '    mainClass.GetScreenLocation(CInt(e.Item.Text), x, y, width, height)

    '    ControlPaint.FillReversibleRectangle(
    '        New Rectangle(x,
    '                      y,
    '                      width,
    '                      height),
    '        Color.DarkRed)

    '    'Thread.Sleep(200)

    '    'ControlPaint.DrawReversibleFrame(
    '    '    New Rectangle(x,
    '    '                  y,
    '    '                  width,
    '    '                  height),
    '    '    Color.DarkRed,
    '    '    FrameStyle.Thick)
    'End Sub

    '打开文件夹
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        FolderBrowserDialog1.SelectedPath = ""

        FolderBrowserDialog1.ShowDialog()

        If FolderBrowserDialog1.SelectedPath = "" Then
            Exit Sub
        End If

        Dim dirs As String() = Directory.GetFiles(FolderBrowserDialog1.SelectedPath, "*.swf")

        ListView5.Items.Clear()
        For i As Integer = 0 To dirs.Count - 1
            Dim qwe1 As Integer = InStrRev(dirs(i), "\") + 1
            Dim qwe2 As Integer = Len(dirs(i)) - 4 - qwe1 + 1
            Dim itm As ListViewItem = ListView5.Items.Add($"{Mid(dirs(i), qwe1, qwe2)}", 0)
            itm.SubItems.Add($"{dirs(i)}")
        Next
    End Sub

    Private Sub ListView5_DoubleClick(sender As Object, e As EventArgs) Handles ListView5.DoubleClick
        If ListView5.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        If ToolStripButton1.Enabled = False Then
            Exit Sub
        End If

        'Me.Text = ListView5.SelectedItems(0).SubItems(1).Text

        screenMain.showFlash.play(ListView5.SelectedItems(0).SubItems(1).Text)
    End Sub

End Class
