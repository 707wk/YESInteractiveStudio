﻿Imports System.Threading

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

    ''' <summary>
    ''' 处理传感器数据完毕信号
    ''' </summary>
    Public Shared EndOfCompletedSensorDataEvent As CountdownEvent

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

#Region "开始异步处理传感器数据"
    ''' <summary>
    ''' 开始异步处理传感器数据
    ''' </summary>
    Public Shared Sub StartAsync()

        If _IsRunning Then
            Exit Sub
        End If
        _IsRunning = True

        If AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems.Count = 0 Then
            Exit Sub
        End If

        EndOfReadSensorDataEvent = New CountdownEvent(AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems.Count)
        EndOfCompletedSensorDataEvent = New CountdownEvent(AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems.Count)

        For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
            tmpNovaStarSender.Connect()
        Next

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

        If AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems.Count = 0 Then
            Exit Sub
        End If

        WorkThread.Abort()
        WorkThread = Nothing

        '停止读取传感器数据
        For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
            tmpNovaStarSender.DisConnect()
        Next

        EndOfReadSensorDataEvent = Nothing
        EndOfCompletedSensorDataEvent = Nothing

    End Sub
#End Region

#Region "主处理函数"
    ''' <summary>
    ''' 主处理函数
    ''' </summary>
    Private Shared Sub WorkFunction()

        Do While _IsRunning

            '读取传感器数据
            For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
                tmpNovaStarSender.StartOfReadSensorDataEvent.Set()
            Next

            '等待读取完毕
            EndOfReadSensorDataEvent.Wait(20)
            EndOfReadSensorDataEvent.Reset()

            '数据分发
            For Each tmpNovaStarSender As NovaStarSender In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
                For Each tmpSensor As Sensor In tmpNovaStarSender.ActiveSensorItems

                    AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems(tmpSensor.DisplayingWindowID).ActiveSensorItems.Add(tmpSensor)

                Next
            Next

            EndOfCompletedSensorDataEvent.Reset()
            '开始处理
            For Each tmpDisplayingWindow In AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems
                tmpDisplayingWindow.StartOfCompletedSensorDataEvent.Set()
            Next
            '等待处理完毕
            EndOfCompletedSensorDataEvent.Wait()

            '清除旧数据
            For Each item In AppSettingHelper.GetInstance.DisplayingScheme.DisplayingWindowItems
                item.ActiveSensorItems.Clear()
            Next

            ''todo:读取间隔
            Thread.Sleep(10)

        Loop

    End Sub
#End Region

End Class
