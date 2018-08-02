﻿Imports System.ComponentModel
Imports System.Net.Sockets
Imports System.Threading
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
    '''' <summary>
    '''' 窗口编辑
    '''' </summary>
    'Dim WindowEditDialog As WindowEdit
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
        ClearWindow()

        ProgramEditDialog.Hide()

        LoadFile(sysInfo.HistoryFile)

        TreeView1.Nodes.Clear()

        For i001 As Integer = 0 To sysInfo.Schedule.WindowList.Count - 1
            Dim Tmpnode001 As New TreeNode With {
                        .Text = sysInfo.Schedule.WindowList.Item(i001).Remark,
                        .ImageIndex = 0,
                        .SelectedImageIndex = 0,
                        .ContextMenuStrip = WindowMenuStrip
                    }
            For Each j001 As ProgramInfo In sysInfo.Schedule.WindowList.Item(i001).ProgramList
                Dim Tmpnode002 As New TreeNode With {
                    .Text = j001.Remark,
                    .ImageIndex = 1,
                    .SelectedImageIndex = 1,
                    .ContextMenuStrip = ProgramMenuStrip
                }

                Tmpnode001.Nodes.Add(Tmpnode002)
            Next

            TreeView1.Nodes.Add(Tmpnode001)

            UpdateWindow(i001)

#Disable Warning BC40000 ' 类型或成员已过时
            Dim tmpThread As New Threading.Thread(AddressOf CreatWindowThread) With {
            .ApartmentState = ApartmentState.STA,
            .IsBackground = True
        }
            tmpThread.Start(i001)
#Enable Warning BC40000 ' 类型或成员已过时
        Next

        TreeView1.ExpandAll()
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

        ''窗口编辑窗体
        'WindowEditDialog = New WindowEdit With {
        '    .FormBorderStyle = FormBorderStyle.None,
        '    .Dock = DockStyle.Fill,
        '    .TopLevel = False,
        '    .Parent = Panel1
        '    }
        'Panel1.Controls.Add(WindowEditDialog)

        '节目编辑窗体
        ProgramEditDialog = New ProgramEdit With {
            .FormBorderStyle = FormBorderStyle.None,
            .Dock = DockStyle.Fill,
            .TopLevel = False,
            .Parent = Panel1
            }
        Panel1.Controls.Add(ProgramEditDialog)

#Region "触摸参数"
        '抗干扰等级
        With ComboBoxItem9
            For i001 As Integer = 1 To 9
                .Items.Add(i001)
            Next
            .SelectedIndex = sysInfo.ClickValidNums
        End With

        '触摸灵敏度
        With ComboBoxItem10
            For i001 As Integer = 1 To 9
                .Items.Add(i001)
            Next
            .SelectedIndex = sysInfo.TouchSensitivity
        End With

        '触摸模式
        With ComboBoxItem11
            .Items.Add(sysInfo.Language.GetS("High Resolution"))
            .Items.Add(sysInfo.Language.GetS("Medium Resolution"))
            .Items.Add(sysInfo.Language.GetS("Low Resolution"))

            .SelectedIndex = sysInfo.TouchMode
        End With
#End Region

#Region "复位时间"
        '复位温度
        With ComboBoxItem7
            .Items.Add(0)
            For i001 As Integer = 5 To 255
                .Items.Add(i001)
            Next

            .Text = sysInfo.ResetTemp
        End With

        '复位时间
        With ComboBoxItem8
            .Items.Add(0)
            For i001 As Integer = 25 To 255
                .Items.Add(i001)
            Next

            .Text = sysInfo.ResetSec
        End With
#End Region

#Region "语言选项"
        '语言选项
        With ComboBoxItem1
            .Items.Add("中文")
            .Items.Add("English")

            .SelectedIndex = sysInfo.SelectLang
        End With
#End Region

        SetLinkControlState(False)

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
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
            sysInfo.Schedule.WindowList = New List(Of WindowInfo)
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

        For Each i001 As WindowInfo In sysInfo.Schedule.WindowList
            i001.PlayDialog.SwitchDisplayMode(Modal)
        Next
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
        For Each i001 As WindowInfo In sysInfo.Schedule.WindowList
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
        If sysInfo.Schedule.WindowList.Count = 0 Then
            Exit Sub
        End If

        If Not ConnectControl() Then
            Exit Sub
        End If

        SetLinkControlState(sysInfo.LinkFlage)
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
    ''' <summary>
    ''' 节目定时切换
    ''' </summary>
    Public Sub ChangePlayMedia()
        For i001 As Integer = 0 To sysInfo.Schedule.WindowList.Count - 1
            Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList(i001)

            With TmpWindowInfo
                If .PlayMediaId = -1 Then
                    Continue For
                End If

                .PlayMediaTime += 1

                If .PlayMediaTime >= .PlayProgramInfo.MediaList(.PlayMediaId).PlayTime Then
                    .PlayMediaTime = 0
                    .PlayMediaId = (.PlayMediaId + 1) Mod .PlayProgramInfo.MediaList.Count

                    .PlayDialog.Play(.PlayProgramInfo.MediaList(.PlayMediaId).Path)
                End If
            End With

            sysInfo.Schedule.WindowList(i001) = TmpWindowInfo
        Next
    End Sub
