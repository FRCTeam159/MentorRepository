namespace GazeboExporter.UI
{
    partial class SettingsForm
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
            this.useFRCsimCheckbox = new System.Windows.Forms.CheckBox();
            this.DebugCheckbox = new System.Windows.Forms.CheckBox();
            this.ExporterSettingsBox = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.logCheckbox = new System.Windows.Forms.CheckBox();
            this.ExporterSettingsBox.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // useFRCsimCheckbox
            // 
            this.useFRCsimCheckbox.AutoSize = true;
            this.useFRCsimCheckbox.Location = new System.Drawing.Point(4, 4);
            this.useFRCsimCheckbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.useFRCsimCheckbox.Name = "useFRCsimCheckbox";
            this.useFRCsimCheckbox.Size = new System.Drawing.Size(322, 30);
            this.useFRCsimCheckbox.TabIndex = 0;
            this.useFRCsimCheckbox.Text = "Enable FRCsim Components";
            this.useFRCsimCheckbox.UseVisualStyleBackColor = true;
            this.useFRCsimCheckbox.CheckedChanged += new System.EventHandler(this.useFRCsimCheckbox_CheckedChanged);
            // 
            // DebugCheckbox
            // 
            this.DebugCheckbox.AutoSize = true;
            this.DebugCheckbox.Location = new System.Drawing.Point(4, 78);
            this.DebugCheckbox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DebugCheckbox.Name = "DebugCheckbox";
            this.DebugCheckbox.Size = new System.Drawing.Size(236, 30);
            this.DebugCheckbox.TabIndex = 1;
            this.DebugCheckbox.Text = "Enable Debug Mode";
            this.DebugCheckbox.UseVisualStyleBackColor = true;
            this.DebugCheckbox.CheckedChanged += new System.EventHandler(this.DebugCheckbox_CheckedChanged);
            // 
            // ExporterSettingsBox
            // 
            this.ExporterSettingsBox.AutoSize = true;
            this.ExporterSettingsBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ExporterSettingsBox.Controls.Add(this.flowLayoutPanel2);
            this.ExporterSettingsBox.Location = new System.Drawing.Point(4, 4);
            this.ExporterSettingsBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ExporterSettingsBox.Name = "ExporterSettingsBox";
            this.ExporterSettingsBox.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ExporterSettingsBox.Size = new System.Drawing.Size(338, 144);
            this.ExporterSettingsBox.TabIndex = 2;
            this.ExporterSettingsBox.TabStop = false;
            this.ExporterSettingsBox.Text = "Exporter Settings";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.useFRCsimCheckbox);
            this.flowLayoutPanel2.Controls.Add(this.logCheckbox);
            this.flowLayoutPanel2.Controls.Add(this.DebugCheckbox);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(4, 28);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(330, 112);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.ExporterSettingsBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(443, 561);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // logCheckbox
            // 
            this.logCheckbox.AutoSize = true;
            this.logCheckbox.Location = new System.Drawing.Point(3, 41);
            this.logCheckbox.Name = "logCheckbox";
            this.logCheckbox.Size = new System.Drawing.Size(223, 30);
            this.logCheckbox.TabIndex = 4;
            this.logCheckbox.Text = "Enable Print to Log";
            this.logCheckbox.UseVisualStyleBackColor = true;
            this.logCheckbox.CheckedChanged += new System.EventHandler(this.logCheckbox_CheckedChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(443, 561);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.TopMost = true;
            this.ExporterSettingsBox.ResumeLayout(false);
            this.ExporterSettingsBox.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox useFRCsimCheckbox;
        private System.Windows.Forms.CheckBox DebugCheckbox;
        private System.Windows.Forms.GroupBox ExporterSettingsBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox logCheckbox;
    }
}