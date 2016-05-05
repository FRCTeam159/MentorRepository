using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    public delegate void DeleteCurrentSelection();

    public partial class ManageRobot : Form
    {
        public DeleteCurrentSelection DeleteCurrentSel;

        private TabPage AttPage;

        public ManageRobot()
        {
            InitializeComponent();
            
            this.TopMost = false; 
        }

        public RobotModel robot {get;set;}

        public ManageRobot(RobotModel robot)
        {
            RobotInfo.WriteToLogFile("Creating Robot Manager (ManageRobot)");
            this.robot = robot;
            InitializeComponent();
            robotGraphPanel1.setRobot(robot);
            robotGraphPanel1.OnSelectionChanged += Select;
            this.TopMost = false;
            Select(robot);
            toggleFRCsim(PluginSettings.UseFRCsim);
            RobotInfo.WriteToLogFile("Robot Manager Created (ManageRobot)");
        }

        public void ExternalSelect(ISelectable o)
        {
            if (o == null)
            {
                o = robot;
            }
            robotGraphPanel1.select(o);
            Select(o);
            if (this.Visible != true)
            {
                this.Show();
            }
        }

        public void Select(ISelectable o)
        {
            System.Diagnostics.Debug.WriteLine("Selecting Object");
            propertyEditor1.setObject(o);
            System.Diagnostics.Debug.WriteLine("Done.");
        }

        private void ManageRobot_FormClosing(object sender, FormClosingEventArgs e)
        {
            RobotInfo.WriteToLogFile("Closing Manage Robot form (ManageRobot)\n");
            if (e.CloseReason == CloseReason.UserClosing)
            {
                propertyEditor1.HideProperties();
                this.Hide();
                e.Cancel = true;
            }
        }

        public int getAttachmentTabIndex()
        {
            return 1;// this.AttachmentTabControl1.SelectedIndex;
        }

        public new void Hide()
        {
            ExternalSelect(null);
            base.Hide();
        }

        /// <summary>
        /// Refreshes the graph panel
        /// </summary>
        public void RefreshGraphPanel()
        {
            robotGraphPanel1.Refresh();
        }

        /// <summary>
        /// toggles the attachment selection panel
        /// </summary>
        /// <param name="enabled">whether the panel should be enabled</param>
        public void toggleFRCsim(bool enabled)
        {
            if (enabled)
            {
                attachmentSelector1.Visible = true;
            }
            else
                attachmentSelector1.Visible = false;
            if(this.Visible == true)
                ExternalSelect(null);
            RobotInfo.WriteToLogFile("Toggling FRCsim Components " + enabled.ToString() + " in Exporter Settings (ManageRobot)");
        }

        private void KeyPress_Handler(object sender, KeyEventArgs e)
        {
            /*if (DeleteCurrentSel != null && e.KeyCode.Equals(Keys.Delete) && Visible)
            {
                DeleteCurrentSel();
                ExternalSelect(null);
            }*/
        }        
    }
}
