<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SerialPortSelectForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.ConnectButton = New System.Windows.Forms.Button()
        Me.RefreshButton = New System.Windows.Forms.Button()
        Me.ComboBox1 = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(73, 67)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(207, 27)
        Me.ConnectButton.TabIndex = 7
        Me.ConnectButton.Text = "Connect to Control"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'RefreshButton
        '
        Me.RefreshButton.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.RefreshButton.FlatAppearance.BorderSize = 0
        Me.RefreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.RefreshButton.Image = Global.InteractiveStudio.My.Resources.Resources.refresh_24px
        Me.RefreshButton.Location = New System.Drawing.Point(199, 5)
        Me.RefreshButton.Name = "RefreshButton"
        Me.RefreshButton.Size = New System.Drawing.Size(34, 34)
        Me.RefreshButton.TabIndex = 6
        Me.RefreshButton.UseVisualStyleBackColor = True
        '
        'ComboBox1
        '
        Me.ComboBox1.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox1.FormattingEnabled = True
        Me.ComboBox1.Location = New System.Drawing.Point(101, 12)
        Me.ComboBox1.Name = "ComboBox1"
        Me.ComboBox1.Size = New System.Drawing.Size(92, 20)
        Me.ComboBox1.TabIndex = 5
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(24, 16)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 12)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Serial Port"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333!))
        Me.TableLayoutPanel1.Controls.Add(Me.ComboBox1, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.RefreshButton, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(33, 12)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(296, 44)
        Me.TableLayoutPanel1.TabIndex = 8
        '
        'Label2
        '
        Me.Label2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label2.Location = New System.Drawing.Point(0, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(375, 106)
        Me.Label2.TabIndex = 9
        Me.Label2.Text = "Waiting..."
        Me.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'SerialPortSelectForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(375, 106)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.ConnectButton)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "SerialPortSelectForm"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "SerialPortSelectForm"
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents ConnectButton As Button
    Friend WithEvents RefreshButton As Button
    Friend WithEvents ComboBox1 As ComboBox
    Friend WithEvents Label1 As Label
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Label2 As Label
End Class
