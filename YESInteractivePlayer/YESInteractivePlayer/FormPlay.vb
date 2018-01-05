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
        Me.TopMost = True

        '隐藏播放控件
        AxShockwaveFlash1.Hide()
        AxShockwaveFlash1.BackgroundColor = 0

        Me.BackColor = Color.Black
        '初始化绘图参数
        Me.gBack = Me.CreateGraphics
        Me.gPen = New Pen(Color.Green)
        Me.gBrush = New SolidBrush(Color.Green)
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.zoomProportion), FontStyle.Regular)

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
    Public Sub closeDialog(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New closeDialogCallback(AddressOf closeDialog), New Object() {tmpN})
            Exit Sub
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' 播放flash
    ''' </summary>
    Public Delegate Sub playCallback(ByVal swfUrl As String)
    Public Sub play(ByVal swfUrl As String)
        If Me.InvokeRequired Then
            Me.Invoke(New playCallback(AddressOf play), New Object() {swfUrl})
            Exit Sub
        End If

        AxShockwaveFlash1.Show()
        AxShockwaveFlash1.StopPlay()
        AxShockwaveFlash1.SAlign = 1
        AxShockwaveFlash1.ScaleMode = 2
        AxShockwaveFlash1.Movie = swfUrl
        AxShockwaveFlash1.Play()
    End Sub

    ''' <summary>
    ''' 设置显示位置、尺寸
    ''' </summary>
    Public Delegate Sub setLocationCallback(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
    Public Sub setLocation(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New setLocationCallback(AddressOf setLocation), New Object() {x, y, width, height})
            Exit Sub
        End If

        Me.Location = New Point(x, y)
        Me.Size = New Size(width, height)
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.zoomProportion), FontStyle.Regular)

        Me.gBack = Me.CreateGraphics
    End Sub

    ''' <summary>
    ''' 切换为播放模式
    ''' </summary>
    Public Delegate Sub switchPlayModeCallback(ByVal tmpN As Boolean)
    Public Sub switchPlayMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchPlayModeCallback(AddressOf switchPlayMode), New Object() {tmpN})
            Exit Sub
        End If

        Me.Refresh()
        '显示播放控件
        AxShockwaveFlash1.Show()
        AxShockwaveFlash1.Movie = AxShockwaveFlash1.Movie
        AxShockwaveFlash1.Play()
    End Sub

    ''' <summary>
    ''' 切换为测试模式
    ''' </summary>
    Public Delegate Sub switchTestModeCallback(ByVal tmpN As Boolean)
    Public Sub switchTestMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchTestModeCallback(AddressOf switchTestMode), New Object() {tmpN})
            Exit Sub
        End If

        '隐藏播放控件
        AxShockwaveFlash1.Hide()

        Me.BackColor = Color.Black
        Me.Refresh()

        For Each tmp In sysInfo.curtainList.Item(curtainListId).screenList
            '缩放后触摸单元高度
            Dim touchPieceHeight As Integer = sysInfo.screenList(tmp).touchPieceHeight
            For i As Integer = 0 To sysInfo.screenList(tmp).height Step touchPieceHeight
                gBack.DrawLine(gPen,
                               sysInfo.screenList(tmp).x + 0,
                               sysInfo.screenList(tmp).y + i,
                               sysInfo.screenList(tmp).x + sysInfo.screenList(tmp).width,
                               sysInfo.screenList(tmp).y + i)
            Next

            '缩放后触摸单元宽度
            Dim touchPieceWidth As Integer = sysInfo.screenList(tmp).touchPieceWidth
            For i As Integer = 0 To sysInfo.screenList(tmp).width Step touchPieceWidth
                gBack.DrawLine(gPen,
                               sysInfo.screenList(tmp).x + i,
                               sysInfo.screenList(tmp).y + 0,
                               sysInfo.screenList(tmp).x + i,
                               sysInfo.screenList(tmp).y + sysInfo.screenList(tmp).height)
            Next
        Next
    End Sub

    ''' <summary>
    ''' 切换为黑屏
    ''' </summary>
    Public Delegate Sub switchBlankScreenModeCallback(ByVal tmpN As Boolean)
    Public Sub switchBlankScreenMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchBlankScreenModeCallback(AddressOf switchBlankScreenMode), New Object() {tmpN})
            Exit Sub
        End If

        Me.Refresh()
        '隐藏播放控件
        AxShockwaveFlash1.Hide()
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

        Dim touchPieceWidth As Integer = sysInfo.screenList(screenId).touchPieceWidth
        Dim touchPieceHeight As Integer = sysInfo.screenList(screenId).touchPieceHeight

        Select Case sysInfo.displayMode
            Case 0
                '点击
                Dim txp As Int16 = sysInfo.screenList(screenId).x + tX * touchPieceWidth + touchPieceWidth \ 2
                Dim typ As Int32 = sysInfo.screenList(screenId).y + tY * touchPieceHeight + touchPieceHeight \ 2

                Dim ttp As Int32 = txp + (typ << 16)
                Dim ttp2 As Int32 = txp + ((typ + 2) << 16)

                '点击-移动-松开
                PostMessage(Me.AxShockwaveFlash1.Handle,
                            WM_LBUTTONDOWN,
                            0,
                            ttp)
                PostMessage(Me.AxShockwaveFlash1.Handle,
                            WM_MOUSEMOVE,
                            0,
                            ttp2)
                PostMessage(Me.AxShockwaveFlash1.Handle,
                            WM_LBUTTONUP,
                            0,
                            ttp)

            Case 1
                '测试
                gBack.DrawString($"√", gFont, gBrush,
                                 sysInfo.screenList(screenId).x + tX * touchPieceWidth + 1,
                                 sysInfo.screenList(screenId).y + tY * touchPieceHeight + 1)

        End Select

    End Sub
End Class