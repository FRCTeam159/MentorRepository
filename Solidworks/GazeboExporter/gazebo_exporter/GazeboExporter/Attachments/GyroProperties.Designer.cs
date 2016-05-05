namespace GazeboExporter.Robot
{
    partial class GyroProperties
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
            this.label1 = new System.Windows.Forms.Label();
            this.analogChannel = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.gyroAxis = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.analogChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Analog channel:";
            // 
            // analogChannel
            // 
            this.analogChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analogChannel.Location = new System.Drawing.Point(132, 5);
            this.analogChannel.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.analogChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.analogChannel.Name = "analogChannel";
            this.analogChannel.Size = new System.Drawing.Size(148, 26);
            this.analogChannel.TabIndex = 3;
            this.analogChannel.ValueChanged += new System.EventHandler(this.analogChannel_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(84, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Axis:";
            // 
            // gyroAxis
            // 
            this.gyroAxis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gyroAxis.FormattingEnabled = true;
            this.gyroAxis.Location = new System.Drawing.Point(132, 39);
            this.gyroAxis.Name = "gyroAxis";
            this.gyroAxis.Size = new System.Drawing.Size(148, 28);
            this.gyroAxis.TabIndex = 6;
            this.gyroAxis.SelectedIndexChanged += new System.EventHandler(this.gyroAxis_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.gyroAxis, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.analogChannel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(283, 70);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // GyroProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "GyroProperties";
            this.Size = new System.Drawing.Size(283, 70);
            this.Load += new System.EventHandler(this.LoadToolTips);
            ((System.ComponentModel.ISupportInitialize)(this.analogChannel)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown analogChannel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox gyroAxis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
