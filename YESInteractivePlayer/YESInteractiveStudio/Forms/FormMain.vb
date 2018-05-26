Public Class FormMain
    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '显示版本号
        With My.Application.Info
            Me.Text = $"{ .ProductName} V{ .Version.ToString}"
        End With

        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2010Blue

        '初始化配置
        Dim tmpDialog As New FormInit
        tmpDialog.ShowDialog()
    End Sub
End Class