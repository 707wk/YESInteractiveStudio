Imports System.Net.Sockets
Imports System.Threading

Public Module ModuleNovaMCTRL510
#Region "连接控制器"
    ''' <summary>
    ''' 连接控制器
    ''' </summary>
    Public Function ConnectControl() As Boolean
#Region "重建历史点击状态"
        '重建历史点击状态
        For i001 As Integer = 0 To sysInfo.ScreenList.Count - 1
            With sysInfo.ScreenList(i001)
                .WindowId = -1

                ReDim .SensorMap((.DefSize.Height \ .DefScanBoardSize.Height) * .SensorLayout.Height,
                                        (.DefSize.Width \ .DefScanBoardSize.Width) * .SensorLayout.Width)
            End With
        Next
#End Region

#Region "标记要连接的控制器"
        For i002 As Integer = 0 To sysInfo.SenderList.Count - 1
            With sysInfo.SenderList(i002)
                .LinkFlage = False
            End With
        Next

        For i003 As Integer = 0 To sysInfo.Schedule.WindowList.Count - 1
            With sysInfo.Schedule.WindowList(i003)
                '遍历窗口内屏幕
                For Each j003 As Integer In .ScreenList
                    '屏幕不存在则跳过
                    If j003 > sysInfo.ScreenList.Count - 1 Then
                        Continue For
                    End If

                    sysInfo.ScreenList(j003).WindowId = i003

                    '遍历屏幕所在控制器
                    For Each k003 As Integer In sysInfo.ScreenList(j003).SenderList
                        '控制器不存在则跳过
                        If k003 > sysInfo.SenderList.Count - 1 Then
                            Continue For
                        End If

                        sysInfo.SenderList(k003).LinkFlage = True
                    Next
                Next
            End With
        Next
#End Region

#Region "检测是否连通"
        '检测是否连通
        Try
            For Each i001 As SenderInfo In sysInfo.SenderList
                With i001
                    If Not .LinkFlage Then
                        Continue For
                    End If

                    Dim TmpStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"
                    If Not My.Computer.Network.Ping(TmpStr, 500) Then
                        MsgBox($"{TmpStr} {sysInfo.Language.GetS("Failed to connect")}",
                               MsgBoxStyle.Information,
                               sysInfo.Language.GetS("Test Connect"))
                        Return False
                        Exit Function
                    End If
                End With
            Next
        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   sysInfo.Language.GetS("Connect Exception"))
            Return False
            Exit Function
        End Try
#End Region

        sysInfo.LinkFlage = True

        SetResetSec(sysInfo.ResetSec, sysInfo.ScanBoardOldFlage)
        SetResetTemp(sysInfo.ResetTemp, sysInfo.ScanBoardOldFlage)
        SetTouchSensitivity(sysInfo.TouchSensitivity, sysInfo.ScanBoardOldFlage)

#Region "建立连接并启动检测线程"
        '建立连接并启动检测线程
        Try
            For i002 As Integer = 0 To sysInfo.SenderList.Count - 1
                With sysInfo.SenderList(i002)
                    If Not .LinkFlage Then
                        Continue For
                    End If

                    Dim TmpStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"
                    .CliSocket = New Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp) With {
                                            .SendTimeout = 100,
                                            .ReceiveTimeout = 100
                    }
                    .CliSocket.Connect(TmpStr, 6000)

                    .WorkThread = New Threading.Thread(AddressOf ControlWorkThread) With {
                        .IsBackground = True'后台启动
                    }
                    .WorkThread.Start(i002)
                End With
            Next
        Catch ex As Exception
            sysInfo.LinkFlage = False

            For Each i003 As SenderInfo In sysInfo.SenderList
                With i003
                    Try
                        .WorkThread.Join()
                    Catch ex002 As Exception
                    End Try
                End With
            Next

            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   sysInfo.Language.GetS("Connected Exception"))
            Return False
            Exit Function
        End Try
#End Region
        Return True
    End Function
#End Region

#Region "断开控制器连接"
    ''' <summary>
    ''' 断开控制器连接
    ''' </summary>
    Public Function DisconnectControl() As Boolean
        sysInfo.LinkFlage = False

        For Each i001 As SenderInfo In sysInfo.SenderList
            With i001
                If .WorkThread IsNot Nothing Then
                    .WorkThread.Join()
                End If
            End With
        Next

        SetResetSec(0, sysInfo.ScanBoardOldFlage)
        SetResetTemp(0, sysInfo.ScanBoardOldFlage)

        Return True
    End Function
