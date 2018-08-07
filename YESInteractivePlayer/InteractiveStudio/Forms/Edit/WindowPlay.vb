Public Class WindowPlay
    Dim WindowId As Integer

#Region "更新序号"
    ''' <summary>
    ''' 更新序号
    ''' </summary>
    Public Sub UpdateID()
        Dim ID As Integer = 1
        For Each i001 As ListViewItem In ListView1.Items
            i001.SubItems(0).Text = ID
            ID += 1
        Next
    End Sub
#End Region

#Region "加载窗体播放信息"
    ''' <summary>
    ''' 加载窗体播放信息
    ''' </summary>
    ''' <param name="WinId"></param>
    Public Sub LoadWindow(ByVal WinId As Integer)
        WindowId = WinId

        Try
            With sysInfo.Schedule.WindowList(WindowId)
                Label1.Text = .PlayMediaId + 1
                Label4.Text = .PlayMediaTime

                ListView1.Items.Clear()
                For Each i001 As MediaInfo In .PlayProgramInfo.MediaList
                    ListView1.Items.Add("")
                    ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(i001.Path)
                    ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(i001.PlayTime)
                Next
                UpdateID()
            End With
        Catch ex As Exception
        End Try

    End Sub
#End Region

    Private Sub WindowPlay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
#Region "样式设置"
        With ListView1
            .View = View.Details
            .GridLines = True
            .LabelEdit = False
            .FullRowSelect = True
            .HideSelection = True
            .ShowItemToolTips = True
            .MultiSelect = False

            .Columns.Add(sysInfo.Language.GetS("ID"), 40)
            .Columns.Add(sysInfo.Language.GetS("File"), 400)
            .Columns.Add(sysInfo.Language.GetS("Play Time(s)"), 100)
        End With

        Timer1.Interval = 1000

        'sysInfo.Language.GetS(Me)
        ChangeControlsLanguage()
#End Region
    End Sub

#Region "定时刷新界面"
    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Try
            With sysInfo.Schedule.WindowList(WindowId)
                Label1.Text = .PlayMediaId + 1
                Label4.Text = .PlayMediaTime
            End With
        Catch ex As Exception
        End Try
    End Sub

    Private Sub WindowPlay_VisibleChanged(sender As Object, e As EventArgs) Handles Me.VisibleChanged
        If Me.Visible Then
            Timer1.Start()
        Else
            Timer1.Stop()
        End If
    End Sub
#End Region

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With sysInfo.Language
            Me.Label2.Text = .GetS("Play File ID")
            Me.GroupBox1.Text = .GetS("Media List")
            Me.Label3.Text = .GetS("Played Time")
        End With
    End Sub
#End Region
End Class