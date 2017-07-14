Imports System.IO
Imports System.Net.Sockets

Module ModuleSetting
    '接收卡信息
    Structure ScanBoardInfo
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

    '显示屏信息
    Structure screenInfo
        '显示屏索引
        Dim index As Integer

        '显示屏 X 偏移(单位像素)
        Dim x As Integer
        '显示屏 Y 偏移(单位像素)
        Dim y As Integer

        '显示屏宽度(单位像素)
        Dim width As Integer
        '显示屏高度(单位像素)
        Dim height As Integer

        '带载宽度(单位像素)
        Dim ScanBoardWidth As Integer
        '带载高度(单位像素)
        Dim ScanBoardHeight As Integer

        '接收卡信息列表
        Dim ScanBoardTable As Hashtable
    End Structure
    Public screenMain As screenInfo

    '发送卡(控制器)信息
    Structure senderInfo
        '控制器索引
        Dim index As Integer
        'IP信息
        Dim ipDate As Byte()
        '连接变量
        Dim cliSocket As Socket
    End Structure
    Public senderArray As senderInfo()

    '查询时间间隔
    Public checkTime As Integer
    '端口号
    'Public Port As Integer
    '连接变量
    'Public cliSocket As Socket()
    '运行模式 0测试 1点击 2忽略
    Public runMode As Integer

    '输出日志
    Public Sub putlog(str As String)
        Dim tmp As StreamWriter = New StreamWriter("log.txt", True)
        tmp.WriteLine(Format(Now(), "[yyyy-MM-dd HH:mm:ss] ") & str)
        tmp.Close()
    End Sub
End Module
