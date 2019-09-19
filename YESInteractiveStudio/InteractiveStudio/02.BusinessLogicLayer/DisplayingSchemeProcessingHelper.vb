''' <summary>
''' 播放方案处理辅助类
''' </summary>
Public Class DisplayingSchemeProcessingHelper
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 是否已显示播放窗体
    ''' </summary>
    Private Shared IsShowWindow As Boolean = False

#Region "显示所有播放窗体"
    ''' <summary>
    ''' 显示所有播放窗体
    ''' </summary>
    Public Shared Sub ShowALLDisplayingWindow()
        If IsShowWindow Then
            Exit Sub
        End If
        IsShowWindow = True

        ComputeSizeAndLocationForALLScreenAndScanBoard()
        ComputeSizeAndLocationForALLSensor()
        '计算播放窗口尺寸
        '显示播放窗体
    End Sub
#End Region

#Region "计算屏幕/接收卡尺寸及坐标"
    ''' <summary>
    ''' 计算屏幕/接收卡尺寸及坐标
    ''' </summary>
    Private Shared Sub ComputeSizeAndLocationForALLScreenAndScanBoard()
        For DisplayingWindowID = 0 To AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Count - 1
            Dim tmpDisplayingWindow = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems(DisplayingWindowID)
            Dim Magnificine = tmpDisplayingWindow.Magnificine

            For Each ScreenID In tmpDisplayingWindow.ScreenIDItems

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
                        .DisplayingWindowID = DisplayingWindowID

                        .LocationInDisplayingWindow.X = tmpDisplayingWindow.Location.X + .LocationOfOriginal.X * Magnificine
                        .LocationInDisplayingWindow.Y = tmpDisplayingWindow.Location.Y + .LocationOfOriginal.Y * Magnificine

                        .SizeOfZoom.Width = .SizeOfOriginal.Width * Magnificine
                        .SizeOfZoom.Height = .SizeOfOriginal.Height * Magnificine
                    End With
                Next

            Next
        Next
    End Sub
#End Region

#Region "计算所有传感器尺寸及坐标"
    ''' <summary>
    ''' 计算所有传感器尺寸及坐标
    ''' </summary>
    Private Shared Sub ComputeSizeAndLocationForALLSensor()
        For DisplayingWindowID = 0 To AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Count - 1
            Dim tmpDisplayingWindow = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems(DisplayingWindowID)

            For Each ScreenID In tmpDisplayingWindow.ScreenIDItems
                '屏幕尺寸及坐标
                Dim tmpNovaStarScreen = AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)

                '接收卡尺寸及坐标
                For Each tmpNovaStarScanBoard In tmpNovaStarScreen.NovaStarScanBoardItems

                    ComputeSizeAndLocationForSensor(tmpNovaStarScanBoard)

                Next

            Next
        Next
    End Sub

    ''' <summary>
    ''' 计算单个传感器尺寸及坐标
    ''' </summary>
    Private Shared Sub ComputeSizeAndLocationForSensor(scanBoard As NovaStarScanBoard)
        Dim tmpYesTechBaseBox As YesTechBaseBox = Activator.CreateInstance(YesTechBaseBox.GetBoxType(scanBoard.SizeOfOriginal))
        tmpYesTechBaseBox.SaveSensorToSenderCache()

    End Sub

#End Region

#Region "关闭所有播放窗体"
    ''' <summary>
    ''' 关闭所有播放窗体
    ''' </summary>
    Public Shared Sub CloseALLDisplayingWindow()
        If Not IsShowWindow Then
            Exit Sub
        End If
        IsShowWindow = False

    End Sub
#End Region

End Class
