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

    ''' <summary>
    ''' 原始显示尺寸
    ''' </summary>
    Public SizeOfOriginal As Size
    ''' <summary>
    ''' 缩放后显示尺寸
    ''' </summary>
    Public SizeOfZoom As Size

    ''' <summary>
    ''' 窗口内屏幕索引列表
    ''' </summary>
    Public ScreenIDList As HashSet(Of Integer)

    ''' <summary>
    ''' 缩放系数 25%/50%/100%
    ''' </summary>
    Public Magnificine As Double

    ''' <summary>
    ''' 播放窗体
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public PlayWindowForm As Form

    ''' <summary>
    ''' 当前播放节目索引
    ''' </summary>
    Public PlayProgramID As Integer
    ''' <summary>
    ''' 当前播放文件索引
    ''' </summary>
    Public PlayProgramFileID As Integer

    ''' <summary>
    ''' 播放节目列表
    ''' </summary>
    Public PlayProgramList As List(Of DisplayingProgram)

End Class
