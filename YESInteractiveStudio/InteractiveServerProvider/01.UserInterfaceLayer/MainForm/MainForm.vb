Imports System.ComponentModel
Imports System.Threading
Imports DevComponents.DotNetBar
Imports Microsoft.Win32
Imports Nova.LCT.GigabitSystem.Common
Imports Nova.Mars.SDK
Imports Wangk.Resource

Public Class MainForm
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Wangk.Tools.LoggerHelper.Log.LogThis("程序启动")

        '初始化配置
        AppSettingHelper.Settings.ToString()

        '#Region "重新启动Nova服务"
        '        ''todo:重新启动Nova服务
        '        Dim tmpProcess = System.Diagnostics.Process.GetProcessesByName("MarsServerProvider")
        '        If tmpProcess.Length > 0 Then
        '            tmpProcess(0).Kill()
        '        End If
        '        Process.Start($".\Nova\Server\MarsServerProvider.exe")
        '#End Region

#Region "样式设置"
        StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light

        '产品版本号
        Dim assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location
        Dim versionStr = System.Diagnostics.FileVersionInfo.GetVersionInfo(assemblyLocation).ProductVersion
        Me.Text = $"{My.Application.Info.ProductName} V{versionStr}"

        NotifyIcon1.ContextMenuStrip = ContextMenuStrip1
        NotifyIcon1.Text = Me.Text

#End Region

    End Sub

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Hide()

        ShowNovaStarSenderList()

#Region "连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Dispose screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StartAsync()
                                          End Sub)

        End Using
#End Region

        '更新接收卡状态
        Timer1.Interval = 2000
        Timer1.Start()

    End Sub

    Private Sub MainForm_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not IsExit Then
            Me.Hide()
            e.Cancel = True
            Exit Sub
        End If

        SensorDataProcessingHelper.StopAsync()

        AppSettingHelper.SaveToLocaltion()
    End Sub

#Region "读取屏幕信息"
    Private Sub ReadScreenInformationButton_Click(sender As Object, e As EventArgs) Handles ReadScreenInformationButton.Click

        Dim tmpNovaStarScreenItems() As NovaStarScreen = Nothing
        Dim tmpNovaStarSenderItems() As NovaStarSender = Nothing

        ''todo:选择串口
        Using tmpSerialPortSelectForm As New SerialPortSelectForm
            If tmpSerialPortSelectForm.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            '开始读取
            Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                .Text = "Reading screen information",
                .ProgressBarStyle = ProgressBarStyle.Marquee
            }

                Dim tmpNovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl

                tmpBackgroundWorkDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
#Region "初始化"
                                                  Dim screenCount As Integer
                                                  Dim senderCount As Integer
                                                  tmpNovaMarsControl.Initialize(tmpSerialPortSelectForm.selectedSerialPort, screenCount, senderCount)

                                                  If screenCount = 0 Then Throw New Exception("No Screen found")
                                                  If senderCount = 0 Then Throw New Exception("No Sender found")
#End Region

#Region "读取屏幕信息"
                                                  ReDim tmpNovaStarScreenItems(screenCount - 1)
                                                  ReDim tmpNovaStarSenderItems(senderCount - 1)
                                                  For itemID = 0 To tmpNovaStarSenderItems.Count - 1
                                                      tmpNovaStarSenderItems(itemID) = New NovaStarSender
                                                  Next

                                                  Dim tmpLEDScreenInfoList As List(Of Nova.Mars.SDK.LEDScreenInfo) = Nothing
                                                  If tmpNovaMarsControl.ReadLEDScreenInfo(tmpLEDScreenInfoList) <>
                                                  Nova.Mars.SDK.OperateResult.OK Then

                                                      Throw New Exception("Failed to reading screen information")
                                                  End If

#End Region

