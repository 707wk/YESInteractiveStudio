Imports InteractiveStudio.InteractiveOptions
''' <summary>
''' 传感器信息
''' </summary>
Public Class Sensor
    ''' <summary>
    ''' 所属播放窗口索引
    ''' </summary>
    Public DisplayingWindowID As Integer

    ''' <summary>
    ''' 索引(网口*100000+接收卡*100+传感器)
    ''' </summary>
    Public Key As Integer

    ''' <summary>
    ''' 点击状态
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public State As SensorState

    ''' <summary>
    ''' 电容值
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public Value As Integer

    ''' <summary>
    ''' 点击中心坐标(screen偏移+传感器旋转后位置)
    ''' </summary>
    Public LocationOfCenter As Point

    ''' <summary>
    ''' 左上角坐标
    ''' </summary>
    Public Location As Point

    ''' <summary>
    ''' 显示尺寸(缩放后显示尺寸)
    ''' </summary>
    Public Size As Integer

End Class
