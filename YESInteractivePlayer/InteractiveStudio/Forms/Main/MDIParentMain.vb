Imports System.ComponentModel
Imports System.Net.Sockets
Imports DevComponents.DotNetBar

Public Class MDIParentMain
#Region "注册/注销热键"
    ''' <summary>
    ''' 声明注册热键API函数
    ''' </summary>
    Public Declare Function RegisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer,
                                                    ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    ''' <summary>
    ''' 声明注销热键API函数
    ''' </summary>
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hWnd As Integer, ByVal id As Integer) As Integer
    ''' <summary>
    ''' 热键消息ID
    ''' </summary>
    Public Const WM_HOTKEY As Short = &H312S
    ''' <summary>
    ''' ALT
    ''' </summary>
    Public Const MOD_ALT As Short = &H1S
    ''' <summary>
    ''' Ctrl
    ''' </summary>
    Public Const MOD_CONTROL As Short = &H2S
    ''' <summary>
    ''' Shift
    ''' </summary>
    Public Const MOD_SHIFT As Short = &H4S
#End Region

#Region "编辑窗体"
    ''' <summary>
    ''' 窗口编辑
    ''' </summary>
    Dim WindowEditDialog As WindowEdit
    ''' <summary>
    ''' 节目编辑
    ''' </summary>
    Dim ProgramEditDialog As ProgramEdit
#End Region

#Region "显示程序名/版本号/文件路径"
    ''' <summary>
    ''' 显示程序名/版本号/文件路径
    ''' </summary>
    Private Sub ShowToolBarInfo()
        With My.Application.Info
            Me.Text = $"{ .ProductName} V{ .Version.Major}.{ .Version.Minor}.{ .Version.Build} [{If(sysInfo.HistoryFile = "", "未保存", sysInfo.HistoryFile)}]"
        End With
    End Sub
#End Region

#Region "加载窗口列表"
    ''' <summary>
    ''' 加载窗口列表
    ''' </summary>
    Public Sub LoadSchedule()

    End Sub
#End Region

#Region "窗体初始化/关闭"
#Region "初始化"
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "初始化变量"
        LoadSetting()

        With sysInfo
            If .VersionArray Is Nothing Then
                ReDim .VersionArray(3 - 1)
            End If

            .MainForm = Me

            '语言包
            .Language = New Wangk.Resource.MultiLanguage
            .Language.Init(.SelectLang, My.Application.Info.Title)

            '日志
            .logger = New Wangk.Tools.Logger With {
                .writelevel = Wangk.Tools.Loglevel.Level_DEBUG,
                .saveDaysMax = 30
            }
            sysInfo.logger.Init()
        End With

        Timer1.Interval = 1000
#End Region

#Region "样式设置"
        ShowToolBarInfo()

        With TreeView1
            .ShowRootLines = False
            .Indent = 30
            .ImageList = TreeViewImageList
        End With

        '隐藏显示电容按钮
        ButtonItem17.Visible = False

        '窗口编辑窗体
        WindowEditDialog = New WindowEdit With {
            .FormBorderStyle = FormBorderStyle.None,
            .Dock = DockStyle.Fill,
            .TopLevel = False,
            .Parent = Panel1
            }
        Panel1.Controls.Add(WindowEditDialog)

        '节目编辑窗体
        ProgramEditDialog = New ProgramEdit With {
            .FormBorderStyle = FormBorderStyle.None,
            .Dock = DockStyle.Fill,
            .TopLevel = False,
            .Parent = Panel1
            }
        Panel1.Controls.Add(ProgramEditDialog)

        SetLinkControlState(False)

        sysInfo.Language.SetControlslanguage(Me)
#End Region

#Region "注册全局快捷键"
        '注册全局快捷键
        RegisterHotKey(Me.Handle.ToInt32, 1, 0, Keys.F1)
        RegisterHotKey(Me.Handle.ToInt32, 2, 0, Keys.F2)
        RegisterHotKey(Me.Handle.ToInt32, 3, 0, Keys.F3)
        RegisterHotKey(Me.Handle.ToInt32, 4, 0, Keys.F4)
#End Region

        DeviceInit.ShowDialog()

        '加载最后打开文件
        If Not LoadFile(sysInfo.HistoryFile) Then
            sysInfo.WindowList = New List(Of WindowInfo)
        End If

#Region "测试数据"
        'For i001 As Integer = 0 To 2 - 1
        '    Dim Tmpnode001 As New TreeNode With {
        '        .Text = $"窗口{i001}",
        '        .ImageIndex = 0,
        '        .SelectedImageIndex = 0
        '    }

        '    For i002 As Integer = 0 To 4 - 1
        '        Dim Tmpnode002 As New TreeNode With {
        '        .Text = $"节目{i002}",
        '        .ImageIndex = 1,
        '        .SelectedImageIndex = 1
        '    }

        '        Tmpnode001.Nodes.Add(Tmpnode002)
        '    Next

        '    TreeView1.Nodes.Add(Tmpnode001)
        'Next
        'TreeView1.ExpandAll()
