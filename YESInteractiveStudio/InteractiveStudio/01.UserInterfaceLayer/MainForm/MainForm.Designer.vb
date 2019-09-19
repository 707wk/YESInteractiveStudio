<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StyleManager1 = New DevComponents.DotNetBar.StyleManager(Me.components)
        Me.RibbonControl1 = New DevComponents.DotNetBar.RibbonControl()
        Me.RibbonPanel1 = New DevComponents.DotNetBar.RibbonPanel()
        Me.RibbonBar3 = New DevComponents.DotNetBar.RibbonBar()
        Me.ItemContainer1 = New DevComponents.DotNetBar.ItemContainer()
        Me.HideWindowsCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.RibbonBar1 = New DevComponents.DotNetBar.RibbonBar()
        Me.InteractCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.TestCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.BlackCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.DebugCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.RibbonPanel2 = New DevComponents.DotNetBar.RibbonPanel()
        Me.RibbonBar5 = New DevComponents.DotNetBar.RibbonBar()
        Me.ItemContainer4 = New DevComponents.DotNetBar.ItemContainer()
        Me.ItemContainer2 = New DevComponents.DotNetBar.ItemContainer()
        Me.LabelItem1 = New DevComponents.DotNetBar.LabelItem()
        Me.ComboBoxItem1 = New DevComponents.DotNetBar.ComboBoxItem()
        Me.ItemContainer3 = New DevComponents.DotNetBar.ItemContainer()
        Me.LabelItem2 = New DevComponents.DotNetBar.LabelItem()
        Me.AutoRunCheckBox = New DevComponents.DotNetBar.CheckBoxItem()
        Me.RibbonBar4 = New DevComponents.DotNetBar.RibbonBar()
        Me.ReadScreenInformationButton = New DevComponents.DotNetBar.ButtonItem()
        Me.PlayWindowSettingsButton = New DevComponents.DotNetBar.ButtonItem()
        Me.ControlButton = New DevComponents.DotNetBar.ButtonItem()
        Me.SensorButton = New DevComponents.DotNetBar.ButtonItem()
        Me.AccuracyButton = New DevComponents.DotNetBar.ButtonItem()
        Me.MCUButton = New DevComponents.DotNetBar.ButtonItem()
        Me.StartTab = New DevComponents.DotNetBar.RibbonTabItem()
        Me.SettingsTab = New DevComponents.DotNetBar.RibbonTabItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.WindowProgramControl1 = New InteractiveStudio.WindowProgramControl()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.RibbonControl1.SuspendLayout()
        Me.RibbonPanel1.SuspendLayout()
        Me.RibbonPanel2.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'StyleManager1
        '
        Me.StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.VisualStudio2012Light
        Me.StyleManager1.MetroColorParameters = New DevComponents.DotNetBar.Metro.ColorTables.MetroColorGeneratorParameters(System.Drawing.Color.FromArgb(CType(CType(239, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(242, Byte), Integer)), System.Drawing.Color.FromArgb(CType(CType(0, Byte), Integer), CType(CType(122, Byte), Integer), CType(CType(204, Byte), Integer)))
        '
        'RibbonControl1
        '
        Me.RibbonControl1.AutoExpand = False
        Me.RibbonControl1.BackColor = System.Drawing.Color.FromArgb(CType(CType(239, Byte), Integer), CType(CType(239, Byte), Integer), CType(CType(242, Byte), Integer))
        '
        '
        '
        Me.RibbonControl1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square
        Me.RibbonControl1.Controls.Add(Me.RibbonPanel1)
        Me.RibbonControl1.Controls.Add(Me.RibbonPanel2)
        Me.RibbonControl1.Dock = System.Windows.Forms.DockStyle.Top
        Me.RibbonControl1.ForeColor = System.Drawing.Color.Black
        Me.RibbonControl1.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.StartTab, Me.SettingsTab})
        Me.RibbonControl1.KeyTipsFont = New System.Drawing.Font("Tahoma", 7.0!)
        Me.RibbonControl1.Location = New System.Drawing.Point(0, 0)
        Me.RibbonControl1.Name = "RibbonControl1"
        Me.RibbonControl1.Size = New System.Drawing.Size(754, 102)
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
        'RibbonPanel1
        '
        Me.RibbonPanel1.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonPanel1.Controls.Add(Me.RibbonBar3)
        Me.RibbonPanel1.Controls.Add(Me.RibbonBar1)
        Me.RibbonPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.RibbonPanel1.Location = New System.Drawing.Point(0, 25)
        Me.RibbonPanel1.Name = "RibbonPanel1"
        Me.RibbonPanel1.Padding = New System.Windows.Forms.Padding(3, 0, 3, 2)
        Me.RibbonPanel1.Size = New System.Drawing.Size(754, 77)
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
        Me.RibbonBar3.Location = New System.Drawing.Point(324, 0)
        Me.RibbonBar3.Name = "RibbonBar3"
        Me.RibbonBar3.Size = New System.Drawing.Size(149, 75)
        Me.RibbonBar3.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar3.TabIndex = 4
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
        Me.RibbonBar1.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.InteractCheckBox, Me.TestCheckBox, Me.BlackCheckBox, Me.DebugCheckBox})
        Me.RibbonBar1.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar1.Location = New System.Drawing.Point(3, 0)
        Me.RibbonBar1.Name = "RibbonBar1"
        Me.RibbonBar1.Size = New System.Drawing.Size(321, 75)
        Me.RibbonBar1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled
        Me.RibbonBar1.TabIndex = 3
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
        'InteractCheckBox
        '
        Me.InteractCheckBox.CheckBoxImageChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode0_32px
        Me.InteractCheckBox.CheckBoxImageUnChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode0G_32px
        Me.InteractCheckBox.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Top
        Me.InteractCheckBox.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton
        Me.InteractCheckBox.Name = "InteractCheckBox"
        Me.InteractCheckBox.Text = "Interact"
        '
        'TestCheckBox
        '
        Me.TestCheckBox.CheckBoxImageChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode1_32px
        Me.TestCheckBox.CheckBoxImageUnChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode1G_32px
        Me.TestCheckBox.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Top
        Me.TestCheckBox.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton
        Me.TestCheckBox.Name = "TestCheckBox"
        Me.TestCheckBox.Text = "Test"
        '
        'BlackCheckBox
        '
        Me.BlackCheckBox.CheckBoxImageChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode2_32px
        Me.BlackCheckBox.CheckBoxImageUnChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode2G_32px
        Me.BlackCheckBox.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Top
        Me.BlackCheckBox.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton
        Me.BlackCheckBox.Name = "BlackCheckBox"
        Me.BlackCheckBox.Text = "Black"
        '
        'DebugCheckBox
        '
        Me.DebugCheckBox.CheckBoxImageChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode3_32px
        Me.DebugCheckBox.CheckBoxImageUnChecked = Global.InteractiveStudio.My.Resources.Resources.DisplayMode3G_32px
        Me.DebugCheckBox.CheckBoxPosition = DevComponents.DotNetBar.eCheckBoxPosition.Top
        Me.DebugCheckBox.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton
        Me.DebugCheckBox.Name = "DebugCheckBox"
        Me.DebugCheckBox.Text = "Debug"
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
        Me.RibbonPanel2.Size = New System.Drawing.Size(754, 77)
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
        Me.RibbonPanel2.Visible = False
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
        Me.RibbonBar5.Location = New System.Drawing.Point(476, 0)
        Me.RibbonBar5.Name = "RibbonBar5"
        Me.RibbonBar5.Size = New System.Drawing.Size(195, 75)
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
        Me.RibbonBar4.Items.AddRange(New DevComponents.DotNetBar.BaseItem() {Me.ReadScreenInformationButton, Me.PlayWindowSettingsButton, Me.ControlButton, Me.SensorButton, Me.AccuracyButton, Me.MCUButton})
        Me.RibbonBar4.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F"
        Me.RibbonBar4.Location = New System.Drawing.Point(3, 0)
        Me.RibbonBar4.Name = "RibbonBar4"
        Me.RibbonBar4.Size = New System.Drawing.Size(473, 75)
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
        'ReadScreenInformationButton
        '
        Me.ReadScreenInformationButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ReadScreenInformationButton.Image = Global.InteractiveStudio.My.Resources.Resources.download_32px
        Me.ReadScreenInformationButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ReadScreenInformationButton.Name = "ReadScreenInformationButton"
        Me.ReadScreenInformationButton.SubItemsExpandWidth = 14
        Me.ReadScreenInformationButton.Text = "Read screen information"
        '
        'PlayWindowSettingsButton
        '
        Me.PlayWindowSettingsButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.PlayWindowSettingsButton.Image = Global.InteractiveStudio.My.Resources.Resources.window_32px
        Me.PlayWindowSettingsButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.PlayWindowSettingsButton.Name = "PlayWindowSettingsButton"
        Me.PlayWindowSettingsButton.SubItemsExpandWidth = 14
        Me.PlayWindowSettingsButton.Text = "Play window settings"
        '
        'ControlButton
        '
        Me.ControlButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.ControlButton.Image = Global.InteractiveStudio.My.Resources.Resources.control_32px
        Me.ControlButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.ControlButton.Name = "ControlButton"
        Me.ControlButton.SubItemsExpandWidth = 14
        Me.ControlButton.Text = "Control settings"
        '
        'SensorButton
        '
        Me.SensorButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.SensorButton.Image = Global.InteractiveStudio.My.Resources.Resources.sensor_24px
        Me.SensorButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.SensorButton.Name = "SensorButton"
        Me.SensorButton.SubItemsExpandWidth = 14
        Me.SensorButton.Text = "Sensor settings"
        '
        'AccuracyButton
        '
        Me.AccuracyButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.AccuracyButton.Image = Global.InteractiveStudio.My.Resources.Resources.cross_24px
        Me.AccuracyButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.AccuracyButton.Name = "AccuracyButton"
        Me.AccuracyButton.SubItemsExpandWidth = 14
        Me.AccuracyButton.Text = "Accuracy Settings"
        '
        'MCUButton
        '
        Me.MCUButton.ButtonStyle = DevComponents.DotNetBar.eButtonStyle.ImageAndText
        Me.MCUButton.Image = Global.InteractiveStudio.My.Resources.Resources.MCU_32px
        Me.MCUButton.ImagePosition = DevComponents.DotNetBar.eImagePosition.Top
        Me.MCUButton.Name = "MCUButton"
        Me.MCUButton.SubItemsExpandWidth = 14
        Me.MCUButton.Text = "MCU settings"
        '
        'StartTab
        '
        Me.StartTab.Checked = True
        Me.StartTab.Name = "StartTab"
        Me.StartTab.Panel = Me.RibbonPanel1
        Me.StartTab.Text = "Start"
        '
        'SettingsTab
        '
        Me.SettingsTab.Name = "SettingsTab"
        Me.SettingsTab.Panel = Me.RibbonPanel2
        Me.SettingsTab.Text = "Settings"
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.ItemSize = New System.Drawing.Size(96, 24)
        Me.TabControl1.Location = New System.Drawing.Point(-3, 108)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(759, 324)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 2
        '
        'TabPage1
        '
        Me.TabPage1.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage1.Controls.Add(Me.WindowProgramControl1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 28)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(751, 292)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "window 1"
        '
        'WindowProgramControl1
        '
        Me.WindowProgramControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.WindowProgramControl1.Location = New System.Drawing.Point(3, 3)
        Me.WindowProgramControl1.Name = "WindowProgramControl1"
        Me.WindowProgramControl1.Size = New System.Drawing.Size(745, 286)
        Me.WindowProgramControl1.TabIndex = 0
        '
        'TabPage2
        '
        Me.TabPage2.BackColor = System.Drawing.SystemColors.Control
        Me.TabPage2.Location = New System.Drawing.Point(4, 28)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(751, 292)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "window 2"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton1})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 424)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(754, 23)
        Me.StatusStrip1.TabIndex = 3
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.Image = Global.InteractiveStudio.My.Resources.Resources.usb_disconnect_32px
        Me.ToolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.ShowDropDownArrow = False
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(166, 21)
        Me.ToolStripDropDownButton1.Text = "Connection abnormality"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(754, 447)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.RibbonControl1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "MainForm"
        Me.RibbonControl1.ResumeLayout(False)
        Me.RibbonControl1.PerformLayout()
        Me.RibbonPanel1.ResumeLayout(False)
        Me.RibbonPanel2.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents StyleManager1 As DevComponents.DotNetBar.StyleManager
    Friend WithEvents RibbonControl1 As DevComponents.DotNetBar.RibbonControl
    Friend WithEvents RibbonPanel1 As DevComponents.DotNetBar.RibbonPanel
    Friend WithEvents RibbonPanel2 As DevComponents.DotNetBar.RibbonPanel
    Friend WithEvents RibbonBar5 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents LabelItem1 As DevComponents.DotNetBar.LabelItem
    Friend WithEvents ComboBoxItem1 As DevComponents.DotNetBar.ComboBoxItem
    Friend WithEvents RibbonBar4 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents ReadScreenInformationButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ControlButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents StartTab As DevComponents.DotNetBar.RibbonTabItem
    Friend WithEvents SettingsTab As DevComponents.DotNetBar.RibbonTabItem
    Friend WithEvents SensorButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ItemContainer4 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer2 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents ItemContainer3 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents LabelItem2 As DevComponents.DotNetBar.LabelItem
    Friend WithEvents AutoRunCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents PlayWindowSettingsButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents WindowProgramControl1 As WindowProgramControl
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents MCUButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents AccuracyButton As DevComponents.DotNetBar.ButtonItem
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents RibbonBar3 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents ItemContainer1 As DevComponents.DotNetBar.ItemContainer
    Friend WithEvents HideWindowsCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents RibbonBar1 As DevComponents.DotNetBar.RibbonBar
    Friend WithEvents DebugCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents TestCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents BlackCheckBox As DevComponents.DotNetBar.CheckBoxItem
    Friend WithEvents InteractCheckBox As DevComponents.DotNetBar.CheckBoxItem
End Class
