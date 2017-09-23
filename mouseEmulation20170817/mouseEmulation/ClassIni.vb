Public Class ClassIni
    '读取ini文件内容
    'GetINI("Send", "Send1", "", path)
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String,
                           ByVal FileName As String) As String
        Dim Str As String = ""
        Str = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function

    '写ini文件操作
    'WriteINI("Send", "Send1", TextBox1.Text, path)
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String,
                             ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function

    '读ini API函数
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (
                             ByVal lpApplicationName As String, ByVal lpKeyName As String,
                             ByVal lpDefault As String, ByVal lpReturnedString As String,
                             ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    '写ini API函数
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (
                             ByVal lpApplicationName As String, ByVal lpKeyName As String,
                             ByVal lpString As String, ByVal lpFileName As String) As Int32
End Class