#End Region
    End Sub
#End Region

#Region "显示"
    Private Sub MDIParentMain_Shown(sender As Object, e As EventArgs) Handles Me.Shown
#Region "显示新程序更新内容"
        With My.Application.Info
            If sysInfo.VersionArray(0) <> .Version.Major + 1 OrElse
                sysInfo.VersionArray(1) <> .Version.Minor OrElse
                sysInfo.VersionArray(2) <> .Version.Build Then
                MsgBox($"新功能提示                                        
        1.界面优化
        2.性能优化
        3.修复已知问题", MsgBoxStyle.Information, "更新内容")
            End If

            sysInfo.VersionArray(0) = .Version.Major
            sysInfo.VersionArray(1) = .Version.Minor
            sysInfo.VersionArray(2) = .Version.Build
        End With
#End Region
    End Sub
#End Region

#Region "切换显示模式"
#Region "切换播放窗体显示模式"
#Region "更新按钮图标"
    ''' <summary>
    ''' 更新按钮图标
    ''' </summary>
    ''' <param name="Mode"></param>
    Public Sub SwitchDisplayModeIco(ByVal Mode As InteractiveOptions.DISPLAYMODE)
        Dim ButtonArray As ButtonItem() = {
            ButtonItem14,
            ButtonItem15,
            ButtonItem16,
            ButtonItem17
        }

        Dim ImageMap As Image(,) = {
            {My.Resources.DisplayMode0_32px, My.Resources.DisplayMode0G_32px},
            {My.Resources.DisplayMode1_32px, My.Resources.DisplayMode1G_32px},
            {My.Resources.DisplayMode2_32px, My.Resources.DisplayMode2G_32px},
            {My.Resources.DisplayMode3_32px, My.Resources.DisplayMode3G_32px}
        }

        For i001 As Integer = 0 To ButtonArray.Count - 1
            ButtonArray(i001).Image = ImageMap(i001, If(Mode = i001, 0, 1))
        Next
    End Sub
#End Region

    ''' <summary>
    ''' 切换播放窗体显示模式
    ''' </summary>
    ''' <param name="Mode"></param>
    Public Sub SwitchDisplayMode(ByVal Mode As InteractiveOptions.DISPLAYMODE)
        sysInfo.DisplayMode = Mode

        SwitchDisplayModeIco(Mode)

        For Each i001 As WindowInfo In sysInfo.WindowList
            i001.PlayDialog.SwitchDisplayMode(Modal)
        Next
    End Sub
#End Region

#Region "切换按钮"
    Private Sub ButtonItem14_Click(sender As Object, e As EventArgs) Handles ButtonItem14.Click
        SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.INTERACT)
    End Sub

    Private Sub ButtonItem15_Click(sender As Object, e As EventArgs) Handles ButtonItem15.Click
        SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.TEST)
    End Sub

    Private Sub ButtonItem16_Click(sender As Object, e As EventArgs) Handles ButtonItem16.Click
        SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.BLACK)
    End Sub

    Private Sub ButtonItem17_Click(sender As Object, e As EventArgs) Handles ButtonItem17.Click
        If ButtonItem17.Visible = False Then
            Exit Sub
        End If

        SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.DEBUG)
    End Sub
#End Region

