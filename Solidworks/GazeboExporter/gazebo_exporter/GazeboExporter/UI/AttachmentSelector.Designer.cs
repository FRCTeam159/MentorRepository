namespace GazeboExporter.UI
{
    partial class AttachmentSelector
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
            this.panel = new GazeboExporter.UI.FancyPanel();
            this.hScrollBar1 = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.AutoScroll = true;
            this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel.MinimumSize = new System.Drawing.Size(2, 175);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(150, 175);
            this.panel.TabIndex = 0;
            this.panel.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.AttachmentSelector_GiveFeedback);
            this.panel.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Paint);
            this.panel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel_MouseDown);
            // 
            // hScrollBar1
            // 
            this.hScrollBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar1.Location = new System.Drawing.Point(0, 154);
            this.hScrollBar1.Name = "hScrollBar1";
            this.hScrollBar1.Size = new System.Drawing.Size(150, 21);
            this.hScrollBar1.SmallChange = 10;
            this.hScrollBar1.TabIndex = 1;
            this.hScrollBar1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBar1_Scroll);
            // 
            // AttachmentSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.hScrollBar1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(0, 175);
            this.Name = "AttachmentSelector";
            this.Size = new System.Drawing.Size(150, 175);
            this.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.AttachmentSelector_GiveFeedback);
            this.Resize += new System.EventHandler(this.panel_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private FancyPanel panel;
        private System.Windows.Forms.HScrollBar hScrollBar1;
    }
}
