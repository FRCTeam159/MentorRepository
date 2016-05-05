namespace GazeboExporter.Robot
{
    partial class ScrewJointSpecialProperties
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ThreadPitchTextBox = new System.Windows.Forms.TextBox();
            this.ThreadPitchLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(172, 41);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Screw Parameters";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ThreadPitchTextBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.ThreadPitchLabel, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 15);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(168, 24);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ThreadPitchTextBox
            // 
            this.ThreadPitchTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ThreadPitchTextBox.Location = new System.Drawing.Point(77, 2);
            this.ThreadPitchTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ThreadPitchTextBox.Name = "ThreadPitchTextBox";
            this.ThreadPitchTextBox.Size = new System.Drawing.Size(89, 20);
            this.ThreadPitchTextBox.TabIndex = 0;
            this.ThreadPitchTextBox.TextChanged += new System.EventHandler(this.ThreadPitchTextBox_TextChanged);
            // 
            // ThreadPitchLabel
            // 
            this.ThreadPitchLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ThreadPitchLabel.AutoSize = true;
            this.ThreadPitchLabel.Location = new System.Drawing.Point(2, 5);
            this.ThreadPitchLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ThreadPitchLabel.Name = "ThreadPitchLabel";
            this.ThreadPitchLabel.Size = new System.Drawing.Size(71, 13);
            this.ThreadPitchLabel.TabIndex = 1;
            this.ThreadPitchLabel.Text = "Thread Pitch:";
            // 
            // ScrewJointSpecialProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ScrewJointSpecialProperties";
            this.Size = new System.Drawing.Size(172, 41);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
            this.Load += new System.EventHandler(this.LoadToolTips);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox ThreadPitchTextBox;
        private System.Windows.Forms.Label ThreadPitchLabel;

    }
}
