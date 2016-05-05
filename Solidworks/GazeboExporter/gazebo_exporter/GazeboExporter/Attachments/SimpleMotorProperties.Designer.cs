namespace GazeboExporter.Robot
{
    partial class SimpleMotorProperties
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
            this.pwmChannel = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.JointList = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.pwmChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // multiplier
            // 
            this.multiplier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.multiplier.Location = new System.Drawing.Point(120, 197);
            this.multiplier.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.multiplier.Name = "multiplier";
            this.multiplier.Size = new System.Drawing.Size(175, 26);
            this.multiplier.TabIndex = 0;
            this.multiplier.LostFocus += new System.EventHandler(this.multiplier_LostFocus);
            // 
            // pwmChannel
            // 
            this.pwmChannel.AllowDrop = true;
            this.pwmChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pwmChannel.Location = new System.Drawing.Point(120, 161);
            this.pwmChannel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.pwmChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.pwmChannel.Name = "pwmChannel";
            this.pwmChannel.Size = new System.Drawing.Size(175, 26);
            this.pwmChannel.TabIndex = 1;
            this.pwmChannel.ValueChanged += new System.EventHandler(this.pwmChannel_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(111, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "PWM channel:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 200);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Multiplier:";
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.Yellow;
            this.tableLayoutPanel1.SetColumnSpan(this.textBox2, 2);
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Location = new System.Drawing.Point(3, 5);
            this.textBox2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(292, 112);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "This is the simples model for a motor in gazebo. The torque/force output to the j" +
    "oint will be equal to the value written to the provided PWM channel (-1 to 1) ti" +
    "mes the given Multiplier.";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(120, 125);
            this.JointList.Name = "JointList";
            this.JointList.Size = new System.Drawing.Size(175, 28);
            this.JointList.TabIndex = 5;
            this.JointList.SelectedIndexChanged += new System.EventHandler(this.JointList_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(67, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Joint:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pwmChannel, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.multiplier, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(298, 228);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // SimpleMotorProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "SimpleMotorProperties";
            this.Size = new System.Drawing.Size(298, 228);
            this.Load += new System.EventHandler(this.LoadToolTips);
            ((System.ComponentModel.ISupportInitialize)(this.pwmChannel)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox multiplier;
        private System.Windows.Forms.NumericUpDown pwmChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
