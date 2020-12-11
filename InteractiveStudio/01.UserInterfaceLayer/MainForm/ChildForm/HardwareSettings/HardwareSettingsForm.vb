Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading
Imports Nova.Mars.SDK
Imports Wangk.Resource

Public Class HardwareSettingsForm
    ''' <summary>
    ''' Nova配置变量
    ''' </summary>
    Public NovaMarsControl As MarsControlSystem
    ''' <summary>
    ''' 读取到的发送卡数据
    ''' </summary>
    Public NovaStarSenderItems As NovaStarSender()

    Private Sub HardwareSettingsForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        With DataGridView1
            .BorderStyle = BorderStyle.None
            .RowHeadersVisible = True
            .AllowUserToResizeRows = False
            .AllowUserToOrderColumns = False
            .AllowUserToResizeColumns = False
            .SelectionMode = DataGridViewSelectionMode.CellSelect
            .MultiSelect = False
            .AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(&HE9, &HED, &HF4)
            .GridColor = Color.FromArgb(&HE5, &HE5, &HE5)
            .CellBorderStyle = DataGridViewCellBorderStyle.SingleVertical
            .EditMode = DataGridViewEditMode.EditOnEnter

            For Each i001 As DataGridViewColumn In .Columns
                i001.SortMode = DataGridViewColumnSortMode.NotSortable
            Next
        End With

        '绑定IP设置事件
        AddHandler NovaMarsControl.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        '复位温度
        With ComboBox1
            For i001 As Integer = 5 To 255
                .Items.Add(i001)
            Next
        End With

        '复位时间
        With ComboBox2
            .Items.Add(0)
            For i001 As Integer = 25 To 255
                .Items.Add(i001)
            Next
        End With

        '接收卡状态
        With TreeView1
            .CheckBoxes = False
            .ImageList = ImageList1
            .ShowLines = True
            .ShowRootLines = True
        End With
#End Region

        ChangeControlsLanguage()

    End Sub

    Private Sub HardwareSettingsForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
#Region "控制器"
        Using tmpDialog As New Wangk.Resource.BackgroundWorkDialog
            tmpDialog.Start(Sub()
#Region "读取接收卡IP"
                                AddHandler NovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

                                For itemID = 0 To NovaStarSenderItems.Count - 1
                                    NovaStarSenderItems(itemID) = New NovaStarSender

                                    SenderIPData = Nothing

                                    NovaMarsControl.GetEquipmentIP(itemID)
                                    GetEquipmentIPDataEvent.WaitOne()

                                    If SenderIPData Is Nothing Then Throw New Exception($"{MultiLanguageHelper.Lang.GetS("Sender")} {itemID} {MultiLanguageHelper.Lang.GetS("no support for interactive")}")

                                    NovaStarSenderItems(itemID).IpData = SenderIPData

                                Next

                                RemoveHandler NovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
#End Region
                            End Sub)

            If tmpDialog.Error IsNot Nothing Then
                MsgBox(tmpDialog.Error.Message,
                       MsgBoxStyle.Information,
                       tmpDialog.Text)
                Me.Close()
            Else
                For Each item In NovaStarSenderItems
                    DataGridView1.Rows.Add({item.IPAddress,
                                           item.IPSubnetMask,
                                           item.IPGateway,
                                           MultiLanguageHelper.Lang.GetS("Apply")})
                Next
            End If

        End Using
        'For Each item In AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems
        '    DataGridView1.Rows.Add({item.IPAddress, item.IPSubnetMask, item.IPGateway, "Apply"})
        'Next
#End Region

        If AppSettingHelper.GetInstance.OldScanBoardBin Then
            RadioButton1.Checked = True
        Else
            RadioButton2.Checked = True
        End If

#Region "传感器"
        TrackBar1.Value = AppSettingHelper.GetInstance.SensorTouchSensitivity
        ComboBox1.Text = AppSettingHelper.GetInstance.SensorResetTemp
        ComboBox2.Text = AppSettingHelper.GetInstance.SensorResetSec
#End Region

#Region "单片机"

#End Region

    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        AppSettingHelper.GetInstance.OldScanBoardBin = True
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        AppSettingHelper.GetInstance.OldScanBoardBin = False
    End Sub

