Imports System.ComponentModel
Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
Imports YESInteractiveSDK.ModuleStructure

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
        'Me.TopMost = True

        '测试时关闭登陆验证
        'checkdog()

        System.IO.Directory.CreateDirectory("./data")

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
            With sysInfo
                ReDim .ScreenList(32 - 1)
                .StartLocation.X = Screen.PrimaryScreen.Bounds.Width / 2
                .StartLocation.Y = Screen.PrimaryScreen.Bounds.Height / 2
                .TouchSensitivity = 5
                .ClickValidNums = 2
                .ResetTemp = 5
                .ResetSec = 25
                .touchMode = 1
            End With
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
        End If

        With sysInfo
            '加载语言包
            .Language = New Wangk.Resource.MultiLanguage
            .Language.SelectLanguageId = .SelectLanguageId

            '检测参数
            .ZoomProportion = If(.ZoomProportion, .ZoomProportion, 1)
            .ZoomTmpNumerator = If(.ZoomTmpNumerator, .ZoomTmpNumerator, 1)
            .ZoomTmpDenominator = If(.ZoomTmpDenominator, .ZoomTmpDenominator, 1)
            .InquireTimeSec = 20
        End With

        '恢复关闭前位置
        Me.Location = sysInfo.StartLocation
        If Me.Location.X > Screen.PrimaryScreen.Bounds.Width - Me.Location.X OrElse
                Me.Location.X < 0 OrElse
            Me.Location.Y > Screen.PrimaryScreen.Bounds.Height - Me.Location.Y OrElse
            Me.Location.Y < 0 Then

            Me.Location = New Point(Screen.PrimaryScreen.Bounds.Width / 2,
                                    Screen.PrimaryScreen.Bounds.Height / 2)
        End If

        '设置下拉列表只读
        ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox2.DropDownStyle = ComboBoxStyle.DropDownList

        '注册热键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)

        '发送卡状态
        Timer1.Interval = 1000

        '设置显示语言
        sysInfo.Language.SetControlslanguage(Me)

        '显示版本号
        With My.Application.Info
            Me.Text = $"{ sysInfo.Language.GetLanguage(.ProductName)} V{ .Version.ToString}"
        End With

        '捕获鼠标标志
        CheckBox2.Checked = sysInfo.SetCaptureFlage

        sysInfo.logger = New Wangk.Tools.Logger With {
            .writelevel = Wangk.Tools.Loglevel.Level_DEBUG,
            .saveDaysMax = 30
        }
        sysInfo.logger.Init()

        '读取屏幕配置
        Dim tmpDialog As New FormNovaInit
        tmpDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 创建播放窗体
    ''' </summary>
    Public Sub CreatDialogThread(ByVal id As Integer)
        Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(id)

        tmp.PlayDialog = New FormPlay With {
            .curtainListId = id
        }

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
                sysInfo.ScreenList(i).CurtainListId = -1
                Continue For
            End If

            With sysInfo.ScreenList(i)
                '创建上次点击状态缓存
                ReDim .ClickHistoryArray((.DefaultHeight \ .DefaultScanBoardHeight) * .TouchPieceRowsNum,
                                         (.DefaultWidth \ .DefaultScanBoardWidth) * .TouchPieceColumnsNum)
            End With
        Next
        Dim qwe As Integer = 0

        '清空控制器连接标记
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            sysInfo.SenderList(i).Link = False
        Next
        '判断哪些控制器要连接
        ''TODO: Collection was modified,控制器配置好后未接屏幕,然后第一次启动程序报错,再启动就没问题
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            For Each j In sysInfo.CurtainList.Item(i).ScreenList
                If Not sysInfo.ScreenList(j).ExistFlage Then
                    Continue For
                End If

                If sysInfo.ScreenList(j).CurtainListId < 0 Then
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

        ''等待播放窗体显示完毕
        'Try
        '    Do
        '        Thread.Sleep(100)

        '        With sysInfo.CurtainList
        '            For i As Integer = 0 To .Count - 1
        '                If Not .Item(i).PlayDialog.Visible Then
        '                    Continue Do
        '                End If
        '            Next
        '        End With

        '        Exit Do
        '    Loop
        'Catch ex As Exception
        'End Try

        'Try
        '    For i As Integer = 0 To sysInfo.CurtainList.Count - 1
        '        With sysInfo.CurtainList.Item(i)
        '            '更新位置及大小
        '            .PlayDialog.SetLocation(.X, .Y, .Width, .Height)
        '        End With
        '    Next
        'Catch ex As Exception
        '    '第一次显示播放窗体时尺寸未改变大小
        '    '暂时用此办法解决
        'End Try

        '将焦点移至主窗体
        Me.Activate()
    End Sub

    Private Sub FormMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
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

        Button9.Text = sysInfo.Language.GetLanguage("电容")
    End Sub

    Private Sub FormMain_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '退出前提示
        If MsgBox($"{sysInfo.Language.GetLanguage("确定退出程序")}?", MsgBoxStyle.YesNo, sysInfo.Language.GetLanguage("退出")) = MsgBoxResult.Yes Then
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
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            sysInfo.CurtainList.Item(i).PlayDialog.CloseDialog(True)
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

    ''' <summary>
    ''' 设置
    ''' </summary>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        Dim tmpDialog As New FormOption
        tmpDialog.ShowDialog()

        ShowCurtain()
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位增量温度 K
    ''' </summary>
    Private Sub SetResetTemp(value As Integer)
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010300")
        sendByte(4) = value

        If Not sysInfo.ScanBoardOldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        Thread.Sleep(100)
    End Sub

    ''' <summary>
    ''' 设置 屏幕定时复位时间
    ''' </summary>
    Private Sub SetResetSec(value As Integer)
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010200")
        sendByte(4) = value

        If Not sysInfo.ScanBoardOldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        Thread.Sleep(100)
    End Sub

    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub OnLinkCon()
        Button6.Enabled = True
        Button7.Enabled = True
        Button8.Enabled = True
        Button9.Enabled = True

        Button6_Click(Nothing, Nothing)

        ToolStripButton2.Enabled = False
        ToolStripButton1.Text = sysInfo.Language.GetLanguage("断开连接")
        ToolStripButton1.Image = My.Resources.disconnect

        'GroupBox2.Enabled = True

        Timer1.Start()
    End Sub

    ''' <summary>
    ''' 断开连接
    ''' </summary>
    Private Sub OffLinkCon()
        Button6.Enabled = False
        Button7.Enabled = False
        Button8.Enabled = False
        Button9.Enabled = False

        ToolStripButton2.Enabled = True
        ToolStripButton1.Text = sysInfo.Language.GetLanguage("连接控制器")
        ToolStripButton1.Image = My.Resources.connect

        'GroupBox2.Enabled = False

        Timer1.Stop()
    End Sub

    ''' <summary>
    ''' 连接/断开连接
    ''' </summary>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If Not sysInfo.LinkFlage Then
            '连接控制器
