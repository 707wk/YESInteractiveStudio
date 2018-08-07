Public Class ScreenLigature
    ''' <summary>
    ''' 临时数据表
    ''' </summary>
    Private TmpDataTable As DataTable

    Private Sub ScreenLigature_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ''todo:屏幕走线
#Region "样式设置"
        Me.Text = sysInfo.Language.GetS("Screen Ligature")

        With ListView1
            .View = View.Details
            .GridLines = True
            .LabelEdit = False
            .FullRowSelect = True
            .HideSelection = True
            .ShowItemToolTips = True
            .MultiSelect = False

            .Columns.Add(sysInfo.Language.GetS("ID"), 40)
            .Columns.Add(sysInfo.Language.GetS("Size"), 80)
        End With

        For i001 As Integer = 0 To sysInfo.ScreenList.Count - 1
            Dim TmpListViewItem As New ListViewItem
            With TmpListViewItem
                .SubItems.Add(i001)
                .SubItems.Add($"{sysInfo.ScreenList(i001).DefSize.Width},{sysInfo.ScreenList(i001).DefSize.Height}")
            End With

            ListView1.Items.Add(TmpListViewItem)
        Next

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
#End Region

#Region "加载数据"
        '创建临时表
        TmpDataTable = New DataTable("ScanBoardTable")

        '添加字段
        TmpDataTable.Columns.Add("ScreenIndex", System.Type.GetType("System.Int32"))
        TmpDataTable.Columns.Add("SenderIndex", System.Type.GetType("System.Int32"))
        TmpDataTable.Columns.Add("PortIndex", System.Type.GetType("System.Int32"))
        TmpDataTable.Columns.Add("ConnectIndex", System.Type.GetType("System.Int32"))
        TmpDataTable.Columns.Add("X", System.Type.GetType("System.Int32"))
        TmpDataTable.Columns.Add("Y", System.Type.GetType("System.Int32"))

        '载入数据
        For Each i In sysInfo.ScanBoardTable.Keys
            Dim qwe As ScanBoardInfo = sysInfo.ScanBoardTable.Item(i)

            Dim row As DataRow = TmpDataTable.NewRow()
            row("ScreenIndex") = qwe.ScreenId
            row("SenderIndex") = qwe.SenderId
            row("PortIndex") = qwe.PortId
            row("ConnectIndex") = qwe.ConnectId
            row("X") = qwe.Location.X
            row("Y") = qwe.Location.Y

            TmpDataTable.Rows.Add(row)
        Next
#End Region
    End Sub

#Region "显示屏幕走线"
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        Dim selectScreenId As Integer = ListView1.SelectedItems(0).Index
        GroupBox2.Text = $"{sysInfo.Language.GetS("Screen")} {selectScreenId} {sysInfo.Language.GetS("Ligature")}"

        Dim tmpHeight As Integer =
            sysInfo.ScreenList(selectScreenId).DefSize.Height * 21 \ sysInfo.ScreenList(selectScreenId).DefScanBoardSize.Height
        Dim tmpWidth As Integer =
            sysInfo.ScreenList(selectScreenId).DefSize.Width * 21 \ sysInfo.ScreenList(selectScreenId).DefScanBoardSize.Width

        Dim LigatureBitmap As Bitmap = New Bitmap(tmpWidth + 1, tmpHeight + 1)
        Dim g As Graphics = Graphics.FromImage(LigatureBitmap)
        Dim mpen As New Pen(Color.Green)

        For i As Integer = 0 To tmpHeight Step 21
            g.DrawLine(mpen, 0, i, tmpWidth, i)
        Next

        For i As Integer = 0 To tmpWidth Step 21
            g.DrawLine(mpen, i, 0, i, tmpHeight)
        Next

        Dim dataRow() As DataRow = TmpDataTable.Select($"ScreenIndex={selectScreenId}", "SenderIndex ASC,PortIndex ASC,ConnectIndex ASC")

        Dim lastSenderIndex As Integer = -1
        Dim lastPortIndex As Integer = -1
        Dim lastX As Integer = 0
        Dim lastY As Integer = 0

        mpen.Color = Color.Red
        mpen.Width = 3
        For i As Integer = 0 To dataRow.Length - 1
            Dim tmpX As Integer = (dataRow(i).Item(4) \ sysInfo.ScreenList(selectScreenId).SensorLayout.Width) * 21 + 10
            Dim tmpY As Integer = (dataRow(i).Item(5) \ sysInfo.ScreenList(selectScreenId).SensorLayout.Height) * 21 + 10

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

        PictureBox1.Image = LigatureBitmap
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.GroupBox1.Text = .GetS("Screen List")
            Me.GroupBox2.Text = .GetS("Ligature")
        End With
    End Sub
#End Region
End Class