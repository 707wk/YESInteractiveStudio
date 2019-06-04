Imports System.ComponentModel
Imports System.IO
Imports System.Reflection
Imports Newtonsoft.Json
Imports YESInteractiveSDK

Public Class PlayWindow
    ''' <summary>
    ''' 窗口ID
    ''' </summary>
    Public WindowId As Integer

#Region "画刷"
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
#End Region

    '''' <summary>
    '''' Flash播放器控件
    '''' </summary>
    'Private FlashControl As AxShockwaveFlashObjects.AxShockwaveFlash
    ''' <summary>
    ''' 捕获鼠标
    ''' </summary>
    Dim SetCaptureFlage As Boolean

    ''' <summary>
    ''' DLL播放器控件
    ''' </summary>
    Private DllControl As YESInteractiveSDK.IYESInterfaceSDK
    ''' <summary>
    ''' 文件类型 0无 1swf 2dll 3Unity
    ''' </summary>
    Public FileType As Integer

    ''' <summary>
    ''' Unity播放控件
    ''' </summary>
    Private UnityControl As UnityPlayControl

#Region "发送消息"
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
#End Region

#Region "窗体初始化/关闭"
    Public Delegate Sub UpdateWindowCallback(ByVal Value As Boolean)
    ''' <summary>
    ''' 刷新位置/尺寸
    ''' </summary>
    Public Overloads Sub UpdateWindow(ByVal Value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateWindowCallback(AddressOf UpdateWindow), New Object() {Value})
            Exit Sub
        End If

        With AppSetting.Schedule.WindowList(WindowId)
            Me.Location = .Location
            Me.Size = .Size
            Me.gFont = New Font("宋体", Convert.ToSingle(12 / (.ZoomPix.Width / .ZoomPix.Height)), FontStyle.Regular)
            Me.gBack = Me.CreateGraphics
        End With
    End Sub

    Private Sub PlayWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Me.TopMost = True
        Me.BackColor = Color.Black
        Me.gPen = New Pen(Color.Green)
        Me.gBrush = New SolidBrush(Color.Green)

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
    End Sub

    Private Sub PlayWindow_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        UpdateWindow(True)
    End Sub

    Private Sub PlayWindow_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        ClearPlayControl()
    End Sub

    '关闭窗体
    Public Delegate Sub CloseCallback(ByVal Value As Boolean)
    Public Overloads Sub Close(ByVal Value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New CloseCallback(AddressOf Close), New Object() {Value})
            Exit Sub
        End If

        Me.Close()
    End Sub

    Public Delegate Sub HideWindowCallback(ByVal Value As Boolean)
    ''' <summary>
    ''' 隐藏窗体
    ''' </summary>
    Public Overloads Sub HideWindow(ByVal Value As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New UpdateWindowCallback(AddressOf HideWindow), New Object() {Value})
            Exit Sub
        End If

        Me.Size = New Size(0, 0)
    End Sub
#End Region

#Region "播放文件"
#Region "清空播放控件"
    ''' <summary>
    ''' 清空播放控件
    ''' </summary>
    Public Sub ClearPlayControl()
        'If FlashControl IsNot Nothing Then
        '    Me.Controls.Remove(FlashControl)
        '    FlashControl.Dispose()
        '    FlashControl = Nothing
        'End If

        If DllControl IsNot Nothing Then
            Me.Controls.Remove(DllControl)
            DllControl.FinalizeAddonFunc(Me)
            DllControl = Nothing
        End If

        If UnityControl IsNot Nothing Then
            Me.Controls.Remove(UnityControl)
            UnityControl.Dispose()
            UnityControl = Nothing
        End If
    End Sub
#End Region

#Region "播放Flash"
    '''' <summary>
    '''' 播放Flash
    '''' </summary>
    'Public Sub PlaySwf(ByVal FilePath As String)
    '    ClearPlayControl()

    '    SetCaptureFlage = False
    '    FileType = 1

    '    FlashControl = New AxShockwaveFlashObjects.AxShockwaveFlash With {
    '        .Dock = DockStyle.Fill
    '    }
    '    Me.Controls.Add(FlashControl)

    '    With FlashControl
    '        .AlignMode = 5 '对齐方式
    '        .ScaleMode = 2 '缩放模式
    '        .Quality = 0 '画面质量
    '        .BackgroundColor = 0
    '        .Movie = FilePath
    '    End With
    'End Sub
#End Region

