namespace GazeboExporter.UI
{
    partial class ManageRobot
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.attachmentSelector1 = new GazeboExporter.UI.AttachmentSelector();
            this.linkSelectorPanel1 = new GazeboExporter.UI.LinkSelectorPanel();
            this.robotGraphPanel1 = new GazeboExporter.UI.RobotGraphPanel();
            this.propertyEditor1 = new GazeboExporter.UI.PropertyEditor();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.attachmentSelector1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.linkSelectorPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 283);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(549, 204);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // attachmentSelector1
            // 
            this.attachmentSelector1.AutoSize = true;
            this.attachmentSelector1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.attachmentSelector1.Location = new System.Drawing.Point(109, 4);
            this.attachmentSelector1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.attachmentSelector1.MinimumSize = new System.Drawing.Size(0, 196);
            this.attachmentSelector1.Name = "attachmentSelector1";
            this.attachmentSelector1.Size = new System.Drawing.Size(437, 196);
            this.attachmentSelector1.TabIndex = 1;
            // 
            // linkSelectorPanel1
            // 
            this.linkSelectorPanel1.AutoSize = true;
            this.linkSelectorPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.linkSelectorPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.linkSelectorPanel1.Location = new System.Drawing.Point(3, 3);
            this.linkSelectorPanel1.MaximumSize = new System.Drawing.Size(100, 196);
            this.linkSelectorPanel1.MinimumSize = new System.Drawing.Size(100, 196);
            this.linkSelectorPanel1.Name = "linkSelectorPanel1";
            this.linkSelectorPanel1.Size = new System.Drawing.Size(100, 196);
            this.linkSelectorPanel1.TabIndex = 2;
            // 
            // robotGraphPanel1
            // 
            this.robotGraphPanel1.AllowDrop = true;
            this.robotGraphPanel1.AutoSize = true;
            this.robotGraphPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.robotGraphPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotGraphPanel1.Location = new System.Drawing.Point(0, 0);
            this.robotGraphPanel1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.robotGraphPanel1.MinimumSize = new System.Drawing.Size(100, 0);
            this.robotGraphPanel1.Name = "robotGraphPanel1";
            this.robotGraphPanel1.Size = new System.Drawing.Size(549, 283);
            this.robotGraphPanel1.TabIndex = 2;
            // 
            // propertyEditor1
            // 
            this.propertyEditor1.AutoSize = true;
            this.propertyEditor1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propertyEditor1.BackColor = System.Drawing.SystemColors.Control;
            this.propertyEditor1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertyEditor1.Dock = System.Windows.Forms.DockStyle.Right;
            this.propertyEditor1.Location = new System.Drawing.Point(549, 0);
            this.propertyEditor1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.propertyEditor1.MinimumSize = new System.Drawing.Size(234, 2);
            this.propertyEditor1.Name = "propertyEditor1";
            this.propertyEditor1.Size = new System.Drawing.Size(234, 487);
            this.propertyEditor1.TabIndex = 0;
            // 
            // ManageRobot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(783, 487);
            this.Controls.Add(this.robotGraphPanel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.propertyEditor1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MinimumSize = new System.Drawing.Size(539, 403);
            this.Name = "ManageRobot";
            this.ShowIcon = false;
            this.Text = "Manage Robot";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManageRobot_FormClosing);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.KeyPress_Handler);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PropertyEditor propertyEditor1;
        private RobotGraphPanel robotGraphPanel1;
        private AttachmentSelector attachmentSelector1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LinkSelectorPanel linkSelectorPanel1;

    }
}