#Region "修改IP信息"
#Disable Warning IDE0069 ' 应释放可释放的字段
    ''' <summary>
    ''' 写入IP标志
    ''' </summary>
    Private ReadOnly SetEquipmentIPDataEvent As New Threading.AutoResetEvent(False)
#Enable Warning IDE0069 ' 应释放可释放的字段
    ''' <summary>
    ''' 写入结果
    ''' </summary>
    Private SetEquipmentIPDataResult As Boolean

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick
        If DataGridView1.Columns(e.ColumnIndex).HeaderText <> MultiLanguageHelper.Lang.GetS("Apply Changes") Then Exit Sub
        If e.RowIndex < 0 Then Exit Sub

        'ip校验正确性
        For itemID = 0 To 3 - 1
            If Not System.Net.IPAddress.TryParse(DataGridView1.Rows(e.RowIndex).Cells(itemID).Value, Nothing) OrElse
                $"{DataGridView1.Rows(e.RowIndex).Cells(itemID).Value}".Split(".").Count <> 4 Then

                MsgBox($"{DataGridView1.Columns(itemID).HeaderText} {MultiLanguageHelper.Lang.GetS("formal error")}", MsgBoxStyle.Information, Me.Text)
                Exit Sub
            End If
        Next

        '临时IP
        Dim TmpIpData(12 - 1) As Byte
        'ip
        Dim tmpIPStr = $"{DataGridView1.Rows(e.RowIndex).Cells(0).Value}".Split(".")
        TmpIpData(3) = Val(tmpIPStr(0))
        TmpIpData(2) = Val(tmpIPStr(1))
        TmpIpData(1) = Val(tmpIPStr(2))
        TmpIpData(0) = Val(tmpIPStr(3))
        '子网掩码
        tmpIPStr = $"{DataGridView1.Rows(e.RowIndex).Cells(1).Value}".Split(".")
        TmpIpData(7) = Val(tmpIPStr(0))
        TmpIpData(6) = Val(tmpIPStr(1))
        TmpIpData(5) = Val(tmpIPStr(2))
        TmpIpData(4) = Val(tmpIPStr(3))
        '网关
        tmpIPStr = $"{DataGridView1.Rows(e.RowIndex).Cells(2).Value}".Split(".")
        TmpIpData(11) = Val(tmpIPStr(0))
        TmpIpData(10) = Val(tmpIPStr(1))
        TmpIpData(9) = Val(tmpIPStr(2))
        TmpIpData(8) = Val(tmpIPStr(3))

        Using tmpDialog As New Wangk.Resource.BackgroundWorkDialog With {
            .Text = MultiLanguageHelper.Lang.GetS("Apply Changes")
        }

            tmpDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
                                NovaMarsControl.SetEquipmentIP(Val(uie.Args), TmpIpData)
                                SetEquipmentIPDataEvent.WaitOne()
                            End Sub,
                            e.RowIndex)

        End Using

        If SetEquipmentIPDataResult Then
            'AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems(e.RowIndex).IpData = TmpIpData
            MsgBox(MultiLanguageHelper.Lang.GetS("IP modify successfully"))
        Else
            MsgBox(MultiLanguageHelper.Lang.GetS("Fail to modify IP"))
        End If

    End Sub

#Region "获取发送卡IP"
#Disable Warning IDE0069 ' 应释放可释放的字段
    ''' <summary>
    ''' 读取到IP标志
    ''' </summary>
    Private ReadOnly GetEquipmentIPDataEvent As New AutoResetEvent(False)
#Enable Warning IDE0069 ' 应释放可释放的字段
    ''' <summary>
    ''' 读到的发送卡IP
    ''' </summary>
    Private SenderIPData As Byte()

    ''' <summary>
    ''' 获取IP通知
    ''' </summary>
    Private Sub GetEquipmentIPData(sender As Object, e As Nova.Mars.SDK.MarsEquipmentIPEventArgs)
        Static Dim senderArrayId As Integer = 0

        If e.IsExecResult Then
            SenderIPData = e.Data
        End If

        GetEquipmentIPDataEvent.Set()
    End Sub
#End Region

#Region "发送设备IP回调事件"
    ''' <summary>
    ''' 发送设备IP回调事件
    ''' </summary>
    Private Sub SendEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        SetEquipmentIPDataResult = e.IsExecResult
        SetEquipmentIPDataEvent.Set()
    End Sub