#Region "连接控制器"
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
                            MsgBox($"{ipStr} {sysInfo.Language.GetLanguage("未能连通")}",
                                   MsgBoxStyle.Information,
                                   sysInfo.Language.GetLanguage("连接"))
                            Exit Sub
                        End If
                    End With
                Next
            Catch ex As Exception
                MsgBox($"{sysInfo.Language.GetLanguage("连接异常")}:{ex.Message}",
                                   MsgBoxStyle.Information,
                                   sysInfo.Language.GetLanguage("连接"))
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
                            .SendTimeout = 100,
                            .ReceiveTimeout = 100
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
                sysInfo.LinkFlage = False

                For Each i In sysInfo.SenderList
                    If i.Link = False Then
                        Continue For
                    End If

                    Try
                        With i
                            .WorkThread.Join()
                        End With
                    Catch ex1 As Exception
                    End Try
                Next

                MsgBox($"{sysInfo.Language.GetLanguage("控制器连接异常")}:{ex.Message}",
                       MsgBoxStyle.Information,
                       sysInfo.Language.GetLanguage("连接"))
                Exit Sub
            End Try

            '启动复位功能
            SetResetTemp(sysInfo.ResetTemp)
            SetResetSec(sysInfo.ResetSec)

            OnLinkCon()
