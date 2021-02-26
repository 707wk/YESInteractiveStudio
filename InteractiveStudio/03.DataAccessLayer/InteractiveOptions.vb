''' <summary>
''' 互动模式
''' </summary>
Public NotInheritable Class InteractiveOptions
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

    ''' <summary>
    ''' 传感器状态
    ''' </summary>
    Public Enum SensorState
        ''' <summary>
        ''' 无操作
        ''' </summary>
        NOOPS
        ''' <summary>
        ''' 按下
        ''' </summary>
        DOWN
        ''' <summary>
        ''' 长按
        ''' </summary>
        PRESS
        '''' <summary>
        '''' 抬起
        '''' </summary>
        'UP
    End Enum

    ''' <summary>
    ''' 发送卡连接状态
    ''' </summary>
    Public Enum SenderConnectState
        ''' <summary>
        ''' 离线
        ''' </summary>
        OffLine
        ''' <summary>
        ''' 在线
        ''' </summary>
        OnLine
    End Enum

End Class
