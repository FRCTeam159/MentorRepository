namespace GazeboExporter.Robot
{
    partial class CameraProperties
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
            this.AttachmentPose = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.FOVbox = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // AttachmentPose
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.AttachmentPose, 2);
            this.AttachmentPose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttachmentPose.Location = new System.Drawing.Point(3, 35);
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
            this.label2.Location = new System.Drawing.Point(3, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(122, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Horizontal FOV:";
            // 
            // FOVbox
            // 
            this.FOVbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FOVbox.Location = new System.Drawing.Point(131, 3);
            this.FOVbox.Name = "FOVbox";
            this.FOVbox.Size = new System.Drawing.Size(162, 26);
            this.FOVbox.TabIndex = 14;
            this.FOVbox.LostFocus += new System.EventHandler(this.FOV_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.AttachmentPose, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.FOVbox, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(296, 67);
            this.tableLayoutPanel1.TabIndex = 15;
            // 
            // CameraProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "CameraProperties";
            this.Size = new System.Drawing.Size(296, 67);
            this.Load += new System.EventHandler(this.LoadToolTips);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button AttachmentPose;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FOVbox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
