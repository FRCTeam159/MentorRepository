namespace GazeboExporter.Robot
{
    partial class InternalLimitSwitchProperties
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dioChannel = new System.Windows.Forms.NumericUpDown();
            this.units = new System.Windows.Forms.ComboBox();
            this.maximum = new System.Windows.Forms.TextBox();
            this.minimum = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.JointList = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.dioChannel)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Digital channel:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Units:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(34, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Maximum: ";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Minimum:";
            // 
            // dioChannel
            // 
            this.dioChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dioChannel.Location = new System.Drawing.Point(124, 37);
            this.dioChannel.Name = "dioChannel";
            this.dioChannel.Size = new System.Drawing.Size(147, 26);
            this.dioChannel.TabIndex = 4;
            this.dioChannel.ValueChanged += new System.EventHandler(this.dioChannel_ValueChanged);
            // 
            // units
            // 
            this.units.Dock = System.Windows.Forms.DockStyle.Fill;
            this.units.FormattingEnabled = true;
            this.units.Location = new System.Drawing.Point(124, 69);
            this.units.Name = "units";
            this.units.Size = new System.Drawing.Size(147, 28);
            this.units.TabIndex = 5;
            this.units.SelectedIndexChanged += new System.EventHandler(this.units_SelectedIndexChanged);
            // 
            // maximum
            // 
            this.maximum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maximum.Location = new System.Drawing.Point(124, 103);
            this.maximum.Name = "maximum";
            this.maximum.Size = new System.Drawing.Size(147, 26);
            this.maximum.TabIndex = 6;
            this.maximum.LostFocus += new System.EventHandler(this.maximum_LostFocus);
            // 
            // minimum
            // 
            this.minimum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minimum.Location = new System.Drawing.Point(124, 135);
            this.minimum.Name = "minimum";
            this.minimum.Size = new System.Drawing.Size(147, 26);
            this.minimum.TabIndex = 7;
            this.minimum.LostFocus += new System.EventHandler(this.minimum_LostFocus);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(71, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Joint:";
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(124, 3);
            this.JointList.Name = "JointList";
            this.JointList.Size = new System.Drawing.Size(147, 28);
            this.JointList.TabIndex = 8;
            this.JointList.SelectedIndexChanged += new System.EventHandler(this.JointList_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.minimum, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.maximum, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dioChannel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.units, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(274, 164);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // InternalLimitSwitchProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "InternalLimitSwitchProperties";
            this.Size = new System.Drawing.Size(274, 164);
            this.Load += new System.EventHandler(this.LoadToolTips);
            ((System.ComponentModel.ISupportInitialize)(this.dioChannel)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion



        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown dioChannel;
        private System.Windows.Forms.ComboBox units;
        private System.Windows.Forms.TextBox maximum;
        private System.Windows.Forms.TextBox minimum;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}