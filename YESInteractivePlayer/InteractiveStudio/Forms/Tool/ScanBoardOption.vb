Imports System.Net.Sockets
Imports System.Threading

Public Class ScanBoardOption
    Private Sub ScanBoardOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''todo:接收卡单片机升级
#Region "样式设置"
        Me.Text = sysInfo.Language.GetS("ScanBoard")

        With ListView1
            .View = View.Details
            .GridLines = True
            .FullRowSelect = True
            .CheckBoxes = False
            .ShowItemToolTips = True
            .Columns.Add(sysInfo.Language.GetS("Sender"), 60, HorizontalAlignment.Left)
            .Columns.Add(sysInfo.Language.GetS("Port"), 60, HorizontalAlignment.Left)
            .Columns.Add(sysInfo.Language.GetS("ScanBoard"), 60, HorizontalAlignment.Left)
            .Columns.Add(sysInfo.Language.GetS("Version"), 60, HorizontalAlignment.Left)
        End With

        CheckBox1.Checked = sysInfo.ScanBoardOldFlage

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
#End Region
    End Sub

#Region "MCU旧版标记"
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        sysInfo.ScanBoardOldFlage = CheckBox1.Checked
    End Sub
#End Region

#Region "查询MCU版本号"
#Region "按钮事件"
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        ToolStripButton1.Enabled = False
    End Sub

    Private Sub ToolStripButton1_MouseUp(sender As Object, e As MouseEventArgs) Handles ToolStripButton1.MouseUp
        If ToolStripButton1.Enabled Then
            Exit Sub
        End If

        ListView1.Items.Clear()

        For i001 As Integer = 0 To sysInfo.SenderList.Count - 1
            GetSenderMCUVersion(i001)
        Next

        ToolStripButton1.Enabled = True
    End Sub
#End Region

#Region "获取发送卡下MCU版本号"
    ''' <summary>
    ''' 获取发送卡下MCU版本号
    ''' </summary>
    Public Sub GetSenderMCUVersion(ByVal SenderId As Integer)
        If Not sysInfo.ScanBoardOldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(SenderId, &HFF, &HFFFF, Wangk.Hash.Hex2Bin("aadb0901"))
        Else
            sysInfo.MainClass.SetOldScanBoardData(SenderId, &HFF, &HFFFF, Wangk.Hash.Hex2Bin("aadb0901"))
        End If

        If CheckBox2.Checked Then
            Thread.Sleep(50)
        Else
            Thread.Sleep(500)
        End If

        Try
            Dim cliSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
                .SendTimeout = 500,
                .ReceiveTimeout = 500
            }
            '连接
            With sysInfo.SenderList(SenderId)
                cliSocket.Connect(
                    String.Format("{0}.{1}.{2}.{3}", .IpDate(3), .IpDate(2), .IpDate(1), .IpDate(0)),
                    6000)
            End With


            Dim bytes(1028 - 1) As Byte
            Dim bytesSend As Integer = cliSocket.Send(Wangk.Hash.Hex2Bin("55d50902"))
            Dim bytesRec As Integer = cliSocket.Receive(bytes)


            Dim getDataSum As Integer = 0
            cliSocket.Send(Wangk.Hash.Hex2Bin("55d50905000000000400"))

            For m As Integer = 0 To 16 - 1
                bytesRec = cliSocket.Receive(bytes)

                For j As Integer = 4 To 1027 Step 32
                    If bytes(j + 0) <> &H55 Then
                        Continue For
                    End If

                    If bytes(j + 1) > 4 Then
                        Continue For
                    End If

                    Dim itm As ListViewItem = ListView1.Items.Add($"{SenderId}")
                    itm.SubItems.Add($"{bytes(j + 1)}")
                    itm.SubItems.Add($"{(bytes(j + 2) * 256 + bytes(j + 3))}")

                    itm.SubItems.Add($"{bytes(j + 4)}.{bytes(j + 5)}.{bytes(j + 6)}")

                    getDataSum += 1
                Next
            Next

            cliSocket.Close()
        Catch ex As Exception
            MsgBox(ex.Message,
                   MsgBoxStyle.Information,
                   sysInfo.Language.GetS("Connect Exception"))
            Exit Sub
        End Try
    End Sub
#End Region
#End Region

#Region "升级MCU程序"
#Region "选择升级文件"
    ''' <summary>
    '''选择升级文件
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim TmpDialog As New OpenFileDialog With {
            .Filter = sysInfo.Language.GetS("MCU Bin") & "|*.bin"
        }
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        TextBox1.Text = TmpDialog.FileName
    End Sub
#End Region

#Region "按钮事件"
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Button2.Enabled = False
    End Sub

    Private Sub Button2_MouseUp(sender As Object, e As MouseEventArgs) Handles Button2.MouseUp
        If Button2.Enabled OrElse
            TextBox1.Text = "" Then
            Button2.Enabled = True
            Exit Sub
        End If

        SendMCUBin()

        Button2.Enabled = True
    End Sub
#End Region

#Region "发送升级包"
    ''' <summary>
    ''' 发送升级包
    ''' </summary>
    Public Sub SendMCUBin()
        '读文件信息
        Dim infoReader As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(TextBox1.Text)
        Dim binLength = infoReader.Length

        '发送升级指令
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb09030000")
        sendByte(4) = binLength \ 256
        sendByte(5) = binLength Mod 256

        If Not sysInfo.ScanBoardOldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        End If

        Thread.Sleep(60)

        For i As Integer = 0 To 10
            If CheckUpdataRecData(Wangk.Hash.Hex2Bin("1a1b")) Then
                Exit For
            End If

            If i = 10 Then
                'Putlog($"升级指令发送失败")
                MsgBox(sysInfo.Language.GetS("Upgrade command failed to send"),
                           MsgBoxStyle.Information,
                           sysInfo.Language.GetS("Update"))
                Exit Sub
            End If
        Next


        '发送升级程序
        ReDim sendByte(131 - 1)
        Dim fs As New System.IO.FileStream(TextBox1.Text, IO.FileMode.Open, IO.FileAccess.Read)
        Dim re As New System.IO.BinaryReader(fs)

        Dim sendIndex As Integer = 0
        Do
            Dim tmpByte(128 - 1) As Byte
            '从文件读取的字节数
            Dim readByteNum As Integer = re.Read(tmpByte, 0, 128)

            sendByte(0) = sendIndex

            Dim checkSum As Integer = 0
            For i As Integer = 1 To 128
                If i <= readByteNum Then
                    sendByte(i) = tmpByte(i - 1)
                    checkSum += sendByte(i)
                Else
                    '不足128字节则填充
                    sendByte(i) = &HFF
                End If
            Next

            sendByte(129) = (checkSum \ 256) Mod 256
            sendByte(130) = checkSum Mod 256

            For i As Integer = 0 To 10
                If Not sysInfo.ScanBoardOldFlage Then
                    sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
                Else
                    sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
                End If

                Thread.Sleep(60)

                If CheckUpdataRecData(Wangk.Hash.Hex2Bin("1c1d")) Then
                    Exit For
                End If

                If i = 10 Then
                    re.Close()
                    fs.Close()
                    MsgBox(sysInfo.Language.GetS("Upgrade data failed to send"),
                           MsgBoxStyle.Information,
                           sysInfo.Language.GetS("Update"))
                    Exit Sub
                End If
            Next

            sendIndex += 1

            ProgressBar1.Value = (sendIndex * 128 * 100) \ binLength
            Label2.Text = $"{ProgressBar1.Value}%"

            If sendIndex * 128 >= binLength Then
                Exit Do
            End If
        Loop


        '发送完毕指令
        If Not sysInfo.ScanBoardOldFlage Then
            sysInfo.MainClass.SetNewScanBoardData(&HFF, &HFF, &HFFFF, Wangk.Hash.Hex2Bin("aadb0909"))
        Else
            sysInfo.MainClass.SetOldScanBoardData(&HFF, &HFF, &HFFFF, Wangk.Hash.Hex2Bin("aadb0909"))
        End If

        re.Close()
        fs.Close()

        MsgBox(sysInfo.Language.GetS("Program upgrade completed"),
               MsgBoxStyle.Information,
               sysInfo.Language.GetS("Update"))
    End Sub