#Region "热键消息处理函数"
    ''' <summary>
    ''' 热键消息处理函数
    ''' </summary>
    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY And sysInfo.LinkFlage Then '判断是否为热键消息
            Select Case m.WParam.ToInt32 '判断热键消息的注册ID
                Case 1
                    SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.INTERACT)
                Case 2
                    SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.TEST)
                Case 3
                    SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.BLACK)
                Case 4
                    If ButtonItem17.Visible = False Then
                        Exit Sub
                    End If

                    SwitchDisplayMode(InteractiveOptions.DISPLAYMODE.DEBUG)
            End Select
        End If

        MyBase.WndProc(m) '循环监听消息
    End Sub
#End Region
#End Region

#Region "关闭"
    Private Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
#Region "退出前保存文件"
        Select Case MsgBox("是否保存修改?", MsgBoxStyle.YesNoCancel, "保存")
            Case MsgBoxResult.Yes '保存
                If sysInfo.HistoryFile = "" Then
                    Dim tmp1 As New SaveFileDialog
                    tmp1.Filter = "Schedule File|*.xml"
                    If tmp1.ShowDialog() <> DialogResult.OK Then
                        Exit Sub
                    End If

                    sysInfo.HistoryFile = tmp1.FileName
                End If

                SaveFile(sysInfo.HistoryFile)

                ''todo:断开连接
                DisconnectControl()

            Case MsgBoxResult.No '不保存
                ''todo:断开连接
                DisconnectControl()

            Case MsgBoxResult.Cancel '取消
                e.Cancel = True
                Exit Sub
        End Select
#End Region

#Region "注销全局快捷键"
        '注销全局快捷键
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F1)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F2)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F3)
        UnregisterHotKey(Me.Handle.ToInt32, Keys.F4)
#End Region

#Region "关闭播放窗体"
        '关闭播放窗体
        For Each i001 As WindowInfo In sysInfo.WindowList
            i001.PlayDialog.Close(True)
        Next
#End Region

        SaveSetting()

#Region "释放nova资源"
        '释放nova资源
        Try
            sysInfo.MainClass.UnInitialize()
        Catch ex As Exception
        End Try
        Try
            sysInfo.RootClass.UnInitialize()
        Catch ex As Exception
        End Try
#End Region
    End Sub
#End Region
#End Region

#Region "右键选中"
    ''' <summary>
    ''' 右键选中
    ''' </summary>
    Private Sub TreeView1_MouseDown(sender As Object, e As MouseEventArgs) Handles TreeView1.MouseDown
        Select Case True
            Case e.Button <> System.Windows.Forms.MouseButtons.Right
            Case IsNothing(TreeView1.GetNodeAt(e.X, e.Y)) = False
                TreeView1.SelectedNode = TreeView1.GetNodeAt(e.X, e.Y)
        End Select
    End Sub
#End Region

#Region "点击窗口/节目"
    ''' <summary>
    ''' 点击窗口/节目
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub TreeView1_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TreeView1.NodeMouseClick
        Select Case e.Node.Level
            Case 0
                '窗口
                GroupBox2.Text = "Window"
                WindowEditDialog.Show()
                ProgramEditDialog.Hide()
            Case 1
                '节目
                GroupBox2.Text = "Program"
                WindowEditDialog.Hide()
                ProgramEditDialog.Show()
        End Select
    End Sub
#End Region

#Region "连接/断开操作"
#Region "连接"
    ''' <summary>
    ''' 连接操作
    ''' </summary>
    Public Sub SetLinkControl()
        If sysInfo.LinkFlage Then
            Exit Sub
        End If

        '无窗口则不处理
        If sysInfo.WindowList.Count = 0 Then
            Exit Sub
        End If

        'todo:计算缩放后尺寸


        If Not ConnectControl() Then
            Exit Sub
        End If

        SetLinkControlState(sysInfo.LinkFlage)
    End Sub

    Private Sub ButtonItem18_Click(sender As Object, e As EventArgs) Handles ButtonItem18.Click
        SetLinkControl()
    End Sub
#End Region

#Region "断开"
    ''' <summary>
    ''' 断开操作
    ''' </summary>
    Public Sub SetOffLinkControl()
        If Not sysInfo.LinkFlage Then
            Exit Sub
        End If

        DisconnectControl()

        SetLinkControlState(sysInfo.LinkFlage)
    End Sub

    Private Sub ButtonItem19_Click(sender As Object, e As EventArgs) Handles ButtonItem19.Click
        SetOffLinkControl()
    End Sub
#End Region

#Region "设置控件状态"
    ''' <summary>
    ''' 设置控件状态
    ''' </summary>
    ''' <param name="State"></param>
    Public Sub SetLinkControlState(ByVal State As Boolean)
        '文件操作
        ButtonItem20.Enabled = Not State
        ButtonItem21.Enabled = Not State
        ButtonItem22.Enabled = Not State
        ButtonItem23.Enabled = Not State

        '连接/断开
        ButtonItem18.Enabled = Not State
        ButtonItem19.Enabled = State

        '显示模式
        ButtonItem14.Enabled = State
        ButtonItem15.Enabled = State
        ButtonItem16.Enabled = State
        ButtonItem17.Enabled = State

        '触摸灵敏度
        ComboBoxItem10.Enabled = Not State
        '温度复位
        ComboBoxItem7.Enabled = Not State
        '时间复位
        ComboBoxItem8.Enabled = Not State

        '控制器
        ButtonItem27.Enabled = Not State
        '接收卡
        ButtonItem28.Enabled = Not State

        '添加窗口
        ToolStripButton1.Enabled = Not State

        If State Then
            Timer1.Start()
        Else
            Timer1.Stop()
        End If
    End Sub
#End Region
#End Region

