''' <summary>
''' 播放方案处理辅助类
''' </summary>
Public NotInheritable Class DisplayingSchemeProcessingHelper
    Private Sub New()
    End Sub

    ''' <summary>
    ''' 是否已显示播放窗口
    ''' </summary>
    Private Shared IsShowWindow As Boolean = False

#Region "显示所有播放窗口"
    ''' <summary>
    ''' 显示所有播放窗口
    ''' </summary>
    Public Shared Sub ShowFormForALLDisplayingWindow()

        If IsShowWindow Then
            Exit Sub
        End If
        IsShowWindow = True

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems

                tmpDisplayingWindow.PlayWindowForm = New PlayWindow With {
                    .DisplayingWindow = tmpDisplayingWindow
                }

#Disable Warning BC40000 ' 类型或成员已过时
                tmpDisplayingWindow.PlayWindowThreadOfCreate = New Threading.Thread(Sub()
                                                                                        tmpDisplayingWindow.PlayWindowForm.Location = tmpDisplayingWindow.Location
                                                                                        tmpDisplayingWindow.PlayWindowForm.Size = tmpDisplayingWindow.SizeOfZoom

                                                                                        tmpDisplayingWindow.PlayWindowForm.ShowDialog()

                                                                                    End Sub) With {
                                                                                    .ApartmentState = Threading.ApartmentState.STA,
                                                                                    .IsBackground = True
                }
#Enable Warning BC40000 ' 类型或成员已过时
                tmpDisplayingWindow.PlayWindowThreadOfCreate.Start()

            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "计算屏幕/接收卡尺寸及坐标"
    ''' <summary>
    ''' 计算屏幕/接收卡尺寸及坐标
    ''' </summary>
    Public Shared Sub ComputeSizeAndLocationForALLScreenAndScanBoard()

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems
                Dim DisplayingWindowID = AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.IndexOf(tmpDisplayingWindow)
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

                            .LocationInDisplayingWindow.X = NovaStarScreen.LocationOfZoom.X + .LocationOfOriginal.X * Magnificine
                            .LocationInDisplayingWindow.Y = NovaStarScreen.LocationOfZoom.Y + .LocationOfOriginal.Y * Magnificine

                            .SizeOfZoom.Width = .SizeOfOriginal.Width * Magnificine
                            .SizeOfZoom.Height = .SizeOfOriginal.Height * Magnificine
                        End With
                    Next

                Next
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "计算所有传感器尺寸及坐标"
    ''' <summary>
    ''' 计算所有传感器尺寸及坐标
    ''' </summary>
    Public Shared Sub ComputeSizeAndLocationForALLSensor()

        Try

            If AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems.Count = 0 Then
                Exit Sub
            End If

            For Each tmpNovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
                tmpNovaStarSender.SensorItems.Clear()
            Next

            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems
                For Each ScreenID In tmpDisplayingWindow.ScreenIDItems
                    '屏幕尺寸及坐标
                    Dim tmpNovaStarScreen = AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)

                    '接收卡尺寸及坐标
                    For Each tmpNovaStarScanBoard In tmpNovaStarScreen.NovaStarScanBoardItems

                        Dim tmpYesTechBaseBox As YesTechBaseBox = YesTechBoxFactory.Create(tmpNovaStarScanBoard)
                        tmpYesTechBaseBox.SaveSensorToSenderCache()

                    Next

                Next
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "计算所有播放窗口尺寸"
    ''' <summary>
    ''' 计算所有播放窗口尺寸
    ''' </summary>
    Public Shared Sub ComputeSizeForALLDisplayingWindow()

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems
                tmpDisplayingWindow.SizeOfZoom.Width = 0
                tmpDisplayingWindow.SizeOfZoom.Height = 0

                For Each ScreenID In tmpDisplayingWindow.ScreenIDItems
                    With AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems(ScreenID)
                        '宽度
                        If .LocationOfZoom.X + .SizeOfZoom.Width > tmpDisplayingWindow.SizeOfZoom.Width Then
                            tmpDisplayingWindow.SizeOfZoom.Width = .LocationOfZoom.X + .SizeOfZoom.Width
                        End If

                        '高度
                        If .LocationOfZoom.Y + .SizeOfZoom.Height > tmpDisplayingWindow.SizeOfZoom.Height Then
                            tmpDisplayingWindow.SizeOfZoom.Height = .LocationOfZoom.Y + .SizeOfZoom.Height
                        End If

                    End With
                Next
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "关闭所有播放窗口"
    ''' <summary>
    ''' 关闭所有播放窗口
    ''' </summary>
    Public Shared Sub CloseFormForALLDisplayingWindow()

        If Not IsShowWindow Then
            Exit Sub
        End If
        IsShowWindow = False

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems

                tmpDisplayingWindow.PlayWindowForm.CloseForm()
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "隐藏/显示所有播放窗口"
    ''' <summary>
    ''' 隐藏/显示所有播放窗口
    ''' </summary>
    Public Shared Sub HideFormForALLDisplayingWindow(value As Boolean)

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems

                tmpDisplayingWindow.PlayWindowForm.HideForm(value)
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

#Region "切换所有窗口显示模式"
    ''' <summary>
    ''' 切换所有窗口显示模式
    ''' </summary>
    Public Shared Sub ChangeDisplayModeForALLDisplayingWindow()

        Try
            For Each tmpDisplayingWindow In AppSettingHelper.Settings.DisplayingScheme.DisplayingWindowItems

                tmpDisplayingWindow.PlayWindowForm.DisplayModeChange()
            Next

        Catch ex As Exception

        End Try

    End Sub
#End Region

End Class