#End Region

    Private Sub HardwareSettingsForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '解绑IP设置事件
        RemoveHandler NovaMarsControl.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

    End Sub
#End Region

#Region "修改传感器参数"
    '灵敏度
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb0305")
        sendByte(3) = TrackBar1.Value

        If SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte) Then
            AppSettingHelper.GetInstance.SensorTouchSensitivity = TrackBar1.Value
            MsgBox(MultiLanguageHelper.Lang.GetS("Updated successfully"), MsgBoxStyle.Information, Me.Text)
        Else
            MsgBox(MultiLanguageHelper.Lang.GetS("Fail to modify"))
        End If

        Threading.Thread.Sleep(200)
    End Sub

    '温度复位幅度
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010300")
        sendByte(4) = Val(ComboBox1.Text)

        If SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte) Then
            AppSettingHelper.GetInstance.SensorResetTemp = Val(ComboBox1.Text)
            MsgBox(MultiLanguageHelper.Lang.GetS("Updated successfully"), MsgBoxStyle.Information, Me.Text)
        Else
            MsgBox(MultiLanguageHelper.Lang.GetS("Fail to modify"))
        End If

        Threading.Thread.Sleep(200)
    End Sub

    '时间复位幅度
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb010200")
        sendByte(4) = Val(ComboBox2.Text)

        If SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte) Then
            AppSettingHelper.GetInstance.SensorResetSec = Val(ComboBox2.Text)
            MsgBox(MultiLanguageHelper.Lang.GetS("Updated successfully"), MsgBoxStyle.Information, Me.Text)
        Else
            MsgBox(MultiLanguageHelper.Lang.GetS("Fail to modify"))
        End If

        Threading.Thread.Sleep(200)
    End Sub
#End Region

#Region "单片机信息"
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Using tmpDialog As New OpenFileDialog With {
            .Filter = $"{MultiLanguageHelper.Lang.GetS("Bin File")}|*.bin"
        }

            If tmpDialog.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            TextBox3.Text = tmpDialog.FileName

        End Using
    End Sub

    '全选子项
    Private Sub TreeView1_AfterCheck(sender As Object, e As TreeViewEventArgs) Handles TreeView1.AfterCheck
        If e.Node.Level = 0 Then
            For Each i001 As TreeNode In e.Node.Nodes
                i001.Checked = e.Node.Checked
            Next
        End If
    End Sub

#Region "升级"
    'Private UpdateMCUSocket As Socket

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        '检查文件
        If String.IsNullOrEmpty(TextBox3.Text) Then
            MsgBox(MultiLanguageHelper.Lang.GetS("Not Select Bin File"),
                   MsgBoxStyle.Information,
                   MultiLanguageHelper.Lang.GetS("Update program"))
            Button5.Enabled = True
            Exit Sub
        End If

        Using tmpScanBoardSelectForm As New ScanBoardSelectForm
            For Each item As TreeNode In TreeView1.Nodes
                tmpScanBoardSelectForm.TreeView1.Nodes.Add(item.Clone)
            Next
            If tmpScanBoardSelectForm.ShowDialog <> DialogResult.OK Then
                Exit Sub
            End If

            'Try
            '    UpdateMCUSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            '    With UpdateMCUSocket
            '        .SendTimeout = 500
            '        .ReceiveTimeout = 500
            '        .Connect(AppSettingHelper.GetInstance.DisplayingScheme.NovaStarSenderItems(CByte(tmpScanBoardSelectForm.Value.Split(",")(0))).IPAddress, 6000)
            '    End With

            Using tmpDialog As New BackgroundWorkDialog With {
                    .Text = MultiLanguageHelper.Lang.GetS("Update program")
                }
                tmpDialog.Start(AddressOf UpdateMCUProgram, tmpScanBoardSelectForm.Value)

                If tmpDialog.Error IsNot Nothing Then
                    MsgBox(tmpDialog.Error.Message,
                           MsgBoxStyle.Information,
                           Button5.Text)
                    'Else
                    '    MsgBox("升级成功",
                    '           MsgBoxStyle.Information,
                    '           Button5.Text)
                End If

            End Using
        End Using
        '    UpdateMCUSocket.Close()

        'Catch ex As Exception

        '    Try
        '        UpdateMCUSocket.Close()
        '    Catch exOfSocket As Exception
        '    End Try

        '    MsgBox(ex.Message,
        '           MsgBoxStyle.Information,
        '           Button5.Text)
        'End Try

    End Sub

