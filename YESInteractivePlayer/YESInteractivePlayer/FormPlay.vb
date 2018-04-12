Public Class FormPlay
    '''' <summary>
    '''' 幕布ID
    '''' </summary>
    'Public curtainId As Integer
    ''' <summary>
    ''' 幕布所在位置索引
    ''' </summary>
    Public curtainListId As Integer

    ''' <summary>
    ''' 背景
    ''' </summary>
    Private gBack As Graphics
    ''' <summary>
    ''' 背景刷
    ''' </summary>
    Private gPen As Pen
    ''' <summary>
    ''' 背景刷
    ''' </summary>
    Private gBrush As SolidBrush
    ''' <summary>
    ''' 背景字体
    ''' </summary>
    Private gFont As Font

    ''' <summary>
    ''' 播放器控件
    ''' </summary>
    Private playFlash As AxShockwaveFlashObjects.AxShockwaveFlash
    ''' <summary>
    ''' 正在播放的文件
    ''' </summary>
    Private movie As String

    '发送消息
    Private Declare Function PostMessage Lib "user32" Alias _
        "PostMessageA" (ByVal hwnd As Int32,
                        ByVal wMsg As Int32,
                        ByVal wParam As Int32,
                        ByVal lParam As Int32) As Int32
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
    Private Const WM_MOUSEMOVE = &H200

    Private Sub FormPlay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '窗体置顶
        'Me.TopMost = True

        '隐藏播放控件
        'AxShockwaveFlash1.Hide()
        'AxShockwaveFlash1.BackgroundColor = 0

        Me.BackColor = Color.Black
        '初始化绘图参数
        Me.gBack = Me.CreateGraphics
        Me.gPen = New Pen(Color.Green)
        Me.gBrush = New SolidBrush(Color.Green)
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.ZoomProportion), FontStyle.Regular)

        ''获取幕布在list中的索引
        'For i As Integer = 0 To sysInfo.curtainList.Count
        '    If sysInfo.curtainList.Item(i).id = Me.curtainId Then
        '        listIndex = i
        '        Exit For
        '    End If
        'Next
    End Sub

    ''' <summary>
    ''' 关闭窗体
    ''' </summary>
    Public Delegate Sub closeDialogCallback(ByVal tmpN As Boolean)
    Public Sub CloseDialog(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New closeDialogCallback(AddressOf CloseDialog), New Object() {tmpN})
            Exit Sub
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' 播放flash
    ''' </summary>
    Public Delegate Sub playCallback(ByVal swfUrl As String)
    Public Sub Play(ByVal swfUrl As String)
        If Me.InvokeRequired Then
            Me.Invoke(New playCallback(AddressOf Play), New Object() {swfUrl})
            Exit Sub
        End If

        'AxShockwaveFlash1.Show()
        'AxShockwaveFlash1.StopPlay()
        If playFlash IsNot Nothing Then
            Me.Controls.Remove(playFlash)
            playFlash.Dispose()
            playFlash = Nothing
        End If

        playFlash = New AxShockwaveFlashObjects.AxShockwaveFlash
        playFlash.Dock = DockStyle.Fill
        Me.Controls.Add(playFlash)

        playFlash.AlignMode = 5 '对齐方式
        playFlash.ScaleMode = 2 '缩放模式
        playFlash.Quality = 0 '画面质量
        playFlash.BackgroundColor = 0
        playFlash.Movie = swfUrl
        movie = swfUrl
        'AxShockwaveFlash1.Play()
    End Sub

    ''' <summary>
    ''' 设置显示位置、尺寸
    ''' </summary>
    Public Delegate Sub setLocationCallback(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
    Public Sub SetLocation(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New setLocationCallback(AddressOf SetLocation), New Object() {x, y, width, height})
            Exit Sub
        End If

        Me.Location = New Point(x, y)
        Me.Size = New Size(width, height)
        Me.Width = width
        Me.Height = height
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.ZoomProportion), FontStyle.Regular)

        Me.gBack = Me.CreateGraphics
    End Sub

    ''' <summary>
    ''' 切换为播放模式
    ''' </summary>
    Public Delegate Sub switchPlayModeCallback(ByVal tmpN As Boolean)
    Public Sub SwitchPlayMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchPlayModeCallback(AddressOf SwitchPlayMode), New Object() {tmpN})
            Exit Sub
        End If

        Me.Refresh()
        '显示播放控件
        'playFlash.Show()
        'playFlash.Movie = playFlash.Movie
        'playFlash.Play()
        If playFlash Is Nothing Then
            playFlash = New AxShockwaveFlashObjects.AxShockwaveFlash
            playFlash.Dock = DockStyle.Fill
            Me.Controls.Add(playFlash)

            playFlash.AlignMode = 5 '对齐方式
            playFlash.ScaleMode = 2 '缩放模式
            playFlash.Quality = 0 '画面质量
            playFlash.BackgroundColor = 0
            playFlash.Movie = movie
        End If


    End Sub

    ''' <summary>
    ''' 切换为测试模式
    ''' </summary>
    Public Delegate Sub switchTestModeCallback(ByVal tmpN As Boolean)
    Public Sub SwitchTestMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchTestModeCallback(AddressOf SwitchTestMode), New Object() {tmpN})
            Exit Sub
        End If

        '隐藏播放控件
        'playFlash.Hide()
        If playFlash IsNot Nothing Then
            Me.Controls.Remove(playFlash)
            playFlash.Dispose()
            playFlash = Nothing
        End If

        Me.BackColor = Color.Black
        Me.Refresh()

        For Each tmp In sysInfo.CurtainList.Item(curtainListId).ScreenList
            '缩放后触摸单元高度
            Dim touchPieceHeight As Integer = sysInfo.ScreenList(tmp).TouchPieceHeight
            For i As Integer = 0 To sysInfo.ScreenList(tmp).Height Step touchPieceHeight
                gBack.DrawLine(gPen,
                               sysInfo.ScreenList(tmp).X + 0,
                               sysInfo.ScreenList(tmp).Y + i,
                               sysInfo.ScreenList(tmp).X + sysInfo.ScreenList(tmp).Width,
                               sysInfo.ScreenList(tmp).Y + i)
            Next

            '缩放后触摸单元宽度
            Dim touchPieceWidth As Integer = sysInfo.ScreenList(tmp).TouchPieceWidth
            For i As Integer = 0 To sysInfo.ScreenList(tmp).Width Step touchPieceWidth
                gBack.DrawLine(gPen,
                               sysInfo.ScreenList(tmp).X + i,
                               sysInfo.ScreenList(tmp).Y + 0,
                               sysInfo.ScreenList(tmp).X + i,
                               sysInfo.ScreenList(tmp).Y + sysInfo.ScreenList(tmp).Height)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 切换为黑屏
    ''' </summary>
    Public Delegate Sub switchBlankScreenModeCallback(ByVal tmpN As Boolean)
    Public Sub SwitchBlankScreenMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchBlankScreenModeCallback(AddressOf SwitchBlankScreenMode), New Object() {tmpN})
            Exit Sub
        End If

        Me.Refresh()
        '隐藏播放控件
        'AxShockwaveFlash1.Hide()
        If playFlash IsNot Nothing Then
            Me.Controls.Remove(playFlash)
            playFlash.Dispose()
            playFlash = Nothing
        End If

        Me.BackColor = Color.Black
    End Sub

    ''' <summary>
    ''' 模拟鼠标点击消息
    ''' </summary>
    Public Delegate Sub MousesimulationClickCallback(ByVal screenId As Integer, ByVal tX As Integer, ByVal tY As Integer, ByVal value As Integer)
    Public Sub MousesimulationClick(ByVal screenId As Integer, ByVal tX As Integer, ByVal tY As Integer, ByVal value As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New MousesimulationClickCallback(AddressOf MousesimulationClick), New Object() {screenId, tX, tY, value})
            Exit Sub
        End If

        Dim touchPieceWidth As Integer = sysInfo.ScreenList(screenId).TouchPieceWidth
        Dim touchPieceHeight As Integer = sysInfo.ScreenList(screenId).TouchPieceHeight

        'Static id As Integer = 0
        'Debug.WriteLine($"{id} x:{tX} y:{tY}")
        'id += 1

        Select Case sysInfo.DisplayMode
            Case 0
                '点击
                Dim txp As Int16 = sysInfo.ScreenList(screenId).X + tX * touchPieceWidth + touchPieceWidth \ 2
                Dim typ As Int32 = sysInfo.ScreenList(screenId).Y + tY * touchPieceHeight + touchPieceHeight \ 2

                Dim ttp As Int32 = txp + (typ << 16)
                Dim ttp2 As Int32 = txp + ((typ + 2) << 16)

                '                Dim tmpStr =
                '$"<invoke name=""pointActive"" returntype=""xml""><arguments><string>{txp}</string><string>{typ}</string></arguments></invoke>"
                '                Debug.WriteLine("c")
                '                Try
                '                    playFlash.CallFunction(tmpStr)
                '                Catch ex As Exception
                '                    Debug.WriteLine(ex.Message)
                '                End Try

                '点击-移动-松开
                PostMessage(playFlash.Handle,
                            WM_LBUTTONDOWN,
                            0,
                            ttp)
                PostMessage(playFlash.Handle,
                            WM_MOUSEMOVE,
                            0,
                            ttp2)
                PostMessage(playFlash.Handle,
                            WM_LBUTTONUP,
                            0,
                            ttp)

            Case 1
                '测试
                gBack.DrawString($"√", gFont, gBrush,
                                 sysInfo.ScreenList(screenId).X + tX * touchPieceWidth + 1,
                                 sysInfo.ScreenList(screenId).Y + tY * touchPieceHeight + 1)
            Case 4
                '显示电容
                gBack.DrawString($"{value And &H7F}", gFont, gBrush,
                                 sysInfo.ScreenList(screenId).X + tX * touchPieceWidth + 1,
                                 sysInfo.ScreenList(screenId).Y + tY * touchPieceHeight + 1)
        End Select

    End Sub

    Private Sub FormPlay_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Debug.WriteLine($"w{Me.Width}h{Me.Height}")
    End Sub
End Class