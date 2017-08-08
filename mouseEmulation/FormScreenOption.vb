Public Class FormScreenOption
    '临时数据表
    Dim tmpDataTable As DataTable
    Private Sub FormScreenOption_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For i As Integer = 0 To screenMain.Length - 1
            DataGridView1.Rows.Add(screenMain(i).showFlage,
                                   screenMain(i).index,
                                   screenMain(i).remark,
                                   $"起始点:{screenMain(i).x},{screenMain(i).y}  size:[{screenMain(i).width},{screenMain(i).height}]")
        Next

        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        '测试数据
        'For i As Integer = 0 To screenMain.Length - 1
        '    DataGridView1.Rows.Add(True,
        '                           i * 10 + 5,
        '                           "无",
        '                           $"起始点")
        'Next
        ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''

        '创建临时表
        tmpDataTable = New DataTable("ScanBoardTable")

        '添加字段
        tmpDataTable.Columns.Add("ScreenIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("SenderIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("PortIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("ConnectIndex", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("X", System.Type.GetType("System.Int32"))
        tmpDataTable.Columns.Add("Y", System.Type.GetType("System.Int32"))

        '设置主键[错误]
        'Dim PrimaryKeyColumns(0) As DataColumn
        'PrimaryKeyColumns(0) = tmpDataTable.Columns("ScreenIndex")
        'tmpDataTable.PrimaryKey = PrimaryKeyColumns

        '载入数据
        For Each i In ScanBoardTable.Keys
            Dim qwe As ScanBoardInfo = ScanBoardTable.Item(i)

            Dim row As DataRow = tmpDataTable.NewRow()
            row("ScreenIndex") = qwe.ScreenIndex
            row("SenderIndex") = qwe.SenderIndex
            row("PortIndex") = qwe.PortIndex
            row("ConnectIndex") = qwe.ConnectIndex
            row("X") = qwe.X
            row("Y") = qwe.Y

            tmpDataTable.Rows.Add(row)
        Next

    End Sub

    Private Sub DataGridView1_CellMouseClick(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView1.CellMouseClick
        Dim selectScreenId As Integer = DataGridView1.Rows(DataGridView1.CurrentCell.RowIndex).Cells(1).Value
        GroupBox2.Text = $"屏幕{selectScreenId} 走线"

        Me.PictureBox1.Update()
        Dim g As Graphics = Me.PictureBox1.CreateGraphics
        Dim mpen As New Pen(Color.Green)
        Dim tmpHeight As Integer = screenMain(selectScreenId).height * 21 \ screenMain(selectScreenId).ScanBoardHeight
        Dim tmpWidth As Integer = screenMain(selectScreenId).width * 21 \ screenMain(selectScreenId).ScanBoardWidth

        For i As Integer = 0 To tmpHeight Step 21
            g.DrawLine(mpen, 0, i, tmpWidth, i)
        Next

        For i As Integer = 0 To tmpWidth Step 21
            g.DrawLine(mpen, i, 0, i, tmpHeight)
        Next

        Dim dataRow() As DataRow = tmpDataTable.Select($"ScreenIndex={selectScreenId}", "SenderIndex ASC,PortIndex ASC,ConnectIndex ASC")

        'TextBox1.Text = $"{dataRow.Length}{vbCrLf}"
        'For Each i In dataRow
        '    TextBox1.AppendText($"{i.Item(0)},{i.Item(1)},{i.Item(2)},{i.Item(3)},{i.Item(4)},{i.Item(5)}{vbCrLf}")
        'Next

        Dim lastSenderIndex As Integer = -1
        Dim lastPortIndex As Integer = -1
        Dim lastX As Integer = 0
        Dim lastY As Integer = 0

        mpen.Color = Color.Red
        mpen.Width = 3
        For i As Integer = 0 To dataRow.Length - 1
            Dim tmpX As Integer = (dataRow(i).Item(4) \ screenMain(selectScreenId).ScanBoardWidth) * 21 + 10
            Dim tmpY As Integer = (dataRow(i).Item(5) \ screenMain(selectScreenId).ScanBoardHeight) * 21 + 10

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