using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    public partial class PropertyEditor : UserControl
    {
        public PropertyEditor()
        {
            InitializeComponent();
        }

        private Control currentEditor;

        public void setObject(ISelectable o)
        {
            objectTypeLabel.Text = o.GetName();
            if (currentEditor != null)
                HideProperties();
                this.tableLayoutPanel1.Controls.Remove(currentEditor);
            currentEditor = o.GetEditorControl();
            ((ManageRobot)this.FindForm()).DeleteCurrentSel = o.Delete;
            if (currentEditor != null)
            {
                currentEditor.Dock = DockStyle.Fill;
                this.tableLayoutPanel1.Controls.Add(currentEditor,0,0);
                if (currentEditor is AttachmentProperties)
                {
                    button1.Visible = true;
                }
                else
                {
                    button1.Visible = false;
                }
            }
        }

        public void HideProperties()
        {
            if (currentEditor is LinkProperties)
            {
                ((LinkProperties)currentEditor).HideProperty();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RobotInfo.WriteToLogFile("\nClicking Remove Attachment button (PropertyEditor)");
            ((AttachmentProperties)currentEditor).RemoveAttachment();
            ((ManageRobot)(this.FindForm())).ExternalSelect(null);
            RobotInfo.WriteToLogFile("Successfully ran Remove Attachment button (JointSpecificProperties)");
        }

        

    }
}