#Region "升级单片机程序"
    ''' <summary>
    ''' 升级单片机程序
    ''' </summary>
    Private Sub UpdateMCUProgram(e As BackgroundWorkEventArgs)

        Dim senderID = CByte(e.Args.ToString.Split(",")(0))
        Dim portID = CByte(e.Args.ToString.Split(",")(1))
        Dim scannerID = CShort(e.Args.ToString.Split(",")(2))

        e.Write(MultiLanguageHelper.Lang.GetS("Update program"))

#Region "发送准备升级指令"
        e.Write($"{MultiLanguageHelper.Lang.GetS("Step")} 1 / 3 : {MultiLanguageHelper.Lang.GetS("Switch to upgrade mode")}")

        '读文件信息
        Dim infoReader As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(TextBox3.Text)
        Dim binLength = infoReader.Length

        '发送升级指令
        Dim sendByte As Byte() = Wangk.Hash.Hex2Bin("aadb09030000")
        sendByte(4) = binLength \ 256
        sendByte(5) = binLength Mod 256

        '下发
        SetScanBoardData(senderID, portID, scannerID, sendByte)
        '等待下位机返回数据
        Threading.Thread.Sleep(60)
        '读取
        If Not CheckScanBoardData(senderID, portID, scannerID, Wangk.Hash.Hex2Bin("1a1b"), 2) Then
            '失败
            Throw New Exception(MultiLanguageHelper.Lang.GetS("Switch to upgrade mode failed"))
        End If
#End Region

#Region "发送升级文件"
        e.Write($"{MultiLanguageHelper.Lang.GetS("Step")} 2 / 3 : {MultiLanguageHelper.Lang.GetS("Update program")} ")

        '发送升级程序
        ReDim sendByte(131 - 1)
        Using re As New System.IO.BinaryReader(
            New System.IO.FileStream(
            TextBox3.Text,
            IO.FileMode.Open,
            IO.FileAccess.Read))

            Dim sendID As Integer = 0
            Do
                Dim tmpByte(128 - 1) As Byte
                '从文件读取的字节数
                Dim readByteNum As Integer = re.Read(tmpByte, 0, 128)

                sendByte(0) = sendID

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

                e.Write(CInt(((sendID + 1) * 128 * 100) \ (binLength + 128)))

                '下发
                SetScanBoardData(senderID, portID, scannerID, sendByte)
                '等待下位机返回数据
                Thread.Sleep(60)

#Region "出错则重复发送"
                '读取
                Dim readData() = GetScanBoardData(senderID, portID, scannerID)
                If readData(0) = &H1C AndAlso
                    readData(1) = &H1D Then
                Else

                    '容错10次
                    For trySendID = 0 To 20
                        If trySendID = 20 Then
                            '20次后发送则标记为发送失败
                            Throw New Exception(MultiLanguageHelper.Lang.GetS("Update program failed"))
                        End If

                        'Console.WriteLine($"{sendID} 重复发送 {trySendID + 1}")

                        '下发
                        SetScanBoardData(senderID, portID, scannerID, sendByte)
                        '等待下位机返回数据
                        Thread.Sleep(60)
                        '读取
                        readData = GetScanBoardData(senderID, portID, scannerID)

                        If readData(0) = &H1C AndAlso
                        readData(1) = &H1D Then
                            Exit For
                        End If
                    Next

                End If
#End Region

                sendID += 1

                If sendID * 128 >= binLength Then
                    Exit Do
                End If
            Loop

        End Using
#End Region

#Region "发送固化指令"
        e.Write($"{MultiLanguageHelper.Lang.GetS("Step")} 3 / 3 : {MultiLanguageHelper.Lang.GetS("Save")}")

        For i001 = 0 To 7
            e.Write(i001 * 100 \ 7)

            '下发
            SetScanBoardData(senderID, portID, scannerID, Wangk.Hash.Hex2Bin("aadb0909"))
            Thread.Sleep(200)
        Next
#End Region

        ''等待单片机重启 5s
        'For i001 = 0 To 5 - 1
        '    e.Write(MultiLanguageHelper.Lang.GetS("Waiting for MCU to restart"), i001 * 100 \ 5)
        '    Thread.Sleep(1000)
        'Next

    End Sub
#End Region

#Region "比较收到的数据"
    ''' <summary>
    ''' 比较收到的数据
    ''' </summary>
    ''' <param name="SenderIndex">发送卡序号</param>
    ''' <param name="PortIndex">输出口序号</param>
    ''' <param name="ConnectIndex">在输出口的第几个</param>
    ''' <param name="checkData">比较的数据</param>
    ''' <param name="TimeSec">超时(s)</param>
    Public Function CheckScanBoardData(SenderIndex As Byte,
                                       PortIndex As Byte,
                                       ConnectIndex As Byte,
                                       checkData() As Byte,
                                       TimeSec As Integer) As Boolean
        If checkData.Length > 24 Then
            Throw New Exception("比较数据长度不能超过24字节")
        End If

        For i001 = 0 To TimeSec * 10 - 1
            Thread.Sleep(100)

            Dim readData() = GetScanBoardData(SenderIndex,
                                              PortIndex,
                                              ConnectIndex)

            If IsAllZero(readData, 24) Then
                Return False
            End If

            If Not CheckDataIsSame(readData, checkData) Then
                Continue For
            End If

            Return True
        Next

        Return False
    End Function
#End Region

#Region "是否全0"
    ''' <summary>
    ''' 是否全0
    ''' </summary>
    Public Shared Function IsAllZero(dataArray As Byte(), length As Integer) As Boolean
        For i001 = 0 To length - 1
            If dataArray(i001) <> &H0 Then
                Return False
            End If
        Next

        Return True
    End Function
