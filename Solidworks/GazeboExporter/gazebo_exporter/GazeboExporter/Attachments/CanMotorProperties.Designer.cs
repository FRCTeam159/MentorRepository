namespace GazeboExporter.Robot
{
    partial class CanMotorProperties
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
            this.multiplier = new System.Windows.Forms.TextBox();
            this.canChannel = new System.Windows.Forms.NumericUpDown();
            this.channelLabel = new System.Windows.Forms.Label();
            this.multiplierLabel = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.JointList = new System.Windows.Forms.ComboBox();
            this.jointLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.canChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplier
            // 
            this.multiplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiplier.Location = new System.Drawing.Point(79, 92);
            this.multiplier.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.multiplier.Name = "multiplier";
            this.multiplier.Size = new System.Drawing.Size(145, 20);
            this.multiplier.TabIndex = 0;
            this.multiplier.LostFocus += new System.EventHandler(this.multiplier_LostFocus);
            // 
            // canChannel
            // 
            this.canChannel.AllowDrop = true;
            this.canChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.canChannel.Location = new System.Drawing.Point(79, 66);
            this.canChannel.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.canChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.canChannel.Name = "canChannel";
            this.canChannel.Size = new System.Drawing.Size(145, 20);
            this.canChannel.TabIndex = 1;
            this.canChannel.ValueChanged += new System.EventHandler(this.canChannel_ValueChanged);
            // 
            // channelLabel
            // 
            this.channelLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.channelLabel.AutoSize = true;
            this.channelLabel.Location = new System.Drawing.Point(2, 69);
            this.channelLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.channelLabel.Name = "channelLabel";
            this.channelLabel.Size = new System.Drawing.Size(73, 13);
            this.channelLabel.TabIndex = 2;
            this.channelLabel.Text = "CAN channel:";
            // 
            // multiplierLabel
            // 
            this.multiplierLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.multiplierLabel.AutoSize = true;
            this.multiplierLabel.Location = new System.Drawing.Point(24, 95);
            this.multiplierLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.multiplierLabel.Name = "multiplierLabel";
            this.multiplierLabel.Size = new System.Drawing.Size(51, 13);
            this.multiplierLabel.TabIndex = 3;
            this.multiplierLabel.Text = "Multiplier:";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Yellow;
            this.tableLayoutPanel1.SetColumnSpan(this.textBox2, 2);
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(2, 3);
            this.textBox2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(222, 32);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "CAN Motor Controller";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(79, 40);
            this.JointList.Margin = new System.Windows.Forms.Padding(2);
            this.JointList.Name = "JointList";
            this.JointList.Size = new System.Drawing.Size(145, 21);
            this.JointList.TabIndex = 5;
            this.JointList.SelectedIndexChanged += new System.EventHandler(this.JointList_SelectedIndexChanged);
            // 
            // jointLabel
            // 
            this.jointLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.jointLabel.AutoSize = true;
            this.jointLabel.Location = new System.Drawing.Point(43, 44);
            this.jointLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.jointLabel.Name = "jointLabel";
            this.jointLabel.Size = new System.Drawing.Size(32, 13);
            this.jointLabel.TabIndex = 6;
            this.jointLabel.Text = "Joint:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.multiplierLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.jointLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.channelLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.canChannel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.multiplier, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(226, 135);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // CanMotorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.Name = "CanMotorProperties";
            this.Size = new System.Drawing.Size(226, 115);
            this.Load += new System.EventHandler(this.LoadToolTips);
            ((System.ComponentModel.ISupportInitialize)(this.canChannel)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox multiplier;
        private System.Windows.Forms.NumericUpDown canChannel;
        private System.Windows.Forms.Label channelLabel;
        private System.Windows.Forms.Label multiplierLabel;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.Label jointLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label minLimitLabel;
        private System.Windows.Forms.TextBox minLimitValue;
        private System.Windows.Forms.Label maxLimitLabel;
        private System.Windows.Forms.TextBox maxLimitValue;
        private System.Windows.Forms.Label feedbackLabel;
        private System.Windows.Forms.ComboBox feedbackTypes;

    }
}
