''' <summary>
''' Windows热键处理辅助类
''' </summary>
Public NotInheritable Class WindowsHotKeyHelper

    Private Sub New()
    End Sub

    ''' <summary>
    ''' 注册热键
    ''' </summary>
    Friend Declare Function RegisterHotKey Lib "user32" (ByVal hWnd As Integer,
                                                         ByVal id As Integer,
                                                         ByVal fsModifiers As Integer,
                                                         ByVal vk As Integer) As Integer
    ''' <summary>
    ''' 注销热键
    ''' </summary>
    Friend Declare Function UnregisterHotKey Lib "user32" (ByVal hWnd As Integer,
                                                           ByVal id As Integer) As Integer

    ''' <summary>
    ''' 热键消息ID
    ''' </summary>
    Public Const WM_HOTKEY As Short = &H312S
    ''' <summary>
    ''' ALT
    ''' </summary>
    Public Const MOD_ALT As Short = &H1S
    ''' <summary>
    ''' Ctrl
    ''' </summary>
    Public Const MOD_CONTROL As Short = &H2S
    ''' <summary>
    ''' Shift
    ''' </summary>
    Public Const MOD_SHIFT As Short = &H4S

End Class
