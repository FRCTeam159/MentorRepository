namespace GazeboExporter.Robot
{
    partial class RobotProperties
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
            this.robotName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.RobotSettings = new System.Windows.Forms.GroupBox();
            this.ExporterTypeBox = new System.Windows.Forms.GroupBox();
            this.SDFExportTypeButton = new System.Windows.Forms.RadioButton();
            this.URDFExportTypeButton = new System.Windows.Forms.RadioButton();
            this.IncludeFRCfieldBox = new System.Windows.Forms.GroupBox();
            this.NoFieldButton = new System.Windows.Forms.RadioButton();
            this.FRC2014FieldButton = new System.Windows.Forms.RadioButton();
            this.FRC2015FieldButton = new System.Windows.Forms.RadioButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.RobotSettings.SuspendLayout();
            this.ExporterTypeBox.SuspendLayout();
            this.IncludeFRCfieldBox.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // robotName
            // 
            this.robotName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.robotName.Location = new System.Drawing.Point(64, 5);
            this.robotName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.robotName.Name = "robotName";
            this.robotName.Size = new System.Drawing.Size(212, 26);
            this.robotName.TabIndex = 1;
            this.robotName.TextChanged += new System.EventHandler(this.robotName_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Name:";
            // 
            // SelectButton
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.SelectButton, 2);
            this.SelectButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectButton.Location = new System.Drawing.Point(3, 39);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(273, 34);
            this.SelectButton.TabIndex = 42;
            this.SelectButton.Text = "Select Model Origin";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // RobotSettings
            // 
            this.RobotSettings.AutoSize = true;
            this.RobotSettings.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.RobotSettings, 2);
            this.RobotSettings.Controls.Add(this.flowLayoutPanel3);
            this.RobotSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RobotSettings.Location = new System.Drawing.Point(3, 79);
            this.RobotSettings.Name = "RobotSettings";
            this.RobotSettings.Size = new System.Drawing.Size(273, 237);
            this.RobotSettings.TabIndex = 43;
            this.RobotSettings.TabStop = false;
            this.RobotSettings.Text = "Model Settings";
            // 
            // ExporterTypeBox
            // 
            this.ExporterTypeBox.AutoSize = true;
            this.ExporterTypeBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ExporterTypeBox.Controls.Add(this.flowLayoutPanel1);
            this.ExporterTypeBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ExporterTypeBox.Location = new System.Drawing.Point(3, 3);
            this.ExporterTypeBox.Name = "ExporterTypeBox";
            this.ExporterTypeBox.Size = new System.Drawing.Size(232, 85);
            this.ExporterTypeBox.TabIndex = 3;
            this.ExporterTypeBox.TabStop = false;
            this.ExporterTypeBox.Text = "Export As:";
            // 
            // SDFExportTypeButton
            // 
            this.SDFExportTypeButton.AutoSize = true;
            this.SDFExportTypeButton.Location = new System.Drawing.Point(3, 3);
            this.SDFExportTypeButton.Name = "SDFExportTypeButton";
            this.SDFExportTypeButton.Size = new System.Drawing.Size(67, 24);
            this.SDFExportTypeButton.TabIndex = 0;
            this.SDFExportTypeButton.Text = "SDF";
            this.SDFExportTypeButton.UseVisualStyleBackColor = true;
            this.SDFExportTypeButton.CheckedChanged += new System.EventHandler(this.ExportButtons_CheckedChanged);
            // 
            // URDFExportTypeButton
            // 
            this.URDFExportTypeButton.AutoSize = true;
            this.URDFExportTypeButton.Location = new System.Drawing.Point(3, 33);
            this.URDFExportTypeButton.Name = "URDFExportTypeButton";
            this.URDFExportTypeButton.Size = new System.Drawing.Size(80, 24);
            this.URDFExportTypeButton.TabIndex = 1;
            this.URDFExportTypeButton.Text = "URDF";
            this.URDFExportTypeButton.UseVisualStyleBackColor = true;
            this.URDFExportTypeButton.CheckedChanged += new System.EventHandler(this.ExportButtons_CheckedChanged);
            // 
            // IncludeFRCfieldBox
            // 
            this.IncludeFRCfieldBox.AutoSize = true;
            this.IncludeFRCfieldBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.IncludeFRCfieldBox.Controls.Add(this.flowLayoutPanel2);
            this.IncludeFRCfieldBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IncludeFRCfieldBox.Location = new System.Drawing.Point(3, 94);
            this.IncludeFRCfieldBox.Name = "IncludeFRCfieldBox";
            this.IncludeFRCfieldBox.Size = new System.Drawing.Size(232, 115);
            this.IncludeFRCfieldBox.TabIndex = 2;
            this.IncludeFRCfieldBox.TabStop = false;
            this.IncludeFRCfieldBox.Text = "Include FRC field";
            // 
            // NoFieldButton
            // 
            this.NoFieldButton.AutoSize = true;
            this.NoFieldButton.Location = new System.Drawing.Point(3, 3);
            this.NoFieldButton.Name = "NoFieldButton";
            this.NoFieldButton.Size = new System.Drawing.Size(92, 24);
            this.NoFieldButton.TabIndex = 0;
            this.NoFieldButton.Text = "No Field";
            this.NoFieldButton.UseVisualStyleBackColor = true;
            this.NoFieldButton.CheckedChanged += new System.EventHandler(this.FieldButtons_CheckedChanged);
            // 
            // FRC2014FieldButton
            // 
            this.FRC2014FieldButton.AutoSize = true;
            this.FRC2014FieldButton.Location = new System.Drawing.Point(3, 33);
            this.FRC2014FieldButton.Name = "FRC2014FieldButton";
            this.FRC2014FieldButton.Size = new System.Drawing.Size(209, 24);
            this.FRC2014FieldButton.TabIndex = 0;
            this.FRC2014FieldButton.TabStop = true;
            this.FRC2014FieldButton.Text = "2014 Field (Aerial Assist)";
            this.FRC2014FieldButton.UseVisualStyleBackColor = true;
            this.FRC2014FieldButton.CheckedChanged += new System.EventHandler(this.FieldButtons_CheckedChanged);
            // 
            // FRC2015FieldButton
            // 
            this.FRC2015FieldButton.AutoSize = true;
            this.FRC2015FieldButton.Location = new System.Drawing.Point(3, 63);
            this.FRC2015FieldButton.Name = "FRC2015FieldButton";
            this.FRC2015FieldButton.Size = new System.Drawing.Size(220, 24);
            this.FRC2015FieldButton.TabIndex = 1;
            this.FRC2015FieldButton.TabStop = true;
            this.FRC2015FieldButton.Text = "2015 Field (Recycle Rush)";
            this.FRC2015FieldButton.UseVisualStyleBackColor = true;
            this.FRC2015FieldButton.CheckedChanged += new System.EventHandler(this.FieldButtons_CheckedChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.robotName, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.RobotSettings, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.SelectButton, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(279, 319);
            this.tableLayoutPanel1.TabIndex = 44;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.SDFExportTypeButton);
            this.flowLayoutPanel1.Controls.Add(this.URDFExportTypeButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(226, 60);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.NoFieldButton);
            this.flowLayoutPanel2.Controls.Add(this.FRC2014FieldButton);
            this.flowLayoutPanel2.Controls.Add(this.FRC2015FieldButton);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(226, 90);
            this.flowLayoutPanel2.TabIndex = 45;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.AutoSize = true;
            this.flowLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel3.Controls.Add(this.ExporterTypeBox);
            this.flowLayoutPanel3.Controls.Add(this.IncludeFRCfieldBox);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(267, 212);
            this.flowLayoutPanel3.TabIndex = 45;
            // 
            // RobotProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "RobotProperties";
            this.Size = new System.Drawing.Size(279, 319);
            this.Load += new System.EventHandler(this.LoadToolTips);
            this.RobotSettings.ResumeLayout(false);
            this.RobotSettings.PerformLayout();
            this.ExporterTypeBox.ResumeLayout(false);
            this.ExporterTypeBox.PerformLayout();
            this.IncludeFRCfieldBox.ResumeLayout(false);
            this.IncludeFRCfieldBox.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox robotName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button SelectButton;
        private System.Windows.Forms.GroupBox RobotSettings;
        private System.Windows.Forms.GroupBox IncludeFRCfieldBox;
        private System.Windows.Forms.RadioButton FRC2014FieldButton;
        private System.Windows.Forms.RadioButton FRC2015FieldButton;
        private System.Windows.Forms.GroupBox ExporterTypeBox;
        private System.Windows.Forms.RadioButton SDFExportTypeButton;
        private System.Windows.Forms.RadioButton URDFExportTypeButton;
        private System.Windows.Forms.RadioButton NoFieldButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
    }
}
