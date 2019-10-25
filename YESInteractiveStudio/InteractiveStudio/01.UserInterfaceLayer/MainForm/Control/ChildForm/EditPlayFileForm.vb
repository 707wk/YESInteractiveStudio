Public Class EditPlayFileForm

    ''' <summary>
    ''' 播放文件信息
    ''' </summary>
    Public DisplayingFile As DisplayingFile

    Private Sub EditPlayFileForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        NumericUpDown1.Maximum = Int32.MaxValue \ 1000

        Label4.Text = IO.Path.GetFileName(DisplayingFile.Path)
        TextBox1.Text = DisplayingFile.Path
        NumericUpDown1.Value = DisplayingFile.PlaySecond
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        DisplayingFile.PlaySecond = NumericUpDown1.Value

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.Close()
    End Sub
End Class