Public Class FormTouchSetting
    '发送数据数组
    Dim sendByte(4 - 1) As Byte

    Private Sub FormTouchSetting_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim sendstr As String = "aadb0305"
        'ReDim sendByte(sendstr.Length \ 2 - 1)

        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next

        NumericUpDown1.Maximum = 9
        NumericUpDown1.Minimum = 1
        If ScreenSensitivity > 9 Or ScreenSensitivity < 1 Then
            Me.NumericUpDown1.Value = 1
        Else
            Me.NumericUpDown1.Value = ScreenSensitivity
        End If

        NumericUpDown2.Maximum = 3
        NumericUpDown2.Minimum = 1
        If ScreenAntiS > 3 Or ScreenAntiS < 1 Then
            Me.NumericUpDown2.Value = 1
        Else
            Me.NumericUpDown2.Value = ScreenAntiS
        End If

        NumericUpDown3.Maximum = 60
        NumericUpDown3.Minimum = 0
        If ResetTemp > 60 Or ResetTemp < 0 Then
            Me.NumericUpDown3.Value = 0
        Else
            Me.NumericUpDown3.Value = ResetTemp
        End If

        NumericUpDown4.Maximum = 256
        NumericUpDown4.Minimum = 0
        If ResetTemp > 256 Or ResetTemp < 0 Then
            Me.NumericUpDown4.Value = 0
        Else
            Me.NumericUpDown4.Value = ResetTimeMin
        End If

        changeLanguage()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        
        sendByte(3) = NumericUpDown1.Value

        'For i As Integer = 0 To senderArray.Length - 1
        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        If Me.RadioButton1.Checked Then InteractMode = 1
        If Me.RadioButton2.Checked Then InteractMode = 2
        If Me.RadioButton3.Checked Then InteractMode = 3
        ScreenSensitivity = Me.NumericUpDown1.Value
        ScreenAntiS = Me.NumericUpDown2.Value
        'Next
        'MsgBox($"发送完毕", MsgBoxStyle.Information, Me.Text)
        Me.Close()
    End Sub

    ''高
    'Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs)
    '    sendByte(3) = &H9
    'End Sub

    ''中
    'Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs)
    '    sendByte(3) = &H5
    'End Sub

    ''低
    'Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs)
    '    sendByte(3) = &H1
    'End Sub

    '更改语言 0:中文 1:English
    Private Sub changeLanguage()
        Me.GroupBox1.Text = $"{If(selectLanguageId = 0, "灵敏度", "Sensitivity")}"
        Me.GroupBox3.Text = $"{If(selectLanguageId = 0, "抗干扰度", "Antijamming Capability")}"
        'Me.RadioButton1.Text = $"{If(selectLanguageId = 0, "高", "Hight")}"
        'Me.RadioButton2.Text = $"{If(selectLanguageId = 0, "中", "Middle")}"
        'Me.RadioButton3.Text = $"{If(selectLanguageId = 0, "低", "Low")}"
        'Me.Button1.Text = $"{If(selectLanguageId = 0, "应用", "Apply")}"
        Me.Text = $"{If(selectLanguageId = 0, "灵敏度调节", "Sensitive Setting")}"
        Me.RadioButton1.Text = $"{If(selectLanguageId = 0, "普通屏", "LED Display")}"
        Me.RadioButton3.Text = $"{If(selectLanguageId = 0, "地砖屏", "Dance Floor")}"
        Me.RadioButton2.Text = $"{If(selectLanguageId = 0, "踩踏屏+", "Dance Floor+")}"
        Me.GroupBox2.Text = $"{If(selectLanguageId = 0, "互动方式", "Interract Mode")}"
        Me.GroupBox4.Text = $"{If(selectLanguageId = 0, "复位设置", "Reset Setting")}"
        Me.Label1.Text = $"{If(selectLanguageId = 0, "温度增值", "Temp Added")}"
        Me.Label4.Text = $"{If(selectLanguageId = 0, "复位间隔", "Reset Inerval")}"
        Me.Button2.Text = $"{If(selectLanguageId = 0, "强制复位", "Reset")}"
        Me.Button3.Text = $"{If(selectLanguageId = 0, "设置", "Setting")}"
        Me.Button4.Text = $"{If(selectLanguageId = 0, "设置", "Setting")}"


    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton2.CheckedChanged
        InteractMode = 2
        ScreenAnti = 2
        NumericUpDown2.Value = 2
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton1.CheckedChanged
        InteractMode = 1
        ScreenAnti = 1
        NumericUpDown2.Value = 1
    End Sub

    Private Sub NumericUpDown1_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown1.ValueChanged

    End Sub

    Private Sub RadioButton3_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButton3.CheckedChanged
        InteractMode = 3
        ScreenAnti = 2
        NumericUpDown2.Value = 2
    End Sub

    Private Sub NumericUpDown2_ValueChanged(sender As Object, e As EventArgs) Handles NumericUpDown2.ValueChanged

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Dim sendstr As String = "aadb0101" '强制复位
        For i As Integer = 0 To sendByte.Length - 1
            sendByte(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        'For i As Integer = 0 To senderArray.Length - 1
        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte)
        MsgBox($"{If(selectLanguageId = 0, "已设置", "OK")}",
                      MsgBoxStyle.Information,
                      $"{If(selectLanguageId = 0, "信息", "Information")}")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim sendByte2(5 - 1) As Byte
        Dim sendstr As String = "aadb0103"
        For i As Integer = 0 To sendByte.Length - 1
            sendByte2(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        Next
        sendByte2(4) = NumericUpDown3.Value
        'For i As Integer = 0 To senderArray.Length - 1

        mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte2)
        ResetTemp = NumericUpDown3.Value
        MsgBox($"{If(selectLanguageId = 0, "已设置", "OK")}",
                      MsgBoxStyle.Information,
                      $"{If(selectLanguageId = 0, "信息", "Information")}")




    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        'Dim sendByte2(5 - 1) As Byte
        'Dim sendstr As String = "aadb0102"
        'For i As Integer = 0 To sendByte.Length - 1
        'sendByte2(i) = Val($"&H{sendstr(i * 2)}{ sendstr(i * 2 + 1)}")
        ' Next
        ' sendByte2(4) = NumericUpDown4.Value

        ' mainClass.SetScanBoardData(&HFF, &HFF, &HFFFF, sendByte2)
        ResetTimeMin = NumericUpDown4.Value
        If ResetTimeMin <= 0 Then
            Form1.Timer1.Enabled = False
        Else
            Form1.Timer1.Interval = ResetTimeMin * 60 * 1000
        End If

        MsgBox($"{If(selectLanguageId = 0, "已设置", "OK")}",
                       MsgBoxStyle.Information,
                       $"{If(selectLanguageId = 0, "信息", "Information")}")
    End Sub

    Private Sub GroupBox4_Enter(sender As Object, e As EventArgs) Handles GroupBox4.Enter

    End Sub
End Class