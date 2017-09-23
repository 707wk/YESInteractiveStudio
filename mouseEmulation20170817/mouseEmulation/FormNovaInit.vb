Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports Nova.Mars.SDK

Public Class FormNovaInit
    '自动滚动到最底端
    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged
        TextBox1.SelectionStart = TextBox1.Text.Length
        TextBox1.ScrollToCaret()
    End Sub

    Private Sub FormNovaInit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Width = Me.Width
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

            'senderArray(senderArrayIndex).index = senderArrayIndex
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
                '移除事件
                RemoveHandler mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
                Me.closeDialog("真是哔了狗了，这个事件居然是另一个线程触发的")
            End If
        Else
            '移除事件
            RemoveHandler mainClass.GetEquipmentIPDataEvent, AddressOf GetEquipmentIPData
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

            MsgBox($"{text}", MsgBoxStyle.Information, Me.Text)

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
            '加密狗验证
            'checkdog()

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
                'screenMain(LEDScreenIndex).index = LEDScreenIndex
                '获取起始位置 大小
                mainClass.GetScreenLocation(LEDScreenIndex,
                                            screenMain(LEDScreenIndex).x,
                                            screenMain(LEDScreenIndex).y,
                                            screenMain(LEDScreenIndex).width,
                                            screenMain(LEDScreenIndex).height)
                '屏幕单元宽度
                screenMain(LEDScreenIndex).ScanBoardWidth = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Width
                '屏幕单元高度
                screenMain(LEDScreenIndex).ScanBoardHeight = LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList(0).Height
                '创建上次点击状态缓存
                ReDim screenMain(LEDScreenIndex).clickHistoryArray((screenMain(LEDScreenIndex).height \ screenMain(LEDScreenIndex).ScanBoardHeight) * 4,
                                                                   (screenMain(LEDScreenIndex).width \ screenMain(LEDScreenIndex).ScanBoardWidth) * 4)

                'putlog($"{LEDScreenIndex}:{(screenMain(LEDScreenIndex).height \ screenMain(LEDScreenIndex).ScanBoardHeight) * 4},{(screenMain(LEDScreenIndex).width \ screenMain(LEDScreenIndex).ScanBoardWidth) * 4}")

                showinfo($">>>>显示屏{LEDScreenIndex}: start[{screenMain(LEDScreenIndex).x},{screenMain(LEDScreenIndex).y}] size[{screenMain(LEDScreenIndex).width},{screenMain(LEDScreenIndex).height}] touch[{screenMain(LEDScreenIndex).ScanBoardWidth},{screenMain(LEDScreenIndex).ScanBoardHeight}]")
                showinfo($"        屏幕块[{LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList.Count}]")

                screenMain(LEDScreenIndex).Senderlist = New List(Of Integer)
                '遍历屏幕
                For Each i In LEDScreenInfoList(LEDScreenIndex).ScanBoardInfoList
                    screenMain(LEDScreenIndex).Senderlist.Add(i.SenderIndex)

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
                    tmpScanBoardInfo.X = (i.X \ screenMain(LEDScreenIndex).ScanBoardWidth) * 4
                    tmpScanBoardInfo.Y = (i.Y \ screenMain(LEDScreenIndex).ScanBoardHeight) * 4
                    'putlog($"{tmpScanBoardInfo.ConnectIndex}:{tmpScanBoardInfo.Y},{tmpScanBoardInfo.X}")

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

            '反序列化
            Dim fStream As FileStream = Nothing
            Dim sfFormatter As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter

            Try
                fStream = New FileStream("screen.ini", FileMode.Open)
            Catch ex As Exception
                'screen.ini不存在会引发异常，但没关系
            End Try

            Try
                If fStream IsNot Nothing Then
                    systeminfo = sfFormatter.Deserialize(fStream)
                End If
            Catch ex As Exception
                showinfo($"ERROR:文件读取失败")
                putlog(ex.Message)
                '打开版本不同或错误的文件则无法读取
            End Try

            Try
                fStream.Close()
            Catch ex As Exception
            End Try

            If systeminfo.playList IsNot Nothing Then
                For i As Integer = 0 To systeminfo.playList.Length - 1
                    '判断设置屏幕索引是否超过读取到的屏幕索引最大值
                    If i > screenMain.Length - 1 Then
                        Exit For
                    End If

                    '赋值上次关闭前数据
                    screenMain(i).showFlage = systeminfo.playList(i).showFlage
                    screenMain(i).remark = systeminfo.playList(i).remark
                    'screenMain(i).filePath = systeminfo.playList(i).filePath
                Next
            End If

            '多余的
            'If systeminfo.filesList IsNot Nothing Then
            '    playFilesList = systeminfo.filesList
            'End If

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

    '写入加密狗数据
    Private Sub register(KeyHandle As Integer, hashcode As String)
        Dim pageNo As Integer = 0
        Dim startAdd As Integer = 0
        Dim pBuffer() As Byte = System.Text.Encoding.Unicode.GetBytes(hashcode)

        If SmartTimeWritePageFile(KeyHandle, 0, 0, 64, pBuffer(0)) <> 0 Then
            'putlog("写该分页失败,错误代码：" & SmartTimeGetLastError())
            End
        End If
    End Sub

    '检测加密狗
    Private Sub checkdog()
        If System.IO.File.Exists("SmartTimeApp.dll") = False Then
            'Me.Close()
            'Form1.Close()
            End
        End If

        Dim KeyHandle(8) As Integer
        Dim KeyNum As Integer
        Dim GUID(32) As Byte
        Dim sGUID As String = Nothing

        If SmartTimeFind("ME触摸地砖屏控制系统", KeyHandle(0), KeyNum) <> 0 Then
            'putlog("查找加密锁失败,错误码是：" & SmartTimeGetLastError())
            'Me.Close()
            'Form1.Close()
            End
        End If
        'putlog("查找到加密锁的个数是：" & KeyNum & vbCrLf)

        If SmartTimeGetUid(KeyHandle(0), GUID(0)) <> 0 Then
            'putlog("获取加密锁的硬件ID失败,错误代码是：" & SmartTimeGetLastError())
            'Me.Close()
            'Form1.Close()
            End
        End If

        For i = 0 To 32 - 1
            sGUID = sGUID & Chr(GUID(i))
        Next

        Dim uPin1 As Integer = &H5025479E
        Dim uPin2 As Integer = &HDE5E769B
        Dim uPin3 As Integer = &HA5D52993
        Dim uPin4 As Integer = &H84878D64

        If SmartTimeOpen(KeyHandle(0), uPin1, uPin2, uPin3, uPin4) <> 0 Then
            'putlog("打开加密锁失败,错误代码是：" & SmartTimeGetLastError())
            'Me.Close()
            'Form1.Close()
            End
        End If

        Dim pageNo As Integer = 0
        Dim startAdd As Integer = 0
        Dim pBuffer(64) As Byte
        Dim hashcode As String
        'Dim descBytes() As Byte = System.Text.Encoding.Unicode.GetBytes("FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF")

        If SmartTimeReadPageFile(KeyHandle(0), pageNo, startAdd, 64, pBuffer(0)) <> 0 Then
            'putlog("读该分页失败,错误代码：" & SmartTimeGetLastError())
            'Me.Close()
            'Form1.Close()
            End
        End If

        Dim zeroNum As Integer = 0
        For i As Integer = 0 To 64 - 1
            If pBuffer(i) = 0 Then
                zeroNum += 1
            End If
        Next

        If zeroNum = 64 Then
            register(KeyHandle(0), getMd5Hash(sGUID & "YESTECH"))
        Else
            hashcode = System.Text.Encoding.Unicode.GetString(pBuffer, 0, 64)
            If hashcode.Equals(getMd5Hash(sGUID & "YESTECH")) = False Then
                'Me.Close()
                'Form1.Close()
                End
            End If
        End If

        SmartTimeClose(KeyHandle(0))
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