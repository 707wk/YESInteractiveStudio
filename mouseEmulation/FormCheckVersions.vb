Imports System.Net.Sockets
Imports System.Threading

Public Class FormCheckVersions
    '程序版本号
    Private verSion As Byte() = {1, 2, 3}
    Private binLength As Integer

    Private Sub FormCheckVersions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '设置列表格式
        ListView1.View = View.Details
        ListView1.GridLines = True
        ListView1.FullRowSelect = True
        ListView1.CheckBoxes = False
        ListView1.ShowItemToolTips = True
        ListView1.Clear()
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "控制器号", "Sender Id")}", 75, HorizontalAlignment.Left)
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "网口号", "Port Id")}", 70, HorizontalAlignment.Left)
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "接收卡号", "ScanBoard Id")}", 75, HorizontalAlignment.Left)
        ListView1.Columns.Add($"{If(selectLanguageId = 0, "程序版本", "Version")}", 70, HorizontalAlignment.Left)

        TextBox1.Text = $"{verSion(0)}.{verSion(1)}.{verSion(2)}"

        Dim infoReader As System.IO.FileInfo
        Try
            infoReader = My.Computer.FileSystem.GetFileInfo($".\bin\update.bin")
        Catch ex As Exception
            MsgBox("升级文件读取失败", MsgBoxStyle.Information, "载入")
            Me.Close()
            Exit Sub
        End Try
        TextBox2.Text = $"{infoReader.Length / 1024} KB"
        binLength = infoReader.Length

        changeLanguage()
    End Sub

    '读版本号
    Private Sub checkScanBoardVer(index As Integer)
        Dim sendByte As Byte()
        Dim sendstr As String = "aadb0901"
        ReDim sendByte(sendstr.Length \ 2 - 1)

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next

        mainClass.SetScanBoardData(index, 255, 65535, sendByte)

        Thread.Sleep(50)

        Dim cliSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        '发送超时
        cliSocket.SendTimeout = 500
        '接收超时
        cliSocket.ReceiveTimeout = 500
        '连接
        cliSocket.Connect($"{senderArray(index).ipDate(3)}.{senderArray(index).ipDate(2)}.{senderArray(index).ipDate(1)}.{senderArray(index).ipDate(0)}", 6000)

        Try
            Dim bytes(1028 - 1) As Byte
            Dim tmpstr As String = "55d50902"
            Dim sendbytes(4 - 1) As Byte
            For i As Integer = 0 To tmpstr.Length \ 2 - 1
                sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
            Next i

            Dim bytesSend As Integer = cliSocket.Send(sendbytes)
            Dim bytesRec As Integer = cliSocket.Receive(bytes)

        Catch ex As Exception
            MsgBox($"{If(selectLanguageId = 0, "发送读取指令错误", "Error sending read instruction")}")
            'Exit Sub
        End Try

        'Dim asd As New Stopwatch
        'asd.Start()
        Dim showstr As String = Nothing
        Try
            Dim bytes(1028 - 1) As Byte
            Dim tmpstr As String = "55d50905000000000400"
            Dim sendbytes(10 - 1) As Byte
            For i As Integer = 0 To tmpstr.Length \ 2 - 1
                sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
            Next i

            Dim bytesSend As Integer = cliSocket.Send(sendbytes)

            For m As Integer = 0 To 16 - 1
                Dim bytesRec As Integer = cliSocket.Receive(bytes)

                For j As Integer = 4 To 1027 Step 32
                    If bytes(j + 0) <> &H55 Then
                        Continue For
                    End If

                    If bytes(j + 1) > 4 Then
                        Continue For
                    End If

                    'If bytes(j + 4) = verSion(0) And
                    '    bytes(j + 5) = verSion(1) And
                    '    bytes(j + 6) = verSion(2) Then
                    '    Continue For
                    'End If

                    Dim itm As ListViewItem = ListView1.Items.Add($"{index }")
                    itm.SubItems.Add($"{bytes(j + 1)}")
                    itm.SubItems.Add($"{(bytes(j + 2) * 256 + bytes(j + 3))}")
                    itm.SubItems.Add($"{bytes(j + 4)}.{bytes(j + 5)}.{bytes(j + 6)}")
                Next
            Next
        Catch ex As Exception
            MsgBox($"{If(selectLanguageId = 0, "接收数据错误", "Packets received errors")}")
            'Exit Sub
        End Try

        cliSocket.Close()
    End Sub

    '检测版本
    Private Sub Button4_MouseUp(sender As Object, e As EventArgs) Handles Button4.MouseUp
        Button4.Enabled = False

        ListView1.Items.Clear()

        For i As Integer = 0 To senderArray.Length - 1
            checkScanBoardVer(i)
        Next

        Button4.Enabled = True
    End Sub

    '广播升级
    Private Sub updateBin(SenderIndex As Integer, PortIndex As Integer, ConnectIndex As Integer)
        Dim sendByte As Byte()
        Dim sendstr As String = "aadb09030000000000"
        ReDim sendByte(sendstr.Length \ 2 - 1)

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next

        sendByte(4) = binLength \ 256
        sendByte(5) = binLength Mod 256

        sendByte(6) = verSion(0)
        sendByte(7) = verSion(1)
        sendByte(8) = verSion(2)

        Me.TextBox3.AppendText($"{SenderIndex}-update:{mainClass.SetScanBoardData(SenderIndex, PortIndex, ConnectIndex, sendByte)}{vbCrLf}")

        Thread.Sleep(50)

        ReDim sendByte(1025 - 1)
        Dim fs As New System.IO.FileStream($".\bin\update.bin", IO.FileMode.Open, IO.FileAccess.Read)
        Dim re As New System.IO.BinaryReader(fs)

        Dim sendIndex As Integer = 0
        Do
            Dim tmpByte(1024 - 1) As Byte
            re.Read(tmpByte, 0, 1024)

            sendByte(0) = sendIndex
            For i As Integer = 1 To 1024
                sendByte(i) = tmpByte(i - 1)
            Next

            Dim tmpstr As String = Nothing
            For qwe As Integer = 0 To 1025 - 1
                tmpstr = tmpstr & $" {sendByte(qwe).ToString("X")}"
            Next

            Me.TextBox3.AppendText($"{SenderIndex}-{sendIndex}:{mainClass.SetScanBoardData(SenderIndex, PortIndex, ConnectIndex, sendByte)}{vbCrLf}")

            sendIndex += 1

            If sendIndex * 1024 >= binLength Then
                Exit Do
            End If

            Thread.Sleep(50)
        Loop

    End Sub

    '升级程序
    Private Sub Button3_MouseUp(sender As Object, e As EventArgs) Handles Button3.MouseUp
        'For i As Integer = 0 To ListView1.Items.Count - 1
        '    updateBin(CInt(ListView1.Items(i).SubItems(0).Text),
        '              CInt(ListView1.Items(i).SubItems(1).Text),
        '              CInt(ListView1.Items(i).SubItems(2).Text))
        'Next
        Button3.Enabled = False

        Me.TextBox3.Clear()

        For i As Integer = 0 To senderArray.Length - 1
            updateBin(i, 255, 65535)
        Next

        Button3.Enabled = True

        Me.TextBox3.AppendText($"{If(selectLanguageId = 0, "发送完毕", "Send Successfully")}")
        'MsgBox("发送完毕")
    End Sub

    '更改语言 0:中文 1:English
    Private Sub changeLanguage()
        Me.GroupBox1.Text = $"{If(selectLanguageId = 0, "不匹配屏幕", "ScanBoard Version")}"
        Me.Button4.Text = $"{If(selectLanguageId = 0, "检测版本", "Check")}"
        Me.Label2.Text = $"{If(selectLanguageId = 0, "程序大小", "Size")}："
        Me.Label1.Text = $"{If(selectLanguageId = 0, "匹配版本", "Version")}："
        Me.Button3.Text = $"{If(selectLanguageId = 0, "升级程序", "Update")}"
        Me.Text = $"{If(selectLanguageId = 0, "版本检测", "Version Check")}"
    End Sub
End Class