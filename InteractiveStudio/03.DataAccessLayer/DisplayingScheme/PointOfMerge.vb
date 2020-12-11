''' <summary>
''' 合并点信息
''' </summary>
Public Class PointOfMerge
    ''' <summary>
    ''' X坐标
    ''' </summary>
    Public X As Integer
    ''' <summary>
    ''' Y坐标
    ''' </summary>
    Public Y As Integer

    ''' <summary>
    ''' X坐标和
    ''' </summary>
    Public XSum As Integer
    ''' <summary>
    ''' Y坐标和
    ''' </summary>
    Public YSum As Integer

    ''' <summary>
    ''' 待合并点数量
    ''' </summary>
    Public SensorCount As Integer = 0

    ''' <summary>
    ''' 是否是新点
    ''' </summary>
    Public IsNew As Boolean = False

End Class
