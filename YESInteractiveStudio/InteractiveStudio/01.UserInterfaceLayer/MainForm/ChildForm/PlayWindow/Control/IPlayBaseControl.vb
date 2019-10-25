Imports System.Windows.Forms.Control
''' <summary>
''' 播放基础控件
''' </summary>
Public Interface IPlayBaseControl
    Inherits IDisposable

    Function Init(controls As ControlCollection, path As String) As Boolean
    Function Remove(controls As ControlCollection) As Boolean
    Function PointActive(values As List(Of YESInteractiveSDK.PointInfo)) As Boolean

    Sub FormActivated()
    Sub FormDeactivate()
End Interface
