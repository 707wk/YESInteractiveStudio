Imports System.Net.Sockets
Imports System.Threading
Imports YESInteractiveSDK.ModuleStructure

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

                    .ScanBoardDateQueue = New Queue(Of Byte())

                    Dim TmpStr As String = $"{ .IpDate(3)}.{ .IpDate(2)}.{ .IpDate(1)}.{ .IpDate(0)}"
                    .CliSocket = New Socket(AddressFamily.InterNetwork,
                                            SocketType.Stream,
                                            ProtocolType.Tcp) With {
                                            .SendTimeout = 100,
                                            .ReceiveTimeout = 100,
                                            .NoDelay = True
                    }
                    .CliSocket.Connect(TmpStr, 6000)

                    '.WorkThread = New Threading.Thread(AddressOf ControlWorkThread) With {
                    '    .IsBackground = True'后台启动
                    '}
                    '.WorkThread.Start(i002)
                End With
            Next
        Catch ex As Exception
            sysInfo.LinkFlage = False

            'Try
            '    sysInfo.WorkThread.Join()
            'Catch ex002 As Exception
            'End Try

            For Each i003 As SenderInfo In sysInfo.SenderList
                With i003
                    Try
                        .CliSocket.Close()
                    Catch ex003 As Exception
                    End Try
                End With
            Next

            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   sysInfo.Language.GetS("Connected Exception"))
            Return False

            Exit Function
        End Try

        sysInfo.WorkThread = New Threading.Thread(AddressOf ControlWorkThread) With {
            .IsBackground = True'后台启动
        }
        sysInfo.WorkThread.Start()
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

        If sysInfo.WorkThread IsNot Nothing Then
            sysInfo.WorkThread.Join()
        End If

        'For Each i001 As SenderInfo In sysInfo.SenderList
        '    With i001
        '        If .WorkThread IsNot Nothing Then
        '            .WorkThread.Join()
        '        End If
        '    End With
        'Next

        SetResetSec(0, sysInfo.ScanBoardOldFlage)
        SetResetTemp(0, sysInfo.ScanBoardOldFlage)

        Return True
    End Function
#End Region

#Region "传感器数据处理线程"
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

                If sysInfo.ScreenList(ScreenId).SensorMap(Point.Y + rowID, Point.X + colID) <> PointState.NOOPS Then
                    checkNums += 1
                End If
            Next
        Next

        'Debug.WriteLine($"{ScreenId} {Point.X},{Point.Y}:{checkNums}")

        Return checkNums >= sysInfo.ClickValidNums
    End Function
#End Region

#Region "计算坐标"
    ''' <summary>
    ''' 计算坐标
    ''' </summary>
    ''' <returns></returns>
    Public Function CalcPointPointInfo(ScreenID As Integer,
                                       Location As Point,
                                       Old As Byte) As PointInfo
        '计算尺寸及位置
        Dim SensorWidth As Integer = sysInfo.ScreenList(ScreenID).ZoomSensorSize.Width
        Dim SensorHeight As Integer = sysInfo.ScreenList(ScreenID).ZoomSensorSize.Height
        Dim txp As Int16 = sysInfo.ScreenList(ScreenID).ZoomLocation.X + Location.X * SensorWidth + (SensorWidth \ 2)
        Dim typ As Int32 = sysInfo.ScreenList(ScreenID).ZoomLocation.Y + Location.Y * SensorHeight + (SensorHeight \ 2)

        Return New PointInfo With {
            .ID = txp + (typ << 16),
            .X = txp,
            .Y = typ,
            .Old = Old
        }
    End Function