#Region "读取热备份信息"
                                                  Dim hotBackUpItems As List(Of SenderRedundancyInfo) = Nothing
                                                  '读取热备份信息
                                                  Using GetHotBackUpEvent As New AutoResetEvent(False)
                                                      tmpNovaMarsControl.GetHotBackUp(Sub(res As HotBackUpState, info As List(Of SenderRedundancyInfo))
                                                                                          If res = HotBackUpState.Successful Then
                                                                                              hotBackUpItems = info
                                                                                              'Throw New Exception($"{MultiLanguageHelper.Lang.GetS("Fail to Get HotBackUp")}")
                                                                                          End If

                                                                                          GetHotBackUpEvent.Set()
                                                                                      End Sub)
                                                      GetHotBackUpEvent.WaitOne()
                                                  End Using

                                                  If hotBackUpItems IsNot Nothing Then
                                                      '只支持发送卡内备份
                                                      For Each item In hotBackUpItems
                                                          If item.MasterSenderIndex <> item.SlaveSenderIndex Then
                                                              Throw New Exception(MultiLanguageHelper.Lang.GetS("The Slave Sender must be the same as the Master Sender in HotBackUp"))
                                                          End If
                                                      Next

                                                      '记录主从网口号
                                                      For Each item In hotBackUpItems
                                                          tmpNovaStarSenderItems(item.MasterSenderIndex).HotBackUpPortItems.Add(item.MasterPortIndex, item.SlavePortIndex)
                                                      Next

                                                      ''统计网口下接收卡ID最大数
                                                      'For Each screenItem In tmpLEDScreenInfoList
                                                      '    For Each scanBoardItem In screenItem.ScanBoardInfoList
                                                      '        If scanBoardItem.SenderIndex = &HFF Then Continue For

                                                      '        With scanBoardItem
                                                      '            If .ConnectIndex > tmpNovaStarSenderItems(.SenderIndex).MaximumConnectID(.PortIndex) Then
                                                      '                tmpNovaStarSenderItems(.SenderIndex).MaximumConnectID(.PortIndex) = .ConnectIndex
                                                      '            End If
                                                      '        End With
                                                      '    Next
                                                      'Next

                                                  End If

#End Region

#Region "遍历屏幕"
                                                  For itemID = 0 To tmpNovaStarScreenItems.Count - 1
                                                      tmpNovaStarScreenItems(itemID) = New NovaStarScreen

                                                      With tmpNovaStarScreenItems(itemID)
                                                          tmpNovaMarsControl.GetScreenLocation(itemID,
                                                                                               .LocationOfOriginal.X,
                                                                                               .LocationOfOriginal.Y,
                                                                                               .SizeOfOriginal.Width,
                                                                                               .SizeOfOriginal.Height)


#Region "读取接收卡信息"
                                                          For Each tmpScanBoard In tmpLEDScreenInfoList(itemID).ScanBoardInfoList
                                                              '接收卡留空则不添加
                                                              If tmpScanBoard.SenderIndex = &HFF Then Continue For

                                                              '连接位置
                                                              Dim tmpNovaStarScanBoard As New NovaStarScanBoard
                                                              With tmpNovaStarScanBoard
                                                                  .SenderID = tmpScanBoard.SenderIndex
                                                                  .PortID = tmpScanBoard.PortIndex
                                                                  .ConnectID = tmpScanBoard.ConnectIndex
                                                                  .LocationOfOriginal.X = tmpScanBoard.X
                                                                  .LocationOfOriginal.Y = tmpScanBoard.Y
                                                                  .SizeOfOriginal.Width = tmpScanBoard.Width
                                                                  .SizeOfOriginal.Height = tmpScanBoard.Height
                                                              End With

                                                              '箱体旋转角度
                                                              tmpNovaMarsControl.ReadCabinetRotateAngle(tmpNovaStarScanBoard.SenderID,
                                                                                                        tmpNovaStarScanBoard.PortID,
                                                                                                        tmpNovaStarScanBoard.ConnectID,
                                                                                                        tmpNovaStarScanBoard.BoxRotateAngle)

                                                              .NovaStarScanBoardItems.Add(tmpNovaStarScanBoard)

                                                          Next
#End Region

                                                      End With

                                                  Next
#End Region

#Region "读取发送卡IP"
                                                  AddHandler tmpNovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

                                                  For itemID = 0 To tmpNovaStarSenderItems.Count - 1
                                                      'tmpNovaStarSenderItems(itemID) = New NovaStarSender

                                                      SenderIPData = Nothing

                                                      tmpNovaMarsControl.GetEquipmentIP(itemID)
                                                      GetEquipmentIPDataEvent.WaitOne()

                                                      If SenderIPData Is Nothing Then Throw New Exception($"Sender {itemID} no support for interactive")

                                                      tmpNovaStarSenderItems(itemID).IpData = SenderIPData

                                                  Next

                                                  RemoveHandler tmpNovaMarsControl.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
