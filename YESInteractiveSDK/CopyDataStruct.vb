Imports System.Runtime.InteropServices

Public Structure COPYDATASTRUCT
    Public dwData As IntPtr

    Public cbData As Integer

    <MarshalAs(UnmanagedType.LPStr)>
    Public lpData As String
End Structure