#End Region

    ''' <summary>
    ''' 处理线程
    ''' </summary>
    Public Sub ControlWorkThread()
        '定时刷新
        Dim lastSec As Integer = -1
        '异常次数
        Dim exceptionNum As Integer = 0
        '读取次数
        Dim readNum As Integer = 0

        '检测到的活动点队列
        Dim WindowPointList As List(Of PointInfo)()
        ReDim WindowPointList(sysInfo.Schedule.WindowList.Count - 1)
        For wID = 0 To WindowPointList.Count - 1
            WindowPointList(wID) = New List(Of PointInfo)
        Next

        '定时器
        'Dim testTime As New Stopwatch

        Do While sysInfo.LinkFlage
            For wID = 0 To WindowPointList.Count - 1
                WindowPointList(wID).Clear()
            Next

#Region "刷新数据"
            If lastSec <> Now.Second Then
                lastSec = Now.Second

                exceptionNum = 0
                sysInfo.ReadNum = readNum
                readNum = 0
            End If
#End Region

#Region "异常处理"
            If exceptionNum > 3 Then
                Dim tmpThread As Thread = New Thread(AddressOf sysInfo.MainForm.DisposeControlOffLink) With {
                    .IsBackground = True
                    }
                tmpThread.Start(True)
                Exit Do
            End If
#End Region

            Try
#Region "接收传感器数据"
                '数据包
                Dim ReceiveData(1024 - 1) As Byte

                For ControlID = 0 To sysInfo.SenderList.Count - 1
                    With sysInfo.SenderList(ControlID)
                        If Not .LinkFlage Then
                            Continue For
                        End If

                        '控制器接收数据
                        .CliSocket.Send(Wangk.Hash.Hex2Bin("55D50902"))
                        .CliSocket.Receive(ReceiveData)

                        '控制器上传数据
                        .CliSocket.Send(Wangk.Hash.Hex2Bin("55D50905000000000400"))

                        '接收卡数据
                        Dim ScanBoardDate() As Byte

                        For receiveID As Integer = 0 To 16 - 1
                            ReDim ReceiveData(1028 - 1)
                            .CliSocket.Receive(ReceiveData)

                            '预处理数据
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
                                    Continue For
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

#Region "旋转传感器位置"
                                Dim tmpDate(16 - 1) As Byte
                                For i002 As Integer = 0 To 16 - 1
                                    tmpDate(i002) = ScanBoardDate(4 + i002)
                                Next

                                Select Case sysInfo.Schedule.ScreenList(tmpScanBoardInfo.ScreenId).BoxRotation
                                    Case 0
                                    Case 90
#Region "90°"
                                        Dim index As Integer = 0
                                        For j002 As Integer = 4 - 1 To 0 Step -1
                                            For i002 As Integer = 0 To 4 - 1
                                                ScanBoardDate(4 + i002 * 4 + j002) = tmpDate(index)
                                                index += 1
                                            Next
                                        Next
#End Region
                                    Case 180
#Region "180°"
                                        If sysInfo.ScreenList(tmpScanBoardInfo.ScreenId).SensorLayout.Width = sysInfo.ScreenList(tmpScanBoardInfo.ScreenId).SensorLayout.Height Then
                                            '单元布局4*4
                                            Dim index As Integer = 0
                                            For i002 As Integer = 4 - 1 To 0 Step -1
                                                For j002 As Integer = 4 - 1 To 0 Step -1
                                                    ScanBoardDate(4 + i002 * 4 + j002) = tmpDate(index)
                                                    index += 1
                                                Next
                                            Next
                                        Else
                                            '单元布局1*4/4*1
                                            For i002 As Integer = 0 To 4 - 1
                                                ScanBoardDate(4 + i002 * 4) = tmpDate((3 - i002) * 4)
                                            Next
                                        End If

#End Region
                                    Case 270
#Region "270°"
                                        If sysInfo.ScreenList(tmpScanBoardInfo.ScreenId).SensorLayout.Width = sysInfo.ScreenList(tmpScanBoardInfo.ScreenId).SensorLayout.Height Then
                                            '单元布局4*4
                                            Dim index As Integer = 0
                                            For j002 As Integer = 0 To 4 - 1
                                                For i002 As Integer = 4 - 1 To 0 Step -1
                                                    ScanBoardDate(4 + i002 * 4 + j002) = tmpDate(index)
                                                    index += 1
                                                Next
                                            Next
                                        Else
                                            '单元布局1*4/4*1
                                            For j002 As Integer = 1 To 4 - 1
                                                ScanBoardDate(4 + j002) = tmpDate(j002 * 4)
                                            Next
                                        End If

#End Region
                                End Select
#End Region

                                .ScanBoardDateQueue.Enqueue(ScanBoardDate)
                                'ScanBoardDateDictionary.Add(ControlID, ScanBoardDate)
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

                                                    Continue For
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

                                            '新点
                                            .SensorMap(Point.Y, Point.X) = PointState.DOWN

                                        Next
                                    Next

                                End With
#End Region

                            Next

                        Next
                    End With
                Next
#End Region

#Region "处理数据"
                For ControlID = 0 To sysInfo.SenderList.Count - 1

                    Do While sysInfo.SenderList(ControlID).ScanBoardDateQueue.Count > 0

                        Dim ScanBoardDate() As Byte = sysInfo.SenderList(ControlID).ScanBoardDateQueue.Dequeue
                        Dim tmpScanBoardInfo As ScanBoardInfo = sysInfo.ScanBoardTable.Item($"{ControlID}-{ScanBoardDate(1)}-{(ScanBoardDate(2) * 256 + ScanBoardDate(3))}")

                        With sysInfo.ScreenList(tmpScanBoardInfo.ScreenId)

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
                                            Continue For
                                        End If
#End Region

#Region "旧点"
                                        If .SensorMap(Point.Y, Point.X) = PointState.PRESS AndAlso
                                            CheckAdjacencyPieceNums(tmpScanBoardInfo.ScreenId, Point) Then
                                            WindowPointList(.WindowId).Add(CalcPointPointInfo(tmpScanBoardInfo.ScreenId,
                                                                                              Point,
                                                                                              1))
                                            Continue For
                                        End If
#End Region

                                        '互动模式下抗干扰启用
                                        If Not CheckAdjacencyPieceNums(tmpScanBoardInfo.ScreenId, Point) AndAlso
                                            sysInfo.DisplayMode = InteractiveOptions.DISPLAYMODE.INTERACT Then
                                            Continue For
                                        End If

                                    End If

#Region "新点"
                                    Dim tmpPointInfo = CalcPointPointInfo(tmpScanBoardInfo.ScreenId,
                                                                         Point,
                                                                         0)

                                    If sysInfo.DisplayMode <> InteractiveOptions.DISPLAYMODE.DEBUG Then
                                        '互动
                                        WindowPointList(.WindowId).Add(tmpPointInfo)
                                    Else
                                        '显示电容
                                        sysInfo.Schedule.WindowList.
                                        Item(.WindowId).
                                        PlayDialog.ShowCapacitance(tmpPointInfo.X, tmpPointInfo.Y, Value)
                                    End If
#End Region
                                Next

                            Next

                        End With

                    Loop
                Next
#End Region

#Region "发送数据"
                For wID = 0 To WindowPointList.Count - 1
                    sysInfo.Schedule.WindowList.
                        Item(wID).
                        PlayDialog.
                        PointActive(WindowPointList(wID))
                Next
#End Region

            Catch ex As Exception
                sysInfo.LastErrorInfo = ex.ToString

                sysInfo.logger.LogThis("通信异常", sysInfo.LastErrorInfo, Wangk.Tools.Loglevel.Level_INFO)

                exceptionNum += 1
            End Try

            readNum += 1
            Thread.Sleep(sysInfo.InquireTimeSec)
        Loop

#Region "关闭网络连接"
        For ControlID = 0 To sysInfo.SenderList.Count - 1
            With sysInfo.SenderList(ControlID)
                If Not .LinkFlage Then
                    Continue For
                End If

                Try
                    .CliSocket.Close()
                Catch ex As Exception
                End Try
            End With
        Next
#End Region

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