#End Region

#Region "判断数据是否相同"
    ''' <summary>
    ''' 判断数据是否相同
    ''' </summary>
    Public Shared Function CheckDataIsSame(scanBoardData() As Byte,
                                     checkData() As Byte) As Boolean
        For i001 = 0 To checkData.Length - 1
            If checkData(i001) <> scanBoardData(i001) Then
                Return False
            End If
        Next

        Return True
    End Function

#End Region

#Region "获取指定接收卡数据"
    ''' <summary>
    ''' 获取指定接收卡数据
    ''' </summary>
    ''' <param name="senderIndex"></param>
    ''' <param name="portIndex"></param>
    ''' <param name="scanIndex"></param>
    Public Function GetScanBoardData(senderIndex As Byte,
                                      portIndex As Byte,
                                      scanIndex As UShort) As Byte()

        Dim tmpData(24 - 1) As Byte
        'Dim testTime As New Stopwatch

        Using tmpSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            With tmpSocket
                .SendTimeout = 500
                .ReceiveTimeout = 500
                .Connect(NovaStarSenderItems(senderIndex).IPAddress, 6000)
            End With

            Try
                Dim tmpReceiveBytes(1028 - 1) As Byte
                '上传指令
                tmpSocket.Send(Wangk.Hash.Hex2Bin("55d50902"))
                tmpSocket.Receive(tmpReceiveBytes)

                '上传数据
                tmpSocket.Send(Wangk.Hash.Hex2Bin("55d50905000000000400"))
                For receiveID = 0 To 16 - 1
                    tmpSocket.Receive(tmpReceiveBytes)

                    For packageID = 0 To 32 - 1
                        Dim packageByteID = 4 + packageID * 32

                        '非有效数据跳过
                        If tmpReceiveBytes(packageByteID) <> &H55 Then Continue For
                        '网口号大于3跳过
                        If tmpReceiveBytes(packageByteID + 1) > 4 Then Continue For

                        Dim portID = tmpReceiveBytes(packageByteID + 1)
                        Dim scannerID = tmpReceiveBytes(packageByteID + 2) * 256 + tmpReceiveBytes(packageByteID + 3)
                        If portID <> portIndex Then Continue For
                        If scannerID <> scanIndex Then Continue For

                        For i001 = 0 To 24 - 1
                            tmpData(i001) = tmpReceiveBytes(packageByteID + 4 + i001)
                        Next

                    Next
                Next

#Disable Warning CA1031 ' Do not catch general exception types
            Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
            End Try

        End Using

        Return tmpData

    End Function
#End Region

#End Region

#Region "查询"
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        TreeView1.Nodes.Clear()

        Using tmpDialog As New BackgroundWorkDialog With {
            .Text = MultiLanguageHelper.Lang.GetS("Query program information")
        }

            tmpDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
                                uie.Write(MultiLanguageHelper.Lang.GetS("Query program information"), 0)

                                Dim sacnnerItemsInEntity As List(Of SacnnerAddr) = NovaMarsControl.GetAllScannerStatusByCom()
                                Dim sacnnerItemsInVirtual As New HashSet(Of Integer)
                                For Each item In sacnnerItemsInEntity
                                    With item
                                        sacnnerItemsInVirtual.Add(.SenderIndex * 1000000 + .PortIndex * 100000 + .ScannerIndex * 100)
                                    End With
                                Next

                                Dim tmpRootNode As New TreeNode

                                For senderID = 0 To NovaStarSenderItems.Count - 1

                                    uie.Write(CInt(senderID * 100 \ NovaStarSenderItems.Count))

                                    QueryMCUProgramVersions(senderID, tmpRootNode, sacnnerItemsInVirtual)
                                Next

                                If sacnnerItemsInVirtual.Count <> 0 Then

                                    Dim tmpNode As TreeNode = Nothing
                                    Dim findStr = MultiLanguageHelper.Lang.GetS("Unknown")
                                    For Each item As TreeNode In tmpRootNode.Nodes
                                        If item.Text = findStr Then
                                            tmpNode = item
                                        End If
                                    Next

                                    If tmpNode Is Nothing Then
                                        tmpNode = tmpRootNode.Nodes.Add(findStr)
                                    End If

                                    For Each item In sacnnerItemsInVirtual

                                        Dim senderID = item \ 1000000
                                        Dim portID = (item Mod 1000000) \ 100000
                                        Dim scannerID = (item Mod 100000) \ 100

                                        Dim tmpAddNode As New TreeNode(
                                        MultiLanguageHelper.Lang.GetS("Sender") & senderID.ToString.PadLeft(2) &
                                        $" -{MultiLanguageHelper.Lang.GetS("Port")}{portID.ToString.PadLeft(2) } " &
                                        $" -{MultiLanguageHelper.Lang.GetS("Connect")}{scannerID.ToString.PadLeft(3)}") With {
                                        .Tag = $"{senderID},{portID},{scannerID}",
                                        .ImageIndex = 3,
                                        .SelectedImageIndex = 3
                                        }

                                        tmpNode.Nodes.Add(tmpAddNode)
                                    Next

                                End If

                                uie.Result = tmpRootNode
                            End Sub, Nothing)

            If tmpDialog.Error IsNot Nothing Then
                MsgBox(tmpDialog.Error.Message,
                           MsgBoxStyle.Information,
                           Button6.Text)
            Else
                For Each item In CType(tmpDialog.Result, TreeNode).Nodes
                    TreeView1.Nodes.Add(item)
                Next
            End If

        End Using

        TreeView1.Sort()
        TreeView1.ExpandAll()
    End Sub

#Region "查询单片机版本"
    ''' <summary>
    ''' 查询单片机版本
    ''' </summary>
    Private Sub QueryMCUProgramVersions(senderID As Integer,
                                        rootNode As TreeNode,
                                        sacnnerItemsInVirtual As HashSet(Of Integer))

        NovaMarsControl.SetNewScanBoardData(senderID, &HFF, &HFFFF, Wangk.Hash.Hex2Bin("aadb0901"))

        Threading.Thread.Sleep(200)

        Using tmpSocket As New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            With tmpSocket
                .SendTimeout = 500
                .ReceiveTimeout = 500
                .Connect(NovaStarSenderItems(senderID).IPAddress, 6000)
            End With

            Dim tmpReceiveBytes(1028 - 1) As Byte
            '上传指令
            tmpSocket.Send(Wangk.Hash.Hex2Bin("55d50902"))
            tmpSocket.Receive(tmpReceiveBytes)

            '上传数据
            tmpSocket.Send(Wangk.Hash.Hex2Bin("55d50905000000000400"))
            For receiveID = 0 To 16 - 1
                tmpSocket.Receive(tmpReceiveBytes)

                For packageID = 0 To 32 - 1
                    Dim packageByteID = 4 + packageID * 32

                    '非有效数据跳过
                    If tmpReceiveBytes(packageByteID) <> &H55 Then Continue For
                    '网口号大于3跳过
                    If tmpReceiveBytes(packageByteID + 1) > 4 Then Continue For

                    Dim portID = tmpReceiveBytes(packageByteID + 1)
                    Dim scannerID = tmpReceiveBytes(packageByteID + 2) * 256 + tmpReceiveBytes(packageByteID + 3)

                    Dim tmpAddNode As New TreeNode(
                        MultiLanguageHelper.Lang.GetS("Sender") & (senderID + 1).ToString.PadLeft(2) &
                        $" -{MultiLanguageHelper.Lang.GetS("Port")}{(portID + 1).ToString.PadLeft(2) } " &
                        $" -{MultiLanguageHelper.Lang.GetS("Connect")}{(scannerID + 1).ToString.PadLeft(3)}") With {
                        .Tag = $"{senderID},{portID},{scannerID}",
                        .ImageIndex = 3,
                        .SelectedImageIndex = 3
                    }

                    Dim findStr = MultiLanguageHelper.Lang.GetS("Unknown")

                    If Not IsAllZero({tmpReceiveBytes(packageByteID + 4),
                                     tmpReceiveBytes(packageByteID + 5),
                                     tmpReceiveBytes(packageByteID + 6)}, 3) Then

                        findStr = MultiLanguageHelper.Lang.GetS("Version") &
                        $": {tmpReceiveBytes(packageByteID + 4)}.{tmpReceiveBytes(packageByteID + 5)}.{tmpReceiveBytes(packageByteID + 6)}"

                        With tmpAddNode
                            .ImageIndex = 1
                            .SelectedImageIndex = 1
                        End With

                        sacnnerItemsInVirtual.Remove(senderID * 1000000 + portID * 100000 + scannerID * 100)

                    End If

                    Dim tmpNode As TreeNode = Nothing
                    For Each item As TreeNode In rootNode.Nodes
                        If item.Text = findStr Then
                            tmpNode = item
                        End If
                    Next

                    If tmpNode Is Nothing Then
                        tmpNode = rootNode.Nodes.Add(findStr)
                    End If
                    tmpNode.Nodes.Add(tmpAddNode)

                Next
            Next
        End Using

    End Sub

