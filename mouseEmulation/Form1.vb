Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports Nova.Mars.SDK

Public Class Form1
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

        '读取ini配置文件
        Dim tmp As New ClassIni
        checkTime = CInt(tmp.GetINI("SYS", "Time", "", ".\setting.ini"))

        '清空日志文件
        Dim sw As StreamWriter = New StreamWriter（"log.txt", False)
        sw.Close（）

        '显示配置
        Timer1.Interval = checkTime
        ToolStripTextBox1.Text = $"{checkTime}"

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
        ListView2.CheckBoxes = False
        ListView2.Clear()
        ListView2.Columns.Add("显示屏索引", 72, HorizontalAlignment.Left)
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
    End Sub

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        rootClass = New MarsHardwareEnumerator

        If rootClass.Initialize() Then
            'TextBox1.AppendText($"连接Nova服务成功{vbCrLf}")
        Else
            MsgBox($"连接Nova服务失败{vbCrLf}")
            Exit Sub
        End If

        Dim SystemCount As Integer = rootClass.CtrlSystemCount()
        If SystemCount Then
            'TextBox1.AppendText($"控制系统数:{rootClass.CtrlSystemCount()}{vbCrLf}")
        Else
            MsgBox($"未找到控制系统{vbCrLf}")
            Exit Sub
        End If

        mainClass = New MarsControlSystem(rootClass)

        AddHandler mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        For i As Integer = 1 To SystemCount
            tmpstr = Nothing
            screenCount = 0
            senderCount = 0

            Dim itm As ListViewItem = ListView1.Items.Add(i.ToString, 0)

            rootClass.GetComNameOfControlSystem(i - 1, tmpstr)
            itm.SubItems.Add(tmpstr)

            mainClass.Initialize(tmpstr, screenCount, senderCount)
            mainClass.UnInitialize()
            itm.SubItems.Add(screenCount)
            itm.SubItems.Add(senderCount)
        Next

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

    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        mainClass.UnInitialize()
        rootClass.UnInitialize()
    End Sub

    '显示控制器列表
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count = 0 Then
            Exit Sub
        End If

        mainClass.Initialize(ListView1.SelectedItems(0).SubItems(1).Text, vbNull, vbNull)

        ListView1.Enabled = False
        ListView2.Items.Clear()

        If mainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
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

        ListView1.Enabled = True
    End Sub

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
                            senderArray(ji).cliSocket.Close()
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
            For i As Integer = 0 To senderArray.Count - 1
                senderArray(i).cliSocket.Close()
            Next

            Timer1.Stop()
            ToolStripButton1.Text = "连接"
            ToolStripButton1.Image = My.Resources.connect
        End If
    End Sub

    '点击区域
    Private Sub getMouseClick(key As String, dataArray As Byte(), index As Integer)
        Dim tmp As ScanBoardInfo = screenMain.ScanBoardTable.Item(key)


        If runMode = 2 Then
            Exit Sub
        End If

        'putlog($"{key} {tmp.ConnectIndex}-{tmp.PortIndex}-{tmp.SenderIndex} {tmp.X} {tmp.Y}")

        Dim w As Integer = screenMain.ScanBoardWidth / 4
        Dim h As Integer = screenMain.ScanBoardHeight / 4

        For i As Integer = 0 To 4 - 1
            For j As Integer = 0 To 4 - 1
                If dataArray(index + i * 4 + j) <> 1 Then
                    Continue For
                End If

                If runMode = 0 Then
                    '测试
                    ControlPaint.FillReversibleRectangle(
                    New Rectangle(screenMain.x + tmp.X + j * w,
                                  screenMain.y + tmp.Y + i * h,
                                  w, h),
                            Color.DarkRed)
                    ControlPaint.FillReversibleRectangle(
                    New Rectangle(screenMain.x + tmp.X + j * w,
                                  screenMain.y + tmp.Y + i * h,
                                  w, h),
                            Color.DarkRed)
                Else
                    '点击
                    Dim oldx As Integer = System.Windows.Forms.Control.MousePosition.X
                    Dim oldy As Integer = System.Windows.Forms.Control.MousePosition.Y

                    '隐藏鼠标指针
                    ShowCursor(False)
                    '移动鼠标然后点击
                    mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move Or MouseEvent.LeftButtonDown Or MouseEvent.LeftButtonUp,
                                (screenMain.x + tmp.X + j * w + screenMain.ScanBoardWidth / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Width,
                                (screenMain.y + tmp.Y + i * h + screenMain.ScanBoardHeight / 4 / 2) * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                    '回原位
                    mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move,
                                oldx * 65536 / Screen.PrimaryScreen.Bounds.Width,
                                oldy * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                    '显示鼠标指针
                    ShowCursor(True)
                End If
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

                        getMouseClick($"{senderArray(index).index}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}", bytes, j + 4)
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

    Private Sub 测试ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测试ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 测试ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 测试ToolStripMenuItem.Image

        runMode = 0
    End Sub

    Private Sub 模拟点击ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 模拟点击ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 模拟点击ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 模拟点击ToolStripMenuItem.Image

        runMode = 1
    End Sub

    Private Sub 忽略ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 忽略ToolStripMenuItem.Click
        ToolStripSplitButton1.Text = 忽略ToolStripMenuItem.Text
        ToolStripSplitButton1.Image = 忽略ToolStripMenuItem.Image

        runMode = 2
    End Sub

    Private Sub ListView2_ItemMouseHover(sender As Object, e As ListViewItemMouseHoverEventArgs) Handles ListView2.ItemMouseHover
        'Me.Text = e.Item.Text
        Dim x As Integer = 0
        Dim y As Integer = 0
        Dim width As Integer = 0
        Dim height As Integer = 0
        '获取起始位置 大小
        mainClass.GetScreenLocation(CInt(e.Item.Text), x, y, width, height)

        ControlPaint.FillReversibleRectangle(
            New Rectangle(x,
                          y,
                          width,
                          height),
            Color.DarkRed)

        'Thread.Sleep(200)

        'ControlPaint.DrawReversibleFrame(
        '    New Rectangle(x,
        '                  y,
        '                  width,
        '                  height),
        '    Color.DarkRed,
        '    FrameStyle.Thick)
    End Sub
End Class
