Public Class FormPlay
    '发送消息
    Private Declare Function PostMessage Lib "user32" Alias "PostMessageA" (ByVal hwnd As Int32, ByVal wMsg As Int32, ByVal wParam As Int32, ByVal lParam As Int32) As Int32
    Private Declare Function CreateCompatibleDC Lib "GDI32" (ByVal hDC As Integer) As Integer
    Private Declare Function CreateCompatibleBitmap Lib "GDI32" (ByVal hDC As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer) As Integer
    Private Declare Function SelectObject Lib "GDI32" (ByVal hDC As Integer, ByVal hObject As Integer) As Integer
    Private Declare Function BitBlt Lib "GDI32" (ByVal srchDC As Integer, ByVal srcX As Integer, ByVal srcY As Integer, ByVal srcW As Integer, ByVal srcH As Integer, ByVal desthDC As Integer, ByVal destX As Integer, ByVal destY As Integer, ByVal op As Integer) As Integer
    Private Declare Function DeleteDC Lib "GDI32" (ByVal hDC As Integer) As Integer
    Private Declare Function DeleteObject Lib "GDI32" (ByVal hObj As Integer) As Integer
    Declare Function GetDC Lib "user32" Alias "GetDC" (ByVal hwnd As Integer) As Integer
    Const SRCCOPY As Integer = &HCC0020
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
        '隐藏播放控件
        AxShockwaveFlash1.Hide()
        Me.BackColor = Color.Black
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
            'Me.BackColor = Color.White '强制测试模式

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

                If (TempI > ((ScreenAnti - 1) * 128)) And (TempS > ((ScreenAntiS - 1) * 128)) Then ' (TempI2 >= 256) AndInteractMode = 2 And 
                    PostMessage(Me.AxShockwaveFlash1.Handle,
                              WM_LBUTTONDOWN,
                              0,
                               ttp)
                    PostMessage(Me.AxShockwaveFlash1.Handle,
                               WM_LBUTTONUP,
                               0,
                               ttp)
                    If recordDataFlage = True Then recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-点击事件-{txp}-{txp}-value-{value}-tempi-{TempI}--runMode-{runMode }") '$"时间{Format(Now(), "yyyyMMddHHmmss")}-
                End If
                ' If InteractMode = 1 Or InteractMode = 3 Then
                'PostMessage(Me.AxShockwaveFlash1.Handle,
                '        WM_LBUTTONDOWN,
                '        0,
                '        ttp)
                ' PostMessage(Me.AxShockwaveFlash1.Handle,
                '  WM_LBUTTONUP,
                '  0,
                '  ttp)
                ' End If

                'PostMessage(Me.AxShockwaveFlash1.Handle,
                'WM_LBUTTONDOWN,
                '0,
                'ttp)
                'PostMessage(Me.AxShockwaveFlash1.Handle,
                'WM_LBUTTONUP,
                '0,
                'ttp)

            ElseIf runMode = 1 Then
                    '测试
                    If (value And &H80) <> &H80 Then
                        Exit Sub
                    End If

                    Dim g As Graphics = Me.CreateGraphics
                    Dim mpen As New SolidBrush(Color.Green)
                    Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)

                If (TempI > ((ScreenAnti - 1) * 128)) And (TempS > ((ScreenAntiS - 1) * 128)) Then ' (TempI2 >= 256) And 增加 0825  InteractMode = 2 And
                    g.DrawString($"√",
                                myFont,
                                mpen,
                               tX * touchPieceWidth + touchPieceWidth \ 2 - 8,
                               tY * touchPieceHeight + touchPieceHeight \ 2 - 8)
                    If recordDataFlage = True Then recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-√-{tX * touchPieceWidth + touchPieceWidth \ 2 - 8}-{tY * touchPieceHeight + touchPieceHeight \ 2 - 8}-value-{value}-tempi-{TempI}-runMode-{runMode }")
                End If
                ' If InteractMode = 1 Or InteractMode = 3 Then
                ' g.DrawString($"√",
                'myFont,
                'mpen,
                'tX * touchPieceWidth + touchPieceWidth \ 2 - 8,
                'tY * touchPieceHeight + touchPieceHeight \ 2 - 8)
                'If recordDataFlage = True Then recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-√-{tX * touchPieceWidth + touchPieceWidth \ 2 - 8}-{tY * touchPieceHeight + touchPieceHeight \ 2 - 8}-value-{value}-tempi-{TempI}")
                ' End If
                'g.DrawString($"√",
                'myFont,
                'mpen,
                'tX * touchPieceWidth + touchPieceWidth \ 2 - 8,
                'tY * touchPieceHeight + touchPieceHeight \ 2 - 8)




            ElseIf runMode = 4 Then
                    '测试(显示电容值)
                    Dim bitmap2 As New Bitmap(Me.Width, Me.Height) '保存图，也就是绘制的大小。
                Dim g As Graphics = Me.CreateGraphics
                Dim mpen As New SolidBrush(Color.Green)
                Dim myFont As Font = New Font("宋体", 12, FontStyle.Regular)


                If (value And &H80) <> &H80 Then
                    g.DrawString($"{value And &H7F}",
                                 myFont,
                                 mpen,
                                 tX * touchPieceWidth + (touchPieceWidth - 16) / 2 - 2,
                                 tY * touchPieceHeight + (touchPieceHeight - 16) / 2 - 1)
                    Exit Sub
                End If


                Dim txp1 As Int16 = tX * touchPieceWidth + 1
                Dim typ1 As Int32 = tY * touchPieceHeight + 1

                '抓图的部分
                Dim hDC, hMDC As Integer
                Dim hBMP, hBMPOld As Integer
                Dim sw, sh As Integer
                hDC = GetDC(Me.Handle)
                hMDC = CreateCompatibleDC(hDC)
                sw = Me.Width
                sh = Me.Height

                hBMP = CreateCompatibleBitmap(hDC, sw, sh)
                hBMPOld = SelectObject(hMDC, hBMP)
                BitBlt(hMDC, 0, 0, sw, sh, hDC, 0, 0, SRCCOPY)
                hBMP = SelectObject(hMDC, hBMPOld)
                Dim bmp As Bitmap = Image.FromHbitmap(New IntPtr(hBMP))
                DeleteDC(hDC)
                DeleteDC(hMDC)
                DeleteObject(hBMP)
                '到此，颜色保存到BMP中去了
                Dim colorS As Color = bmp.GetPixel(txp1, typ1)
                If colorS = Color.FromArgb(255, 0, 0, 0) Then '黑色为（255,0,0，0）‘白色255, 255, 255, 255
                    mpen.Color = Color.FromArgb(255, 100, 0, 0)
                End If
                If colorS = Color.FromArgb(255, 100, 0, 0) Then
                    mpen.Color = Color.FromArgb(255, 0, 100, 0)
                End If
                If colorS = Color.FromArgb(255, 0, 100, 0) Then
                    mpen.Color = Color.FromArgb(255, 0, 0, 100)
                End If
                If colorS = Color.FromArgb(255, 0, 0, 100) Then
                    mpen.Color = Color.FromArgb(255, 100, 0, 0)
                End If
                If (TempI > ((ScreenAnti - 1) * 128)) And (TempS > ((ScreenAntiS - 1) * 128)) Then '(TempI2 >= 256) And InteractMode = 2 And 
                    g.FillRectangle(mpen,
                    tX * touchPieceWidth + 1,
                    tY * touchPieceHeight + 1,
                    touchPieceWidth - 1,
                    touchPieceHeight - 1)

                    '记录点击的时间事件和地砖屏一共触发点击的次数
                    If recordDataFlage Then
                        recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-方块-{tX * touchPieceWidth + 1}-{tY * touchPieceHeight + 1}-value-{value}-tempi-{TempI}-runMode-{runMode} ") '
                        Dim tmp3 As New ClassIni
                        Dim tmpI As Integer
                        Try
                            tmpI = CInt(tmp3.GetINI("ScreenInteraction", $"X-Y:{txp1}-{typ1}", "0", ".\InteractionNum.ini"))
                            tmpI += 1
                            tmp3.WriteINI("ScreenInteraction", $"X-Y:{txp1}-{typ1}", CStr(tmpI), ".\InteractionNum.ini")
                        Catch ex As Exception
                            tmp3.WriteINI("ScreenInteraction", $"X-Y:{txp1}-{typ1}", "1", ".\InteractionNum.ini")
                        End Try

                    End If

                End If
                'If InteractMode = 1 Or InteractMode = 3 Then
                ' g.FillRectangle(mpen,
                ' tX * touchPieceWidth + 1,
                'tY * touchPieceHeight + 1,
                'touchPieceWidth - 1,
                'touchPieceHeight - 1)

                '  End If
                ' If recordDataFlage = True Then recordDataFile.WriteLine($"时间-{Format(Now(), "yyyyMMddHHmmss")}-方块-{tX * touchPieceWidth + 1}-{tY * touchPieceHeight + 1}-value-{value}-tempi-{TempI}")
            End If
            End If
    End Sub
    Dim colorList As Color() = {Color.Green, Color.Blue, Color.Yellow}

    Private Sub AxShockwaveFlash1_Enter(sender As Object, e As EventArgs) Handles AxShockwaveFlash1.Enter

    End Sub
End Class