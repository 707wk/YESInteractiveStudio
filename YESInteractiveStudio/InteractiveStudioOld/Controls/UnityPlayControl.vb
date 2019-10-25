Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Xml
Imports System.Xml.Serialization
Imports InteractiveStudio.UnityConfig.ModuleUnityConfig
Imports YESInteractiveSDK

Public Class UnityPlayControl

#Region "消息结构"
    Public Structure COPYDATASTRUCT
        Public dwData As IntPtr

        Public cbData As Integer

        <MarshalAs(UnmanagedType.LPStr)>
        Public lpData As String
    End Structure
#End Region

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

    Public Sub New(UnityPath As String, SizeStr As String)

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。
        UnityProcess = New Process

        With UnityProcess.StartInfo
            UnityProcess.StartInfo.FileName = UnityPath
            .Arguments = $"{SizeStr} -parentHWND {Panel1.Handle.ToInt32} {Environment.CommandLine}"
            .UseShellExecute = False
            .CreateNoWindow = True
            .WorkingDirectory = IO.Path.GetDirectoryName(.FileName)
        End With
    End Sub

    Private Sub UserControl1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "写入位置及尺寸"
        'Dim tmpConfig As applicationConfig

        'Using fStream As New FileStream($"{IO.Path.GetDirectoryName(UnityProcess.StartInfo.FileName)}\config.xml", FileMode.Open)
        '    Dim XmlSerializer As XmlSerializer = New XmlSerializer(GetType(applicationConfig))
        '    tmpConfig = XmlSerializer.Deserialize(fStream)
        'End Using

        'For i001 = 0 To tmpConfig.MovieControl.Count - 1
        '    Select Case tmpConfig.MovieControl(i001).key
        '        Case "Top"
        '            tmpConfig.MovieControl(i001).value = Me.Top

        '        Case "Left"
        '            tmpConfig.MovieControl(i001).value = Me.Left

        '        Case "Width"
        '            tmpConfig.MovieControl(i001).value = Me.Width

        '        Case "Height"
        '            tmpConfig.MovieControl(i001).value = Me.Height

        '    End Select
        'Next

        'Using fStream As New FileStream($"{IO.Path.GetDirectoryName(UnityProcess.StartInfo.FileName)}\config.xml", FileMode.Create)
        '    Dim ns As XmlSerializerNamespaces = New XmlSerializerNamespaces()
        '    ns.Add("", "") '删除命名空间
        '    '添加编码属性
        '    Dim tmpXmlTextWriter As XmlTextWriter = New XmlTextWriter(fStream, Encoding.UTF8) With {
        '        .Formatting = Formatting.Indented '子节点缩进
        '    }
        '    Dim sfFormatter As New XmlSerializer(GetType(applicationConfig))
        '    sfFormatter.Serialize(tmpXmlTextWriter, tmpConfig, ns)
        'End Using
#End Region

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

        If UnityHwnd <> &H0 Then
            'Unity播放窗口
            SendMessage(UnityHwnd, WM_COPYDATA, 0, cds)
        Else
            '非Unity程序
            SendMessage(UnityProcess.MainWindowHandle, WM_COPYDATA, 0, cds)
        End If
    End Sub
End Class
