''' <summary>
''' Nova接收卡信息
''' </summary>
Public Class NovaStarScanBoard
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
    Public ScannerID As Integer

    ''' <summary>
    ''' 原始坐标
    ''' </summary>
    Public LocationOfOriginal As Point

    ''' <summary>
    ''' 原始尺寸
    ''' </summary>
    Public SizeOfOriginal As Size

    ''' <summary>
    ''' 箱体旋转角度
    ''' </summary>
    Public BoxRotateAngle As Nova.LCT.GigabitSystem.Common.RotateAngle

End Class
