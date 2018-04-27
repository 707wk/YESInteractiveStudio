Imports System.Threading
Imports Nova.Mars.SDK

Public Class FormNovaInit
    '''' <summary>
    '''' 自动滚动到最底端
    '''' </summary>
    'Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs)
    '    TextBox1.SelectionStart = TextBox1.Text.Length
    '    TextBox1.ScrollToCaret()
    'End Sub

    Private Sub FormNovaInit_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Me.ShowInTaskbar = False
    End Sub

    Private Sub FormNovaInit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '启动后台线程
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    ''' <summary>
    ''' 获取ip通知
    ''' </summary>
    Private Sub GetEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        Static Dim senderArrayId As Integer = 0
        If e.IsExecResult Then
            'senderArray(senderArrayIndex).index = senderArrayIndex
            sysInfo.SenderList(senderArrayId).IpDate = e.Data

            Showinfo($"{sysInfo.Language.GetLanguage("控制器")}{senderArrayId}")
            Showinfo($"    ip:{e.Data(3)}.{e.Data(2)}.{e.Data(1)}.{e.Data(0)}")
            Showinfo($"    {sysInfo.Language.GetLanguage("掩码")}:{e.Data(7)}.{e.Data(6)}.{e.Data(5)}.{e.Data(4)}")
            Showinfo($"    {sysInfo.Language.GetLanguage("网关")}:{e.Data(11)}.{e.Data(10)}.{e.Data(9)}.{e.Data(8)}")

            senderArrayId += 1
            If senderArrayId < sysInfo.SenderList.Length Then
                sysInfo.MainClass.GetEquipmentIP(senderArrayId)
            Else
                Showinfo(sysInfo.Language.GetLanguage("加载完成"))
                '移除事件
                RemoveHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.CloseDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
            End If
        Else
            '移除事件
            RemoveHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("获取设备IP失败！请检查设备后，重新启动程序")}")
        End If
    End Sub

    '关闭窗体
    Public Delegate Sub closeDialogCallback(ByVal text As String)
    Public Sub CloseDialog(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New closeDialogCallback(AddressOf CloseDialog), New Object() {text})
            Exit Sub
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' 显示信息
    ''' </summary>
    Public Delegate Sub showinfoCallback(ByVal text As String)
    Public Sub Showinfo(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New showinfoCallback(AddressOf Showinfo), New Object() {text})
            Exit Sub
        End If

        Label1.Text = text
        '有错误则提示并退出
        If text.IndexOf("ERROR") = -1 Then
            Exit Sub
        End If

        MsgBox($"{text}", MsgBoxStyle.Information, Me.Text)

        'Putlog(text)
        'Thread.Sleep(2000)

        '释放nova资源
        'If sysInfo.MainClass Is Nothing Then
        'Else
        '    sysInfo.MainClass.UnInitialize()
        'End If
        'If sysInfo.RootClass Is Nothing Then
        'Else
        '    sysInfo.RootClass.UnInitialize()
        'End If

        End
        'Application.Exit()
    End Sub

    ''' <summary>
    ''' 读取屏幕信息
    ''' </summary>
    Public Delegate Sub novaInitializeCallback(ByVal text As String)
    Public Sub NovaInitialize(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New novaInitializeCallback(AddressOf NovaInitialize), New Object() {text})
            Exit Sub
        End If

        '加密狗验证
        'checkdog()

        Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing

        Showinfo(sysInfo.Language.GetLanguage("连接Nova服务中"))
        sysInfo.RootClass = New MarsHardwareEnumerator

        If sysInfo.RootClass.Initialize() Then
            'Showinfo(sysInfo.Language.GetLanguage("连接Nova服务成功"))
        Else
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("连接Nova服务失败")}")
            'Application.Exit()
            Exit Sub
        End If

        Showinfo(sysInfo.Language.GetLanguage("查找控制系统中"))

        Dim SystemCount As Integer = sysInfo.RootClass.CtrlSystemCount()
        If SystemCount Then
            'Showinfo($"{sysInfo.Language.GetLanguage("控制系统数")}:{SystemCount}")
        Else
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("未找到控制系统")}")
            'Application.Exit()
            Exit Sub
        End If

        sysInfo.MainClass = New MarsControlSystem(sysInfo.RootClass)
        '绑定读取到ip事件
        AddHandler sysInfo.MainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        sysInfo.RootClass.GetComNameOfControlSystem(0, tmpstr)
        Showinfo($"{sysInfo.Language.GetLanguage("初始化屏幕")}:{sysInfo.MainClass.Initialize(tmpstr, screenCount, senderCount)}")
        Showinfo($"{sysInfo.Language.GetLanguage("显示屏个数")}:{screenCount} {sysInfo.Language.GetLanguage("控制器个数")}:{senderCount}")

        If senderCount = 0 Then
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("未找到控制器")}")
            Exit Sub
        End If

        Showinfo(sysInfo.Language.GetLanguage("读取显示屏信息中"))
        If sysInfo.MainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("读显示屏信息失败")}")
            Exit Sub
        End If

        If LEDScreenInfoList Is Nothing OrElse
            LEDScreenInfoList.Count = 0 Then
            Showinfo($"ERROR:{sysInfo.Language.GetLanguage("未找到显示屏")}")
            Exit Sub
        End If

        '获取到的显示屏 X 偏移
        Dim x As Integer
        '获取到的显示屏 Y 偏移
        Dim y As Integer
        ''获取到的显示屏宽度
        'Dim width As Integer
        ''获取到的显示屏高度
        'Dim height As Integer

        Showinfo(sysInfo.Language.GetLanguage("载入屏幕信息中"))
        sysInfo.ScanBoardTable = New Hashtable
        'ReDim sysInfo.screenList(screenCount - 1)
        For i As Integer = 0 To sysInfo.ScreenList.Length - 1
            If i < screenCount Then
                sysInfo.ScreenList(i).ExistFlage = True
            Else
                sysInfo.ScreenList(i).ExistFlage = False
            End If
        Next

        ReDim sysInfo.SenderList(senderCount - 1)
        For i As Integer = 0 To sysInfo.SenderList.Length - 1
            ReDim sysInfo.SenderList(i).TmpIpData(12 - 1)
        Next

        For LEDScreenId As Integer = 0 To screenCount - 1
            'sysInfo.mainClass.GetScreenLocation(LEDScreenId, x, y, width, height)

            '屏幕索引
            'screenMain(LEDScreenIndex).index = LEDScreenIndex
            '获取起始位置 大小
            sysInfo.MainClass.GetScreenLocation(LEDScreenId,
                                            x,
                                            y,
                                            sysInfo.ScreenList(LEDScreenId).DefaultWidth,
                                            sysInfo.ScreenList(LEDScreenId).DefaultHeight)
            '屏幕单元宽度
            sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Width
            '屏幕单元高度
            sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Height
            ''TODO:自动判断行列数
            'Debug.WriteLine($"{LEDScreenId} {sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth},{sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight}")

            If sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth * 4 = sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight Then
                '4 1
                sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum = 4
                sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum = 1
            ElseIf sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth = sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight * 4 Then
                '1 4
                sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum = 1
                sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum = 4
            ElseIf sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth = sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight Then
                '4 4
                sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum = 4
                sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum = 4
            End If
            'Debug.WriteLine($"{LEDScreenId} {sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum},{sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum}")

            '创建上次点击状态缓存
            ReDim sysInfo.ScreenList(LEDScreenId).ClickHistoryArray((sysInfo.ScreenList(LEDScreenId).DefaultHeight \ sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight) * sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum,
                                                                   (sysInfo.ScreenList(LEDScreenId).DefaultWidth \ sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth) * sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum)

            'putlog($"{LEDScreenIndex}:{(screenMain(LEDScreenIndex).height \ screenMain(LEDScreenIndex).ScanBoardHeight) * 4},{(screenMain(LEDScreenIndex).width \ screenMain(LEDScreenIndex).ScanBoardWidth) * 4}")

            '            Showinfo($">>>>{sysInfo.Language.GetLanguage("显示屏")}{LEDScreenId}: start[{sysInfo.ScreenList(LEDScreenId).DefaultX},{sysInfo.ScreenList(LEDScreenId).DefaultY}]
            'size[{sysInfo.ScreenList(LEDScreenId).DefaultWidth},{sysInfo.ScreenList(LEDScreenId).DefaultHeight}]
            'touch[{sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth},{sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight}]")
            '            Showinfo($"        {sysInfo.Language.GetLanguage("屏幕块")}[{LEDScreenInfoList(LEDScreenId).ScanBoardInfoList.Count}]")

            sysInfo.ScreenList(LEDScreenId).SenderList = New List(Of Integer)
            '遍历屏幕
            'Dim tmpIndex As Integer = 0
            For Each i In LEDScreenInfoList(LEDScreenId).ScanBoardInfoList
                '接收卡留空则不添加
                If i.SenderIndex = &HFF Then
                    Continue For
                End If

                sysInfo.ScreenList(LEDScreenId).SenderList.Add(i.SenderIndex)

                'Dim itm As ListViewItem = ListView3.Items.Add($"{LEDScreenIndex} {i.SenderIndex}", 0)
                'itm.SubItems.Add($"{i.PortIndex}")
                'itm.SubItems.Add($"{i.ConnectIndex}")
                'itm.SubItems.Add($"{i.X},{i.Y} [{i.Width},{i.Height}]")

                Dim tmpScanBoardInfo As ScanBoardInfo
                '屏幕索引
                tmpScanBoardInfo.ScreenId = LEDScreenId
                '控制器索引
                tmpScanBoardInfo.SenderId = i.SenderIndex
                '在屏幕数组的下表
                'tmpScanBoardInfo.linkIndex = tmpIndex
                'tmpIndex += 1
                '网口索引
                tmpScanBoardInfo.PortId = i.PortIndex
                '接收卡索引
                tmpScanBoardInfo.ConnectId = i.ConnectIndex
                '屏幕块位置
                tmpScanBoardInfo.X = (i.X \ sysInfo.ScreenList(LEDScreenId).DefaultScanBoardWidth) * sysInfo.ScreenList(LEDScreenId).TouchPieceColumnsNum
                tmpScanBoardInfo.Y = (i.Y \ sysInfo.ScreenList(LEDScreenId).DefaultScanBoardHeight) * sysInfo.ScreenList(LEDScreenId).TouchPieceRowsNum
                'putlog($"{tmpScanBoardInfo.ConnectIndex}:{tmpScanBoardInfo.Y},{tmpScanBoardInfo.X}")

                sysInfo.ScanBoardTable.Add($"{i.SenderIndex}-{i.PortIndex}-{i.ConnectIndex}", tmpScanBoardInfo)

                'showinfo($"        {tmpScanBoardInfo.ScreenIndex}-{tmpScanBoardInfo.SenderIndex}-{tmpScanBoardInfo.PortIndex}-{tmpScanBoardInfo.ConnectIndex}: [{tmpScanBoardInfo.X},{tmpScanBoardInfo.Y}]")
            Next

            '缩放
            'If sysInfo.zoomFlage Then
            '缩放比例
            'Dim zoomProportionWidth As Double = 1 / zoomWidth ' Screen.PrimaryScreen.Bounds.Width / zoomWidth
            'Dim zoomProportionHeight As Double = 1 / zoomHeight ' Screen.PrimaryScreen.Bounds.Height / zoomHeight
            With sysInfo.ScreenList(LEDScreenId)
                .X = sysInfo.ScreenList(LEDScreenId).DefaultX / sysInfo.ZoomProportion
                .Y = .DefaultY / sysInfo.ZoomProportion
                .Width = .DefaultWidth / sysInfo.ZoomProportion
                .Height = .DefaultHeight / sysInfo.ZoomProportion
                .ScanBoardWidth = .DefaultScanBoardWidth / sysInfo.ZoomProportion
                .ScanBoardHeight = .DefaultScanBoardHeight / sysInfo.ZoomProportion
                .TouchPieceHeight = .DefaultScanBoardHeight / .TouchPieceRowsNum / sysInfo.ZoomProportion
                .TouchPieceWidth = .DefaultScanBoardWidth / .TouchPieceColumnsNum / sysInfo.ZoomProportion
            End With
        Next

        sysInfo.MainClass.GetEquipmentIP(0)
    End Sub

    ''' <summary>
    ''' 后台线程
    ''' </summary>
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        '判断诺瓦服务是否启动
        If System.Diagnostics.Process.GetProcessesByName("MarsServerProvider").Length = 0 Then
            Dim tmpProcessHwnd As Process = Process.Start($".\Nova\Server\MarsServerProvider.exe")
            Showinfo($"{sysInfo.Language.GetLanguage("启动Nova服务")}：{If(tmpProcessHwnd.Handle, True, False)}")
            Thread.Sleep(5000)
        End If

        NovaInitialize("")
    End Sub
End Class