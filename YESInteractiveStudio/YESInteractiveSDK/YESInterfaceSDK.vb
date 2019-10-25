﻿Imports System.Windows.Forms

''' <summary>
''' YESInteractive插件需要实现的接口
''' </summary>
Public Interface IYESInterfaceSDK
    Inherits IDisposable

    ''' <summary>
    ''' 加载插件
    ''' </summary>
    ''' <param name="controls">父控件</param>
    Sub InitAddonFunc(controls As Control.ControlCollection)
    '''' <summary>
    '''' 卸载插件
    '''' </summary>
    '''' <param name="controls">父控件</param>
    'Sub FinalizeAddonFunc(controls As Control.ControlCollection)

    '''' <summary>
    '''' 点活动事件
    '''' </summary>
    'Sub PointActive(ByVal Point As PointInfo)

    ''' <summary>
    ''' 点活动事件
    ''' </summary>
    Sub PointActive(ByVal Point As List(Of PointInfo))
End Interface
