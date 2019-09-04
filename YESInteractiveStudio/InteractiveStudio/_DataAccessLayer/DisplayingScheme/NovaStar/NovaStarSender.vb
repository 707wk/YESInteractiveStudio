Imports System.Net.Sockets
Imports System.Threading
Imports InteractiveStudio
Imports InteractiveStudio.InteractiveOptions
''' <summary>
''' Nova发送卡信息
''' </summary>
Public Class NovaStarSender

    ''' <summary>
    ''' 发送卡索引
    ''' </summary>
    Public ID As Integer

    '''' <summary>
    '''' 是否需要连接
    '''' </summary>
    'Public IsNeedToConnection As Boolean

    ''' <summary>
    ''' IP信息
    ''' </summary>
    Public IpData As Byte()

    Private _state As SenderConnectState
    ''' <summary>
    ''' 连接状态
    ''' </summary>
    Public ReadOnly Property State As SenderConnectState
        Get
            Return _state
        End Get
    End Property

    ''' <summary>
    ''' 工作线程
    ''' </summary>
    Private WorkThread As Thread
    ''' <summary>
    ''' 是否断开连接
    ''' </summary>
    Private IsDisConnect As Boolean = False

    ''' <summary>
    ''' 连接控制器
    ''' </summary>
    Public Sub Connect()
        If Not IsDisConnect Then
            Exit Sub
        End If
        IsDisConnect = False

        WorkThread = New Thread(AddressOf GetSensorData) With {
            .IsBackground = True
        }
        WorkThread.Start()

    End Sub

    ''' <summary>
    ''' 断开控制器连接
    ''' </summary>
    Public Sub DisConnect()
        If IsDisConnect Then
            Exit Sub
        End If
        IsDisConnect = True

        WorkThread.Join()
        WorkThread = Nothing

    End Sub

    ''' <summary>
    ''' 工作线程
    ''' </summary>
    Private Sub WorkFunction()
        Do While Not IsDisConnect

            Try
                GetSensorData()

            Catch ex As Exception
                Wangk.Tools.LoggerManager.Log.LogThis("工作线程",
                                                      ex.ToString,
                                                      Wangk.Tools.Loglevel.Level_WARN)
                _state = SenderConnectState.OffLine
            End Try

        Loop
    End Sub

    ''' <summary>
    ''' 获取传感器数据
    ''' </summary>
    Private Sub GetSensorData()

        Using socket = New Socket(AddressFamily.InterNetwork,
                                  SocketType.Stream,
                                  ProtocolType.Tcp) With {
                                  .SendTimeout = 100,
                                  .ReceiveTimeout = 100,
                                  .NoDelay = True
        }
            socket.Connect($"{ IpData(3)}.{ IpData(2)}.{ IpData(1)}.{ IpData(0)}", 6000)

            '异常次数
            Dim exceptionCount As Integer = 0
            Dim exceptionStr As String

            Do While Not IsDisConnect

                Try
#Region "获取传感器数据"
                    '数据包
                    Dim ReceiveData(1024 - 1) As Byte

                    '控制器接收数据
                    socket.Send(Wangk.Hash.Hex2Bin("55D50902"))
                    socket.Receive(ReceiveData)

                    '控制器上传数据
                    socket.Send(Wangk.Hash.Hex2Bin("55D50905000000000400"))

                    For receiveID As Integer = 0 To 16 - 1
                        socket.Receive(ReceiveData)

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

                            Dim portID As Integer = ReceiveData(PacketId + 1)
                            Dim scannerID As Integer = ReceiveData(PacketId + 2) * 256 + ReceiveData(PacketId + 3)
                            Dim novaStarScanBoardDictionaryKey As Integer = ID * 10000 + portID * 1000 + scannerID

                            '查找接收卡位置
                            If Not AppSettingManager.Settings.DisplayingScheme.NovaStarScanBoardDictionary.ContainsKey(novaStarScanBoardDictionaryKey) Then
                                Continue For
                            End If

                            Dim tmpScanBoardInfo As NovaStarScanBoard =
                                AppSettingManager.Settings.DisplayingScheme.NovaStarScanBoardDictionary(novaStarScanBoardDictionaryKey)

                            '未显示则跳过

#End Region


                        Next

                    Next
#End Region


                Catch ex As Exception
                    exceptionStr = ex.ToString

                    Wangk.Tools.LoggerManager.Log.LogThis("通信异常",
                                                          exceptionStr,
                                                          Wangk.Tools.Loglevel.Level_WARN)

                    exceptionCount += 1

                    If exceptionCount > 3 Then
                        Throw New Exception(exceptionStr)
                    End If

                End Try

                'Thread.Sleep(20)
            Loop

        End Using

    End Sub

End Class
