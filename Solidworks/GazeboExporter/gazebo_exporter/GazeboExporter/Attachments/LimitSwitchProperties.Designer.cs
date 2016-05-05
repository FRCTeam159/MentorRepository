namespace GazeboExporter.Robot
{
    partial class LimitSwitchProperties
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
            this.dioChannel = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.dioChannel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(126, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Digital channel";
            // 
            // digital channel
            // 
            this.dioChannel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dioChannel.Location = new System.Drawing.Point(168, 25);
            this.dioChannel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dioChannel.Maximum = new decimal(new int[] {
            48,
            0,
            0,
            0});

            this.dioChannel.Name = "dioChannel";
            this.dioChannel.Size = new System.Drawing.Size(147, 26);
            this.dioChannel.TabIndex = 3;
            this.dioChannel.ValueChanged += new System.EventHandler(this.dioChannel_ValueChanged);
            // 
            // LimitSwitchProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.dioChannel);
            this.Controls.Add(this.label1);
            this.Name = "LimitSwitchProperties";
            this.Size = new System.Drawing.Size(338, 750);
            ((System.ComponentModel.ISupportInitialize)(this.dioChannel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown dioChannel;
    }
}
