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
    ''' 背景刷
    ''' </summary>
    Private gBrushDown As SolidBrush
    ''' <summary>
    ''' 背景字体
    ''' </summary>
    Private gFont As Font

    Private Sub FormPlay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '窗体置顶
        'Me.TopMost = True

        Me.BackColor = Color.Black
        '初始化绘图参数
        Me.gBack = Me.CreateGraphics
        Me.gPen = New Pen(Color.Green)
        Me.gBrush = New SolidBrush(Color.Green)
        Me.gBrushDown = New SolidBrush(Color.Red)
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.ZoomProportion), FontStyle.Regular)
    End Sub

    Private Sub FormPlay_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        SwitchTestMode(True)
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
        Me.gFont = New Font("宋体", Convert.ToSingle(12 / sysInfo.ZoomProportion), FontStyle.Regular)

        Me.gBack = Me.CreateGraphics
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

        Select Case sysInfo.DisplayMode
            Case 0
                '测试
                gBack.DrawString($"√", gFont, gBrush,
                                 sysInfo.ScreenList(screenId).X + tX * touchPieceWidth + 1,
                                 sysInfo.ScreenList(screenId).Y + tY * touchPieceHeight + 1)
            Case 1
                '显示电容
                'If value And &H80 Then
                '    gBack.FillRectangle(gBrushDown,
                '    tX * touchPieceWidth + 1,
                '    tY * touchPieceHeight + 1,
                '    touchPieceWidth - 1,
                '    touchPieceHeight - 1)
                'Else
                gBack.DrawString($"{value And &H7F}", gFont, gBrush,
                                 sysInfo.ScreenList(screenId).X + tX * touchPieceWidth + 1,
                                 sysInfo.ScreenList(screenId).Y + tY * touchPieceHeight + 1)
                'End If

        End Select
    End Sub
End Class