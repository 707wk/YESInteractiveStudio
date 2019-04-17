Imports System.Xml.Serialization

Namespace UnityConfig
    ''' <summary>
    ''' Unity素材配置文件
    ''' </summary>
    Module ModuleUnityConfig
        Public Structure argue
            <XmlAttribute>
            Dim key As String
            <XmlAttribute>
            Dim value As String
        End Structure

        Public Structure applicationConfig
            Dim MovieControl As argue()
        End Structure
    End Module

End Namespace