#End Region

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        CorrectionInquireTime()

        ChangePlayMedia()
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

        MsgBox($"{sysInfo.LastErrorInfo},{sysInfo.Language.GetS("请重新连接控制器或重启控制器")}",
               MsgBoxStyle.Information,
               sysInfo.Language.GetS("控制器连接异常"))
    End Sub
#End Region

#Region "播放窗体操作"
#Region "创建窗体"
    ''' <summary>
    ''' 创建窗体
    ''' </summary>
    Public Sub CreatWindowThread(ByVal WindowId As Integer)
        Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList.Item(WindowId)
        TmpWindowInfo.PlayDialog = New PlayWindow With {
            .WindowId = WindowId
        }
        sysInfo.Schedule.WindowList.Item(WindowId) = TmpWindowInfo

        TmpWindowInfo.PlayDialog.ShowDialog()
    End Sub
#End Region

#Region "添加窗体节点"
    Public Sub CreatWindowNode(ByVal WindowId As Integer)
        Dim Tmpnode001 As New TreeNode With {
                        .Text = $"窗口{WindowId}",
                        .ImageIndex = 0,
                        .SelectedImageIndex = 0,
                        .ContextMenuStrip = WindowMenuStrip
                    }
        TreeView1.Nodes.Add(Tmpnode001)
    End Sub
#End Region

#Region "添加播放窗口"
    ''' <summary>
    ''' 添加播放窗口
    ''' </summary>
    Public Sub AddNewWindow()
        CreatWindowNode(sysInfo.Schedule.WindowList.Count + 1)

        Dim TmpWindowInfo As New WindowInfo With {
            .Remark = $"窗口{sysInfo.Schedule.WindowList.Count + 1}",
            .ShowFlage = True,
            .Size = New Size(1, 1),
            .ScreenList = New List(Of Integer),
            .ZoomPix = New Size(1, 1),
            .ProgramList = New List(Of ProgramInfo),
            .PlayMediaId = -1
        }
        sysInfo.Schedule.WindowList.Add(TmpWindowInfo)

#Disable Warning BC40000 ' 类型或成员已过时
        Dim tmpThread As New Threading.Thread(AddressOf CreatWindowThread) With {
            .ApartmentState = ApartmentState.STA,
            .IsBackground = True
        }
        tmpThread.Start(sysInfo.Schedule.WindowList.Count - 1)
#Enable Warning BC40000 ' 类型或成员已过时
    End Sub
#End Region

