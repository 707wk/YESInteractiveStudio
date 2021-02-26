Imports Newtonsoft.Json
''' <summary>
''' 全局配置辅助类
''' </summary>
Public Class AppSettingHelper
    Private Sub New()
    End Sub

#Region "程序集GUID"
    <Newtonsoft.Json.JsonIgnore>
    Private _GUID As String
    ''' <summary>
    ''' 程序集GUID
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property GUID As String
        Get
            Return _GUID
        End Get
    End Property
#End Region

#Region "程序集文件版本"
    <Newtonsoft.Json.JsonIgnore>
    Private _ProductVersion As String
    ''' <summary>
    ''' 程序集文件版本
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public ReadOnly Property ProductVersion As String
        Get
            Return _ProductVersion
        End Get
    End Property
#End Region

#Region "配置参数"
    ''' <summary>
    ''' 实例
    ''' </summary>
    Private Shared instance As AppSettingHelper
    ''' <summary>
    ''' 获取实例
    ''' </summary>
    Public Shared ReadOnly Property GetInstance As AppSettingHelper
        Get
            If instance Is Nothing Then
                LoadFromLocaltion()

                '程序集GUID
                Dim guid_attr As Attribute = Attribute.GetCustomAttribute(Reflection.Assembly.GetExecutingAssembly(), GetType(Runtime.InteropServices.GuidAttribute))
                instance._GUID = CType(guid_attr, Runtime.InteropServices.GuidAttribute).Value

                '程序集文件版本
                Dim assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location
                instance._ProductVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion

                ''更新检测地址
                'instance.AppUpdateInfoPath = System.Configuration.ConfigurationManager.AppSettings(NameOf(AppSettingHelper.AppUpdateInfoPath))

                '语言包
                Wangk.Resource.MultiLanguageHelper.Init(instance.SelectLang, My.Application.Info.ProductName)

            End If

            Return instance
        End Get
    End Property
#End Region

#Region "从本地读取配置"
    ''' <summary>
    ''' 从本地读取配置
    ''' </summary>
    Private Shared Sub LoadFromLocaltion()
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.ProductName}")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.ProductName}\Data")

        '反序列化
        Try
            instance = JsonConvert.DeserializeObject(Of AppSettingHelper)(
                System.IO.File.ReadAllText($"{Path}\Hunan Yestech\{My.Application.Info.ProductName}\Data\Setting.json",
                                           System.Text.Encoding.UTF8))

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
            '使用默认参数
            instance = New AppSettingHelper

            With instance
                .PositionaIAccuracy = 64
                .ValidSensorMinimum = 2
                .SensorTouchSensitivity = 5
                .SensorResetTemp = 5
                .SensorResetSec = 25
                .SelectLang = Wangk.Resource.MultiLanguage.LANG.EN
                .DisplayingScheme = New DisplayingScheme
                With .DisplayingScheme
                    .DisplayingWindowItems = New List(Of DisplayingWindow)
                End With

            End With
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

        instance.ValidSensorMinimum = If(instance.ValidSensorMinimum > 0, instance.ValidSensorMinimum, 2)

    End Sub
#End Region

#Region "保存配置到本地"
    ''' <summary>
    ''' 保存配置到本地
    ''' </summary>
    Public Shared Sub SaveToLocaltion()
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.ProductName}")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.ProductName}\Data")

        '序列化
        Try
            Using t As System.IO.StreamWriter = New System.IO.StreamWriter(
                    $"{Path}\Hunan Yestech\{My.Application.Info.ProductName}\Data\Setting.json",
                    False,
                    System.Text.Encoding.UTF8)

                t.Write(JsonConvert.SerializeObject(instance))
            End Using

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
            MsgBox(ex.ToString, MsgBoxStyle.Information, My.Application.Info.Title)
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub
#End Region

#Region "日志记录"
    ''' <summary>
    ''' 日志记录
    ''' </summary>
    <Newtonsoft.Json.JsonIgnore>
    Public Logger As NLog.Logger = NLog.LogManager.GetCurrentClassLogger()
#End Region

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
    ''' 定位精度(点合并范围) 默认50像素
    ''' </summary>
    Public PositionaIAccuracy As Integer

    ''' <summary>
    ''' 有效最小感应点数
    ''' </summary>
    Public ValidSensorMinimum As Integer

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

    ''' <summary>
    ''' 是否自启
    ''' </summary>
    Public IsAutoRun As Boolean

End Class