#End Region

                                              End Sub)

                If tmpBackgroundWorkDialog.Error IsNot Nothing Then
                    MsgBox(tmpBackgroundWorkDialog.Error.Message,
                           MsgBoxStyle.Information,
                           tmpBackgroundWorkDialog.Text)
                    Exit Sub
                End If

            End Using
        End Using

#Region "清空旧播放窗体/发送卡列表/屏幕列表"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Clear old screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                          End Sub)

        End Using
#End Region

        '复制新配置
        AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems = tmpNovaStarScreenItems
        AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems = tmpNovaStarSenderItems

        '显示播放窗口编辑窗体
        Using tmpPlayWindowSettingsForm As New PlayWindowSettingsForm With {
            .IsMustSave = True
        }
            tmpPlayWindowSettingsForm.ShowDialog()
        End Using

        ShowNovaStarSenderList()

#Region "连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Dispose screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              '计算传感器位置
                                              DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLScreenAndScanBoard()
                                              DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLSensor()
                                              'DisplayingSchemeProcessingHelper.ComputeSizeForALLDisplayingWindow()

                                              SensorDataProcessingHelper.StartAsync()
                                          End Sub)

        End Using
#End Region

    End Sub

#Region "获取发送卡IP"
    ''' <summary>
    ''' 读取到IP标志
    ''' </summary>
    Private GetEquipmentIPDataEvent As New AutoResetEvent(False)
    ''' <summary>
    ''' 读到的发送卡IP
    ''' </summary>
    Private SenderIPData As Byte()

    ''' <summary>
    ''' 获取IP通知
    ''' </summary>
    Private Sub GetEquipmentIPData(sender As Object, e As Nova.Mars.SDK.MarsEquipmentIPEventArgs)
        Static Dim senderArrayId As Integer = 0

        If e.IsExecResult Then
            SenderIPData = e.Data
        End If

        GetEquipmentIPDataEvent.Set()
    End Sub
#End Region

#End Region

#Region "播放窗口设置"
    Private Sub PlayWindowSettingsButton_Click(sender As Object, e As EventArgs) Handles PlayWindowSettingsButton.Click
        If AppSettingHelper.Settings.DisplayingScheme.NovaStarScreenItems Is Nothing Then
            Exit Sub
        End If

#Region "清空旧播放窗体"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Dispose screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                          End Sub)

        End Using
#End Region

        Dim tmpDialog As New PlayWindowSettingsForm
        Dim isChanged As Boolean = tmpDialog.ShowDialog() = DialogResult.OK

#Region "显示播放窗口/连接控制器"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Dispose screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              If isChanged Then
                                                  '计算传感器位置
                                                  DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLScreenAndScanBoard()
                                                  DisplayingSchemeProcessingHelper.ComputeSizeAndLocationForALLSensor()
                                                  'DisplayingSchemeProcessingHelper.ComputeSizeForALLDisplayingWindow()
                                              End If

                                              SensorDataProcessingHelper.StartAsync()
                                          End Sub)

        End Using
#End Region

    End Sub
#End Region

#Region "精度设置"
    Private Sub AccuracyButton_Click(sender As Object, e As EventArgs) Handles AccuracyButton.Click
        Dim tmpDialog As New AccuracySettingsForm
        tmpDialog.ShowDialog()
    End Sub
#End Region

#Region "硬件设置"
    Private Sub HardwareButton_Click(sender As Object, e As EventArgs) Handles HardwareButton.Click
#Region "停止互动"
        Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Dispose screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

            tmpBackgroundWorkDialog.Start(Sub()
                                              SensorDataProcessingHelper.StopAsync()
                                          End Sub)

        End Using
#End Region

        ''todo:选择串口
        Dim tmpSerialPortSelectForm As New SerialPortSelectForm
        If tmpSerialPortSelectForm.ShowDialog() = DialogResult.OK Then

            '开始读取
            Using tmpBackgroundWorkDialog As New Wangk.Resource.BackgroundWorkDialog With {
                    .Text = "Reading screen information",
                    .ProgressBarStyle = ProgressBarStyle.Marquee
                }

                Dim tmpNovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl
                Dim screenCount As Integer
                Dim senderCount As Integer

                tmpBackgroundWorkDialog.Start(Sub(uie As Wangk.Resource.BackgroundWorkEventArgs)
#Region "初始化"
                                                  tmpNovaMarsControl.Initialize(tmpSerialPortSelectForm.selectedSerialPort, screenCount, senderCount)

                                                  If screenCount = 0 Then Throw New Exception("No Screen found")
                                                  If senderCount = 0 Then Throw New Exception("No Sender found")
#End Region
                                              End Sub)

                If tmpBackgroundWorkDialog.Error Is Nothing Then
                    Dim tmpDialog As New HardwareSettingsForm
                    With tmpDialog
                        .NovaMarsControl = tmpSerialPortSelectForm.NovaMarsControl
                        ReDim .NovaStarSenderItems(senderCount - 1)
                    End With

                    tmpDialog.ShowDialog()
                Else
                    MsgBox(tmpBackgroundWorkDialog.Error.Message,
                               MsgBoxStyle.Information,
                               tmpBackgroundWorkDialog.Text)
                End If

            End Using

        End If

#Region "开始互动"
        Using tmpDialog As New Wangk.Resource.UIWorkDialog With {
                    .Text = "Dispose screen information"
        }

            tmpDialog.Start(Sub()
                                Try
                                    tmpSerialPortSelectForm.Dispose()
#Disable Warning CA1031 ' Do not catch general exception types
                                Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types
                                End Try

                                SensorDataProcessingHelper.StartAsync()
                            End Sub)

        End Using
#End Region

    End Sub
#End Region

#Region "显示接收卡列表"
    ''' <summary>
    ''' 显示接收卡列表
    ''' </summary>
    Private Sub ShowNovaStarSenderList()
        ToolStripDropDownButton1.DropDownItems.Clear()

        If AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems Is Nothing Then
            Exit Sub
        End If

        For Each tmpNovaStarSender In AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems
            ToolStripDropDownButton1.DropDownItems.Add(New ToolStripMenuItem(
                                                       $"Sender:{tmpNovaStarSender.IPAddress}",
                                                       My.Resources.usb_disconnect_32px)
                                                       )
        Next

    End Sub

    ''' <summary>
    ''' 显示接收卡状态
    ''' </summary>
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Dim stateOfOK As Boolean = True

        Try
            If AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems Is Nothing Then
                Exit Sub
            End If

            For NovaStarSenderID = 0 To AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems.Count - 1
                Dim tmpNovaStarSender = AppSettingHelper.Settings.DisplayingScheme.NovaStarSenderItems(NovaStarSenderID)
                If tmpNovaStarSender.State = InteractiveOptions.SenderConnectState.OffLine Then
                    stateOfOK = False
                    ToolStripDropDownButton1.DropDownItems(NovaStarSenderID).Image = My.Resources.usb_disconnect_32px

                Else
                    ToolStripDropDownButton1.DropDownItems(NovaStarSenderID).Image = My.Resources.usb_connect_32px

                End If
            Next

            If stateOfOK Then
                ToolStripDropDownButton1.Image = My.Resources.Resources.usb_connect_32px
                ToolStripDropDownButton1.Text = "Normal Connection"

            Else
                ToolStripDropDownButton1.Image = My.Resources.Resources.usb_disconnect_32px
                ToolStripDropDownButton1.Text = "Connection abnormality"

            End If

#Disable Warning CA1031 ' Do not catch general exception types
        Catch ex As Exception
#Enable Warning CA1031 ' Do not catch general exception types

        End Try

    End Sub

#End Region

#Region "托盘图标"
    Private IsExit As Boolean = False
    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        IsExit = True
        Me.Close()
    End Sub

    Private Sub NotifyIcon1_MouseClick(sender As Object, e As MouseEventArgs) Handles NotifyIcon1.MouseClick
        If e.Button = MouseButtons.Left Then
            Me.Show()
            Me.Focus()
        End If
    End Sub
#End Region

End Class