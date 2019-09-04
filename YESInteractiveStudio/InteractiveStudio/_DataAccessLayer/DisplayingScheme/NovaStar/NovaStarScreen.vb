''' <summary>
''' Nova屏幕信息
''' </summary>
Public Class NovaStarScreen
    ''' <summary>
    ''' 原始坐标
    ''' </summary>
    Public LocationOfOriginal As Point
    ''' <summary>
    ''' 缩放后坐标
    ''' </summary>
    Public LocationOfZoom As Point

    ''' <summary>
    ''' 原始尺寸
    ''' </summary>
    Public SizeOfOriginal As Size
    ''' <summary>
    ''' 缩放后尺寸
    ''' </summary>
    Public SizeOfZoom As Size

    ''' <summary>
    ''' 接收卡列表
    ''' </summary>
    Public NovaStarScanBoardList As List(Of NovaStarScanBoard)

End Class
