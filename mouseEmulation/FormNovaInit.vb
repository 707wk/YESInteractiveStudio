Imports System.ComponentModel
Imports System.Threading
Imports Nova.Mars.SDK

Public Class FormNovaInit
    '自动滚动到最底端
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        TextBox1.SelectionStart = TextBox1.Text.Length
        TextBox1.ScrollToCaret()
    End Sub

    Private Sub FormNovaInit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Dim tmpProcessHwnd As Process = Process.Start($".\Server\MarsServerProvider.exe")
        'Me.TextBox1.AppendText($"启动Nova服务：{If(tmpProcessHwnd.Handle, True, False)}")

        'BackgroundWorker1.WorkerReportsProgress = True
    End Sub

    Private Sub FormNovaInit_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        '启动后台线程初始化数据
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    '获取ip通知
    Private Sub GetEquipmentIPData(sender As Object, e As MarsEquipmentIPEventArgs)
        Static Dim senderArrayIndex As Integer = 0
        If e.IsExecResult Then

            senderArray(senderArrayIndex).index = senderArrayIndex
            senderArray(senderArrayIndex).ipDate = e.Data

            showinfo($"控制器{senderArrayIndex}")
            showinfo($"    ip:{e.Data(3)}.{e.Data(2)}.{e.Data(1)}.{e.Data(0)}")
            showinfo($"    掩码:{e.Data(7)}.{e.Data(6)}.{e.Data(5)}.{e.Data(4)}")
            showinfo($"    网关:{e.Data(11)}.{e.Data(10)}.{e.Data(9)}.{e.Data(8)}")

            senderArrayIndex += 1
            If senderArrayIndex < senderArray.Length Then
                mainClass.GetEquipmentIP(senderArrayIndex)
            Else
                showinfo($"加载完成")
                Me.closeDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
            End If
        Else
            showinfo("ERROR:获取设备IP失败！请检查设备后，重新启动程序")
        End If
    End Sub

    '关闭窗体
    Public Delegate Sub closeDialogCallback(ByVal text As String)
    Public Sub closeDialog(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New closeDialogCallback(AddressOf closeDialog)
            Me.Invoke(d, New Object() {text})
        Else
            Me.Close()
        End If
    End Sub

    '显示信息
    Public Delegate Sub showinfoCallback(ByVal text As String)
    Public Sub showinfo(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New showinfoCallback(AddressOf showinfo)
            Me.Invoke(d, New Object() {text})
        Else
            TextBox1.AppendText($"{text}{vbCrLf}")
            '有错误则提示并退出
            If text.IndexOf("ERROR") = -1 Then
                Exit Sub
            End If

            MsgBox($"{text}")

            Me.Close()
            Form1.Close()
            'Application.Exit()
            'System.Environment.Exit(0)
        End If
    End Sub

    '读取屏幕信息
    Public Delegate Sub novaInitializeCallback(ByVal text As String)
    Public Sub novaInitialize(ByVal text As String)
        If Me.InvokeRequired Then
            Dim d As New novaInitializeCallback(AddressOf novaInitialize)
            Me.Invoke(d, New Object() {text})
        Else
            Dim LEDScreenInfoList As List(Of LEDScreenInfo) = Nothing

            'Dim tmpProcessHwnd As Process = Process.Start($".\Server\MarsServerProvider.exe")
            'BackgroundWorker1.ReportProgress(1, $"启动Nova服务：{If(tmpProcessHwnd.Handle, True, False)}")
            'Thread.Sleep(3000)

            showinfo("连接Nova服务中")
            rootClass = New MarsHardwareEnumerator

            If rootClass.Initialize() Then
                showinfo($"连接Nova服务成功")
            Else
                showinfo($"ERROR:连接Nova服务失败")
                'Application.Exit()
                Exit Sub
            End If

            showinfo("查找控制系统中")

            Dim SystemCount As Integer = rootClass.CtrlSystemCount()
            If SystemCount Then
                showinfo($"控制系统数:{SystemCount}")
            Else
                showinfo($"ERROR:未找到控制系统")
                'Application.Exit()
                Exit Sub
            End If

            mainClass = New MarsControlSystem(rootClass)
            '绑定读取到ip事件
            AddHandler mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData

            Dim screenCount As Integer
            Dim senderCount As Integer
            Dim tmpstr As String = Nothing
            rootClass.GetComNameOfControlSystem(0, tmpstr)
            showinfo($"初始化屏幕:{mainClass.Initialize(tmpstr, screenCount, senderCount)}")
            showinfo($"显示屏个数:{screenCount} 控制器个数:{senderCount}")

            If senderCount = 0 Then
                showinfo($"ERROR:未找到控制器")
                Exit Sub
            End If

            showinfo("读取显示屏信息中")
            If mainClass.ReadLEDScreenInfo(LEDScreenInfoList) Then
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
            '获取到的显示屏宽度
            Dim width As Integer
            '获取到的显示屏高度
            Dim height As Integer

            showinfo($"载入屏幕信息中")
            ScanBoardTable = New Hashtable
            ReDim screenMain(screenCount - 1)
            ReDim senderArray(senderCount - 1)
            For i As Integer = 0 To senderArray.Length - 1
                ReDim senderArray(i).tmpIpData(12 - 1)
            Next

            For LEDScreenIndex As Integer = 0 To screenCount - 1
                mainClass.GetScreenLocation(LEDScreenIndex, x, y, width, height)

                '屏幕索引
                screenMain(LEDScreenIndex).index = LEDScreenIndex
                '获取起始位置 大小
                mainClass.GetScreenLocation(LEDScreenIndex,
                                            screenMain(LEDScreenIndex).x,
                                            screenMain(LEDScreenIndex).y,
                                            screenMain(LEDScreenIndex).width,
                                            screenMain(LEDScreenIndex).height)
                '带载宽度
                screenMain(LEDScreenIndex).ScanBoardWidth = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Width
                '带载高度
                screenMain(LEDScreenIndex).ScanBoardHeight = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Height

                showinfo($">>>>显示屏{screenMain(LEDScreenIndex).index}: start[{screenMain(LEDScreenIndex).x},{screenMain(LEDScreenIndex).y}] size[{screenMain(LEDScreenIndex).width},{screenMain(LEDScreenIndex).height}] touch[{screenMain(LEDScreenIndex).ScanBoardWidth},{screenMain(LEDScreenIndex).ScanBoardHeight}]")
                showinfo($"        屏幕块[{LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList.Count}]")
                '遍历屏幕
                For Each i In LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList
                    'SenderIndexlist.Add(i.SenderIndex)

                    'Dim itm As ListViewItem = ListView3.Items.Add($"{LEDScreenIndex} {i.SenderIndex}", 0)
                    'itm.SubItems.Add($"{i.PortIndex}")
                    'itm.SubItems.Add($"{i.ConnectIndex}")
                    'itm.SubItems.Add($"{i.X},{i.Y} [{i.Width},{i.Height}]")

                    Dim tmpScanBoardInfo As ScanBoardInfo
                    '屏幕索引
                    tmpScanBoardInfo.ScreenIndex = LEDScreenIndex
                    '控制器索引
                    tmpScanBoardInfo.SenderIndex = i.SenderIndex
                    '网口索引
                    tmpScanBoardInfo.PortIndex = i.PortIndex
                    '接收卡索引
                    tmpScanBoardInfo.ConnectIndex = i.ConnectIndex
                    '屏幕块位置
                    tmpScanBoardInfo.X = i.X
                    tmpScanBoardInfo.Y = i.Y

                    ScanBoardTable.Add($"{i.SenderIndex}-{i.PortIndex}-{i.ConnectIndex}", tmpScanBoardInfo)

                    'showinfo($"        {tmpScanBoardInfo.ScreenIndex}-{tmpScanBoardInfo.SenderIndex}-{tmpScanBoardInfo.PortIndex}-{tmpScanBoardInfo.ConnectIndex}: [{tmpScanBoardInfo.X},{tmpScanBoardInfo.Y}]")
                Next
            Next

            'showinfo($"------------------------------")
            'For Each i In ScanBoardTable.Keys
            '    Dim tmp As ScanBoardInfo = ScanBoardTable.Item(i)

            '    showinfo($"   {tmp.ScreenIndex},{tmp.SenderIndex},{tmp.PortIndex},{tmp.ConnectIndex},{tmp.X},{tmp.Y}")
            'Next

            mainClass.GetEquipmentIP(0)
            'showinfo($"加载完成")
            'MsgBox("加载完成")
            'Thread.Sleep(1000)
            'Me.Close()
        End If
    End Sub

    '后台线程
    Private Sub BackgroundWorker1_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        If System.Diagnostics.Process.GetProcessesByName("MarsServerProvider").Length = 0 Then
            Dim tmpProcessHwnd As Process = Process.Start($".\Server\MarsServerProvider.exe")
            showinfo($"启动Nova服务：{If(tmpProcessHwnd.Handle, True, False)}")
            Thread.Sleep(5000)
        End If

        novaInitialize("")

    End Sub

    'Private Sub FormNovaInit_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
    '    putlog("close")
    'End Sub

    '线程消息处理
    'Private Sub BackgroundWorker1_ProgressChanged(sender As Object, e As ProgressChangedEventArgs) Handles BackgroundWorker1.ProgressChanged
    '    TextBox1.AppendText($"{e.UserState.ToString}{vbCrLf}")
    '    '有错误则提示并退出
    '    If e.UserState.ToString.IndexOf("ERROR") = -1 Then
    '        Exit Sub
    '    End If

    '    MsgBox($"{e.UserState.ToString}")
    '    Application.Exit()
    'End Sub

    '读取完成
    'Private Sub BackgroundWorker1_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker1.RunWorkerCompleted
    '    Thread.Sleep(2000)
    '    Me.Close()
    'End Sub
End Class