#Region "更新屏幕的窗口ID"
    ''' <summary>
    ''' 更新屏幕的窗口ID
    ''' </summary>
    Public Sub UpdateWindowIdInScreen()
        For i002 As Integer = 0 To sysInfo.ScreenList.Count - 1
            With sysInfo.ScreenList(i002)
                .WindowId = -1
            End With
        Next

        For i001 As Integer = 0 To sysInfo.Schedule.WindowList.Count - 1
            sysInfo.Schedule.WindowList.Item(i001).PlayDialog.WindowId = i001

            For Each j001 As Integer In sysInfo.Schedule.WindowList.Item(i001).ScreenList
                sysInfo.ScreenList(j001).WindowId = i001
            Next
        Next
    End Sub
#End Region

#Region "更新播放窗体尺寸"
    ''' <summary>
    ''' 更新播放窗体尺寸
    ''' </summary>
    Public Sub UpdateWindow(ByVal WindowId As Integer)
        Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList.Item(WindowId)
        With TmpWindowInfo
            .Size = New Size(0, 0)
            Dim zoomProportion As Double = .ZoomPix.Width / .ZoomPix.Height

            For Each i001 As Integer In .ScreenList
                '屏幕不存在则跳过
                If i001 > sysInfo.ScreenList.Count - 1 Then
                    Continue For
                End If

                Dim TmpPoint As Point = sysInfo.Schedule.ScreenLocations(i001)

                '查找最大宽度
                If .Size.Width < TmpPoint.X + sysInfo.ScreenList(i001).DefSize.Width Then
                    .Size.Width = TmpPoint.X + sysInfo.ScreenList(i001).DefSize.Width
                End If
                '查找最大高度
                If .Size.Height < TmpPoint.Y + sysInfo.ScreenList(i001).DefSize.Height Then
                    .Size.Height = TmpPoint.Y + sysInfo.ScreenList(i001).DefSize.Height
                End If

                '更新屏幕参数
                With sysInfo.ScreenList(i001)
                    '位置
                    .ZoomLocation.X = TmpPoint.X / zoomProportion
                    .ZoomLocation.Y = TmpPoint.Y / zoomProportion
                    '尺寸
                    .ZoomSize.Width = .DefSize.Width / zoomProportion
                    .ZoomSize.Height = .DefSize.Height / zoomProportion
                    '接收卡大小
                    .ZoomScanBoardSize.Width = .DefScanBoardSize.Width / zoomProportion
                    .ZoomScanBoardSize.Height = .DefScanBoardSize.Height / zoomProportion
                    '传感器大小
                    .ZoomSensorSize.Width = .ZoomScanBoardSize.Width / .SensorLayout.Width / zoomProportion
                    .ZoomSensorSize.Height = .ZoomScanBoardSize.Height / .SensorLayout.Height / zoomProportion
                End With
            Next

            '缩放窗口尺寸
            .Size.Width = .Size.Width / zoomProportion
            .Size.Height = .Size.Height / zoomProportion
        End With
        sysInfo.Schedule.WindowList.Item(WindowId) = TmpWindowInfo
    End Sub
#End Region

#Region "删除窗体节点"
    Public Sub DeleteWindowNode(ByVal WindowId As Integer)
        TreeView1.Nodes.RemoveAt(WindowId)
    End Sub
#End Region

#Region "删除窗口"
    ''' <summary>
    ''' 删除窗口
    ''' </summary>
    Public Sub DeleteWindow(ByVal WindowId As Integer)
        DeleteWindowNode(WindowId)

        With sysInfo.Schedule.WindowList
            .Item(WindowId).PlayDialog.Close(True)
            .RemoveAt(WindowId)
        End With
    End Sub
#End Region

#Region "关闭所有窗体"
    ''' <summary>
    ''' 关闭所有窗体
    ''' </summary>
    Public Sub ClearWindow()
        For Each i001 As WindowInfo In sysInfo.Schedule.WindowList
            i001.PlayDialog.Close(True)
        Next
    End Sub
#End Region
#End Region

#Region "节目操作"
#Region "新建节目"
    ''' <summary>
    ''' 新建节目
    ''' </summary>
    Public Sub AddNewProgram(ByVal WindowId As Integer)
        Dim Tmpnode002 As New TreeNode With {
            .Text = $"节目{sysInfo.Schedule.WindowList(WindowId).ProgramList.Count + 1}",
            .ImageIndex = 1,
            .SelectedImageIndex = 1,
            .ContextMenuStrip = ProgramMenuStrip
        }
        TreeView1.Nodes(WindowId).Nodes.Add(Tmpnode002)

        sysInfo.Schedule.WindowList(WindowId).ProgramList.Add(
            New ProgramInfo With {
            .Remark = Tmpnode002.Text,
            .MediaList = New List(Of MediaInfo)
            })
    End Sub
#End Region

#Region "更新节目"
    ''' <summary>
    ''' 更新节目
    ''' </summary>
    Public Sub UpdateProgram(ByVal WindowId As Integer, ByVal ProgramId As Integer)
        TreeView1.Nodes(WindowId).Nodes(ProgramId).Text = sysInfo.Schedule.WindowList(WindowId).ProgramList(ProgramId).Remark
    End Sub
