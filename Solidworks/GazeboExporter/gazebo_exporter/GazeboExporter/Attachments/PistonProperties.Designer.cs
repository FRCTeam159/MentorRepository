namespace GazeboExporter.Robot
{
    partial class PistonProperties
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
            this.direction = new System.Windows.Forms.ComboBox();
            this.forwardForce = new System.Windows.Forms.TextBox();
            this.reverseForce = new System.Windows.Forms.TextBox();
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
            this.label1.Location = new System.Drawing.Point(49, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Channel:";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Direction:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 20);
            this.label3.TabIndex = 2;
            this.label3.Text = "Forward Force:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 20);
            this.label4.TabIndex = 3;
            this.label4.Text = "Reverse Force:";
            // 
            // dioChannel
            // 
            this.dioChannel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dioChannel.Location = new System.Drawing.Point(126, 37);
            this.dioChannel.Name = "dioChannel";
            this.dioChannel.Size = new System.Drawing.Size(147, 26);
            this.dioChannel.TabIndex = 4;
            this.dioChannel.ValueChanged += new System.EventHandler(this.channel_ValueChanged);
            // 
            // direction
            // 
            this.direction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.direction.FormattingEnabled = true;
            this.direction.Location = new System.Drawing.Point(126, 69);
            this.direction.Name = "direction";
            this.direction.Size = new System.Drawing.Size(147, 28);
            this.direction.TabIndex = 5;
            this.direction.SelectedIndexChanged += new System.EventHandler(this.direction_SelectedIndexChanged);
            // 
            // forwardForce
            // 
            this.forwardForce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.forwardForce.Location = new System.Drawing.Point(126, 103);
            this.forwardForce.Name = "forwardForce";
            this.forwardForce.Size = new System.Drawing.Size(147, 26);
            this.forwardForce.TabIndex = 6;
            this.forwardForce.LostFocus += new System.EventHandler(this.forwardForce_TextChanged);
            // 
            // reverseForce
            // 
            this.reverseForce.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reverseForce.Location = new System.Drawing.Point(126, 135);
            this.reverseForce.Name = "reverseForce";
            this.reverseForce.Size = new System.Drawing.Size(147, 26);
            this.reverseForce.TabIndex = 7;
            this.reverseForce.LostFocus += new System.EventHandler(this.reverseForce_TextChanged);
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(73, 7);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 20);
            this.label5.TabIndex = 9;
            this.label5.Text = "Joint:";
            // 
            // JointList
            // 
            this.JointList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.JointList.FormattingEnabled = true;
            this.JointList.Location = new System.Drawing.Point(126, 3);
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
            this.tableLayoutPanel1.Controls.Add(this.dioChannel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.JointList, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.direction, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.forwardForce, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.reverseForce, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(276, 164);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // PistonProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "PistonProperties";
            this.Size = new System.Drawing.Size(276, 164);
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
        private System.Windows.Forms.ComboBox direction;
        private System.Windows.Forms.TextBox forwardForce;
        private System.Windows.Forms.TextBox reverseForce;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox JointList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

    }
}