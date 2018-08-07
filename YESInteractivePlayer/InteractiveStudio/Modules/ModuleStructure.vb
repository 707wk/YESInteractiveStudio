Imports System.Net.Sockets
Imports System.Threading
Imports System.Xml.Serialization
Imports Nova.Mars.SDK

Public Module ModuleStructure
#Region "接收卡信息"
    ''' <summary>
    ''' 接收卡信息
    ''' </summary>
    Public Structure ScanBoardInfo
        ''' <summary>
        ''' 屏幕索引
        ''' </summary>
        Dim ScreenId As Integer
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
        ''' 索引坐标
        ''' </summary>
        Dim Location As Point

        ''' <summary>
        ''' 旋转角度
        ''' </summary>
        Dim Angles As Integer
    End Structure
#End Region

#Region "发送卡信息"
    ''' <summary>
    ''' 发送卡信息
    ''' </summary>
    Public Structure SenderInfo
        ''' <summary>
        ''' 是否需要连接
        ''' </summary>
        Dim LinkFlage As Boolean

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
#End Region

#Region "显示屏信息"
    ''' <summary>
    ''' 显示屏信息
    ''' </summary>
    Public Structure ScreenInfo
        ''' <summary>
        ''' 窗口ID -1不显示
        ''' </summary>
        Dim WindowId As Integer

        '''' <summary>
        '''' 默认位置
        '''' </summary>
        'Dim DefLocation As Point
        ''' <summary>
        ''' 缩放后位置
        ''' </summary>
        Dim ZoomLocation As Point

        ''' <summary>
        ''' 默认尺寸
        ''' </summary>
        Dim DefSize As Size
        ''' <summary>
        ''' 缩放后尺寸
        ''' </summary>
        Dim ZoomSize As Size

        ''' <summary>
        ''' 默认接收卡尺寸
        ''' </summary>
        Dim DefScanBoardSize As Size
        ''' <summary>
        ''' 缩放后接收卡尺寸
        ''' </summary>
        Dim ZoomScanBoardSize As Size

        ''' <summary>
        ''' 感应单元布局 Height行数 Width列数
        ''' </summary>
        Dim SensorLayout As Size
        ''' <summary>
        ''' 感应单元尺寸
        ''' </summary>
        Dim SensorSize As Size
        ''' <summary>
        ''' 缩放后感应单元尺寸
        ''' </summary>
        Dim ZoomSensorSize As Size

        ''' <summary>
        ''' 历史点击状态
        ''' </summary>
        Dim ClickHistoryMap(,) As Integer

        ''' <summary>
        ''' 屏幕所属的发送卡列表
        ''' </summary>
        Dim SenderList As List(Of Integer)
    End Structure
#End Region

#Region "播放方案"
#Region "窗口中的屏幕信息"
    '''' <summary>
    '''' 窗口中的屏幕信息
    '''' </summary>
    'Structure ScreenInWindow
    '    ''' <summary>
    '    ''' 屏幕ID
    '    ''' </summary>
    '    Dim ScreenID As Integer
    '    ''' <summary>
    '    ''' 屏幕在窗口中位置
    '    ''' 编辑后需写入到ScreenInfo的DefLocation
    '    ''' </summary>
    '    Dim Location As Point
    'End Structure
#End Region

#Region "文件信息"
    ''' <summary>
    ''' 文件信息
    ''' </summary>
    Structure MediaInfo
        ''' <summary>
        ''' 节目ID
        ''' </summary>
        <XmlIgnore>
        Dim ProgramID As Integer
        ''' <summary>
        ''' 文件ID
        ''' </summary>
        <XmlIgnore>
        Dim MediaID As Integer

        ''' <summary>
        ''' 文件路径
        ''' </summary>
        Dim Path As String
        ''' <summary>
        ''' 播放时长
        ''' </summary>
        Dim PlayTime As Integer
    End Structure
#End Region

#Region "节目信息"
    ''' <summary>
    ''' 节目信息
    ''' </summary>
    Structure ProgramInfo
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim Remark As String

        ''' <summary>
        ''' 播放列表
        ''' </summary>
        Dim MediaList As List(Of MediaInfo)
    End Structure
#End Region

#Region "窗口主信息"
    ''' <summary>
    ''' 窗口信息
    ''' </summary>
    Public Structure WindowInfo
        ''' <summary>
        ''' 备注
        ''' </summary>
        Dim Remark As String

        ''' <summary>
        ''' 是否显示
        ''' </summary>
        Dim ShowFlage As Boolean

        ''' <summary>
        ''' 位置
        ''' </summary>
        Dim Location As Point

        ''' <summary>
        ''' 尺寸
        ''' </summary>
        <XmlIgnore>
        Dim Size As Size

        ''' <summary>
        ''' 幕布所含屏幕列表
        ''' </summary>
        Dim ScreenList As List(Of Integer)

        ''' <summary>
        ''' 缩放像素 Width默认 Height显示像素
        ''' </summary>
        Dim ZoomPix As Size

        ''' <summary>
        ''' 播放窗体
        ''' </summary>
        <XmlIgnore>
        Dim PlayDialog As PlayWindow

        ''' <summary>
        ''' 正在播放的节目
        ''' </summary>
        <XmlIgnore>
        Dim PlayProgramInfo As ProgramInfo
        ''' <summary>
        ''' 正在播放的文件序号
        ''' </summary>
        <XmlIgnore>
        Dim PlayMediaId As Integer
        ''' <summary>
        ''' 已播放时长
        ''' </summary>
        <XmlIgnore>
        Dim PlayMediaTime As Integer

        ''' <summary>
        ''' 节目列表
        ''' </summary>
        Dim ProgramList As List(Of ProgramInfo)
    End Structure
#End Region

    ''' <summary>
    ''' 播放方案
    ''' </summary>
    Structure ScheduleInfo
        ''' <summary>
        ''' 窗口列表 不能大于屏幕数
        ''' </summary>
        Dim WindowList As List(Of WindowInfo)

        ''' <summary>
        ''' 屏幕位置
        ''' </summary>
        Dim ScreenLocations As Point()
    End Structure
