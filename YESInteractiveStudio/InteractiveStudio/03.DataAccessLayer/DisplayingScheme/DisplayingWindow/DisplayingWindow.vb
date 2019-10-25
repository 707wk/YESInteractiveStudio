Imports System.Threading
''' <summary>
''' 播放窗口
''' </summary>
Public Class DisplayingWindow
    ''' <summary>
    ''' 名称
    ''' </summary>
    Public Name As String

    ''' <summary>
    ''' 显示位置
    ''' </summary>
    Public Location As Point

    '''' <summary>
    '''' 原始显示尺寸
    '''' </summary>
    'Public SizeOfOriginal As Size
    ''' <summary>
    ''' 缩放后显示尺寸
    ''' </summary>
    Public SizeOfZoom As Size

    ''' <summary>
    ''' 窗口内屏幕索引列表
    ''' </summary>
    Public ScreenIDItems As New HashSet(Of Integer)

    ''' <summary>
    ''' 缩放系数 0.25/0.5/1
    ''' </summary>
    Public Magnificine As Double = 1

    ''' <summary>
    ''' 播放窗口
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public PlayWindowForm As PlayWindow
    ''' <summary>
    ''' 创建播放窗口线程
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public PlayWindowThreadOfCreate As Thread

    '''' <summary>
    '''' 当前播放节目索引
    '''' </summary>
    'Public PlayProgramID As Integer
    ''' <summary>
    ''' 当前播放文件索引
    ''' </summary>
    Public PlayFileID As Integer

    '''' <summary>
    '''' 播放节目列表
    '''' </summary>
    'Public PlayProgramItems As List(Of DisplayingProgram)
    ''' <summary>
    ''' 播放文件列表
    ''' </summary>
    Public PlayFileItems As New List(Of DisplayingFile)

    ''' <summary>
    ''' 是否自动播放
    ''' </summary>
    Public IsAutoPlay As Boolean

    ''' <summary>
    ''' 活动点列表
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ActiveSensorItems As New List(Of Sensor)

    ''' <summary>
    ''' 开始处理传感器数据信号
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public StartOfCompletedSensorDataEvent As New AutoResetEvent(False)

End Class
