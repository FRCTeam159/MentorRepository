namespace GazeboExporter.Robot
{
    partial class JointSpecificProperties
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
            this.TopLayout = new System.Windows.Forms.TableLayoutPanel();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.NameLabel = new System.Windows.Forms.Label();
            this.JointTypeLabel = new System.Windows.Forms.Label();
            this.JointTypeCombobox = new System.Windows.Forms.ComboBox();
            this.JointNameLabel = new System.Windows.Forms.Label();
            this.JointPoseButton = new System.Windows.Forms.Button();
            this.TopLayout.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopLayout
            // 
            this.TopLayout.AutoScroll = true;
            this.TopLayout.AutoSize = true;
            this.TopLayout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TopLayout.ColumnCount = 1;
            this.TopLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TopLayout.Controls.Add(this.RemoveButton, 0, 2);
            this.TopLayout.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.TopLayout.Controls.Add(this.JointPoseButton, 0, 1);
            this.TopLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TopLayout.Location = new System.Drawing.Point(0, 0);
            this.TopLayout.Name = "TopLayout";
            this.TopLayout.RowCount = 4;
            this.TopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TopLayout.Size = new System.Drawing.Size(189, 136);
            this.TopLayout.TabIndex = 6;
            // 
            // RemoveButton
            // 
            this.RemoveButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RemoveButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoveButton.Location = new System.Drawing.Point(3, 99);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(183, 34);
            this.RemoveButton.TabIndex = 39;
            this.RemoveButton.Text = "Remove joint";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.NameLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.JointTypeLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.JointTypeCombobox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.JointNameLabel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(183, 54);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // NameLabel
            // 
            this.NameLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.NameLabel.AutoSize = true;
            this.NameLabel.Location = new System.Drawing.Point(33, 0);
            this.NameLabel.Name = "NameLabel";
            this.NameLabel.Size = new System.Drawing.Size(55, 20);
            this.NameLabel.TabIndex = 0;
            this.NameLabel.Text = "Name:";
            // 
            // JointTypeLabel
            // 
            this.JointTypeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.JointTypeLabel.AutoSize = true;
            this.JointTypeLabel.Location = new System.Drawing.Point(3, 27);
            this.JointTypeLabel.Name = "JointTypeLabel";
            this.JointTypeLabel.Size = new System.Drawing.Size(85, 20);
            this.JointTypeLabel.TabIndex = 1;
            this.JointTypeLabel.Text = "Joint Type:";
            // 
            // JointTypeCombobox
            // 
            this.JointTypeCombobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointTypeCombobox.FormattingEnabled = true;
            this.JointTypeCombobox.Location = new System.Drawing.Point(94, 23);
            this.JointTypeCombobox.Name = "JointTypeCombobox";
            this.JointTypeCombobox.Size = new System.Drawing.Size(86, 28);
            this.JointTypeCombobox.TabIndex = 2;
            this.JointTypeCombobox.SelectedIndexChanged += new System.EventHandler(this.JointTypeCombobox_SelectedIndexChanged);
            // 
            // JointNameLabel
            // 
            this.JointNameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.JointNameLabel.AutoSize = true;
            this.JointNameLabel.Location = new System.Drawing.Point(126, 0);
            this.JointNameLabel.Name = "JointNameLabel";
            this.JointNameLabel.Size = new System.Drawing.Size(21, 20);
            this.JointNameLabel.TabIndex = 3;
            this.JointNameLabel.Text = "\"\"";
            // 
            // JointPoseButton
            // 
            this.JointPoseButton.AutoSize = true;
            this.JointPoseButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.JointPoseButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointPoseButton.Location = new System.Drawing.Point(3, 63);
            this.JointPoseButton.MinimumSize = new System.Drawing.Size(0, 29);
            this.JointPoseButton.Name = "JointPoseButton";
            this.JointPoseButton.Size = new System.Drawing.Size(183, 30);
            this.JointPoseButton.TabIndex = 4;
            this.JointPoseButton.Text = "Select Joint Pose";
            this.JointPoseButton.UseVisualStyleBackColor = true;
            this.JointPoseButton.Click += new System.EventHandler(this.JointPoseButton_Click);
            // 
            // JointSpecificProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.TopLayout);
            this.DoubleBuffered = true;
            this.Name = "JointSpecificProperties";
            this.Size = new System.Drawing.Size(189, 136);
            this.TopLayout.ResumeLayout(false);
            this.TopLayout.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel TopLayout;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label NameLabel;
        private System.Windows.Forms.Label JointTypeLabel;
        private System.Windows.Forms.ComboBox JointTypeCombobox;
        private System.Windows.Forms.Label JointNameLabel;
        private System.Windows.Forms.Button JointPoseButton;

    }
}