#Region "播放DLL"
    ''' <summary>
    ''' 播放DLL
    ''' </summary>
    Public Sub PlayDLL(ByVal FilePath As String)
        ClearPlayControl()

        FileType = 2

        Dim ass = Assembly.LoadFrom(FilePath)
        Dim tp = ass.GetType($"{Path.GetFileNameWithoutExtension(FilePath)}.{Path.GetFileNameWithoutExtension(FilePath)}")
        Dim obj = System.Activator.CreateInstance(tp)
        DllControl = CType(obj, YESInteractiveSDK.IYESInterfaceSDK)
        DllControl.InitAddonFunc(Me)

    End Sub
#End Region

#Region "播放Unity"
    ''' <summary>
    ''' 播放Unity
    ''' </summary>
    Public Sub PlayUnity(ByVal FilePath As String)
        ClearPlayControl()

        FileType = 3

        If UnityControl IsNot Nothing Then
            UnityControl.Dispose()
        End If

        UnityControl = New UnityPlayControl(FilePath, $"{Me.Top},{Me.Left},{Me.Width},{Me.Height}")

        Me.Controls.Add(UnityControl)
        With UnityControl
            .Visible = True
            .Dock = DockStyle.Fill
        End With
        UnityControl.ParentFormShown()
    End Sub
#End Region

    Public Delegate Sub PlayCallback(ByVal FilePath As String)
    ''' <summary>
    ''' 播放文件
    ''' </summary>
    Public Sub Play(ByVal FilePath As String)
        If Me.InvokeRequired Then
            Me.Invoke(New PlayCallback(AddressOf Play), New Object() {FilePath})
            Exit Sub
        End If

        Select Case System.IO.Path.GetExtension(FilePath).ToLower()
            'Case ".swf"
            '    PlaySwf(FilePath)
            Case ".dll"
                PlayDLL(FilePath)
            Case ".exe"
                PlayUnity(FilePath)
        End Select

    End Sub
#End Region

#Region "切换显示模式"
    ''' <summary>
    ''' 播放文件
    ''' </summary>
    Public Sub PlayMode()
        Dim TmpWindowInfo As WindowInfo = AppSetting.Schedule.WindowList(WindowId)

        With TmpWindowInfo
            Me.Refresh()

            If .PlayMediaId = -1 OrElse
                .PlayProgramInfo.MediaList.Count = 0 Then
                Exit Sub
            End If

            If .PlayMediaId >= .PlayProgramInfo.MediaList.Count Then
                .PlayMediaId = 0
            End If
            Play(.PlayProgramInfo.MediaList(.PlayMediaId).Path)
        End With

        AppSetting.Schedule.WindowList(WindowId) = TmpWindowInfo
    End Sub

    ''' <summary>
    ''' 测试
    ''' </summary>
    Public Sub TestMode()
        ClearPlayControl()

        Me.BackColor = Color.Black
        Me.Refresh()

        For Each tmp As Integer In AppSetting.Schedule.WindowList.Item(WindowId).ScreenList
            With AppSetting.ScreenList(tmp)
                '缩放后触摸单元高度
                For i As Integer = 0 To .ZoomSize.Height Step .ZoomSensorSize.Height
                    gBack.DrawLine(gPen,
                                   .ZoomLocation.X + 0,
                                   .ZoomLocation.Y + i,
                                   .ZoomLocation.X + .ZoomSize.Width,
                                   .ZoomLocation.Y + i)
                Next

                '缩放后触摸单元宽度
                For i As Integer = 0 To .ZoomSize.Width Step .ZoomSensorSize.Width
                    gBack.DrawLine(gPen,
                                   .ZoomLocation.X + i,
                                   .ZoomLocation.Y + 0,
                                   .ZoomLocation.X + i,
                                   .ZoomLocation.Y + .ZoomSize.Height)
                Next
            End With
        Next
    End Sub

    ''' <summary>
    ''' 黑屏
    ''' </summary>
    Public Sub BlackMode()
        ClearPlayControl()

        Me.BackColor = Color.Black
        Me.Refresh()
    End Sub

    Public Delegate Sub SwitchDisplayModeCallback(ByVal Mode As InteractiveOptions.DISPLAYMODE)
    ''' <summary>
    ''' 切换显示模式
    ''' </summary>
    Public Sub SwitchDisplayMode(ByVal Mode As InteractiveOptions.DISPLAYMODE)
        If Me.InvokeRequired Then
            Me.Invoke(New SwitchDisplayModeCallback(AddressOf SwitchDisplayMode), New Object() {Mode})
            Exit Sub
        End If

        Select Case Mode
            Case InteractiveOptions.DISPLAYMODE.INTERACT
                PlayMode()

            Case InteractiveOptions.DISPLAYMODE.TEST
                TestMode()

            Case InteractiveOptions.DISPLAYMODE.BLACK
                BlackMode()

            Case InteractiveOptions.DISPLAYMODE.DEBUG
                TestMode()

        End Select
    End Sub
#End Region

#Region "点击事件"
    Public Delegate Sub PointActiveCallback(values As List(Of PointInfo))
    ''' <summary>
    ''' 点击事件
    ''' </summary>
    Public Sub PointActive(values As List(Of PointInfo))
        If Me.InvokeRequired Then
            Me.Invoke(New PointActiveCallback(AddressOf PointActive),
                      New Object() {values})
            Exit Sub
        End If

        ''计算尺寸及位置
        'Dim SensorWidth As Integer = sysInfo.ScreenList(ScreenID).ZoomSensorSize.Width
        'Dim SensorHeight As Integer = sysInfo.ScreenList(ScreenID).ZoomSensorSize.Height
        'Dim txp As Int16 = sysInfo.ScreenList(ScreenID).ZoomLocation.X + Location.X * SensorWidth + (SensorWidth \ 2)
        'Dim typ As Int32 = sysInfo.ScreenList(ScreenID).ZoomLocation.Y + Location.Y * SensorHeight + (SensorHeight \ 2)
        Try

            Select Case AppSetting.DisplayMode
                Case InteractiveOptions.DISPLAYMODE.INTERACT
