Imports System.ComponentModel
Imports Nova.Mars.SDK

Public Class ControlNetwork
    Private Sub ControlNetwork_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''todo:控制器ip设置
#Region "样式设置"
        Me.Text = AppSetting.Language.GetS("Control Network")

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

        For i As Integer = 0 To AppSetting.SenderList.Length - 1
            DataGridView1.Rows.Add(i,
                                   $"{AppSetting.SenderList(i).IpDate(3)}.{AppSetting.SenderList(i).IpDate(2)}.{AppSetting.SenderList(i).IpDate(1)}.{AppSetting.SenderList(i).IpDate(0)}",
                                   $"{AppSetting.SenderList(i).IpDate(7)}.{AppSetting.SenderList(i).IpDate(6)}.{AppSetting.SenderList(i).IpDate(5)}.{AppSetting.SenderList(i).IpDate(4)}",
                                   $"{AppSetting.SenderList(i).IpDate(11)}.{AppSetting.SenderList(i).IpDate(10)}.{AppSetting.SenderList(i).IpDate(9)}.{AppSetting.SenderList(i).IpDate(8)}")
        Next

        For i As Integer = 0 To AppSetting.SenderList.Length - 1
            For j As Integer = 0 To 12 - 1
                AppSetting.SenderList(i).TmpIpData(j) = AppSetting.SenderList(i).IpDate(j)
            Next
        Next

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()

        '绑定设置到ip事件
        AddHandler AppSetting.NovaMarsControl.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData
#End Region
    End Sub

    Private Sub ControlNetwork_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        '解绑设置到ip事件
        RemoveHandler AppSetting.NovaMarsControl.SendEquipmentIPDataEvent, AddressOf SendEquipmentIPData
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
            $"{AppSetting.SenderList(i).TmpIpData(3)}.{AppSetting.SenderList(i).TmpIpData(2)}.{AppSetting.SenderList(i).TmpIpData(1)}.{AppSetting.SenderList(i).TmpIpData(0)}"
        DataGridView1.Rows(i).Cells(2).Value =
            $"{AppSetting.SenderList(i).TmpIpData(7)}.{AppSetting.SenderList(i).TmpIpData(6)}.{AppSetting.SenderList(i).TmpIpData(5)}.{AppSetting.SenderList(i).TmpIpData(4)}"
        DataGridView1.Rows(i).Cells(3).Value =
            $"{AppSetting.SenderList(i).TmpIpData(11)}.{AppSetting.SenderList(i).TmpIpData(10)}.{AppSetting.SenderList(i).TmpIpData(9)}.{AppSetting.SenderList(i).TmpIpData(8)}"
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
            MsgBox(AppSetting.Language.GetS("Invalid Argument"),
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

                MsgBox(AppSetting.Language.GetS("Invalid Argument"),
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
                AppSetting.SenderList(e.RowIndex).TmpIpData(3) = Val(ipDataStrArr(0))
                AppSetting.SenderList(e.RowIndex).TmpIpData(2) = Val(ipDataStrArr(1))
                AppSetting.SenderList(e.RowIndex).TmpIpData(1) = Val(ipDataStrArr(2))
                AppSetting.SenderList(e.RowIndex).TmpIpData(0) = Val(ipDataStrArr(3))
            Case 2
                '子网掩码
                AppSetting.SenderList(e.RowIndex).TmpIpData(7) = Val(ipDataStrArr(0))
                AppSetting.SenderList(e.RowIndex).TmpIpData(6) = Val(ipDataStrArr(1))
                AppSetting.SenderList(e.RowIndex).TmpIpData(5) = Val(ipDataStrArr(2))
                AppSetting.SenderList(e.RowIndex).TmpIpData(4) = Val(ipDataStrArr(3))
            Case 3
                '网关
                AppSetting.SenderList(e.RowIndex).TmpIpData(11) = Val(ipDataStrArr(0))
                AppSetting.SenderList(e.RowIndex).TmpIpData(10) = Val(ipDataStrArr(1))
                AppSetting.SenderList(e.RowIndex).TmpIpData(9) = Val(ipDataStrArr(2))
                AppSetting.SenderList(e.RowIndex).TmpIpData(8) = Val(ipDataStrArr(3))
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
            If senderArrayIndex < AppSetting.SenderList.Length - 1 Then
                AppSetting.SenderList(senderArrayIndex).IpDate = AppSetting.SenderList(senderArrayIndex).TmpIpData

                senderArrayIndex += 1

                AppSetting.NovaMarsControl.SetEquipmentIP(senderArrayIndex, AppSetting.SenderList(senderArrayIndex).TmpIpData)
            Else
                AppSetting.SenderList(senderArrayIndex).IpDate = AppSetting.SenderList(senderArrayIndex).TmpIpData
                ShowMsgBox($"{AppSetting.Language.GetS("Modified Control IP successfully")}!")
            End If
        Else
            ShowMsgBox($"{AppSetting.Language.GetS("Control")}{senderArrayIndex} {AppSetting.Language.GetS("Failed to modify IP")}!")
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
        AppSetting.NovaMarsControl.SetEquipmentIP(0, AppSetting.SenderList(senderArrayIndex).TmpIpData)
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With AppSetting.Language
            Me.GroupBox1.Text = .GetS("Control Network")
            Me.Column1.HeaderText = .GetS("ID")
            Me.Column2.HeaderText = .GetS("IP Address")
            Me.Column3.HeaderText = .GetS("Subnet Mask")
            Me.Column4.HeaderText = .GetS("Gateway")
            Me.Button1.Text = .GetS("Apply")
        End With
    End Sub
#End Region
End Class