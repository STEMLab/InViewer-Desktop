namespace InviewerDesktopGUI
{
    partial class IndoorGMLViewerForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IndoorGMLViewerForm));
            this.treeView_IndoorGML = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openIndoorGMLOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitQToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cullingSpacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalSpaceToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.transitionSpaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cellSpaceBoundaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stateSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.largeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.smallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideSelectedItemsHToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shotSelectedItemsSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toggleSelectedItemsAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToSelectedItemGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cameraTypeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.perspectiveViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.orthographicViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.sideViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frontViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer_Main = new System.Windows.Forms.SplitContainer();
            this.splitContainer_Right = new System.Windows.Forms.SplitContainer();
            this.richTextBox_IndoorGML = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_OpenFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Fit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SideView = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_FrontView = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_TopView = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_RotateView = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Ortho = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Query = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).BeginInit();
            this.splitContainer_Main.Panel1.SuspendLayout();
            this.splitContainer_Main.Panel2.SuspendLayout();
            this.splitContainer_Main.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Right)).BeginInit();
            this.splitContainer_Right.Panel2.SuspendLayout();
            this.splitContainer_Right.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView_IndoorGML
            // 
            this.treeView_IndoorGML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView_IndoorGML.CheckBoxes = true;
            this.treeView_IndoorGML.Location = new System.Drawing.Point(3, 3);
            this.treeView_IndoorGML.Name = "treeView_IndoorGML";
            this.treeView_IndoorGML.Size = new System.Drawing.Size(225, 631);
            this.treeView_IndoorGML.TabIndex = 0;
            this.treeView_IndoorGML.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.node_AfterCheck);
            this.treeView_IndoorGML.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_IndoorGML_AfterSelect);
            this.treeView_IndoorGML.DoubleClick += new System.EventHandler(this.treeView_IndoorGML_DoubleClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileFToolStripMenuItem,
            this.editEToolStripMenuItem,
            this.viewVToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1561, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileFToolStripMenuItem
            // 
            this.fileFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openIndoorGMLOToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitQToolStripMenuItem});
            this.fileFToolStripMenuItem.Name = "fileFToolStripMenuItem";
            this.fileFToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.fileFToolStripMenuItem.Text = "File(&F)";
            // 
            // openIndoorGMLOToolStripMenuItem
            // 
            this.openIndoorGMLOToolStripMenuItem.Name = "openIndoorGMLOToolStripMenuItem";
            this.openIndoorGMLOToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.openIndoorGMLOToolStripMenuItem.Text = "Open IndoorGML (&O)";
            this.openIndoorGMLOToolStripMenuItem.Click += new System.EventHandler(this.openIndoorGMLOToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(185, 6);
            // 
            // quitQToolStripMenuItem
            // 
            this.quitQToolStripMenuItem.Name = "quitQToolStripMenuItem";
            this.quitQToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
            this.quitQToolStripMenuItem.Text = "Quit(&Q)";
            this.quitQToolStripMenuItem.Click += new System.EventHandler(this.quitQToolStripMenuItem_Click);
            // 
            // editEToolStripMenuItem
            // 
            this.editEToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cullingSpacesToolStripMenuItem,
            this.stateSizeToolStripMenuItem});
            this.editEToolStripMenuItem.Name = "editEToolStripMenuItem";
            this.editEToolStripMenuItem.Size = new System.Drawing.Size(86, 20);
            this.editEToolStripMenuItem.Text = "Materials(&M)";
            // 
            // cullingSpacesToolStripMenuItem
            // 
            this.cullingSpacesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cellSpaceToolStripMenuItem,
            this.generalSpaceToolStripMenuItem1,
            this.transitionSpaceToolStripMenuItem,
            this.cellSpaceBoundaryToolStripMenuItem});
            this.cullingSpacesToolStripMenuItem.Name = "cullingSpacesToolStripMenuItem";
            this.cullingSpacesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.cullingSpacesToolStripMenuItem.Text = "Back-face Culling";
            // 
            // cellSpaceToolStripMenuItem
            // 
            this.cellSpaceToolStripMenuItem.CheckOnClick = true;
            this.cellSpaceToolStripMenuItem.Name = "cellSpaceToolStripMenuItem";
            this.cellSpaceToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.cellSpaceToolStripMenuItem.Text = "Cell Space";
            this.cellSpaceToolStripMenuItem.Click += new System.EventHandler(this.cellSpaceToolStripMenuItem_Click);
            // 
            // generalSpaceToolStripMenuItem1
            // 
            this.generalSpaceToolStripMenuItem1.CheckOnClick = true;
            this.generalSpaceToolStripMenuItem1.Name = "generalSpaceToolStripMenuItem1";
            this.generalSpaceToolStripMenuItem1.Size = new System.Drawing.Size(185, 22);
            this.generalSpaceToolStripMenuItem1.Text = "General Space";
            this.generalSpaceToolStripMenuItem1.Click += new System.EventHandler(this.generalSpaceToolStripMenuItem1_Click);
            // 
            // transitionSpaceToolStripMenuItem
            // 
            this.transitionSpaceToolStripMenuItem.CheckOnClick = true;
            this.transitionSpaceToolStripMenuItem.Name = "transitionSpaceToolStripMenuItem";
            this.transitionSpaceToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.transitionSpaceToolStripMenuItem.Text = "Transition Space";
            this.transitionSpaceToolStripMenuItem.Click += new System.EventHandler(this.transitionSpaceToolStripMenuItem_Click);
            // 
            // cellSpaceBoundaryToolStripMenuItem
            // 
            this.cellSpaceBoundaryToolStripMenuItem.CheckOnClick = true;
            this.cellSpaceBoundaryToolStripMenuItem.Name = "cellSpaceBoundaryToolStripMenuItem";
            this.cellSpaceBoundaryToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.cellSpaceBoundaryToolStripMenuItem.Text = "Cell Space Boundary";
            this.cellSpaceBoundaryToolStripMenuItem.Click += new System.EventHandler(this.cellSpaceBoundaryToolStripMenuItem_Click);
            // 
            // stateSizeToolStripMenuItem
            // 
            this.stateSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.largeToolStripMenuItem,
            this.middleToolStripMenuItem,
            this.smallToolStripMenuItem});
            this.stateSizeToolStripMenuItem.Name = "stateSizeToolStripMenuItem";
            this.stateSizeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.stateSizeToolStripMenuItem.Text = "State Size";
            // 
            // largeToolStripMenuItem
            // 
            this.largeToolStripMenuItem.CheckOnClick = true;
            this.largeToolStripMenuItem.Name = "largeToolStripMenuItem";
            this.largeToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.largeToolStripMenuItem.Text = "Large";
            this.largeToolStripMenuItem.Click += new System.EventHandler(this.largeToolStripMenuItem_Click);
            // 
            // middleToolStripMenuItem
            // 
            this.middleToolStripMenuItem.Checked = true;
            this.middleToolStripMenuItem.CheckOnClick = true;
            this.middleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.middleToolStripMenuItem.Name = "middleToolStripMenuItem";
            this.middleToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.middleToolStripMenuItem.Text = "Middle";
            this.middleToolStripMenuItem.Click += new System.EventHandler(this.middleToolStripMenuItem_Click);
            // 
            // smallToolStripMenuItem
            // 
            this.smallToolStripMenuItem.CheckOnClick = true;
            this.smallToolStripMenuItem.Name = "smallToolStripMenuItem";
            this.smallToolStripMenuItem.Size = new System.Drawing.Size(111, 22);
            this.smallToolStripMenuItem.Text = "Small";
            this.smallToolStripMenuItem.Click += new System.EventHandler(this.smallToolStripMenuItem_Click);
            // 
            // viewVToolStripMenuItem
            // 
            this.viewVToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideSelectedItemsHToolStripMenuItem,
            this.shotSelectedItemsSToolStripMenuItem,
            this.toggleSelectedItemsAToolStripMenuItem,
            this.goToSelectedItemGToolStripMenuItem,
            this.toolStripSeparator2,
            this.cameraTypeToolStripMenuItem,
            this.toolStripSeparator3,
            this.sideViewToolStripMenuItem,
            this.frontViewToolStripMenuItem,
            this.topViewToolStripMenuItem});
            this.viewVToolStripMenuItem.Name = "viewVToolStripMenuItem";
            this.viewVToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.viewVToolStripMenuItem.Text = "View(&V)";
            // 
            // hideSelectedItemsHToolStripMenuItem
            // 
            this.hideSelectedItemsHToolStripMenuItem.Name = "hideSelectedItemsHToolStripMenuItem";
            this.hideSelectedItemsHToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.H)));
            this.hideSelectedItemsHToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.hideSelectedItemsHToolStripMenuItem.Text = "Hide selected items (&H)";
            this.hideSelectedItemsHToolStripMenuItem.Click += new System.EventHandler(this.hideSelectedItemsHToolStripMenuItem_Click);
            // 
            // shotSelectedItemsSToolStripMenuItem
            // 
            this.shotSelectedItemsSToolStripMenuItem.Name = "shotSelectedItemsSToolStripMenuItem";
            this.shotSelectedItemsSToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.shotSelectedItemsSToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.shotSelectedItemsSToolStripMenuItem.Text = "Show selected items (&J)";
            this.shotSelectedItemsSToolStripMenuItem.Click += new System.EventHandler(this.shotSelectedItemsSToolStripMenuItem_Click);
            // 
            // toggleSelectedItemsAToolStripMenuItem
            // 
            this.toggleSelectedItemsAToolStripMenuItem.Name = "toggleSelectedItemsAToolStripMenuItem";
            this.toggleSelectedItemsAToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.A)));
            this.toggleSelectedItemsAToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.toggleSelectedItemsAToolStripMenuItem.Text = "Toggle selected item (&A)";
            this.toggleSelectedItemsAToolStripMenuItem.Click += new System.EventHandler(this.toggleSelectedItemsAToolStripMenuItem_Click);
            // 
            // goToSelectedItemGToolStripMenuItem
            // 
            this.goToSelectedItemGToolStripMenuItem.Name = "goToSelectedItemGToolStripMenuItem";
            this.goToSelectedItemGToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.goToSelectedItemGToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.goToSelectedItemGToolStripMenuItem.Text = "Go to selected item (&G)";
            this.goToSelectedItemGToolStripMenuItem.Click += new System.EventHandler(this.goToSelectedItemGToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(273, 6);
            // 
            // cameraTypeToolStripMenuItem
            // 
            this.cameraTypeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.perspectiveViewToolStripMenuItem,
            this.orthographicViewToolStripMenuItem});
            this.cameraTypeToolStripMenuItem.Name = "cameraTypeToolStripMenuItem";
            this.cameraTypeToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.cameraTypeToolStripMenuItem.Text = "Projection";
            // 
            // perspectiveViewToolStripMenuItem
            // 
            this.perspectiveViewToolStripMenuItem.Checked = true;
            this.perspectiveViewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.perspectiveViewToolStripMenuItem.Name = "perspectiveViewToolStripMenuItem";
            this.perspectiveViewToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.perspectiveViewToolStripMenuItem.Text = "Perspective View";
            this.perspectiveViewToolStripMenuItem.Click += new System.EventHandler(this.perspectiveViewToolStripMenuItem_Click);
            // 
            // orthographicViewToolStripMenuItem
            // 
            this.orthographicViewToolStripMenuItem.Name = "orthographicViewToolStripMenuItem";
            this.orthographicViewToolStripMenuItem.Size = new System.Drawing.Size(175, 22);
            this.orthographicViewToolStripMenuItem.Text = "Orthographic View";
            this.orthographicViewToolStripMenuItem.Click += new System.EventHandler(this.orthographicViewToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(273, 6);
            // 
            // sideViewToolStripMenuItem
            // 
            this.sideViewToolStripMenuItem.Name = "sideViewToolStripMenuItem";
            this.sideViewToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.sideViewToolStripMenuItem.Text = "Side View";
            this.sideViewToolStripMenuItem.Click += new System.EventHandler(this.sideViewToolStripMenuItem_Click);
            // 
            // frontViewToolStripMenuItem
            // 
            this.frontViewToolStripMenuItem.Name = "frontViewToolStripMenuItem";
            this.frontViewToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.frontViewToolStripMenuItem.Text = "Front View";
            this.frontViewToolStripMenuItem.Click += new System.EventHandler(this.frontViewToolStripMenuItem_Click);
            // 
            // topViewToolStripMenuItem
            // 
            this.topViewToolStripMenuItem.Name = "topViewToolStripMenuItem";
            this.topViewToolStripMenuItem.Size = new System.Drawing.Size(276, 22);
            this.topViewToolStripMenuItem.Text = "Top View";
            this.topViewToolStripMenuItem.Click += new System.EventHandler(this.topViewToolStripMenuItem_Click);
            // 
            // splitContainer_Main
            // 
            this.splitContainer_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer_Main.Location = new System.Drawing.Point(12, 67);
            this.splitContainer_Main.Name = "splitContainer_Main";
            // 
            // splitContainer_Main.Panel1
            // 
            this.splitContainer_Main.Panel1.Controls.Add(this.treeView_IndoorGML);
            // 
            // splitContainer_Main.Panel2
            // 
            this.splitContainer_Main.Panel2.Controls.Add(this.splitContainer_Right);
            this.splitContainer_Main.Size = new System.Drawing.Size(1537, 647);
            this.splitContainer_Main.SplitterDistance = 231;
            this.splitContainer_Main.TabIndex = 3;
            // 
            // splitContainer_Right
            // 
            this.splitContainer_Right.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer_Right.Location = new System.Drawing.Point(0, 0);
            this.splitContainer_Right.Name = "splitContainer_Right";
            this.splitContainer_Right.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer_Right.Panel1
            // 
            this.splitContainer_Right.Panel1.BackColor = System.Drawing.Color.Coral;
            this.splitContainer_Right.Panel1.Resize += new System.EventHandler(this.Panel_Unity3D_Resize);
            // 
            // splitContainer_Right.Panel2
            // 
            this.splitContainer_Right.Panel2.Controls.Add(this.richTextBox_IndoorGML);
            this.splitContainer_Right.Size = new System.Drawing.Size(1302, 647);
            this.splitContainer_Right.SplitterDistance = 453;
            this.splitContainer_Right.TabIndex = 0;
            // 
            // richTextBox_IndoorGML
            // 
            this.richTextBox_IndoorGML.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox_IndoorGML.AutoWordSelection = true;
            this.richTextBox_IndoorGML.HideSelection = false;
            this.richTextBox_IndoorGML.Location = new System.Drawing.Point(3, 3);
            this.richTextBox_IndoorGML.Name = "richTextBox_IndoorGML";
            this.richTextBox_IndoorGML.ReadOnly = true;
            this.richTextBox_IndoorGML.Size = new System.Drawing.Size(1296, 174);
            this.richTextBox_IndoorGML.TabIndex = 0;
            this.richTextBox_IndoorGML.Text = "";
            this.richTextBox_IndoorGML.WordWrap = false;
            this.richTextBox_IndoorGML.SelectionChanged += new System.EventHandler(this.richTextBox_IndoorGML_SelectionChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(35, 35);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_OpenFile,
            this.toolStripSeparator4,
            this.toolStripButton_Fit,
            this.toolStripButton_SideView,
            this.toolStripButton_FrontView,
            this.toolStripButton_TopView,
            this.toolStripButton_RotateView,
            this.toolStripSeparator6,
            this.toolStripButton_Ortho,
            this.toolStripSeparator5,
            this.toolStripButton_Query});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1561, 40);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_OpenFile
            // 
            this.toolStripButton_OpenFile.AutoSize = false;
            this.toolStripButton_OpenFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_OpenFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_OpenFile.Image")));
            this.toolStripButton_OpenFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_OpenFile.Name = "toolStripButton_OpenFile";
            this.toolStripButton_OpenFile.Size = new System.Drawing.Size(40, 40);
            this.toolStripButton_OpenFile.Text = "Open a IndoorGML file";
            this.toolStripButton_OpenFile.Click += new System.EventHandler(this.toolStripButton_OpenFile_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 40);
            // 
            // toolStripButton_Fit
            // 
            this.toolStripButton_Fit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Fit.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Fit.Image")));
            this.toolStripButton_Fit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Fit.Name = "toolStripButton_Fit";
            this.toolStripButton_Fit.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_Fit.Text = "Fit to window";
            this.toolStripButton_Fit.Click += new System.EventHandler(this.toolStripButton_Fit_Click);
            // 
            // toolStripButton_SideView
            // 
            this.toolStripButton_SideView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_SideView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SideView.Image")));
            this.toolStripButton_SideView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SideView.Name = "toolStripButton_SideView";
            this.toolStripButton_SideView.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_SideView.Text = "Side View";
            this.toolStripButton_SideView.Click += new System.EventHandler(this.toolStripButton_SideView_Click);
            // 
            // toolStripButton_FrontView
            // 
            this.toolStripButton_FrontView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_FrontView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_FrontView.Image")));
            this.toolStripButton_FrontView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_FrontView.Name = "toolStripButton_FrontView";
            this.toolStripButton_FrontView.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_FrontView.Text = "Front View";
            this.toolStripButton_FrontView.Click += new System.EventHandler(this.toolStripButton_FrontView_Click);
            // 
            // toolStripButton_TopView
            // 
            this.toolStripButton_TopView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_TopView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_TopView.Image")));
            this.toolStripButton_TopView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_TopView.Name = "toolStripButton_TopView";
            this.toolStripButton_TopView.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_TopView.Text = "Top View";
            this.toolStripButton_TopView.Click += new System.EventHandler(this.toolStripButton_TopView_Click);
            // 
            // toolStripButton_RotateView
            // 
            this.toolStripButton_RotateView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_RotateView.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_RotateView.Image")));
            this.toolStripButton_RotateView.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_RotateView.Name = "toolStripButton_RotateView";
            this.toolStripButton_RotateView.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_RotateView.Text = "toolStripButton1";
            this.toolStripButton_RotateView.Click += new System.EventHandler(this.toolStripButton_RotateView_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 40);
            // 
            // toolStripButton_Ortho
            // 
            this.toolStripButton_Ortho.CheckOnClick = true;
            this.toolStripButton_Ortho.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Ortho.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Ortho.Image")));
            this.toolStripButton_Ortho.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Ortho.Name = "toolStripButton_Ortho";
            this.toolStripButton_Ortho.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_Ortho.Text = "Toggle orthogonal-view mode";
            this.toolStripButton_Ortho.Click += new System.EventHandler(this.toolStripButton_Ortho_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 40);
            this.toolStripSeparator5.Visible = false;
            // 
            // toolStripButton_Query
            // 
            this.toolStripButton_Query.CheckOnClick = true;
            this.toolStripButton_Query.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton_Query.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Query.Image")));
            this.toolStripButton_Query.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Query.Name = "toolStripButton_Query";
            this.toolStripButton_Query.Size = new System.Drawing.Size(39, 37);
            this.toolStripButton_Query.Text = "toolStripButton1";
            this.toolStripButton_Query.Click += new System.EventHandler(this.toolStripButton_Query_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 704);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1561, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1546, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Ready";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // IndoorGMLViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1561, 726);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.splitContainer_Main);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IndoorGMLViewerForm";
            this.Text = "Inviewer-Desktop";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer_Main.Panel1.ResumeLayout(false);
            this.splitContainer_Main.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Main)).EndInit();
            this.splitContainer_Main.ResumeLayout(false);
            this.splitContainer_Right.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer_Right)).EndInit();
            this.splitContainer_Right.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView_IndoorGML;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editEToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openIndoorGMLOToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitQToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer_Main;
        private System.Windows.Forms.SplitContainer splitContainer_Right;
        private System.Windows.Forms.ToolStripMenuItem hideSelectedItemsHToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem shotSelectedItemsSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toggleSelectedItemsAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cullingSpacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cellSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalSpaceToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem transitionSpaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cellSpaceBoundaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stateSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem largeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem middleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem smallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToSelectedItemGToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cameraTypeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem orthographicViewToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton_OpenFile;
        private System.Windows.Forms.ToolStripButton toolStripButton_Fit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolStripButton_SideView;
        private System.Windows.Forms.ToolStripButton toolStripButton_FrontView;
        private System.Windows.Forms.ToolStripButton toolStripButton_TopView;
        private System.Windows.Forms.ToolStripButton toolStripButton_Ortho;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem perspectiveViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sideViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frontViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_RotateView;
        private System.Windows.Forms.ToolStripButton toolStripButton_Query;
        private System.Windows.Forms.RichTextBox richTextBox_IndoorGML;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}