#End Region

#Region "删除节目"
    ''' <summary>
    ''' 删除节目
    ''' </summary>
    ''' <param name="WindowId"></param>
    ''' <param name="ProgramId"></param>
    Public Sub DeleteProgram(ByVal WindowId As Integer, ByVal ProgramId As Integer)
        TreeView1.Nodes(WindowId).Nodes.RemoveAt(ProgramId)

        Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList(WindowId)
        With TmpWindowInfo
            If .ProgramList(ProgramId).Remark = .PlayProgramInfo.Remark Then
                .PlayProgramInfo = Nothing
                .PlayMediaId = -1
            End If

            .ProgramList.RemoveAt(ProgramId)
        End With
        sysInfo.Schedule.WindowList(WindowId) = TmpWindowInfo
    End Sub
#End Region

#Region "播放节目"
    ''' <summary>
    ''' 播放节目
    ''' </summary>
    Public Sub PlayProgram(ByVal WindowId As Integer, ByVal ProgramId As Integer)
        Dim TmpWindowInfo As WindowInfo = sysInfo.Schedule.WindowList(WindowId)

        With TmpWindowInfo
            If .ProgramList(ProgramId).MediaList.Count = 0 Then
                Exit Sub
            End If
            For Each i001 As MediaInfo In .ProgramList(ProgramId).MediaList
                If Not System.IO.File.Exists(i001.Path) Then
                    MsgBox($"{i001.Path} {sysInfo.Language.GetS("not found")}",
                           MsgBoxStyle.Information,
                           sysInfo.Language.GetS("播放节目"))

                    Exit Sub
                End If
            Next

            .PlayProgramInfo = .ProgramList(ProgramId)
            .PlayMediaId = 0
            .PlayMediaTime = 0
        End With

        sysInfo.Schedule.WindowList(WindowId) = TmpWindowInfo
    End Sub
#End Region
#End Region

#Region "功能区"
#Region "文件"
#Region "新建"
    ''' <summary>
    ''' 新建
    ''' </summary>
    Private Sub ButtonItem20_Click(sender As Object, e As EventArgs) Handles ButtonItem20.Click
        '新建前保存旧文件
        Select Case MsgBox("是否保存修改?", MsgBoxStyle.YesNoCancel, "保存")
            Case MsgBoxResult.Yes '保存
                If sysInfo.HistoryFile = "" Then
                    Dim tmp1 As New SaveFileDialog
                    tmp1.Filter = "播放方案文件|*.xml"
                    If tmp1.ShowDialog() <> DialogResult.OK Then
                        Exit Sub
                    End If

                    sysInfo.HistoryFile = tmp1.FileName
                End If

                SaveFile(sysInfo.HistoryFile)
            Case MsgBoxResult.No '不保存

            Case MsgBoxResult.Cancel '取消
                Exit Sub
        End Select

        '新建空文件
        sysInfo.HistoryFile = ""
        LoadFile(sysInfo.HistoryFile)

        ShowToolBarInfo()

        ''todo:重新刷新数据
        LoadSchedule()
    End Sub
#End Region

#Region "打开"
    ''' <summary>
    ''' 打开
    ''' </summary>
    Private Sub ButtonItem21_Click(sender As Object, e As EventArgs) Handles ButtonItem21.Click
        '新建前保存旧文件
        Select Case MsgBox("是否保存修改?", MsgBoxStyle.YesNoCancel, "保存")
            Case MsgBoxResult.Yes '保存
                If sysInfo.HistoryFile = "" Then
                    Dim tmp1 As New SaveFileDialog With {
                        .Filter = "播放方案文件|*.xml"
                    }
                    If tmp1.ShowDialog() <> DialogResult.OK Then
                        Exit Sub
                    End If

                    sysInfo.HistoryFile = tmp1.FileName
                End If

                SaveFile(sysInfo.HistoryFile)
            Case MsgBoxResult.No '不保存

            Case MsgBoxResult.Cancel '取消
                Exit Sub
        End Select

        '选择打开文件的路径
        Dim tmp As New OpenFileDialog With {
            .Filter = "播放方案文件|*.xml"
        }
        If tmp.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        '文件打开异常则打开旧文件
        If Not LoadFile(tmp.FileName) Then
            LoadFile(sysInfo.HistoryFile)
        Else
            sysInfo.HistoryFile = tmp.FileName
        End If

        ShowToolBarInfo()

        ''todo:重新刷新数据
        LoadSchedule()
    End Sub
