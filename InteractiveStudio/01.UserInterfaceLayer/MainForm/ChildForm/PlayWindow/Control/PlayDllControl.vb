Imports System.Reflection
Imports InteractiveStudio
Imports InteractiveSDK

Public Class PlayDllControl
    Implements IPlayBaseControl

    Public Sub FormActivated() Implements IPlayBaseControl.FormActivated

    End Sub

    Public Sub FormDeactivate() Implements IPlayBaseControl.FormDeactivate

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 要检测冗余调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                '' TODO: 释放托管状态(托管对象)。
            End If

            DllControl?.Dispose()
            DllControl = Nothing

            '' TODO: 释放未托管资源(未托管对象)并在以下内容中替代 Finalize()。
            '' TODO: 将大型字段设置为 null。
        End If
        disposedValue = True
    End Sub

    '' TODO: 仅当以上 Dispose(disposing As Boolean)拥有用于释放未托管资源的代码时才替代 Finalize()。
    'Protected Overrides Sub Finalize()
    '    ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Visual Basic 添加此代码以正确实现可释放模式。
    Public Sub Dispose() Implements IDisposable.Dispose
        ' 请勿更改此代码。将清理代码放入以上 Dispose(disposing As Boolean)中。
        Dispose(True)
        '' TODO: 如果在以上内容中替代了 Finalize()，则取消注释以下行。
        GC.SuppressFinalize(Me)
    End Sub
#End Region

    ''' <summary>
    ''' DLL播放器控件
    ''' </summary>
    Private DllControl As InteractiveSDK.IInterfaceSDK

    Public Function Init(controls As Control.ControlCollection, path As String) As Boolean Implements IPlayBaseControl.Init
        Dim ass = Assembly.LoadFrom(path)
        Dim tp = ass.GetType($"{IO.Path.GetFileNameWithoutExtension(path)}.{IO.Path.GetFileNameWithoutExtension(path)}")
        Dim obj = System.Activator.CreateInstance(tp)
        DllControl = CType(obj, InteractiveSDK.IInterfaceSDK)
        DllControl.InitAddonFunc(controls)

        Return True
    End Function

    'Public Function Remove(controls As Control.ControlCollection) As Boolean Implements IPlayBaseControl.Remove
    '    controls.Remove(DllControl)
    '    DllControl.Dispose()
    '    DllControl = Nothing

    '    Return True
    'End Function

    Public Function PointActive(values As List(Of PointInfo)) As Boolean Implements IPlayBaseControl.PointActive
        DllControl.PointActive(values)

        Return True
    End Function
End Class
