Imports System.Net.Sockets
Imports System.Threading
Imports Nova.Mars.SDK
Imports YESInteractiveSDK.ModuleStructure

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
    Public Structure SenderInfo
        ''' <summary>
        ''' 控制器索引
        ''' </summary>
        Dim Id As Integer
        ''' <summary>
        ''' 是否需要连接
        ''' </summary>
        Dim Link As Boolean
        ''' <summary>
        ''' IP信息
        ''' </summary>
        Dim IpDate As Byte()
        ''' <summary>
        ''' 临时保存修改后的IP信息
        ''' </summary>
        Dim TmpIpData As Byte()
        ''' <summary>
        ''' socket连接变量
        ''' </summary>
        Dim CliSocket As Socket
        ''' <summary>
        ''' 状态检测线程
        ''' </summary>
        Dim WorkThread As Thread

        ''' <summary>
        ''' 每秒最大查询次数
        ''' </summary>
        Dim MaxReadNum As Integer
    End Structure

    ''' <summary>
    ''' 显示屏信息
    ''' </summary>
    <Serializable()>
    Public Structure ScreenInfo
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
        Dim ExistFlage As Boolean
        '''' <summary>
        '''' 幕布id
        '''' </summary>
        'Dim curtainId As Integer
        ''' <summary>
        ''' 幕布所在位置索引
        ''' </summary>
        Dim CurtainListId As Integer

        ''' <summary>
        ''' 缩放后的 X 偏移(单位像素)
        ''' </summary>
        Dim X As Integer
        ''' <summary>
        '''  显示屏在幕布中的 X 偏移(单位像素)
        ''' </summary>
        Dim DefaultX As Integer
        ''' <summary>
        ''' 缩放后的 Y 偏移(单位像素)
        ''' </summary>
        Dim Y As Integer
        ''' <summary>
        ''' 显示屏在幕布中的 Y 偏移(单位像素)
        ''' </summary>
        Dim DefaultY As Integer

        ''' <summary>
        ''' 缩放后显示屏宽度(单位像素)
        ''' </summary>
        Dim Width As Integer
        ''' <summary>
        ''' 读取的默认显示屏宽度(单位像素)
        ''' </summary>
        Dim DefaultWidth As Integer
        ''' <summary>
        ''' 缩放后显示屏高度(单位像素)
        ''' </summary>
        Dim Height As Integer
        ''' <summary>
        ''' 读取的默认显示屏高度(单位像素)
        ''' </summary>
        Dim DefaultHeight As Integer

        ''' <summary>
        ''' 缩放后接收卡显示单元宽度(单位像素)
        ''' </summary>
        Dim ScanBoardWidth As Integer
        ''' <summary>
        ''' 读取的默认接收卡显示单元宽度(单位像素)
        ''' </summary>
        Dim DefaultScanBoardWidth As Integer
        ''' <summary>
        ''' 缩放后接收卡显示单元高度(单位像素)
        ''' </summary>
        Dim ScanBoardHeight As Integer
        ''' <summary>
        ''' 读取的默认接收卡显示单元高度(单位像素)
        ''' </summary>
        Dim DefaultScanBoardHeight As Integer

        ''' <summary>
        ''' 触摸单元列数 默认4
        ''' </summary>
        Dim TouchPieceColumnsNum As Integer
        ''' <summary>
        ''' 触摸单元行数 默认4
        ''' </summary>
        Dim TouchPieceRowsNum As Integer

        ''' <summary>
        ''' 触摸单元高度
        ''' </summary>
        Dim TouchPieceHeight As Integer
        ''' <summary>
        ''' 触摸单元宽度
        ''' </summary>
        Dim TouchPieceWidth As Integer

        ''' <summary>
        ''' 上次点击状态
        ''' </summary>
        <NonSerialized()>
        Dim ClickHistoryArray(,) As Integer

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
    Public Structure CurtainInfo
        '''' <summary>
        '''' 幕布索引
        '''' </summary>
        'Dim id As Integer
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim Remark As String
        '''' <summary>
        '''' 是否显示
        '''' </summary>
        'Dim showFlage As Boolean
        ''' <summary>
        ''' 播放文件名
        ''' </summary>
        <NonSerialized()>
        Dim file As String

        ''' <summary>
        ''' 缩放后 X 偏移(单位像素)
        ''' </summary>
        Dim X As Integer
        ''' <summary>
        ''' X 偏移(单位像素)
        ''' </summary>
        Dim DefaultX As Integer
        ''' <summary>
        ''' 缩放后 Y 偏移(单位像素)
        ''' </summary>
        Dim Y As Integer
        ''' <summary>
        ''' Y 偏移(单位像素)
        ''' </summary>
        Dim DefaultY As Integer
        ''' <summary>
        ''' 缩放后宽度(单位像素)
        ''' </summary>
        Dim Width As Integer
        ''' <summary>
        ''' 宽度(单位像素)
        ''' </summary>
        Dim DefaultWidth As Integer
        ''' <summary>
        ''' 缩放后高度(单位像素)
        ''' </summary>
        Dim Height As Integer
        ''' <summary>
        ''' 高度(单位像素)
        ''' </summary>
        Dim DefaultHeight As Integer

        ''' <summary>
        ''' 幕布所含屏幕列表
        ''' </summary>
        Dim ScreenList As List(Of Integer)

        ''' <summary>
        ''' 播放窗体
        ''' </summary>
        <NonSerialized()>
        Dim PlayDialog As FormPlay
        '''' <summary>
        '''' 点活动事件队列
        '''' </summary>
        '<NonSerialized()>
        'Dim PointActiveQueue As Queue(Of PointInfo)
    End Structure

    ''' <summary>
    ''' 系统配置
    ''' </summary>
    <Serializable()>
    Public Structure SystemInfo
        ''' <summary>
        ''' 连接状态
        ''' </summary>
        <NonSerialized()>
        Dim LinkFlage As Boolean

        ''' <summary>
        ''' 日志记录
        ''' </summary>
        <NonSerialized()>
        Dim logger As Wangk.Tools.Logger
        ''' <summary>
        ''' 切换语言类
        ''' </summary>
        <NonSerialized()>
        Dim Language As Wangk.Resource.MultiLanguage
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
        Dim SenderList() As SenderInfo
        ''' <summary>
        ''' 屏幕列表 最大32个
        ''' </summary>
        Dim ScreenList() As ScreenInfo
        ''' <summary>
        ''' 幕布列表 长度不能大于屏幕数
        ''' </summary>
        Dim CurtainList As List(Of CurtainInfo)

        ''' <summary>
        ''' Nova连接变量
        ''' </summary>
        <NonSerialized()>
        Dim RootClass As MarsHardwareEnumerator
        ''' <summary>
        ''' Nova配置变量
        ''' </summary>
        <NonSerialized()>
        Dim MainClass As MarsControlSystem

        ''' <summary>
        ''' 接收卡是否是旧版
        ''' </summary>
        Dim ScanBoardOldFlage As Boolean
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '播放相关变量
        ''' <summary>
        ''' 播放列表 key文件名 value文件绝对路径
        ''' </summary>
        Dim FilesList As Hashtable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '窗体相关变量
        ''' <summary>
        ''' 窗口显示位置
        ''' </summary>
        Dim StartLocation As Point

        ''' <summary>
        ''' 缩放分子
        ''' </summary>
        Dim ZoomTmpNumerator As Integer
        ''' <summary>
        ''' 缩放分母
        ''' </summary>
        Dim ZoomTmpDenominator As Integer
        ''' <summary>
        ''' 缩放比例
        ''' </summary>
        Dim ZoomProportion As Double
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
        Dim SelectLanguageId As Integer
        '''' <summary>
        '''' 语言包索引
        '''' </summary>
        '<NonSerialized()>
        'Dim LanguageTable As Hashtable
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动相关变量
        ''' <summary>
        ''' 显示模式
        ''' 0点击
        ''' 1测试
        ''' 2黑屏
        ''' 3忽略 4显示电容
        ''' </summary>
        Dim DisplayMode As Integer
        ''' <summary>
        ''' 触摸模式 01合1 14合1 216合1
        ''' </summary>
        Dim touchMode As Integer
        ''' <summary>
        ''' 触摸灵敏度 低1-9高
        ''' </summary>
        Dim TouchSensitivity As Integer
        ''' <summary>
        ''' 抗干扰等级 同时点击数大于等于几个有效
        ''' </summary>
        Dim ClickValidNums As Integer

        ''' <summary>
        ''' 屏幕定时复位增量温度 K
        ''' </summary>
        Dim ResetTemp As Integer
        ''' <summary>
        ''' 屏幕定时复位时间 Sec
        ''' </summary>
        Dim ResetSec As Integer

        ''' <summary>
        ''' 查询时间间隔 ms
        ''' </summary>
        <NonSerialized()>
        Dim InquireTimeSec As Integer

        ''' <summary>
        ''' 捕获鼠标 默认使用接口
        ''' </summary>
        Dim SetCaptureFlage As Boolean
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    End Structure
End Module
