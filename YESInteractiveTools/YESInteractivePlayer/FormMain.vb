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

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '测试时关闭登陆验证
        'checkdog()

        '读取最后编译日期
        Dim txtTmp As System.IO.TextReader = System.IO.File.OpenText(".\data\CreationDate.ini")
        Me.Text = $"{My.Application.Info.ProductName}" ' [{txtTmp.ReadLine()}]"

        System.IO.Directory.CreateDirectory("./data")
        System.IO.Directory.CreateDirectory("./logs")

        Putlog("启动 " & Application.ExecutablePath)

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
                'End
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
        sysInfo.ResetTemp = If(sysInfo.ResetTemp, sysInfo.ResetTemp, 1)
        sysInfo.ResetSec = If(sysInfo.ResetSec, sysInfo.ResetSec, 1)

        Me.Location = sysInfo.StartLocation

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)

        '运行F1ToolStripMenuItem.BackColor = Color.FromArgb(&H0, &HE3, &HB)
        测试F2ToolStripMenuItem.BackColor = Color.FromArgb(&HFF, &H7F, &H27)
        显示电容f4ToolStripMenuItem.BackColor = Color.FromArgb(&H2D, &H94, &HB0)
        黑屏F3ToolStripMenuItem.BackColor = Color.FromArgb(&H7F, &H7F, &H7F)
        忽略F4ToolStripMenuItem.BackColor = Color.FromArgb(&HB0, &HAF, &HDF)
        ToolStripDropDownButton1.BackColor = 测试F2ToolStripMenuItem.BackColor

        '发送卡状态
        Timer1.Interval = 1000

        '删除旧log文件
        DeleteLog(30)

        '设置显示语言
        SetControlslanguage(Me)
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
        '显示标记的幕布窗体
        Dim tmpCreatDialogThread As Threading.Thread
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            With sysInfo.CurtainList.Item(i)
                'ComboBox1.Items.Add($"{i} { .Remark}")

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
                    sysInfo.SenderList(k).Link = True
                Next
            Next
        Next

        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            ToolStripDropDownButton2.DropDownItems(i).Enabled = sysInfo.SenderList(i).Link
        Next

        Thread.Sleep(1000)
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
                    测试F2ToolStripMenuItem_Click(Nothing, Nothing)
                Case 2
                    显示电容f4ToolStripMenuItem_Click(Nothing, Nothing)
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

        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)

        '关闭播放窗体
        For Each i In sysInfo.CurtainList
            i.PlayDialog.CloseDialog(True)
        Next

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

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
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
    End Sub

    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub OnLinkCon()
        ToolStripDropDownButton1.Enabled = True
        Button5.Enabled = False
        ToolStripButton1.Text = GetLanguage("断开连接")
        ToolStripButton1.BackColor = Color.OrangeRed
        Timer1.Start()
    End Sub

    ''' <summary>
    ''' 断开连接
    ''' </summary>
    Private Sub OffLinkCon()
        ToolStripDropDownButton1.Enabled = False
        Button5.Enabled = True
        ToolStripButton1.Text = GetLanguage("连接控制器")
        ToolStripButton1.BackColor = Color.FromArgb(&H0, &HE3, &HB)
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

            '建立与控制器的连接
            Try
                For i As Integer = 0 To sysInfo.SenderList.Length - 1
                    If sysInfo.SenderList(i).Link = False Then
                        Continue For
                    End If

                    With sysInfo.SenderList(i)
                        Dim ipStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"

                        .CliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
                            .SendTimeout = 1000,
                            .ReceiveTimeout = 1000
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

                MsgBox($"控制器连接错误:{ex.Message}",
                       MsgBoxStyle.Information,
                       "连接")
                Exit Sub
            End Try

            '启动复位功能
            SetResetTemp(sysInfo.ResetTemp)
            SetResetSec(sysInfo.ResetSec)

            OnLinkCon()
            sysInfo.LinkFlage = True
        Else
            '断开控制器

            For Each i In sysInfo.SenderList
                If i.Link = False Then
                    Continue For
                End If

                With i
                    .WorkThread.Abort()
                    .CliSocket.Close()
                End With
            Next

            '关闭复位功能
            SetResetTemp(0)
            SetResetSec(0)

            OffLinkCon()
            sysInfo.LinkFlage = False
        End If
    End Sub

    ''' <summary>
    ''' 异常时断开连接并提示
    ''' </summary>
    Public Delegate Sub showExceptionCallback(ByVal nums As Integer)
    Public Sub ShowException(ByVal nums As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New showExceptionCallback(AddressOf ShowException), New Object() {nums})
            Exit Sub
        End If

        ToolStripButton1_Click(Nothing, Nothing)

        MsgBox($"控制器已连续 {nums} 次未返回数据!",
               MsgBoxStyle.Information,
               Me.Text)
    End Sub

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

        Do
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
                ShowException(exceptionNums)
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
                Dim bytesSend As Integer = sysInfo.SenderList(senderId).CliSocket.Send(sendbytes)

                '诺瓦每次只发送1K数据，16K数据分16次发送
                For i As Integer = 0 To 16 - 1

                    Dim bytesRec As Integer = sysInfo.SenderList(senderId).CliSocket.Receive(bytes)
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

                        If sysInfo.DisplayMode <> 0 And
                            sysInfo.DisplayMode <> 1 Then
                            Exit For
                        End If

                        '查找接收卡位置[由像素改为索引]
                        If sysInfo.ScanBoardTable.Item($"{senderId}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}") Is Nothing Then
                            Continue For
                        End If
                        Dim tmp As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{senderId}-{bytes(j + 1)}-{(bytes(j + 2) * 256 + bytes(j + 3))}")

                        ''计算总点击块
                        'Dim tmpClickValidSum As Integer = 0
                        'For k = 0 To sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum * sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum - 1
                        '    tmpClickValidSum = tmpClickValidSum + If(bytes(j + 4 + k) And &H80, 1, 0)
                        'Next

                        'If tmpClickValidSum < sysInfo.ClickValidNums Then
                        '    Continue For
                        'End If

                        'Debug.WriteLine(tmpClickValidSum)

                        For k As Integer = 0 To sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum - 1
                            For l As Integer = 0 To sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum - 1
                                'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = bytes(j + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l) And &H80

                                Select Case sysInfo.DisplayMode
                                    Case 0
                                        If (bytes(j + 4 + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l) And &H80) <> &H80 Then
                                            'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = 0
                                            Continue For
                                        End If

                                        sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                            PlayDialog.
                                            MousesimulationClick(tmp.ScreenId,
                                                                 tmp.X + l,
                                                                 tmp.Y + k,
                                                                 bytes(j + 4 + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l))
                                    Case 1
                                        sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                            PlayDialog.
                                            MousesimulationClick(tmp.ScreenId,
                                                                 tmp.X + l,
                                                                 tmp.Y + k,
                                                                 bytes(j + 4 + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l))
                                End Select


                                'If sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                '    sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                '    Continue For
                                'End If

                                'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                'sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                '    PlayDialog.
                                '    MousesimulationClick(tmp.ScreenId,
                                '                         tmp.X + l,
                                '                         tmp.Y + k,
                                '                         bytes(j + 4 + k * sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum + l))
                            Next
                        Next

                    Next
                Next

                readNum += 1
            Catch ex As Exception
                exceptionNums += 1
            End Try

            Thread.Sleep(sysInfo.InquireTimeSec)
        Loop
    End Sub

    Private Sub 测试F2ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 测试F2ToolStripMenuItem.Click
        sysInfo.DisplayMode = 0

        ToolStripDropDownButton1.Text = 测试F2ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 测试F2ToolStripMenuItem.BackColor

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchTestMode(True)
        Next
    End Sub

    Private Sub 显示电容f4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 显示电容f4ToolStripMenuItem.Click
        sysInfo.DisplayMode = 1

        ToolStripDropDownButton1.Text = 显示电容f4ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 显示电容f4ToolStripMenuItem.BackColor

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchTestMode(True)
        Next
    End Sub

    Private Sub 黑屏F3ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 黑屏F3ToolStripMenuItem.Click
        sysInfo.DisplayMode = 2

        ToolStripDropDownButton1.Text = 黑屏F3ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 黑屏F3ToolStripMenuItem.BackColor

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchBlankScreenMode(True)
        Next
    End Sub

    Private Sub 忽略F4ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 忽略F4ToolStripMenuItem.Click
        sysInfo.DisplayMode = 3

        ToolStripDropDownButton1.Text = 忽略F4ToolStripMenuItem.Text
        ToolStripDropDownButton1.BackColor = 忽略F4ToolStripMenuItem.BackColor
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

        ToolStripDropDownButton2.BackColor = If(minReadNum, Color.FromArgb(&H0, &HE3, &HB), Color.Yellow)
    End Sub
End Class