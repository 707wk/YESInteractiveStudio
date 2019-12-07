Imports System.Net.Sockets
Imports System.Threading
Imports InteractiveServerProvider.InteractiveOptions

''' <summary>
''' Nova发送卡信息
''' </summary>
Public Class NovaStarSender

    '''' <summary>
    '''' 发送卡索引
    '''' </summary>
    'Public ID As Integer

    '''' <summary>
    '''' 是否需要连接
    '''' </summary>
    'Public IsNeedToConnection As Boolean

    '''' <summary>
    '''' 是否能互动
    '''' </summary>
    'Public IsCanInteractive As Boolean

    ''' <summary>
    ''' IP信息
    ''' </summary>
    Public IpData() As Byte

#Region "IP地址"
    ''' <summary>
    ''' IP地址
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property IPAddress As String
        Get
            Try
                Return $"{ IpData(3)}.{ IpData(2)}.{ IpData(1)}.{ IpData(0)}"
            Catch ex As Exception
                Return ""
            End Try
        End Get
        'Set(value As String)

        '    If Not System.Net.IPAddress.TryParse(value, Nothing) OrElse
        '        value.Split(".").Count <> 4 Then
        '        Throw New Exception("IP address formal error")
        '    End If

        '    Dim tmpIPStr = value.Split(".")
        '    IpData(3) = Val(tmpIPStr(0))
        '    IpData(2) = Val(tmpIPStr(1))
        '    IpData(1) = Val(tmpIPStr(2))
        '    IpData(0) = Val(tmpIPStr(3))

        'End Set
    End Property
#End Region

#Region "子网掩码"
    ''' <summary>
    ''' 获取字符串形式的子网掩码
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property IPSubnetMask As String
        Get
            Try
                Return $"{ IpData(7)}.{ IpData(6)}.{ IpData(5)}.{ IpData(4)}"
            Catch ex As Exception
                Return ""
            End Try
        End Get
        'Set(value As String)
        '    If Not System.Net.IPAddress.TryParse(value, Nothing) OrElse
        '        value.Split(".").Count <> 4 Then
        '        Throw New Exception("IP subnet mask formal error")
        '    End If

        '    Dim tmpIPStr = value.Split(".")
        '    IpData(7) = Val(tmpIPStr(0))
        '    IpData(6) = Val(tmpIPStr(1))
        '    IpData(5) = Val(tmpIPStr(2))
        '    IpData(4) = Val(tmpIPStr(3))
        'End Set
    End Property
#End Region

#Region "网关地址"
    ''' <summary>
    ''' 获取字符串形式的网关地址
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property IPGateway As String
        Get
            Try
                Return $"{ IpData(11)}.{ IpData(10)}.{ IpData(9)}.{ IpData(8)}"
            Catch ex As Exception
                Return ""
            End Try
        End Get
        'Set(value As String)
        '    If Not System.Net.IPAddress.TryParse(value, Nothing) OrElse
        '        value.Split(".").Count <> 4 Then
        '        Throw New Exception("IP gateway formal error")
        '    End If

        '    Dim tmpIPStr = value.Split(".")
        '    IpData(11) = Val(tmpIPStr(0))
        '    IpData(10) = Val(tmpIPStr(1))
        '    IpData(9) = Val(tmpIPStr(2))
        '    IpData(8) = Val(tmpIPStr(3))
        'End Set
    End Property
#End Region

    ''' <summary>
    ''' 发送卡内热备份端口查找表
    ''' </summary>
    Public HotBackUpPortItems As New Dictionary(Of Integer, Integer)
    ''' <summary>
    ''' 传感器查找表(网口ID * 100000 + 连接ID * 100 + 传感器Key)
    ''' </summary>
    Public SensorItems As New Dictionary(Of Integer, Sensor)

    ''' <summary>
    ''' 活动点列表
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ActiveSensorItems As New List(Of Sensor)

    <Newtonsoft.Json.JsonIgnore>
    Private _state As SenderConnectState = SenderConnectState.OffLine
    ''' <summary>
    ''' 连接状态
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property State As SenderConnectState
        Get
            Return _state
        End Get
    End Property

    ''' <summary>
    ''' 工作线程
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Private WorkThread As Thread

    ''' <summary>
    ''' 开始读取传感器数据信号
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public StartOfReadSensorDataEvent As AutoResetEvent

    ''' <summary>
    ''' 是否已连接
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Private IsConnect As Boolean = False

    ''' <summary>
    ''' 连接控制器
    ''' </summary>
    Public Sub Connect()
        If IsConnect Then
            Exit Sub
        End If
        IsConnect = True

        StartOfReadSensorDataEvent = New AutoResetEvent(False)

        WorkThread = New Thread(AddressOf WorkFunction) With {
            .IsBackground = True
        }
        WorkThread.Start()

    End Sub

    ''' <summary>
    ''' 断开控制器连接
    ''' </summary>
    Public Sub DisConnect()
        If Not IsConnect Then
            Exit Sub
        End If
        IsConnect = False

        '防止阻塞
        StartOfReadSensorDataEvent.Set()

        WorkThread.Join()
        WorkThread = Nothing

        StartOfReadSensorDataEvent = Nothing

    End Sub

    ''' <summary>
    ''' 工作线程
    ''' </summary>
    Private Sub WorkFunction()
        Do While IsConnect

            Try
                If Not My.Computer.Network.Ping(Me.IPAddress, 500) Then
                    Exit Try
                End If

                GetSensorData()

            Catch ex As Exception
                Wangk.Tools.LoggerHelper.Log.LogThis("工作线程",
                                                      ex.ToString,
                                                      Wangk.Tools.Logger.LogLevel.Level_WARN)
                _state = SenderConnectState.OffLine
            End Try

            Thread.Sleep(1000)

        Loop
    End Sub

    ''' <summary>
    ''' 获取传感器数据
    ''' </summary>
    Private Sub GetSensorData()

        ''todo:socket超时时间
        Using socket = New Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp) With {
                                  .SendTimeout = 50,
                                  .ReceiveTimeout = 50,
                                  .NoDelay = True
        }
            socket.Connect(Me.IPAddress(), 6000)

            '异常次数
            Dim exceptionCount As Integer = 0
            '最后异常信息
            Dim exceptionStr As String
            '临时存储
            Dim tmpSensor As Sensor = Nothing

            Do While IsConnect

                StartOfReadSensorDataEvent.WaitOne()

                If Not IsConnect Then
                    Exit Do
                End If

                Try
#Region "获取传感器数据"
                    '数据包
                    Dim ReceiveData(1028 - 1) As Byte

                    '控制器接收数据
                    socket.Send(Wangk.Hash.Hex2Bin("55D50902"))
                    socket.Receive(ReceiveData)

                    '控制器上传数据
                    socket.Send(Wangk.Hash.Hex2Bin("55D50905000000000400"))

                    ActiveSensorItems.Clear()

                    For receiveID As Integer = 0 To 16 - 1
                        socket.Receive(ReceiveData)

                        '预处理数据
                        For packetID As Integer = 4 To ReceiveData.Count - 1 Step 32
#Region "有效性校验"
                            '有效数据头
                            If ReceiveData(packetID) <> &H55 Then
                                Continue For
                            End If

                            '网口号不大于4
                            If ReceiveData(packetID + 1) > 4 Then
                                Continue For
                            End If
#End Region

                            Dim portID As Integer = ReceiveData(packetID + 1)
                            Dim scannerID As Integer = ReceiveData(packetID + 2) * 256 + ReceiveData(packetID + 3)
                            '网口*100000+接收卡*100+传感器
                            Dim novaStarScanBoardKey As Integer = portID * 100000 + scannerID * 100

                            For sensorID = 0 To 16 - 1

                                If Not SensorItems.TryGetValue(novaStarScanBoardKey + sensorID, tmpSensor) Then
                                    Continue For
                                End If

#Region "互动"
                                '未感应
                                If (ReceiveData(packetID + 4 + sensorID) And &H80) <> &H80 Then

                                            'If tmpSensor.State = SensorState.DOWN OrElse
                                            '    tmpSensor.State = SensorState.PRESS Then

                                            '    tmpSensor.State = SensorState.UP
                                            'Else

                                            tmpSensor.State = SensorState.NOOPS
                                            'End If

                                            Continue For
                                        End If

                                        If tmpSensor.State = SensorState.DOWN OrElse
                                            tmpSensor.State = SensorState.PRESS Then

                                            tmpSensor.State = SensorState.PRESS
                                        Else
                                            tmpSensor.State = SensorState.DOWN
                                        End If
#End Region


                                ActiveSensorItems.Add(tmpSensor)

                            Next

                        Next

                    Next
#End Region

                    exceptionCount = 0

                    _state = SenderConnectState.OnLine

                Catch ex As Exception

                    exceptionStr = ex.ToString

                    Wangk.Tools.LoggerHelper.Log.LogThis("通信异常",
                                                          exceptionStr,
                                                          Wangk.Tools.Logger.LogLevel.Level_WARN)

                    exceptionCount += 1

                    If exceptionCount > 3 Then
                        Throw New Exception(exceptionStr)
                    End If

                End Try

                SensorDataProcessingHelper.EndOfReadSensorDataEvent.Signal()

            Loop

        End Using

    End Sub

End Class
