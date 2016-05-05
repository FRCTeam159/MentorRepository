using System;

namespace GazeboExporter
{
    partial class ProgressLogForm
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
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.currentOperation = new System.Windows.Forms.Label();
            this.operationLog = new System.Windows.Forms.RichTextBox();
            this.button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(14, 65);
            this.progressBar.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(673, 29);
            this.progressBar.Step = 1;
            this.progressBar.TabIndex = 0;
            // 
            // currentOperation
            // 
            this.currentOperation.AutoSize = true;
            this.currentOperation.Location = new System.Drawing.Point(14, 26);
            this.currentOperation.Name = "currentOperation";
            this.currentOperation.Size = new System.Drawing.Size(148, 20);
            this.currentOperation.TabIndex = 1;
            this.currentOperation.Text = "<current operation>";
            // 
            // operationLog
            // 
            this.operationLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.operationLog.BackColor = System.Drawing.Color.White;
            this.operationLog.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.operationLog.Location = new System.Drawing.Point(14, 120);
            this.operationLog.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.operationLog.Name = "operationLog";
            this.operationLog.ReadOnly = true;
            this.operationLog.Size = new System.Drawing.Size(672, 359);
            this.operationLog.TabIndex = 2;
            this.operationLog.TabStop = false;
            this.operationLog.Text = "";
            // 
            // button
            // 
            this.button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button.Location = new System.Drawing.Point(546, 488);
            this.button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button.Name = "button";
            this.button.Size = new System.Drawing.Size(141, 39);
            this.button.TabIndex = 3;
            this.button.Text = "Button2";
            this.button.UseVisualStyleBackColor = true;
            this.button.Click += new System.EventHandler(this.button_Click_1);
            // 
            // ProgressLogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 541);
            this.Controls.Add(this.button);
            this.Controls.Add(this.operationLog);
            this.Controls.Add(this.currentOperation);
            this.Controls.Add(this.progressBar);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(717, 586);
            this.Name = "ProgressLogForm";
            this.ShowIcon = false;
            this.Text = "<operation name>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ProgressLogForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        internal void dump(object C, object appData, object roaming, object gazeboExporter, object log)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label currentOperation;
        private System.Windows.Forms.RichTextBox operationLog;
        private System.Windows.Forms.Button button;
    }
}