#End Region
        Else
            '断开控制器
#Region "断开控制器"
            sysInfo.LinkFlage = False

            Thread.Sleep(300)

            For Each i In sysInfo.SenderList
                If i.Link = False Then
                    Continue For
                End If

                Try
                    With i
                        .WorkThread.Join()
                        '.WorkThread.Abort()
                    End With
                Catch ex As Exception
                End Try
            Next
            'Thread.Sleep(200)

            '关闭复位功能
            SetResetTemp(0)
            SetResetSec(0)

            OffLinkCon()
#End Region
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

        sysInfo.logger.LogThis("控制器连接异常", lastErrorStr, Wangk.Tools.Loglevel.Level_DEBUG)

        MsgBox($"{sysInfo.Language.GetLanguage("控制器连接异常")}:{lastErrorStr},{sysInfo.Language.GetLanguage("请重新连接控制器或重启控制器")}",
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
        '接收的传感器数据
        Dim ReceDataArray As Byte(,)
        ReDim ReceDataArray(16 - 1, 1028 - 1)

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
                Dim tmpThread As Thread = New Thread(AddressOf ShowException) With {
                    .IsBackground = True
                    }
                tmpThread.Start(lastErrorStr)
                'ShowException(lastErrorStr)
                Exit Do
            End If

            'Dim asd As New Stopwatch
            'asd.Start()

            '接收传感器数据
#Region "接收传感器数据"
            Try
                '向发送卡 请求接收传感器数据数据
                Dim bytes(1028 - 1) As Byte
                '发送数据
                Dim bytesSend As Integer = sysInfo.SenderList(senderId).CliSocket.Send(Wangk.Hash.Hex2Bin("55d50902"))
                '接收数据
                Dim bytesRec As Integer = sysInfo.SenderList(senderId).CliSocket.Receive(bytes)

            Catch ex As SocketException
                lastErrorStr = ex.Message
                'sysInfo.logger.LogThis("请求传感器数据通信异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
                exceptionNums += 1

                Thread.Sleep(100)
                Continue Do
                'Catch ex As ThreadAbortException
                '不记录终止异常
            Catch ex As Exception
                sysInfo.logger.LogThis("请求传感器数据其他异常", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try

            '向发送卡 请求发送传感器数据数据
            Dim index As Integer
            Try
                '向发送卡发送数据
                Dim bytes2(1028 - 1) As Byte
                '发送到发送卡数据
                Dim bytesSend2 As Integer = sysInfo.SenderList(senderId).CliSocket.Send(Wangk.Hash.Hex2Bin("55d50905000000000400"))

                '清空历史数据
                For index = 0 To 16 - 1
                    For i1 As Integer = 0 To 1028 - 1
                        ReceDataArray(index, i1) = 0
                    Next
                Next

                '诺瓦每次只发送1K数据，16K数据分16次发送
                For index = 0 To 16 - 1
                    sysInfo.SenderList(senderId).CliSocket.Receive(bytes2)
                    For i1 As Integer = 0 To 1028 - 1
                        ReceDataArray(index, i1) = bytes2(i1)
                    Next
                Next
            Catch ex As SocketException
                lastErrorStr = ex.Message
                'sysInfo.logger.LogThis($"接收传感器数据数据通信异常{index}", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
                exceptionNums += 1
                'Catch ex As ThreadAbortException
                '不记录终止异常
            Catch ex As Exception
                sysInfo.logger.LogThis($"接收传感器数据数据其他异常{index}", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try
#End Region
            readNum += 1

            '处理收到的数据
            Try
                For index = 0 To 16 - 1
                    'TextBox5.Text = ""
                    '分析接收到的数据
                    For j As Integer = 4 To 1027 Step 32
                        '判断有效性
#Region "判断有效性"
                        '有效数据头
                        If ReceDataArray(index, j + 0) <> &H55 Then
                            Continue For
                        End If

                        '网口号大于4则丢弃
                        If ReceDataArray(index, j + 1) > 4 Then
                            Continue For
                        End If

                        If sysInfo.DisplayMode <> 0 And
                            sysInfo.DisplayMode <> 1 And
                            sysInfo.DisplayMode <> 4 Then
                            Exit For
                        End If

                        '查找接收卡位置[由像素改为索引]
                        If sysInfo.ScanBoardTable.Item($"{senderId}-{ReceDataArray(index, j + 1)}-{(ReceDataArray(index, j + 2) * 256 + ReceDataArray(index, j + 3))}") Is Nothing Then
                            Continue For
                        End If
                        Dim tmp As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{senderId}-{ReceDataArray(index, j + 1)}-{(ReceDataArray(index, j + 2) * 256 + ReceDataArray(index, j + 3))}")

                        '未显示则跳过
                        If sysInfo.ScreenList(tmp.ScreenId).CurtainListId < 0 Then
                            Continue For
                        End If
#End Region
                        '计算总点击块
                        Dim tmpClickValidSum As Integer = 0
                        For k = 0 To 4 * 4 - 1
                            tmpClickValidSum = tmpClickValidSum + If(ReceDataArray(index, j + 4 + k) And &H80, 1, 0)
                        Next

                        '按触摸模式计算点击坐标
                        If sysInfo.touchMode = 0 OrElse
                            sysInfo.DisplayMode <> 0 OrElse
                            sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum <> sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum Then
                            '1合1
#Region "1合1"
                            For k As Integer = 0 To 4 - 1 '行数
                                If k >= sysInfo.ScreenList(tmp.ScreenId).TouchPieceRowsNum Then
                                    Continue For
                                End If

                                For l As Integer = 0 To 4 - 1 '列数
                                    If l >= sysInfo.ScreenList(tmp.ScreenId).TouchPieceColumnsNum Then
                                        Continue For
                                    End If

                                    If sysInfo.DisplayMode = 0 OrElse '点击
                                            sysInfo.DisplayMode = 1 Then '测试
                                        '未被点击
                                        If (ReceDataArray(index, j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                            '抬起事件
                                            ''todo:抬起事件
                                            If (sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) And &H80) = &H80 Then
                                                sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                                       PlayDialog.
                                                        MousesimulationClick(tmp.ScreenId,
                                                            tmp.X + l,
                                                            tmp.Y + k,
                                                            0,
                                                            PointActivity.UP)
                                            End If

                                            sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = 0

                                            Continue For
                                        End If

                                        '被点击过
                                        If sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                            '
                                            'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                            Continue For
                                        End If

                                        '新点击点
                                        sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                        If tmpClickValidSum < sysInfo.ClickValidNums AndAlso
                                                sysInfo.DisplayMode = 0 Then
                                            '互动模式下抗干扰才启用
                                            Continue For
                                        End If

                                        'If Not CheckAdjacencyPieceNums(tmp.ScreenId, tmp.Y + k, tmp.X + l) Then
                                        '    Continue For
                                        'End If
                                    End If
                                    '
                                    'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                    'Debug.WriteLine($"download {Format(Now(), "yyyy/MM/dd HH:mm:ss.fff")}")

                                    '按下事件
                                    sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                            PlayDialog.
                                            MousesimulationClick(tmp.ScreenId,
                                                                 tmp.X + l,
                                                                 tmp.Y + k,
                                                                 ReceDataArray(index, j + 4 + k * 4 + l),
                                                                 PointActivity.DOWN)

                                    'Debug.WriteLine($"1-1 {Format(Now(), "HH:mm:ss.fff")} {tmp.X + l },{tmp.Y + k}")
                                Next
                            Next
#End Region

                        ElseIf sysInfo.touchMode = 1 Then
                            '4合1
#Region "4合1"
                            '是否有新点击点标记
                            Dim NewPointActiveFlage As Boolean = False

                            For k As Integer = 0 To 2 - 1 '行数
                                For l As Integer = 0 To 2 - 1 '列数
                                    '是否有新点击点
                                    NewPointActiveFlage = False

                                    For i001 As Integer = 0 To 2 - 1 '行数
                                        For j002 As Integer = 0 To 2 - 1 '列数

                                            Dim xId As Integer = tmp.X + l * 2 + j002
                                            Dim yId As Integer = tmp.Y + k * 2 + i001
                                            '未被点击
                                            If (ReceDataArray(index, j + 4 + k * 8 + l * 2 + i001 * 4 + j002) And &H80) <> &H80 Then
                                                '抬起事件
                                                sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(yId, xId) = 0

                                                Continue For
                                            End If

                                            '被点击过
                                            If sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(yId, xId) Then
                                                'sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80
                                                'Debug.WriteLine("C")
                                                Continue For
                                            End If

                                            '新点击点
                                            sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(yId, xId) = &H80

                                            NewPointActiveFlage = True

                                            'Debug.WriteLine(k * 8 + l * 2 + i001 * 4 + j002)
                                        Next
                                    Next

                                    If tmpClickValidSum < sysInfo.ClickValidNums Then
                                        '互动模式下抗干扰才启用
                                        Continue For
                                    End If

                                    If Not NewPointActiveFlage Then
                                        Continue For
                                    End If

                                    '按下事件
                                    sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                            PlayDialog.
                                            MousesimulationClick(tmp.ScreenId,
                                                                 tmp.X + l * 2,
                                                                 tmp.Y + k * 2,
                                                                 0,
                                                                 PointActivity.DOWN)
                                    'Debug.WriteLine($"4-1 {NewPointActiveFlage} {Format(Now(), "HH:mm:ss.fff")} {tmp.X + l * 2},{tmp.Y + k * 2}")
                                Next
                            Next
#End Region

                        ElseIf sysInfo.touchMode = 2 Then
                            '16合1
#Region "16合1"
                            '是否有新点击点标记
                            Dim NewPointActiveFlage As Boolean = False

                            For k As Integer = 0 To 4 - 1 '行数
                                For l As Integer = 0 To 4 - 1 '列数

                                    '未被点击
                                    If (ReceDataArray(index, j + 4 + k * 4 + l) And &H80) <> &H80 Then
                                        '抬起事件
                                        sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = 0

                                        Continue For
                                    End If

                                    '被点击过
                                    If sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) Then
                                        Continue For
                                    End If

                                    '新点击点
                                    sysInfo.ScreenList(tmp.ScreenId).ClickHistoryArray(tmp.Y + k, tmp.X + l) = &H80

                                    NewPointActiveFlage = True
                                Next
                            Next

                            If tmpClickValidSum < sysInfo.ClickValidNums Then
                                '互动模式下抗干扰才启用
                                Continue For
                            End If

                            If Not NewPointActiveFlage Then
                                Continue For
                            End If

                            '按下事件
                            sysInfo.CurtainList.Item(sysInfo.ScreenList(tmp.ScreenId).CurtainListId).
                                    PlayDialog.
                                    MousesimulationClick(tmp.ScreenId,
                                                         tmp.X,
                                                         tmp.Y,
                                                         0,
                                                         PointActivity.DOWN)

                            'Debug.WriteLine($"16-1 {Format(Now(), "HH:mm:ss.fff")} {tmp.X },{tmp.Y }")

#End Region
                        End If

                    Next
                Next

            Catch ex As Exception
                sysInfo.logger.LogThis($"处理传感器数据数据异常{index}", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try

            Thread.Sleep(sysInfo.InquireTimeSec)
        Loop

        Try
            sysInfo.SenderList(senderId).CliSocket.Close()
        Catch ex As Exception
        End Try

        Debug.WriteLine($"sender{senderId} exit")
    End Sub

    '选择幕布
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        ComboBox2.Text = sysInfo.CurtainList.Item(ComboBox1.SelectedIndex).file
    End Sub

    '添加文件
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        '添加flash文件
        Dim tmp As New OpenFileDialog
        tmp.Filter = "Flash或DLL插件|*.SWF;*.DLL"
        'tmp.Filter = "Flash|*.SWF"
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
    ''' 上次选中播放文件名
    ''' </summary>
    Dim LastSelectPlayFile As String = Nothing
    ''' <summary>
    ''' 播放
    ''' </summary>
    Private Sub ComboBox2_DropDownClosed(sender As Object, e As EventArgs) Handles ComboBox2.DropDownClosed
        If ComboBox1.Text = "" Then
            Exit Sub
        End If

        If sysInfo.DisplayMode <> 0 Then
            Exit Sub
        End If

        If ComboBox2.SelectedIndex < 0 OrElse
            LastSelectPlayFile = ComboBox2.Items(ComboBox2.SelectedIndex) Then
            Exit Sub
        End If

        'LastSelectPlayFile = ComboBox2.Items(ComboBox2.SelectedIndex)

        sysInfo.CurtainList.Item(ComboBox1.SelectedIndex).PlayDialog.Play(sysInfo.FilesList.Item(ComboBox2.Text))
        Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(ComboBox1.SelectedIndex)
        tmp.file = ComboBox2.Text
        sysInfo.CurtainList.Item(ComboBox1.SelectedIndex) = tmp
    End Sub

    Private Sub ComboBox2_TextChanged(sender As Object, e As EventArgs) Handles ComboBox2.TextChanged
        LastSelectPlayFile = ComboBox2.Text
    End Sub

    ''' <summary>
    ''' 互动
    ''' </summary>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        sysInfo.DisplayMode = 0

        Button6.Image = My.Resources.DisplayMode0
        Button7.Image = My.Resources.DisplayMode1G
        Button8.Image = My.Resources.DisplayMode2G
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchPlayMode(True)
        Next

        'GroupBox2.Enabled = True
    End Sub

    ''' <summary>
    ''' 测试
    ''' </summary>
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        sysInfo.DisplayMode = 1

        Button6.Image = My.Resources.DisplayMode0G
        Button7.Image = My.Resources.DisplayMode1
        Button8.Image = My.Resources.DisplayMode2G
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchTestMode(True)
        Next

        'GroupBox2.Enabled = False
    End Sub

    ''' <summary>
    ''' 黑屏
    ''' </summary>
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        sysInfo.DisplayMode = 2

        Button6.Image = My.Resources.DisplayMode0G
        Button7.Image = My.Resources.DisplayMode1G
        Button8.Image = My.Resources.DisplayMode2
        Button9.Image = My.Resources.DisplayMode3G

        For Each i In sysInfo.CurtainList
            i.PlayDialog.SwitchBlankScreenMode(True)
        Next

        'GroupBox2.Enabled = False
    End Sub

    ''' <summary>
    ''' 忽略
    ''' </summary>
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        sysInfo.DisplayMode = If(DebugFlage, 4, 3)

        'Debug.WriteLine("mode:" + sysInfo.DisplayMode.ToString)

        If DebugFlage Then
            For Each i In sysInfo.CurtainList
                i.PlayDialog.SwitchTestMode(True)
            Next
        End If

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

    ''' <summary>
    ''' 捕获鼠标
    ''' </summary>
    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        sysInfo.SetCaptureFlage = CheckBox2.Checked
    End Sub

    ''' 定时刷新发送卡状态
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        '修正检测间隔
#Region "修正检测间隔"
        'Debug.Write($"{Format(Now(), "HH:mm:ss")} {sysInfo.InquireTimeSec}-")

        Dim minReadNum As Integer = &HFFFF
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            With sysInfo.SenderList(i)
                If Not .Link Then
                    Continue For
                End If

                minReadNum = If(minReadNum > .MaxReadNum, .MaxReadNum, minReadNum)
            End With

        Next

        If minReadNum < 40 AndAlso
            sysInfo.InquireTimeSec > 0 Then
            sysInfo.InquireTimeSec -= 1
        ElseIf minReadNum > 42 Then
            sysInfo.InquireTimeSec += 1
        End If
#End Region
    End Sub

End Class