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

#Region "IDisposable Support"
    Private disposedValue As Boolean ' 要检测冗余调用

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                '' TODO: 释放托管状态(托管对象)。
            End If

            UnityControl?.Dispose()
            UnityControl = Nothing

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

    'Public Sub Dispose() Implements IPlayBaseControl.Dispose
    '        If UnityControl IsNot Nothing Then
    '            UnityControl.Dispose()
    '            UnityControl = Nothing
    '        End If
    '    End Sub

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

    Public Function PointActive(values As List(Of PointInfo)) As Boolean Implements IPlayBaseControl.PointActive
        Dim tmpArray = values.ToArray
        For Each item In tmpArray
            item.Y = UnityControl.Height - item.Y
        Next

        UnityControl.PutMessage(JsonConvert.SerializeObject(tmpArray))

        Return True
    End Function
End Class
