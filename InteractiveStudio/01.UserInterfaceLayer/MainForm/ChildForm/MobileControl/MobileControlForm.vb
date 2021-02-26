Imports System.Net.NetworkInformation
Imports System.Net.Sockets
Imports QRCoder
Imports Wangk.Resource

Public Class MobileControlForm
    Private Sub MobileControlForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If HttpServerHelper.IsRunning Then
            OpenServerButton.Checked = True
        Else
            CloseServerButton.Checked = True
        End If

        ChangeControlsLanguage()
    End Sub

    Private Sub OpenServerButton_CheckedChanged(sender As Object, e As EventArgs) Handles OpenServerButton.CheckedChanged
        If Not OpenServerButton.Checked Then
            Exit Sub
        End If

        Dim qrGenerator As QRCodeGenerator = New QRCodeGenerator

        Dim IPItems = Net.Dns.GetHostAddresses(Net.Dns.GetHostName())
        For Each item In IPItems
            If item.AddressFamily = AddressFamily.InterNetwork Then
                Dim urlStr = $"http://{item}:8080"

                TextBox1.AppendText(urlStr & vbCrLf)

                Dim qrCodeData As QRCodeData = qrGenerator.CreateQrCode(urlStr, QRCodeGenerator.ECCLevel.M)
                Dim qrCode As QRCode = New QRCode(qrCodeData)
                Dim tmpPictureBox = New PictureBox With {
                    .Width = 160,
                    .Height = 160,
                    .SizeMode = ImageLayout.Zoom,
                    .Image = qrCode.GetGraphic(2)
                }
                FlowLayoutPanel1.Controls.Add(tmpPictureBox)
            End If
        Next

        HttpServerHelper.StartServer()
    End Sub

    Private Sub CloseServerButton_CheckedChanged(sender As Object, e As EventArgs) Handles CloseServerButton.CheckedChanged
        If Not CloseServerButton.Checked Then
            Exit Sub
        End If

        TextBox1.Clear()
        FlowLayoutPanel1.Controls.Clear()

        HttpServerHelper.StopServer()
    End Sub

#Region "切换控件语言"
    ''' <summary>
    ''' 切换控件语言
    ''' </summary>
    Public Sub ChangeControlsLanguage()
        With MultiLanguageHelper.Lang
            Me.Label2.Text = .GetS("● Enter any address in your phone's browser")
            Me.GroupBox1.Text = .GetS("Web server")
            Me.OpenServerButton.Text = .GetS("Open")
            Me.CloseServerButton.Text = .GetS("Close")
            Me.Label3.Text = .GetS("● Or scan QR code")
            Me.Text = .GetS("MobileControlForm")
        End With
    End Sub
#End Region
End Class