Imports System.Reflection
Imports InteractiveStudio
Imports YESInteractiveSDK

Public Class PlayDllControl
    Implements IPlayBaseControl

    Public Sub FormActivated() Implements IPlayBaseControl.FormActivated

    End Sub

    Public Sub FormDeactivate() Implements IPlayBaseControl.FormDeactivate

    End Sub

    Public Sub Dispose() Implements IPlayBaseControl.Dispose
        If DllControl IsNot Nothing Then
            DllControl.Dispose()
            DllControl = Nothing
        End If
    End Sub

    ''' <summary>
    ''' DLL播放器控件
    ''' </summary>
    Private DllControl As YESInteractiveSDK.IYESInterfaceSDK

    Public Function Init(controls As Control.ControlCollection, path As String) As Boolean Implements IPlayBaseControl.Init
        Dim ass = Assembly.LoadFrom(path)
        Dim tp = ass.GetType($"{IO.Path.GetFileNameWithoutExtension(path)}.{IO.Path.GetFileNameWithoutExtension(path)}")
        Dim obj = System.Activator.CreateInstance(tp)
        DllControl = CType(obj, YESInteractiveSDK.IYESInterfaceSDK)
        DllControl.InitAddonFunc(controls)

        Return True
    End Function

    Public Function Remove(controls As Control.ControlCollection) As Boolean Implements IPlayBaseControl.Remove
        controls.Remove(DllControl)
        DllControl.Dispose()
        DllControl = Nothing

        Return True
    End Function

    Public Function PointActive(values As List(Of PointInfo)) As Boolean Implements IPlayBaseControl.PointActive
        DllControl.PointActive(values)

        Return True
    End Function
End Class
