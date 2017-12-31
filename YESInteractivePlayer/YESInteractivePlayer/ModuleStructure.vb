Imports System.Net.Sockets
Imports System.Threading
Imports Nova.Mars.SDK

Module ModuleStructure
    ''' <summary>
    ''' 接收卡信息
    ''' </summary>
    Public Structure ScanBoardInfo
        ''' <summary>
        ''' 屏幕索引
        ''' </summary>
        Dim ScreenId As Integer
        '''' <summary>
        '''' 在接收卡数组的下标
        '''' </summary>
        'Dim ScanBoardArrayId As Integer
        ''' <summary>
        ''' 控制器索引
        ''' </summary>
        Dim SenderId As Integer
        ''' <summary>
        ''' 网口索引
        ''' </summary>
        Dim PortId As Integer
        ''' <summary>
        ''' 连接序号
        ''' </summary>
        Dim ConnectId As Integer

        ''' <summary>
        ''' X轴索引
        ''' </summary>
        Dim X As Integer
        ''' <summary>
        ''' Y轴索引
        ''' </summary>
        Dim Y As Integer
    End Structure

    ''' <summary>
    ''' 发送卡信息
    ''' </summary>
    Public Structure senderInfo
        ''' <summary>
        ''' 控制器索引
        ''' </summary>
        Dim id As Integer
        ''' <summary>
        ''' 是否需要连接
        ''' </summary>
        Dim link As Boolean
        ''' <summary>
        ''' IP信息
        ''' </summary>
        Dim ipDate As Byte()
        ''' <summary>
        ''' 临时保存修改后的IP信息
        ''' </summary>
        Dim tmpIpData As Byte()
        ''' <summary>
        ''' socket连接变量
        ''' </summary>
        Dim cliSocket As Socket
        ''' <summary>
        ''' 状态检测线程
        ''' </summary>
        Dim workThread As Thread
    End Structure

    ''' <summary>
    ''' 显示屏信息
    ''' </summary>
    <Serializable()>
    Public Structure screenInfo
        '''' <summary>
        '''' 显示屏索引
        '''' </summary>
        'Dim id As Integer
        '''' <summary>
        '''' 备注
        '''' </summary>
        'Dim remark As String
        ''' <summary>
        ''' 是否存在
        ''' </summary>
        Dim existFlage As Boolean
        '''' <summary>
        '''' 幕布id
        '''' </summary>
        'Dim curtainId As Integer
        ''' <summary>
        ''' 幕布所在位置索引
        ''' </summary>
        Dim curtainListId As Integer

        ''' <summary>
        ''' 缩放后的 X 偏移(单位像素)
        ''' </summary>
        Dim x As Integer
        ''' <summary>
        '''  显示屏在幕布中的 X 偏移(单位像素)
        ''' </summary>
        Dim defaultX As Integer
        ''' <summary>
        ''' 缩放后的 Y 偏移(单位像素)
        ''' </summary>
        Dim y As Integer
        ''' <summary>
        ''' 显示屏在幕布中的 Y 偏移(单位像素)
        ''' </summary>
        Dim defaultY As Integer

        ''' <summary>
        ''' 缩放后显示屏宽度(单位像素)
        ''' </summary>
        Dim width As Integer
        ''' <summary>
        ''' 读取的默认显示屏宽度(单位像素)
        ''' </summary>
        Dim defaultWidth As Integer
        ''' <summary>
        ''' 缩放后显示屏高度(单位像素)
        ''' </summary>
        Dim height As Integer
        ''' <summary>
        ''' 读取的默认显示屏高度(单位像素)
        ''' </summary>
        Dim defaultHeight As Integer

        ''' <summary>
        ''' 缩放后接收卡显示单元宽度(单位像素)
        ''' </summary>
        Dim ScanBoardWidth As Integer
        ''' <summary>
        ''' 读取的默认接收卡显示单元宽度(单位像素)
        ''' </summary>
        Dim defaultScanBoardWidth As Integer
        ''' <summary>
        ''' 缩放后接收卡显示单元高度(单位像素)
        ''' </summary>
        Dim ScanBoardHeight As Integer
        ''' <summary>
        ''' 读取的默认接收卡显示单元高度(单位像素)
        ''' </summary>
        Dim defaultScanBoardHeight As Integer

        ''' <summary>
        ''' 触摸单元列数 默认4
        ''' </summary>
        Dim touchPieceColumnsNum As Integer
        ''' <summary>
        ''' 触摸单元行数 默认4
        ''' </summary>
        Dim touchPieceRowsNum As Integer

        ''' <summary>
        ''' 触摸单元高度
        ''' </summary>
        Dim touchPieceHeight As Integer
        ''' <summary>
        ''' 触摸单元宽度
        ''' </summary>
        Dim touchPieceWidth As Integer

        ''' <summary>
        ''' 上次点击状态
        ''' </summary>
        <NonSerialized()>
        Dim clickHistoryArray(,) As Integer

        ''' <summary>
        ''' 屏幕所属的发送卡列表
        ''' </summary>
        <NonSerialized()>
        Dim SenderList As List(Of Integer)
    End Structure

    ''' <summary>
    ''' 幕布信息
    ''' </summary>
    <Serializable()>
    Public Structure curtainInfo
        '''' <summary>
        '''' 幕布索引
        '''' </summary>
        'Dim id As Integer
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim remark As String
        '''' <summary>
        '''' 是否显示
        '''' </summary>
        'Dim showFlage As Boolean
        '''' <summary>
        '''' 播放文件名
        '''' </summary>
        'Dim file As String

        ''' <summary>
        ''' 缩放后 X 偏移(单位像素)
        ''' </summary>
        Dim x As Integer
        ''' <summary>
        ''' X 偏移(单位像素)
        ''' </summary>
        Dim defaultX As Integer
        ''' <summary>
        ''' 缩放后 Y 偏移(单位像素)
        ''' </summary>
        Dim y As Integer
        ''' <summary>
        ''' Y 偏移(单位像素)
        ''' </summary>
        Dim defaultY As Integer
        ''' <summary>
        ''' 缩放后宽度(单位像素)
        ''' </summary>
        Dim width As Integer
        ''' <summary>
        ''' 宽度(单位像素)
        ''' </summary>
        Dim defaultWidth As Integer
        ''' <summary>
        ''' 缩放后高度(单位像素)
        ''' </summary>
        Dim height As Integer
        ''' <summary>
        ''' 高度(单位像素)
        ''' </summary>
        Dim defaultHeight As Integer

        ''' <summary>
        ''' 幕布所含屏幕列表
        ''' </summary>
        Dim screenList As List(Of Integer)

        ''' <summary>
        ''' 播放窗体
        ''' </summary>
        <NonSerialized()>
        Dim playDialog As FormPlay
    End Structure

    ''' <summary>
    ''' 系统配置
    ''' </summary>
    <Serializable()>
    Public Structure systemInfo
        ''' <summary>
        ''' 连接状态
        ''' </summary>
        <NonSerialized()>
        Dim LinkFlage As Boolean

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '硬件相关变量
        ''' <summary>
        ''' 接收卡索引表 key发送卡id-网口id-接收卡id value ScanBoardInfo
        ''' </summary>
        <NonSerialized()>
        Dim ScanBoardTable As Hashtable
        ''' <summary>
        ''' 发送卡列表
        ''' </summary>
        <NonSerialized()>
        Dim senderList() As senderInfo
        ''' <summary>
        ''' 屏幕列表 最大32个
        ''' </summary>
        Dim screenList() As screenInfo
        ''' <summary>
        ''' 幕布列表 长度不能大于屏幕数
        ''' </summary>
        Dim curtainList As List(Of curtainInfo)

        ''' <summary>
        ''' Nova连接变量
        ''' </summary>
        <NonSerialized()>
        Dim rootClass As MarsHardwareEnumerator
        ''' <summary>
        ''' Nova配置变量
        ''' </summary>
        <NonSerialized()>
        Dim mainClass As MarsControlSystem
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '播放相关变量
        ''' <summary>
        ''' 播放列表 key文件名 value文件绝对路径
        ''' </summary>
        Dim filesList As Hashtable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '窗体相关变量
        ''' <summary>
        ''' 窗口显示位置
        ''' </summary>
        Dim startLocation As Point
        ''' <summary>
        ''' 缩放比例 1-3
        ''' </summary>
        Dim zoomProportion As Double
        '''' <summary>
        '''' 宽度缩放系数
        '''' </summary>
        'Dim zoomWidthCoefficient As Double
        '''' <summary>
        '''' 高度缩放系数
        '''' </summary>
        'Dim zoomHeightCoefficient As Double
        ''' <summary>
        ''' 语言类型 0中文 1English
        ''' </summary>
        Dim selectLanguageId As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动相关变量
        ''' <summary>
        ''' 显示模式
        ''' 0点击
        ''' 1测试
        ''' 2黑屏
        ''' 3忽略
        ''' </summary>
        Dim displayMode As Integer
        '''' <summary>
        '''' 触摸模式 01合1 14合1 [废弃]
        '''' </summary>
        'Dim touchMode As Integer
        ''' <summary>
        ''' 触摸灵敏度 低1-9高
        ''' </summary>
        Dim touchSensitivity As Integer
        ''' <summary>
        ''' 抗干扰等级 同时点击数大于等于几个有效
        ''' </summary>
        Dim clickValidNums As Integer

        ''' <summary>
        ''' 屏幕定时复位增量温度 K
        ''' </summary>
        Dim resetTemp As Integer
        ''' <summary>
        ''' 屏幕定时复位时间
        ''' </summary>
        Dim resetSec As Integer

        ''' <summary>
        ''' 查询时间间隔 ms
        ''' </summary>
        Dim inquireTimeSec As Integer
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    End Structure
End Module
