Imports Wangk.Resource

Public Class ScanBoardSelectForm
    Public Value As String

    Private Sub ScanBoardSelectForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '接收卡状态
        With TreeView1
            .CheckBoxes = False
            .ImageList = ImageList1
            .ShowLines = True
            .ShowRootLines = True
        End With

        ChangeControlsLanguage()

    End Sub

    Private Sub ScanBoardSelectForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        TreeView1.ExpandAll()
    End Sub

    Private Sub TreeView1_NodeMouseClick(sender As Object, e As TreeNodeMouseClickEventArgs) Handles TreeView1.NodeMouseClick
        If e.Node.Level = 0 Then Exit Sub

        Value = e.Node.Tag
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Text = "ScanBoardSelectForm"
        End With
    End Sub
#End Region

End Class