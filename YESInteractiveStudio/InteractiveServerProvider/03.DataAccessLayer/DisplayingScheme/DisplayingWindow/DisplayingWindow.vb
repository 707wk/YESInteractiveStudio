Imports System.Threading
''' <summary>
''' 播放窗口
''' </summary>
Public Class DisplayingWindow
    '''' <summary>
    '''' 显示位置
    '''' </summary>
    'Public Location As Point

    '''' <summary>
    '''' 原始显示尺寸
    '''' </summary>
    'Public SizeOfOriginal As Size
    '''' <summary>
    '''' 缩放后显示尺寸
    '''' </summary>
    'Public SizeOfZoom As Size

    ''' <summary>
    ''' 窗口内屏幕索引列表
    ''' </summary>
    Public ScreenIDItems As New HashSet(Of Integer)

    ''' <summary>
    ''' 缩放系数 0.25/0.5/1
    ''' </summary>
    Public Magnificine As Double = 1

    ''' <summary>
    ''' 活动点列表
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ActiveSensorItems As New List(Of Sensor)

End Class
