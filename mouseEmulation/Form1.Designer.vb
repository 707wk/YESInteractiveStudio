<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ListView2 = New System.Windows.Forms.ListView()
        Me.ListView3 = New System.Windows.Forms.ListView()
        Me.ListView4 = New System.Windows.Forms.ListView()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripLabel1 = New System.Windows.Forms.ToolStripLabel()
        Me.ToolStripTextBox1 = New System.Windows.Forms.ToolStripTextBox()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripSplitButton1 = New System.Windows.Forms.ToolStripSplitButton()
        Me.模拟点击ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.测试ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.黑屏ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.忽略ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripButton2 = New System.Windows.Forms.ToolStripButton()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ListView5 = New System.Windows.Forms.ListView()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.StatusStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'Timer1
        '
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 345)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1164, 22)
        Me.StatusStrip1.TabIndex = 1
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabel1
        '
        Me.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1"
        Me.ToolStripStatusLabel1.Size = New System.Drawing.Size(134, 17)
        Me.ToolStripStatusLabel1.Text = "ToolStripStatusLabel1"
        '
        'ListView1
        '
        Me.ListView1.Location = New System.Drawing.Point(719, 28)
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(150, 135)
        Me.ListView1.TabIndex = 2
        Me.ListView1.UseCompatibleStateImageBehavior = False
        '
        'ListView2
        '
        Me.ListView2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListView2.Location = New System.Drawing.Point(12, 28)
        Me.ListView2.Name = "ListView2"
        Me.ListView2.Size = New System.Drawing.Size(256, 314)
        Me.ListView2.TabIndex = 3
        Me.ListView2.UseCompatibleStateImageBehavior = False
        '
        'ListView3
        '
        Me.ListView3.Location = New System.Drawing.Point(875, 28)
        Me.ListView3.Name = "ListView3"
        Me.ListView3.Size = New System.Drawing.Size(142, 135)
        Me.ListView3.TabIndex = 4
        Me.ListView3.UseCompatibleStateImageBehavior = False
        '
        'ListView4
        '
        Me.ListView4.Location = New System.Drawing.Point(1023, 28)
        Me.ListView4.Name = "ListView4"
        Me.ListView4.Size = New System.Drawing.Size(129, 135)
        Me.ListView4.TabIndex = 5
        Me.ListView4.UseCompatibleStateImageBehavior = False
        '
        'ToolStrip1
        '
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripButton1, Me.ToolStripSeparator1, Me.ToolStripLabel1, Me.ToolStripTextBox1, Me.ToolStripSeparator2, Me.ToolStripSplitButton1, Me.ToolStripButton2})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1164, 25)
        Me.ToolStrip1.TabIndex = 6
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.Image = Global.mouseEmulation.My.Resources.Resources.connect
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(52, 22)
        Me.ToolStripButton1.Text = "连接"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripLabel1
        '
        Me.ToolStripLabel1.Name = "ToolStripLabel1"
        Me.ToolStripLabel1.Size = New System.Drawing.Size(59, 22)
        Me.ToolStripLabel1.Text = "查询间隔:"
        '
        'ToolStripTextBox1
        '
        Me.ToolStripTextBox1.Name = "ToolStripTextBox1"
        Me.ToolStripTextBox1.Size = New System.Drawing.Size(50, 25)
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
        '
        'ToolStripSplitButton1
        '
        Me.ToolStripSplitButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.模拟点击ToolStripMenuItem, Me.测试ToolStripMenuItem, Me.黑屏ToolStripMenuItem, Me.忽略ToolStripMenuItem})
        Me.ToolStripSplitButton1.Image = Global.mouseEmulation.My.Resources.Resources.click
        Me.ToolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripSplitButton1.Name = "ToolStripSplitButton1"
        Me.ToolStripSplitButton1.Size = New System.Drawing.Size(85, 22)
        Me.ToolStripSplitButton1.Text = "点击(&F1)"
        '
        '模拟点击ToolStripMenuItem
        '
        Me.模拟点击ToolStripMenuItem.Image = Global.mouseEmulation.My.Resources.Resources.click
        Me.模拟点击ToolStripMenuItem.Name = "模拟点击ToolStripMenuItem"
        Me.模拟点击ToolStripMenuItem.Size = New System.Drawing.Size(121, 22)
        Me.模拟点击ToolStripMenuItem.Text = "点击(&F1)"
        '
        '测试ToolStripMenuItem
        '
        Me.测试ToolStripMenuItem.Image = Global.mouseEmulation.My.Resources.Resources.test
        Me.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem"
        Me.测试ToolStripMenuItem.Size = New System.Drawing.Size(121, 22)
        Me.测试ToolStripMenuItem.Text = "测试(&F2)"
        '
        '黑屏ToolStripMenuItem
        '
        Me.黑屏ToolStripMenuItem.Image = Global.mouseEmulation.My.Resources.Resources.blankscreen
        Me.黑屏ToolStripMenuItem.Name = "黑屏ToolStripMenuItem"
        Me.黑屏ToolStripMenuItem.Size = New System.Drawing.Size(121, 22)
        Me.黑屏ToolStripMenuItem.Text = "黑屏(&F3)"
        '
        '忽略ToolStripMenuItem
        '
        Me.忽略ToolStripMenuItem.Image = Global.mouseEmulation.My.Resources.Resources.disenable
        Me.忽略ToolStripMenuItem.Name = "忽略ToolStripMenuItem"
        Me.忽略ToolStripMenuItem.Size = New System.Drawing.Size(121, 22)
        Me.忽略ToolStripMenuItem.Text = "忽略(&F4)"
        '
        'ToolStripButton2
        '
        Me.ToolStripButton2.Image = Global.mouseEmulation.My.Resources.Resources.openFolder
        Me.ToolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton2.Name = "ToolStripButton2"
        Me.ToolStripButton2.Size = New System.Drawing.Size(88, 22)
        Me.ToolStripButton2.Text = "打开文件夹"
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(517, 169)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(635, 173)
        Me.TextBox1.TabIndex = 7
        '
        'ListView5
        '
        Me.ListView5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ListView5.Location = New System.Drawing.Point(274, 28)
        Me.ListView5.Name = "ListView5"
        Me.ListView5.Size = New System.Drawing.Size(166, 314)
        Me.ListView5.TabIndex = 8
        Me.ListView5.UseCompatibleStateImageBehavior = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1164, 367)
        Me.Controls.Add(Me.ListView5)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.ListView4)
        Me.Controls.Add(Me.ListView3)
        Me.Controls.Add(Me.ListView2)
        Me.Controls.Add(Me.ListView1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "Form1"
        Me.Text = "ME触摸地砖屏控制系统"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Timer1 As Timer
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ToolStripStatusLabel1 As ToolStripStatusLabel
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ListView2 As ListView
    Friend WithEvents ListView3 As ListView
    Friend WithEvents ListView4 As ListView
    Friend WithEvents ToolStrip1 As ToolStrip
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripLabel1 As ToolStripLabel
    Friend WithEvents ToolStripTextBox1 As ToolStripTextBox
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripSplitButton1 As ToolStripSplitButton
    Friend WithEvents 模拟点击ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 测试ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents 忽略ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents ToolStripButton2 As ToolStripButton
    Friend WithEvents ListView5 As ListView
    Friend WithEvents FolderBrowserDialog1 As FolderBrowserDialog
    Friend WithEvents 黑屏ToolStripMenuItem As ToolStripMenuItem
End Class