#End Region

#Region "保存"
    ''' <summary>
    ''' 保存
    ''' </summary>
    Private Sub ButtonItem22_Click(sender As Object, e As EventArgs) Handles ButtonItem22.Click
        '新文件则选择保存路径
        If sysInfo.HistoryFile = "" Then
            Dim tmp1 As New SaveFileDialog
            'tmp.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            tmp1.Filter = "播放方案文件|*.xml"
            If tmp1.ShowDialog() <> DialogResult.OK Then
                Exit Sub
            End If

            sysInfo.HistoryFile = tmp1.FileName
        End If

        SaveFile(sysInfo.HistoryFile)

        ShowToolBarInfo()
    End Sub
#End Region

#Region "另存为"
    ''' <summary>
    ''' 另存为
    ''' </summary>
    Private Sub ButtonItem23_Click(sender As Object, e As EventArgs) Handles ButtonItem23.Click
        Dim tmp As New SaveFileDialog With {
            .Filter = "播放方案文件|*.xml"
        }
        If tmp.ShowDialog() <> DialogResult.OK Then
            Exit Sub
        End If

        sysInfo.HistoryFile = tmp.FileName

        SaveFile(sysInfo.HistoryFile)

        ShowToolBarInfo()
    End Sub
#End Region
#End Region

#Region "设备操作"
    ''' <summary>
    ''' 连接
    ''' </summary>
    Private Sub ButtonItem18_Click(sender As Object, e As EventArgs) Handles ButtonItem18.Click
        SetLinkControl()
    End Sub

    ''' <summary>
    ''' 断开
    ''' </summary>
    Private Sub ButtonItem19_Click(sender As Object, e As EventArgs) Handles ButtonItem19.Click
        SetOffLinkControl()
    End Sub

#Region "切换显示模式"
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
#End Region

#Region "互动参数"
    ''' <summary>
    ''' 抗干扰等级
    ''' </summary>
    Private Sub ComboBoxItem9_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem9.SelectedIndexChanged
        sysInfo.ClickValidNums = Val(ComboBoxItem9.Text)
    End Sub

    ''' <summary>
    ''' 触摸灵敏度
    ''' </summary>
    Private Sub ComboBoxItem10_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem10.SelectedIndexChanged
        sysInfo.TouchSensitivity = ComboBoxItem10.SelectedIndex
    End Sub

    ''' <summary>
    ''' 触摸模式
    ''' </summary>
    Private Sub ComboBoxItem11_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem11.SelectedIndexChanged
        sysInfo.TouchMode = ComboBoxItem11.SelectedIndex
    End Sub

    ''' <summary>
    ''' 复位温度
    ''' </summary>
    Private Sub ComboBoxItem7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem7.SelectedIndexChanged
        sysInfo.ResetTemp = Val(ComboBoxItem7.Text)
    End Sub

    ''' <summary>
    ''' 复位时间
    ''' </summary>
    Private Sub ComboBoxItem8_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem8.SelectedIndexChanged
        sysInfo.ResetSec = Val(ComboBoxItem8.Text)
    End Sub
#End Region

#Region "设置"
    ''' <summary>
    ''' 显示语言
    ''' </summary>
    Private Sub ComboBoxItem1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxItem1.SelectedIndexChanged
        sysInfo.SelectLang = ComboBoxItem1.SelectedIndex
    End Sub

    ''' <summary>
    ''' 屏幕走线
    ''' </summary>
    Private Sub ButtonItem26_Click(sender As Object, e As EventArgs) Handles ButtonItem26.Click
        Dim TmpDialog As New ScreenLigature
        TmpDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 控制器
    ''' </summary>
    Private Sub ButtonItem27_Click(sender As Object, e As EventArgs) Handles ButtonItem27.Click
        Dim TmpDialog As New ControlNetwork
        TmpDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 接收卡
    ''' </summary>
    Private Sub ButtonItem28_Click(sender As Object, e As EventArgs) Handles ButtonItem28.Click
        Dim TmpDialog As New ScanBoardOption
        TmpDialog.ShowDialog()
    End Sub

    ''' <summary>
    ''' 高级用户
    ''' </summary>
    Private Sub ButtonItem1_Click(sender As Object, e As EventArgs) Handles ButtonItem1.Click
        Dim TmpDialog As New Wangk.Resource.InputBox With {
            .Title = "高级用户登陆",
            .InputTips = "输入密码",
            .PasswordChar = "*"
        }
        If TmpDialog.ShowDialog <> DialogResult.OK Then
            Exit Sub
        End If

        If System.IO.Path.GetExtension(TmpDialog.InputStr).ToLower() = "yestech" Then
            ButtonItem17.Visible = True
            ButtonItem1.Enabled = False
        End If
    End Sub
