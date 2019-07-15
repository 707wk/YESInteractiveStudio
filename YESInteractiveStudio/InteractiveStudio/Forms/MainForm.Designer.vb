<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StyleManager1 = New DevComponents.DotNetBar.StyleManager(Me.components)
        Me.RibbonControl1 = New DevComponents.DotNetBar.RibbonControl()
        Me.RibbonPanel2 = New DevComponents.DotNetBar.RibbonPanel()
        Me.RibbonBar5 = New DevComponents.DotNetBar.RibbonBar()
        Me.ItemContainer4 = New DevComponents.DotNetBar.ItemContainer()
        Me.LabelItem1 = New DevComponents.DotNetBar.LabelItem()
        Me.ComboBoxItem1 = New DevComponents.DotNetBar.ComboBoxItem()
        Me.RibbonBar4 = New DevComponents.DotNetBar.RibbonBar()
        Me.RibbonPanel1 = New DevComponents.DotNetBar.RibbonPanel()
        Me.RibbonBar3 = New DevComponents.DotNetBar.RibbonBar()
        Me.ItemContainer1 = New DevComponents.DotNetBar.ItemContainer()
        Me.HideWindowsCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.RibbonBar2 = New DevComponents.DotNetBar.RibbonBar()
        Me.RibbonBar1 = New DevComponents.DotNetBar.RibbonBar()
        Me.DeviceTab = New DevComponents.DotNetBar.RibbonTabItem()
        Me.SettingsTab = New DevComponents.DotNetBar.RibbonTabItem()
        Me.ButtonItem1 = New DevComponents.DotNetBar.ButtonItem()
        Me.ItemContainer2 = New DevComponents.DotNetBar.ItemContainer()
        Me.ItemContainer3 = New DevComponents.DotNetBar.ItemContainer()
        Me.LabelItem2 = New DevComponents.DotNetBar.LabelItem()
        Me.AutoRunCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.ReadScreenInformationButton = New DevComponents.DotNetBar.ButtonItem()
        Me.ControlIPButton = New DevComponents.DotNetBar.ButtonItem()
        Me.ScreenLayoutButton = New DevComponents.DotNetBar.ButtonItem()
        Me.MCUButton = New DevComponents.DotNetBar.ButtonItem()
        Me.InteractButton = New DevComponents.DotNetBar.ButtonItem()
        Me.TestButton = New DevComponents.DotNetBar.ButtonItem()
        Me.BlackButton = New DevComponents.DotNetBar.ButtonItem()
        Me.DebugButton = New DevComponents.DotNetBar.ButtonItem()
        Me.ConnectState = New DevComponents.DotNetBar.ButtonItem()
        Me.RibbonControl1.SuspendLayout()
        Me.RibbonPanel2.SuspendLayout()
        Me.RibbonPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StyleManager1
        '
        Me.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light
        Me.StyleManager1.MetroColorParameters = New DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.FromArgb(CType(CType(239, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(242, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer)))
        '
        'RibbonControl1
        '
        Me.RibbonControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(239, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(242, Byte), Integer))
        '
        '
        '
        Me.RibbonControl1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonControl1.Controls.Add(Me.RibbonPanel2)
        Me.RibbonControl1.Controls.Add(Me.RibbonPanel1)
        Me.RibbonControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.RibbonControl1.ForeColor = System.Drawing.Color.Black
        Me.RibbonControl1.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.DeviceTab, Me.SettingsTab})
        Me.RibbonControl1.KeyTipsFont = New System.Drawing.Font("Tahoma", 7.0!)
        Me.RibbonControl1.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl1.Name = "RibbonControl1"
        Me.RibbonControl1.QuickToolbarItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ButtonItem1})
        Me.RibbonControl1.Size = New System.Drawing.Size(800, 102)
        Me.RibbonControl1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonControl1.SystemText.MaximizeRibbonText = "&Maximize the Ribbon"
        Me.RibbonControl1.SystemText.MinimizeRibbonText = "Mi&nimize the Ribbon"
        Me.RibbonControl1.SystemText.QatAddItemText = "&Add to Quick Access Toolbar"
        Me.RibbonControl1.SystemText.QatCustomizeMenuLabel = "<b>Customize Quick Access Toolbar</b>"
        Me.RibbonControl1.SystemText.QatCustomizeText = "&Customize Quick Access Toolbar..."
        Me.RibbonControl1.SystemText.QatDialogAddButton = "&Add >>"
        Me.RibbonControl1.SystemText.QatDialogCancelButton = "Cancel"
        Me.RibbonControl1.SystemText.QatDialogCaption = "Customize Quick Access Toolbar"
        Me.RibbonControl1.SystemText.QatDialogCategoriesLabel = "&Choose commands from:"
        Me.RibbonControl1.SystemText.QatDialogOkButton = "OK"
        Me.RibbonControl1.SystemText.QatDialogPlacementCheckbox = "&Place Quick Access Toolbar below the Ribbon"
        Me.RibbonControl1.SystemText.QatDialogRemoveButton = "&Remove"
        Me.RibbonControl1.SystemText.QatPlaceAboveRibbonText = "&Place Quick Access Toolbar above the Ribbon"
        Me.RibbonControl1.SystemText.QatPlaceBelowRibbonText = "&Place Quick Access Toolbar below the Ribbon"
        Me.RibbonControl1.SystemText.QatRemoveItemText = "&Remove from Quick Access Toolbar"
        Me.RibbonControl1.TabGroupHeight = 14
        Me.RibbonControl1.TabIndex = 1
        Me.RibbonControl1.Text = "RibbonControl1"
        '
        'RibbonPanel2
        '
        Me.RibbonPanel2.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonPanel2.Controls.Add(Me.RibbonBar5)
        Me.RibbonPanel2.Controls.Add(Me.RibbonBar4)
        Me.RibbonPanel2.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RibbonPanel2.Location = New System.Drawing.Point(0, 25)
        Me.RibbonPanel2.Name = "RibbonPanel2"
        Me.RibbonPanel2.Padding = New System.Windows.Forms.Padding(3, 0, 3, 2)
        Me.RibbonPanel2.Size = New System.Drawing.Size(800, 77)
        '
        '
        '
        Me.RibbonPanel2.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonPanel2.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonPanel2.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonPanel2.TabIndex = 2
        '
        'RibbonBar5
        '
        Me.RibbonBar5.AutoOverflowEnabled = True
        '
        '
        '
        Me.RibbonBar5.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar5.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar5.ContainerControlProcessDialogKey = True
        Me.RibbonBar5.Dock = System.Windows.Forms.DockStyle.Left
        Me.RibbonBar5.DragDropSupport = True
        Me.RibbonBar5.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer4})
        Me.RibbonBar5.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar5.Location = New System.Drawing.Point(403, 0)
        Me.RibbonBar5.Name = "RibbonBar5"
        Me.RibbonBar5.Size = New System.Drawing.Size(385, 75)
        Me.RibbonBar5.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar5.TabIndex = 1
        Me.RibbonBar5.Text = "RibbonBar5"
        '
        '
        '
        Me.RibbonBar5.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar5.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar5.TitleVisible = False
        '
        'ItemContainer4
        '
        '
        '
        '
        Me.ItemContainer4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ItemContainer4.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical
        Me.ItemContainer4.Name = "ItemContainer4"
        Me.ItemContainer4.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer2, Me.ItemContainer3})
        '
        '
        '
        Me.ItemContainer4.TitleMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.ItemContainer4.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'LabelItem1
        '
        Me.LabelItem1.Name = "LabelItem1"
        Me.LabelItem1.Text = "Language"
        '
        'ComboBoxItem1
        '
        Me.ComboBoxItem1.ComboWidth = 80
        Me.ComboBoxItem1.DropDownHeight = 106
        Me.ComboBoxItem1.ItemHeight = 16
        Me.ComboBoxItem1.Name = "ComboBoxItem1"
        '
        'RibbonBar4
        '
        Me.RibbonBar4.AutoOverflowEnabled = True
        '
        '
        '
        Me.RibbonBar4.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar4.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar4.ContainerControlProcessDialogKey = True
        Me.RibbonBar4.Dock = System.Windows.Forms.DockStyle.Left
        Me.RibbonBar4.DragDropSupport = True
        Me.RibbonBar4.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ReadScreenInformationButton, Me.ControlIPButton, Me.ScreenLayoutButton, Me.MCUButton})
        Me.RibbonBar4.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar4.Location = New System.Drawing.Point(3, 0)
        Me.RibbonBar4.Name = "RibbonBar4"
        Me.RibbonBar4.Size = New System.Drawing.Size(400, 75)
        Me.RibbonBar4.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar4.TabIndex = 0
        Me.RibbonBar4.Text = "RibbonBar4"
        '
        '
        '
        Me.RibbonBar4.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar4.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar4.TitleVisible = False
        '
        'RibbonPanel1
        '
        Me.RibbonPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonPanel1.Controls.Add(Me.RibbonBar3)
        Me.RibbonPanel1.Controls.Add(Me.RibbonBar2)
        Me.RibbonPanel1.Controls.Add(Me.RibbonBar1)
        Me.RibbonPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RibbonPanel1.Location = New System.Drawing.Point(0, 0)
        Me.RibbonPanel1.Name = "RibbonPanel1"
        Me.RibbonPanel1.Padding = New System.Windows.Forms.Padding(3, 0, 3, 2)
        Me.RibbonPanel1.Size = New System.Drawing.Size(800, 102)
        '
        '
        '
        Me.RibbonPanel1.Style.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonPanel1.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonPanel1.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonPanel1.TabIndex = 1
        Me.RibbonPanel1.Visible = False
        '
        'RibbonBar3
        '
        Me.RibbonBar3.AutoOverflowEnabled = True
        '
        '
        '
        Me.RibbonBar3.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar3.ContainerControlProcessDialogKey = True
        Me.RibbonBar3.Dock = System.Windows.Forms.DockStyle.Left
        Me.RibbonBar3.DragDropSupport = True
        Me.RibbonBar3.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ItemContainer1})
        Me.RibbonBar3.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar3.Location = New System.Drawing.Point(440, 0)
        Me.RibbonBar3.Name = "RibbonBar3"
        Me.RibbonBar3.Size = New System.Drawing.Size(168, 100)
        Me.RibbonBar3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar3.TabIndex = 2
        Me.RibbonBar3.Text = "RibbonBar3"
        '
        '
        '
        Me.RibbonBar3.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar3.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar3.TitleVisible = False
        '
        'ItemContainer1
        '
        '
        '
        '
        Me.ItemContainer1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ItemContainer1.LayoutOrientation = DevComponents.DotNetBar.eOrientation.Vertical
        Me.ItemContainer1.Name = "ItemContainer1"
        Me.ItemContainer1.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.HideWindowsCheckBox})
        '
        '
        '
        Me.ItemContainer1.TitleMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.ItemContainer1.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'HideWindowsCheckBox
        '
        Me.HideWindowsCheckBox.Name = "HideWindowsCheckBox"
        Me.HideWindowsCheckBox.Text = "Hide windows"
        '
        'RibbonBar2
        '
        Me.RibbonBar2.AutoOverflowEnabled = True
        Me.RibbonBar2.BackColor = System.Drawing.SystemColors.Control
        '
        '
        '
        Me.RibbonBar2.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar2.ContainerControlProcessDialogKey = True
        Me.RibbonBar2.Dock = System.Windows.Forms.DockStyle.Left
        Me.RibbonBar2.DragDropSupport = True
        Me.RibbonBar2.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.InteractButton, Me.TestButton, Me.BlackButton, Me.DebugButton})
        Me.RibbonBar2.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar2.Location = New System.Drawing.Point(150, 0)
        Me.RibbonBar2.Name = "RibbonBar2"
        Me.RibbonBar2.Size = New System.Drawing.Size(290, 100)
        Me.RibbonBar2.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar2.TabIndex = 1
        Me.RibbonBar2.Text = "RibbonBar2"
        '
        '
        '
        Me.RibbonBar2.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar2.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar2.TitleVisible = False
        '
        'RibbonBar1
        '
        Me.RibbonBar1.AutoOverflowEnabled = True
        '
        '
        '
        Me.RibbonBar1.BackgroundMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar1.ContainerControlProcessDialogKey = True
        Me.RibbonBar1.Dock = System.Windows.Forms.DockStyle.Left
        Me.RibbonBar1.DragDropSupport = True
        Me.RibbonBar1.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ConnectState})
        Me.RibbonBar1.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar1.Location = New System.Drawing.Point(3, 0)
        Me.RibbonBar1.Name = "RibbonBar1"
        Me.RibbonBar1.Size = New System.Drawing.Size(147, 100)
        Me.RibbonBar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar1.TabIndex = 0
        Me.RibbonBar1.Text = "RibbonBar1"
        '
        '
        '
        Me.RibbonBar1.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.RibbonBar1.TitleStyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonBar1.TitleVisible = False
        '
        'DeviceTab
        '
        Me.DeviceTab.Name = "DeviceTab"
        Me.DeviceTab.Panel = Me.RibbonPanel1
        Me.DeviceTab.Text = "Device"
        '
        'SettingsTab
        '
        Me.SettingsTab.Checked = True
        Me.SettingsTab.Name = "SettingsTab"
        Me.SettingsTab.Panel = Me.RibbonPanel2
        Me.SettingsTab.Text = "Settings"
        '
        'ButtonItem1
        '
        Me.ButtonItem1.Name = "ButtonItem1"
        Me.ButtonItem1.Text = "ButtonItem1"
        '
        'ItemContainer2
        '
        '
        '
        '
        Me.ItemContainer2.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ItemContainer2.Name = "ItemContainer2"
        Me.ItemContainer2.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.LabelItem1, Me.ComboBoxItem1})
        '
        '
        '
        Me.ItemContainer2.TitleMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.ItemContainer2.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'ItemContainer3
        '
        '
        '
        '
        Me.ItemContainer3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.ItemContainer3.Name = "ItemContainer3"
        Me.ItemContainer3.SubItems.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.LabelItem2, Me.AutoRunCheckBox})
        '
        '
        '
        Me.ItemContainer3.TitleMouseOverStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        '
        '
        Me.ItemContainer3.TitleStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        '
        'LabelItem2
        '
        Me.LabelItem2.Name = "LabelItem2"
        Me.LabelItem2.Text = "Auto run"
        '
        'AutoRunCheckBox
        '
        Me.AutoRunCheckBox.Name = "AutoRunCheckBox"
        Me.AutoRunCheckBox.Text = "AutoRun"
        Me.AutoRunCheckBox.TextVisible = False
        '
        'ReadScreenInformationButton
        '
        Me.ReadScreenInformationButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ReadScreenInformationButton.Image = Global.InteractiveStudio.My.Resources.Resources.download_32px
        Me.ReadScreenInformationButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ReadScreenInformationButton.Name = "ReadScreenInformationButton"
        Me.ReadScreenInformationButton.SubItemsExpandWidth = 14
        Me.ReadScreenInformationButton.Text = "Read screen information"
        '
        'ControlIPButton
        '
        Me.ControlIPButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ControlIPButton.Image = Global.InteractiveStudio.My.Resources.Resources.control_32px
        Me.ControlIPButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ControlIPButton.Name = "ControlIPButton"
        Me.ControlIPButton.SubItemsExpandWidth = 14
        Me.ControlIPButton.Text = "Control IP"
        '
        'ScreenLayoutButton
        '
        Me.ScreenLayoutButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ScreenLayoutButton.Image = Global.InteractiveStudio.My.Resources.Resources.screenLayout_32px
        Me.ScreenLayoutButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ScreenLayoutButton.Name = "ScreenLayoutButton"
        Me.ScreenLayoutButton.SubItemsExpandWidth = 14
        Me.ScreenLayoutButton.Text = "Screen layout"
        '
        'MCUButton
        '
        Me.MCUButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.MCUButton.Image = Global.InteractiveStudio.My.Resources.Resources.MCU_32px
        Me.MCUButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.MCUButton.Name = "MCUButton"
        Me.MCUButton.SubItemsExpandWidth = 14
        Me.MCUButton.Text = "MCU information"
        '
        'InteractButton
        '
        Me.InteractButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.InteractButton.Image = Global.InteractiveStudio.My.Resources.Resources.DisplayMode0_32px
        Me.InteractButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.InteractButton.Name = "InteractButton"
        Me.InteractButton.SubItemsExpandWidth = 14
        Me.InteractButton.Text = "Interact"
        '
        'TestButton
        '
        Me.TestButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.TestButton.Image = Global.InteractiveStudio.My.Resources.Resources.DisplayMode1_32px
        Me.TestButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.TestButton.Name = "TestButton"
        Me.TestButton.SubItemsExpandWidth = 14
        Me.TestButton.Text = "Test"
        '
        'BlackButton
        '
        Me.BlackButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.BlackButton.Image = Global.InteractiveStudio.My.Resources.Resources.DisplayMode2_32px
        Me.BlackButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.BlackButton.Name = "BlackButton"
        Me.BlackButton.SubItemsExpandWidth = 14
        Me.BlackButton.Text = "Black"
        '
        'DebugButton
        '
        Me.DebugButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.DebugButton.Image = Global.InteractiveStudio.My.Resources.Resources.DisplayMode3_32px
        Me.DebugButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.DebugButton.Name = "DebugButton"
        Me.DebugButton.SubItemsExpandWidth = 14
        Me.DebugButton.Text = "Debug"
        '
        'ConnectState
        '
        Me.ConnectState.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ConnectState.Image = Global.InteractiveStudio.My.Resources.Resources.usb_disconnect_32px
        Me.ConnectState.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ConnectState.Name = "ConnectState"
        Me.ConnectState.SubItemsExpandWidth = 14
        Me.ConnectState.Text = "Disconnect"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.RibbonControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MainForm"
        Me.RibbonControl1.ResumeLayout(False)
        Me.RibbonControl1.PerformLayout()
        Me.RibbonPanel2.ResumeLayout(False)
        Me.RibbonPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents StyleManager1 As DevComponents.DotNetBar.StyleManager
    Friend WithEvents RibbonControl1 As DevComponents.DotNetBar.RibbonControl
    Friend WithEvents RibbonPanel1 As DevComponents.DotNetBar.RibbonPanel
    Friend WithEvents RibbonBar3 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents ItemContainer1 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents HideWindowsCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents RibbonBar2 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents InteractButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents TestButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents BlackButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents DebugButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents RibbonBar1 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents ConnectState As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents RibbonPanel2 As DevComponents.DotNetBar.RibbonPanel
    Friend WithEvents RibbonBar5 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents LabelItem1 As DevComponents.DotNetBar.LabelItem
    Friend WithEvents ComboBoxItem1 As DevComponents.DotNetBar.ComboBoxItem
    Friend WithEvents RibbonBar4 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents ReadScreenInformationButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ControlIPButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ScreenLayoutButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents DeviceTab As DevComponents.DotNetBar.RibbonTabItem
    Friend WithEvents SettingsTab As DevComponents.DotNetBar.RibbonTabItem
    Friend WithEvents ButtonItem1 As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents MCUButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ItemContainer4 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer2 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer3 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents LabelItem2 As DevComponents.DotNetBar.LabelItem
    Friend WithEvents AutoRunCheckBox As DevComponents.DotNetBar.CheckBoxItem
End Class
