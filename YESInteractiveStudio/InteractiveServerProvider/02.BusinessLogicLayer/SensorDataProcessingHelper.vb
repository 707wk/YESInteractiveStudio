Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports System.Threading
Imports Newtonsoft.Json
Imports YESInteractiveSDK

''' <summary>
''' 传感器数据处理辅助类
''' </summary>
Public NotInheritable Class SensorDataProcessingHelper
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 数据处理线程
    ''' </summary>
    Private Shared WorkThread As Threading.Thread

    ''' <summary>
    ''' 读取传感器数据完毕信号
    ''' </summary>
    Public Shared EndOfReadSensorDataEvent As CountdownEvent

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
    ''' UDP发送端
    ''' </summary>
    Private Shared UdpServer As UdpClient
    ''' <summary>
    ''' 发送IP及端口号
    ''' </summary>
    Private Shared UdpServerIPEndPoint As IPEndPoint

#Region "开始异步处理传感器数据"
    ''' <summary>
    ''' 开始异步处理传感器数据
    ''' </summary>
    Public Shared Sub StartAsync()

        If _IsRunning Then
            Exit Sub
        End If
        _IsRunning = True

        If AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems.Count = 0 Then
            Exit Sub
        End If

        EndOfReadSensorDataEvent = New CountdownEvent(AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems.Count)

        For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
            tmpNovaStarSender.Connect()
        Next

        UdpServer = New UdpClient
        UdpServerIPEndPoint = New IPEndPoint(IPAddress.Broadcast, 8716)

        WorkThread = New Threading.Thread(AddressOf WorkFunction) With {
            .IsBackground = True
        }
        WorkThread.Start()

    End Sub
#End Region

#Region "停止异步处理传感器数据"
    ''' <summary>
    ''' 停止异步处理传感器数据
    ''' </summary>
    Public Shared Sub StopAsync()

        If Not _IsRunning Then
            Exit Sub
        End If
        _IsRunning = False

        If AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems.Count = 0 Then
            Exit Sub
        End If

        WorkThread.Abort()
        WorkThread = Nothing

        UdpServer.Close()

        '停止读取传感器数据
        For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
            tmpNovaStarSender.DisConnect()
        Next

        EndOfReadSensorDataEvent = Nothing

    End Sub
#End Region

#Region "主处理函数"
    ''' <summary>
    ''' 主处理函数
    ''' </summary>
    Private Shared Sub WorkFunction()
        '活动点列表
        Dim PointOfMergeItems As New List(Of PointOfMerge)
        '合并点列表
        Dim PointInfoItems As New List(Of PointInfo)

        Do While _IsRunning

            '读取传感器数据
            For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
                tmpNovaStarSender.StartOfReadSensorDataEvent.Set()
            Next

            '等待读取完毕
            EndOfReadSensorDataEvent.Wait(50)
            EndOfReadSensorDataEvent.Reset()

            ''todo:替换为点合并处理模块
            Dim distance2Pow = Math.Pow(AppSettingHelper.Settings.PositionaIAccuracy, 2)
            Dim ValidSensorMinimum = AppSettingHelper.Settings.ValidSensorMinimum

            PointOfMergeItems.Clear()
            For Each sensorItem In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ActiveSensorItems

                Dim isInclude As Boolean = False
                For Each pointOfMergeItem In PointOfMergeItems
                    '判断是否在合并范围内
                    If Math.Pow(pointOfMergeItem.X - sensorItem.LocationOfCenter.X, 2) + Math.Pow(pointOfMergeItem.Y - sensorItem.LocationOfCenter.Y, 2) <= distance2Pow Then

                        pointOfMergeItem.XSum += sensorItem.LocationOfCenter.X
                        pointOfMergeItem.YSum += sensorItem.LocationOfCenter.Y
                        pointOfMergeItem.SensorCount += 1

                        '有一个点被感应则标记为新点
                        If sensorItem.State = InteractiveOptions.SensorState.DOWN Then
                            pointOfMergeItem.IsNew = True
                        End If

                        isInclude = True
                        Exit For

                    End If
                Next

                '没有被合并则添加为新聚合点
                If Not isInclude Then
                    PointOfMergeItems.Add(New PointOfMerge() With {
                              .X = sensorItem.LocationOfCenter.X,
                              .Y = sensorItem.LocationOfCenter.Y,
                              .XSum = .X,
                              .YSum = .Y,
                              .SensorCount = 1,
                              .IsNew = (sensorItem.State = InteractiveOptions.SensorState.DOWN)
                                          })
                End If

            Next

            '过滤及转换
            PointInfoItems.Clear()
            For Each item In PointOfMergeItems
                If item.SensorCount < ValidSensorMinimum Then
                    Continue For
                End If

                item.X = item.XSum \ item.SensorCount
                item.Y = item.YSum \ item.SensorCount

                PointInfoItems.Add(New PointInfo With {
                               .ID = PointInfoItems.Count + 1,
                               .X = item.X,
                               .Y = item.Y,
                               .Old = If(item.IsNew, 0, 1)
                               })

            Next

            'UDP广播发送
            Dim tmpBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(PointInfoItems))
            UdpServer.Send(tmpBytes, tmpBytes.Length, UdpServerIPEndPoint)
            'Wangk.Tools.LoggerHelper.Log.LogThis(tmpStr)

            '清除旧数据
            AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ActiveSensorItems.Clear()

            ''todo:读取间隔
            Thread.Sleep(10)

        Loop

    End Sub
#End Region

End Class
