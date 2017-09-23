Imports System.IO
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

        'X轴索引
        Dim X As Integer
        'Y轴索引
        Dim Y As Integer
    End Structure
    Public InteractMode As Integer
    '1： 普通模式 2：地砖踩踏模式 3：地砖四合一模式
    Public TempI As Integer
    '统计单张接收 特定模式 单触发块 开关量和
    'Public TempI2 As Integer
    '统计单张接收  开关量和
    Public TempS As Integer
    '屏幕灵敏度设置
    Public ScreenSensitivity As Integer
    '踩踏模式下 历史点击记录 只适合点击事件
    Public ScreenM2clickH As Integer
    ''踩踏模式下 点击记录 只适合点击事件
    Public ScreenM2click As Integer

    Public ReceiveData(,) As Integer
    Public ReceiveDataH(,) As Integer
    '屏幕特定模式 单触发块 抗干扰参数   （统计接收卡 单触发块 开关量和）
    Public ScreenAnti As Integer
    '屏幕总抗干扰参数 （统计接收卡开关量和）
    Public ScreenAntiS As Integer
    '屏幕定时复位增量温度 K
    Public ResetTemp As Integer
    '屏幕定时复位时间  min
    Public ResetTimeMin As Integer

    '接收卡信息索引表
    Public ScanBoardTable As Hashtable

    '显示屏信息
    Public Structure screenInfo
        '显示屏索引
        'Dim index As Integer
        '备注
        Dim remark As String
        '是否显示
        Dim showFlage As Boolean
        '播放文件路径
        Dim filePath As String

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
        '上次点击状态
        Dim clickHistoryArray(,) As Integer

        '屏幕所在的发送卡序列
        Dim SenderList As List(Of Integer)

        '播放窗体
        Dim playDialog As FormPlay
    End Structure
    Public screenMain As screenInfo()

    '发送卡(控制器)信息
    Public Structure senderInfo
        '控制器索引
        'Dim index As Integer
        '是否需要连接
        Dim link As Boolean
        'IP信息
        Dim ipDate As Byte()
        '临时保存修改后的IP信息
        Dim tmpIpData As Byte()
        '连接变量
        Dim cliSocket As Socket
    End Structure
    Public senderArray As senderInfo()

    '屏幕播放信息
    <Serializable()>
    Public Structure screenPlayInfo
        '屏幕索引
        'Dim screenIndex As Integer
        '备注
        Dim remark As String
        '是否显示
        Dim showFlage As Boolean
        '播放文件路径
        'Dim filePath As String
    End Structure
    '播放相关信息
    <Serializable()>
    Public Structure playInfoSave
        '屏幕播放信息列表
        Dim playList As screenPlayInfo()

        '播放列表
        Dim filesList As Hashtable
    End Structure
    Public systeminfo As playInfoSave

    '播放列表[废弃]
    'Public playFilesList As Hashtable

    '查询时间间隔
    Public checkTime As Integer
    '运行模式
    '0点击
    '1测试
    '2黑屏
    '3忽略
    '4测试(显示电容)
    Public runMode As Integer

    '记录数据标记
    Public recordDataFlage As Boolean
    Public recordDataFile As StreamWriter

    '更改语言
    '0:中文
    '1:English
    Public selectLanguageId As Integer

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