#End Region

#Region "互动选项"
    ''' <summary>
    ''' 互动选项
    ''' </summary>
    Public Structure InteractiveOptions
#Region "显示模式"
        ''' <summary>
        ''' 显示模式
        ''' </summary>
        Public Enum DISPLAYMODE
            ''' <summary>
            ''' 互动
            ''' </summary>
            INTERACT
            ''' <summary>
            ''' 测试
            ''' </summary>
            TEST
            ''' <summary>
            ''' 黑屏
            ''' </summary>
            BLACK
            ''' <summary>
            ''' 调试
            ''' </summary>
            DEBUG
        End Enum
#End Region

#Region "触摸模式"
        ''' <summary>
        ''' 触摸模式
        ''' </summary>
        Public Enum TOUCHMODE
            ''' <summary>
            ''' 1合1
            ''' </summary>
            T121
            ''' <summary>
            ''' 4合1
            ''' </summary>
            T421
            ''' <summary>
            ''' 16合1
            ''' </summary>
            T1621
        End Enum
#End Region
    End Structure
#End Region

#Region "系统配置"
    ''' <summary>
    ''' 系统配置
    ''' </summary>
    Public Structure SystemInfo
#Region "系统参数"
        ''' <summary>
        ''' 打开文件记录
        ''' </summary>
        Dim HistoryFile As String

        ''' <summary>
        ''' 上次运行时程序版本号,用于显示新程序更新内容
        ''' </summary>
        Dim VersionArray As Integer()

        ''' <summary>
        ''' 语言类型
        ''' </summary>
        Dim SelectLang As Wangk.Resource.MultiLanguage.LANG

        ''' <summary>
        ''' 切换语言类
        ''' </summary>
        <XmlIgnore>
        Dim Language As Wangk.Resource.MultiLanguage

        ''' <summary>
        ''' 日志记录
        ''' </summary>
        <XmlIgnore>
        Dim logger As Wangk.Tools.Logger
#End Region

#Region "播放方案"
        ''' <summary>
        ''' 播放方案
        ''' </summary>
        <XmlIgnore>
        Dim Schedule As ScheduleInfo
#End Region

#Region "控制器"
        ''' <summary>
        ''' 接收卡索引表 key发送卡id-网口id-接收卡id value ScanBoardInfo
        ''' </summary>
        <XmlIgnore>
        Dim ScanBoardTable As Hashtable
        ''' <summary>
        ''' 发送卡列表
        ''' </summary>
        <XmlIgnore>
        Dim SenderList() As SenderInfo
        ''' <summary>
        ''' 屏幕列表
        ''' </summary>
        <XmlIgnore>
        Dim ScreenList() As ScreenInfo

        ''' <summary>
        ''' Nova连接变量
        ''' </summary>
        <XmlIgnore>
        Dim RootClass As MarsHardwareEnumerator
        ''' <summary>
        ''' Nova配置变量
        ''' </summary>
        <XmlIgnore>
        Dim MainClass As MarsControlSystem

        ''' <summary>
        ''' MCU程序旧版标记
        ''' </summary>
        Dim ScanBoardOldFlage As Boolean
#End Region

#Region "运行参数"
        ''' <summary>
        ''' 连接状态
        ''' </summary>
        <XmlIgnore>
        Dim LinkFlage As Boolean

        ''' <summary>
        ''' 最后一次故障信息
        ''' </summary>
        <XmlIgnore>
        Dim LastErrorInfo As String

        ''' <summary>
        ''' 外部调用内部函数
        ''' </summary>
        <XmlIgnore>
        Dim MainForm As MDIParentMain
#Region "互动参数"
        '互动相关变量
        ''' <summary>
        ''' 显示模式
        ''' </summary>
        <XmlIgnore>
        Dim DisplayMode As InteractiveOptions.DISPLAYMODE
        ''' <summary>
        ''' 触摸模式
        ''' </summary>
        Dim TouchMode As InteractiveOptions.TOUCHMODE
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
        <XmlIgnore>
        Dim InquireTimeSec As Integer
#End Region
#End Region
    End Structure
#End Region
End Module
