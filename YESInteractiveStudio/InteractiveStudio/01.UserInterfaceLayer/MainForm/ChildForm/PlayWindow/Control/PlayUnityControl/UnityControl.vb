Imports System.Runtime.InteropServices

Public Class UnityControl
    Inherits Panel

#Region "消息结构"
    Public Structure COPYDATASTRUCT
        Public dwData As IntPtr

        Public cbData As Integer

        <MarshalAs(UnmanagedType.LPStr)>
        Public lpData As String
    End Structure
#End Region

#Region "消息处理"
    Private Declare Function MoveWindow Lib "User32.dll" (handle As IntPtr,
                                                  x As Integer,
                                                  y As Integer,
                                                  width As Integer,
                                                  height As Integer,
                                                  redraw As Boolean) As Boolean

    Private Delegate Function WindowEnumProc(hwnd As IntPtr, lparam As IntPtr) As Integer

    Private Declare Function EnumChildWindows Lib "User32.dll" (hwnd As IntPtr,
                                                        func As WindowEnumProc,
                                                        lParam As IntPtr) As Boolean

    Private Declare Function SendMessageW Lib "User32.dll" (hwnd As IntPtr,
                                                    msg As Integer,
                                                    wParam As IntPtr,
                                                    lParam As IntPtr) As Integer

    Private Declare Function SendMessage Lib "User32.dll" Alias "SendMessageW" (hWnd As Integer,
                                                   Msg As Integer,
                                                   wParam As Integer,
                                                   ByRef lParam As COPYDATASTRUCT) As Integer

    Private UnityProcess As Process
    Private UnityHwnd As IntPtr = IntPtr.Zero

    Private Const WM_ACTIVATE As Integer = &H6
    Private Const WM_COPYDATA As Integer = &H4A
    Private ReadOnly WA_ACTIVE As IntPtr = New IntPtr(1)
    Private ReadOnly WA_INACTIVE As IntPtr = New IntPtr(0)
#End Region

    Public Sub Play(path As String)

        UnityProcess = New Process
        With UnityProcess.StartInfo
            UnityProcess.StartInfo.FileName = path
            .Arguments = $"{Me.Top},{Me.Left},{Me.Width},{Me.Height} -parentHWND {Me.Handle.ToInt32} {Environment.CommandLine}"
            .UseShellExecute = False
            .CreateNoWindow = True
            .WorkingDirectory = IO.Path.GetDirectoryName(.FileName)
        End With

        UnityProcess.Start()
        UnityProcess.WaitForInputIdle()

        EnumChildWindows(Me.Handle, AddressOf WindowEnum, IntPtr.Zero)
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

    Private Sub Me_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        MoveWindow(UnityHwnd, 0, 0, Me.Width, Me.Height, True)
        ActivateUnityWindow()
    End Sub

    Public Sub ParentFormShown()
        Threading.Thread.Sleep(500)
        Me_Resize(Nothing, Nothing)
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

    Private Sub UnityControl_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
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

        If UnityHwnd <> &H0 Then
            'Unity播放窗口
            SendMessage(UnityHwnd, WM_COPYDATA, 0, cds)
        Else
            '非Unity程序
            SendMessage(UnityProcess.MainWindowHandle, WM_COPYDATA, 0, cds)
        End If
    End Sub

End Class
