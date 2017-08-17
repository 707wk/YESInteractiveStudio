Public Class FormPlay
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

    '随机变量
    Dim r As New Random
    '触摸板宽度
    Public touchPieceWidth As Integer
    '触摸板高度
    Public touchPieceHeight As Integer

    Private Sub FormPlay_Load(sender As Object, e As EventArgs) Handles Me.Load
        Me.TopMost = True
        Me.StartPosition = FormStartPosition.Manual
    End Sub

    '关闭窗体
    Public Delegate Sub closeDialogCallback(ByVal swfUrl As String)
    Public Sub closeDialog(ByVal swfUrl As String)
        If Me.InvokeRequired Then
            Dim d As New closeDialogCallback(AddressOf closeDialog)
            Me.Invoke(d, New Object() {swfUrl})
        Else
            Me.Close()
        End If
    End Sub

    '播放flash
    Public Delegate Sub playCallback(ByVal swfUrl As String)
    Public Sub play(ByVal swfUrl As String)
        If Me.InvokeRequired Then
            Dim d As New playCallback(AddressOf play)
            Me.Invoke(d, New Object() {swfUrl})
        Else
            AxShockwaveFlash1.StopPlay()
            AxShockwaveFlash1.SAlign = 1
            AxShockwaveFlash1.ScaleMode = 2
            AxShockwaveFlash1.Movie = swfUrl
            AxShockwaveFlash1.Play()
        End If
    End Sub

    '设置显示位置、尺寸
    Public Delegate Sub setLocationCallback(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
    Public Sub setLocation(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        If Me.InvokeRequired Then
            Dim d As New setLocationCallback(AddressOf setLocation)
            Me.Invoke(d, New Object() {x, y, width, height})
        Else
            Me.Location = New Point(x, y)
            Me.Size = New Size(width, height)
        End If
    End Sub

    '切换为播放模式
    Public Delegate Sub switchPlayModeCallback(ByVal text As String)
    Public Sub switchPlayMode(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New switchPlayModeCallback(AddressOf switchPlayMode)
            Me.Invoke(d, New Object() {text})
        Else
            Me.Refresh()

            '显示播放控件
            AxShockwaveFlash1.Show()
            AxShockwaveFlash1.Movie = AxShockwaveFlash1.Movie
            AxShockwaveFlash1.Play()
        End If
    End Sub

    '切换为测试模式
    Public Delegate Sub switchTestModeCallback(ByVal text As String)
    Public Sub switchTestMode(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New switchTestModeCallback(AddressOf switchTestMode)
            Me.Invoke(d, New Object() {text})
        Else
            '隐藏播放控件
            AxShockwaveFlash1.Hide()

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
        End If
    End Sub

    '切换为测试模式(显示电容)
    Public Delegate Sub switchTestModeWithValueCallback(ByVal text As String)
    Public Sub switchTestModeWithValue(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New switchTestModeWithValueCallback(AddressOf switchTestModeWithValue)
            Me.Invoke(d, New Object() {text})
        Else
            '隐藏播放控件
            AxShockwaveFlash1.Hide()

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
        End If
    End Sub

    '切换为黑屏
    Public Delegate Sub switchBlankScreenModeCallback(ByVal text As String)
    Public Sub switchBlankScreenMode(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New switchBlankScreenModeCallback(AddressOf switchBlankScreenMode)
            Me.Invoke(d, New Object() {text})
        Else
            Me.Refresh()
            '隐藏播放控件
            AxShockwaveFlash1.Hide()
            Me.BackColor = Color.Black
        End If
    End Sub

    '模拟鼠标点击消息
    Public Delegate Sub MousesimulationClickCallback(ByVal tX As Integer, ByVal tY As Integer, ByVal value As Integer)
    Public Sub MousesimulationClick(ByVal tX As Integer, ByVal tY As Integer, ByVal value As Integer)
        If Me.InvokeRequired Then
            Dim d As New MousesimulationClickCallback(AddressOf MousesimulationClick)
            Me.Invoke(d, New Object() {tX, tY, value})
        Else
            If runMode = 0 Then
                '模拟点击
                'If (value And &H80) <> &H80 Then
                '    Exit Sub
                'End If

                Dim txp As Int16 = tX * touchPieceWidth + touchPieceWidth \ 2
                Dim typ As Int32 = tY * touchPieceHeight + touchPieceHeight \ 2
                Dim ttp As Int32 = txp + (typ << 16)

                PostMessage(Me.AxShockwaveFlash1.Handle,
                            WM_LBUTTONDOWN,
                            0,
                            ttp)
                PostMessage(Me.AxShockwaveFlash1.Handle,
                            WM_LBUTTONUP,
                            0,
                            ttp)

            ElseIf runMode = 1 Then
                '测试
                If (value And &H80) <> &H80 Then
                    Exit Sub
                End If

                Dim g As Graphics = Me.CreateGraphics
                Dim mpen As New SolidBrush(Color.Green)
                Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)

                g.DrawString($"√",
                             myFont,
                             mpen,
                             tX * touchPieceWidth,
                             tY * touchPieceHeight)

            ElseIf runMode = 4 Then
                '测试(显示电容值)
                Dim g As Graphics = Me.CreateGraphics
                Dim mpen As New SolidBrush(Color.Green)
                Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)


                If (value And &H80) <> &H80 Then
                    g.DrawString($"{value And &H7F}",
                                 myFont,
                                 mpen,
                                 tX * touchPieceWidth,
                                 tY * touchPieceHeight)
                    Exit Sub
                End If

                mpen.Color = Color.FromArgb(255, r.Next(0, 255), r.Next(0, 255), r.Next(0, 255)) 'colorList(r.Next(0, 3)) '
                g.FillRectangle(mpen,
                                tX * touchPieceWidth,
                                tY * touchPieceHeight,
                                touchPieceWidth,
                                touchPieceHeight)
            End If
        End If
    End Sub
    Dim colorList As Color() = {Color.Green, Color.Blue, Color.Yellow}
End Class