#End Region

#Region "比较收到的数据"
    ''' <summary>
    ''' 比较收到的数据
    ''' </summary>
    Private Function CheckUpdataRecData(checkData() As Byte) As Boolean
        Dim recSum As Integer = 0
        Dim errorSum As Integer = 0

        For Each sender As SenderInfo In sysInfo.SenderList
            Dim cliSocket As Socket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
            .SendTimeout = 1000,
                .ReceiveTimeout = 1000
            }
            '连接
            cliSocket.Connect($"{sender.IpDate(3)}.{sender.IpDate(2)}.{sender.IpDate(1)}.{sender.IpDate(0)}", 6000)

            Try
                Dim bytes(1028 - 1) As Byte
                Dim bytesSend As Integer = cliSocket.Send(Wangk.Hash.Hex2Bin("55d50902"))
                Dim bytesRec As Integer = cliSocket.Receive(bytes)

            Catch ex As Exception
                cliSocket.Close()
                Return False
            End Try

            Try
                Dim bytes(1028 - 1) As Byte
                Dim bytesSend As Integer = cliSocket.Send(Wangk.Hash.Hex2Bin("55d50905000000000400"))

                For m As Integer = 0 To 16 - 1
                    Dim bytesRec As Integer = cliSocket.Receive(bytes)

                    For j As Integer = 4 To 1027 Step 32
                        If bytes(j + 0) <> &H55 Then
                            Continue For
                        End If

                        If bytes(j + 1) > 4 Then
                            Continue For
                        End If

                        For i As Integer = 0 To checkData.Length - 1
                            If bytes(j + 4 + i) <> checkData(i) Then
                                errorSum += 1
                                Exit For
                            End If
                        Next

                        recSum = recSum + 1
                    Next
                Next
            Catch ex As Exception
                errorSum += 1
            End Try

            cliSocket.Close()
        Next

        If errorSum > 0 Then
            Return False
        End If

        Return If(recSum > 0, True, False)
    End Function
#End Region

#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.GroupBox1.Text = .GetS("ScanBoard List")
            Me.ToolStripButton1.Text = .GetS("Get MCU Version")
            Me.GroupBox2.Text = .GetS("MCU")
            Me.Label2.Text = .GetS("0%")
            Me.Button2.Text = .GetS("Update")
            Me.Button1.Text = .GetS("Browse ...")
            Me.Label1.Text = .GetS("File")
            Me.GroupBox3.Text = .GetS("ScanBoard Version")
            Me.CheckBox1.Text = .GetS("Old ScanBoard Version")
            Me.CheckBox2.Text = .GetS("Old MCU Version")
        End With
    End Sub
#End Region
End Class