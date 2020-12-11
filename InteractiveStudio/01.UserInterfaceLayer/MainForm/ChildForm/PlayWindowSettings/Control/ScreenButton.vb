Public Class ScreenButton
    Inherits Button

    ''' <summary>
    ''' 屏幕ID
    ''' </summary>
    Public ScreenId As Integer

    Public Sub New(id As Integer)
        ScreenId = id

        FlatStyle = FlatStyle.Flat
        TextAlign = ContentAlignment.TopLeft
        BackColor = Color.FromArgb(50, 50, 50)
        ForeColor = Color.LightGreen
        Font = New Font("微软雅黑", 9)

        Size = AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems(ScreenId).SizeOfOriginal
        Location = AppSettingHelper.GetInstance.DisplayingScheme.NovaStarScreenItems(ScreenId).LocationOfOriginal

        ScreenButton_Move()
    End Sub

    Private Sub ScreenButton_Move(Optional sender As Object = Nothing,
                                  Optional e As EventArgs = Nothing) Handles Me.Move
        Text = $"Screen {ScreenId}
Size:
     Width={Me.Size.Width},Height={Me.Size.Height}
Location:
     X={Me.Location.X},Y={Me.Location.Y}"
    End Sub

#Region "鼠标移动"
    ''' <summary>
    ''' 按下时鼠标的屏幕坐标
    ''' </summary>
    Private oldMousePoint As Point
    '按下时控件的坐标
    Private oldMyselfPoint As Point
    ''' <summary>
    ''' 操作类型 0无 1移动
    ''' </summary>
    Private OperateType As Integer

    Private Sub ScreenSizeClass_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Me.BringToFront()
        Me.Focus()

        '左键拖动
        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        OperateType = 1
        oldMyselfPoint = Me.Location
        oldMousePoint = Control.MousePosition

    End Sub

    Private Sub ScreenSizeClass_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If OperateType = 0 Then
            Exit Sub
        End If

        Dim TmpPoint As Point = New Point(oldMyselfPoint.X + Control.MousePosition.X - oldMousePoint.X,
                                          oldMyselfPoint.Y + Control.MousePosition.Y - oldMousePoint.Y)
        '设定移动范围
        If TmpPoint.X < 0 Then
            TmpPoint = New Point(0, TmpPoint.Y)
        End If
        If TmpPoint.Y < 0 Then
            TmpPoint = New Point(TmpPoint.X, 0)
        End If

        Me.Location = TmpPoint
    End Sub

    Private Sub ScreenSizeClass_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        OperateType = 0
        oldMyselfPoint = Me.Location
    End Sub

#End Region

#Region "键盘移动"
    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, keyData As Keys) As Boolean
        Select Case keyData
            Case Keys.Up
                If Me.Top > 0 Then Me.Top -= 1
                Return True
            Case Keys.Down
                Me.Top += 1
                Return True
            Case Keys.Left
                If Me.Left > 0 Then Me.Left -= 1
                Return True
            Case Keys.Right
                Me.Left += 1
                Return True
        End Select

        Return MyBase.ProcessCmdKey(msg, keyData)

    End Function
#End Region

End Class
