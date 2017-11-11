Public Class ClassIni
    ''' <summary>
    ''' 读取ini文件内容
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    Public Function GetINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String,
                           ByVal FileName As String) As String
        Dim Str As String = ""
        Str = LSet(Str, 256)
        GetPrivateProfileString(Section, AppName, lpDefault, Str, Len(Str), FileName)
        Return Microsoft.VisualBasic.Left(Str, InStr(Str, Chr(0)) - 1)
    End Function

    ''' <summary>
    ''' 写ini文件操作
    ''' </summary>
    ''' <param name="Section"></param>
    ''' <param name="AppName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="FileName"></param>
    ''' <returns></returns>
    Public Function WriteINI(ByVal Section As String, ByVal AppName As String, ByVal lpDefault As String,
                             ByVal FileName As String) As Long
        WriteINI = WritePrivateProfileString(Section, AppName, lpDefault, FileName)
    End Function

    ''' <summary>
    ''' 读ini API函数
    ''' </summary>
    ''' <param name="lpApplicationName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="lpDefault"></param>
    ''' <param name="lpReturnedString"></param>
    ''' <param name="nSize"></param>
    ''' <param name="lpFileName"></param>
    ''' <returns></returns>
    Private Declare Function GetPrivateProfileString Lib "kernel32" Alias "GetPrivateProfileStringA" (
                             ByVal lpApplicationName As String, ByVal lpKeyName As String,
                             ByVal lpDefault As String, ByVal lpReturnedString As String,
                             ByVal nSize As Int32, ByVal lpFileName As String) As Int32
    ''' <summary>
    ''' 写ini API函数
    ''' </summary>
    ''' <param name="lpApplicationName"></param>
    ''' <param name="lpKeyName"></param>
    ''' <param name="lpString"></param>
    ''' <param name="lpFileName"></param>
    ''' <returns></returns>
    Private Declare Function WritePrivateProfileString Lib "kernel32" Alias "WritePrivateProfileStringA" (
                             ByVal lpApplicationName As String, ByVal lpKeyName As String,
                             ByVal lpString As String, ByVal lpFileName As String) As Int32
End Class
