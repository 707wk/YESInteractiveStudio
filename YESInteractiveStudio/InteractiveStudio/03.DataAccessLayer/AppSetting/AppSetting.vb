''' <summary>
''' 全局配置类
''' </summary>
Public Class AppSetting
#Region "工具"
    ''' <summary>
    ''' 语言类型
    ''' </summary>
    Public SelectLang As Wangk.Resource.MultiLanguage.LANG
#End Region

#Region "播放方案"
    ''' <summary>
    ''' 播放方案
    ''' </summary>
    Public DisplayingScheme As DisplayingScheme
#End Region

#Region "互动参数"
    ''' <summary>
    ''' 显示模式
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public DisplayMode As InteractiveOptions.DISPLAYMODE
    ''' <summary>
    ''' 感应精度 范围 高 1-9 低
    ''' </summary>
    Public TouchPrecision As Integer

    ''' <summary>
    ''' 感应点抗干扰 相邻点击数大于等于几个有效 范围 1-9
    ''' </summary>
    Public ClickValidNums As Integer

    ''' <summary>
    ''' 传感器触摸灵敏度 范围 低 1-9 高
    ''' </summary>
    Public SensorTouchSensitivity As Integer
    ''' <summary>
    ''' 传感器定时复位温度阈值° 范围 0-255
    ''' </summary>
    Public SensorResetTemp As Integer
    ''' <summary>
    ''' 传感器定时复位时间阈值″ 范围 0-255
    ''' </summary>
    Public SensorResetSec As Integer
#End Region

End Class