#End Region

#Region "帮助"
    ''' <summary>
    ''' 用户手册
    ''' </summary>
    Private Sub ButtonItem24_Click(sender As Object, e As EventArgs) Handles ButtonItem24.Click
        System.Diagnostics.Process.Start($"{My.Application.Info.Title} User Manual {[Enum].
                                         GetName(GetType(Wangk.Resource.MultiLanguage.LANG),
                                                 sysInfo.SelectLang)}.chm")
        'todo:编写用户手册
    End Sub

    ''' <summary>
    ''' 关于
    ''' </summary>
    Private Sub ButtonItem25_Click(sender As Object, e As EventArgs) Handles ButtonItem25.Click
        Dim TmpDialog As New AboutBox
        TmpDialog.ShowDialog()
    End Sub
#End Region
#End Region

#Region "播放方案"
#Region "添加窗口"
    ''' <summary>
    ''' 添加窗口
    ''' </summary>
    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        AddNewWindow()
    End Sub
#End Region

#Region "编辑窗口"
    ''' <summary>
    ''' 编辑窗口
    ''' </summary>
    Private Sub EditWindowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditWindowToolStripMenuItem.Click
        Dim TmpDialog As New WindowEdit With {
            .WindowId = TreeView1.SelectedNode.Index
        }
        TmpDialog.ShowDialog()

        UpdateWindowIdInScreen()

        UpdateWindow(TmpDialog.WindowId)
    End Sub
#End Region

#Region "删除窗口"
    ''' <summary>
    ''' 删除窗口
    ''' </summary>
    Private Sub DeleteWindowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteWindowToolStripMenuItem.Click
        DeleteWindow(TreeView1.SelectedNode.Index)

        UpdateWindowIdInScreen()
    End Sub
#End Region

#Region "节目"
#Region "添加节目"
    ''' <summary>
    ''' 添加节目
    ''' </summary>
    Private Sub ToolStripButton2_Click(sender As Object, e As EventArgs) Handles ToolStripButton2.Click
        If TreeView1.SelectedNode Is Nothing Then
            Exit Sub
        End If

        Dim WindowId As Integer
        Select Case TreeView1.SelectedNode.Level
            Case 0
                '窗口
                WindowId = TreeView1.SelectedNode.Index
            Case 1
                '节目
                WindowId = TreeView1.SelectedNode.Parent.Index
        End Select

        AddNewProgram(WindowId)
    End Sub
#End Region

