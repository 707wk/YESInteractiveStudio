''' <summary>
''' 播放方案处理辅助类
''' </summary>
Public NotInheritable Class DisplayingSchemeProcessingHelper
    Private Sub New()
    End Sub

#Region "计算屏幕/接收卡尺寸及坐标"
    ''' <summary>
    ''' 计算屏幕/接收卡尺寸及坐标
    ''' </summary>
    Public Shared Sub ComputeSizeAndLocationForALLScreenAndScanBoard()

        Try
            Dim Magnificine = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.Magnificine

            For Each ScreenID In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems

                '屏幕尺寸及坐标
                Dim NovaStarScreen = AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)
                With NovaStarScreen
                    .LocationOfZoom.X = .LocationOfOriginal.X * Magnificine
                    .LocationOfZoom.Y = .LocationOfOriginal.Y * Magnificine

                    .SizeOfZoom.Width = .SizeOfOriginal.Width * Magnificine
                    .SizeOfZoom.Height = .SizeOfOriginal.Height * Magnificine

                End With

                '接收卡尺寸及坐标
                For Each tmpNovaStarScanBoard In NovaStarScreen.NovaStarScanBoardItems
                    With tmpNovaStarScanBoard

                        .LocationInDisplayingWindow.X = NovaStarScreen.LocationOfZoom.X + .LocationOfOriginal.X * Magnificine
                        .LocationInDisplayingWindow.Y = NovaStarScreen.LocationOfZoom.Y + .LocationOfOriginal.Y * Magnificine

                        .SizeOfZoom.Width = .SizeOfOriginal.Width * Magnificine
                        .SizeOfZoom.Height = .SizeOfOriginal.Height * Magnificine
                    End With
                Next

            Next

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub
#End Region

#Region "计算所有传感器尺寸及坐标"
    ''' <summary>
    ''' 计算所有传感器尺寸及坐标
    ''' </summary>
    Public Shared Sub ComputeSizeAndLocationForALLSensor()

        Try

            If AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems.Count = 0 Then
                Exit Sub
            End If

            For Each tmpNovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
                tmpNovaStarSender.SensorItems.Clear()
            Next

            For Each ScreenID In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem.ScreenIDItems
                '屏幕尺寸及坐标
                Dim tmpNovaStarScreen = AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)

                '接收卡尺寸及坐标
                For Each tmpNovaStarScanBoard In tmpNovaStarScreen.NovaStarScanBoardItems

                    Dim tmpYesTechBaseBox As YesTechBaseBox = YesTechBoxFactory.Create(tmpNovaStarScanBoard)
                    tmpYesTechBaseBox.SaveSensorToSenderCache()

                Next
            Next

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub
#End Region

#Region "计算所有播放窗口尺寸"
    '''' <summary>
    '''' 计算所有播放窗口尺寸
    '''' </summary>
    'Public Shared Sub ComputeSizeForALLDisplayingWindow()

    '    Try
    '        Dim tmpDisplayingWindow = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItem
    '        tmpDisplayingWindow.SizeOfZoom.Width = 0
    '        tmpDisplayingWindow.SizeOfZoom.Height = 0

    '        For Each ScreenID In tmpDisplayingWindow.ScreenIDItems
    '            With AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)
    '                '宽度
    '                If .LocationOfZoom.X + .SizeOfZoom.Width > tmpDisplayingWindow.SizeOfZoom.Width Then
    '                    tmpDisplayingWindow.SizeOfZoom.Width = .LocationOfZoom.X + .SizeOfZoom.Width
    '                End If

    '                '高度
    '                If .LocationOfZoom.Y + .SizeOfZoom.Height > tmpDisplayingWindow.SizeOfZoom.Height Then
    '                    tmpDisplayingWindow.SizeOfZoom.Height = .LocationOfZoom.Y + .SizeOfZoom.Height
    '                End If

    '            End With

    '        Next

    '    Catch ex As Exception
    '    End Try

    'End Sub
#End Region

End Class
