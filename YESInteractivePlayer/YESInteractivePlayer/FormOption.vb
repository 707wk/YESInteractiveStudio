Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Text.RegularExpressions
Imports System.Threading
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
    Private tmpDataTable As DataTable

    ''' <summary>
    ''' 加载数据
    ''' </summary>
    Private Sub FormOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '常规
        NumericUpDown8.Value = sysInfo.ZoomTmpNumerator
        NumericUpDown9.Value = sysInfo.ZoomTmpDenominator
        TextBox1.Text = sysInfo.ZoomProportion

        ComboBox1.Items.Add("中文")
        ComboBox1.Items.Add("English")
        ComboBox1.SelectedIndex = sysInfo.SelectLanguageId

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '幕布
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            With sysInfo.CurtainList.Item(i)
                DataGridView2.Rows.Add(i + 1,
                                  .Remark,
                                   $"{ .X},{ .Y} [{ .Width},{ .Height}]")
            End With
        Next

        '初始化屏幕下拉列表
        ComboBox3.DropDownStyle = ComboBoxStyle.DropDownList
        ComboBox3.Visible = False
        For i As Integer = 0 To sysInfo.ScreenList.Length - 1
            If Not sysInfo.ScreenList(i).ExistFlage Then
                Continue For
            End If
            ComboBox3.Items.Add($"{i}")
        Next
        DataGridView3.Controls.Add(ComboBox3)

        DataGridView3.ContextMenuStrip = ContextMenuStrip1

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '屏幕
        For i As Integer = 0 To sysInfo.ScreenList.Length - 1
            If Not sysInfo.ScreenList(i).ExistFlage Then
                Continue For
            End If

            DataGridView4.Rows.Add(i,
                                   $"{sysInfo.ScreenList(i).DefaultWidth},{sysInfo.ScreenList(i).DefaultHeight}")
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
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            DataGridView1.Rows.Add(i,
                                   $"{sysInfo.SenderList(i).IpDate(3)}.{sysInfo.SenderList(i).IpDate(2)}.{sysInfo.SenderList(i).IpDate(1)}.{sysInfo.SenderList(i).IpDate(0)}",
                                   $"{sysInfo.SenderList(i).IpDate(7)}.{sysInfo.SenderList(i).IpDate(6)}.{sysInfo.SenderList(i).IpDate(5)}.{sysInfo.SenderList(i).IpDate(4)}",
                                   $"{sysInfo.SenderList(i).IpDate(11)}.{sysInfo.SenderList(i).IpDate(10)}.{sysInfo.SenderList(i).IpDate(9)}.{sysInfo.SenderList(i).IpDate(8)}")
        Next

        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            For j As Integer = 0 To 12 - 1
                sysInfo.SenderList(i).TmpIpData(j) = sysInfo.SenderList(i).IpDate(j)
            Next
        Next

        '绑定设置到ip事件
        AddHandler sysInfo.MainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动性
        '互动模式
        ComboBox2.Items.Add(sysInfo.Language.GetLanguage("单个感应"))
        ComboBox2.Items.Add(sysInfo.Language.GetLanguage("4合1感应"))
        'ComboBox2.SelectedIndex = sysInfo.touchMode

        '检测间隔
        NumericUpDown1.Value = sysInfo.InquireTimeSec

        '触摸灵敏度
        NumericUpDown2.Value = sysInfo.TouchSensitivity

        '抗干扰等级
        NumericUpDown3.Value = sysInfo.ClickValidNums

        '复位温度
        NumericUpDown4.Value = sysInfo.ResetTemp
        '复位时间间隔
        NumericUpDown5.Value = sysInfo.ResetSec

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '接收卡
        '设置接收卡列表样式
        ListView1.View = View.Details
        ListView1.GridLines = True
        ListView1.FullRowSelect = True
        ListView1.CheckBoxes = False
        ListView1.ShowItemToolTips = True
        ListView1.Clear()
        ListView1.Columns.Add(sysInfo.Language.GetLanguage("控制器号"), 60, HorizontalAlignment.Left)
        ListView1.Columns.Add(sysInfo.Language.GetLanguage("网口号"), 50, HorizontalAlignment.Left)
        ListView1.Columns.Add(sysInfo.Language.GetLanguage("接收卡号"), 60, HorizontalAlignment.Left)
        ListView1.Columns.Add(sysInfo.Language.GetLanguage("值"), 60, HorizontalAlignment.Left)

        '设置显示语言
        sysInfo.Language.SetControlslanguage(Me)
    End Sub

    ''' <summary>
    ''' 保存数据
    ''' </summary>
    Private Sub FormOption_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '常规
        'If Val(TextBox1.Text) Then
        'sysInfo.zoomProportion = Val(TextBox1.Text)
        'End If

        sysInfo.SelectLanguageId = ComboBox1.SelectedIndex

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '幕布
        For i As Integer = 0 To sysInfo.CurtainList.Count - 1
            '刷新屏幕记录的幕布位置
            For Each j In sysInfo.CurtainList.Item(i).ScreenList
                sysInfo.ScreenList(j).CurtainListId = i
            Next

            '计算幕布面积
            Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(i)
            tmp.DefaultHeight = 0
            tmp.DefaultWidth = 0

            For Each j In sysInfo.CurtainList.Item(i).ScreenList
                '最大高度
                tmp.DefaultHeight =
                If(tmp.DefaultHeight < sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                tmp.DefaultHeight)

                '最大宽度
                tmp.DefaultWidth =
                If(tmp.DefaultWidth < sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                tmp.DefaultWidth)
            Next

            '计算缩放后位置及大小
            With tmp
                .X = .DefaultX '/ sysInfo.zoomProportion
                .Y = .DefaultY '/ sysInfo.zoomProportion
                .Height = .DefaultHeight / sysInfo.ZoomProportion
                .Width = .DefaultWidth / sysInfo.ZoomProportion
            End With

            sysInfo.CurtainList.Item(i) = tmp
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '屏幕
        '计算缩放后位置及大小
        For i As Integer = 0 To sysInfo.ScreenList.Length - 1
            With sysInfo.ScreenList(i)
                .X = .DefaultX / sysInfo.ZoomProportion
                .Y = .DefaultY / sysInfo.ZoomProportion
                .Height = .DefaultHeight / sysInfo.ZoomProportion
                .Width = .DefaultWidth / sysInfo.ZoomProportion
                .ScanBoardHeight = .DefaultScanBoardHeight / sysInfo.ZoomProportion
                .ScanBoardWidth = .DefaultScanBoardWidth / sysInfo.ZoomProportion
                .TouchPieceHeight = .DefaultScanBoardHeight / .TouchPieceRowsNum / sysInfo.ZoomProportion
                .TouchPieceWidth = .DefaultScanBoardWidth / .TouchPieceColumnsNum / sysInfo.ZoomProportion
            End With
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '控制器
        '移除事件
        RemoveHandler sysInfo.MainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '互动性
        '互动模式
        'sysInfo.touchMode = ComboBox2.SelectedIndex

        ''检测间隔
        'sysInfo.InquireTimeSec = NumericUpDown1.Value

        '抗干扰等级
        sysInfo.ClickValidNums = NumericUpDown3.Value

        '复位温度
        sysInfo.ResetTemp = NumericUpDown4.Value
        '复位时间间隔
        sysInfo.ResetSec = NumericUpDown5.Value

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '接收卡
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
    ''' 右键选中
    ''' </summary>
    Private Sub DataGridView4_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView4.CellMouseDown
        Select Case True
            Case e.Button <> System.Windows.Forms.MouseButtons.Right
            Case e.RowIndex < 0 Or e.ColumnIndex < 0
            Case DataGridView4.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False
                DataGridView4.ClearSelection()
                DataGridView4.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
        End Select
    End Sub

    ''' <summary>
    ''' 点击第一行添加屏幕
    ''' </summary>
    Private Sub DataGridView3_CurrentCellChanged(sender As Object, e As EventArgs) Handles DataGridView3.CurrentCellChanged
        If DataGridView3.CurrentCell Is Nothing Then
            Exit Sub
        End If

        Dim columnIndex As Integer = DataGridView3.CurrentCell.ColumnIndex
        Dim rowIndex As Integer = DataGridView3.CurrentCell.RowIndex
        If rowIndex = DataGridView3.Rows.Count - 1 Then
            '最后一行只读
            DataGridView3.Rows(rowIndex).Cells(columnIndex).ReadOnly = True
        ElseIf columnIndex <> 0 Then
            DataGridView3.Rows(rowIndex).Cells(columnIndex).ReadOnly = False
        End If

        ComboBox3.Visible = False
        If columnIndex = 0 Then
            '点击第一列单元格则显示下拉列表
            Dim rect As Rectangle = DataGridView3.GetCellDisplayRectangle(columnIndex, rowIndex, False)
            ComboBox3.Left = rect.Left
            ComboBox3.Top = rect.Top
            ComboBox3.Width = rect.Width
            ComboBox3.Height = rect.Height
            '将单元格的内容显示为下拉列表的当前项
            ComboBox3.SelectedIndex = ComboBox3.Items.IndexOf($"{DataGridView3.Rows(rowIndex).Cells(columnIndex).Value}")
            ComboBox3.Visible = True
        End If
    End Sub

    ''' <summary>
    ''' 选择屏幕ID
    ''' </summary>
    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        If ComboBox3.SelectedIndex = -1 Then
            Exit Sub
        End If

        '检测重复
        For Each i As DataGridViewRow In DataGridView3.Rows
            If i.Cells(0).Value = ComboBox3.Items(ComboBox3.SelectedIndex) Then
                Exit Sub
            End If
        Next

        If DataGridView3.Rows(DataGridView3.CurrentCell.RowIndex).Cells(0).Value Is Nothing Then
            '新增
            Dim row As DataGridViewRow = New DataGridViewRow
            Dim textboxcell As DataGridViewTextBoxCell = New DataGridViewTextBoxCell
            textboxcell.Value = ComboBox3.Items(ComboBox3.SelectedIndex)
            row.Cells.Add(textboxcell)
            DataGridView3.Rows.Add(row)
        Else
            '修改
            DataGridView3.Rows(DataGridView3.CurrentCell.RowIndex).Cells(0).Value = ComboBox3.Items(ComboBox3.SelectedIndex)
        End If
    End Sub

    ''' <summary>
    ''' 更新输入
    ''' </summary>
    Private Sub DataGridView3_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellEndEdit
        Dim editValue As Integer

        Dim reg As New Regex("\d+")
        Dim m As Match

        m = reg.Match($"{DataGridView3.Rows(e.RowIndex).Cells(e.ColumnIndex).Value}")
        If m.Success Then
            editValue = CInt(m.Value)
        End If

        Select Case e.ColumnIndex
            Case 0
            Case 1
            Case 2
            Case 3
                editValue = If(editValue, editValue, 4)
            Case 4
                editValue = If(editValue, editValue, 4)
        End Select

        DataGridView3.Rows(e.RowIndex).Cells(e.ColumnIndex).Value = editValue
    End Sub

    ''' <summary>
    ''' 更新显示IP信息
    ''' </summary>
    Private Sub UpdataIp(i As Integer)
        DataGridView1.Rows(i).Cells(1).Value =
            $"{sysInfo.SenderList(i).TmpIpData(3)}.{sysInfo.SenderList(i).TmpIpData(2)}.{sysInfo.SenderList(i).TmpIpData(1)}.{sysInfo.SenderList(i).TmpIpData(0)}"
        DataGridView1.Rows(i).Cells(2).Value =
            $"{sysInfo.SenderList(i).TmpIpData(7)}.{sysInfo.SenderList(i).TmpIpData(6)}.{sysInfo.SenderList(i).TmpIpData(5)}.{sysInfo.SenderList(i).TmpIpData(4)}"
        DataGridView1.Rows(i).Cells(3).Value =
            $"{sysInfo.SenderList(i).TmpIpData(11)}.{sysInfo.SenderList(i).TmpIpData(10)}.{sysInfo.SenderList(i).TmpIpData(9)}.{sysInfo.SenderList(i).TmpIpData(8)}"
    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Dim setIpStr As String = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

        '切割ip字符串
        Dim ipDataStrArr() As String
        ipDataStrArr = Split(setIpStr, ".")

        '判断长度
        If ipDataStrArr.Length <> 4 Then
            MsgBox($"非法参数",
                   MsgBoxStyle.Information,
                   Me.Text)
            UpdataIp(e.RowIndex)
            Exit Sub
        End If

        '判断数值
        For i As Integer = 0 To 4 - 1
            If Not IsNumeric(ipDataStrArr(i)) Then
                MsgBox($"非法参数",
                       MsgBoxStyle.Information,
                       Me.Text)
                UpdataIp(e.RowIndex)
                Exit Sub
            End If

            Dim reg As New Regex("\d+")
            Dim m As Match = reg.Match(ipDataStrArr(i))

            If m.Success Then
            Else
                MsgBox($"非法参数",
                       MsgBoxStyle.Information,
                       Me.Text)
                UpdataIp(e.RowIndex)
                Exit Sub
            End If

            Dim tmpNum As Integer = CInt(m.Value)

            If tmpNum > 255 Then
                MsgBox($"非法参数",
                       MsgBoxStyle.Information,
                       Me.Text)
                UpdataIp(e.RowIndex)
                Exit Sub
            End If
        Next

        '保存ip信息
        Select Case e.ColumnIndex
            Case 1
                'ip
                sysInfo.SenderList(e.RowIndex).TmpIpData(3) = CInt(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(2) = CInt(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(1) = CInt(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(0) = CInt(ipDataStrArr(3))
            Case 2
                '子网掩码
                sysInfo.SenderList(e.RowIndex).TmpIpData(7) = CInt(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(6) = CInt(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(5) = CInt(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(4) = CInt(ipDataStrArr(3))
            Case 3
                '网关
                sysInfo.SenderList(e.RowIndex).TmpIpData(11) = CInt(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(10) = CInt(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(9) = CInt(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(8) = CInt(ipDataStrArr(3))
        End Select
    End Sub

    ''' <summary>
    ''' 设置ip通知
    ''' </summary>
    Dim senderArrayIndex As Integer = 0
    Private Sub SendEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        'Static Dim senderArrayIndex As Integer = 0
        If e.IsExecResult Then
            If senderArrayIndex < sysInfo.SenderList.Length - 1 Then
                sysInfo.SenderList(senderArrayIndex).IpDate = sysInfo.SenderList(senderArrayIndex).TmpIpData

                senderArrayIndex += 1

                sysInfo.MainClass.SetEquipmentIP(senderArrayIndex, sysInfo.SenderList(senderArrayIndex).TmpIpData)
            Else
                sysInfo.SenderList(senderArrayIndex).IpDate = sysInfo.SenderList(senderArrayIndex).TmpIpData
                MsgBox($"控制器ip设置成功!",
                       MsgBoxStyle.Information,
                       Me.Text)
            End If
        Else
            MsgBox($"控制器{senderArrayIndex} 设置IP数据失败!请检查设备后重新设置!",
                   MsgBoxStyle.Information,
                   Me.Text)
        End If
    End Sub

    ''' <summary>
    ''' 保存控制器ip
    ''' </summary>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        senderArrayIndex = 0
        sysInfo.MainClass.SetEquipmentIP(0, sysInfo.SenderList(senderArrayIndex).TmpIpData)
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
        Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(DataGridView2.SelectedCells(0).RowIndex)

        Label11.Text = DataGridView2.SelectedCells(0).RowIndex + 1
        TextBox2.Text = tmp.Remark
        NumericUpDown6.Value = tmp.DefaultX
        NumericUpDown7.Value = tmp.DefaultY

        DataGridView3.Rows.Clear()
        For Each i In tmp.ScreenList
            DataGridView3.Rows.Add(
                $"{i}",
                $"{sysInfo.ScreenList(i).DefaultX}",
                $"{sysInfo.ScreenList(i).DefaultY}",
                $"{sysInfo.ScreenList(i).TouchPieceRowsNum}",
                $"{sysInfo.ScreenList(i).TouchPieceColumnsNum}")
        Next
    End Sub

    ''' <summary>
    ''' 新增幕布
    ''' </summary>
    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        '判断幕布数是否超过屏幕数
        Dim sumScreenNum As Integer = 0
        For Each i As ScreenInfo In sysInfo.ScreenList
            sumScreenNum += If(i.ExistFlage, 1, 0)
        Next
        If sysInfo.CurtainList.Count = sumScreenNum Then
            MsgBox($"幕布数已达最大值",
                   MsgBoxStyle.Information,
                   $"新增幕布")
            Exit Sub
        End If

        Label11.Text = DataGridView2.Rows.Count + 1

        Dim tmp As New CurtainInfo

        'tmp.id = maxCurtainId + 1
        'maxCurtainId += 1
        tmp.Remark = TextBox2.Text
        tmp.DefaultY = NumericUpDown6.Value
        tmp.DefaultY = NumericUpDown7.Value

        tmp.ScreenList = New List(Of Integer)
        For i As Integer = 0 To DataGridView3.Rows.Count - 1 - 1
            Try
                With DataGridView3.Rows(i)
                    tmp.ScreenList.Add(.Cells(0).Value)

                    sysInfo.ScreenList(.Cells(0).Value).DefaultX = .Cells(1).Value
                    sysInfo.ScreenList(.Cells(0).Value).DefaultY = .Cells(2).Value
                    sysInfo.ScreenList(.Cells(0).Value).TouchPieceRowsNum = .Cells(3).Value
                    sysInfo.ScreenList(.Cells(0).Value).TouchPieceColumnsNum = .Cells(4).Value
                End With

            Catch ex As Exception
                MsgBox($"第{i + 1}行数据错误:{ex.Message}", MsgBoxStyle.Information, "新增")
                Exit Sub
            End Try

        Next

        sysInfo.CurtainList.Add(tmp)

        Dim maxHeight = 0
        Dim maxWidth = 0

        For Each j In tmp.ScreenList
            '最大高度
            maxHeight =
                If(maxHeight < sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                maxHeight)

            '最大宽度
            maxWidth =
                If(maxWidth < sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                maxWidth)
        Next
        maxHeight = maxHeight / sysInfo.ZoomProportion
        maxWidth = maxWidth / sysInfo.ZoomProportion

        DataGridView2.Rows.Add(DataGridView2.Rows.Count + 1,
                                   tmp.Remark,
                                   $"{tmp.DefaultX},{tmp.DefaultY} [{ maxWidth},{ maxHeight}]")
    End Sub

    ''' <summary>
    ''' 删除幕布
    ''' </summary>
    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If Label11.Text = "NULL" Then
            Exit Sub
        End If

        '删除时先关闭窗体
        Try
            sysInfo.CurtainList.Item(CInt(Label11.Text) - 1).PlayDialog.CloseDialog(True)
        Catch ex As Exception
        End Try

        sysInfo.CurtainList.RemoveAt(CInt(Label11.Text) - 1)
        DataGridView2.Rows.RemoveAt(CInt(Label11.Text) - 1)
        Label11.Text = "NULL"
    End Sub

    ''' <summary>
    ''' 保存幕布信息修改
    ''' </summary>
    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If Label11.Text = "NULL" Then
            Exit Sub
        End If

        Dim tmp As CurtainInfo = sysInfo.CurtainList.Item(CInt(Label11.Text) - 1)
        'tmp.id = maxCurtainId + 1
        'maxCurtainId += 1
        tmp.Remark = TextBox2.Text
        tmp.DefaultX = NumericUpDown6.Value
        tmp.DefaultY = NumericUpDown7.Value
        'tmp.x = tmp.defaultX '/ sysInfo.zoomProportion
        'tmp.y = tmp.defaultY '/ sysInfo.zoomProportion
        'tmp.playDialog

        tmp.ScreenList = New List(Of Integer)
        For i As Integer = 0 To DataGridView3.Rows.Count - 1 - 1
            Try
                With DataGridView3.Rows(i)
                    tmp.ScreenList.Add(.Cells(0).Value)

                    sysInfo.ScreenList(.Cells(0).Value).DefaultX = .Cells(1).Value
                    sysInfo.ScreenList(.Cells(0).Value).DefaultY = .Cells(2).Value
                    sysInfo.ScreenList(.Cells(0).Value).TouchPieceRowsNum = .Cells(3).Value
                    sysInfo.ScreenList(.Cells(0).Value).TouchPieceColumnsNum = .Cells(4).Value
                End With

            Catch ex As Exception
                MsgBox($"第{i + 1}行数据错误:{ex.Message}", MsgBoxStyle.Information, "保存修改")
                Exit Sub
            End Try
        Next

        sysInfo.CurtainList.Item(CInt(Label11.Text) - 1) = tmp
        'DataGridView2.Rows(CInt(Label11.Text) - 1).Cells(0).Value = DataGridView2.Rows.Count + 1
        DataGridView2.Rows(CInt(Label11.Text) - 1).Cells(1).Value = tmp.Remark
        DataGridView2.Rows(CInt(Label11.Text)-1).Cells(2).Value = $"{tmp.DefaultX},{tmp.DefaultY}"

        Dim maxHeight = 0
        Dim maxWidth = 0

        For Each j In tmp.ScreenList
            '最大高度
            maxHeight =
                If(maxHeight < sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                sysInfo.ScreenList(j).DefaultY + sysInfo.ScreenList(j).DefaultHeight,
                maxHeight)

            '最大宽度
            maxWidth =
                If(maxWidth < sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                sysInfo.ScreenList(j).DefaultX + sysInfo.ScreenList(j).DefaultWidth,
                maxWidth)
        Next
        maxHeight = maxHeight / sysInfo.ZoomProportion
        maxWidth = maxWidth / sysInfo.ZoomProportion

        DataGridView2.Rows(CInt(Label11.Text) - 1).Cells(2).Value = $"{tmp.DefaultX},{tmp.DefaultY} [{maxWidth},{maxHeight}]"
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
        If sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte) Then
            '触摸灵敏度
            sysInfo.TouchSensitivity = NumericUpDown2.Value
        Else
            MsgBox($"指令发送失败",
                   MsgBoxStyle.Information,
                   $"触摸灵敏度")
        End If

        Thread.Sleep(100)
    End Sub

    ''' <summary>
    ''' 显示点击屏幕走线
    ''' </summary>
    Private Sub DataGridView4_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView4.CellMouseClick
        Dim selectScreenId As Integer = DataGridView4.SelectedCells(0).RowIndex
        GroupBox13.Text = $"{sysInfo.Language.GetLanguage("屏幕")}{selectScreenId} {sysInfo.Language.GetLanguage("走线")}"

        Me.PictureBox1.Update()
        Dim g As Graphics = Me.PictureBox1.CreateGraphics
        Dim mpen As New Pen(Color.Green)
        Dim tmpHeight As Integer =
            sysInfo.ScreenList(selectScreenId).DefaultHeight * 21 \ sysInfo.ScreenList(selectScreenId).DefaultScanBoardHeight
        Dim tmpWidth As Integer =
            sysInfo.ScreenList(selectScreenId).DefaultWidth * 21 \ sysInfo.ScreenList(selectScreenId).DefaultScanBoardWidth

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
        'Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>")

        mpen.Color = Color.Red
        mpen.Width = 3
        For i As Integer = 0 To dataRow.Length - 1
            Dim tmpX As Integer = (dataRow(i).Item(4) \ sysInfo.ScreenList(selectScreenId).TouchPieceColumnsNum) * 21 + 10
            Dim tmpY As Integer = (dataRow(i).Item(5) \ sysInfo.ScreenList(selectScreenId).TouchPieceRowsNum) * 21 + 10

            'For k As Integer = 0 To 6 - 1
            '    Debug.Write($"{dataRow(i).Item(k)} ")
            'Next
            'Debug.WriteLine("")

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

    ''' <summary>
    ''' 计算缩放比例
    ''' </summary>
    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        TextBox1.Text = NumericUpDown8.Value / NumericUpDown9.Value
        sysInfo.ZoomTmpNumerator = NumericUpDown8.Value
        sysInfo.ZoomTmpDenominator = NumericUpDown9.Value
        sysInfo.ZoomProportion = Val(TextBox1.Text)
    End Sub

    ''' <summary>
    ''' 删除屏幕
    ''' </summary>
    Private Sub 删除ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 删除ToolStripMenuItem.Click
        Dim rowIndex As Integer = DataGridView3.SelectedCells(0).RowIndex
        If Not DataGridView3.Rows(rowIndex).IsNewRow Then
            DataGridView3.Rows.RemoveAt(rowIndex)
        End If
    End Sub

    ''' <summary>
    ''' 查询版本号 鼠标按下时
    ''' </summary>
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Enabled = False

        ListView1.Columns(3).Text = sysInfo.Language.GetLanguage("版本号")
        ListView1.Items.Clear()
        'TextBox4.Clear()
    End Sub

    ''' <summary>
    ''' 查询版本号 鼠标松开时
    ''' </summary>
    Private Sub Button1_MouseUp(sender As Object, e As MouseEventArgs) Handles Button1.MouseUp
        If Button1.Enabled Then
            Exit Sub
        End If

        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            GetScanBoardData(i, 0)
        Next

        Button1.Enabled = True
    End Sub

    '''' <summary>
    '''' 查询复位次数 鼠标按下时
    '''' </summary>
    'Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
    '    Button2.Enabled = False

    '    ListView1.Columns(3).Text = GetLanguage("复位次数")
    '    ListView1.Items.Clear()
    '    'TextBox4.Clear()
    'End Sub

    '''' <summary>
    '''' 查询复位次数 鼠标松开时
    '''' </summary>
    'Private Sub Button2_MouseUp(sender As Object, e As MouseEventArgs) Handles Button2.MouseUp
    '    If Button2.Enabled Then
    '        Exit Sub
    '    End If

    '    For i As Integer = 0 To sysInfo.SenderList.Length - 1
    '        GetScanBoardData(i, 1)
    '    Next

    '    Button2.Enabled = True
    'End Sub

    ''' <summary>
    ''' 获取接收卡数据
    ''' </summary>
    Private Sub GetScanBoardData(senderId As Integer, dataType As Integer)
        Dim sendstr As String = "00"
        Select Case dataType
            Case 0
                sendstr = "aadb0901"
                'Case 1
                '    sendstr = "aadb0201"
        End Select

        Dim sendByte(sendstr.Length \ 2 - 1) As Byte

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next

        sysInfo.MainClass.SetScanBoardData(senderId, 255, 65535, sendByte)

        Thread.Sleep(500)

        Dim cliSocket As Socket
        Try
            cliSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp) With {
            .SendTimeout = 500,
            .ReceiveTimeout = 500
            }
            '连接
            With sysInfo.SenderList(senderId)
                cliSocket.Connect(
                    String.Format("{0}.{1}.{2}.{3}", .IpDate(3), .IpDate(2), .IpDate(1), .IpDate(0)),
                    6000)
            End With
        Catch ex As Exception
            MsgBox($"连接异常:{ex.Message}",
                   MsgBoxStyle.Information,
                   "获取接收卡数据")
            Exit Sub
        End Try

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
            'MsgBox($"发送读取指令异常:{ex.Message}",
            '       MsgBoxStyle.Information,
            '       "获取接收卡数据")
            'TextBox4.AppendText($"发送读取指令异常:{ex.Message}{vbCrLf}")
            'Exit Sub
        End Try

        'Dim asd As New Stopwatch
        'asd.Start()
        Dim getDataSum As Integer = 0

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

                    Dim itm As ListViewItem = ListView1.Items.Add($"{senderId}")
                    itm.SubItems.Add($"{bytes(j + 1)}")
                    itm.SubItems.Add($"{(bytes(j + 2) * 256 + bytes(j + 3))}")

                    Select Case dataType
                        Case 0
                            itm.SubItems.Add($"{bytes(j + 4)}.{bytes(j + 5)}.{bytes(j + 6)}")
                        Case 1
                            itm.SubItems.Add($"{bytes(j + 4) + bytes(j + 5) * &H100 + bytes(j + 6) * &H10000}")
                    End Select

                    getDataSum += 1
                Next
            Next
        Catch ex As Exception
            'TextBox4.AppendText($"接收数据异常:{ex.Message}{vbCrLf}")
        End Try

        cliSocket.Close()

        'TextBox4.AppendText($"发送卡{senderId} 读取到 {getDataSum} 个接收卡数据{vbCrLf}")
    End Sub

    '''' <summary>
    '''' 自动跳转
    '''' </summary>
    'Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
    '    TextBox4.ScrollToCaret()
    'End Sub

    ''' <summary>
    ''' 选择升级文件
    ''' </summary>
    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        Dim tmp As New OpenFileDialog With {
            .Filter = "bin|*.bin"
        }
        If tmp.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        TextBox3.Text = tmp.FileName
    End Sub

    ''' <summary>
    ''' 发送升级程序 鼠标按下时
    ''' </summary>
    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Button9.Enabled = False

        'TextBox4.Clear()
    End Sub

    ''' <summary>
    ''' 发送升级程序 鼠标松开时
    ''' </summary>
    Private Sub Button9_MouseUp(sender As Object, e As MouseEventArgs) Handles Button9.MouseUp
        If Button9.Enabled Then
            Exit Sub
        End If

        SendUpdata()

        Button9.Enabled = True
    End Sub

    ''' <summary>
    ''' 升级接收卡程序
    ''' </summary>
    Private Sub SendUpdata()
        If TextBox3.Text = "" Then
            Exit Sub
        End If

        '读文件信息
        Dim infoReader As System.IO.FileInfo = My.Computer.FileSystem.GetFileInfo(TextBox3.Text)
        Dim binLength = infoReader.Length
        'TextBox4.AppendText($"升级文件大小: {binLength} Byte{vbCrLf}")


        '发送升级指令
        Dim sendByte As Byte()
        Dim temps As String = ""
        Dim sendstr As String = "aadb09030000"
        ReDim sendByte(sendstr.Length \ 2 - 1)

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next

        sendByte(4) = binLength \ 256
        sendByte(5) = binLength Mod 256

        sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)

        Thread.Sleep(60)

        For i As Integer = 0 To 10
            If CheckUpdataRecData({&H1A, &H1B}) Then
                Exit For
            End If

            If i = 10 Then
                'Putlog($"升级指令发送失败")
                MsgBox($"升级指令发送失败",
                           MsgBoxStyle.Information,
                           "升级")
                Exit Sub
            End If
        Next


        '发送升级程序
        ReDim sendByte(131 - 1)
        Dim fs As New System.IO.FileStream(TextBox3.Text, IO.FileMode.Open, IO.FileAccess.Read)
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
                sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
                'TextBox4.AppendText($"第 {sendIndex} 包数据 第{i}次发送{vbCrLf}")
                Thread.Sleep(60)

                If CheckUpdataRecData({&H1C, &H1D}) Then
                    Exit For
                End If

                If i = 10 Then
                    'Putlog($"第 {sendIndex} 包升级数据发送失败")
                    re.Close()
                    fs.Close()
                    MsgBox($"升级数据发送失败",
                           MsgBoxStyle.Information,
                           "升级")
                    Exit Sub
                End If
            Next

            sendIndex += 1

            ProgressBar1.Value = (sendIndex * 128 * 100) \ binLength
            Label15.Text = $"{ProgressBar1.Value}%"

            If sendIndex * 128 >= binLength Then
                Exit Do
            End If
        Loop


        '发送完毕指令
        sysInfo.MainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, {&HAA, &HDB, &H9, &H9})
        re.Close()
        fs.Close()

        'TextBox4.AppendText($"数据发送完毕{vbCrLf}")
        MsgBox($"程序升级完毕",
               MsgBoxStyle.Information,
               "升级")
    End Sub

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
                Dim tmpstr As String = "55d50902"
                Dim sendbytes(4 - 1) As Byte
                For i As Integer = 0 To tmpstr.Length \ 2 - 1
                    sendbytes(i) = Val("&H" & tmpstr(i * 2)) * 16 + Val("&H" & tmpstr(i * 2 + 1))
                Next i

                Dim bytesSend As Integer = cliSocket.Send(sendbytes)
                Dim bytesRec As Integer = cliSocket.Receive(bytes)

            Catch ex As Exception
                'TextBox4.AppendText($"发送读取指令异常:{ex.Message}{vbCrLf}")
                cliSocket.Close()
                Return False
            End Try

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

                        For i As Integer = 0 To checkData.Length - 1
                            If bytes(j + 4 + i) <> checkData(i) Then
                                errorSum += 1
                                'cliSocket.Close()
                                'TextBox4.AppendText("校验值错误")
                                'Return False
                                Exit For
                            End If
                        Next

                        recSum = recSum + 1
                    Next
                Next
            Catch ex As Exception
                'TextBox4.AppendText($"接收数据异常:{ex.Message}{vbCrLf}")
                errorSum += 1
                'cliSocket.Close()
                'Return False
            End Try

            cliSocket.Close()
        Next

        If errorSum > 0 Then
            Return False
        End If

        Return If(recSum > 0, True, False)
    End Function
End Class