namespace GazeboExporter.UI
{
    partial class LinkSelector
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
            this.linkSelectTree = new System.Windows.Forms.TreeView();
            this.addLinkButton = new System.Windows.Forms.Button();
            this.removeLinkButton = new System.Windows.Forms.Button();
            this.editLinkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // linkSelectTree
            // 
            this.linkSelectTree.AllowDrop = true;
            this.linkSelectTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.linkSelectTree.FullRowSelect = true;
            this.linkSelectTree.HideSelection = false;
            this.linkSelectTree.Location = new System.Drawing.Point(0, 0);
            this.linkSelectTree.Name = "linkSelectTree";
            this.linkSelectTree.Size = new System.Drawing.Size(300, 350);
            this.linkSelectTree.TabIndex = 0;
            this.linkSelectTree.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.linkSelectTree_ItemDrag);
            this.linkSelectTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.linkSelectTree_AfterSelect);
            this.linkSelectTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.linkSelectTree_NodeMouseClick);
            this.linkSelectTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.linkSelectTree_DragDrop);
            this.linkSelectTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.linkSelectTree_DragEnter);
            this.linkSelectTree.DragOver += new System.Windows.Forms.DragEventHandler(this.linkSelectTree_DragOver);
            this.linkSelectTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.linkSelectTree_KeyDown);
            // 
            // addLinkButton
            // 
            this.addLinkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.addLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.addLinkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addLinkButton.Location = new System.Drawing.Point(77, 355);
            this.addLinkButton.Name = "addLinkButton";
            this.addLinkButton.Size = new System.Drawing.Size(45, 45);
            this.addLinkButton.TabIndex = 1;
            this.addLinkButton.Text = "+";
            this.addLinkButton.UseVisualStyleBackColor = true;
            this.addLinkButton.Click += new System.EventHandler(this.addLinkButton_Click);
            // 
            // removeLinkButton
            // 
            this.removeLinkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.removeLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeLinkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeLinkButton.Location = new System.Drawing.Point(128, 355);
            this.removeLinkButton.Name = "removeLinkButton";
            this.removeLinkButton.Size = new System.Drawing.Size(45, 45);
            this.removeLinkButton.TabIndex = 2;
            this.removeLinkButton.Text = "-";
            this.removeLinkButton.UseVisualStyleBackColor = true;
            this.removeLinkButton.Click += new System.EventHandler(this.removeLinkButton_Click);
            // 
            // editLinkButton
            // 
            this.editLinkButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.editLinkButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.editLinkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editLinkButton.Location = new System.Drawing.Point(179, 355);
            this.editLinkButton.Name = "editLinkButton";
            this.editLinkButton.Size = new System.Drawing.Size(45, 45);
            this.editLinkButton.TabIndex = 3;
            this.editLinkButton.Text = "E";
            this.editLinkButton.UseVisualStyleBackColor = true;
            this.editLinkButton.Click += new System.EventHandler(this.editLinkButton_Click);
            // 
            // LinkSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.Controls.Add(this.editLinkButton);
            this.Controls.Add(this.removeLinkButton);
            this.Controls.Add(this.addLinkButton);
            this.Controls.Add(this.linkSelectTree);
            this.Name = "LinkSelector";
            this.Size = new System.Drawing.Size(300, 400);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView linkSelectTree;
        private System.Windows.Forms.Button addLinkButton;
        private System.Windows.Forms.Button removeLinkButton;
        private System.Windows.Forms.Button editLinkButton;
    }
}
