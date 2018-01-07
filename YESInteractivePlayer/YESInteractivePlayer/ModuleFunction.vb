Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Module ModuleFunction
    ''' <summary>
    ''' 输出日志
    ''' </summary>
    Public Sub putlog(str As String)
        Dim tmp As StreamWriter = New StreamWriter($"logs\{Format(Now(), "yyyyMMdd")}.log", True)
        tmp.WriteLine(Format(Now(), "[yyyy-MM-dd HH:mm:ss] ") & str)
        tmp.Close()
    End Sub

    ''' <summary>
    ''' 获取字符串MD5值
    ''' </summary>
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

    ''' <summary>
    ''' 设置显示语言
    ''' </summary>
    Public Sub setControlslanguage(parent As Control)
        For Each c As Control In parent.Controls
            If TypeOf c Is Panel Or
               TypeOf c Is GroupBox Then
                Try
                    Dim tmpstr2() As String = sysInfo.languageTable.Item(c.Text)
                    c.Text = tmpstr2(sysInfo.selectLanguageId + 1)
                Catch ex As Exception
                    putlog($"{parent.GetType.ToString} {c.Text} 更改显示语言异常:{ex.Message}")
                End Try

                For Each i In c.Controls
                    setLanguage(c)
                Next
            Else
                setLanguage(c)
            End If
        Next
    End Sub

    Public Sub setLanguage(con As Control)
        Static conTmp

        If TypeOf con Is Button Then
            '按钮
            conTmp = CType(con, Button)
            conTmp.Text = getLanguage(conTmp.Text)
        ElseIf TypeOf con Is Label Then
            '文本标签
            conTmp = CType(con, Label)
            conTmp.Text = getLanguage(conTmp.Text)
        ElseIf TypeOf con Is CheckBox Then
            '复选框
            conTmp = CType(con, CheckBox)
            conTmp.Text = getLanguage(conTmp.Text)
        ElseIf TypeOf con Is MenuStrip Then
            '菜单栏
            conTmp = CType(con, MenuStrip)
            For Each i As ToolStripMenuItem In conTmp.Items
                i.Text = getLanguage(i.Text)

                For Each j In i.DropDownItems
                    setLanguage(j)
                Next
            Next
        ElseIf TypeOf con Is ToolStrip Then
            '工具栏
            conTmp = CType(con, ToolStrip)
            For Each i In conTmp.Items
                If TypeOf i Is ToolStripButton Then
                    conTmp = CType(i, ToolStripButton)
                    conTmp.Text = getLanguage(conTmp.Text)
                ElseIf TypeOf i Is ToolStripLabel Then
                    conTmp = CType(i, ToolStripLabel)
                    conTmp.Text = getLanguage(conTmp.Text)
                ElseIf TypeOf i Is ToolStripSplitButton Then
                    conTmp = CType(i, ToolStripSplitButton)
                    conTmp.Text = getLanguage(conTmp.Text)

                    For Each j In conTmp.DropDownItems
                        j.Text = getLanguage(j.Text)
                    Next
                ElseIf TypeOf i Is ToolStripDropDownButton Then
                    conTmp = CType(i, ToolStripDropDownButton)
                    conTmp.Text = getLanguage(conTmp.Text)

                    For Each j In conTmp.DropDownItems
                        j.Text = getLanguage(j.Text)
                    Next
                End If
            Next
        ElseIf TypeOf con Is DataGridView Then
            '数据表格
            conTmp = CType(con, DataGridView)
            For Each i In conTmp.Columns
                i.HeaderText = getLanguage(i.HeaderText)
            Next
        ElseIf TypeOf con Is TabControl Then
            '选项卡
            conTmp = CType(con, TabControl)
            For Each i As TabPage In conTmp.TabPages
                i.Text = getLanguage(i.Text)

                For Each j In i.Controls
                    setLanguage(j)
                Next
            Next
        ElseIf TypeOf con Is ListView Then
            '列表
            conTmp = CType(con, ListView)
            For Each i As ColumnHeader In conTmp.Columns
                i.Text = getLanguage(i.Text)
            Next
        ElseIf TypeOf con Is GroupBox Then
            '分组框
            conTmp = CType(con, GroupBox)
            conTmp.text = getLanguage(conTmp.text)
            For Each i In conTmp.Controls
                setLanguage(i)
            Next
        End If
    End Sub

    ''' <summary>
    ''' 获取实际显示文本
    ''' </summary>
    Public Function getLanguage(keyStr As String)
        Dim tmpstr2() As String = sysInfo.languageTable.Item(keyStr)
        If tmpstr2 Is Nothing Then
            Return keyStr
        ElseIf sysInfo.selectLanguageId + 1 > tmpstr2.Length Then
            Return keyStr
        Else
            Return tmpstr2(sysInfo.selectLanguageId)
        End If
    End Function
End Module
