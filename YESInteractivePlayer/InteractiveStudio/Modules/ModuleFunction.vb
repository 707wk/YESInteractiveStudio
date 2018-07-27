Imports System.IO
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization

Module ModuleFunction
#Region "读取配置"
    ''' <summary>
    ''' 读取配置
    ''' </summary>
    Public Function LoadSetting() As Boolean
        System.IO.Directory.CreateDirectory("./Data")

        '反序列化
        Try
            Using fStream As New FileStream("./Data/Setting.xml", FileMode.Open)
                Dim XmlSerializer As XmlSerializer = New XmlSerializer(GetType(SystemInfo))
                sysInfo = XmlSerializer.Deserialize(fStream)
            End Using
        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   "读取配置异常")

            '第一次使用初始化参数
            With sysInfo
                .TouchSensitivity = 5
                .ClickValidNums = 2
                .ResetTemp = 5
                .ResetSec = 25
                .TouchMode = 1
            End With
            Return False
        End Try

        Return True
    End Function
#End Region

#Region "保存配置"
    ''' <summary>
    ''' 保存配置
    ''' </summary>
    Public Function SaveSetting() As Boolean
        System.IO.Directory.CreateDirectory("./Data")

        '序列化
        Try
            Using fStream As New FileStream("./Data/Setting.xml", FileMode.Create)
                Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
                ns.Add("", "") '删除命名空间
                '添加编码属性
                Dim tmpXmlTextWriter As XmlTextWriter = New XmlTextWriter(fStream, Encoding.UTF8) With {
                    .Formatting = Formatting.Indented '子节点缩进
                }
                Dim sfFormatter As New XmlSerializer(GetType(SystemInfo))
                sfFormatter.Serialize(tmpXmlTextWriter, sysInfo, ns)
            End Using

        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   "保存配置异常")
            Return False
        End Try

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
                Dim XmlSerializer As XmlSerializer = New XmlSerializer(GetType(WindowInfo))
                sysInfo.WindowList = XmlSerializer.Deserialize(fStream)
            End Using
        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   "读取配置异常")
            Return False
        End Try

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
                Dim sfFormatter As New XmlSerializer(GetType(WindowInfo))
                sfFormatter.Serialize(tmpXmlTextWriter, sysInfo.WindowList, ns)
            End Using

        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   "保存文件异常")
            Return False
        End Try

        Return True
    End Function
#End Region
End Module
