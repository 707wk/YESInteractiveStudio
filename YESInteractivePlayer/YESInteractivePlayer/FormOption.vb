Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports Nova.Mars.SDK

Public Class FormOption
    '''' <summary>
    '''' 最大幕布id
    '''' </summary>
    'Private maxCurtainId As Integer = 0
    '''' <summary>
    '''' 选中幕布id
    '''' </summary>
    'Private selectCurtainId As Integer = 0
    ''' <summary>
    ''' 临时数据表
    ''' </summary>
    Dim tmpDataTable As DataTable

    ''' <summary>
    ''' 加载数据
    ''' </summary>
    Private Sub FormOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '常规
        TextBox1.Text = sysInfo.zoomProportion

        ComboBox1.Items.Add("中文")
        ComboBox1.Items.Add("English")
        ComboBox1.SelectedIndex = sysInfo.selectLanguageId

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '幕布
        For i As Integer = 0 To sysInfo.curtainList.Count - 1
            DataGridView2.Rows.Add(i + 1,
                                   sysInfo.curtainList.Item(i).remark,
                                   $"{sysInfo.curtainList.Item(i).x},{sysInfo.curtainList.Item(i).y}")

            'maxCurtainId = If(maxCurtainId < i + 1, i + 1, maxCurtainId)
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '屏幕
        For i As Integer = 0 To sysInfo.screenList.Length - 1
            If Not sysInfo.screenList(i).existFlage Then
                Continue For
            End If

            DataGridView4.Rows.Add(i,
                                   $"{sysInfo.screenList(i).defaultWidth},{sysInfo.screenList(i).defaultHeight}")
        Next

        '创建临时表
        tmpDataTable = New DataTable("ScanBoardTable")

        '添加字段
        tmpDataTable.Columns.Add("ScreenIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("SenderIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("PortIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("ConnectIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("X", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("Y", System.Type.GetType("System.Int32"))

        '载入数据
        For Each i In sysInfo.ScanBoardTable.Keys
            Dim qwe As ScanBoardInfo = sysInfo.ScanBoardTable.Item(i)

            Dim row As DataRow = tmpDataTable.NewRow()
            row("ScreenIndex") = qwe.ScreenId
            row("SenderIndex") = qwe.SenderId
            row("PortIndex") = qwe.PortId
            row("ConnectIndex") = qwe.ConnectId
            row("X") = qwe.X
            row("Y") = qwe.Y

            tmpDataTable.Rows.Add(row)
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '控制器
        For i As Integer = 0 To sysInfo.senderList.Length - 1
            DataGridView1.Rows.Add(i,
                                   $"{sysInfo.senderList(i).ipDate(3)}.{sysInfo.senderList(i).ipDate(2)}.{sysInfo.senderList(i).ipDate(1)}.{sysInfo.senderList(i).ipDate(0)}",
                                   $"{sysInfo.senderList(i).ipDate(7)}.{sysInfo.senderList(i).ipDate(6)}.{sysInfo.senderList(i).ipDate(5)}.{sysInfo.senderList(i).ipDate(4)}",
                                   $"{sysInfo.senderList(i).ipDate(11)}.{sysInfo.senderList(i).ipDate(10)}.{sysInfo.senderList(i).ipDate(9)}.{sysInfo.senderList(i).ipDate(8)}")
        Next

        For i As Integer = 0 To sysInfo.senderList.Length - 1
            For j As Integer = 0 To 12 - 1
                sysInfo.senderList(i).tmpIpData(j) = sysInfo.senderList(i).ipDate(j)
            Next
        Next

        '绑定设置到ip事件
        AddHandler sysInfo.mainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动性
        '互动模式
        ComboBox2.Items.Add("单个感应")
        ComboBox2.Items.Add("4合1感应")
        'ComboBox2.SelectedIndex = sysInfo.touchMode

        '检测间隔
        NumericUpDown1.Value = sysInfo.inquireTimeSec

        '触摸灵敏度
        NumericUpDown2.Value = sysInfo.touchSensitivity

        '抗干扰等级
        NumericUpDown3.Value = sysInfo.clickValidNums

        '复位温度
        NumericUpDown4.Value = sysInfo.resetTemp
        '复位时间间隔
        NumericUpDown5.Value = sysInfo.resetSec
    End Sub

    ''' <summary>
    ''' 保存数据
    ''' </summary>
    Private Sub FormOption_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '常规
        sysInfo.zoomProportion = Val(TextBox1.Text)

        sysInfo.selectLanguageId = ComboBox1.SelectedIndex

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '幕布
        For i As Integer = 0 To sysInfo.curtainList.Count - 1
            '刷新屏幕记录的幕布位置
            For Each j In sysInfo.curtainList.Item(i).screenList
                sysInfo.screenList(j).curtainListId = i
            Next

            '计算幕布面积
            Dim tmp As curtainInfo = sysInfo.curtainList.Item(i)
            tmp.defaultHeight = 0
            tmp.defaultWidth = 0

            For Each j In sysInfo.curtainList.Item(i).screenList
                '最大高度
                tmp.defaultHeight =
                If(tmp.defaultHeight < sysInfo.screenList(j).defaultY + sysInfo.screenList(j).defaultHeight,
                sysInfo.screenList(j).defaultY + sysInfo.screenList(j).defaultHeight,
                tmp.defaultHeight)

                '最大宽度
                tmp.defaultWidth =
                If(tmp.defaultWidth < sysInfo.screenList(j).defaultX + sysInfo.screenList(j).defaultWidth,
                sysInfo.screenList(j).defaultX + sysInfo.screenList(j).defaultWidth,
                tmp.defaultWidth)
            Next

            '计算缩放后位置及大小
            With tmp
                .x = .defaultX '/ sysInfo.zoomProportion
                .y = .defaultY '/ sysInfo.zoomProportion
                .height = .defaultHeight / sysInfo.zoomProportion
                .width = .defaultWidth / sysInfo.zoomProportion
            End With

            sysInfo.curtainList.Item(i) = tmp
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '屏幕
        '计算缩放后位置及大小
        For i As Integer = 0 To sysInfo.screenList.Length - 1
            With sysInfo.screenList(i)
                .x = .defaultX / sysInfo.zoomProportion
                .y = .defaultY / sysInfo.zoomProportion
                .height = .defaultHeight / sysInfo.zoomProportion
                .width = .defaultWidth / sysInfo.zoomProportion
                .ScanBoardHeight = .defaultScanBoardHeight / sysInfo.zoomProportion
                .ScanBoardWidth = .defaultScanBoardWidth / sysInfo.zoomProportion
                .touchPieceHeight = .defaultScanBoardHeight / .touchPieceRowsNum / sysInfo.zoomProportion
                .touchPieceWidth = .defaultScanBoardWidth / .touchPieceColumnsNum / sysInfo.zoomProportion
            End With
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '控制器
        '移除事件
        RemoveHandler sysInfo.mainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动性
        '互动模式
        'sysInfo.touchMode = ComboBox2.SelectedIndex

        '检测间隔
        sysInfo.inquireTimeSec = NumericUpDown1.Value

        '抗干扰等级
        sysInfo.clickValidNums = NumericUpDown3.Value

        '复位温度
        sysInfo.resetTemp = NumericUpDown4.Value
        '复位时间间隔
        sysInfo.resetSec = NumericUpDown5.Value

    End Sub

    ''' <summary>
    ''' 右键选中
    ''' </summary>
    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseDown
        Select Case True
            Case e.Button <> System.Windows.Forms.MouseButtons.Right
            Case e.RowIndex < 0 Or e.ColumnIndex < 0
            Case DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False
                DataGridView1.ClearSelection()
                DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
        End Select
    End Sub

    ''' <summary>
    ''' 右键选中
    ''' </summary>
    Private Sub DataGridView2_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView2.CellMouseDown
        Select Case True
            Case e.Button <> System.Windows.Forms.MouseButtons.Right
            Case e.RowIndex < 0 Or e.ColumnIndex < 0
            Case DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False
                DataGridView2.ClearSelection()
                DataGridView2.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
        End Select
    End Sub

    ''' <summary>
    ''' 右键选中
    ''' </summary>
    Private Sub DataGridView3_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView3.CellMouseDown
        Select Case True
            Case e.Button <> System.Windows.Forms.MouseButtons.Right
            Case e.RowIndex < 0 Or e.ColumnIndex < 0
            Case DataGridView3.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False
                DataGridView3.ClearSelection()
                DataGridView3.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
        End Select
    End Sub

    ''' <summary>
    ''' 更新显示IP信息
    ''' </summary>
    Private Sub updataIp(i As Integer)
        DataGridView1.Rows(i).Cells(1).Value =
            $"{sysInfo.senderList(i).tmpIpData(3)}.{sysInfo.senderList(i).tmpIpData(2)}.{sysInfo.senderList(i).tmpIpData(1)}.{sysInfo.senderList(i).tmpIpData(0)}"
        DataGridView1.Rows(i).Cells(2).Value =
            $"{sysInfo.senderList(i).tmpIpData(7)}.{sysInfo.senderList(i).tmpIpData(6)}.{sysInfo.senderList(i).tmpIpData(5)}.{sysInfo.senderList(i).tmpIpData(4)}"
        DataGridView1.Rows(i).Cells(3).Value =
            $"{sysInfo.senderList(i).tmpIpData(11)}.{sysInfo.senderList(i).tmpIpData(10)}.{sysInfo.senderList(i).tmpIpData(9)}.{sysInfo.senderList(i).tmpIpData(8)}"
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Dim setIpStr As String = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

        '切割ip字符串
        Dim ipDataStrArr() As String
        ipDataStrArr = Split(setIpStr, ".")

        '判断长度
        If ipDataStrArr.Length <> 4 Then
            MsgBox($"非法参数", MsgBoxStyle.Information, Me.Text)
            updataIp(e.RowIndex)
            Exit Sub
        End If

        '判断数值
        For i As Integer = 0 To 4 - 1
            Dim reg As New Regex("\d+")
            Dim m As Match = reg.Match(ipDataStrArr(i))

            If m.Success Then
            Else
                MsgBox($"非法参数", MsgBoxStyle.Information, Me.Text)
                updataIp(e.RowIndex)
                Exit Sub
            End If

            Dim tmpNum As Integer = CInt(m.Value)

            If tmpNum > 255 Then
                MsgBox($"非法参数", MsgBoxStyle.Information, Me.Text)
                updataIp(e.RowIndex)
                Exit Sub
            End If
        Next

        '保存ip信息
        Select Case e.ColumnIndex
            Case 1
                'ip
                sysInfo.senderList(e.RowIndex).tmpIpData(3) = CInt(ipDataStrArr(0))
                sysInfo.senderList(e.RowIndex).tmpIpData(2) = CInt(ipDataStrArr(1))
                sysInfo.senderList(e.RowIndex).tmpIpData(1) = CInt(ipDataStrArr(2))
                sysInfo.senderList(e.RowIndex).tmpIpData(0) = CInt(ipDataStrArr(3))
            Case 2
                '子网掩码
                sysInfo.senderList(e.RowIndex).tmpIpData(7) = CInt(ipDataStrArr(0))
                sysInfo.senderList(e.RowIndex).tmpIpData(6) = CInt(ipDataStrArr(1))
                sysInfo.senderList(e.RowIndex).tmpIpData(5) = CInt(ipDataStrArr(2))
                sysInfo.senderList(e.RowIndex).tmpIpData(4) = CInt(ipDataStrArr(3))
            Case 3
                '网关
                sysInfo.senderList(e.RowIndex).tmpIpData(11) = CInt(ipDataStrArr(0))
                sysInfo.senderList(e.RowIndex).tmpIpData(10) = CInt(ipDataStrArr(1))
                sysInfo.senderList(e.RowIndex).tmpIpData(9) = CInt(ipDataStrArr(2))
                sysInfo.senderList(e.RowIndex).tmpIpData(8) = CInt(ipDataStrArr(3))
        End Select
    End Sub

    ''' <summary>
    ''' 设置ip通知
    ''' </summary>
    Dim senderArrayIndex As Integer = 0
    Private Sub SendEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        'Static Dim senderArrayIndex As Integer = 0
        If e.IsExecResult Then
            If senderArrayIndex < sysInfo.senderList.Length - 1 Then
                sysInfo.senderList(senderArrayIndex).ipDate = sysInfo.senderList(senderArrayIndex).tmpIpData

                senderArrayIndex += 1

                sysInfo.mainClass.SetEquipmentIP(senderArrayIndex, sysInfo.senderList(senderArrayIndex).tmpIpData)
            Else
                sysInfo.senderList(senderArrayIndex).ipDate = sysInfo.senderList(senderArrayIndex).tmpIpData
                MsgBox($"控制器ip设置成功!")
            End If
        Else
            MsgBox($"控制器{senderArrayIndex} 设置IP数据失败!请检查设备后重新设置!")
        End If
    End Sub

    ''' <summary>
    ''' 保存控制器ip
    ''' </summary>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        senderArrayIndex = 0
        sysInfo.mainClass.SetEquipmentIP(0, sysInfo.senderList(senderArrayIndex).tmpIpData)
    End Sub

    '''' <summary>
    '''' 设置复位温度变化幅度
    '''' </summary>
    'Private Sub Button1_Click(sender As Object, e As EventArgs)
    '    sysInfo.resetTemp = NumericUpDown4.Value
    '    'sysInfo.resetMin = 0
    'End Sub

    '''' <summary>
    '''' 设置复位时间间隔 min
    '''' </summary>
    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    'sysInfo.resetTemp = 0
    '    sysInfo.resetSec = NumericUpDown5.Value
    'End Sub

    ''' <summary>
    ''' 显示选中幕布信息
    ''' </summary>
    Private Sub DataGridView2_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView2.CellMouseClick
        Dim tmp As curtainInfo = sysInfo.curtainList.Item(DataGridView2.SelectedCells(0).RowIndex)

        Label11.Text = DataGridView2.SelectedCells(0).RowIndex + 1
        TextBox2.Text = tmp.remark
        NumericUpDown6.Value = tmp.defaultX
        NumericUpDown7.Value = tmp.defaultY

        DataGridView3.Rows.Clear()
        For Each i In tmp.screenList
            DataGridView3.Rows.Add(
                $"{i}",
                $"{sysInfo.screenList(i).defaultX}",
                $"{sysInfo.screenList(i).defaultY}",
                $"{sysInfo.screenList(i).touchPieceRowsNum}",
                $"{sysInfo.screenList(i).touchPieceColumnsNum}")
        Next
    End Sub

    ''' <summary>
    ''' 新增幕布
    ''' </summary>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Dim tmp As New curtainInfo

        'tmp.id = maxCurtainId + 1
        'maxCurtainId += 1
        tmp.remark = TextBox2.Text
        tmp.defaultY = NumericUpDown6.Value
        tmp.defaultY = NumericUpDown7.Value

        tmp.screenList = New List(Of Integer)
        For i As Integer = 0 To DataGridView3.Rows.Count - 1 - 1
            With DataGridView3.Rows(i)
                tmp.screenList.Add(.Cells(0).Value)

                sysInfo.screenList(.Cells(0).Value).defaultX = .Cells(1).Value
                sysInfo.screenList(.Cells(0).Value).defaultY = .Cells(2).Value
                'sysInfo.screenList(.Cells(0).Value).x = .Cells(1).Value / sysInfo.zoomProportion
                'sysInfo.screenList(.Cells(0).Value).y = .Cells(2).Value / sysInfo.zoomProportion
                sysInfo.screenList(.Cells(0).Value).touchPieceRowsNum = .Cells(3).Value
                sysInfo.screenList(.Cells(0).Value).touchPieceColumnsNum = .Cells(4).Value
            End With
        Next
        'For Each i As DataGridViewRow In DataGridView3.Rows

        'Next

        sysInfo.curtainList.Add(tmp)
        DataGridView2.Rows.Add(DataGridView2.Rows.Count + 1,
                                   tmp.remark,
                                   $"{tmp.defaultX},{tmp.defaultY}")
    End Sub

    ''' <summary>
    ''' 删除幕布
    ''' </summary>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If Label11.Text = "NULL" Then
            Exit Sub
        End If

        '删除时先关闭窗体
        sysInfo.curtainList.Item(DataGridView2.SelectedCells(0).RowIndex).playDialog.closeDialog(True)

        sysInfo.curtainList.RemoveAt(DataGridView2.SelectedCells(0).RowIndex)
        DataGridView2.Rows.RemoveAt(DataGridView2.SelectedCells(0).RowIndex)
        Label11.Text = "NULL"
    End Sub

    ''' <summary>
    ''' 保存幕布信息修改
    ''' </summary>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Label11.Text = "NULL" Then
            Exit Sub
        End If

        Dim tmp As curtainInfo = sysInfo.curtainList.Item(DataGridView2.SelectedCells(0).RowIndex)
        'tmp.id = maxCurtainId + 1
        'maxCurtainId += 1
        tmp.remark = TextBox2.Text
        tmp.defaultX = NumericUpDown6.Value
        tmp.defaultY = NumericUpDown7.Value
        'tmp.x = tmp.defaultX '/ sysInfo.zoomProportion
        'tmp.y = tmp.defaultY '/ sysInfo.zoomProportion
        'tmp.playDialog

        tmp.screenList = New List(Of Integer)
        For i As Integer = 0 To DataGridView3.Rows.Count - 1 - 1
            With DataGridView3.Rows(i)
                tmp.screenList.Add(.Cells(0).Value)

                sysInfo.screenList(.Cells(0).Value).defaultX = .Cells(1).Value
                sysInfo.screenList(.Cells(0).Value).defaultY = .Cells(2).Value
                'sysInfo.screenList(.Cells(0).Value).x = .Cells(1).Value / sysInfo.zoomProportion
                'sysInfo.screenList(.Cells(0).Value).y = .Cells(2).Value / sysInfo.zoomProportion
                sysInfo.screenList(.Cells(0).Value).touchPieceRowsNum = .Cells(3).Value
                sysInfo.screenList(.Cells(0).Value).touchPieceColumnsNum = .Cells(4).Value
            End With
        Next


        sysInfo.curtainList.Item(DataGridView2.SelectedCells(0).RowIndex) = tmp
        'DataGridView2.Rows(DataGridView2.SelectedCells(0).RowIndex).Cells(0).Value = DataGridView2.Rows.Count + 1
        DataGridView2.Rows(DataGridView2.SelectedCells(0).RowIndex).Cells(1).Value = tmp.remark
        DataGridView2.Rows(DataGridView2.SelectedCells(0).RowIndex).Cells(2).Value = $"{tmp.defaultX},{tmp.defaultY}"
    End Sub

    ''' <summary>
    ''' 修改屏幕灵敏度
    ''' </summary>
    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        Dim sendstr As String = "aadb0305"
        Dim sendByte(sendstr.Length \ 2 - 1) As Byte

        For i As Integer = 0 To sendstr.Length \ 2 - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte(3) = NumericUpDown2.Value
        If sysInfo.mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte) Then
            '触摸灵敏度
            sysInfo.touchSensitivity = NumericUpDown2.Value
        Else
            MsgBox($"指令发送失败",
                   MsgBoxStyle.Information,
                   $"触摸灵敏度")
        End If

    End Sub

    ''' <summary>
    ''' 显示点击屏幕走线
    ''' </summary>
    Private Sub DataGridView4_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView4.CellMouseClick
        Dim selectScreenId As Integer = DataGridView4.SelectedCells(0).RowIndex
        GroupBox13.Text = $"屏幕{selectScreenId} 走线"

        Me.PictureBox1.Update()
        Dim g As Graphics = Me.PictureBox1.CreateGraphics
        Dim mpen As New Pen(Color.Green)
        Dim tmpHeight As Integer =
            sysInfo.screenList(selectScreenId).defaultHeight * 21 \ sysInfo.screenList(selectScreenId).defaultScanBoardHeight
        Dim tmpWidth As Integer =
            sysInfo.screenList(selectScreenId).defaultWidth * 21 \ sysInfo.screenList(selectScreenId).defaultScanBoardWidth

        For i As Integer = 0 To tmpHeight Step 21
            g.DrawLine(mpen, 0, i, tmpWidth, i)
        Next

        For i As Integer = 0 To tmpWidth Step 21
            g.DrawLine(mpen, i, 0, i, tmpHeight)
        Next

        Dim dataRow() As DataRow = tmpDataTable.Select($"ScreenIndex={selectScreenId}", "SenderIndex ASC,PortIndex ASC,ConnectIndex ASC")

        Dim lastSenderIndex As Integer = -1
        Dim lastPortIndex As Integer = -1
        Dim lastX As Integer = 0
        Dim lastY As Integer = 0

        mpen.Color = Color.Red
        mpen.Width = 3
        For i As Integer = 0 To dataRow.Length - 1
            Dim tmpX As Integer = (dataRow(i).Item(4) \ 4) * 21 + 10
            Dim tmpY As Integer = (dataRow(i).Item(5) \ 4) * 21 + 10

            If dataRow(i).Item(1) = lastSenderIndex And dataRow(i).Item(2) = lastPortIndex Then
                g.DrawLine(mpen, lastX, lastY, tmpX, tmpY)

                lastX = tmpX
                lastY = tmpY
            Else
                g.FillEllipse(New SolidBrush(Color.Green), tmpX - 5, tmpY - 5, 10, 10)
                lastSenderIndex = dataRow(i).Item(1)
                lastPortIndex = dataRow(i).Item(2)
                lastX = tmpX
                lastY = tmpY
            End If
        Next
    End Sub
End Class