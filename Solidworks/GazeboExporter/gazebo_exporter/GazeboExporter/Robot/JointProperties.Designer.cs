namespace GazeboExporter.UI
{
    partial class JointProperties
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
            this.MimicGroupBox = new System.Windows.Forms.GroupBox();
            this.MimicOffsetLabel = new System.Windows.Forms.Label();
            this.MimicMultiplierTextbox = new System.Windows.Forms.TextBox();
            this.MimicMultiplierLabel = new System.Windows.Forms.Label();
            this.MimicJointCombobox = new System.Windows.Forms.ComboBox();
            this.MimicOffsetTextbox = new System.Windows.Forms.TextBox();
            this.MimicJointLabel = new System.Windows.Forms.Label();
            this.MimicCheckbox = new System.Windows.Forms.CheckBox();
            this.MotionGroupBox = new System.Windows.Forms.GroupBox();
            this.JointPose = new System.Windows.Forms.Button();
            this.MotionUpperTextBox = new System.Windows.Forms.TextBox();
            this.MotionLowerTextBox = new System.Windows.Forms.TextBox();
            this.MotionLowerLabel = new System.Windows.Forms.Label();
            this.MotionUpperLabel = new System.Windows.Forms.Label();
            this.MotionCheckbox = new System.Windows.Forms.CheckBox();
            this.PhysicalGroupBox = new System.Windows.Forms.GroupBox();
            this.EffortTextBox = new System.Windows.Forms.TextBox();
            this.VelocityTextBox = new System.Windows.Forms.TextBox();
            this.VelocityLabel = new System.Windows.Forms.Label();
            this.EffortLabel = new System.Windows.Forms.Label();
            this.NameLabel = new System.Windows.Forms.Label();
            this.NameLabel2 = new System.Windows.Forms.Label();
            this.JointGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.MimicGroupBox.SuspendLayout();
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
            this.DampingTextBox.Location = new System.Drawing.Point(67, 17);
            this.DampingTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.DampingTextBox.Name = "DampingTextBox";
            this.DampingTextBox.Size = new System.Drawing.Size(56, 20);
            this.DampingTextBox.TabIndex = 2;
            this.DampingTextBox.TextChanged += new System.EventHandler(this.DampingTextBox_TextChanged);
            // 
            // FrictionTextBox
            // 
            this.FrictionTextBox.Location = new System.Drawing.Point(67, 41);
            this.FrictionTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.FrictionTextBox.Name = "FrictionTextBox";
            this.FrictionTextBox.Size = new System.Drawing.Size(56, 20);
            this.FrictionTextBox.TabIndex = 3;
            this.FrictionTextBox.TextChanged += new System.EventHandler(this.FrictionTextBox_TextChanged);
            // 
            // DampingLabel
            // 
            this.DampingLabel.AutoSize = true;
            this.DampingLabel.Location = new System.Drawing.Point(7, 20);
            this.DampingLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.DampingLabel.Name = "DampingLabel";
            this.DampingLabel.Size = new System.Drawing.Size(52, 13);
            this.DampingLabel.TabIndex = 4;
            this.DampingLabel.Text = "Damping:";
            // 
            // FrictionLabel
            // 
            this.FrictionLabel.AutoSize = true;
            this.FrictionLabel.Location = new System.Drawing.Point(17, 44);
            this.FrictionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.FrictionLabel.Name = "FrictionLabel";
            this.FrictionLabel.Size = new System.Drawing.Size(44, 13);
            this.FrictionLabel.TabIndex = 5;
            this.FrictionLabel.Text = "Friction:";
            // 
            // MimicGroupBox
            // 
            this.MimicGroupBox.Controls.Add(this.MimicOffsetLabel);
            this.MimicGroupBox.Controls.Add(this.MimicMultiplierTextbox);
            this.MimicGroupBox.Controls.Add(this.MimicMultiplierLabel);
            this.MimicGroupBox.Controls.Add(this.MimicJointCombobox);
            this.MimicGroupBox.Controls.Add(this.MimicOffsetTextbox);
            this.MimicGroupBox.Controls.Add(this.MimicJointLabel);
            this.MimicGroupBox.Location = new System.Drawing.Point(1, 196);
            this.MimicGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.MimicGroupBox.Name = "MimicGroupBox";
            this.MimicGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.MimicGroupBox.Size = new System.Drawing.Size(221, 89);
            this.MimicGroupBox.TabIndex = 6;
            this.MimicGroupBox.TabStop = false;
            this.MimicGroupBox.Text = "Mimic Properties";
            // 
            // MimicOffsetLabel
            // 
            this.MimicOffsetLabel.AutoSize = true;
            this.MimicOffsetLabel.Location = new System.Drawing.Point(23, 68);
            this.MimicOffsetLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MimicOffsetLabel.Name = "MimicOffsetLabel";
            this.MimicOffsetLabel.Size = new System.Drawing.Size(38, 13);
            this.MimicOffsetLabel.TabIndex = 12;
            this.MimicOffsetLabel.Text = "Offset:";
            // 
            // MimicMultiplierTextbox
            // 
            this.MimicMultiplierTextbox.Location = new System.Drawing.Point(65, 41);
            this.MimicMultiplierTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.MimicMultiplierTextbox.Name = "MimicMultiplierTextbox";
            this.MimicMultiplierTextbox.Size = new System.Drawing.Size(143, 20);
            this.MimicMultiplierTextbox.TabIndex = 9;
            this.MimicMultiplierTextbox.TextChanged += new System.EventHandler(this.MimicMultiplierTextbox_TextChanged);
            // 
            // MimicMultiplierLabel
            // 
            this.MimicMultiplierLabel.AutoSize = true;
            this.MimicMultiplierLabel.Location = new System.Drawing.Point(10, 44);
            this.MimicMultiplierLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MimicMultiplierLabel.Name = "MimicMultiplierLabel";
            this.MimicMultiplierLabel.Size = new System.Drawing.Size(51, 13);
            this.MimicMultiplierLabel.TabIndex = 11;
            this.MimicMultiplierLabel.Text = "Multiplier:";
            // 
            // MimicJointCombobox
            // 
            this.MimicJointCombobox.FormattingEnabled = true;
            this.MimicJointCombobox.Location = new System.Drawing.Point(65, 15);
            this.MimicJointCombobox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MimicJointCombobox.Name = "MimicJointCombobox";
            this.MimicJointCombobox.Size = new System.Drawing.Size(143, 21);
            this.MimicJointCombobox.TabIndex = 7;
            this.MimicJointCombobox.SelectedIndexChanged += new System.EventHandler(this.MimicJointCombobox_SelectedIndexChanged);
            // 
            // MimicOffsetTextbox
            // 
            this.MimicOffsetTextbox.Location = new System.Drawing.Point(65, 65);
            this.MimicOffsetTextbox.Margin = new System.Windows.Forms.Padding(2);
            this.MimicOffsetTextbox.Name = "MimicOffsetTextbox";
            this.MimicOffsetTextbox.Size = new System.Drawing.Size(143, 20);
            this.MimicOffsetTextbox.TabIndex = 10;
            this.MimicOffsetTextbox.TextChanged += new System.EventHandler(this.MimicOffsetTextbox_TextChanged);
            // 
            // MimicJointLabel
            // 
            this.MimicJointLabel.AutoSize = true;
            this.MimicJointLabel.Location = new System.Drawing.Point(30, 18);
            this.MimicJointLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MimicJointLabel.Name = "MimicJointLabel";
            this.MimicJointLabel.Size = new System.Drawing.Size(32, 13);
            this.MimicJointLabel.TabIndex = 8;
            this.MimicJointLabel.Text = "Joint:";
            // 
            // MimicCheckbox
            // 
            this.MimicCheckbox.AutoSize = true;
            this.MimicCheckbox.Location = new System.Drawing.Point(28, 175);
            this.MimicCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.MimicCheckbox.Name = "MimicCheckbox";
            this.MimicCheckbox.Size = new System.Drawing.Size(107, 17);
            this.MimicCheckbox.TabIndex = 0;
            this.MimicCheckbox.Text = "Mimic Other Joint";
            this.MimicCheckbox.UseVisualStyleBackColor = true;
            this.MimicCheckbox.CheckedChanged += new System.EventHandler(this.MimicCheckbox_CheckedChanged);
            // 
            // MotionGroupBox
            // 
            this.MotionGroupBox.Controls.Add(this.JointPose);
            this.MotionGroupBox.Controls.Add(this.MotionUpperTextBox);
            this.MotionGroupBox.Controls.Add(this.MotionLowerTextBox);
            this.MotionGroupBox.Controls.Add(this.MotionLowerLabel);
            this.MotionGroupBox.Controls.Add(this.MotionUpperLabel);
            this.MotionGroupBox.Location = new System.Drawing.Point(2, 321);
            this.MotionGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.MotionGroupBox.Name = "MotionGroupBox";
            this.MotionGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.MotionGroupBox.Size = new System.Drawing.Size(221, 85);
            this.MotionGroupBox.TabIndex = 7;
            this.MotionGroupBox.TabStop = false;
            this.MotionGroupBox.Text = "Motion Limits Properties";
            // 
            // JointPose
            // 
            this.JointPose.Location = new System.Drawing.Point(11, 17);
            this.JointPose.Margin = new System.Windows.Forms.Padding(2);
            this.JointPose.Name = "JointPose";
            this.JointPose.Size = new System.Drawing.Size(197, 19);
            this.JointPose.TabIndex = 13;
            this.JointPose.Text = "Joint Pose";
            this.JointPose.UseVisualStyleBackColor = true;
            this.JointPose.Click += new System.EventHandler(this.JointPose_Click);
            // 
            // MotionUpperTextBox
            // 
            this.MotionUpperTextBox.Location = new System.Drawing.Point(65, 40);
            this.MotionUpperTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.MotionUpperTextBox.Name = "MotionUpperTextBox";
            this.MotionUpperTextBox.Size = new System.Drawing.Size(56, 20);
            this.MotionUpperTextBox.TabIndex = 12;
            this.MotionUpperTextBox.TextChanged += new System.EventHandler(this.MotionUpperTextBox_TextChanged);
            // 
            // MotionLowerTextBox
            // 
            this.MotionLowerTextBox.Location = new System.Drawing.Point(65, 60);
            this.MotionLowerTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.MotionLowerTextBox.Name = "MotionLowerTextBox";
            this.MotionLowerTextBox.Size = new System.Drawing.Size(56, 20);
            this.MotionLowerTextBox.TabIndex = 9;
            this.MotionLowerTextBox.TextChanged += new System.EventHandler(this.MotionLowerTextBox_TextChanged);
            // 
            // MotionLowerLabel
            // 
            this.MotionLowerLabel.AutoSize = true;
            this.MotionLowerLabel.Location = new System.Drawing.Point(11, 63);
            this.MotionLowerLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MotionLowerLabel.Name = "MotionLowerLabel";
            this.MotionLowerLabel.Size = new System.Drawing.Size(39, 13);
            this.MotionLowerLabel.TabIndex = 11;
            this.MotionLowerLabel.Text = "Lower:";
            // 
            // MotionUpperLabel
            // 
            this.MotionUpperLabel.AutoSize = true;
            this.MotionUpperLabel.Location = new System.Drawing.Point(11, 43);
            this.MotionUpperLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.MotionUpperLabel.Name = "MotionUpperLabel";
            this.MotionUpperLabel.Size = new System.Drawing.Size(39, 13);
            this.MotionUpperLabel.TabIndex = 8;
            this.MotionUpperLabel.Text = "Upper:";
            // 
            // MotionCheckbox
            // 
            this.MotionCheckbox.AutoSize = true;
            this.MotionCheckbox.Location = new System.Drawing.Point(28, 300);
            this.MotionCheckbox.Margin = new System.Windows.Forms.Padding(2);
            this.MotionCheckbox.Name = "MotionCheckbox";
            this.MotionCheckbox.Size = new System.Drawing.Size(145, 17);
            this.MotionCheckbox.TabIndex = 17;
            this.MotionCheckbox.Text = "Set motion limits manually";
            this.MotionCheckbox.UseVisualStyleBackColor = true;
            this.MotionCheckbox.CheckedChanged += new System.EventHandler(this.MotionCheckbox_CheckedChanged);
            // 
            // PhysicalGroupBox
            // 
            this.PhysicalGroupBox.Controls.Add(this.EffortTextBox);
            this.PhysicalGroupBox.Controls.Add(this.VelocityTextBox);
            this.PhysicalGroupBox.Controls.Add(this.VelocityLabel);
            this.PhysicalGroupBox.Controls.Add(this.EffortLabel);
            this.PhysicalGroupBox.Location = new System.Drawing.Point(2, 419);
            this.PhysicalGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.PhysicalGroupBox.Name = "PhysicalGroupBox";
            this.PhysicalGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.PhysicalGroupBox.Size = new System.Drawing.Size(221, 66);
            this.PhysicalGroupBox.TabIndex = 18;
            this.PhysicalGroupBox.TabStop = false;
            this.PhysicalGroupBox.Text = "Physical Limit Properties";
            // 
            // EffortTextBox
            // 
            this.EffortTextBox.Location = new System.Drawing.Point(65, 16);
            this.EffortTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.EffortTextBox.Name = "EffortTextBox";
            this.EffortTextBox.Size = new System.Drawing.Size(56, 20);
            this.EffortTextBox.TabIndex = 12;
            this.EffortTextBox.TextChanged += new System.EventHandler(this.EffortTextBox_TextChanged);
            // 
            // VelocityTextBox
            // 
            this.VelocityTextBox.Location = new System.Drawing.Point(65, 40);
            this.VelocityTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.VelocityTextBox.Name = "VelocityTextBox";
            this.VelocityTextBox.Size = new System.Drawing.Size(56, 20);
            this.VelocityTextBox.TabIndex = 9;
            this.VelocityTextBox.TextChanged += new System.EventHandler(this.VelocityTextBox_TextChanged);
            // 
            // VelocityLabel
            // 
            this.VelocityLabel.AutoSize = true;
            this.VelocityLabel.Location = new System.Drawing.Point(11, 39);
            this.VelocityLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.VelocityLabel.Name = "VelocityLabel";
            this.VelocityLabel.Size = new System.Drawing.Size(47, 13);
            this.VelocityLabel.TabIndex = 11;
            this.VelocityLabel.Text = "Velocity:";
            // 
            // EffortLabel
            // 
            this.EffortLabel.AutoSize = true;
            this.EffortLabel.Location = new System.Drawing.Point(15, 19);
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
            this.JointGroupBox.Controls.Add(this.DampingLabel);
            this.JointGroupBox.Controls.Add(this.DampingTextBox);
            this.JointGroupBox.Controls.Add(this.FrictionTextBox);
            this.JointGroupBox.Controls.Add(this.FrictionLabel);
            this.JointGroupBox.Location = new System.Drawing.Point(2, 103);
            this.JointGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.JointGroupBox.Name = "JointGroupBox";
            this.JointGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.JointGroupBox.Size = new System.Drawing.Size(221, 68);
            this.JointGroupBox.TabIndex = 21;
            this.JointGroupBox.TabStop = false;
            this.JointGroupBox.Text = "Joint Properties";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(13, 68);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(197, 19);
            this.button1.TabIndex = 22;
            this.button1.Text = "Joint Pose2";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // JointProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.JointGroupBox);
            this.Controls.Add(this.NameLabel2);
            this.Controls.Add(this.NameLabel);
            this.Controls.Add(this.PhysicalGroupBox);
            this.Controls.Add(this.MotionCheckbox);
            this.Controls.Add(this.MotionGroupBox);
            this.Controls.Add(this.MimicCheckbox);
            this.Controls.Add(this.MimicGroupBox);
            this.Controls.Add(this.jointTypeLabel);
            this.Controls.Add(this.jointType);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "JointProperties";
            this.Size = new System.Drawing.Size(225, 487);
            this.MimicGroupBox.ResumeLayout(false);
            this.MimicGroupBox.PerformLayout();
            this.MotionGroupBox.ResumeLayout(false);
            this.MotionGroupBox.PerformLayout();
            this.PhysicalGroupBox.ResumeLayout(false);
            this.PhysicalGroupBox.PerformLayout();
            this.JointGroupBox.ResumeLayout(false);
            this.JointGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox jointType;
        private System.Windows.Forms.Label jointTypeLabel;
        private System.Windows.Forms.TextBox DampingTextBox;
        private System.Windows.Forms.TextBox FrictionTextBox;
        private System.Windows.Forms.Label DampingLabel;
        private System.Windows.Forms.Label FrictionLabel;

        private System.Windows.Forms.CheckBox MimicCheckbox;
        private System.Windows.Forms.GroupBox MimicGroupBox;
        private System.Windows.Forms.Label MimicJointLabel;
        private System.Windows.Forms.ComboBox MimicJointCombobox;
        private System.Windows.Forms.Label MimicMultiplierLabel;
        private System.Windows.Forms.TextBox MimicMultiplierTextbox;
        private System.Windows.Forms.Label MimicOffsetLabel;
        private System.Windows.Forms.TextBox MimicOffsetTextbox;

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
        private System.Windows.Forms.Button button1;
    }
}
