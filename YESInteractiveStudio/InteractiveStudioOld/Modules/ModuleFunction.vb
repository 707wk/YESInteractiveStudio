﻿Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Module ModuleFunction
#Region "读取配置"
    ''' <summary>
    ''' 读取配置
    ''' </summary>
    Public Function LoadSetting() As Boolean
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.Title}")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.Title}\Data")

        '反序列化
        Try
            Using fStream As New FileStream($"{Path}\Hunan Yestech\{My.Application.Info.Title}\Data\Setting.xml",
                                            FileMode.Open)
                Dim XmlSerializer As XmlSerializer = New XmlSerializer(GetType(SystemInfo))
                AppSetting = XmlSerializer.Deserialize(fStream)
            End Using

        Catch ex As Exception
            '使用默认参数
            With AppSetting
                .HistoryFile = ""
                .TouchSensitivity = 5
                .ClickValidNums = 2
                .ResetTemp = 5
                .ResetSec = 25
                '.TouchMode = 1
            End With
            Return True
            'Catch ex2 As Exception
            '    MsgBox(ex2.Message,
            '           MsgBoxStyle.Information,
            '           "读取配置异常")
        End Try

        Return True
    End Function
#End Region

#Region "保存配置"
    ''' <summary>
    ''' 保存配置
    ''' </summary>
    Public Function SaveSetting() As Boolean
        Dim Path As String = System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)

        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.Title}")
        System.IO.Directory.CreateDirectory($"{Path}\Hunan Yestech\{My.Application.Info.Title}\Data")

        '序列化
        'Try
        Using fStream As New FileStream($"{Path}\Hunan Yestech\{My.Application.Info.Title}\Data\Setting.xml",
                                            FileMode.Create)
                Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
                ns.Add("", "") '删除命名空间
                '添加编码属性
                Dim tmpXmlTextWriter As XmlTextWriter = New XmlTextWriter(fStream, Encoding.UTF8) With {
                    .Formatting = Formatting.Indented '子节点缩进
                }
                Dim sfFormatter As New XmlSerializer(GetType(SystemInfo))
                sfFormatter.Serialize(tmpXmlTextWriter, AppSetting, ns)
            End Using

        'Catch ex As Exception
        '    MsgBox(ex.Message,
        '           MsgBoxStyle.Information,
        '           "Save Setting")
        '    Return False
        'End Try

        Return True
    End Function
#End Region

#Region "读取文件"
    ''' <summary>
    ''' 读取文件
    ''' </summary>
    Public Function LoadFile(ByVal Path As String) As Boolean
        '反序列化
        Try
            Using fStream As New FileStream(Path, FileMode.Open)
                Dim XmlSerializer As XmlSerializer = New XmlSerializer(GetType(ScheduleInfo))
                AppSetting.Schedule = XmlSerializer.Deserialize(fStream)
            End Using
        Catch ex As Exception
            'MsgBox(ex.Message,
            '       MsgBoxStyle.Information,
            '       "读取文件异常")
            'Return False
            AppSetting.HistoryFile = ""
            AppSetting.Schedule.WindowList = New List(Of WindowInfo)
        End Try

        AppSetting.InquireTimeSec = 50
        For Each i001 In AppSetting.Schedule.WindowList
            i001.PlayMediaId = -1
        Next
        ReDim Preserve AppSetting.Schedule.ScreenList(AppSetting.ScreenList.Count - 1)

        Return True
    End Function
#End Region

#Region "保存文件"
    ''' <summary>
    ''' 保存文件
    ''' </summary>
    Public Function SaveFile(ByVal Path As String) As Boolean
        '序列化
        Try
            Using fStream As New FileStream(Path, FileMode.Create)
                Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
                ns.Add("", "") '删除命名空间
                '添加编码属性
                Dim tmpXmlTextWriter As XmlTextWriter = New XmlTextWriter(fStream, Encoding.UTF8) With {
                    .Formatting = Formatting.Indented '子节点缩进
                }
                Dim sfFormatter As New XmlSerializer(GetType(ScheduleInfo))
                sfFormatter.Serialize(tmpXmlTextWriter, AppSetting.Schedule, ns)
            End Using

        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   "Save File")
            Return False
        End Try

        Return True
    End Function
#End Region

#Region "屏幕索引转窗口屏幕索引"
    '''' <summary>
    '''' 屏幕索引转窗口屏幕索引
    '''' </summary>
    '''' <param name="WindowId"></param>
    '''' <param name="ScreenId"></param>
    '''' <returns></returns>
    'Public Function SI2SLI(ByVal WindowId As Integer, ByVal ScreenId As Integer) As Integer
    '    Dim ID As Integer = 0
    '    For Each i001 As ScreenInWindow In sysInfo.Schedule.WindowList(WindowId).ScreenList
    '        If i001.ScreenID = ScreenId Then
    '            Return ID
    '        End If

    '        ID += 1
    '    Next

    '    Return 0
    'End Function
#End Region
End Module