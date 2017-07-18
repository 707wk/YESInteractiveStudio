Public Class FormPlayFlash
    '播放控件变量
    Dim playFlash1 As AxShockwaveFlashObjects.AxShockwaveFlash
    '触摸板宽度
    Dim touchPieceWidth As Integer
    '触摸板高度
    Dim touchPieceHeight As Integer
    '运行模式
    Public runMode As Integer

    Private Sub FormPlayFlash_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.TopMost = True
        Me.StartPosition = FormStartPosition.Manual

        playFlash1 = New AxShockwaveFlashObjects.AxShockwaveFlash
        playFlash1.Dock = DockStyle.Fill
        Me.Controls.Add(playFlash1)
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
        Me.Location = New Point(x, y)
        'putlog($"x:{x} y:{y}")
        'Me.Location = New Point(x, y)
        Me.Size = New Size(width, height)
        'Me.Left = x
        'Me.Top = y

        'Me.Width = width
        'Me.Height = height

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

        runMode = 0
    End Sub

    '切换为测试模式
    Public Sub switchTestMode(pTouchPieceWidth As Integer, pTouchPieceHeight As Integer)
        touchPieceWidth = pTouchPieceWidth
        touchPieceHeight = pTouchPieceHeight

        Me.Refresh()

        '隐藏播放控件
        'Try
        'playFlash1.Back()
        'playFlash1.Stop()
        playFlash1.Hide()
        'Catch ex As Exception
        'End Try

        Me.BackColor = Control.DefaultBackColor

        runMode = 1
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

        runMode = 2
    End Sub

    Private Sub FormPlayFlash_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        '黑屏模式不点击
        If runMode = 2 Then
            Exit Sub
        End If

        Dim g As Graphics = Me.CreateGraphics
        Dim mpen As New SolidBrush(Color.Green)

        Me.Text = $"{(e.X \ touchPieceWidth * touchPieceWidth)} {(e.Y \ touchPieceHeight * touchPieceHeight)}"

        g.FillRectangle(mpen,
                        (e.X \ touchPieceWidth) * touchPieceWidth,
                        (e.Y \ touchPieceHeight) * touchPieceHeight,
                        touchPieceWidth,
                        touchPieceHeight)
    End Sub
End Class