#Region "互动"
                    '互动
                    Select Case FileType
                        Case 1
#Region "swf"
'                            For Each i001 As PointInfo In values
'                                'swf
'                                '非第一次按下则丢弃
'                                If i001.Old Then
'                                    Continue For
'                                End If

'                                If Not SetCaptureFlage Then
'#Region "启用接口"
'                                    '启用接口
'                                    Try
'                                        FlashControl.
'                                        CallFunction($"<invoke name=""pointActive"" returntype=""xml""><arguments><string>{i001.X}</string><string>{i001.Y}</string></arguments></invoke>")
'                                    Catch ex As Exception
'                                        SetCaptureFlage = True
'                                    End Try
'#End Region
'                                Else
'#Region "捕获鼠标"
'                                    '捕获鼠标
'                                    Dim ttp As Int32 = i001.X + (i001.Y << 16)
'                                    Dim ttp2 As Int32 = i001.X + ((i001.Y + 2) << 16)

'                                    '点击-移动 - 松开
'                                    PostMessage(FlashControl.Handle,
'                                        WM_LBUTTONDOWN,
'                                        0,
'                                        ttp)
'                                    PostMessage(FlashControl.Handle,
'                                        WM_MOUSEMOVE,
'                                        0,
'                                        ttp2)
'                                    PostMessage(FlashControl.Handle,
'                                        WM_LBUTTONUP,
'                                        0,
'                                        ttp)
'#End Region
'                                End If
'                            Next
#End Region
                        Case 2
#Region "dll"
                            'dll
                            DllControl.PointActive(values.ToArray)
#End Region
                        Case 3
#Region "Unity"
                            Dim tmpArray = values.ToArray
                            For i001 = 0 To tmpArray.Count - 1
                                tmpArray(i001).Y = Me.Height - tmpArray(i001).Y
                            Next

                            UnityControl.PutMessage(JsonConvert.SerializeObject(tmpArray))
#End Region
                    End Select
#End Region

                Case InteractiveOptions.DISPLAYMODE.TEST
#Region "测试"
                    For Each i001 As PointInfo In values
                        '非按第一次按下则丢弃
                        If i001.Old Then
                            Continue For
                        End If

                        gBack.DrawString($"√", gFont, gBrush, i001.X - gFont.SizeInPoints + 2, i001.Y - gFont.SizeInPoints + 2)
                    Next
#End Region

                    '            Case InteractiveOptions.DISPLAYMODE.DEBUG
                    '#Region "调试"
                    '                For Each i001 As PointInfo In values
                    '                    'todo:显示电容
                    '                    'gBack.DrawString($"{i001.Value And &H7F}", gFont, gBrush, i001.X - gFont.SizeInPoints / 2, i001.Y - gFont.SizeInPoints / 2)
                    '                Next
                    '#End Region

            End Select

        Catch ex As Exception

        End Try
    End Sub
#End Region

#Region "显示电容值"
    Public Delegate Sub ShowCapacitanceCallback(x As Integer, y As Integer, Value As Byte)
    ''' <summary>
    ''' 显示电容值
    ''' </summary>
    Public Sub ShowCapacitance(x As Integer, y As Integer, Value As Byte)
        If Me.InvokeRequired Then
            Me.Invoke(New ShowCapacitanceCallback(AddressOf ShowCapacitance),
                      New Object() {x, y, Value})
            Exit Sub
        End If

        gBack.DrawString($"{Value And &H7F}", gFont, gBrush, x - gFont.SizeInPoints + 2, y - gFont.SizeInPoints + 2)
    End Sub
#End Region

#Region "Unity事件"
    Private Sub PlayWindow_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If UnityControl IsNot Nothing Then
            UnityControl.ParentFormActivated()
        End If
    End Sub

    Private Sub PlayWindow_Deactivate(sender As Object, e As EventArgs) Handles Me.Deactivate
        If UnityControl IsNot Nothing Then
            UnityControl.ParentFormDeactivate()
        End If
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With AppSetting.Language

        End With
    End Sub
#End Region
End Class