#Region "定时处理"
#Region "修正查询时间间隔"
    ''' <summary>
    ''' 修正查询时间间隔
    ''' </summary>
    Public Sub CorrectionInquireTime()
        Dim minReadNum As Integer = &HFFFF

        For Each i001 As SenderInfo In sysInfo.SenderList
            With i001
                If Not .LinkFlage Then
                    Continue For
                End If

                minReadNum = If(minReadNum > .MaxReadNum, minReadNum, .MaxReadNum)
            End With
        Next

        If minReadNum < 40 AndAlso
            sysInfo.InquireTimeSec > 0 Then

            sysInfo.InquireTimeSec -= 1
        ElseIf minReadNum > 42 AndAlso
             sysInfo.InquireTimeSec < 1000 Then

            sysInfo.InquireTimeSec += 1
        End If
    End Sub
#End Region

#Region "节目定时切换"
    'todo:节目定时切换
#End Region

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        CorrectionInquireTime()


    End Sub
#End Region

#Region "控制器离线"
    Public Delegate Sub DisposeControlOffLinkCallback(ByVal ControlID As Integer)
    ''' <summary>
    ''' 控制器离线处理
    ''' </summary>
    Public Sub DisposeControlOffLink(ByVal ControlID As Integer)
        If Me.InvokeRequired Then
            Me.Invoke(New DisposeControlOffLinkCallback(AddressOf DisposeControlOffLink), New Object() {ControlID})
            Exit Sub
        End If

        DisconnectControl()

        sysInfo.logger.LogThis("控制器离线", sysInfo.LastErrorInfo, Wangk.Tools.Loglevel.Level_DEBUG)

        MsgBox($"{sysInfo.LastErrorInfo},{sysInfo.Language.GetLang("请重新连接控制器或重启控制器")}",
               MsgBoxStyle.Information,
               sysInfo.Language.GetLang("控制器连接异常"))
    End Sub
#End Region

#Region "播放窗体操作"
    ''' <summary>
    ''' 创建窗体
    ''' </summary>
    Public Sub CreatWindowThread(ByVal WindowId As Integer)

    End Sub

    ''' <summary>
    ''' 添加播放窗口
    ''' </summary>
    Public Sub AddNewWindow(ByVal WindowId As Integer)

    End Sub

    ''' <summary>
    ''' 更新播放窗体尺寸
    ''' </summary>
    Public Sub UpdateWindow(ByVal WindowId As Integer)

    End Sub

    ''' <summary>
    ''' 删除窗口
    ''' </summary>
    Public Sub DeleteWindow(ByVal WindowId As Integer)

    End Sub

    ''' <summary>
    ''' 关闭所有窗体
    ''' </summary>
    Public Sub ClearWindow()
        For Each i001 As WindowInfo In sysInfo.WindowList
            i001.PlayDialog.Close(True)
        Next
    End Sub

    ''' <summary>
    ''' 加载窗口
    ''' </summary>
    Public Sub InitWinsow()


        '显示新窗体
    End Sub

#Region "更新所有窗体信息"
    ''' <summary>
    ''' 更新所有窗体信息
    ''' </summary>
    Public Sub UpdateAllWinsow()
#Region "重建历史点击状态"
        '重建历史点击状态
        For i001 As Integer = 0 To sysInfo.ScreenList.Count - 1
            With sysInfo.ScreenList(i001)
                .WindowId = -1

                ReDim .ClickHistoryMap((.DefSize.Height \ .DefScanBoardSize.Height) * .SensorLayout.Height,
                                        (.DefSize.Width \ .DefScanBoardSize.Width) * .SensorLayout.Width)
            End With
        Next
#End Region

#Region "标记要连接的控制器"
        For i002 As Integer = 0 To sysInfo.SenderList.Count - 1
            With sysInfo.SenderList(i002)
                .LinkFlage = False
            End With
        Next

        For i003 As Integer = 0 To sysInfo.WindowList.Count - 1
            With sysInfo.WindowList(i003)
                '遍历窗口内屏幕
                For j003 As Integer = 0 To .ScreenList.Count - 1
                    With .ScreenList(j003)
                        '屏幕不存在则跳过
                        If .ScreenID > sysInfo.ScreenList.Count - 1 Then
                            Continue For
                        End If

                        sysInfo.ScreenList(.ScreenID).WindowId = i003

                        '遍历屏幕所在控制器
                        For Each k003 As Integer In sysInfo.ScreenList(.ScreenID).SenderList
                            '控制器不存在则跳过
                            If k003 > sysInfo.SenderList.Count - 1 Then
                                Continue For
                            End If

                            sysInfo.SenderList(k003).LinkFlage = True
                        Next
                    End With
                Next
            End With
        Next
#End Region
    End Sub
#End Region
#End Region
End Class