#Region "编辑节目"
    ''' <summary>
    ''' 编辑节目
    ''' </summary>
    Private Sub TreeView1_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TreeView1.NodeMouseClick
        Select Case e.Node.Level
            Case 0
                '窗口
                'GroupBox2.Text = "Window"
                'WindowEditDialog.Show()
                ProgramEditDialog.Hide()
            Case 1
                '节目
                'GroupBox2.Text = "Program"
                'WindowEditDialog.Hide()
                ProgramEditDialog.LoadProgram(e.Node.Parent.Index, e.Node.Index)
                ProgramEditDialog.Show()
        End Select
    End Sub
#End Region

#Region "删除节目"
    ''' <summary>
    ''' 删除节目
    ''' </summary>
    Private Sub DeleteProgramToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteProgramToolStripMenuItem.Click
        With TreeView1.SelectedNode
            DeleteProgram(.Parent.Index, .Index)
        End With

        ProgramEditDialog.Hide()
    End Sub
#End Region

#Region "播放节目"
    ''' <summary>
    ''' 播放节目
    ''' </summary>
    Private Sub PlayToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PlayToolStripMenuItem.Click
        With TreeView1.SelectedNode
            PlayProgram(.Parent.Index, .Index)
        End With
    End Sub

    Private Sub ComboBoxItem9_Click(sender As Object, e As EventArgs) Handles ComboBoxItem9.Click

    End Sub
#End Region
#End Region
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.LabelItem5.Text = .GetS("Temp Change Over")
            Me.LabelItem6.Text = .GetS("Reset Time Interval")
            Me.LabelItem3.Text = .GetS("Anti-interference")
            Me.LabelItem4.Text = .GetS("Touch Sensitivity")
            Me.LabelItem7.Text = .GetS("Language")
            Me.RibbonBar7.Text = .GetS("Screen Configuration")
            Me.ButtonItem26.Text = .GetS("Ligature")
            Me.ButtonItem27.Text = .GetS("Controls")
            Me.ButtonItem28.Text = .GetS("ScanBoard")
            Me.ButtonItem1.Text = .GetS("PowerUser")
            Me.RibbonBar6.Text = .GetS("General")
            Me.LabelItem1.Text = .GetS("Language")
            Me.RibbonBar3.Text = .GetS("Display Mode")
            Me.ButtonItem14.Text = .GetS("Interact")
            Me.ButtonItem15.Text = .GetS("Test")
            Me.ButtonItem16.Text = .GetS("Black")
            Me.ButtonItem17.Text = .GetS("Debug")
            Me.RibbonBar2.Text = .GetS("Control")
            Me.ButtonItem18.Text = .GetS("Connect")
            Me.ButtonItem19.Text = .GetS("Disconnect")
            Me.ButtonItem20.Text = .GetS("New")
            Me.ButtonItem21.Text = .GetS("Open")
            Me.ButtonItem22.Text = .GetS("Save")
            Me.ButtonItem23.Text = .GetS("Save As")
            Me.RibbonBar5.Text = .GetS("Reset")
            Me.LabelItem2.Text = .GetS("Temp Change Over")
            Me.LabelItem9.Text = .GetS("Reset Time Interval")
            Me.RibbonBar4.Text = .GetS("Reaction")
            Me.LabelItem10.Text = .GetS("Anti-interference")
            Me.LabelItem11.Text = .GetS("Touch Sensitivity")
            Me.LabelItem8.Text = .GetS("TouchMode")
            Me.ButtonItem24.Text = .GetS("User Manual")
            Me.ButtonItem25.Text = .GetS("About")
            Me.RibbonTabItem1.Text = .GetS("Schedule")
            Me.RibbonTabItem2.Text = .GetS("Device")
            Me.RibbonTabItem3.Text = .GetS("Interactivity")
            Me.RibbonTabItem4.Text = .GetS("Settings")
            Me.RibbonTabItem5.Text = .GetS("Help")
            Me.GroupBox1.Text = .GetS("Schedule")
            Me.ToolStrip1.Text = .GetS("ToolStrip1")
            Me.ToolStripButton1.Text = .GetS("Add Window")
            Me.ToolStripButton2.Text = .GetS("Add Program")
            Me.GroupBox2.Text = .GetS("Program")
            Me.EditWindowToolStripMenuItem.Text = .GetS("Edit Window")
            Me.DeleteWindowToolStripMenuItem.Text = .GetS("Delete Window")
            Me.PlayToolStripMenuItem.Text = .GetS("Play")
            Me.DeleteProgramToolStripMenuItem.Text = .GetS("Delete Program")
        End With
    End Sub
#End Region
End Class