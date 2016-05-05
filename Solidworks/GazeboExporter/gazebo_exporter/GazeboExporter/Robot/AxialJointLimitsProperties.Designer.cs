namespace GazeboExporter.Robot
{
    partial class AxialJointLimitsProperties
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
            this.LimitsGroupbox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ContinousCheckbox = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.LowerLimitTextbox = new System.Windows.Forms.TextBox();
            this.UpperLImitLabel = new System.Windows.Forms.Label();
            this.LowerLimitLabel = new System.Windows.Forms.Label();
            this.UpperLimitTextbox = new System.Windows.Forms.TextBox();
            this.LimitSelectionButton = new System.Windows.Forms.Button();
            this.ManualLimitsCheckbox = new System.Windows.Forms.CheckBox();
            this.LimitsGroupbox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // LimitsGroupbox
            // 
            this.LimitsGroupbox.AutoSize = true;
            this.LimitsGroupbox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LimitsGroupbox.Controls.Add(this.tableLayoutPanel1);
            this.LimitsGroupbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LimitsGroupbox.Location = new System.Drawing.Point(0, 0);
            this.LimitsGroupbox.Name = "LimitsGroupbox";
            this.LimitsGroupbox.Size = new System.Drawing.Size(218, 191);
            this.LimitsGroupbox.TabIndex = 0;
            this.LimitsGroupbox.TabStop = false;
            this.LimitsGroupbox.Text = "Limits";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ContinousCheckbox, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LimitSelectionButton, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.ManualLimitsCheckbox, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(212, 166);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ContinousCheckbox
            // 
            this.ContinousCheckbox.AutoSize = true;
            this.ContinousCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContinousCheckbox.Location = new System.Drawing.Point(3, 139);
            this.ContinousCheckbox.Name = "ContinousCheckbox";
            this.ContinousCheckbox.Size = new System.Drawing.Size(206, 24);
            this.ContinousCheckbox.TabIndex = 3;
            this.ContinousCheckbox.Text = "Is Continuous";
            this.ContinousCheckbox.UseVisualStyleBackColor = true;
            this.ContinousCheckbox.CheckedChanged += new System.EventHandler(this.ContinousCheckbox_CheckedChanged);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.LowerLimitTextbox, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.UpperLImitLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.LowerLimitLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.UpperLimitTextbox, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(206, 64);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // LowerLimitTextbox
            // 
            this.LowerLimitTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LowerLimitTextbox.Location = new System.Drawing.Point(103, 35);
            this.LowerLimitTextbox.Name = "LowerLimitTextbox";
            this.LowerLimitTextbox.Size = new System.Drawing.Size(100, 26);
            this.LowerLimitTextbox.TabIndex = 3;
            this.LowerLimitTextbox.TextChanged += new System.EventHandler(this.LowerLimitTextbox_TextChanged);
            // 
            // UpperLImitLabel
            // 
            this.UpperLImitLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.UpperLImitLabel.AutoSize = true;
            this.UpperLImitLabel.Location = new System.Drawing.Point(3, 6);
            this.UpperLImitLabel.Name = "UpperLImitLabel";
            this.UpperLImitLabel.Size = new System.Drawing.Size(94, 20);
            this.UpperLImitLabel.TabIndex = 0;
            this.UpperLImitLabel.Text = "Upper Limit:";
            // 
            // LowerLimitLabel
            // 
            this.LowerLimitLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.LowerLimitLabel.AutoSize = true;
            this.LowerLimitLabel.Location = new System.Drawing.Point(4, 38);
            this.LowerLimitLabel.Name = "LowerLimitLabel";
            this.LowerLimitLabel.Size = new System.Drawing.Size(93, 20);
            this.LowerLimitLabel.TabIndex = 1;
            this.LowerLimitLabel.Text = "Lower Limit:";
            // 
            // UpperLimitTextbox
            // 
            this.UpperLimitTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UpperLimitTextbox.Location = new System.Drawing.Point(103, 3);
            this.UpperLimitTextbox.Name = "UpperLimitTextbox";
            this.UpperLimitTextbox.Size = new System.Drawing.Size(100, 26);
            this.UpperLimitTextbox.TabIndex = 2;
            this.UpperLimitTextbox.TextChanged += new System.EventHandler(this.UpperLimitTextbox_TextChanged);
            // 
            // LimitSelectionButton
            // 
            this.LimitSelectionButton.AutoSize = true;
            this.LimitSelectionButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LimitSelectionButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LimitSelectionButton.Location = new System.Drawing.Point(3, 73);
            this.LimitSelectionButton.MinimumSize = new System.Drawing.Size(0, 30);
            this.LimitSelectionButton.Name = "LimitSelectionButton";
            this.LimitSelectionButton.Size = new System.Drawing.Size(206, 30);
            this.LimitSelectionButton.TabIndex = 1;
            this.LimitSelectionButton.Text = "Select Limits";
            this.LimitSelectionButton.UseVisualStyleBackColor = true;
            this.LimitSelectionButton.Click += new System.EventHandler(this.LimitSelectionButton_Click);
            // 
            // ManualLimitsCheckbox
            // 
            this.ManualLimitsCheckbox.AutoSize = true;
            this.ManualLimitsCheckbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManualLimitsCheckbox.Location = new System.Drawing.Point(3, 109);
            this.ManualLimitsCheckbox.Name = "ManualLimitsCheckbox";
            this.ManualLimitsCheckbox.Size = new System.Drawing.Size(206, 24);
            this.ManualLimitsCheckbox.TabIndex = 2;
            this.ManualLimitsCheckbox.Text = "Select Manual Limits";
            this.ManualLimitsCheckbox.UseVisualStyleBackColor = true;
            this.ManualLimitsCheckbox.CheckedChanged += new System.EventHandler(this.ManualLimitsCheckbox_CheckedChanged);
            // 
            // AxialJointLimitsProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.LimitsGroupbox);
            this.Name = "AxialJointLimitsProperties";
            this.Size = new System.Drawing.Size(218, 191);
            this.LimitsGroupbox.ResumeLayout(false);
            this.LimitsGroupbox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Load += new System.EventHandler(this.LoadToolTips);

        }

        #endregion

        private System.Windows.Forms.GroupBox LimitsGroupbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TextBox LowerLimitTextbox;
        private System.Windows.Forms.Label UpperLImitLabel;
        private System.Windows.Forms.Label LowerLimitLabel;
        private System.Windows.Forms.TextBox UpperLimitTextbox;
        private System.Windows.Forms.Button LimitSelectionButton;
        private System.Windows.Forms.CheckBox ManualLimitsCheckbox;
        private System.Windows.Forms.CheckBox ContinousCheckbox;
    }
}
