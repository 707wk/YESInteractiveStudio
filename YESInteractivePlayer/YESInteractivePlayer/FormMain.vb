﻿Imports System.ComponentModel
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
    ''' <summary>
    ''' 调试模式
    ''' </summary>
    Private DebugFlage As Boolean

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.KeyPreview = True

        '测试时关闭登陆验证
        'checkdog()

        System.IO.Directory.CreateDirectory("./data")
        'System.IO.Directory.CreateDirectory("./logs")

        'Putlog("启动 " & Application.ExecutablePath)

        '反序列化
        If System.IO.File.Exists("./data/setting.db") Then
            Dim fStream As FileStream = New FileStream("./data/Setting.db", FileMode.Open)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

            Try
                sysInfo = sfFormatter.Deserialize(fStream)
            Catch ex As Exception
                MsgBox($"配置文件读取异常:{ex.Message}",
                       MsgBoxStyle.Information,
                       "读取配置")
                End
                'Application.Exit()
                '打开版本不同或错误的文件则无法读取
            End Try

            fStream.Close()
        Else
            '第一次使用初始化参数
            ReDim sysInfo.ScreenList(32 - 1)
            For i As Integer = 0 To sysInfo.ScreenList.Length - 1
                sysInfo.ScreenList(i).TouchPieceColumnsNum = 4
                sysInfo.ScreenList(i).TouchPieceRowsNum = 4
            Next
            'sysInfo.curtainList = New List(Of curtainInfo)
            sysInfo.StartLocation.X = Screen.PrimaryScreen.Bounds.Width / 2
            sysInfo.StartLocation.Y = Screen.PrimaryScreen.Bounds.Height / 2
            'sysInfo.filesList = New Hashtable
        End If

        If sysInfo.CurtainList Is Nothing Then
            sysInfo.CurtainList = New List(Of CurtainInfo)
        End If

        If sysInfo.FilesList Is Nothing Then
            sysInfo.FilesList = New Hashtable
        Else
            '添加历史文件列表
            Dim objFile As System.IO.File
            Dim tmpfilesListkeys As New ArrayList(sysInfo.FilesList.Keys)
            For Each i In tmpfilesListkeys
#Disable Warning BC42025
                ' 通过实例访问共享成员、常量成员、枚举成员或嵌套类型
                '删除不存在的文件路径
                If Not objFile.Exists(sysInfo.FilesList.Item(i).ToString) Then
#Enable Warning BC42025
                    sysInfo.FilesList.Remove(i)
                    Continue For
                End If
                ComboBox2.Items.Add(i)
            Next

            If ComboBox2.Items.Count Then
                ComboBox2.SelectedIndex = 0
            End If
        End If

        '加载语言包
        sysInfo.LanguageTable = New Hashtable
        Try
            Dim IOsR As StreamReader = New StreamReader("./data/LanguageTable.ini")
            Do
                Dim tmpstr As String = IOsR.ReadLine
                '判断数据合法性
                If tmpstr Is Nothing Then
                    Exit Do
                End If

                '判断数据个数
                Dim tmpstr2() As String = tmpstr.Split("_")
                If tmpstr2.Length < 2 Then
                    Continue Do
                End If

                Dim textArray(tmpstr2.Length - 1 - 1) As String
                For i As Integer = 0 To textArray.Length - 1
                    textArray(i) = tmpstr2(i + 1)
                Next

                sysInfo.LanguageTable.Add(tmpstr2(0), textArray)
            Loop
        Catch ex As Exception
            MsgBox($"语言包读取异常:{ex.Message}", MsgBoxStyle.Information, "加载语言包")
            'Application.Exit()
        End Try

        sysInfo.ZoomProportion = If(sysInfo.ZoomProportion, sysInfo.ZoomProportion, 1)
        sysInfo.ZoomTmpNumerator = If(sysInfo.ZoomTmpNumerator, sysInfo.ZoomTmpNumerator, 1)
        sysInfo.ZoomTmpDenominator = If(sysInfo.ZoomTmpDenominator, sysInfo.ZoomTmpDenominator, 1)
        sysInfo.TouchSensitivity = If(sysInfo.TouchSensitivity, sysInfo.TouchSensitivity, 1)
        sysInfo.ClickValidNums = If(sysInfo.ClickValidNums, sysInfo.ClickValidNums, 1)
        sysInfo.ResetTemp = sysInfo.ResetTemp
        sysInfo.ResetSec = sysInfo.ResetSec

        Me.Location = sysInfo.StartLocation
        If Me.Location.X > Screen.PrimaryScreen.Bounds.Width Or
            Me.Location.Y > Screen.PrimaryScreen.Bounds.Height Then

            Me.Location = New Point(Screen.PrimaryScreen.Bounds.Width / 2,
                                    Screen.PrimaryScreen.Bounds.Height / 2)
        End If

        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)

        '运行F1ToolStripMenuItem.BackColor = Color.FromArgb(&H0, &HE3, &HB)
        '测试F2ToolStripMenuItem.BackColor = Color.FromArgb(&HFF, &H7F, &H27)
        '黑屏F3ToolStripMenuItem.BackColor = Color.FromArgb(&H7F, &H7F, &H7F)
        '忽略F4ToolStripMenuItem.BackColor = Color.FromArgb(&HB0, &HAF, &HDF)
        'ToolStripDropDownButton1.BackColor = 运行F1ToolStripMenuItem.BackColor

        '发送卡状态
        Timer1.Interval = 1000

        '设置显示语言
        SetControlslanguage(Me)

        '显示版本号
        With My.Application.Info
            '版本号每修改一次加1
            Me.Text = $"{ GetLanguage(.ProductName)} V{ .Version.ToString}"
        End With

        sysInfo.logger = New Wangk.Tools.Logger With {
            .writelevel = Wangk.Tools.Loglevel.Level_DEBUG,
            .saveDaysMax = 30
        }
        sysInfo.logger.Init()
    End Sub

    '''' <summary>
    '''' 删除多少天前的log文件
    '''' </summary>
    'Private Sub DeleteLog(saveDays As Integer)
    '    Dim nowtime As DateTime = DateTime.Now
    '    Dim files As String() = Directory.GetFiles("./logs")
    '    For Each file In files
    '        Dim f As FileInfo = New FileInfo(file)
    '        Dim t As TimeSpan = nowtime - f.LastWriteTime
    '        If (t.Days > saveDays) Then
    '            f.Delete()
    '        End If
    '    Next
    'End Sub

    ''' <summary>
    ''' 创建播放窗体
    ''' </summary>
    Public Sub CreatDialogThread(ByVal id As Integer)
        Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(id)

        tmp.PlayDialog = New FormPlay

        tmp.PlayDialog.curtainListId = id

        sysInfo.CurtainList.Item(id) = tmp

        tmp.PlayDialog.SetLocation(tmp.X, tmp.Y, tmp.Width, tmp.Height)
        tmp.PlayDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 显示播放窗体
    ''' </summary>
    Private Sub ShowCurtain()
        '刷新屏幕下拉列表列表
        '刷新播放信息列表
        ComboBox1.Items.Clear()

        '显示标记的幕布窗体
        Dim tmpCreatDialogThread As Threading.Thread
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            With sysInfo.CurtainList.Item(i)
                ComboBox1.Items.Add($"{i} { .Remark}")

                If .PlayDialog Is Nothing Then
                    '新幕布则创建新窗体
                    tmpCreatDialogThread = New Threading.Thread(AddressOf CreatDialogThread)
                    tmpCreatDialogThread.SetApartmentState(ApartmentState.STA)
                    tmpCreatDialogThread.IsBackground = True
                    tmpCreatDialogThread.Start(i)
                Else
                    '已存在则更新位置及大小
                    .PlayDialog.SetLocation(.X, .Y, .Width, .Height)
                End If
            End With
        Next

        '重建点击历史缓存[20180310]
        For i As Integer = 0 To sysInfo.ScreenList.Length - 1
            If Not sysInfo.ScreenList(i).ExistFlage Then
                Continue For
            End If

            With sysInfo.ScreenList(i)
                '创建上次点击状态缓存
                ReDim .ClickHistoryArray((.DefaultHeight \ .DefaultScanBoardHeight) * .TouchPieceRowsNum,
                                         (.DefaultWidth \ .DefaultScanBoardWidth) * .TouchPieceColumnsNum)
            End With
        Next

        '清空控制器连接标记
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            sysInfo.SenderList(i).Link = False
        Next
        '判断哪些控制器要连接
        For Each i In sysInfo.CurtainList
            For Each j In i.ScreenList
                If Not sysInfo.ScreenList(j).ExistFlage Then
                    Continue For
                End If

                For Each k In sysInfo.ScreenList(j).SenderList
                    If k >= sysInfo.SenderList.Length Then
                        Continue For
                    End If

                    sysInfo.SenderList(k).Link = True
                Next
            Next
        Next

        If ComboBox1.Items.Count Then
            ComboBox1.SelectedIndex = 0
        End If

        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            ToolStripDropDownButton2.DropDownItems(i).Enabled = sysInfo.SenderList(i).Link
        Next

        '等待播放窗体显示完毕
        Thread.Sleep(1000)

        Try
            For i As Integer = 0 To sysInfo.CurtainList.Count - 1
                With sysInfo.CurtainList.Item(i)
                    '更新位置及大小
                    .PlayDialog.SetLocation(.X, .Y, .Width, .Height)
                End With
            Next
        Catch ex As Exception
            '第一次显示播放窗体时尺寸未改变大小
            '暂时用此办法解决
        End Try

        '将焦点移至主窗体
        Me.Activate()
    End Sub

    Private Sub FormMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '读取屏幕参数
        Dim tmpDialog As New FormNovaInit
        tmpDialog.ShowDialog()

        '加载发送卡列表
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            ToolStripDropDownButton2.DropDownItems.Add($"控制器{i}")
        Next

        '显示幕布
        ShowCurtain()

        '启动时为未连接状态
        OffLinkCon()
    End Sub

    ''' <summary>
    ''' 热键消息处理函数
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY And sysInfo.LinkFlage Then '判断是否为热键消息
            Select Case m.WParam.ToInt32 '判断热键消息的注册ID
                Case 1
                    Button6_Click(Nothing, Nothing)
                Case 2
                    Button7_Click(Nothing, Nothing)
                Case 3
                    Button8_Click(Nothing, Nothing)
                Case 4
                    Button9_Click(Nothing, Nothing)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub

    ''' <summary>
    ''' 开启调试模式
    ''' </summary>
    Private Sub FormMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Static password As String = Nothing
        password = password & Convert.ToChar(e.KeyValue)
        If password.Length > 128 Then
            password = Microsoft.VisualBasic.Right(password, 32)
        End If

        If password.IndexOf("YESTECH") = -1 Then
            Exit Sub
        End If

        DebugFlage = True

        Button9.Text = GetLanguage("电容")
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
        sysInfo.StartLocation = Me.Location

        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)

        '关闭播放窗体
        For Each i In sysInfo.CurtainList
            i.PlayDialog.CloseDialog(True)
        Next

        '序列化
        Try
            Dim fStream As New FileStream("./data/Setting.db", FileMode.Create)
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            sfFormatter.Serialize(fStream, sysInfo)
            fStream.Close()
        Catch ex As Exception
            '不知道会不会引发异常，加个保险
        End Try

        '释放nova资源
        If sysInfo.MainClass Is Nothing Then
        Else
            sysInfo.MainClass.UnInitialize()
        End If
        If sysInfo.RootClass Is Nothing Then
        Else
            sysInfo.RootClass.UnInitialize()
        End If
    End Sub

    'Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click

    'End Sub

    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim tmpDialog As New FormOption
        tmpDialog.ShowDialog()

        ShowCurtain()
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位增量温度 K
    ''' </summary>
    Private Sub SetResetTemp(value As Integer)
        Dim sendstr As String = "aadb010300"
        Dim sendByte(sendstr.Length \ 2 - 1) As Byte
        For i As Integer = 0 To sendstr.Length \ 2 - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(4) = value

        sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)

        Thread.Sleep(100)
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位时间
    ''' </summary>
    Private Sub SetResetSec(value As Integer)
        Dim sendstr As String = "aadb010200"
        Dim sendByte(sendstr.Length \ 2 - 1) As Byte
        For i As Integer = 0 To sendstr.Length \ 2 - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(4) = value

        sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)

        Thread.Sleep(100)
    End Sub

    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub OnLinkCon()
        'ToolStripDropDownButton1.Enabled = True
        Button6.Enabled = True
        Button7.Enabled = True
        Button8.Enabled = True
        Button9.Enabled = True

        Button6_Click(Nothing, Nothing)

        ToolStripButton2.Enabled = False
        ToolStripButton1.Text = GetLanguage("断开连接")
        'ToolStripButton1.BackColor = Color.OrangeRed
        ToolStripButton1.Image = My.Resources.disconnect
        Timer1.Start()
    End Sub

    ''' <summary>
    ''' 断开连接
    ''' </summary>
    Private Sub OffLinkCon()
        'ToolStripDropDownButton1.Enabled = False
        Button6.Enabled = False
        Button7.Enabled = False
        Button8.Enabled = False
        Button9.Enabled = False

        ToolStripButton2.Enabled = True
        ToolStripButton1.Text = GetLanguage("连接控制器")
        'ToolStripButton1.BackColor = Color.FromArgb(&H0, &HE3, &HB)
        ToolStripButton1.Image = My.Resources.connect

        ToolStripDropDownButton2.Image = My.Resources.ServerFault

        Timer1.Stop()
    End Sub

    ''' <summary>
    ''' 连接/断开连接
    ''' </summary>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If Not sysInfo.LinkFlage Then
            '连接控制器
            If sysInfo.CurtainList.Count = 0 Then
                Exit Sub
            End If

            Try
                '检测连接状态
                For Each i In sysInfo.SenderList
                    With i
                        If .Link = False Then
                            Continue For
                        End If

                        Dim ipStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"
                        'ping 设备IP地址
                        If My.Computer.Network.Ping(ipStr, 500) = False Then
                            MsgBox($"{ipStr} 未能连通",
                                   MsgBoxStyle.Information,
                                   "连接")
                            Exit Sub
                        End If
                    End With
                Next
            Catch ex As Exception
                MsgBox($"连接异常:{ex.Message}",
                                   MsgBoxStyle.Information,
                                   "连接")
                Exit Sub
            End Try

            sysInfo.LinkFlage = True

            '建立与控制器的连接
            Try
                For i As Integer = 0 To sysInfo.SenderList.Length - 1
                    If sysInfo.SenderList(i).Link = False Then
                        Continue For
                    End If

                    With sysInfo.SenderList(i)
                        Dim ipStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"

                        .CliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
                            .SendTimeout = 200,
                            .ReceiveTimeout = 200
                        }
                        '连接
                        .CliSocket.Connect(ipStr, 6000)

                        .WorkThread = New Threading.Thread(AddressOf CommunicWorkThread) With {
                            .IsBackground = True '线程后台运行
                        }
                        '线程开始
                        .WorkThread.Start(i)
                    End With
                Next
            Catch ex As Exception
                '异常关闭连接
                For Each i In sysInfo.SenderList
                    Try
                        i.WorkThread.Abort()
                    Catch ex2 As Exception
                    End Try
                    Try
                        i.CliSocket.Close()
                    Catch ex3 As Exception
                    End Try
                Next
                sysInfo.LinkFlage = True
                MsgBox($"控制器连接异常:{ex.Message}",
                       MsgBoxStyle.Information,
                       "连接")
                Exit Sub
            End Try

            '启动复位功能
            SetResetTemp(sysInfo.ResetTemp)
            SetResetSec(sysInfo.ResetSec)

            OnLinkCon()

        Else
            '断开控制器
            sysInfo.LinkFlage = False

            For Each i In sysInfo.SenderList
                If i.Link = False Then
                    Continue For
                End If

                Try
                    With i
                        .WorkThread.Join()
                        '.WorkThread.Abort()
                        '.CliSocket.Close()
                    End With
                Catch ex As Exception
                End Try

            Next
            'Thread.Sleep(200)

            '关闭复位功能
            SetResetTemp(0)
            SetResetSec(0)

            OffLinkCon()
        End If
    End Sub

    ''' <summary>
    ''' 异常时断开连接并提示
    ''' </summary>
    Public Delegate Sub showExceptionCallback(ByVal lastErrorStr As String)
    Public Sub ShowException(ByVal lastErrorStr As String)
        If Me.InvokeRequired Then
            Me.Invoke(New showExceptionCallback(AddressOf ShowException), New Object() {lastErrorStr})
            Exit Sub
        End If

        ToolStripButton1_Click(Nothing, Nothing)

        MsgBox($"控制器连接异常:{lastErrorStr},请重启控制器",
               MsgBoxStyle.Information,
               Me.Text)
    End Sub

    ''' <summary>
    ''' 检测相邻触发数是否大于等于抗干扰数
    ''' </summary>
    Private Function CheckAdjacencyPieceNums(ScreenId As Integer, Y As Integer, X As Integer) As Boolean
        '忽略自身
        Dim checkNums As Integer = -1

        For i = -1 To 1
            For j = -1 To 1

                '屏幕边缘则跳过
                If X = 0 Or
                    Y = 0 Or
                    Y = sysInfo.ScreenList(ScreenId).ClickHistoryArray.GetLength(0) - 1 Or
                    X = sysInfo.ScreenList(ScreenId).ClickHistoryArray.GetLength(1) - 1 Then
                    Continue For
                End If

                If sysInfo.ScreenList(ScreenId).ClickHistoryArray(Y + i, X + j) = &H80 Then
                    checkNums += 1
                End If
            Next
        Next

        Return If(checkNums >= sysInfo.ClickValidNums, True, False)
    End Function

    ''' <summary>
    ''' 检测线程
    ''' </summary>
    Private Sub CommunicWorkThread(ByVal senderId As Integer)
        '上次运行时间
        Dim lastsec As Integer = -1
        '当前运行时间
        Dim nowsec As Integer = 0
        '异常次数
        Dim exceptionNums As Integer = 0
        '每秒查询次数
        Dim readNum As Integer = 0
        '最后一次异常信息
        Dim lastErrorStr As String = Nothing

        Do While sysInfo.LinkFlage
            '获取当前时间秒
            nowsec = Now().Second
            If lastsec <> nowsec Then
                exceptionNums = 0
                sysInfo.SenderList(senderId).MaxReadNum = readNum
                readNum = 0
                lastsec = nowsec
            End If

            '出现三次异常，进行提示，并且终止进程
            If exceptionNums > 3 Then
                ShowException(lastErrorStr)
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
                Dim bytesSend As Integer = sysInfo.SenderList(senderId).CliSocket.Send(sendbytes)
                '接收数据
                Dim bytesRec As Integer = sysInfo.SenderList(senderId).CliSocket.Receive(bytes)

            Catch ex As SocketException
                lastErrorStr = ex.Message
                sysInfo.logger.LogThis("请求接收传感器数据异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
                exceptionNums += 1
                Continue Do
            Catch ex As ThreadAbortException
                Debug.WriteLine($"stop read")
                '不记录终止异常
            Catch ex As Exception
                sysInfo.logger.LogThis("请求传感器数据其他异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try

            'Dim asd As New Stopwatch
            'asd.Start()
            '向发送卡 请求发送传感器数据数据
            Dim qweasd As Integer = 0
            Try
                '向发送卡发送数据
                Dim bytes2(1028 - 1) As Byte
                Dim tmpstr2 As String = "55d50905000000000400"
                Dim sendbytes2(10 - 1) As Byte
                For i As Integer = 0 To tmpstr2.Length \ 2 - 1
                    sendbytes2(i) = Val("&H" & tmpstr2(i * 2)) * 16 + Val("&H" & tmpstr2(i * 2 + 1))
                Next i
                '发送到发送卡数据
                Dim bytesSend2 As Integer = sysInfo.SenderList(senderId).CliSocket.Send(sendbytes2)

                '诺瓦每次只发送1K数据，16K数据分16次发送
                For i As Integer = 0 To 16 - 1
                    qweasd = i
                    Dim bytesRec2 As Integer = sysInfo.SenderList(senderId).CliSocket.Receive(bytes2)
                    'TextBox5.Text = ""
                    '分析接收到的数据
                    For j As Integer = 4 To 1027 Step 32
                        '有效数据头
                        If bytes2(j + 0) <> &H55 Then
                            Continue For
                        End If

                        '网口号大于4则丢弃
                        If bytes2(j + 1) > 4 Then
                            Continue For
                        End If

                        If sysInfo.DisplayMode <> 0 And
                            sysInfo.DisplayMode <> 1 And
                            sysInfo.DisplayMode <> 4 Then
                            Exit For
                        End If

                        '查找接收卡位置[由像素改为索引]
                        If sysInfo.ScanBoardTable.Item($"{senderId}-{bytes2(j + 1)}-{(bytes2(j + 2) * 256 + bytes2(j + 3))}") Is Nothing Then
                            Continue For
                        End If
                        Dim tmp As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{senderId}-{bytes2(j + 1)}-{(bytes2(j + 2) * 256 + bytes2(j + 3))}")

                        '计算总点击块
                        Dim tmpClickValidSum As Integer = 0
                        For k = 0 To 4 * 4 - 1
                            tmpClickValidSum = tmpClickValidSum + If(bytes2(j + 4 + k) And &H80, 1, 0)
                        Next

                        ''If tmpClickValidSum < 1 And
                        If tmpClickValidSum < sysInfo.ClickValidNums And
                            sysInfo.DisplayMode = 0 Then
                            '互动模式下抗干扰才启用
                            Continue For
                        End If

                        'Debug.WriteLine(tmpClickValidSum)

                        For k As Integer = 0 To 4 - 1 'sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum - 1
                            If k >= sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum Then
                                Continue For
                            End If

                            For l As Integer = 0 To 4 - 1 'sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum - 1
                                If l >= sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum Then
                                    Continue For
                                End If

                                If sysInfo.DisplayMode = 0 Or
                                                sysInfo.DisplayMode = 1 Then
                                    '互动或测试时启用
                                    'Static id As Integer = 0

                                    'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = bytes(j + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l) And &H80
                                    'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = bytes2(j + k * 4 + l) And &H80

                                    '未被点击
                                    If (bytes2(j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                        sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = 0

                                        Continue For
                                    End If

                                    '被点击过
                                    If sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                        sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                        Continue For
                                    End If

                                    sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                    'Debug.WriteLine($"new {id} x:{tmp.X} + {l} y:{tmp.Y} + {k}")
                                    'id += 1

                                    'If Not CheckAdjacencyPieceNums(tmp.ScreenId, tmp.Y + k, tmp.X + l) Then
                                    '    Continue For
                                    'End If
                                End If

                                sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                    PlayDialog.
                                    MousesimulationClick(tmp.ScreenId,
                                                         tmp.X + l,
                                                         tmp.Y + k,
                                                         bytes2(j + 4 + k * 4 + l))
                            Next
                        Next

                    Next
                Next

                readNum += 1

            Catch ex As SocketException
                lastErrorStr = ex.Message
                sysInfo.logger.LogThis("接收传感器数据数据通信异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
                exceptionNums += 1
            Catch ex As ThreadAbortException
                Debug.WriteLine($"stop:{qweasd}")
                '不记录终止异常
            Catch ex As Exception
                sysInfo.logger.LogThis("接收传感器数据数据其他异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try

            Thread.Sleep(sysInfo.InquireTimeSec)
        Loop

        Try
            sysInfo.SenderList(senderId).CliSocket.Close()
        Catch ex As Exception
        End Try
        Debug.WriteLine("end")
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
            If sysInfo.FilesList.Item(tmp.SafeFileNames(i)) IsNot Nothing Then
                Continue For
            End If

            ComboBox2.Items.Add(tmp.SafeFileNames(i))
            sysInfo.FilesList.Add(tmp.SafeFileNames(i), tmp.FileNames(i))
        Next

        '显示第一个添加的文件名
        ComboBox2.Text = tmp.SafeFileNames(0)
    End Sub

    ''' <summary>
    ''' 删除文件
    ''' </summary>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        sysInfo.FilesList.Remove(ComboBox2.Text)
        ComboBox2.Items.Remove(ComboBox2.Text)
        If ComboBox2.Items.Count Then
            ComboBox2.SelectedIndex = 0
        End If
    End Sub

    ''' <summary>
    ''' 清空播放列表
    ''' </summary>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        ComboBox2.Items.Clear()
        sysInfo.FilesList.Clear()
    End Sub

    ''' <summary>
    ''' 播放
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ComboBox1.Text = "" Then
            Exit Sub
        End If

        sysInfo.CurtainList.Item(ComboBox1.SelectedIndex).PlayDialog.Play(sysInfo.FilesList.Item(ComboBox2.Text))
    End Sub

    ''' <summary>
    ''' 播放至所有屏幕
    ''' </summary>
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If ComboBox1.Text = "" Then
            Exit Sub
        End If

        For Each i In sysInfo.CurtainList
            i.PlayDialog.Play(sysInfo.FilesList.Item(ComboBox2.Text))
        Next
    End Sub

    ''' <summary>
    ''' 互动
    ''' </summary>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        sysInfo.DisplayMode = 0

        'ToolStripDropDownButton1.Text = 运行F1ToolStripMenuItem.Text
        'ToolStripDropDownButton1.BackColor = 运行F1ToolStripMenuItem.BackColor

        Button6.Image = My.Resources.DisplayMode0
        Button7.Image = My.Resources.DisplayMode1G
        Button8.Image = My.Resources.DisplayMode2G
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchPlayMode(True)
        Next
    End Sub

    ''' <summary>
    ''' 测试
    ''' </summary>
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        sysInfo.DisplayMode = 1

        'ToolStripDropDownButton1.Text = 测试F2ToolStripMenuItem.Text
        'ToolStripDropDownButton1.BackColor = 测试F2ToolStripMenuItem.BackColor

        Button6.Image = My.Resources.DisplayMode0G
        Button7.Image = My.Resources.DisplayMode1
        Button8.Image = My.Resources.DisplayMode2G
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchTestMode(True)
        Next
    End Sub

    ''' <summary>
    ''' 黑屏
    ''' </summary>
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        sysInfo.DisplayMode = 2

        'ToolStripDropDownButton1.Text = 黑屏F3ToolStripMenuItem.Text
        'ToolStripDropDownButton1.BackColor = 黑屏F3ToolStripMenuItem.BackColor

        Button6.Image = My.Resources.DisplayMode0G
        Button7.Image = My.Resources.DisplayMode1G
        Button8.Image = My.Resources.DisplayMode2
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchBlankScreenMode(True)
        Next
    End Sub

    ''' <summary>
    ''' 忽略
    ''' </summary>
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        sysInfo.DisplayMode = If(DebugFlage, 4, 3)

        If DebugFlage Then
            For Each i In sysInfo.CurtainList
                i.PlayDialog.SwitchTestMode(True)
            Next
        End If

        'ToolStripDropDownButton1.Text = 忽略F4ToolStripMenuItem.Text
        'ToolStripDropDownButton1.BackColor = 忽略F4ToolStripMenuItem.BackColor

        Button6.Image = My.Resources.DisplayMode0G
        Button7.Image = My.Resources.DisplayMode1G
        Button8.Image = My.Resources.DisplayMode2G
        Button9.Image = My.Resources.DisplayMode3

    End Sub

    ''' <summary>
    ''' 隐藏播放窗体
    ''' </summary>
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If Not CheckBox1.Checked Then
            For i As Integer = 0 To sysInfo.CurtainList.Count - 1
                With sysInfo.CurtainList.Item(i)
                    .PlayDialog.SetLocation(.X, .Y, .Width, .Height)
                End With
            Next
        Else
            For i As Integer = 0 To sysInfo.CurtainList.Count - 1
                With sysInfo.CurtainList.Item(i)
                    .PlayDialog.SetLocation(.X, .Y, .0, .0)
                End With
            Next
        End If
    End Sub

    '定时刷新发送卡状态
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim minReadNum As Integer = &HFFFF
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            With sysInfo.SenderList(i)
                If Not .Link Then
                    Continue For
                End If

                ToolStripDropDownButton2.DropDownItems(i).Text = $"控制器{i}:{ .MaxReadNum}"
                minReadNum = If(minReadNum > .MaxReadNum, .MaxReadNum, minReadNum)
            End With

        Next

        ToolStripDropDownButton2.Image = If(minReadNum, My.Resources.ServerNormal, My.Resources.ServerFault)
    End Sub
End Class