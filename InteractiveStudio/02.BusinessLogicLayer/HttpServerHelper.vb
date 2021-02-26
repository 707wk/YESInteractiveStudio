Imports System.IO
Imports System.Net.Sockets
Imports System.Threading
''' <summary>
''' 简易的HTTP及WebApi服务
''' </summary>
Public NotInheritable Class HttpServerHelper

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 数据处理线程
    ''' </summary>
    Private Shared WorkThread As Thread

    '''' <summary>
    '''' 并行数控制信号量
    '''' </summary>
    'Private Shared ConnectMaximumSemaphoreSlim As SemaphoreSlim

    '''' <summary>
    '''' 服务套接字
    '''' </summary>
    'Private Shared HttpServer As TcpListener

    ''' <summary>
    ''' 端口号
    ''' </summary>
    Private Const PortNumber As Integer = 8080

    ''' <summary>
    ''' 主窗体
    ''' </summary>
    Public Shared UIMainForm As MainForm

#Region "是否运行"
    Private Shared _IsRunning As Boolean = False
    ''' <summary>
    ''' 是否运行
    ''' </summary>
    ''' <returns></returns>
    Public Shared ReadOnly Property IsRunning As Boolean
        Get
            Return _IsRunning
        End Get
    End Property
#End Region

    ''' <summary>
    ''' 开始服务
    ''' </summary>
    Public Shared Sub StartServer()
        If _IsRunning Then
            Exit Sub
        End If
        _IsRunning = True

        InitFileMIMETypeTable()

        WorkThread = New Thread(AddressOf WorkMainFunction) With {
            .IsBackground = True
        }
        WorkThread.Start()

    End Sub

    ''' <summary>
    ''' 停止服务
    ''' </summary>
    Public Shared Sub StopServer()
        If Not _IsRunning Then
            Exit Sub
        End If
        _IsRunning = False

        WorkThread.Join()
        WorkThread = Nothing

    End Sub

    ''' <summary>
    ''' 工作主函数
    ''' </summary>
    Private Shared Sub WorkMainFunction()

        'ConnectMaximumSemaphoreSlim = New SemaphoreSlim(4)
        '服务套接字
        Dim httpServer = New TcpListener(Net.IPAddress.Any, PortNumber)
        httpServer.Start()

        Try
            Do While _IsRunning

                If Not httpServer.Pending Then
                    Thread.Sleep(500)
                    Continue Do
                End If

                'ConnectMaximumSemaphoreSlim.Wait()

                Dim tmpTcpClient = httpServer.AcceptTcpClient()

                'ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf ClientDisposeFunction), tmpTcpClient)
                ClientDisposeFunction(tmpTcpClient)

            Loop
#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
        Finally
            httpServer.Stop()
        End Try

    End Sub

#Region "文件对应MIME标识"
    ''' <summary>
    ''' MIME查找表
    ''' </summary>
    Private Shared ReadOnly FileMIMETypeTable As New Dictionary(Of String, String)
    ''' <summary>
    ''' 初始化MIME查找表
    ''' </summary>
    Private Shared Sub InitFileMIMETypeTable()
        If FileMIMETypeTable.Count = 0 Then
            With FileMIMETypeTable
                .Add(".js", "text/javascript")
                .Add(".css", "text/css")
                .Add(".json", "application/json")
                .Add("", "text/html")
            End With
        End If
    End Sub
    ''' <summary>
    ''' 获取文件对应MIME标识
    ''' </summary>
    Private Shared Function GetFileMIMEHeader(value As String) As String
        If FileMIMETypeTable.ContainsKey(value) Then
            Return FileMIMETypeTable(value)
        End If

        Return "text/html"
    End Function
