﻿Imports System.IO
Imports System.Net.Sockets
Imports Nova.Mars.SDK

Module ModuleSetting
    '接收卡信息
    Public Structure ScanBoardInfo
        '屏幕索引
        Dim ScreenIndex As Integer
        '控制器索引
        Dim SenderIndex As Integer
        '网口索引
        Dim PortIndex As Integer
        '连接序号
        Dim ConnectIndex As Integer

        'X偏移
        Dim X As Integer
        'Y偏移
        Dim Y As Integer
    End Structure

    '接收卡信息索引表
    Public ScanBoardTable As Hashtable

    '显示屏信息
    <Serializable()>
    Public Structure screenInfo
        '显示屏索引
        Dim index As Integer
        '备注
        Dim remark As String
        '是否显示
        Dim showFlage As Boolean

        '显示屏 X 偏移(单位像素)
        Dim x As Integer
        '显示屏 Y 偏移(单位像素)
        Dim y As Integer

        '显示屏宽度(单位像素)
        Dim width As Integer
        '显示屏高度(单位像素)
        Dim height As Integer

        '屏幕单元宽度(单位像素)
        Dim ScanBoardWidth As Integer
        '屏幕单元高度(单位像素)
        Dim ScanBoardHeight As Integer

        '播放窗体
        Dim playDialog As FormPlay
    End Structure
    Public screenMain As screenInfo()

    '发送卡(控制器)信息
    Public Structure senderInfo
        '控制器索引
        Dim index As Integer
        '是否连接
        Dim link As Boolean
        'IP信息
        Dim ipDate As Byte()
        '修改后的IP信息
        Dim tmpIpData As Byte()
        '连接变量
        Dim cliSocket As Socket
    End Structure
    Public senderArray As senderInfo()

    '查询时间间隔
    Public checkTime As Integer
    '运行模式
    '0点击
    '1点击(捕获鼠标)
    '2测试
    '3测试(显示电容)
    '4黑屏
    '5忽略
    Public runMode As Integer

    'Nova服务
    Public rootClass As MarsHardwareEnumerator
    Public mainClass As MarsControlSystem

    '输出日志
    Public Sub putlog(str As String)
        Dim tmp As StreamWriter = New StreamWriter("log.txt", True)
        tmp.WriteLine(Format(Now(), "[yyyy-MM-dd HH:mm:ss] ") & str)
        tmp.Close()
    End Sub
End Module