#End Region

#Region "处理线程"
#Region "检测相邻触发数是否大于等于抗干扰数"
    ''' <summary>
    ''' 检测相邻触发数是否大于等于抗干扰数
    ''' </summary>
    Private Function CheckAdjacencyPieceNums(ScreenId As Integer, Point As Point) As Boolean
        '忽略自身
        Dim checkNums As Integer = 0

        For rowID = -1 To 1
            For colID = -1 To 1

                '屏幕边缘则跳过
                If Point.X + colID < 0 OrElse
                   Point.Y + rowID < 0 OrElse
                    Point.Y + rowID >= sysInfo.ScreenList(ScreenId).SensorMap.GetLength(0) - 1 OrElse
                    Point.X + colID >= sysInfo.ScreenList(ScreenId).SensorMap.GetLength(1) - 1 Then
                    Continue For
                End If

                If sysInfo.ScreenList(ScreenId).SensorMap(Point.Y + rowID, Point.X + colID) = PointState.DOWN OrElse
                    sysInfo.ScreenList(ScreenId).SensorMap(Point.Y + rowID, Point.X + colID) = PointState.PRESS Then
                    checkNums += 1
                End If
            Next
        Next

        'Debug.WriteLine($"{ScreenId} {Point.X},{Point.Y}:{checkNums}")

        Return checkNums >= sysInfo.ClickValidNums
    End Function
#End Region

    ''' <summary>
    ''' 处理线程
    ''' </summary>
    ''' <param name="ControlID">控制器ID</param>
    Public Sub ControlWorkThread(ByVal ControlID As Integer)
        '定时刷新
        Dim lastSec As Integer = -1
        '异常次数
        Dim exceptionNum As Integer = 0
        '读取次数
        Dim readNum As Integer = 0

        '接收缓存
        Dim ScanBoardDateQueue As New Queue(Of Byte())
        ''检测到的活动点队列
        'Dim NewClickList As New Queue(Of ActivePointInfo)

        'Dim testTime As New Stopwatch
        Do While sysInfo.LinkFlage
            'testTime.Restart()

#Region "刷新数据"
            If lastSec <> Now.Second Then
                lastSec = Now.Second

                exceptionNum = 0
                sysInfo.SenderList(ControlID).MaxReadNum = readNum
                readNum = 0
            End If
#End Region

#Region "异常处理"
            If exceptionNum > 8 Then
                Dim tmpThread As Thread = New Thread(AddressOf sysInfo.MainForm.DisposeControlOffLink) With {
                    .IsBackground = True
                    }
                tmpThread.Start(ControlID)
                Exit Do
            End If
#End Region

#Region "接收传感器数据"
#Region "控制器接收数据"
            Try
                Dim ReceiveData(1024) As Byte
                With sysInfo.SenderList(ControlID).CliSocket
                    .Send(Wangk.Hash.Hex2Bin("55D50902"))
                    .Receive(ReceiveData)
                End With
            Catch ex As SocketException
                sysInfo.LastErrorInfo = ex.Message
                exceptionNum += 1
                'Debug.WriteLine("rec:" & ex.Message)
                'Thread.Sleep(100)
                Continue Do
            Catch ex As Exception
                sysInfo.logger.LogThis("控制器接收数据", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try
#End Region

#Region "控制器上传数据"
            Try
                With sysInfo.SenderList(ControlID).CliSocket
                    .Send(Wangk.Hash.Hex2Bin("55D50905000000000400"))

                    '数据包
                    Dim ReceiveData() As Byte
                    '接收卡数据
                    Dim ScanBoardDate() As Byte

                    For i001 As Integer = 0 To 16 - 1
                        ReDim ReceiveData(1028 - 1)

                        .Receive(ReceiveData)

#Region "预处理数据"
                        For PacketId As Integer = 4 To ReceiveData.Count - 1 Step 32
#Region "有效性校验"
                            '有效数据头
                            If ReceiveData(PacketId) <> &H55 Then
                                Continue For
                            End If

                            '网口号不大于4
                            If ReceiveData(PacketId + 1) > 4 Then
                                Continue For
                            End If

                            '黑屏则不处理
                            If sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.BLACK Then
                                Continue Do
                            End If

                            '查找接收卡位置[由像素改为索引]
                            If sysInfo.ScanBoardTable.Item($"{ControlID}-{ReceiveData(PacketId + 1)}-{(ReceiveData(PacketId + 2) * 256 + ReceiveData(PacketId + 3))}") Is Nothing Then
                                Continue For
                            End If
                            Dim tmpScanBoardInfo As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{ControlID}-{ReceiveData(PacketId + 1)}-{(ReceiveData(PacketId + 2) * 256 + ReceiveData(PacketId + 3))}")

                            '未显示则跳过
                            If sysInfo.ScreenList(tmpScanBoardInfo.ScreenId).WindowId < 0 Then
                                Continue For
                            End If
#End Region

#Region "添加到待处理队列"
                            ReDim ScanBoardDate(32 - 1)
                            For i002 As Integer = 0 To ScanBoardDate.Count - 1
                                ScanBoardDate(i002) = ReceiveData(PacketId + i002)
                            Next
                            ScanBoardDateQueue.Enqueue(ScanBoardDate)
#End Region

#Region "更新点状态"
                            With sysInfo.ScreenList(tmpScanBoardInfo.ScreenId)
                                '行
                                For rowID As Integer = 0 To 4 - 1
                                    If rowID >= .SensorLayout.Height Then
                                        Exit For
                                    End If

                                    '列
                                    For colID As Integer = 0 To 4 - 1
                                        If colID >= .SensorLayout.Width Then
                                            Exit For
                                        End If

                                        Dim Value As Byte = ScanBoardDate(4 + rowID * 4 + colID）
                                        Dim Point As New Point(tmpScanBoardInfo.Location.X + colID,
                                                           tmpScanBoardInfo.Location.Y + rowID)

#Region "无点"
                                        '无点
                                        If (Value And &H80) <> &H80 Then
                                            '抬起
                                            If .SensorMap(Point.Y, Point.X) = PointState.DOWN OrElse
                                                .SensorMap(Point.Y, Point.X) = PointState.PRESS Then

                                                .SensorMap(Point.Y, Point.X) = PointState.UP
                                            End If

                                            .SensorMap(Point.Y, Point.X) = PointState.NOOPS
                                            Continue For
                                        End If
#End Region

#Region "旧点"
                                        '旧点
                                        If .SensorMap(Point.Y, Point.X) = PointState.DOWN OrElse
                                            .SensorMap(Point.Y, Point.X) = PointState.PRESS Then

                                            .SensorMap(Point.Y, Point.X) = PointState.PRESS
                                            Continue For
                                        End If
#End Region

#Region "新点"
                                        '新点
                                        .SensorMap(Point.Y, Point.X) = PointState.DOWN
#End Region

                                    Next
                                Next

                            End With
#End Region

                        Next
#End Region
                    Next
                End With
            Catch ex As SocketException
                sysInfo.LastErrorInfo = ex.Message
                exceptionNum += 1
                'Debug.WriteLine("控制器上传数据:" & ex.Message)
            Catch ex As Exception
                sysInfo.logger.LogThis("控制器上传数据", ex.ToString, Wangk.Tools.Loglevel.Level_DEBUG)
            End Try
#End Region

            readNum += 1
#End Region

#Region "处理数据"
            Try
                Do While ScanBoardDateQueue.Count > 0
                    Dim ScanBoardDate() As Byte = ScanBoardDateQueue.Dequeue
                    Dim tmpScanBoardInfo As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{ControlID}-{ScanBoardDate(1)}-{(ScanBoardDate(2) * 256 + ScanBoardDate(3))}")

#Region "处理触发传感器"
                    With sysInfo.ScreenList(tmpScanBoardInfo.ScreenId)
                        If sysInfo.TouchMode = InteractiveOptions.TOUCHMODE.T121 OrElse
                            sysInfo.DisplayMode <> 0 OrElse
                            .SensorLayout.Width <> .SensorLayout.Height Then
#Region "1合1"
                            For rowID As Integer = 0 To 4 - 1
                                If rowID >= .SensorLayout.Height Then
                                    Exit For
                                End If

                                For colID As Integer = 0 To 4 - 1
                                    If colID >= .SensorLayout.Width Then
                                        Exit For
                                    End If

                                    Dim Value As Byte = ScanBoardDate(4 + rowID * 4 + colID）
                                    Dim Point As New Point(tmpScanBoardInfo.Location.X + colID,
                                                           tmpScanBoardInfo.Location.Y + rowID)

                                    If sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT OrElse
                                            sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.TEST Then
#Region "无点"
                                        '无点
                                        If (Value And &H80) <> &H80 Then
                                            '抬起
                                            If .SensorMap(Point.Y, Point.X) = PointState.DOWN OrElse
                                                .SensorMap(Point.Y, Point.X) = PointState.PRESS Then

                                                sysInfo.Schedule.WindowList.
                                                    Item(.WindowId).
                                                    PlayDialog.
                                                    PointActive(tmpScanBoardInfo.ScreenId,
                                                                Point,
                                                                Value,
                                                                YESInteractiveSDK.PointActivity.UP)
                                            End If

                                            Continue For
                                        End If
#End Region

#Region "旧点"
                                        '旧点
                                        If .SensorMap(Point.Y, Point.X) = PointState.PRESS Then
                                            Continue For
                                        End If
#End Region

                                        '互动模式下抗干扰启用
                                        '旧
                                        'If activateSensorNum < sysInfo.ClickValidNums AndAlso
                                        '        sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT Then
                                        '    Continue For
                                        'End If
                                        '新
                                        If Not CheckAdjacencyPieceNums(tmpScanBoardInfo.ScreenId, Point) AndAlso
                                            sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT Then

                                            Continue For
                                        End If

                                    End If

#Region "按下"
                                    '按下
                                    sysInfo.Schedule.WindowList.
                                        Item(.WindowId).
                                        PlayDialog.
                                        PointActive(tmpScanBoardInfo.ScreenId,
                                                    Point,
                                                    Value,
                                                    YESInteractiveSDK.PointActivity.DOWN)
#End Region
                                Next
                            Next
#End Region
                        ElseIf sysInfo.TouchMode = InteractiveOptions.TOUCHMODE.T421 Then
#Region "4合1"
                            '新点点击标记
                            Dim NewPointActiveFlage As Boolean = False
                            '触发传感器个数
                            Dim activateSensorNum As Integer = 0
                            For i001 As Integer = 0 To 16 - 1
                                activateSensorNum += If(ScanBoardDate(4 + i001) And &H80, 1, 0)
                            Next

                            For rowID As Integer = 0 To 2 - 1
                                For colID As Integer = 0 To 2 - 1
                                    NewPointActiveFlage = False

                                    Dim Point As New Point(tmpScanBoardInfo.Location.X + colID * 2,
                                                           tmpScanBoardInfo.Location.Y + rowID * 2)

                                    For lampRowID As Integer = 0 To 2 - 1
                                        For lampColID As Integer = 0 To 2 - 1
                                            Dim xID As Integer = Point.X + lampColID
                                            Dim yID As Integer = Point.Y + lampRowID
                                            Dim Value As Byte = ScanBoardDate(4 + rowID * 8 + colID * 2 + lampRowID * 4 + lampColID）
#Region "无点"
                                            '无点
                                            If (Value And &H80) <> &H80 Then
                                                Continue For
                                            End If
#End Region

#Region "旧点"
                                            '旧点
                                            If .SensorMap(yID, xID) = PointState.PRESS Then
                                                Continue For
                                            End If
#End Region

#Region "新点"
                                            '新点
                                            NewPointActiveFlage = True
#End Region

                                        Next
                                    Next

                                    '互动模式下抗干扰启用
                                    If activateSensorNum < sysInfo.ClickValidNums Then
                                        Continue For
                                    End If

                                    '没有新点则不触发
                                    If Not NewPointActiveFlage Then
                                        Continue For
                                    End If

#Region "按下"
                                    '按下
                                    sysInfo.Schedule.WindowList.
                                        Item(.WindowId).
                                        PlayDialog.
                                        PointActive(tmpScanBoardInfo.ScreenId,
                                                    Point,
                                                    0,
                                                    YESInteractiveSDK.PointActivity.DOWN)
#End Region
                                Next
                            Next
#End Region
                        Else 'If sysInfo.TouchMode = InteractiveOptions.TOUCHMODE.T1621 Then
#Region "16合1"
                            '新点点击标记
                            Dim NewPointActiveFlage As Boolean = False
                            '触发传感器个数
                            Dim activateSensorNum As Integer = 0
                            For i001 As Integer = 0 To 16 - 1
                                activateSensorNum += If(ScanBoardDate(4 + i001) And &H80, 1, 0)
                            Next

                            For rowID As Integer = 0 To 4 - 1
                                For colID As Integer = 0 To 4 - 1
                                    Dim Value As Byte = ScanBoardDate(4 + rowID * 4 + colID）
                                    'Dim ValueID As Integer = PacketId + 4 + rowId * 4 + colId
                                    Dim Point As New Point(tmpScanBoardInfo.Location.X + colID,
                                                           tmpScanBoardInfo.Location.Y + rowID)

#Region "无点"
                                    '无点
                                    If (Value And &H80) <> &H80 Then
                                        Continue For
                                    End If
#End Region

#Region "旧点"
                                    '旧点
                                    If .SensorMap(Point.Y, Point.X) = PointState.PRESS Then
                                        Continue For
                                    End If
#End Region

#Region "新点"
                                    '新点
                                    NewPointActiveFlage = True
#End Region

                                Next
                            Next

                            '互动模式下抗干扰启用
                            If activateSensorNum < sysInfo.ClickValidNums Then
                                Continue Do
                            End If

                            '没有新点则不触发
                            If Not NewPointActiveFlage Then
                                Continue Do
                            End If

#Region "按下"
                            '按下
                            sysInfo.Schedule.WindowList.
                                        Item(.WindowId).
                                        PlayDialog.
                                        PointActive(tmpScanBoardInfo.ScreenId,
                                                    tmpScanBoardInfo.Location,
                                                    0,
                                                    YESInteractiveSDK.PointActivity.DOWN)
#End Region
#End Region
                        End If
                    End With
#End Region
                Loop
            Catch ex As Exception
            End Try
#End Region
            'Debug.WriteLine($"End {testTime.ElapsedMilliseconds}ms")

            Thread.Sleep(sysInfo.InquireTimeSec)
        Loop

        Try
            sysInfo.SenderList(ControlID).CliSocket.Close()
        Catch ex As Exception
        End Try

        Debug.WriteLine($"Control:{ControlID} Exit")
    End Sub
#End Region

#Region "MCU传感器灵敏度"
    ''' <summary>
    ''' MCU传感器灵敏度
    ''' </summary>
    ''' <param name="Value">1-9级,越大越灵敏</param>
    ''' <param name="OldFlage">启用旧版SDK</param>
    Public Sub SetTouchSensitivity(ByVal Value As Integer, ByVal OldFlage As Boolean)
        If Value < 1 Then
            Value = 1
        ElseIf Value > 9 Then
            Value = 9
        End If

        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb0305")
        sendByte(3) = Value

        Dim result As Boolean
        If Not OldFlage Then
            result = sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            result = sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        If result Then
            '触摸灵敏度
            sysInfo.TouchSensitivity = Value
        End If

        '等待MCU接收完毕
        Thread.Sleep(100)
    End Sub
#End Region

#Region "MCU传感器温度增量复位"
    ''' <summary>
    ''' MCU传感器温度增量复位
    ''' </summary>
    ''' <param name="Value">绝对变化值 0-255度</param>
    ''' <param name="OldFlage">启用旧版SDK</param>
    Private Sub SetResetTemp(ByVal Value As Integer, ByVal OldFlage As Boolean)
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010300")
        sendByte(4) = Value

        If Not OldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        '等待MCU接收完毕
        Thread.Sleep(100)
    End Sub
#End Region

#Region "MCU传感器定时复位"
    ''' <summary>
    ''' MCU传感器定时复位
    ''' </summary>
    ''' <param name="Value">0-255秒</param>
    ''' <param name="OldFlage">启用旧版SDK</param>
    Private Sub SetResetSec(ByVal Value As Integer, ByVal OldFlage As Boolean)
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010200")
        sendByte(4) = Value

        If Not OldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        '等待MCU接收完毕
        Thread.Sleep(100)
    End Sub
#End Region
End Module