#End Region

#End Region
#End Region

    ''' <summary>
    ''' 发送数据给接收卡
    ''' </summary>
    Private Function SetScanBoardData(senderIndex As Byte,
                                      portIndex As Byte,
                                      scanIndex As UShort,
                                      data() As Byte) As Boolean

        If AppSettingHelper.GetInstance.OldScanBoardBin Then
            Return NovaMarsControl.SetOldScanBoardData(senderIndex, portIndex, scanIndex, data)
        Else
            Return NovaMarsControl.SetNewScanBoardData(senderIndex, portIndex, scanIndex, data)
        End If

    End Function

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Column3.HeaderText = .GetS("Subnet Mask")
            Me.TabPage1.Text = .GetS("Control")
            Me.GroupBox3.Text = .GetS("Control IP")
            Me.Column2.HeaderText = .GetS("IP Address")
            Me.Column4.HeaderText = .GetS("Gateway")
            Me.Column1.HeaderText = .GetS("Apply Changes")
            Me.TabPage2.Text = .GetS("Sensor")
            Me.GroupBox2.Text = .GetS("Sensor option")
            Me.Label7.Text = .GetS("High")
            Me.Label8.Text = .GetS("Low")
            Me.Label3.Text = .GetS("Reset Time Interval")
            Me.Label1.Text = .GetS("Sensitivity")
            Me.Label2.Text = .GetS("Temp Change Over")
            Me.Button1.Text = .GetS("Apply")
            Me.Button2.Text = .GetS("Apply")
            Me.Button3.Text = .GetS("Apply")
            Me.TabPage3.Text = .GetS("MCU")
            Me.Label5.Text = .GetS("Update Bin file")
            Me.Button4.Text = .GetS("Select")
            Me.Button5.Text = .GetS("Update hardware program")
            Me.Button6.Text = .GetS("Query MCU program information")
            Me.GroupBox1.Text = .GetS("MCU information")
            Me.Label4.Text = .GetS("Scanner versions")
            Me.RadioButton2.Text = .GetS("4.4.0.0 And above")
            Me.RadioButton1.Text = .GetS("Under 4.4.0.0")
            Me.Text = .GetS("HardwareSettingsForm")
        End With
    End Sub
#End Region

End Class