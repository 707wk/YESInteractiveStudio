Public Module ModuleStructure
    ''' <summary>
    ''' 触发点信息
    ''' </summary>
    Structure PointInfo
        ''' <summary>
        ''' X坐标
        ''' </summary>
        Dim X As Integer
        ''' <summary>
        ''' Y坐标
        ''' </summary>
        Dim Y As Integer
        ''' <summary>
        ''' 分组ID
        ''' </summary>
        Dim GroupID As Integer
        ''' <summary>
        ''' 动作
        ''' </summary>
        Dim Activity As PointActivity
        ''' <summary>
        ''' 点击时长
        ''' </summary>
        Dim TickTime As Integer
    End Structure

    ''' <summary>
    ''' 触发动作
    ''' </summary>
    Enum PointActivity
        ''' <summary>
        ''' 按下
        ''' </summary>
        DOWN
        ''' <summary>
        ''' 长按
        ''' </summary>
        PRESS
        ''' <summary>
        ''' 抬起
        ''' </summary>
        UP
    End Enum
End Module
