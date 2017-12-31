Imports System.Threading
Imports Nova.Mars.SDK

Public Class FormNovaInit
    ''' <summary>
    ''' 自动滚动到最底端
    ''' </summary>
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        TextBox1.SelectionStart = TextBox1.Text.Length
        TextBox1.ScrollToCaret()
    End Sub

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
            sysInfo.senderList(senderArrayId).ipDate = e.Data

            showinfo($"控制器{senderArrayId}")
            showinfo($"    ip:{e.Data(3)}.{e.Data(2)}.{e.Data(1)}.{e.Data(0)}")
            showinfo($"    掩码:{e.Data(7)}.{e.Data(6)}.{e.Data(5)}.{e.Data(4)}")
            showinfo($"    网关:{e.Data(11)}.{e.Data(10)}.{e.Data(9)}.{e.Data(8)}")

            senderArrayId += 1
            If senderArrayId < sysInfo.senderList.Length Then
                sysInfo.mainClass.GetEquipmentIP(senderArrayId)
            Else
                showinfo($"加载完成")
                '移除事件
                RemoveHandler sysInfo.mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.closeDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
            End If
        Else
            '移除事件
            RemoveHandler sysInfo.mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
            showinfo("ERROR:获取设备IP失败！请检查设备后，重新启动程序")
        End If
    End Sub

    '关闭窗体
    Public Delegate Sub closeDialogCallback(ByVal text As String)
    Public Sub closeDialog(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New closeDialogCallback(AddressOf closeDialog), New Object() {text})
            Exit Sub
        End If

        Me.Close()
    End Sub

    ''' <summary>
    ''' 显示信息
    ''' </summary>
    Public Delegate Sub showinfoCallback(ByVal text As String)
    Public Sub showinfo(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New showinfoCallback(AddressOf showinfo), New Object() {text})
            Exit Sub
        End If

        TextBox1.AppendText($"{text}{vbCrLf}")
        '有错误则提示并退出
        If text.IndexOf("ERROR") = -1 Then
            Exit Sub
        End If

        putlog(text)
        Thread.Sleep(2000)

        '释放nova资源
        If sysInfo.mainClass Is Nothing Then
        Else
            sysInfo.mainClass.UnInitialize()
        End If
        If sysInfo.rootClass Is Nothing Then
        Else
            sysInfo.rootClass.UnInitialize()
        End If

        Application.Exit()
    End Sub

    ''' <summary>
    ''' 读取屏幕信息
    ''' </summary>
    Public Delegate Sub novaInitializeCallback(ByVal text As String)
    Public Sub novaInitialize(ByVal text As String)
        If Me.InvokeRequired Then
            Me.Invoke(New novaInitializeCallback(AddressOf novaInitialize), New Object() {text})
            Exit Sub
        End If

        '加密狗验证
        'checkdog()

        Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing

        showinfo("连接Nova服务中")
        sysInfo.rootClass = New MarsHardwareEnumerator

        If sysInfo.rootClass.Initialize() Then
            showinfo($"连接Nova服务成功")
        Else
            showinfo($"ERROR:连接Nova服务失败")
            'Application.Exit()
            Exit Sub
        End If

        showinfo("查找控制系统中")

        Dim SystemCount As Integer = sysInfo.rootClass.CtrlSystemCount()
        If SystemCount Then
            showinfo($"控制系统数:{SystemCount}")
        Else
            showinfo($"ERROR:未找到控制系统")
            'Application.Exit()
            Exit Sub
        End If

        sysInfo.mainClass = New MarsControlSystem(sysInfo.rootClass)
        '绑定读取到ip事件
        AddHandler sysInfo.mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

        Dim screenCount As Integer
        Dim senderCount As Integer
        Dim tmpstr As String = Nothing
        sysInfo.rootClass.GetComNameOfControlSystem(0, tmpstr)
        showinfo($"初始化屏幕:{sysInfo.mainClass.Initialize(tmpstr, screenCount, senderCount)}")
        showinfo($"显示屏个数:{screenCount} 控制器个数:{senderCount}")

        If senderCount = 0 Then
            showinfo($"ERROR:未找到控制器")
            Exit Sub
        End If

        showinfo("读取显示屏信息中")
        If sysInfo.mainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
            showinfo($"ERROR:读显示屏信息失败")
            Exit Sub
        End If

        If LEDScreenInfoList.Count = 0 Then
            showinfo($"ERROR:未找到显示屏")
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

        showinfo($"载入屏幕信息中")
        sysInfo.ScanBoardTable = New Hashtable
        'ReDim sysInfo.screenList(screenCount - 1)
        For i As Integer = 0 To sysInfo.screenList.Length - 1
            If i < screenCount Then
                sysInfo.screenList(i).existFlage = True
            Else
                sysInfo.screenList(i).existFlage = False
            End If
        Next

        ReDim sysInfo.senderList(senderCount - 1)
        For i As Integer = 0 To sysInfo.senderList.Length - 1
            ReDim sysInfo.senderList(i).tmpIpData(12 - 1)
        Next

        For LEDScreenId As Integer = 0 To screenCount - 1
            'sysInfo.mainClass.GetScreenLocation(LEDScreenId, x, y, width, height)

            '屏幕索引
            'screenMain(LEDScreenIndex).index = LEDScreenIndex
            '获取起始位置 大小
            sysInfo.mainClass.GetScreenLocation(LEDScreenId,
                                            x,
                                            y,
                                            sysInfo.screenList(LEDScreenId).defaultWidth,
                                            sysInfo.screenList(LEDScreenId).defaultHeight)
            '屏幕单元宽度
            sysInfo.screenList(LEDScreenId).defaultScanBoardWidth = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Width
            '屏幕单元高度
            sysInfo.screenList(LEDScreenId).defaultScanBoardHeight = LEDScreenInfoList(LEDScreenId).ScanBoardInfoList(0).Height
            '创建上次点击状态缓存
            ReDim sysInfo.screenList(LEDScreenId).clickHistoryArray((sysInfo.screenList(LEDScreenId).defaultHeight \ sysInfo.screenList(LEDScreenId).defaultScanBoardHeight) * sysInfo.screenList(LEDScreenId).touchPieceRowsNum,
                                                                   (sysInfo.screenList(LEDScreenId).defaultWidth \ sysInfo.screenList(LEDScreenId).defaultScanBoardWidth) * sysInfo.screenList(LEDScreenId).touchPieceColumnsNum)

            'putlog($"{LEDScreenIndex}:{(screenMain(LEDScreenIndex).height \ screenMain(LEDScreenIndex).ScanBoardHeight) * 4},{(screenMain(LEDScreenIndex).width \ screenMain(LEDScreenIndex).ScanBoardWidth) * 4}")

            showinfo($">>>>显示屏{LEDScreenId}: start[_
{sysInfo.screenList(LEDScreenId).defaultX},_
{sysInfo.screenList(LEDScreenId).defaultY}] size[_
{sysInfo.screenList(LEDScreenId).defaultWidth},_
{sysInfo.screenList(LEDScreenId).defaultHeight}] touch[_
{sysInfo.screenList(LEDScreenId).defaultScanBoardWidth},_
{sysInfo.screenList(LEDScreenId).defaultScanBoardHeight}]")
            showinfo($"        屏幕块[{LEDScreenInfoList(LEDScreenId).ScanBoardInfoList.Count}]")

            sysInfo.screenList(LEDScreenId).SenderList = New List(Of Integer)
            '遍历屏幕
            'Dim tmpIndex As Integer = 0
            For Each i In LEDScreenInfoList(LEDScreenId).ScanBoardInfoList
                sysInfo.screenList(LEDScreenId).SenderList.Add(i.SenderIndex)

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
                tmpScanBoardInfo.X = (i.X \ sysInfo.screenList(LEDScreenId).defaultScanBoardWidth) * sysInfo.screenList(LEDScreenId).touchPieceColumnsNum
                tmpScanBoardInfo.Y = (i.Y \ sysInfo.screenList(LEDScreenId).defaultScanBoardHeight) * sysInfo.screenList(LEDScreenId).touchPieceRowsNum
                'putlog($"{tmpScanBoardInfo.ConnectIndex}:{tmpScanBoardInfo.Y},{tmpScanBoardInfo.X}")

                sysInfo.ScanBoardTable.Add($"{i.SenderIndex}-{i.PortIndex}-{i.ConnectIndex}", tmpScanBoardInfo)

                'showinfo($"        {tmpScanBoardInfo.ScreenIndex}-{tmpScanBoardInfo.SenderIndex}-{tmpScanBoardInfo.PortIndex}-{tmpScanBoardInfo.ConnectIndex}: [{tmpScanBoardInfo.X},{tmpScanBoardInfo.Y}]")
            Next

            '缩放
            'If sysInfo.zoomFlage Then
            '缩放比例
            'Dim zoomProportionWidth As Double = 1 / zoomWidth ' Screen.PrimaryScreen.Bounds.Width / zoomWidth
            'Dim zoomProportionHeight As Double = 1 / zoomHeight ' Screen.PrimaryScreen.Bounds.Height / zoomHeight
            With sysInfo.screenList(LEDScreenId)
                .x = sysInfo.screenList(LEDScreenId).defaultX / sysInfo.zoomProportion
                .y = .defaultY / sysInfo.zoomProportion
                .width = .defaultWidth / sysInfo.zoomProportion
                .height = .defaultHeight / sysInfo.zoomProportion
                .ScanBoardWidth = .defaultScanBoardWidth / sysInfo.zoomProportion
                .ScanBoardHeight = .defaultScanBoardHeight / sysInfo.zoomProportion
                .touchPieceHeight = .defaultScanBoardHeight / .touchPieceRowsNum / sysInfo.zoomProportion
                .touchPieceWidth = .defaultScanBoardWidth / .touchPieceColumnsNum / sysInfo.zoomProportion
            End With
        Next

        sysInfo.mainClass.GetEquipmentIP(0)
    End Sub

    ''' <summary>
    ''' 后台线程
    ''' </summary>
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If System.Diagnostics.Process.GetProcessesByName("MarsServerProvider").Length = 0 Then
            Dim tmpProcessHwnd As Process = Process.Start($".\Nova\Server\MarsServerProvider.exe")
            showinfo($"启动Nova服务：{If(tmpProcessHwnd.Handle, True, False)}")
            Thread.Sleep(5000)
        End If

        novaInitialize("")
    End Sub
End Class