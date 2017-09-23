Imports System.Security.Cryptography
Imports System.Text

Module ModuleMd5Hash
    Public Function getMd5Hash(ByVal input As String) As String
        ' 创建新的一个MD5CryptoServiceProvider对象的实例。  
        Dim md5Hasher As New MD5CryptoServiceProvider()
        ' 输入的字符串转换为字节数组，并计算哈希。  
        Dim data As Byte() = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input))
        ' 创建一个新的StringBuilder收集的字节，并创建一个字符串。  
        Dim sBuilder As New StringBuilder()
        ' 通过每个字节的哈希数据和格式为十六进制字符串的每一个循环。  
        'Dim i As Integer
        For i As Integer = 0 To data.Length - 1
            sBuilder.Append(data(i).ToString("X2"))
        Next i
        ' 返回十六进制字符串。  
        Return sBuilder.ToString()
    End Function
End Module
