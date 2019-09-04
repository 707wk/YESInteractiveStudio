''' <summary>
''' 播放方案类
''' </summary>
Public Class DisplayingScheme

    ''' <summary>
    ''' 播放窗口列表
    ''' </summary>
    Public DisplayingWindowList As List(Of DisplayingWindow)

    ''' <summary>
    ''' 屏幕数组
    ''' </summary>
    Public NovaStarScreenArray As NovaStarScreen()

    ''' <summary>
    ''' 发送卡数组
    ''' </summary>
    Public NovaStarSenderArray As NovaStarSender()

    '''' <summary>
    '''' 接收卡查找表 Key=发送卡*10000+网口*1000+接收卡
    '''' </summary>
    'Public NovaStarScanBoardDictionary As Dictionary(Of Integer, NovaStarScanBoard)

    ''' <summary>
    ''' 传感器查找表 Key=发送卡*1000000+网口*100000+接收卡*100+传感器
    ''' </summary>
    Public SensorDictionary As Dictionary(Of Integer, Sensor)

End Class
