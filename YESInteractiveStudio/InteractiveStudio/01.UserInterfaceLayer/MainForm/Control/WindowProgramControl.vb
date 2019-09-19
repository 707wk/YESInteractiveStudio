Public Class WindowProgramControl
    Private Sub WindowProgramControl_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Dock = DockStyle.Fill

        With TreeView1
            .ImageList = ProgramImageList
            .ShowLines = False
            .ShowRootLines = False
            .FullRowSelect = True
        End With
    End Sub
End Class
