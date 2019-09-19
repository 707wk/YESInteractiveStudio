''' <summary>
''' 基础箱体
''' </summary>
Public MustInherit Class YesTechBaseBox
    ''' <summary>
    ''' 接收卡信息
    ''' </summary>
    Friend ScanBoard As NovaStarScanBoard

    Public Sub New(value As NovaStarScanBoard)
        ScanBoard = value
    End Sub

    ''' <summary>
    ''' 将传感器数据存入接收卡缓存
    ''' </summary>
    Public Sub SaveSensorToSenderCache()
        Select Case ScanBoard.BoxRotateAngle
            Case Nova.LCT.GigabitSystem.Common.RotateAngle.R_360
                BoxRotateAngle360()

            Case Nova.LCT.GigabitSystem.Common.RotateAngle.R_90
                BoxRotateAngle90()

            Case Nova.LCT.GigabitSystem.Common.RotateAngle.R_180
                BoxRotateAngle180()

            Case Nova.LCT.GigabitSystem.Common.RotateAngle.R_270
                BoxRotateAngle270()

        End Select
    End Sub

    Friend MustOverride Sub BoxRotateAngle360()

    Friend MustOverride Sub BoxRotateAngle90()

    Friend MustOverride Sub BoxRotateAngle180()

    Friend MustOverride Sub BoxRotateAngle270()

    ''' <summary>
    ''' 判断箱体类型
    ''' </summary>
    Public Shared Function GetBoxType(size As Size) As Type
        If size.Width = size.Height Then
            Return GetType(YesTechMG7)

        ElseIf size.Width = size.Height \ 4 OrElse
            size.Width \ 4 = size.Height Then
            Return GetType(YesTechMG11)

        ElseIf size.Width = size.Height \ 2 OrElse
            size.Width \ 2 = size.Height Then
            Return GetType(YesTechMG15)

        Else
            Throw New Exception($"未知箱体:{size.ToString}")
        End If

    End Function

End Class
