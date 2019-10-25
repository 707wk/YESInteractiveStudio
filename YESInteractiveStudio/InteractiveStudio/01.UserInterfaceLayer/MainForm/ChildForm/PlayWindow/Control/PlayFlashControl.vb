Imports InteractiveStudio
Imports YESInteractiveSDK

Public Class PlayFlashControl
    Implements IPlayBaseControl

    Public Sub FormActivated() Implements IPlayBaseControl.FormActivated

    End Sub

    Public Sub FormDeactivate() Implements IPlayBaseControl.FormDeactivate

    End Sub

    Protected Sub Dispose() Implements IPlayBaseControl.Dispose
        If FlashControl IsNot Nothing Then
            FlashControl.Dispose()
            FlashControl = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Flash播放器控件
    ''' </summary>
    Private FlashControl As AxShockwaveFlashObjects.AxShockwaveFlash
    ''' <summary>
    ''' 捕获鼠标
    ''' </summary>
    Dim SetCaptureFlage As Boolean = False

#Region "发送消息"
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

    Public Function Init(controls As Control.ControlCollection, path As String) As Boolean Implements IPlayBaseControl.Init
        FlashControl = New AxShockwaveFlashObjects.AxShockwaveFlash With {
            .Dock = DockStyle.Fill
        }
        controls.Add(FlashControl)

        With FlashControl
            .AlignMode = 5 '对齐方式
            .ScaleMode = 2 '缩放模式
            .Quality = 0 '画面质量
            .BackgroundColor = 0
            .Movie = path
        End With

        Return True
    End Function

    Public Function Remove(controls As Control.ControlCollection) As Boolean Implements IPlayBaseControl.Remove
        controls.Remove(FlashControl)
        FlashControl.Dispose()
        FlashControl = Nothing

        Return True
    End Function

    Public Function PointActive(values As List(Of PointInfo)) As Boolean Implements IPlayBaseControl.PointActive

        For Each i001 As PointInfo In values
            'swf
            '非第一次按下则丢弃
            If i001.Old Then
                Continue For
            End If

            If Not SetCaptureFlage Then
#Region "启用接口"
                '启用接口
                Try
                    FlashControl.
                    CallFunction($"<invoke name=""pointActive"" returntype=""xml""><arguments><string>{i001.X}</string><string>{i001.Y}</string></arguments></invoke>")
                Catch ex As Exception
                    SetCaptureFlage = True
                End Try
#End Region
            Else
#Region "捕获鼠标"
                '捕获鼠标
                Dim ttp As Int32 = i001.X + (i001.Y << 16)
                Dim ttp2 As Int32 = i001.X + ((i001.Y + 2) << 16)

                '点击-移动 - 松开
                PostMessage(FlashControl.Handle,
                    WM_LBUTTONDOWN,
                    0,
                    ttp)
                PostMessage(FlashControl.Handle,
                    WM_MOUSEMOVE,
                    0,
                    ttp2)
                PostMessage(FlashControl.Handle,
                    WM_LBUTTONUP,
                    0,
                    ttp)
#End Region
            End If
        Next

        Return True
    End Function
End Class
