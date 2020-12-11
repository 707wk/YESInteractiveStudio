''' <summary>
''' Nova接收卡信息
''' </summary>
Public Class NovaStarScanBoard
    ''' <summary>
    ''' 所属播放窗口索引
    ''' </summary>
    Public DisplayingWindowID As Integer

    ''' <summary>
    ''' 发送卡索引
    ''' </summary>
    Public SenderID As Integer
    ''' <summary>
    ''' 网口索引
    ''' </summary>
    Public PortID As Integer
    ''' <summary>
    ''' 接收卡索引
    ''' </summary>
    Public ConnectID As Integer

    ''' <summary>
    ''' 原始坐标
    ''' </summary>
    Public LocationOfOriginal As Point
    ''' <summary>
    ''' 在播放窗口的坐标
    ''' </summary>
    Public LocationInDisplayingWindow As Point

    ''' <summary>
    ''' 原始尺寸
    ''' </summary>
    Public SizeOfOriginal As Size
    ''' <summary>
    ''' 缩放后尺寸
    ''' </summary>
    Public SizeOfZoom As Size

    ''' <summary>
    ''' 箱体顺时针旋转角度
    ''' </summary>
    Public BoxRotateAngle As Nova.LCT.GigabitSystem.Common.RotateAngle

End Class
