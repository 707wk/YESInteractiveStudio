Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading

Public Class FormMain
    ''' <summary>
    ''' 声明注册热键API函数
    ''' </summary>
    Public Declare Function RegisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer,
                                                    ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    ''' <summary>
    ''' 声明注销热键API函数
    ''' </summary>
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer) As Integer
    ''' <summary>
    ''' 热键消息ID
    ''' </summary>
    Public Const WM_HOTKEY As Short = &H312S
    ''' <summary>
    ''' ALT
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

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '测试时关闭登陆验证
        'checkdog()

        '读取最后编译日期
        Dim txtTmp As System.IO.TextReader = System.IO.File.OpenText(".\data\creationDate.ini")
        Me.Text = $"{My.Application.Info.ProductName} [{txtTmp.ReadLine()}]"

        System.IO.Directory.CreateDirectory("./data")
        System.IO.Directory.CreateDirectory("./logs")

        putlog("启动 " & Application.ExecutablePath)

        '反序列化
        If System.IO.File.Exists("./data/setting.db") Then
            Dim fStream As FileStream = New FileStream("./data/setting.db", FileMode.Open)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

            Try
                sysInfo = sfFormatter.Deserialize(fStream)
            Catch ex As Exception
                MsgBox($"配置文件读取失败:{ex.Message}", MsgBoxStyle.Information, "读取配置")
                End
                'Application.Exit()
                '打开版本不同或错误的文件则无法读取
            End Try

            fStream.Close()
        Else
            '第一次使用初始化参数
            ReDim sysInfo.screenList(32 - 1)
            For i As Integer = 0 To sysInfo.screenList.Length - 1
                sysInfo.screenList(i).touchPieceColumnsNum = 4
                sysInfo.screenList(i).touchPieceRowsNum = 4
            Next
            'sysInfo.curtainList = New List(Of curtainInfo)
            sysInfo.startLocation.X = Screen.PrimaryScreen.Bounds.Width / 2
            sysInfo.startLocation.Y = Screen.PrimaryScreen.Bounds.Height / 2
            'sysInfo.filesList = New Hashtable
        End If

        If sysInfo.curtainList Is Nothing Then
            sysInfo.curtainList = New List(Of curtainInfo)
            'Else
            '    '添加存在的幕布列表
            '    For i As Integer = 0 To sysInfo.curtainList.Count - 1
            '        ComboBox1.Items.Add($"幕布{i} {sysInfo.curtainList.Item(i).remark}")
            '    Next
        End If

        If sysInfo.filesList Is Nothing Then
            sysInfo.filesList = New Hashtable
        Else
            '添加历史文件列表
            Dim objFile As System.IO.File
            Dim tmpfilesListkeys As New ArrayList(sysInfo.filesList.Keys)
            For Each i In tmpfilesListkeys
#Disable Warning BC42025
                ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
                '删除不存在的文件路径
                If Not objFile.Exists(sysInfo.filesList.Item(i).ToString) Then
#Enable Warning BC42025
                    sysInfo.filesList.Remove(i)
                    Continue For
                End If
                ComboBox2.Items.Add(i)
            Next

            If ComboBox2.Items.Count Then
                ComboBox2.SelectedIndex = 0
            End If
        End If

        sysInfo.zoomProportion = If(sysInfo.zoomProportion, sysInfo.zoomProportion, 1)
        sysInfo.touchSensitivity = If(sysInfo.touchSensitivity, sysInfo.touchSensitivity, 1)
        sysInfo.clickValidNums = If(sysInfo.clickValidNums, sysInfo.clickValidNums, 1)
        sysInfo.resetTemp = If(sysInfo.resetTemp, sysInfo.resetTemp, 1)
        sysInfo.resetSec = If(sysInfo.resetSec, sysInfo.resetSec, 1)

        Me.Location = sysInfo.startLocation

        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)

        运行F1ToolStripMenuItem.BackColor = Color.FromArgb(&H0, &HE3, &HB)
        测试F2ToolStripMenuItem.BackColor = Color.FromArgb(&HFF, &H7F, &H27)
        黑屏F3ToolStripMenuItem.BackColor = Color.FromArgb(&H7F, &H7F, &H7F)
        忽略F4ToolStripMenuItem.BackColor = Color.FromArgb(&HB0, &HAF, &HDF)
        ToolStripDropDownButton1.BackColor = 运行F1ToolStripMenuItem.BackColor

        '发送卡状态
        Timer1.Interval = 1000

        '发送卡状态列表样式
        For i As Integer = 0 To 10 - 1
            DataGridView1.Columns.Add($"C{i}", $"{i}")
            DataGridView1.Columns(i).AutoSizeMode = DataGridViewAutoSizeColumnMode.None
            DataGridView1.Columns(i).Width = 35
        Next
        DataGridView1.ColumnHeadersVisible = False
        DataGridView1.RowHeadersVisible = False
        DataGridView1.AllowUserToResizeColumns = False
        DataGridView1.AllowUserToResizeRows = False
        DataGridView1.MultiSelect = False
        DataGridView1.Rows.Clear()
        'DataGridView1.GridColor = Color.FromArgb(&H33, &H99, &HFF)
    End Sub

    ''' <summary>
    ''' 创建播放窗体
    ''' </summary>
    Public Sub creatDialogThread(ByVal id As Integer)
        Dim tmp As curtainInfo = sysInfo.curtainList.Item(id)

        tmp.playDialog = New FormPlay

        tmp.playDialog.curtainListId = id

        sysInfo.curtainList.Item(id) = tmp

        tmp.playDialog.setLocation(tmp.x, tmp.y, tmp.width, tmp.height)
        tmp.playDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 显示播放窗体
    ''' </summary>
    Private Sub showCurtain()
        '刷新屏幕下拉列表列表
        '刷新播放信息列表
        ComboBox1.Items.Clear()

        '显示标记的幕布窗体
        Dim tmpCreatDialogThread As Threading.Thread
        For i As Integer = 0 To sysInfo.curtainList.Count - 1
            With sysInfo.curtainList.Item(i)
                ComboBox1.Items.Add($"幕布{i} { .remark}")

                If .playDialog Is Nothing Then
                    '新幕布则创建新窗体
                    tmpCreatDialogThread = New Threading.Thread(AddressOf creatDialogThread)
                    tmpCreatDialogThread.SetApartmentState(ApartmentState.STA)
                    tmpCreatDialogThread.IsBackground = True
                    tmpCreatDialogThread.Start(i)
                Else
                    '已存在则更新位置及大小
                    .playDialog.setLocation(.x, .y, .width, .height)
                End If
            End With
        Next

        '清空控制器连接标记
        For i As Integer = 0 To sysInfo.senderList.Length - 1
            sysInfo.senderList(i).link = False
        Next
        '判断哪些控制器要连接
        For Each i In sysInfo.curtainList
            For Each j In i.screenList
                If Not sysInfo.screenList(j).existFlage Then
                    Continue For
                End If

                For Each k In sysInfo.screenList(j).SenderList
                    sysInfo.senderList(k).link = True
                Next
            Next
        Next

        If ComboBox1.Items.Count Then
            ComboBox1.SelectedIndex = 0
        End If

        Thread.Sleep(1000)
        Me.Activate()
    End Sub

    Private Sub FormMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '读取屏幕参数
        Dim tmpDialog As New FormNovaInit
        tmpDialog.ShowDialog()

        '显示幕布
        showCurtain()

        '启动时为未连接状态
        offLinkCon()

        '加载发送卡列表
        For i As Integer = 0 To sysInfo.senderList.Length - 1 Step 10
            DataGridView1.Rows.Add("")
        Next
    End Sub

    ''' <summary>
    ''' 热键消息处理函数
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY And sysInfo.LinkFlage Then '判断是否为热键消息
            Select Case m.WParam.ToInt32 '判断热键消息的注册ID
                Case 1
                    运行F1ToolStripMenuItem_Click(Nothing, Nothing)
                Case 2
                    测试F2ToolStripMenuItem_Click(Nothing, Nothing)
                Case 3
                    黑屏F3ToolStripMenuItem_Click(Nothing, Nothing)
                Case 4
                    忽略F4ToolStripMenuItem_Click(Nothing, Nothing)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub

    Private Sub FormMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '退出前提示
        If MsgBox("确定退出程序?", MsgBoxStyle.YesNo, "退出") = MsgBoxResult.Yes Then
            If sysInfo.LinkFlage Then
                ToolStripButton1_Click(Nothing, Nothing)
            End If
        Else
            e.Cancel = True
            Exit Sub
        End If

        '保存关闭时窗体位置
        sysInfo.startLocation = Me.Location

        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)

        '关闭播放窗体
        For Each i In sysInfo.curtainList
            i.playDialog.closeDialog(True)
        Next

        '序列化
        Try
            Dim fStream As New FileStream("./data/setting.db", FileMode.Create)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            sfFormatter.Serialize(fStream, sysInfo)
            fStream.Close()
        Catch ex As Exception
            '不知道会不会引发异常，加个保险
        End Try

        '释放nova资源
        If sysInfo.mainClass Is Nothing Then
        Else
            sysInfo.mainClass.UnInitialize()
        End If
        If sysInfo.rootClass Is Nothing Then
        Else
            sysInfo.rootClass.UnInitialize()
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim tmpDialog As New FormOption
        tmpDialog.ShowDialog()

        showCurtain()
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位增量温度 K
    ''' </summary>
    Private Sub setResetTemp(value As Integer)
        Dim sendstr As String = "aadb010300"
        Dim sendByte(sendstr.Length \ 2 - 1) As Byte
        For i As Integer = 0 To sendstr.Length \ 2 - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(4) = value

        sysInfo.mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位时间
    ''' </summary>
    Private Sub setResetSec(value As Integer)
        Dim sendstr As String = "aadb010200"
        Dim sendByte(sendstr.Length \ 2 - 1) As Byte
        For i As Integer = 0 To sendstr.Length \ 2 - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(4) = value

        sysInfo.mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
    End Sub

    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub onLinkCon()
        ToolStripDropDownButton1.Enabled = True
        Button5.Enabled = False
        ToolStripButton1.Text = "断开连接"
        ToolStripButton1.BackColor = Color.OrangeRed
        Timer1.Start()
    End Sub

    ''' <summary>
    ''' 断开连接
    ''' </summary>
    Private Sub offLinkCon()
        ToolStripDropDownButton1.Enabled = False
        Button5.Enabled = True
        ToolStripButton1.Text = "连接控制器"
        ToolStripButton1.BackColor = Color.FromArgb(&H0, &HE3, &HB)
        Timer1.Stop()
    End Sub

    ''' <summary>
    ''' 连接/断开连接
    ''' </summary>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If Not sysInfo.LinkFlage Then
            '连接控制器
            If sysInfo.curtainList.Count = 0 Then
                Exit Sub
            End If

            '检测连接状态
            For Each i In sysInfo.senderList
                With i
                    If .link = False Then
                        Continue For
                    End If

                    Dim ipStr As String = $"{ .ipDate(3)}.{ .ipDate(2)}.{ .ipDate(1)}.{ .ipDate(0)}"
                    'ping 设备IP地址
                    If My.Computer.Network.Ping(ipStr, 500) = False Then
                        MsgBox($"{ipStr} 未能连通", MsgBoxStyle.Information, "连接")
                        Exit Sub
                    End If
                End With
            Next

            '建立与控制器的连接
            Try
                For i As Integer = 0 To sysInfo.senderList.Length - 1
                    If sysInfo.senderList(i).link = False Then
                        Continue For
                    End If

                    With sysInfo.senderList(i)
                        Dim ipStr As String = $"{ .ipDate(3)}.{ .ipDate(2)}.{ .ipDate(1)}.{ .ipDate(0)}"

                        .cliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
                            .SendTimeout = 1000,
                            .ReceiveTimeout = 1000
                        }
                        '连接
                        .cliSocket.Connect(ipStr, 6000)

                        .workThread = New Threading.Thread(AddressOf communicWorkThread) With {
                            .IsBackground = True '线程后台运行
                        }
                        '线程开始
                        .workThread.Start(i)
                    End With
                Next
            Catch ex As Exception
                '异常关闭连接
                For Each i In sysInfo.senderList
                    Try
                        i.workThread.Abort()
                    Catch ex2 As Exception
                    End Try
                    Try
                        i.cliSocket.Close()
                    Catch ex3 As Exception
                    End Try
                Next

                MsgBox($"控制器连接错误:{ex.Message}", MsgBoxStyle.Information, "连接")
                Exit Sub
            End Try

            '启动复位功能
            setResetTemp(sysInfo.resetTemp)
            setResetSec(sysInfo.resetSec)

            onLinkCon()
            sysInfo.LinkFlage = True
        Else
            '断开控制器

            For Each i In sysInfo.senderList
                If i.link = False Then
                    Continue For
                End If

                With i
                    .workThread.Abort()
                    .cliSocket.Close()
                End With
            Next

            '关闭复位功能
            setResetTemp(0)
            setResetSec(0)

            offLinkCon()
            sysInfo.LinkFlage = False
        End If
    End Sub

    ''' <summary>
    ''' 异常时断开连接并提示
    ''' </summary>
    Public Delegate Sub showExceptionCallback(ByVal nums As Integer)
    Public Sub showException(ByVal nums As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New showExceptionCallback(AddressOf showException), New Object() {nums})
            Exit Sub
        End If

        ToolStripButton1_Click(Nothing, Nothing)

        MsgBox($"控制器已连续 {nums} 次未返回数据!", MsgBoxStyle.Information, Me.Text)
    End Sub

    ''' <summary>
    ''' 检测线程
    ''' </summary>
    Private Sub communicWorkThread(ByVal senderId As Integer)
        '上次运行时间
        Dim lastsec As Integer = -1
        '当前运行时间
        Dim nowsec As Integer = 0
        '异常次数
        Dim exceptionNums As Integer = 0
        '每秒查询次数
        Dim readNum As Integer = 0

        Do
            '获取当前时间秒
            nowsec = Now().Second
            If lastsec <> nowsec Then
                exceptionNums = 0
                sysInfo.senderList(senderId).MaxReadNum = readNum
                readNum = 0
                lastsec = nowsec
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

            Try
                '向发送卡 请求接收传感器数据数据
                Dim bytes(1028 - 1) As Byte
                Dim tmpstr As String = "55d50902"
                Dim sendbytes(4 - 1) As Byte
                For i As Integer = 0 To tmpstr.Length \ 2 - 1
                    sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                Next i
                '发送数据
                Dim bytesSend As Integer = sysInfo.senderList(senderId).cliSocket.Send(sendbytes)
                '接收数据
                Dim bytesRec As Integer = sysInfo.senderList(senderId).cliSocket.Receive(bytes)

            Catch ex As Exception
                exceptionNums += 1
                Continue Do
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
                Dim bytesSend As Integer = sysInfo.senderList(senderId).cliSocket.Send(sendbytes)

                '诺瓦每次只发送1K数据，16K数据分16次发送
                For i As Integer = 0 To 16 - 1

                    Dim bytesRec As Integer = sysInfo.senderList(senderId).cliSocket.Receive(bytes)
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

                        If sysInfo.displayMode <> 0 And
                            sysInfo.displayMode <> 1 Then
                            Exit For
                        End If

                        '查找接收卡位置[由像素改为索引]
                        If sysInfo.ScanBoardTable.Item($"{senderId}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}") Is Nothing Then
                            Continue For
                        End If
                        Dim tmp As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{senderId}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}")

                        '计算总点击块
                        Dim tmpClickValidSum As Integer = 0
                        For k = 0 To sysInfo.screenList(tmp.ScreenId).touchPieceRowsNum * sysInfo.screenList(tmp.Screenid).touchPieceColumnsNum - 1
                            tmpClickValidSum = tmpClickValidSum + If(bytes(j + 4 + k) And &H80, 1, 0)
                        Next

                        If tmpClickValidSum < sysInfo.clickValidNums Then
                            Continue For
                        End If

                        'Debug.WriteLine(tmpClickValidSum)

                        For k As Integer = 0 To sysInfo.screenList(tmp.ScreenId).touchPieceRowsNum - 1
                            For l As Integer = 0 To sysInfo.screenList(tmp.ScreenId).touchPieceColumnsNum - 1
                                sysInfo.screenList(tmp.ScreenId).clickHistoryArray(tmp.Y + k, tmp.X + l) = bytes(j + k * sysInfo.screenList(tmp.ScreenId).touchPieceRowsNum + l) And &H80

                                If (bytes(j + 4 + k * sysInfo.screenList(tmp.ScreenId).touchPieceRowsNum + l) And &H80) <> &H80 Then
                                    sysInfo.screenList(tmp.ScreenId).clickHistoryArray(tmp.Y + k, tmp.X + l) = 0
                                    Continue For
                                End If

                                If sysInfo.screenList(tmp.ScreenId).clickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                    sysInfo.screenList(tmp.ScreenId).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                    Continue For
                                End If

                                sysInfo.screenList(tmp.ScreenId).clickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                sysInfo.curtainList.Item(sysInfo.screenList(tmp.ScreenId).curtainListId).
                                    playDialog.
                                    MousesimulationClick(tmp.ScreenId,
                                                         tmp.X + l,
                                                         tmp.Y + k,
                                                         bytes(j + 4 + k * sysInfo.screenList(tmp.ScreenId).touchPieceRowsNum + l))
                            Next
                        Next

                    Next
                Next

                readNum += 1
            Catch ex As Exception
                exceptionNums += 1
            End Try

            Thread.Sleep(sysInfo.inquireTimeSec)
        Loop
    End Sub

    '添加文件
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        '添加flash文件
        Dim tmp As New OpenFileDialog
        tmp.Filter = "swf|*.swf"
        tmp.Multiselect = True
        If tmp.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        '添加到播放列表
        For i As Integer = 0 To tmp.FileNames.Length - 1
            If sysInfo.filesList.Item(tmp.SafeFileNames(i)) IsNot Nothing Then
                Continue For
            End If

            ComboBox2.Items.Add(tmp.SafeFileNames(i))
            sysInfo.filesList.Add(tmp.SafeFileNames(i), tmp.FileNames(i))
        Next
    End Sub

    ''' <summary>
    ''' 删除文件
    ''' </summary>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        sysInfo.filesList.Remove(ComboBox2.Text)
        ComboBox2.Items.Remove(ComboBox2.Text)
        If ComboBox2.Items.Count Then
            ComboBox2.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>
    ''' 播放
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.Text = "" Then
            Exit Sub
        End If

        sysInfo.curtainList.Item(ComboBox1.SelectedIndex).playDialog.play(sysInfo.filesList.Item(ComboBox2.Text))
    End Sub

    ''' <summary>
    ''' 播放至所有屏幕
    ''' </summary>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox1.Text = "" Then
            Exit Sub
        End If

        For Each i In sysInfo.curtainList
            i.playDialog.play(sysInfo.filesList.Item(ComboBox2.Text))
        Next
    End Sub

    Private Sub 运行F1ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 运行F1ToolStripMenuItem.Click
        sysInfo.displayMode = 0

        ToolStripDropDownButton1.Text = 运行F1ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 运行F1ToolStripMenuItem.BackColor

        For Each i In sysInfo.curtainList
            i.playDialog.switchPlayMode(True)
        Next
    End Sub

    Private Sub 测试F2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测试F2ToolStripMenuItem.Click
        sysInfo.displayMode = 1

        ToolStripDropDownButton1.Text = 测试F2ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 测试F2ToolStripMenuItem.BackColor

        For Each i In sysInfo.curtainList
            i.playDialog.switchTestMode(True)
        Next
    End Sub

    Private Sub 黑屏F3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 黑屏F3ToolStripMenuItem.Click
        sysInfo.displayMode = 2

        ToolStripDropDownButton1.Text = 黑屏F3ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 黑屏F3ToolStripMenuItem.BackColor

        For Each i In sysInfo.curtainList
            i.playDialog.switchBlankScreenMode(True)
        Next
    End Sub

    Private Sub 忽略F4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 忽略F4ToolStripMenuItem.Click
        sysInfo.displayMode = 3

        ToolStripDropDownButton1.Text = 忽略F4ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 忽略F4ToolStripMenuItem.BackColor
    End Sub

    ''' <summary>
    ''' 隐藏播放窗体
    ''' </summary>
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Not CheckBox1.Checked Then
            For i As Integer = 0 To sysInfo.curtainList.Count - 1
                With sysInfo.curtainList.Item(i)
                    .playDialog.setLocation(.x, .y, .width, .height)
                End With
            Next
        Else
            For i As Integer = 0 To sysInfo.curtainList.Count - 1
                With sysInfo.curtainList.Item(i)
                    .playDialog.setLocation(.x, .y, .0, .0)
                End With
            Next
        End If
    End Sub

    '定时刷新发送卡状态
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        For i As Integer = 0 To sysInfo.senderList.Length - 1 Step 10
            DataGridView1.Rows(i \ 10).Cells(i Mod 10).Value = $"{sysInfo.senderList(i).MaxReadNum}"
        Next
    End Sub
End Class