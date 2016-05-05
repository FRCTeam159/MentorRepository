namespace GazeboExporter.UI
{
    partial class LinkProperties
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.NameLabel = new System.Windows.Forms.Label();
            this.colorPicker = new System.Windows.Forms.ColorDialog();
            this.ColorLabel = new System.Windows.Forms.Label();
            this.changeColor = new System.Windows.Forms.Button();
            this.CustomMassValCheckbox = new System.Windows.Forms.CheckBox();
            this.MassLabel = new System.Windows.Forms.Label();
            this.MassTextbox = new System.Windows.Forms.TextBox();
            this.IxxLabel = new System.Windows.Forms.Label();
            this.IxxTextbox = new System.Windows.Forms.TextBox();
            this.IxyLabel = new System.Windows.Forms.Label();
            this.IxyTextbox = new System.Windows.Forms.TextBox();
            this.IyyLabel = new System.Windows.Forms.Label();
            this.IyyTextbox = new System.Windows.Forms.TextBox();
            this.IxzLabel = new System.Windows.Forms.Label();
            this.IxzTextbox = new System.Windows.Forms.TextBox();
            this.IzzLabel = new System.Windows.Forms.Label();
            this.IzzTextbox = new System.Windows.Forms.TextBox();
            this.IyzLabel = new System.Windows.Forms.Label();
            this.IyzTextbox = new System.Windows.Forms.TextBox();
            this.ComXLabel = new System.Windows.Forms.Label();
            this.ComXTextbox = new System.Windows.Forms.TextBox();
            this.ComZLabel = new System.Windows.Forms.Label();
            this.ComZTextbox = new System.Windows.Forms.TextBox();
            this.ComYLabel = new System.Windows.Forms.Label();
            this.ComYTextbox = new System.Windows.Forms.TextBox();
            this.mu1Label = new System.Windows.Forms.Label();
            this.Mu1Textbox = new System.Windows.Forms.TextBox();
            this.mu2Label = new System.Windows.Forms.Label();
            this.Mu2Textbox = new System.Windows.Forms.TextBox();
            this.kdLabel = new System.Windows.Forms.Label();
            this.KdTextbox = new System.Windows.Forms.TextBox();
            this.kpLabel = new System.Windows.Forms.Label();
            this.KpTextbox = new System.Windows.Forms.TextBox();
            this.InertiaButton = new System.Windows.Forms.Button();
            this.PhysicalGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.selfCollideCheckbox = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LinearDampingTextbox = new System.Windows.Forms.TextBox();
            this.AngularDampingTextbox = new System.Windows.Forms.TextBox();
            this.CustomInertiaGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.PhysicalButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.VisualButton = new System.Windows.Forms.Button();
            this.CollisionButton = new System.Windows.Forms.Button();
            this.PhysicalGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.CustomInertiaGroupBox.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameTextBox
            // 
            this.nameTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nameTextBox.Location = new System.Drawing.Point(64, 5);
            this.nameTextBox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.Size = new System.Drawing.Size(259, 26);
            this.nameTextBox.TabIndex = 0;
            this.nameTextBox.Leave += new System.EventHandler(this.nameTextBox_LostFocus);
            // 
            // NameLabel
            // 
            this.NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(3, 8);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(55, 20);
            this.NameLabel.TabIndex = 1;
            this.NameLabel.Text = "Name:";
            // 
            // ColorLabel
            // 
            this.ColorLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ColorLabel.AutoSize = true;
            this.ColorLabel.Location = new System.Drawing.Point(8, 43);
            this.ColorLabel.Name = "ColorLabel";
            this.ColorLabel.Size = new System.Drawing.Size(50, 20);
            this.ColorLabel.TabIndex = 2;
            this.ColorLabel.Text = "Color:";
            // 
            // changeColor
            // 
            this.changeColor.BackColor = System.Drawing.Color.White;
            this.changeColor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changeColor.Location = new System.Drawing.Point(64, 39);
            this.changeColor.Name = "changeColor";
            this.changeColor.Size = new System.Drawing.Size(259, 29);
            this.changeColor.TabIndex = 3;
            this.changeColor.UseVisualStyleBackColor = false;
            this.changeColor.Click += new System.EventHandler(this.changeColor_Click);
            // 
            // CustomMassValCheckbox
            // 
            this.CustomMassValCheckbox.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.CustomMassValCheckbox, 2);
            this.CustomMassValCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CustomMassValCheckbox.Location = new System.Drawing.Point(3, 363);
            this.CustomMassValCheckbox.Name = "CustomMassValCheckbox";
            this.CustomMassValCheckbox.Size = new System.Drawing.Size(320, 24);
            this.CustomMassValCheckbox.TabIndex = 4;
            this.CustomMassValCheckbox.Text = "Use Custom Inertia Values";
            this.CustomMassValCheckbox.UseVisualStyleBackColor = true;
            this.CustomMassValCheckbox.CheckedChanged += new System.EventHandler(this.CustomMassValCheckbox_CheckedChanged);
            // 
            // MassLabel
            // 
            this.MassLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.MassLabel.AutoSize = true;
            this.MassLabel.Location = new System.Drawing.Point(9, 8);
            this.MassLabel.Name = "MassLabel";
            this.MassLabel.Size = new System.Drawing.Size(51, 20);
            this.MassLabel.TabIndex = 6;
            this.MassLabel.Text = "Mass:";
            // 
            // MassTextbox
            // 
            this.MassTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MassTextbox.Location = new System.Drawing.Point(66, 5);
            this.MassTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MassTextbox.Name = "MassTextbox";
            this.MassTextbox.Size = new System.Drawing.Size(92, 26);
            this.MassTextbox.TabIndex = 5;
            this.MassTextbox.TextChanged += new System.EventHandler(this.MassTextbox_TextChanged);
            // 
            // IxxLabel
            // 
            this.IxxLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IxxLabel.AutoSize = true;
            this.IxxLabel.Location = new System.Drawing.Point(30, 80);
            this.IxxLabel.Name = "IxxLabel";
            this.IxxLabel.Size = new System.Drawing.Size(30, 20);
            this.IxxLabel.TabIndex = 8;
            this.IxxLabel.Text = "ixx:";
            // 
            // IxxTextbox
            // 
            this.IxxTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IxxTextbox.Location = new System.Drawing.Point(66, 77);
            this.IxxTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IxxTextbox.Name = "IxxTextbox";
            this.IxxTextbox.Size = new System.Drawing.Size(92, 26);
            this.IxxTextbox.TabIndex = 7;
            this.IxxTextbox.TextChanged += new System.EventHandler(this.IxxTextbox_TextChanged);
            // 
            // IxyLabel
            // 
            this.IxyLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IxyLabel.AutoSize = true;
            this.IxyLabel.Location = new System.Drawing.Point(191, 80);
            this.IxyLabel.Name = "IxyLabel";
            this.IxyLabel.Size = new System.Drawing.Size(30, 20);
            this.IxyLabel.TabIndex = 10;
            this.IxyLabel.Text = "ixy:";
            // 
            // IxyTextbox
            // 
            this.IxyTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IxyTextbox.Location = new System.Drawing.Point(227, 77);
            this.IxyTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IxyTextbox.Name = "IxyTextbox";
            this.IxyTextbox.Size = new System.Drawing.Size(84, 26);
            this.IxyTextbox.TabIndex = 9;
            this.IxyTextbox.TextChanged += new System.EventHandler(this.IxyTextbox_TextChanged);
            // 
            // IyyLabel
            // 
            this.IyyLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IyyLabel.AutoSize = true;
            this.IyyLabel.Location = new System.Drawing.Point(30, 116);
            this.IyyLabel.Name = "IyyLabel";
            this.IyyLabel.Size = new System.Drawing.Size(30, 20);
            this.IyyLabel.TabIndex = 14;
            this.IyyLabel.Text = "iyy:";
            // 
            // IyyTextbox
            // 
            this.IyyTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IyyTextbox.Location = new System.Drawing.Point(66, 113);
            this.IyyTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IyyTextbox.Name = "IyyTextbox";
            this.IyyTextbox.Size = new System.Drawing.Size(92, 26);
            this.IyyTextbox.TabIndex = 13;
            this.IyyTextbox.TextChanged += new System.EventHandler(this.IyyTextbox_TextChanged);
            // 
            // IxzLabel
            // 
            this.IxzLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IxzLabel.AutoSize = true;
            this.IxzLabel.Location = new System.Drawing.Point(190, 152);
            this.IxzLabel.Name = "IxzLabel";
            this.IxzLabel.Size = new System.Drawing.Size(31, 20);
            this.IxzLabel.TabIndex = 12;
            this.IxzLabel.Text = "ixz:";
            // 
            // IxzTextbox
            // 
            this.IxzTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IxzTextbox.Location = new System.Drawing.Point(227, 149);
            this.IxzTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IxzTextbox.Name = "IxzTextbox";
            this.IxzTextbox.Size = new System.Drawing.Size(84, 26);
            this.IxzTextbox.TabIndex = 11;
            this.IxzTextbox.TextChanged += new System.EventHandler(this.IxzTextbox_TextChanged);
            // 
            // IzzLabel
            // 
            this.IzzLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IzzLabel.AutoSize = true;
            this.IzzLabel.Location = new System.Drawing.Point(28, 152);
            this.IzzLabel.Name = "IzzLabel";
            this.IzzLabel.Size = new System.Drawing.Size(32, 20);
            this.IzzLabel.TabIndex = 18;
            this.IzzLabel.Text = "izz:";
            // 
            // IzzTextbox
            // 
            this.IzzTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IzzTextbox.Location = new System.Drawing.Point(66, 149);
            this.IzzTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IzzTextbox.Name = "IzzTextbox";
            this.IzzTextbox.Size = new System.Drawing.Size(92, 26);
            this.IzzTextbox.TabIndex = 17;
            this.IzzTextbox.TextChanged += new System.EventHandler(this.IzzTextbox_TextChanged);
            // 
            // IyzLabel
            // 
            this.IyzLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.IyzLabel.AutoSize = true;
            this.IyzLabel.Location = new System.Drawing.Point(190, 116);
            this.IyzLabel.Name = "IyzLabel";
            this.IyzLabel.Size = new System.Drawing.Size(31, 20);
            this.IyzLabel.TabIndex = 16;
            this.IyzLabel.Text = "iyz:";
            // 
            // IyzTextbox
            // 
            this.IyzTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IyzTextbox.Location = new System.Drawing.Point(227, 113);
            this.IyzTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.IyzTextbox.Name = "IyzTextbox";
            this.IyzTextbox.Size = new System.Drawing.Size(84, 26);
            this.IyzTextbox.TabIndex = 15;
            this.IyzTextbox.TextChanged += new System.EventHandler(this.IyzTextbox_TextChanged);
            // 
            // ComXLabel
            // 
            this.ComXLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ComXLabel.AutoSize = true;
            this.ComXLabel.Location = new System.Drawing.Point(164, 8);
            this.ComXLabel.Name = "ComXLabel";
            this.ComXLabel.Size = new System.Drawing.Size(57, 20);
            this.ComXLabel.TabIndex = 20;
            this.ComXLabel.Text = "ComX:";
            // 
            // ComXTextbox
            // 
            this.ComXTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComXTextbox.Location = new System.Drawing.Point(227, 5);
            this.ComXTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ComXTextbox.Name = "ComXTextbox";
            this.ComXTextbox.Size = new System.Drawing.Size(84, 26);
            this.ComXTextbox.TabIndex = 19;
            this.ComXTextbox.TextChanged += new System.EventHandler(this.ComXTextbox_TextChanged);
            // 
            // ComZLabel
            // 
            this.ComZLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ComZLabel.AutoSize = true;
            this.ComZLabel.Location = new System.Drawing.Point(165, 44);
            this.ComZLabel.Name = "ComZLabel";
            this.ComZLabel.Size = new System.Drawing.Size(56, 20);
            this.ComZLabel.TabIndex = 22;
            this.ComZLabel.Text = "ComZ:";
            // 
            // ComZTextbox
            // 
            this.ComZTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComZTextbox.Location = new System.Drawing.Point(227, 41);
            this.ComZTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ComZTextbox.Name = "ComZTextbox";
            this.ComZTextbox.Size = new System.Drawing.Size(84, 26);
            this.ComZTextbox.TabIndex = 21;
            this.ComZTextbox.TextChanged += new System.EventHandler(this.ComZTextbox_TextChanged);
            // 
            // ComYLabel
            // 
            this.ComYLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ComYLabel.AutoSize = true;
            this.ComYLabel.Location = new System.Drawing.Point(3, 44);
            this.ComYLabel.Name = "ComYLabel";
            this.ComYLabel.Size = new System.Drawing.Size(57, 20);
            this.ComYLabel.TabIndex = 24;
            this.ComYLabel.Text = "ComY:";
            // 
            // ComYTextbox
            // 
            this.ComYTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ComYTextbox.Location = new System.Drawing.Point(66, 41);
            this.ComYTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.ComYTextbox.Name = "ComYTextbox";
            this.ComYTextbox.Size = new System.Drawing.Size(92, 26);
            this.ComYTextbox.TabIndex = 23;
            this.ComYTextbox.TextChanged += new System.EventHandler(this.ComYTextbox_TextChanged);
            // 
            // mu1Label
            // 
            this.mu1Label.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mu1Label.AutoSize = true;
            this.mu1Label.Location = new System.Drawing.Point(40, 8);
            this.mu1Label.Name = "mu1Label";
            this.mu1Label.Size = new System.Drawing.Size(44, 20);
            this.mu1Label.TabIndex = 26;
            this.mu1Label.Text = "mu1:";
            // 
            // Mu1Textbox
            // 
            this.Mu1Textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Mu1Textbox.Location = new System.Drawing.Point(90, 5);
            this.Mu1Textbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Mu1Textbox.Name = "Mu1Textbox";
            this.Mu1Textbox.Size = new System.Drawing.Size(60, 26);
            this.Mu1Textbox.TabIndex = 25;
            this.Mu1Textbox.TextChanged += new System.EventHandler(this.Mu1Textbox_TextChanged);
            // 
            // mu2Label
            // 
            this.mu2Label.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.mu2Label.AutoSize = true;
            this.mu2Label.Location = new System.Drawing.Point(201, 8);
            this.mu2Label.Name = "mu2Label";
            this.mu2Label.Size = new System.Drawing.Size(44, 20);
            this.mu2Label.TabIndex = 28;
            this.mu2Label.Text = "mu2:";
            // 
            // Mu2Textbox
            // 
            this.Mu2Textbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Mu2Textbox.Location = new System.Drawing.Point(251, 5);
            this.Mu2Textbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Mu2Textbox.Name = "Mu2Textbox";
            this.Mu2Textbox.Size = new System.Drawing.Size(60, 26);
            this.Mu2Textbox.TabIndex = 27;
            this.Mu2Textbox.TextChanged += new System.EventHandler(this.Mu2Textbox_TextChanged);
            // 
            // kdLabel
            // 
            this.kdLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.kdLabel.AutoSize = true;
            this.kdLabel.Location = new System.Drawing.Point(215, 44);
            this.kdLabel.Name = "kdLabel";
            this.kdLabel.Size = new System.Drawing.Size(30, 20);
            this.kdLabel.TabIndex = 36;
            this.kdLabel.Text = "kd:";
            // 
            // KdTextbox
            // 
            this.KdTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KdTextbox.Location = new System.Drawing.Point(251, 41);
            this.KdTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.KdTextbox.Name = "KdTextbox";
            this.KdTextbox.Size = new System.Drawing.Size(60, 26);
            this.KdTextbox.TabIndex = 35;
            this.KdTextbox.TextChanged += new System.EventHandler(this.KdTextbox_TextChanged);
            // 
            // kpLabel
            // 
            this.kpLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.kpLabel.AutoSize = true;
            this.kpLabel.Location = new System.Drawing.Point(54, 44);
            this.kpLabel.Name = "kpLabel";
            this.kpLabel.Size = new System.Drawing.Size(30, 20);
            this.kpLabel.TabIndex = 34;
            this.kpLabel.Text = "kp:";
            // 
            // KpTextbox
            // 
            this.KpTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.KpTextbox.Location = new System.Drawing.Point(90, 41);
            this.KpTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.KpTextbox.Name = "KpTextbox";
            this.KpTextbox.Size = new System.Drawing.Size(60, 26);
            this.KpTextbox.TabIndex = 33;
            this.KpTextbox.TextChanged += new System.EventHandler(this.KpTextBox_TextChanged);
            // 
            // InertiaButton
            // 
            this.InertiaButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.InertiaButton.Location = new System.Drawing.Point(3, 202);
            this.InertiaButton.Name = "InertiaButton";
            this.InertiaButton.Size = new System.Drawing.Size(314, 34);
            this.InertiaButton.TabIndex = 37;
            this.InertiaButton.Text = "Recalculate Inertial Values";
            this.InertiaButton.UseVisualStyleBackColor = true;
            this.InertiaButton.Click += new System.EventHandler(this.InertiaButton_Click);
            // 
            // PhysicalGroupBox
            // 
            this.PhysicalGroupBox.AutoSize = true;
            this.PhysicalGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.PhysicalGroupBox, 2);
            this.PhysicalGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.PhysicalGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PhysicalGroupBox.Location = new System.Drawing.Point(3, 194);
            this.PhysicalGroupBox.Name = "PhysicalGroupBox";
            this.PhysicalGroupBox.Size = new System.Drawing.Size(320, 163);
            this.PhysicalGroupBox.TabIndex = 38;
            this.PhysicalGroupBox.TabStop = false;
            this.PhysicalGroupBox.Text = "Physical Properties";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.selfCollideCheckbox, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.mu1Label, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.KdTextbox, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.Mu2Textbox, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.kpLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.LinearDampingTextbox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.kdLabel, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.Mu1Textbox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.KpTextbox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.mu2Label, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.AngularDampingTextbox, 3, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(314, 138);
            this.tableLayoutPanel2.TabIndex = 43;
            // 
            // selfCollideCheckbox
            // 
            this.selfCollideCheckbox.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.selfCollideCheckbox, 4);
            this.selfCollideCheckbox.Location = new System.Drawing.Point(3, 111);
            this.selfCollideCheckbox.Name = "selfCollideCheckbox";
            this.selfCollideCheckbox.Size = new System.Drawing.Size(114, 24);
            this.selfCollideCheckbox.TabIndex = 42;
            this.selfCollideCheckbox.Text = "Self Collide";
            this.selfCollideCheckbox.UseVisualStyleBackColor = true;
            this.selfCollideCheckbox.CheckedChanged += new System.EventHandler(this.selfCollideCheckbox_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(156, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 20);
            this.label3.TabIndex = 46;
            this.label3.Text = "Ang Damp:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 20);
            this.label4.TabIndex = 44;
            this.label4.Text = "Lin Damp:";
            // 
            // LinearDampingTextbox
            // 
            this.LinearDampingTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LinearDampingTextbox.Location = new System.Drawing.Point(90, 77);
            this.LinearDampingTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.LinearDampingTextbox.Name = "LinearDampingTextbox";
            this.LinearDampingTextbox.Size = new System.Drawing.Size(60, 26);
            this.LinearDampingTextbox.TabIndex = 45;
            this.LinearDampingTextbox.TextChanged += new System.EventHandler(this.LinearDampningTextbox_TextChanged);
            // 
            // AngularDampingTextbox
            // 
            this.AngularDampingTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AngularDampingTextbox.Location = new System.Drawing.Point(251, 77);
            this.AngularDampingTextbox.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.AngularDampingTextbox.Name = "AngularDampingTextbox";
            this.AngularDampingTextbox.Size = new System.Drawing.Size(60, 26);
            this.AngularDampingTextbox.TabIndex = 47;
            this.AngularDampingTextbox.TextChanged += new System.EventHandler(this.AngularDampingTextbox_TextChanged);
            // 
            // CustomInertiaGroupBox
            // 
            this.CustomInertiaGroupBox.AutoSize = true;
            this.CustomInertiaGroupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.CustomInertiaGroupBox, 2);
            this.CustomInertiaGroupBox.Controls.Add(this.tableLayoutPanel3);
            this.CustomInertiaGroupBox.Controls.Add(this.InertiaButton);
            this.CustomInertiaGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CustomInertiaGroupBox.Location = new System.Drawing.Point(3, 393);
            this.CustomInertiaGroupBox.Name = "CustomInertiaGroupBox";
            this.CustomInertiaGroupBox.Size = new System.Drawing.Size(320, 239);
            this.CustomInertiaGroupBox.TabIndex = 39;
            this.CustomInertiaGroupBox.TabStop = false;
            this.CustomInertiaGroupBox.Text = "Custom Inertia Values";
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.IxzTextbox, 3, 4);
            this.tableLayoutPanel3.Controls.Add(this.ComXTextbox, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.ComZTextbox, 3, 1);
            this.tableLayoutPanel3.Controls.Add(this.IyzLabel, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.MassLabel, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.IxyTextbox, 3, 2);
            this.tableLayoutPanel3.Controls.Add(this.IyzTextbox, 3, 3);
            this.tableLayoutPanel3.Controls.Add(this.IxzLabel, 2, 4);
            this.tableLayoutPanel3.Controls.Add(this.ComYLabel, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.IxxLabel, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.ComYTextbox, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.ComZLabel, 2, 1);
            this.tableLayoutPanel3.Controls.Add(this.IyyLabel, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.IyyTextbox, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.IxyLabel, 2, 2);
            this.tableLayoutPanel3.Controls.Add(this.ComXLabel, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.IzzLabel, 0, 4);
            this.tableLayoutPanel3.Controls.Add(this.MassTextbox, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.IxxTextbox, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.IzzTextbox, 1, 4);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 6;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(314, 180);
            this.tableLayoutPanel3.TabIndex = 43;
            // 
            // RemoveButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.RemoveButton, 2);
            this.RemoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoveButton.Location = new System.Drawing.Point(3, 638);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(320, 34);
            this.RemoveButton.TabIndex = 38;
            this.RemoveButton.Text = "Remove link";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // PhysicalButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.PhysicalButton, 2);
            this.PhysicalButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PhysicalButton.Location = new System.Drawing.Point(3, 154);
            this.PhysicalButton.Name = "PhysicalButton";
            this.PhysicalButton.Size = new System.Drawing.Size(320, 34);
            this.PhysicalButton.TabIndex = 41;
            this.PhysicalButton.Text = "Physical Components";
            this.PhysicalButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.PhysicalButton.UseVisualStyleBackColor = true;
            this.PhysicalButton.Click += new System.EventHandler(this.PhysicalButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.NameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.RemoveButton, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.CustomInertiaGroupBox, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.PhysicalButton, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.PhysicalGroupBox, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.CustomMassValCheckbox, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.nameTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ColorLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.changeColor, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.VisualButton, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.CollisionButton, 0, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 9;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(326, 675);
            this.tableLayoutPanel1.TabIndex = 42;
            // 
            // VisualButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.VisualButton, 2);
            this.VisualButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VisualButton.Location = new System.Drawing.Point(3, 74);
            this.VisualButton.Name = "VisualButton";
            this.VisualButton.Size = new System.Drawing.Size(320, 34);
            this.VisualButton.TabIndex = 42;
            this.VisualButton.Text = "Visual Components";
            this.VisualButton.UseVisualStyleBackColor = true;
            this.VisualButton.Click += new System.EventHandler(this.VisualButton_Click);
            // 
            // COllisionButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.CollisionButton, 2);
            this.CollisionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CollisionButton.Location = new System.Drawing.Point(3, 114);
            this.CollisionButton.Name = "COllisionButton";
            this.CollisionButton.Size = new System.Drawing.Size(320, 34);
            this.CollisionButton.TabIndex = 43;
            this.CollisionButton.Text = "Collision Components";
            this.CollisionButton.UseVisualStyleBackColor = true;
            this.CollisionButton.Click += new System.EventHandler(this.COllisionButton_Click);
            // 
            // LinkProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "LinkProperties";
            this.Size = new System.Drawing.Size(326, 675);
            this.Load += new System.EventHandler(this.LoadToolTips);
            this.ParentChanged += new System.EventHandler(this.Parent_Changed);
            this.PhysicalGroupBox.ResumeLayout(false);
            this.PhysicalGroupBox.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.CustomInertiaGroupBox.ResumeLayout(false);
            this.CustomInertiaGroupBox.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.ColorDialog colorPicker;
        private System.Windows.Forms.Label ColorLabel;
        private System.Windows.Forms.Button changeColor;
        private System.Windows.Forms.CheckBox CustomMassValCheckbox;
        private System.Windows.Forms.Label MassLabel;
        private System.Windows.Forms.TextBox MassTextbox;
        private System.Windows.Forms.Label IxxLabel;
        private System.Windows.Forms.TextBox IxxTextbox;
        private System.Windows.Forms.Label IxyLabel;
        private System.Windows.Forms.TextBox IxyTextbox;
        private System.Windows.Forms.Label IyyLabel;
        private System.Windows.Forms.TextBox IyyTextbox;
        private System.Windows.Forms.Label IxzLabel;
        private System.Windows.Forms.TextBox IxzTextbox;
        private System.Windows.Forms.Label IzzLabel;
        private System.Windows.Forms.TextBox IzzTextbox;
        private System.Windows.Forms.Label IyzLabel;
        private System.Windows.Forms.TextBox IyzTextbox;
        private System.Windows.Forms.Label ComXLabel;
        private System.Windows.Forms.TextBox ComXTextbox;
        private System.Windows.Forms.Label ComZLabel;
        private System.Windows.Forms.TextBox ComZTextbox;
        private System.Windows.Forms.Label ComYLabel;
        private System.Windows.Forms.TextBox ComYTextbox;
        private System.Windows.Forms.Label mu1Label;
        private System.Windows.Forms.TextBox Mu1Textbox;
        private System.Windows.Forms.Label mu2Label;
        private System.Windows.Forms.TextBox Mu2Textbox;
        private System.Windows.Forms.Label kdLabel;
        private System.Windows.Forms.TextBox KdTextbox;
        private System.Windows.Forms.Label kpLabel;
        private System.Windows.Forms.TextBox KpTextbox;
        private System.Windows.Forms.Button InertiaButton;
        private System.Windows.Forms.GroupBox PhysicalGroupBox;
        private System.Windows.Forms.GroupBox CustomInertiaGroupBox;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button PhysicalButton;
        private System.Windows.Forms.CheckBox selfCollideCheckbox;
        private System.Windows.Forms.TextBox LinearDampingTextbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox AngularDampingTextbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button VisualButton;
        private System.Windows.Forms.Button CollisionButton;
    }
}
