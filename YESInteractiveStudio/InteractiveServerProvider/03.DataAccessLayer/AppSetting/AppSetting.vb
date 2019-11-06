﻿''' <summary>
''' 全局配置类
''' </summary>
Public Class AppSetting
#Region "播放方案"
    ''' <summary>
    ''' 播放方案
    ''' </summary>
    Public DisplayingScheme As DisplayingScheme
#End Region

#Region "互动参数"
    ''' <summary>
    ''' 定位精度(点合并范围) 默认50像素
    ''' </summary>
    Public PositionaIAccuracy As Integer

    ''' <summary>
    ''' 有效最小感应点数
    ''' </summary>
    Public ValidSensorMinimum As Integer

    '''' <summary>
    '''' 感应点抗干扰 相邻点击数大于等于几个有效 范围 1-9
    '''' </summary>
    'Public ClickValidNums As Integer

    ''' <summary>
    ''' 传感器触摸灵敏度 范围 低 1-9 高
    ''' </summary>
    Public SensorTouchSensitivity As Integer
    ''' <summary>
    ''' 传感器定时复位温度阈值° 范围 0-255
    ''' </summary>
    Public SensorResetTemp As Integer
    ''' <summary>
    ''' 传感器定时复位时间阈值 范围 0-255
    ''' </summary>
    Public SensorResetSec As Integer

    ''' <summary>
    ''' 接收卡程序版本标志
    ''' </summary>
    Public OldScanBoardBin As Boolean

#End Region

End Class