''' <summary>
''' 基础箱体
''' </summary>
Public MustInherit Class YesTechBaseBox
    ''' <summary>
    ''' 接收卡信息
    ''' </summary>
    Friend ScanBoard As NovaStarScanBoard

    ''' <summary>
    ''' 传感器集合
    ''' </summary>
    Friend SensorItems As New List(Of Sensor)

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

        For Each tmpSensor In SensorItems
            AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems(ScanBoard.SenderID).SensorItems.Add(tmpSensor.Key, tmpSensor)
        Next

    End Sub

    Friend MustOverride Sub BoxRotateAngle360()

    Friend MustOverride Sub BoxRotateAngle90()

    Friend MustOverride Sub BoxRotateAngle180()

    Friend MustOverride Sub BoxRotateAngle270()

End Class
