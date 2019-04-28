Public Class ScreenButton
    Inherits Button
    ''' <summary>
    ''' 计算位置用的坐标
    ''' </summary>
    Private oldPoint As Point
    ''' <summary>
    ''' 操作类型 0无 1移动
    ''' </summary>
    Private OperateType As Integer

    ''' <summary>
    ''' 屏幕ID
    ''' </summary>
    Public ScreenId As Integer

    Public Sub New()
        FlatStyle = FlatStyle.Flat
        TextAlign = ContentAlignment.TopLeft
        BackColor = Color.FromArgb(0, 0, 0)
        ForeColor = Color.FromArgb(68, 233, 54)
    End Sub

    Private Sub ScreenSizeClass_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Me.BringToFront()

        '左键拖动
        If e.Button <> MouseButtons.Left Then
            Exit Sub
        End If

        OperateType = 1

        oldPoint = New Point(e.X, e.Y)
    End Sub

    Private Sub ScreenSizeClass_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If OperateType = 0 Then
            Exit Sub
        End If

        Dim TmpPoint As Point = New Point(Me.Location.X + e.X - oldPoint.X,
                             Me.Location.Y + e.Y - oldPoint.Y)
        '设定移动范围
        '20180910修复
        If TmpPoint.X < 0 Then
            TmpPoint = New Point(0, TmpPoint.Y)
        End If
        If TmpPoint.Y < 0 Then
            TmpPoint = New Point(TmpPoint.X, 0)
        End If

        Me.Location = TmpPoint

        sysInfo.Schedule.ScreenList(ScreenId).Loaction = Me.Location

        '        With sysInfo.ScreenList(ScreenId)
        '            Me.Text = $"Screen {ScreenId}
        'Size: { .DefSize.Width},{ .DefSize.Height}"
        '        End With
    End Sub

    Private Sub ScreenSizeClass_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        OperateType = 0

        'sysInfo.Schedule.ScreenLocations(ScreenId) = Me.Location
    End Sub
End Class
