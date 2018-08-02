Public Class ScreenSingleSelect
    Public SelectScreenID As Integer

    Private Sub ScreenSingleSelect_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        Me.Text = sysInfo.Language.GetS("Select Screen")

        With ListView1
            .View = View.Details
            .GridLines = True
            .LabelEdit = False
            .FullRowSelect = True
            .HideSelection = True
            .ShowItemToolTips = True
            .MultiSelect = False

            .Columns.Add("序号", 40)
            .Columns.Add("尺寸", 80)
        End With

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()

#Region "排除已选择的屏幕"
        Dim TmpScreenList As New List(Of Integer)
        For i001 As Integer = 0 To sysInfo.ScreenList.Count - 1
            TmpScreenList.Add(i001)
        Next
        For Each i002 As WindowInfo In sysInfo.Schedule.WindowList
            For Each j002 As Integer In i002.ScreenList
                TmpScreenList.Remove(j002)
            Next
        Next
        For Each i002 In TmpScreenList
            Dim TmpListViewItem As New ListViewItem
            With TmpListViewItem
                .SubItems.Add(i002)
                .SubItems.Add($"{sysInfo.ScreenList(i002).DefSize.Width},{sysInfo.ScreenList(i002).DefSize.Height}")
            End With

            ListView1.Items.Add(TmpListViewItem)
        Next
#End Region
#End Region
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.DialogResult = DialogResult.OK
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
        If ListView1.SelectedItems.Count < 1 Then
            Exit Sub
        End If

        SelectScreenID = Val(ListView1.SelectedItems(0).SubItems(0).Text)
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.Button1.Text = .GetS("OK")
        End With
    End Sub
#End Region
End Class