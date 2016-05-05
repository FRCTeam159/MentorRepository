namespace GazeboExporter.Robot
{
    partial class PotentiometerProperties
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
            this.label3 = new System.Windows.Forms.Label();
            this.JointList = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.analogChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Analog channel:";
            // 
            // analogChannel
            // 
            this.analogChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analogChannel.Location = new System.Drawing.Point(132, 38);
            this.analogChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.analogChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.analogChannel.Name = "analogChannel";
            this.analogChannel.Size = new System.Drawing.Size(147, 26);
            this.analogChannel.TabIndex = 3;
            this.analogChannel.ValueChanged += new System.EventHandler(this.analogChannel_ValueChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(79, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Joint:";
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(132, 3);
            this.JointList.Name = "JointList";
            this.JointList.Size = new System.Drawing.Size(147, 28);
            this.JointList.TabIndex = 7;
            this.JointList.SelectedIndexChanged += new System.EventHandler(this.JointList_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.analogChannel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(282, 68);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // PotentiometerProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PotentiometerProperties";
            this.Size = new System.Drawing.Size(282, 68);
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
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
