namespace GazeboExporter.Robot
{
    partial class RangefinderProperties
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
            this.AttachmentPose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Radius = new System.Windows.Forms.TextBox();
            this.MinDist = new System.Windows.Forms.TextBox();
            this.MaxDist = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.analogChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(123, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Analog channel:";
            // 
            // analogChannel
            // 
            this.analogChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.analogChannel.Location = new System.Drawing.Point(132, 4);
            this.analogChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.analogChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.analogChannel.Name = "analogChannel";
            this.analogChannel.Size = new System.Drawing.Size(161, 26);
            this.analogChannel.TabIndex = 3;
            this.analogChannel.ValueChanged += new System.EventHandler(this.analogChannel_ValueChanged);
            // 
            // AttachmentPose
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.AttachmentPose, 2);
            this.AttachmentPose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttachmentPose.Location = new System.Drawing.Point(3, 133);
            this.AttachmentPose.Name = "AttachmentPose";
            this.AttachmentPose.Size = new System.Drawing.Size(290, 29);
            this.AttachmentPose.TabIndex = 7;
            this.AttachmentPose.Text = "Attachment Pose";
            this.AttachmentPose.UseVisualStyleBackColor = true;
            this.AttachmentPose.Click += new System.EventHandler(this.AttachmentPose_Click);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Radius:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 20);
            this.label3.TabIndex = 11;
            this.label3.Text = "Minimum Dist:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 20);
            this.label4.TabIndex = 13;
            this.label4.Text = "Maximum Dist:";
            // 
            // Radius
            // 
            this.Radius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Radius.Location = new System.Drawing.Point(132, 37);
            this.Radius.Name = "Radius";
            this.Radius.Size = new System.Drawing.Size(161, 26);
            this.Radius.TabIndex = 14;
            this.Radius.LostFocus += new System.EventHandler(this.Radius_TextChanged);
            // 
            // MinDist
            // 
            this.MinDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MinDist.Location = new System.Drawing.Point(132, 69);
            this.MinDist.Name = "MinDist";
            this.MinDist.Size = new System.Drawing.Size(161, 26);
            this.MinDist.TabIndex = 15;
            this.MinDist.LostFocus += new System.EventHandler(this.MinDist_TextChanged);
            // 
            // MaxDist
            // 
            this.MaxDist.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MaxDist.Location = new System.Drawing.Point(132, 101);
            this.MaxDist.Name = "MaxDist";
            this.MaxDist.Size = new System.Drawing.Size(161, 26);
            this.MaxDist.TabIndex = 16;
            this.MaxDist.LostFocus += new System.EventHandler(this.MaxDist_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.analogChannel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Radius, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.MinDist, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.MaxDist, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.AttachmentPose, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(296, 165);
            this.tableLayoutPanel1.TabIndex = 17;
            // 
            // RangefinderProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "RangefinderProperties";
            this.Size = new System.Drawing.Size(296, 165);
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
        private System.Windows.Forms.Button AttachmentPose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox Radius;
        private System.Windows.Forms.TextBox MinDist;
        private System.Windows.Forms.TextBox MaxDist;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
