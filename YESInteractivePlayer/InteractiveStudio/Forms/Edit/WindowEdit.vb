Imports System.ComponentModel

Public Class WindowEdit
    ''' <summary>
    ''' 窗口ID
    ''' </summary>
    Public WindowId As Integer

    '''' <summary>
    '''' 屏幕控件
    '''' </summary>
    Private ScreenControls As Button()

#Region "清空屏幕"
    ''' <summary>
    ''' 清空屏幕
    ''' </summary>
    Public Sub ClearScreen()
        sysInfo.Schedule.WindowList(WindowId).ScreenList.Clear()

        For i001 As Integer = 0 To ScreenControls.Count - 1
            ScreenControls(i001).Visible = False
        Next
    End Sub

    Private Sub ToolStripButton4_Click(sender As Object, e As EventArgs) Handles ToolStripButton4.Click
        ClearScreen()
    End Sub
#End Region

#Region "加载窗体信息"
    ''' <summary>
    ''' 加载窗体信息
    ''' </summary>
    Public Sub LoadWindow()
        With sysInfo.Schedule.WindowList(WindowId)
            TextBox1.Text = .Remark

            NumericUpDown1.Value = .Location.X
            NumericUpDown2.Value = .Location.Y

            NumericUpDown3.Value = .ZoomPix.Width
            NumericUpDown4.Value = .ZoomPix.Height

            For Each i001 As Integer In .ScreenList
                If i001 >= ScreenControls.Count Then
                    Continue For
                End If

                ScreenControls(i001).Location = sysInfo.Schedule.ScreenLocations(i001)
                ScreenControls(i001).Size = sysInfo.ScreenList(i001).DefSize
                ScreenControls(i001).Visible = True
            Next
        End With
    End Sub
#End Region

    Private Sub WindowEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = sysInfo.Schedule.WindowList.Item(WindowId).Remark

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()

        ReDim ScreenControls(sysInfo.ScreenList.Count - 1)
        For i001 As Integer = 0 To ScreenControls.Count - 1
            ScreenControls(i001) = New ScreenButton With {
                .ScreenId = i001,
                .Visible = False,
                .ContextMenuStrip = ScreenMenuStrip
            }
            Panel1.Controls.Add(ScreenControls(i001))

            'AddHandler ScreenControls(i001).Click, AddressOf ScreenControls_Click
            AddHandler ScreenControls(i001).MouseDown, AddressOf ScreenControls_MouseDown
            AddHandler ScreenControls(i001).MouseMove, AddressOf ScreenControls_MouseMove
        Next

        LoadWindow()
    End Sub

    Private Sub WindowEdit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        For i001 As Integer = 0 To ScreenControls.Count - 1
            'RemoveHandler ScreenControls(i001).Click, AddressOf ScreenControls_Click
            RemoveHandler ScreenControls(i001).MouseDown, AddressOf ScreenControls_MouseDown
            RemoveHandler ScreenControls(i001).MouseMove, AddressOf ScreenControls_MouseMove
        Next
    End Sub

#Region "显示屏幕信息"
    ''' <summary>
    ''' 显示选中屏幕信息
    ''' </summary>
    Private Sub ScreenControls_MouseDown(sender As Object, e As EventArgs)
        Dim TmpScreenButton As ScreenButton = CType(sender, ScreenButton)

        TextBox2.Text = TmpScreenButton.ScreenId
        NumericUpDown6.Value = TmpScreenButton.Location.X
        NumericUpDown5.Value = TmpScreenButton.Location.Y
    End Sub

    ''' <summary>
    ''' 显示屏幕移动位置
    ''' </summary>
    Private Sub ScreenControls_MouseMove(sender As Object, e As EventArgs)
        Dim TmpScreenButton As ScreenButton = CType(sender, ScreenButton)

        NumericUpDown6.Value = TmpScreenButton.Location.X
        NumericUpDown5.Value = TmpScreenButton.Location.Y
    End Sub
#End Region

#Region "添加屏幕"
    ''' <summary>
    ''' 添加屏幕
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ToolStripButton3_Click(sender As Object, e As EventArgs) Handles ToolStripButton3.Click
        Dim TmpDialog As New ScreenSingleSelect
        If TmpDialog.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        sysInfo.Schedule.WindowList(WindowId).ScreenList.Add(TmpDialog.SelectScreenID)

        ScreenControls(TmpDialog.SelectScreenID).Location = New Point(0, 0)
        ScreenControls(TmpDialog.SelectScreenID).Size = sysInfo.ScreenList(TmpDialog.SelectScreenID).DefSize
        ScreenControls(TmpDialog.SelectScreenID).Visible = True
    End Sub
#End Region

#Region "保存修改"
    ''' <summary>
    ''' 窗口信息
    ''' </summary>
    Private Sub GroupBox3_Leave(sender As Object, e As EventArgs) Handles GroupBox3.Leave
        Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList.Item(WindowId)
        With TmpWindowInfo
            .Remark = TextBox1.Text

            .Location.X = NumericUpDown1.Value
            .Location.Y = NumericUpDown2.Value

            .ZoomPix.Width = NumericUpDown3.Value
            .ZoomPix.Height = NumericUpDown4.Value
        End With
    End Sub

    ''' <summary>
    ''' 改变屏幕位置
    ''' </summary>
    Public Sub ChangeScreenLocation()
        If TextBox2.Text = "" Then
            Exit Sub
        End If

        sysInfo.Schedule.ScreenLocations(Val(TextBox2.Text)) = New Point(NumericUpDown6.Value, NumericUpDown5.Value)
        ScreenControls(Val(TextBox2.Text)).Location = sysInfo.Schedule.ScreenLocations(Val(TextBox2.Text))
    End Sub

    Private Sub NumericUpDown6_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown6.ValueChanged
        ChangeScreenLocation()
    End Sub

    Private Sub NumericUpDown5_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown5.ValueChanged
        ChangeScreenLocation()
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.Label1.Text = .GetS("Remark")
            Me.Label2.Text = .GetS("Location")
            Me.Label3.Text = .GetS("X")
            Me.Label4.Text = .GetS("Y")
            Me.Label6.Text = .GetS("Actual Pixels")
            Me.Label7.Text = .GetS("Zoom Pixels")
            Me.Label9.Text = .GetS("Y")
            Me.Label10.Text = .GetS("X")
            Me.Label11.Text = .GetS("Location")
            Me.GroupBox2.Text = .GetS("Screen Setting")
            Me.Label5.Text = .GetS("ID")
            Me.GroupBox3.Text = .GetS("Window Setting")
            Me.ToolStripLabel1.Text = .GetS("Zoom 100%")
            Me.ToolStripButton1.Text = .GetS("Zoom In")
            Me.ToolStripButton2.Text = .GetS("Zoom Out")
            Me.ToolStripButton3.Text = .GetS("Add Screen")
            Me.ToolStripButton4.Text = .GetS("Clear")
            Me.DeleteScreenToolStripMenuItem.Text = .GetS("Delete Screen")
        End With
    End Sub
#End Region
End Class