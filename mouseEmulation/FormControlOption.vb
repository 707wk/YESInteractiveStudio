Imports System.ComponentModel
Imports System.Net
Imports System.Text.RegularExpressions
Imports Nova.Mars.SDK

Public Class FormControlOption
    Private Sub FormControlOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i As Integer = 0 To senderArray.Length - 1
            DataGridView1.Rows.Add(i,
                                   $"{senderArray(i).ipDate(3)}.{senderArray(i).ipDate(2)}.{senderArray(i).ipDate(1)}.{senderArray(i).ipDate(0)}",
                                   $"{senderArray(i).ipDate(7)}.{senderArray(i).ipDate(6)}.{senderArray(i).ipDate(5)}.{senderArray(i).ipDate(4)}",
                                   $"{senderArray(i).ipDate(11)}.{senderArray(i).ipDate(10)}.{senderArray(i).ipDate(9)}.{senderArray(i).ipDate(8)}")
        Next

        For i As Integer = 0 To senderArray.Length - 1
            'DataGridView1.Rows(i).Cells(1).ToolTipText = DataGridView1.Rows(i).Cells(1).Value
            'DataGridView1.Rows(i).Cells(2).ToolTipText = DataGridView1.Rows(i).Cells(2).Value
            'DataGridView1.Rows(i).Cells(3).ToolTipText = DataGridView1.Rows(i).Cells(3).Value
            For j As Integer = 0 To 12 - 1
                senderArray(i).tmpIpData(j) = senderArray(i).ipDate(j)
            Next
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '测试数据
        'For i As Integer = 10 To 32
        '    DataGridView1.Rows.Add(i,
        '                           $"192.168.11.{i}",
        '                           $"255.255.255.255",
        '                           $"255.255.{i}.254")
        'Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '绑定设置到ip事件
        AddHandler mainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData
    End Sub

    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseDown
        '右键选中
        If e.Button = System.Windows.Forms.MouseButtons.Right Then
            If e.RowIndex >= 0 And e.ColumnIndex >= 0 Then
                '选中行头或列头均不触发
                'MsgBox(e.RowIndex & " : " & e.ColumnIndex)
                '若行已是选中状态就不再进行设置
                If (DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = False) Then
                    DataGridView1.ClearSelection()
                    DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Selected = True
                End If
            End If
        End If
    End Sub

    Private Sub DataGridView1_SelectionChanged(sender As Object, e As EventArgs) Handles DataGridView1.SelectionChanged
        Me.Text = DataGridView1.Rows(DataGridView1.CurrentCell.RowIndex).Cells(0).Value
    End Sub

    Private Sub updataIp(i As Integer)
        DataGridView1.Rows(i).Cells(1).Value = $"{senderArray(i).tmpIpData(3)}.{senderArray(i).tmpIpData(2)}.{senderArray(i).tmpIpData(1)}.{senderArray(i).tmpIpData(0)}"
        DataGridView1.Rows(i).Cells(2).Value = $"{senderArray(i).tmpIpData(7)}.{senderArray(i).tmpIpData(6)}.{senderArray(i).tmpIpData(5)}.{senderArray(i).tmpIpData(4)}"
        DataGridView1.Rows(i).Cells(3).Value = $"{senderArray(i).tmpIpData(11)}.{senderArray(i).tmpIpData(10)}.{senderArray(i).tmpIpData(9)}.{senderArray(i).tmpIpData(8)}"

    End Sub

    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Dim setIpStr As String = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

        '切割ip字符串
        Dim ipDataStrArr() As String
        ipDataStrArr = Split(setIpStr, ".")

        '判断长度
        If ipDataStrArr.Length <> 4 Then
            MsgBox("非法参数", MsgBoxStyle.Information, Me.Text)
            updataIp(e.RowIndex)
            Exit Sub
        End If

        '判断数值
        For i As Integer = 0 To 4 - 1
            Dim reg As New Regex("\d+")
            Dim m As Match = reg.Match(ipDataStrArr(i))

            If m.Success Then
            Else
                MsgBox("非法参数", MsgBoxStyle.Information, Me.Text)
                updataIp(e.RowIndex)
                Exit Sub
            End If

            Dim tmpNum As Integer = CInt(m.Value)

            If tmpNum > 255 Then
                MsgBox("非法参数", MsgBoxStyle.Information, Me.Text)
                updataIp(e.RowIndex)
                Exit Sub
            End If
        Next

        ''DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).ToolTipText = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

        '保存ip信息
        Select Case e.ColumnIndex
            Case 1
                'ip
                senderArray(e.RowIndex).tmpIpData(3) = CInt(ipDataStrArr(0))
                senderArray(e.RowIndex).tmpIpData(2) = CInt(ipDataStrArr(1))
                senderArray(e.RowIndex).tmpIpData(1) = CInt(ipDataStrArr(2))
                senderArray(e.RowIndex).tmpIpData(0) = CInt(ipDataStrArr(3))
            Case 2
                '子网掩码
                senderArray(e.RowIndex).tmpIpData(7) = CInt(ipDataStrArr(0))
                senderArray(e.RowIndex).tmpIpData(6) = CInt(ipDataStrArr(1))
                senderArray(e.RowIndex).tmpIpData(5) = CInt(ipDataStrArr(2))
                senderArray(e.RowIndex).tmpIpData(4) = CInt(ipDataStrArr(3))
            Case 3
                '网关
                senderArray(e.RowIndex).tmpIpData(11) = CInt(ipDataStrArr(0))
                senderArray(e.RowIndex).tmpIpData(10) = CInt(ipDataStrArr(1))
                senderArray(e.RowIndex).tmpIpData(9) = CInt(ipDataStrArr(2))
                senderArray(e.RowIndex).tmpIpData(8) = CInt(ipDataStrArr(3))
        End Select
    End Sub

    '设置ip通知
    Dim senderArrayIndex As Integer = 0
    Private Sub SendEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        'Static Dim senderArrayIndex As Integer = 0
        If e.IsExecResult Then
            If senderArrayIndex < senderArray.Length - 1 Then
                senderArray(senderArrayIndex).ipDate = senderArray(senderArrayIndex).tmpIpData

                senderArrayIndex += 1

                mainClass.SetEquipmentIP(senderArrayIndex, senderArray(senderArrayIndex).tmpIpData)
            Else
                senderArray(senderArrayIndex).ipDate = senderArray(senderArrayIndex).tmpIpData
                Me.closeDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
            End If
        Else
            MsgBox($"控制器{senderArrayIndex}设置IP数据失败！请检查设备后，重新设置！")
            'senderArrayIndex = senderArray.Length
            Me.closeDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
        End If
    End Sub

    '关闭时保存ip数据
    'Private Sub FormControlOption_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
    '如此BT的解决方式
    'If senderArray.Length <> senderArrayIndex Then
    '    e.Cancel = True

    '    'For i As Integer = 0 To senderArray.Length - 1
    '    '    '切割ip字符串
    '    '    Dim ipDataStrArr() As String
    '    '    'ip
    '    '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(1).Value, ".")
    '    '    senderArray(i).tmpIpData(3) = CInt(ipDataStrArr(0))
    '    '    senderArray(i).tmpIpData(2) = CInt(ipDataStrArr(1))
    '    '    senderArray(i).tmpIpData(1) = CInt(ipDataStrArr(2))
    '    '    senderArray(i).tmpIpData(0) = CInt(ipDataStrArr(3))
    '    '    '子网掩码
    '    '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(2).Value, ".")
    '    '    senderArray(i).tmpIpData(7) = CInt(ipDataStrArr(0))
    '    '    senderArray(i).tmpIpData(6) = CInt(ipDataStrArr(1))
    '    '    senderArray(i).tmpIpData(5) = CInt(ipDataStrArr(2))
    '    '    senderArray(i).tmpIpData(4) = CInt(ipDataStrArr(3))
    '    '    '网关
    '    '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(3).Value, ".")
    '    '    senderArray(i).tmpIpData(11) = CInt(ipDataStrArr(0))
    '    '    senderArray(i).tmpIpData(10) = CInt(ipDataStrArr(1))
    '    '    senderArray(i).tmpIpData(9) = CInt(ipDataStrArr(2))
    '    '    senderArray(i).tmpIpData(8) = CInt(ipDataStrArr(3))
    '    'Next

    '    senderArrayIndex = 0
    '    mainClass.SetEquipmentIP(0, senderArray(senderArrayIndex).tmpIpData)
    'End If
    'End Sub

    '关闭窗体
    Public Delegate Sub closeDialogCallback(ByVal text As String)
    Public Sub closeDialog(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New closeDialogCallback(AddressOf closeDialog)
            Me.Invoke(d, New Object() {text})
        Else
            '移除事件
            RemoveHandler mainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData

            Me.Close()
        End If
    End Sub

    '保存修改
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        'For i As Integer = 0 To senderArray.Length - 1
        '    '切割ip字符串
        '    Dim ipDataStrArr() As String
        '    'ip
        '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(1).Value, ".")
        '    senderArray(i).tmpIpData(3) = CInt(ipDataStrArr(0))
        '    senderArray(i).tmpIpData(2) = CInt(ipDataStrArr(1))
        '    senderArray(i).tmpIpData(1) = CInt(ipDataStrArr(2))
        '    senderArray(i).tmpIpData(0) = CInt(ipDataStrArr(3))
        '    '子网掩码
        '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(2).Value, ".")
        '    senderArray(i).tmpIpData(7) = CInt(ipDataStrArr(0))
        '    senderArray(i).tmpIpData(6) = CInt(ipDataStrArr(1))
        '    senderArray(i).tmpIpData(5) = CInt(ipDataStrArr(2))
        '    senderArray(i).tmpIpData(4) = CInt(ipDataStrArr(3))
        '    '网关
        '    ipDataStrArr = Split(DataGridView1.Rows(i).Cells(3).Value, ".")
        '    senderArray(i).tmpIpData(11) = CInt(ipDataStrArr(0))
        '    senderArray(i).tmpIpData(10) = CInt(ipDataStrArr(1))
        '    senderArray(i).tmpIpData(9) = CInt(ipDataStrArr(2))
        '    senderArray(i).tmpIpData(8) = CInt(ipDataStrArr(3))
        'Next

        'Me.Close()
        'senderArrayIndex = 0
        'Dim tmpIpData(12 - 1) As Byte
        ''切割ip字符串
        'Dim ipDataStrArr() As String
        ''ip
        'ipDataStrArr = Split(DataGridView1.Rows(senderArrayIndex).Cells(1).Value, ".")
        'tmpIpData(3) = CInt(ipDataStrArr(0))
        'tmpIpData(2) = CInt(ipDataStrArr(1))
        'tmpIpData(1) = CInt(ipDataStrArr(2))
        'tmpIpData(0) = CInt(ipDataStrArr(3))
        ''子网掩码
        'ipDataStrArr = Split(DataGridView1.Rows(senderArrayIndex).Cells(2).Value, ".")
        'tmpIpData(7) = CInt(ipDataStrArr(0))
        'tmpIpData(6) = CInt(ipDataStrArr(1))
        'tmpIpData(5) = CInt(ipDataStrArr(2))
        'tmpIpData(4) = CInt(ipDataStrArr(3))
        ''网关
        'ipDataStrArr = Split(DataGridView1.Rows(senderArrayIndex).Cells(3).Value, ".")
        'tmpIpData(11) = CInt(ipDataStrArr(0))
        'tmpIpData(10) = CInt(ipDataStrArr(1))
        'tmpIpData(9) = CInt(ipDataStrArr(2))
        'tmpIpData(8) = CInt(ipDataStrArr(3))

        senderArrayIndex = 0
        mainClass.SetEquipmentIP(0, senderArray(senderArrayIndex).tmpIpData)
    End Sub
End Class