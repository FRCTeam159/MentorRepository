namespace GazeboExporter.Robot
{
    partial class QuadEncoderProperties
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
            this.dioChannelA = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.dioChannelB = new System.Windows.Forms.NumericUpDown();
            this.pulsesPerRev = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.JointList = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dioChannelA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dioChannelB)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(137, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "A Output channel:";
            // 
            // dioChannelA
            // 
            this.dioChannelA.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dioChannelA.Location = new System.Drawing.Point(146, 39);
            this.dioChannelA.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dioChannelA.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.dioChannelA.Name = "dioChannelA";
            this.dioChannelA.Size = new System.Drawing.Size(148, 26);
            this.dioChannelA.TabIndex = 3;
            this.dioChannelA.ValueChanged += new System.EventHandler(this.dioChannelA_ValueChanged);
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "B Output channel:";
            // 
            // dioChannelB
            // 
            this.dioChannelB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dioChannelB.Location = new System.Drawing.Point(146, 75);
            this.dioChannelB.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.dioChannelB.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});
            this.dioChannelB.Name = "dioChannelB";
            this.dioChannelB.Size = new System.Drawing.Size(148, 26);
            this.dioChannelB.TabIndex = 5;
            this.dioChannelB.ValueChanged += new System.EventHandler(this.dioChannelB_ValueChanged);
            // 
            // pulsesPerRev
            // 
            this.pulsesPerRev.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pulsesPerRev.Location = new System.Drawing.Point(146, 111);
            this.pulsesPerRev.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.pulsesPerRev.Name = "pulsesPerRev";
            this.pulsesPerRev.Size = new System.Drawing.Size(148, 26);
            this.pulsesPerRev.TabIndex = 7;
            this.pulsesPerRev.LostFocus += new System.EventHandler(this.pulsesPerRev_TextChanged);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Signal pulse rate:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(93, 7);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 20);
            this.label4.TabIndex = 10;
            this.label4.Text = "Joint:";
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(146, 3);
            this.JointList.Name = "JointList";
            this.JointList.Size = new System.Drawing.Size(148, 28);
            this.JointList.TabIndex = 9;
            this.JointList.SelectedIndexChanged += new System.EventHandler(this.JointList_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.dioChannelA, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.dioChannelB, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pulsesPerRev, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(297, 142);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // QuadEncoderProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "QuadEncoderProperties";
            this.Size = new System.Drawing.Size(297, 142);
            this.Load += new System.EventHandler(this.LoadToolTips);
            ((System.ComponentModel.ISupportInitialize)(this.dioChannelA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dioChannelB)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown dioChannelA;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown dioChannelB;
        private System.Windows.Forms.TextBox pulsesPerRev;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
