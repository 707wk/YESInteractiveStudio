﻿Imports DevComponents.DotNetBar
Imports Wangk.Resource

Public NotInheritable Class UIFormHelper
    Private Sub MainFormHelper()
    End Sub

    Public Shared UIForm As MainForm

    ''' <summary>
    ''' 删除状态的单元格样式
    ''' </summary>
    Public Shared DeleteDataGridViewCellStyle As New DataGridViewCellStyle With {
        .Font = New Font(MainForm.Font.Name, MainForm.Font.Size, FontStyle.Italic Or FontStyle.Strikeout),
        .ForeColor = Color.Gray
    }

    ''' <summary>
    ''' 常规背景色
    ''' </summary>
    Public Shared NormalColor As Color = Color.FromArgb(18, 150, 219)
    ''' <summary>
    ''' 操作成功背景色
    ''' </summary>
    Public Shared SuccessColor As Color = Color.FromArgb(127, 187, 66)
    ''' <summary>
    ''' 警告背景色
    ''' </summary>
    Public Shared WarningColor As Color = Color.FromArgb(253, 184, 19)
    ''' <summary>
    ''' 错误背景色
    ''' </summary>
    Public Shared ErrorColor As Color = Color.FromArgb(240, 81, 37) '216, 99, 68)

#Region "创建操作列"
    ''' <summary>
    ''' 创建操作列
    ''' </summary>
    Public Shared Function GetDataGridViewLinkColumn(
                                                    headerText As String,
                                                    foreColor As Color,
                                                    Optional visible As Boolean = True) As CheckBoxDataGridViewButtonColumn

        Dim tmpColumn = New CheckBoxDataGridViewButtonColumn
        With tmpColumn
            .Visible = visible
            .HeaderText = If(String.IsNullOrWhiteSpace(headerText), " ", headerText)
            .ForeColor = foreColor
        End With

        Return tmpColumn

    End Function
#End Region

    ''' <summary>
    ''' 信息显示时间(ms)
    ''' </summary>
    Public Const ToastInterval = 1500

#Region "正常信息"
    ''' <summary>
    ''' 正常信息
    ''' </summary>
    Public Shared Sub ToastInfo(message As String,
                                Optional parent As Control = Nothing,
                                Optional timeoutInterval As Integer = ToastInterval)

        ToastNotification.ToastBackColor = NormalColor
        ToastNotification.Show(If(parent, UIForm),
                               message,
                               My.Resources.toastInfo_32px,
                               timeoutInterval,
                               eToastGlowColor.Blue,
                               eToastPosition.MiddleCenter)
    End Sub
#End Region

#Region "操作成功信息"
    ''' <summary>
    ''' 操作成功信息
    ''' </summary>
    Public Shared Sub ToastSuccess(message As String,
                                   Optional parent As Control = Nothing,
                                   Optional timeoutInterval As Integer = ToastInterval)

        ToastNotification.ToastBackColor = SuccessColor
        ToastNotification.Show(If(parent, UIForm),
                               message,
                               My.Resources.toastSuccess_32px,
                               timeoutInterval,
                               eToastGlowColor.Green,
                               eToastPosition.MiddleCenter)
    End Sub
#End Region

#Region "警告消息"
    ''' <summary>
    ''' 警告消息
    ''' </summary>
    Public Shared Sub ToastWarning(message As String,
                                   Optional parent As Control = Nothing,
                                   Optional timeoutInterval As Integer = ToastInterval)

        ToastNotification.ToastBackColor = WarningColor
        ToastNotification.Show(If(parent, UIForm),
                               message,
                               My.Resources.toastInfo_32px,
                               timeoutInterval,
                               eToastGlowColor.Orange,
                               eToastPosition.MiddleCenter)
    End Sub
#End Region

#Region "错误消息"
    ''' <summary>
    ''' 错误消息
    ''' </summary>
    Public Shared Sub ToastError(message As String,
                                 Optional parent As Control = Nothing,
                                 Optional timeoutInterval As Integer = ToastInterval)

        ToastNotification.ToastBackColor = ErrorColor
        ToastNotification.Show(If(parent, UIForm),
                               message,
                               My.Resources.toastFail_32px,
                               timeoutInterval,
                               eToastGlowColor.Red,
                               eToastPosition.MiddleCenter)
    End Sub
#End Region

End Class
