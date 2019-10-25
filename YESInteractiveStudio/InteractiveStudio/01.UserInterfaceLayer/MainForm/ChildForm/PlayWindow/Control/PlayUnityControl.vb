Imports InteractiveStudio
Imports Newtonsoft.Json
Imports YESInteractiveSDK

Public Class PlayUnityControl
    Implements IPlayBaseControl

    Public Sub FormActivated() Implements IPlayBaseControl.FormActivated
        UnityControl.ParentFormActivated()
    End Sub

    Public Sub FormDeactivate() Implements IPlayBaseControl.FormDeactivate
        UnityControl.ParentFormDeactivate()
    End Sub

    Public Sub Dispose() Implements IPlayBaseControl.Dispose
        If UnityControl IsNot Nothing Then
            UnityControl.Dispose()
            UnityControl = Nothing
        End If
    End Sub

    ''' <summary>
    ''' Unity播放控件
    ''' </summary>
    Private UnityControl As UnityControl

    Public Function Init(controls As Control.ControlCollection, path As String) As Boolean Implements IPlayBaseControl.Init
        UnityControl = New UnityControl()

        controls.Add(UnityControl)
        With UnityControl
            .Visible = True
            .Dock = DockStyle.Fill
        End With
        UnityControl.Play(path)
        UnityControl.ParentFormShown()

        Return True
    End Function

    Public Function Remove(controls As Control.ControlCollection) As Boolean Implements IPlayBaseControl.Remove
        If UnityControl IsNot Nothing Then
            UnityControl.Dispose()
            UnityControl = Nothing
        End If

        Return True
    End Function

    Public Function PointActive(values As List(Of PointInfo)) As Boolean Implements IPlayBaseControl.PointActive
        Dim tmpArray = values.ToArray
        For Each item In tmpArray
            item.Y = UnityControl.Height - item.Y
        Next

        UnityControl.PutMessage(JsonConvert.SerializeObject(tmpArray))

        Return True
    End Function
End Class
