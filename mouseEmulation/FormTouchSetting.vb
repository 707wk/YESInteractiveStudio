Public Class FormTouchSetting
    '发送数据数组
    Dim sendByte(4 - 1) As Byte

    Private Sub FormTouchSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sendstr As String = "aadb0305"
        'ReDim sendByte(sendstr.Length \ 2 - 1)

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For i As Integer = 0 To senderArray.Length - 1
            mainClass.SetScanBoardData(i, 255, 65535, sendByte)
        Next
        'MsgBox($"发送完毕", MsgBoxStyle.Information, Me.Text)
        Me.Close()
    End Sub

    '高
    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        sendByte(3) = &H9
    End Sub

    '中
    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        sendByte(3) = &H5
    End Sub

    '低
    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        sendByte(3) = &H1
    End Sub
End Class