namespace GazeboExporter.Robot
{
    partial class GearBoxPanel
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.ReferenceJointCombobox = new System.Windows.Forms.ComboBox();
            this.GearboxRatioTextbox = new System.Windows.Forms.TextBox();
            this.GearboxRatioLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ReferenceJointLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.ReferenceJointLabel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ReferenceJointCombobox, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(234, 92);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ReferenceJointCombobox
            // 
            this.ReferenceJointCombobox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ReferenceJointCombobox.FormattingEnabled = true;
            this.ReferenceJointCombobox.Location = new System.Drawing.Point(3, 61);
            this.ReferenceJointCombobox.Name = "ReferenceJointCombobox";
            this.ReferenceJointCombobox.Size = new System.Drawing.Size(228, 28);
            this.ReferenceJointCombobox.TabIndex = 1;
            this.ReferenceJointCombobox.SelectedIndexChanged += new System.EventHandler(this.ReferenceJointCombobox_SelectedIndexChanged);
            // 
            // GearboxRatioTextbox
            // 
            this.GearboxRatioTextbox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GearboxRatioTextbox.Location = new System.Drawing.Point(125, 3);
            this.GearboxRatioTextbox.Name = "GearboxRatioTextbox";
            this.GearboxRatioTextbox.Size = new System.Drawing.Size(100, 26);
            this.GearboxRatioTextbox.TabIndex = 1;
            this.GearboxRatioTextbox.TextChanged += new System.EventHandler(this.GearboxRatioTextbox_TextChanged);
            // 
            // GearboxRatioLabel
            // 
            this.GearboxRatioLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.GearboxRatioLabel.AutoSize = true;
            this.GearboxRatioLabel.Location = new System.Drawing.Point(3, 6);
            this.GearboxRatioLabel.Name = "GearboxRatioLabel";
            this.GearboxRatioLabel.Size = new System.Drawing.Size(116, 20);
            this.GearboxRatioLabel.TabIndex = 0;
            this.GearboxRatioLabel.Text = "Gearbox Ratio:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.GearboxRatioLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.GearboxRatioTextbox, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(228, 32);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // ReferenceJointLabel
            // 
            this.ReferenceJointLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.ReferenceJointLabel.AutoSize = true;
            this.ReferenceJointLabel.Location = new System.Drawing.Point(51, 38);
            this.ReferenceJointLabel.Name = "ReferenceJointLabel";
            this.ReferenceJointLabel.Size = new System.Drawing.Size(131, 20);
            this.ReferenceJointLabel.TabIndex = 1;
            this.ReferenceJointLabel.Text = "Reference Label:";
            // 
            // GearBoxPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "GearBoxPanel";
            this.Size = new System.Drawing.Size(234, 92);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label ReferenceJointLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label GearboxRatioLabel;
        private System.Windows.Forms.TextBox GearboxRatioTextbox;
        private System.Windows.Forms.ComboBox ReferenceJointCombobox;
    }
}