#End Region

    ''' <summary>
    ''' 处理客户端请求
    ''' </summary>
    Private Shared Sub ClientDisposeFunction(client As TcpClient)
        Using tmpTcpStream = client.GetStream
            Try
                Dim receiveBytes(1024 - 1) As Byte
                tmpTcpStream.Read(receiveBytes, 0, receiveBytes.Length)
                Dim bufferStr = Text.Encoding.UTF8.GetString(receiveBytes)

                Dim httpMethodstartPos = bufferStr.IndexOf(" ", 0)
                Dim httpMethod = bufferStr.Substring(0, httpMethodstartPos)
                Dim rawUrlStartPos = bufferStr.IndexOf("HTTP", httpMethod.Length)
                Dim rawUrl = bufferStr.Substring(httpMethodstartPos + 1, rawUrlStartPos - 1 - httpMethodstartPos - 1)
                Dim httpVersion = bufferStr.Substring(rawUrlStartPos, bufferStr.IndexOf(vbCrLf, rawUrlStartPos) - rawUrlStartPos)

                Dim filePath = $"./Web{rawUrl}"

                '根路径则显示默认主页
                If rawUrl = "/" Then
                    filePath = $"./Web/index.html"
                End If
                '判断请求类型
                If IO.File.Exists(filePath) Then
                    'Console.WriteLine($"发送文件 {filePath}")

                    SendHeader(httpVersion, GetFileMIMEHeader(IO.Path.GetExtension(filePath).ToLower), "200 OK", tmpTcpStream)

                    '文件
                    Using tmpFileStream As New FileStream(filePath, FileMode.Open, FileAccess.Read)
                        Dim tmpBuffer(1024) As Byte
                        Dim tmpBufferCount = 0
                        Do
                            tmpBufferCount = tmpFileStream.Read(tmpBuffer, 0, tmpBuffer.Count)
                            tmpTcpStream.Write(tmpBuffer, 0, tmpBufferCount)
                        Loop While tmpBufferCount > 0

                    End Using

                ElseIf rawUrl.IndexOf("?action=") > 0 Then

                    Dim parametersStr = rawUrl.Substring(2, rawUrl.Length - 2)
                    Dim parameters As Dictionary(Of String, String) =
                        parametersStr.Split("&").ToDictionary(Function(value As String) As String
                                                                  Return value.Split("=")(0)
                                                              End Function,
                                                              Function(value As String) As String
                                                                  Return value.Split("=")(1)
                                                              End Function)

                    If httpMethod.ToLower = "get" Then
                        'Console.WriteLine($"拉取数据 {rawUrl}")
                        SendHeader(httpVersion, GetFileMIMEHeader(".json"), "200 OK", tmpTcpStream)

                        If parameters("action") = "loadSchemeList" Then

#Region "生成临时数据"
                            Dim tmpList As New List(Of WebScheme)
                            For Each i001 In AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems
                                Dim tmp = New WebScheme With {
                                    .Name = i001.Name
                                }
                                For Each j001 In i001.PlayFileItems
                                    tmp.FileItems.Add(IO.Path.GetFileName(j001.Path))
                                Next
                                tmpList.Add(tmp)
                            Next
#End Region

                            Dim sendBytes = Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(tmpList))
                            tmpTcpStream.Write(sendBytes, 0, sendBytes.Length)
                        End If

                    ElseIf httpMethod.ToLower = "post" Then
                        Dim windowID = Val(parameters("windowID"))
                        Dim fileID = Val(parameters("fileID"))

                        'Console.WriteLine($"提交数据 {rawUrl}")
                        SendHeader(httpVersion, GetFileMIMEHeader(".json"), "200 OK", tmpTcpStream)

                        If parameters("action") = "playFile" Then
                            UIMainForm.RemotePlayFile(windowID, fileID)
                            'Console.WriteLine($"播放 窗口{windowID} 文件{fileID}")
                        End If
                    End If

                Else
                    SendHeader(httpVersion, GetFileMIMEHeader(""), "404 Notfound", tmpTcpStream)
                    '不存在
                    'Console.WriteLine($"{filePath} 路径错误")
                End If
#Disable Warning CA1031 ' Do not catch general exception types
            Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
            End Try

        End Using

        client.Close()

        'ConnectMaximumSemaphoreSlim.Release()
    End Sub

#Region "发送头部标识"
    ''' <summary>
    ''' 发送头部标识
    ''' </summary>
    Private Shared Sub SendHeader(httpVersion As String,
                                  MIMEHeader As String,
                                  statusCode As String,
                                  tcpStream As NetworkStream)

        If String.IsNullOrEmpty(MIMEHeader) Then
            MIMEHeader = "text/html"
        End If

        Dim bufferStr = $"{httpVersion} {statusCode}
Content-Type:{MIMEHeader};charset=UTF-8
Access-Control-allow-Method:*
Access-Control-Allow-Origin:*
Access-Control-Allow-Credentials:true

"

        Dim sendBytes = Text.Encoding.UTF8.GetBytes(bufferStr)

        tcpStream.Write(sendBytes, 0, sendBytes.Length)
    End Sub
#End Region

End Class
