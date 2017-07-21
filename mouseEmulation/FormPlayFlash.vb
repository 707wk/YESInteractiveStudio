Public Class FormPlayFlash
    '发送消息
    Private Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32
    '鼠标事件常量 　　
    Private Const WM_LBUTTONDBLCLK = &H203
    Private Const WM_LBUTTONDOWN = &H201
    Private Const WM_LBUTTONUP = &H202
    Private Const WM_MBUTTONDBLCLK = &H209
    Private Const WM_MBUTTONDOWN = &H207
    Private Const WM_MBUTTONUP = &H208
    Private Const WM_RBUTTONDBLCLK = &H206
    Private Const WM_RBUTTONDOWN = &H204
    Private Const WM_RBUTTONUP = &H205

    '播放控件变量
    Dim playFlash1 As AxShockwaveFlashObjects.AxShockwaveFlash
    '触摸板宽度
    Public touchPieceWidth As Integer
    '触摸板高度
    Public touchPieceHeight As Integer
    '运行模式
    'Public runMode As Integer
    '电容值
    'Public capacitance As Integer

    Private Sub FormPlayFlash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Me.StartPosition = FormStartPosition.Manual

        playFlash1 = New AxShockwaveFlashObjects.AxShockwaveFlash
        playFlash1.Dock = DockStyle.Fill
        Me.Controls.Add(playFlash1)

        'playFlash1.Hide()
    End Sub

    '播放flash
    Public Sub play(swfUrl As String)
        'Try
        playFlash1.StopPlay()
        'Me.Controls.Remove(playFlash1)
        'Catch ex As Exception
        'End Try

        'playFlash1 = New AxShockwaveFlashObjects.AxShockwaveFlash
        'Me.Controls.Add(playFlash1)
        'playFlash1.Dock = DockStyle.Fill
        playFlash1.SAlign = 1
        playFlash1.ScaleMode = 2
        playFlash1.Movie = swfUrl
        playFlash1.Play()
    End Sub

    '设置显示位置、尺寸
    Public Sub setLocation(x As Integer, y As Integer, width As Integer, height As Integer)
        'putlog($"{x} {y} {width} {height}")
        Me.Location = New Point(x, y)
        'putlog($"x:{x} y:{y}")
        'Me.Location = New Point(x, y)
        Me.Size = New Size(width, height)
        'Me.Left = x
        'Me.Top = y

        'putlog($"{Me.Size.Width} {Me.Size.Height}")
        'Me.Width = width
        'Me.Height = height
        'playFlash1.Show()
        'putlog($"{Me.Left} {Me.Top} {Me.Size.Width} {Me.Size.Height}")
    End Sub

    '切换为播放模式
    Public Sub switchPlayMode()
        Me.Refresh()

        '显示播放控件
        'Try
        playFlash1.Show()
        playFlash1.Movie = playFlash1.Movie
        playFlash1.Play()
        'Catch ex As Exception
        'End Try

        'runMode = 0
    End Sub

    '切换为测试模式
    Public Sub switchTestMode()
        'touchPieceWidth = pTouchPieceWidth
        'touchPieceHeight = pTouchPieceHeight

        Me.Refresh()

        '隐藏播放控件
        'Try
        'playFlash1.Back()
        'playFlash1.Stop()
        playFlash1.Hide()
        'Catch ex As Exception
        'End Try

        Me.BackColor = Color.Black 'Control.DefaultBackColor

        Me.Refresh()

        Dim g As Graphics = Me.CreateGraphics
        Dim mpen As New Pen(Color.Green)
        For i As Integer = touchPieceHeight To Me.Height Step touchPieceHeight
            g.DrawLine(mpen, 0, i, Me.Width, i)
        Next

        For i As Integer = touchPieceWidth To Me.Width Step touchPieceWidth
            g.DrawLine(mpen, i, 0, i, Me.Height)
        Next

        'runMode = 2
    End Sub

    '切换为测试模式(显示电容)
    Public Sub switchTestModeWithValue()
        'touchPieceWidth = pTouchPieceWidth
        'touchPieceHeight = pTouchPieceHeight

        Me.Refresh()

        '隐藏播放控件
        'Try
        'playFlash1.Back()
        'playFlash1.Stop()
        playFlash1.Hide()
        'Catch ex As Exception
        'End Try

        Me.BackColor = Color.Black 'Control.DefaultBackColor

        Me.Refresh()

        Dim g As Graphics = Me.CreateGraphics
        Dim mpen As New Pen(Color.Green)
        For i As Integer = touchPieceHeight To Me.Height Step touchPieceHeight
            g.DrawLine(mpen, 0, i, Me.Width, i)
        Next

        For i As Integer = touchPieceWidth To Me.Width Step touchPieceWidth
            g.DrawLine(mpen, i, 0, i, Me.Height)
        Next

        'runMode = 3
    End Sub

    '切换为黑屏
    Public Sub switchBlankScreenMode()
        Me.Refresh()

        '隐藏播放控件
        'Try
        'playFlash1.Back()
        'playFlash1.Stop()
        playFlash1.Hide()
        'Catch ex As Exception
        'End Try

        Me.BackColor = Color.Black

        'runMode = 4
    End Sub

    '模拟点击
    Public Sub MousesimulationClick(tX As Integer, tY As Integer, tDataArray As Byte(), tIndex As Integer)
        '黑屏模式不点击
        'If runMode <> 0 And runMode <> 1 And runMode <> 2 And runMode <> 3 Then
        '    Exit Sub
        'End If

        If runMode = 0 Then
            '模拟点击
            For i As Integer = 0 To 4 - 1
                For j As Integer = 0 To 4 - 1
                    If (tDataArray(tIndex + i * 4 + j) And &H80) <> &H80 Then
                        Continue For
                    End If

                    Dim txp As Int16 = tX + j * touchPieceWidth + touchPieceWidth \ 2
                    Dim typ As Int32 = tY + i * touchPieceHeight + touchPieceHeight \ 2
                    Dim ttp As Int32 = txp + (typ << 16)

                    PostMessage(Me.playFlash1.Handle,
                                WM_LBUTTONDOWN,
                                0,
                                ttp)
                    'putlog($"x:{txp} y:{typ} {(txp + (typ << 16))}")
                    PostMessage(Me.playFlash1.Handle,
                                WM_LBUTTONUP,
                                0,
                                ttp)

                    'screenMain.showFlash.capacitance = dataArray(index + i * 4 + j) And &H7F
                    '移动鼠标然后点击
                    'mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move Or MouseEvent.LeftButtonDown Or MouseEvent.LeftButtonUp,
                    '                (screenMain.x + tX + j * touchPieceWidth + touchPieceWidth / 2) * 65536 / Screen.PrimaryScreen.Bounds.Width,
                    '                (screenMain.y + tY + i * touchPieceWidth + touchPieceHeight / 2) * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                    '回原位
                    'mouse_event(MouseEvent.AbsoluteLocation Or MouseEvent.Move,
                    '                oldx * 65536 / Screen.PrimaryScreen.Bounds.Width,
                    '                oldy * 65536 / Screen.PrimaryScreen.Bounds.Height, 0, 0)
                Next
            Next
        ElseIf RunMode = 2 Then
            '测试
            Dim g As Graphics = Me.CreateGraphics
            Dim mpen As New SolidBrush(Color.Green)
            Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)

            For i As Integer = 0 To 4 - 1
                For j As Integer = 0 To 4 - 1


                    If (tDataArray(tIndex + i * 4 + j) And &H80) <> &H80 Then

                        'mpen.Color = Color.Black
                        'g.DrawString($"{(tDataArray(tIndex + i * 4 + j))}",'And &H7F)}",
                        '         myFont,
                        '         mpen,
                        '         tX + j * touchPieceWidth,
                        '         tY + i * touchPieceHeight)

                        Continue For
                    End If

                    'mpen.Color = Color.Green
                    'g.FillRectangle(mpen,
                    '     tX + j * touchPieceWidth,
                    '     tY + i * touchPieceHeight,
                    '     touchPieceWidth,
                    '     touchPieceHeight)

                    g.DrawString($"√",'And &H7F)}",
                                 myFont,
                                 mpen,
                                 tX + j * touchPieceWidth,
                                 tY + i * touchPieceHeight)
                Next
            Next
        ElseIf RunMode = 3 Then
            '测试(显示电容值)
            Dim g As Graphics = Me.CreateGraphics
            Dim mpen As New SolidBrush(Color.Green)
            Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)

            For i As Integer = 0 To 4 - 1
                For j As Integer = 0 To 4 - 1


                    If (tDataArray(tIndex + i * 4 + j) And &H80) <> &H80 Then

                        'mpen.Color = Color.Black
                        g.DrawString($"{(tDataArray(tIndex + i * 4 + j))}",'And &H7F)}",
                                 myFont,
                                 mpen,
                                 tX + j * touchPieceWidth,
                                 tY + i * touchPieceHeight)

                        Continue For
                    End If

                    'mpen.Color = Color.Green
                    g.FillRectangle(mpen,
                         tX + j * touchPieceWidth,
                         tY + i * touchPieceHeight,
                         touchPieceWidth,
                         touchPieceHeight)
                Next
            Next
        End If

    End Sub
End Class