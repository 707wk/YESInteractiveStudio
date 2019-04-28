Imports System.ComponentModel
Imports Nova.Mars.SDK

Public Class ControlNetwork
    Private Sub ControlNetwork_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''todo:控制器ip设置
#Region "样式设置"
        Me.Text = sysInfo.Language.GetS("Control Network")

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

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()

        '绑定设置到ip事件
        AddHandler sysInfo.MainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData
#End Region
    End Sub

    Private Sub ControlNetwork_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '解绑设置到ip事件
        RemoveHandler sysInfo.MainClass.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData
    End Sub

#Region "右键选中"
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
#End Region

#Region "保存修改后的ip信息"
#Region "更新显示IP信息"
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
#End Region

#Region "输入检测"
    Private Sub DataGridView1_CellEndEdit(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellEndEdit
        Dim setIpStr As String = DataGridView1.Rows(e.RowIndex).Cells(e.ColumnIndex).Value

        '切割ip字符串
        Dim ipDataStrArr() As String
        ipDataStrArr = Split(setIpStr, ".")

        '判断长度
        If ipDataStrArr.Length <> 4 Then
            MsgBox(sysInfo.Language.GetS("Invalid Argument"),
                   MsgBoxStyle.Information,
                   Me.Text)
            UpdataIp(e.RowIndex)
            Exit Sub
        End If

        '判断数值
        For i As Integer = 0 To 4 - 1
            Dim tmpNum As Integer = Val(ipDataStrArr(i))

            If tmpNum < 0 OrElse
                tmpNum > 255 Then

                MsgBox(sysInfo.Language.GetS("Invalid Argument"),
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
                sysInfo.SenderList(e.RowIndex).TmpIpData(3) = Val(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(2) = Val(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(1) = Val(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(0) = Val(ipDataStrArr(3))
            Case 2
                '子网掩码
                sysInfo.SenderList(e.RowIndex).TmpIpData(7) = Val(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(6) = Val(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(5) = Val(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(4) = Val(ipDataStrArr(3))
            Case 3
                '网关
                sysInfo.SenderList(e.RowIndex).TmpIpData(11) = Val(ipDataStrArr(0))
                sysInfo.SenderList(e.RowIndex).TmpIpData(10) = Val(ipDataStrArr(1))
                sysInfo.SenderList(e.RowIndex).TmpIpData(9) = Val(ipDataStrArr(2))
                sysInfo.SenderList(e.RowIndex).TmpIpData(8) = Val(ipDataStrArr(3))
        End Select
    End Sub
#End Region

#Region "设置ip通知"
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
                ShowMsgBox($"{sysInfo.Language.GetS("Modified Control IP successfully")}!")
            End If
        Else
            ShowMsgBox($"{sysInfo.Language.GetS("Control")}{senderArrayIndex} {sysInfo.Language.GetS("Failed to modify IP")}!")
        End If
    End Sub
#End Region

#Region "提示信息"
    Public Delegate Sub ShowMsgBoxCallback(ByVal Str As String)
    ''' <summary>
    ''' 提示信息
    ''' </summary>
    Public Sub ShowMsgBox(ByVal Str As String)
        If Me.InvokeRequired Then
            Me.Invoke(New ShowMsgBoxCallback(AddressOf ShowMsgBox), New Object() {Str})
            Exit Sub
        End If

        Button1.Enabled = True
        MsgBox(Str,
               MsgBoxStyle.Information,
               Me.Text)
    End Sub
#End Region

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Button1.Enabled = False
        senderArrayIndex = 0
        sysInfo.MainClass.SetEquipmentIP(0, sysInfo.SenderList(senderArrayIndex).TmpIpData)
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.GroupBox1.Text = .GetS("Control Network")
            Me.Column1.HeaderText = .GetS("ID")
            Me.Column2.HeaderText = .GetS("IP Address")
            Me.Column3.HeaderText = .GetS("Gateway")
            Me.Column4.HeaderText = .GetS("Subnet Mask")
            Me.Button1.Text = .GetS("Apply")
        End With
    End Sub
#End Region
End Class