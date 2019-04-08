Imports YESInteractiveSDK

Public Class UnityPlayControl
    Declare Function MoveWindow Lib "User32.dll" (handle As IntPtr,
                                                  x As Integer,
                                                  y As Integer,
                                                  width As Integer,
                                                  height As Integer,
                                                  redraw As Boolean) As Boolean

    Delegate Function WindowEnumProc(hwnd As IntPtr, lparam As IntPtr) As Integer

    Declare Function EnumChildWindows Lib "User32.dll" (hwnd As IntPtr,
                                                        func As WindowEnumProc,
                                                        lParam As IntPtr) As Boolean

    Declare Function SendMessageW Lib "User32.dll" (hwnd As IntPtr,
                                                    msg As Integer,
                                                    wParam As IntPtr,
                                                    lParam As IntPtr) As Integer

    Declare Function SendMessage Lib "User32.dll" Alias "SendMessageW" (hWnd As Integer,
                                                   Msg As Integer,
                                                   wParam As Integer,
                                                   ByRef lParam As COPYDATASTRUCT) As Integer

    Private UnityProcess As Process
    Private UnityHwnd As IntPtr = IntPtr.Zero

    Private Const WM_ACTIVATE As Integer = &H6
    Private Const WM_COPYDATA As Integer = &H4A
    Private ReadOnly WA_ACTIVE As IntPtr = New IntPtr(1)
    Private ReadOnly WA_INACTIVE As IntPtr = New IntPtr(0)

    Public Sub New(UnityPath As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        UnityProcess = New Process
        UnityProcess.StartInfo.FileName = UnityPath
    End Sub

    Private Sub UserControl1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        With UnityProcess.StartInfo
            .Arguments = $"-parentHWND {Panel1.Handle.ToInt32} {Environment.CommandLine}"
            .UseShellExecute = False
            .CreateNoWindow = True
            .WorkingDirectory = IO.Path.GetDirectoryName(.FileName)
        End With
        UnityProcess.Start()

        UnityProcess.WaitForInputIdle()

        EnumChildWindows(Panel1.Handle, AddressOf WindowEnum, IntPtr.Zero)
    End Sub

    Private Sub ActivateUnityWindow()
        SendMessageW(UnityHwnd, WM_ACTIVATE, WA_ACTIVE, IntPtr.Zero)
    End Sub

    Private Sub DeactivateUnityWindow()
        SendMessageW(UnityHwnd, WM_ACTIVATE, WA_INACTIVE, IntPtr.Zero)
    End Sub

    Private Function WindowEnum(hwnd As IntPtr, lparam As IntPtr) As Integer
        UnityHwnd = hwnd
        ActivateUnityWindow()

        Return 0
    End Function

    Private Sub Panel1_Resize(sender As Object, e As EventArgs) Handles Panel1.Resize
        MoveWindow(UnityHwnd, 0, 0, Panel1.Width, Panel1.Height, True)
        ActivateUnityWindow()
    End Sub

    Public Sub ParentFormShown()
        Threading.Thread.Sleep(500)
        Panel1_Resize(Nothing, Nothing)
    End Sub

    'Public Sub ParentFormClosed()
    '    Try
    '        UnityProcess.CloseMainWindow()

    '        While UnityProcess.HasExited = False
    '            UnityProcess.Kill()
    '        End While

    '    Catch ex As Exception

    '    End Try
    'End Sub

    Public Sub ParentFormActivated()
        ActivateUnityWindow()
    End Sub

    Public Sub ParentFormDeactivate()
        DeactivateUnityWindow()
    End Sub

    Private Sub UserControl1_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Try
            UnityProcess.CloseMainWindow()

            While UnityProcess.HasExited = False
                UnityProcess.Kill()
            End While

        Catch ex As Exception

        End Try
    End Sub

    Public Sub PutMessage(Value As String)
        Dim cds As COPYDATASTRUCT
        cds.dwData = CType(0, IntPtr)
        cds.cbData = Value.Length + 1
        cds.lpData = Value

        SendMessage(UnityHwnd, WM_COPYDATA, 0, cds)
        SendMessage(UnityProcess.MainWindowHandle, WM_COPYDATA, 0, cds)
    End Sub
End Class
