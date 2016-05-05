namespace GazeboExporter.Robot
{
    partial class SingleAxisJointPhysicalProperties
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
            this.DampingLabel = new System.Windows.Forms.Label();
            this.FrictionLabel = new System.Windows.Forms.Label();
            this.EffortLabel = new System.Windows.Forms.Label();
            this.Axis1Label = new System.Windows.Forms.Label();
            this.Axis1DampingTextbox = new System.Windows.Forms.TextBox();
            this.Axis1FrictionTextbox = new System.Windows.Forms.TextBox();
            this.Axis1EffortTextbox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(195, 141);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Joint Properties";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.DampingLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.FrictionLabel, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.EffortLabel, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.Axis1Label, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.Axis1DampingTextbox, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.Axis1FrictionTextbox, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.Axis1EffortTextbox, 1, 3);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 22);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(189, 116);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // DampingLabel
            // 
            this.DampingLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.DampingLabel.AutoSize = true;
            this.DampingLabel.Location = new System.Drawing.Point(3, 26);
            this.DampingLabel.Name = "DampingLabel";
            this.DampingLabel.Size = new System.Drawing.Size(77, 20);
            this.DampingLabel.TabIndex = 0;
            this.DampingLabel.Text = "Damping:";
            // 
            // FrictionLabel
            // 
            this.FrictionLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.FrictionLabel.AutoSize = true;
            this.FrictionLabel.Location = new System.Drawing.Point(15, 58);
            this.FrictionLabel.Name = "FrictionLabel";
            this.FrictionLabel.Size = new System.Drawing.Size(65, 20);
            this.FrictionLabel.TabIndex = 1;
            this.FrictionLabel.Text = "Friction:";
            // 
            // EffortLabel
            // 
            this.EffortLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.EffortLabel.AutoSize = true;
            this.EffortLabel.Location = new System.Drawing.Point(27, 90);
            this.EffortLabel.Name = "EffortLabel";
            this.EffortLabel.Size = new System.Drawing.Size(53, 20);
            this.EffortLabel.TabIndex = 2;
            this.EffortLabel.Text = "Effort:";
            // 
            // Axis1Label
            // 
            this.Axis1Label.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.Axis1Label.AutoSize = true;
            this.Axis1Label.Location = new System.Drawing.Point(112, 0);
            this.Axis1Label.Name = "Axis1Label";
            this.Axis1Label.Size = new System.Drawing.Size(47, 20);
            this.Axis1Label.TabIndex = 3;
            this.Axis1Label.Text = "Axis1";
            // 
            // Axis1DampingTextbox
            // 
            this.Axis1DampingTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis1DampingTextbox.Location = new System.Drawing.Point(86, 23);
            this.Axis1DampingTextbox.Name = "Axis1DampingTextbox";
            this.Axis1DampingTextbox.Size = new System.Drawing.Size(100, 26);
            this.Axis1DampingTextbox.TabIndex = 4;
            this.Axis1DampingTextbox.TextChanged += new System.EventHandler(this.Axis1DampingTextbox_TextChanged);
            // 
            // Axis1FrictionTextbox
            // 
            this.Axis1FrictionTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis1FrictionTextbox.Location = new System.Drawing.Point(86, 55);
            this.Axis1FrictionTextbox.Name = "Axis1FrictionTextbox";
            this.Axis1FrictionTextbox.Size = new System.Drawing.Size(100, 26);
            this.Axis1FrictionTextbox.TabIndex = 5;
            this.Axis1FrictionTextbox.TextChanged += new System.EventHandler(this.Axis1FrictionTextbox_TextChanged);
            // 
            // Axis1EffortTextbox
            // 
            this.Axis1EffortTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Axis1EffortTextbox.Location = new System.Drawing.Point(86, 87);
            this.Axis1EffortTextbox.Name = "Axis1EffortTextbox";
            this.Axis1EffortTextbox.Size = new System.Drawing.Size(100, 26);
            this.Axis1EffortTextbox.TabIndex = 6;
            this.Axis1EffortTextbox.TextChanged += new System.EventHandler(this.Axis1EffortTextbox_TextChanged);
            // 
            // AxialJointPhysicalProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.groupBox1);
            this.Name = "AxialJointPhysicalProperties";
            this.Size = new System.Drawing.Size(195, 141);
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
        private System.Windows.Forms.Label DampingLabel;
        private System.Windows.Forms.Label FrictionLabel;
        private System.Windows.Forms.Label EffortLabel;
        private System.Windows.Forms.Label Axis1Label;
        private System.Windows.Forms.TextBox Axis1DampingTextbox;
        private System.Windows.Forms.TextBox Axis1FrictionTextbox;
        private System.Windows.Forms.TextBox Axis1EffortTextbox;
    }
}
