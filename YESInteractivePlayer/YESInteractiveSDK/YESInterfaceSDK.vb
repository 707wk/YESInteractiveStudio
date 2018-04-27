Imports System.Windows.Forms

''' <summary>
''' YESInteractive插件需要实现的接口
''' </summary>
Public Interface IYESInterfaceSDK
    ''' <summary>
    ''' 加载插件
    ''' </summary>
    ''' <param name="Parent">父控件</param>
    Sub InitAddonFunc(ByVal Parent As Control)
    ''' <summary>
    ''' 卸载插件
    ''' </summary>
    ''' <param name="Parent">父控件</param>
    Sub FinalizeAddonFunc(ByVal Parent As Control)

    ''' <summary>
    ''' 点活动事件
    ''' </summary>
    Sub PointActive(ByVal Point As PointInfo)

    ''' <summary>
    ''' 点活动事件
    ''' </summary>
    Sub PointActive(ByVal Point As PointInfo())
End Interface
