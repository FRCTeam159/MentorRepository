namespace GazeboExporter.UI
{
    partial class JointProperties2
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
            this.jointType = new System.Windows.Forms.ComboBox();
            this.jointTypeLabel = new System.Windows.Forms.Label();
            this.DampingTextBox = new System.Windows.Forms.TextBox();
            this.FrictionTextBox = new System.Windows.Forms.TextBox();
            this.DampingLabel = new System.Windows.Forms.Label();
            this.FrictionLabel = new System.Windows.Forms.Label();
            this.ExtraGroupBox = new System.Windows.Forms.GroupBox();
            this.ThreadPitchLabel = new System.Windows.Forms.Label();
            this.GearboxRatioTextbox = new System.Windows.Forms.TextBox();
            this.GearboxRatioLabel = new System.Windows.Forms.Label();
            this.ThreadPitchTextbox = new System.Windows.Forms.TextBox();
            this.MotionGroupBox = new System.Windows.Forms.GroupBox();
            this.LimitsPose = new System.Windows.Forms.Button();
            this.ContinuousCheckbox = new System.Windows.Forms.CheckBox();
            this.MotionUpperTextBox = new System.Windows.Forms.TextBox();
            this.MotionLowerTextBox = new System.Windows.Forms.TextBox();
            this.MotionLowerLabel = new System.Windows.Forms.Label();
            this.MotionUpperLabel = new System.Windows.Forms.Label();
            this.MotionCheckbox = new System.Windows.Forms.CheckBox();
            this.JointPose = new System.Windows.Forms.Button();
            this.PhysicalGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.Velocity2TextBox = new System.Windows.Forms.TextBox();
            this.VelocityTextBox = new System.Windows.Forms.TextBox();
            this.VelocityLabel = new System.Windows.Forms.Label();
            this.Effort2TextBox = new System.Windows.Forms.TextBox();
            this.EffortTextBox = new System.Windows.Forms.TextBox();
            this.EffortLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameLabel2 = new System.Windows.Forms.Label();
            this.JointGroupBox = new System.Windows.Forms.GroupBox();
            this.Damping2TextBox = new System.Windows.Forms.TextBox();
            this.Friction2TextBox = new System.Windows.Forms.TextBox();
            this.Axis2Label = new System.Windows.Forms.Label();
            this.Axis1Label = new System.Windows.Forms.Label();
            this.MimicJointLabel = new System.Windows.Forms.Label();
            this.MimicJointCombobox = new System.Windows.Forms.ComboBox();
            this.ExtraGroupBox.SuspendLayout();
            this.MotionGroupBox.SuspendLayout();
            this.PhysicalGroupBox.SuspendLayout();
            this.JointGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // jointType
            // 
            this.jointType.FormattingEnabled = true;
            this.jointType.Location = new System.Drawing.Point(69, 42);
            this.jointType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.jointType.Name = "jointType";
            this.jointType.Size = new System.Drawing.Size(143, 21);
            this.jointType.TabIndex = 0;
            this.jointType.SelectedIndexChanged += new System.EventHandler(this.jointType_SelectedIndexChanged);
            // 
            // jointTypeLabel
            // 
            this.jointTypeLabel.AutoSize = true;
            this.jointTypeLabel.Location = new System.Drawing.Point(28, 45);
            this.jointTypeLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.jointTypeLabel.Name = "jointTypeLabel";
            this.jointTypeLabel.Size = new System.Drawing.Size(34, 13);
            this.jointTypeLabel.TabIndex = 1;
            this.jointTypeLabel.Text = "Type:";
            // 
            // DampingTextBox
            // 
            this.DampingTextBox.Location = new System.Drawing.Point(57, 29);
            this.DampingTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.DampingTextBox.Name = "DampingTextBox";
            this.DampingTextBox.Size = new System.Drawing.Size(69, 20);
            this.DampingTextBox.TabIndex = 2;
            this.DampingTextBox.TextChanged += new System.EventHandler(this.DampingTextBox_TextChanged);
            // 
            // FrictionTextBox
            // 
            this.FrictionTextBox.Location = new System.Drawing.Point(57, 53);
            this.FrictionTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.FrictionTextBox.Name = "FrictionTextBox";
            this.FrictionTextBox.Size = new System.Drawing.Size(69, 20);
            this.FrictionTextBox.TabIndex = 3;
            this.FrictionTextBox.TextChanged += new System.EventHandler(this.FrictionTextBox_TextChanged);
            // 
            // DampingLabel
            // 
            this.DampingLabel.AutoSize = true;
            this.DampingLabel.Location = new System.Drawing.Point(3, 32);
            this.DampingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DampingLabel.Name = "DampingLabel";
            this.DampingLabel.Size = new System.Drawing.Size(52, 13);
            this.DampingLabel.TabIndex = 4;
            this.DampingLabel.Text = "Damping:";
            // 
            // FrictionLabel
            // 
            this.FrictionLabel.AutoSize = true;
            this.FrictionLabel.Location = new System.Drawing.Point(11, 56);
            this.FrictionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FrictionLabel.Name = "FrictionLabel";
            this.FrictionLabel.Size = new System.Drawing.Size(44, 13);
            this.FrictionLabel.TabIndex = 5;
            this.FrictionLabel.Text = "Friction:";
            // 
            // ExtraGroupBox
            // 
            this.ExtraGroupBox.Controls.Add(this.MimicJointLabel);
            this.ExtraGroupBox.Controls.Add(this.MimicJointCombobox);
            this.ExtraGroupBox.Controls.Add(this.ThreadPitchLabel);
            this.ExtraGroupBox.Controls.Add(this.GearboxRatioTextbox);
            this.ExtraGroupBox.Controls.Add(this.GearboxRatioLabel);
            this.ExtraGroupBox.Controls.Add(this.ThreadPitchTextbox);
            this.ExtraGroupBox.Location = new System.Drawing.Point(2, 322);
            this.ExtraGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExtraGroupBox.Name = "ExtraGroupBox";
            this.ExtraGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ExtraGroupBox.Size = new System.Drawing.Size(221, 91);
            this.ExtraGroupBox.TabIndex = 6;
            this.ExtraGroupBox.TabStop = false;
            this.ExtraGroupBox.Text = "Special Properties";
            // 
            // ThreadPitchLabel
            // 
            this.ThreadPitchLabel.AutoSize = true;
            this.ThreadPitchLabel.Location = new System.Drawing.Point(6, 69);
            this.ThreadPitchLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ThreadPitchLabel.Name = "ThreadPitchLabel";
            this.ThreadPitchLabel.Size = new System.Drawing.Size(74, 13);
            this.ThreadPitchLabel.TabIndex = 12;
            this.ThreadPitchLabel.Text = "Thread Pitch: ";
            // 
            // GearboxRatioTextbox
            // 
            this.GearboxRatioTextbox.Location = new System.Drawing.Point(81, 42);
            this.GearboxRatioTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.GearboxRatioTextbox.Name = "GearboxRatioTextbox";
            this.GearboxRatioTextbox.Size = new System.Drawing.Size(127, 20);
            this.GearboxRatioTextbox.TabIndex = 9;
            this.GearboxRatioTextbox.TextChanged += new System.EventHandler(this.GearboxRatioTextbox_TextChanged);
            // 
            // GearboxRatioLabel
            // 
            this.GearboxRatioLabel.AutoSize = true;
            this.GearboxRatioLabel.Location = new System.Drawing.Point(2, 45);
            this.GearboxRatioLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.GearboxRatioLabel.Name = "GearboxRatioLabel";
            this.GearboxRatioLabel.Size = new System.Drawing.Size(78, 13);
            this.GearboxRatioLabel.TabIndex = 11;
            this.GearboxRatioLabel.Text = "Gearbox Ratio:";
            // 
            // ThreadPitchTextbox
            // 
            this.ThreadPitchTextbox.Location = new System.Drawing.Point(81, 66);
            this.ThreadPitchTextbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ThreadPitchTextbox.Name = "ThreadPitchTextbox";
            this.ThreadPitchTextbox.Size = new System.Drawing.Size(127, 20);
            this.ThreadPitchTextbox.TabIndex = 10;
            this.ThreadPitchTextbox.TextChanged += new System.EventHandler(this.ThreadPitchTextbox_TextChanged);
            // 
            // MotionGroupBox
            // 
            this.MotionGroupBox.Controls.Add(this.LimitsPose);
            this.MotionGroupBox.Controls.Add(this.ContinuousCheckbox);
            this.MotionGroupBox.Controls.Add(this.MotionUpperTextBox);
            this.MotionGroupBox.Controls.Add(this.MotionLowerTextBox);
            this.MotionGroupBox.Controls.Add(this.MotionLowerLabel);
            this.MotionGroupBox.Controls.Add(this.MotionUpperLabel);
            this.MotionGroupBox.Controls.Add(this.MotionCheckbox);
            this.MotionGroupBox.Location = new System.Drawing.Point(2, 211);
            this.MotionGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MotionGroupBox.Name = "MotionGroupBox";
            this.MotionGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MotionGroupBox.Size = new System.Drawing.Size(221, 107);
            this.MotionGroupBox.TabIndex = 7;
            this.MotionGroupBox.TabStop = false;
            this.MotionGroupBox.Text = "Axis Motion Limits Properties";
            // 
            // LimitsPose
            // 
            this.LimitsPose.Location = new System.Drawing.Point(13, 62);
            this.LimitsPose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.LimitsPose.Name = "LimitsPose";
            this.LimitsPose.Size = new System.Drawing.Size(196, 19);
            this.LimitsPose.TabIndex = 22;
            this.LimitsPose.Text = "Select Limits Visually";
            this.LimitsPose.UseVisualStyleBackColor = true;
            this.LimitsPose.Click += new System.EventHandler(this.LimitsPose_Click);
            // 
            // ContinuousCheckbox
            // 
            this.ContinuousCheckbox.AutoSize = true;
            this.ContinuousCheckbox.Location = new System.Drawing.Point(8, 86);
            this.ContinuousCheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ContinuousCheckbox.Name = "ContinuousCheckbox";
            this.ContinuousCheckbox.Size = new System.Drawing.Size(101, 17);
            this.ContinuousCheckbox.TabIndex = 18;
            this.ContinuousCheckbox.Text = "Continuous joint";
            this.ContinuousCheckbox.UseVisualStyleBackColor = true;
            this.ContinuousCheckbox.CheckedChanged += new System.EventHandler(this.ContinuousCheckbox_CheckedChanged);
            // 
            // MotionUpperTextBox
            // 
            this.MotionUpperTextBox.Location = new System.Drawing.Point(57, 17);
            this.MotionUpperTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MotionUpperTextBox.Name = "MotionUpperTextBox";
            this.MotionUpperTextBox.Size = new System.Drawing.Size(151, 20);
            this.MotionUpperTextBox.TabIndex = 12;
            this.MotionUpperTextBox.LostFocus += new System.EventHandler(this.MotionUpperTextBox_LostFocus);
            // 
            // MotionLowerTextBox
            // 
            this.MotionLowerTextBox.Location = new System.Drawing.Point(57, 38);
            this.MotionLowerTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MotionLowerTextBox.Name = "MotionLowerTextBox";
            this.MotionLowerTextBox.Size = new System.Drawing.Size(151, 20);
            this.MotionLowerTextBox.TabIndex = 9;
            this.MotionLowerTextBox.LostFocus += new System.EventHandler(this.MotionLowerTextBox_LostFocus);
            // 
            // MotionLowerLabel
            // 
            this.MotionLowerLabel.AutoSize = true;
            this.MotionLowerLabel.Location = new System.Drawing.Point(16, 40);
            this.MotionLowerLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MotionLowerLabel.Name = "MotionLowerLabel";
            this.MotionLowerLabel.Size = new System.Drawing.Size(39, 13);
            this.MotionLowerLabel.TabIndex = 11;
            this.MotionLowerLabel.Text = "Lower:";
            // 
            // MotionUpperLabel
            // 
            this.MotionUpperLabel.AutoSize = true;
            this.MotionUpperLabel.Location = new System.Drawing.Point(15, 19);
            this.MotionUpperLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MotionUpperLabel.Name = "MotionUpperLabel";
            this.MotionUpperLabel.Size = new System.Drawing.Size(39, 13);
            this.MotionUpperLabel.TabIndex = 8;
            this.MotionUpperLabel.Text = "Upper:";
            // 
            // MotionCheckbox
            // 
            this.MotionCheckbox.AutoSize = true;
            this.MotionCheckbox.Location = new System.Drawing.Point(8, 86);
            this.MotionCheckbox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MotionCheckbox.Name = "MotionCheckbox";
            this.MotionCheckbox.Size = new System.Drawing.Size(148, 17);
            this.MotionCheckbox.TabIndex = 17;
            this.MotionCheckbox.Text = "Input limit values manually";
            this.MotionCheckbox.UseVisualStyleBackColor = true;
            this.MotionCheckbox.CheckedChanged += new System.EventHandler(this.MotionCheckbox_CheckedChanged);
            // 
            // JointPose
            // 
            this.JointPose.Location = new System.Drawing.Point(15, 68);
            this.JointPose.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.JointPose.Name = "JointPose";
            this.JointPose.Size = new System.Drawing.Size(197, 19);
            this.JointPose.TabIndex = 13;
            this.JointPose.Text = "Joint Pose";
            this.JointPose.UseVisualStyleBackColor = true;
            this.JointPose.Click += new System.EventHandler(this.JointPose_Click);
            // 
            // PhysicalGroupBox
            // 
            this.PhysicalGroupBox.Controls.Add(this.label1);
            this.PhysicalGroupBox.Controls.Add(this.label2);
            this.PhysicalGroupBox.Controls.Add(this.Velocity2TextBox);
            this.PhysicalGroupBox.Controls.Add(this.VelocityTextBox);
            this.PhysicalGroupBox.Controls.Add(this.VelocityLabel);
            this.PhysicalGroupBox.Enabled = false;
            this.PhysicalGroupBox.Location = new System.Drawing.Point(2, 430);
            this.PhysicalGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PhysicalGroupBox.Name = "PhysicalGroupBox";
            this.PhysicalGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PhysicalGroupBox.Size = new System.Drawing.Size(221, 55);
            this.PhysicalGroupBox.TabIndex = 18;
            this.PhysicalGroupBox.TabStop = false;
            this.PhysicalGroupBox.Text = "Physical Limit Properties";
            this.PhysicalGroupBox.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(151, 16);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Axis 2:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Axis 1:";
            // 
            // Velocity2TextBox
            // 
            this.Velocity2TextBox.Location = new System.Drawing.Point(138, 31);
            this.Velocity2TextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Velocity2TextBox.Name = "Velocity2TextBox";
            this.Velocity2TextBox.Size = new System.Drawing.Size(77, 20);
            this.Velocity2TextBox.TabIndex = 13;
            // 
            // VelocityTextBox
            // 
            this.VelocityTextBox.Location = new System.Drawing.Point(57, 30);
            this.VelocityTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.VelocityTextBox.Name = "VelocityTextBox";
            this.VelocityTextBox.Size = new System.Drawing.Size(77, 20);
            this.VelocityTextBox.TabIndex = 9;
            this.VelocityTextBox.TextChanged += new System.EventHandler(this.VelocityTextBox_TextChanged);
            // 
            // VelocityLabel
            // 
            this.VelocityLabel.AutoSize = true;
            this.VelocityLabel.Location = new System.Drawing.Point(8, 31);
            this.VelocityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.VelocityLabel.Name = "VelocityLabel";
            this.VelocityLabel.Size = new System.Drawing.Size(47, 13);
            this.VelocityLabel.TabIndex = 11;
            this.VelocityLabel.Text = "Velocity:";
            // 
            // Effort2TextBox
            // 
            this.Effort2TextBox.Location = new System.Drawing.Point(139, 76);
            this.Effort2TextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Effort2TextBox.Name = "Effort2TextBox";
            this.Effort2TextBox.Size = new System.Drawing.Size(69, 20);
            this.Effort2TextBox.TabIndex = 14;
            this.Effort2TextBox.TextChanged += new System.EventHandler(this.Effort2TextBox_TextChanged);
            // 
            // EffortTextBox
            // 
            this.EffortTextBox.Location = new System.Drawing.Point(57, 76);
            this.EffortTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.EffortTextBox.Name = "EffortTextBox";
            this.EffortTextBox.Size = new System.Drawing.Size(69, 20);
            this.EffortTextBox.TabIndex = 12;
            this.EffortTextBox.TextChanged += new System.EventHandler(this.EffortTextBox_TextChanged);
            // 
            // EffortLabel
            // 
            this.EffortLabel.AutoSize = true;
            this.EffortLabel.Location = new System.Drawing.Point(19, 79);
            this.EffortLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.EffortLabel.Name = "EffortLabel";
            this.EffortLabel.Size = new System.Drawing.Size(35, 13);
            this.EffortLabel.TabIndex = 8;
            this.EffortLabel.Text = "Effort:";
            // 
            // NameLabel
            // 
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(25, 19);
            this.NameLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(38, 13);
            this.NameLabel.TabIndex = 19;
            this.NameLabel.Text = "Name:";
            // 
            // NameLabel2
            // 
            this.NameLabel2.AutoSize = true;
            this.NameLabel2.Location = new System.Drawing.Point(67, 19);
            this.NameLabel2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.NameLabel2.Name = "NameLabel2";
            this.NameLabel2.Size = new System.Drawing.Size(68, 13);
            this.NameLabel2.TabIndex = 20;
            this.NameLabel2.Text = "(parent-child)";
            // 
            // JointGroupBox
            // 
            this.JointGroupBox.Controls.Add(this.Damping2TextBox);
            this.JointGroupBox.Controls.Add(this.Friction2TextBox);
            this.JointGroupBox.Controls.Add(this.Effort2TextBox);
            this.JointGroupBox.Controls.Add(this.Axis2Label);
            this.JointGroupBox.Controls.Add(this.Axis1Label);
            this.JointGroupBox.Controls.Add(this.EffortTextBox);
            this.JointGroupBox.Controls.Add(this.DampingLabel);
            this.JointGroupBox.Controls.Add(this.DampingTextBox);
            this.JointGroupBox.Controls.Add(this.FrictionTextBox);
            this.JointGroupBox.Controls.Add(this.EffortLabel);
            this.JointGroupBox.Controls.Add(this.FrictionLabel);
            this.JointGroupBox.Location = new System.Drawing.Point(2, 103);
            this.JointGroupBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.JointGroupBox.Name = "JointGroupBox";
            this.JointGroupBox.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.JointGroupBox.Size = new System.Drawing.Size(221, 104);
            this.JointGroupBox.TabIndex = 21;
            this.JointGroupBox.TabStop = false;
            this.JointGroupBox.Text = "Joint Properties";
            // 
            // Damping2TextBox
            // 
            this.Damping2TextBox.Location = new System.Drawing.Point(139, 29);
            this.Damping2TextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Damping2TextBox.Name = "Damping2TextBox";
            this.Damping2TextBox.Size = new System.Drawing.Size(69, 20);
            this.Damping2TextBox.TabIndex = 22;
            this.Damping2TextBox.TextChanged += new System.EventHandler(this.Damping2TextBox_TextChanged);
            // 
            // Friction2TextBox
            // 
            this.Friction2TextBox.Location = new System.Drawing.Point(139, 53);
            this.Friction2TextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Friction2TextBox.Name = "Friction2TextBox";
            this.Friction2TextBox.Size = new System.Drawing.Size(69, 20);
            this.Friction2TextBox.TabIndex = 23;
            this.Friction2TextBox.TextChanged += new System.EventHandler(this.Friction2TextBox_TextChanged);
            // 
            // Axis2Label
            // 
            this.Axis2Label.AutoSize = true;
            this.Axis2Label.Location = new System.Drawing.Point(153, 13);
            this.Axis2Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Axis2Label.Name = "Axis2Label";
            this.Axis2Label.Size = new System.Drawing.Size(38, 13);
            this.Axis2Label.TabIndex = 21;
            this.Axis2Label.Text = "Axis 2:";
            // 
            // Axis1Label
            // 
            this.Axis1Label.AutoSize = true;
            this.Axis1Label.Location = new System.Drawing.Point(79, 13);
            this.Axis1Label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.Axis1Label.Name = "Axis1Label";
            this.Axis1Label.Size = new System.Drawing.Size(38, 13);
            this.Axis1Label.TabIndex = 20;
            this.Axis1Label.Text = "Axis 1:";
            // 
            // MimicJointLabel
            // 
            this.MimicJointLabel.AutoSize = true;
            this.MimicJointLabel.Location = new System.Drawing.Point(11, 19);
            this.MimicJointLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MimicJointLabel.Name = "MimicJointLabel";
            this.MimicJointLabel.Size = new System.Drawing.Size(62, 13);
            this.MimicJointLabel.TabIndex = 23;
            this.MimicJointLabel.Text = "Mimic Joint:";
            // 
            // MimicJointCombobox
            // 
            this.MimicJointCombobox.FormattingEnabled = true;
            this.MimicJointCombobox.Location = new System.Drawing.Point(81, 16);
            this.MimicJointCombobox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MimicJointCombobox.Name = "MimicJointCombobox";
            this.MimicJointCombobox.Size = new System.Drawing.Size(127, 21);
            this.MimicJointCombobox.TabIndex = 22;
            this.MimicJointCombobox.SelectedIndexChanged += new System.EventHandler(this.MimicJointCombobox_SelectedIndexChanged);
            // 
            // JointProperties2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.JointPose);
            this.Controls.Add(this.JointGroupBox);
            this.Controls.Add(this.NameLabel2);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.ExtraGroupBox);
            this.Controls.Add(this.PhysicalGroupBox);
            this.Controls.Add(this.MotionGroupBox);
            this.Controls.Add(this.jointTypeLabel);
            this.Controls.Add(this.jointType);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "JointProperties2";
            this.Size = new System.Drawing.Size(225, 487);
            this.ExtraGroupBox.ResumeLayout(false);
            this.ExtraGroupBox.PerformLayout();
            this.MotionGroupBox.ResumeLayout(false);
            this.MotionGroupBox.PerformLayout();
            this.PhysicalGroupBox.ResumeLayout(false);
            this.PhysicalGroupBox.PerformLayout();
            this.JointGroupBox.ResumeLayout(false);
            this.JointGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

            this.Load += new System.EventHandler(this.LoadToolTips);
        }

        #endregion

        private System.Windows.Forms.ComboBox jointType;
        private System.Windows.Forms.Label jointTypeLabel;
        private System.Windows.Forms.TextBox DampingTextBox;
        private System.Windows.Forms.TextBox FrictionTextBox;
        private System.Windows.Forms.Label DampingLabel;
        private System.Windows.Forms.Label FrictionLabel;
        private System.Windows.Forms.GroupBox ExtraGroupBox;
        private System.Windows.Forms.Label GearboxRatioLabel;
        private System.Windows.Forms.TextBox GearboxRatioTextbox;
        private System.Windows.Forms.Label ThreadPitchLabel;
        private System.Windows.Forms.TextBox ThreadPitchTextbox;

        public System.Windows.Forms.CheckBox MotionCheckbox;
        private System.Windows.Forms.GroupBox MotionGroupBox;
        private System.Windows.Forms.Button JointPose;
        private System.Windows.Forms.TextBox MotionUpperTextBox;
        private System.Windows.Forms.Label MotionUpperLabel;
        private System.Windows.Forms.TextBox MotionLowerTextBox;
        private System.Windows.Forms.Label MotionLowerLabel;

        private System.Windows.Forms.GroupBox PhysicalGroupBox;
        private System.Windows.Forms.TextBox EffortTextBox;
        private System.Windows.Forms.TextBox VelocityTextBox;
        private System.Windows.Forms.Label VelocityLabel;
        private System.Windows.Forms.Label EffortLabel;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label NameLabel2;
        private System.Windows.Forms.GroupBox JointGroupBox;
        private System.Windows.Forms.TextBox Effort2TextBox;
        private System.Windows.Forms.TextBox Velocity2TextBox;
        private System.Windows.Forms.TextBox Damping2TextBox;
        private System.Windows.Forms.TextBox Friction2TextBox;
        private System.Windows.Forms.Label Axis2Label;
        private System.Windows.Forms.Label Axis1Label;
        private System.Windows.Forms.Button LimitsPose;
        public System.Windows.Forms.CheckBox ContinuousCheckbox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label MimicJointLabel;
        private System.Windows.Forms.ComboBox MimicJointCombobox;
    }
}
