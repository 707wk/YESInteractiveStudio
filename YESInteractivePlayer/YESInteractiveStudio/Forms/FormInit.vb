Imports System.Threading

Public Class FormInit
    Private Sub FormInit_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub FormInit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim tmp As New Thread(AddressOf qwe) With {
            .IsBackground = True
        }
        tmp.Start()
    End Sub

    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ''' <summary>
    ''' 测试界面
    ''' </summary>
    Private Sub qwe()
        Thread.Sleep(3000)

        SwitchTestMode(True)
    End Sub

    Public Delegate Sub switchTestModeCallback(ByVal tmpN As Boolean)
    Public Sub SwitchTestMode(ByVal tmpN As Boolean)
        If Me.InvokeRequired Then
            Me.Invoke(New switchTestModeCallback(AddressOf SwitchTestMode), New Object() {tmpN})
            Exit Sub
        End If

        Me.Close()
    End Sub
    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
End Class