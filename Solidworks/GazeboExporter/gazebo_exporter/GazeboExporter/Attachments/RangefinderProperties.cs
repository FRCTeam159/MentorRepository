using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.UI;


namespace GazeboExporter.Robot
{
    public partial class RangefinderProperties : UserControl,AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for Rangefinder attachments
        /// </summary>
        public RangefinderProperties()
        {
            InitializeComponent();
        }

        Rangefinder currentRangefinder;

        /// <summary>
        /// Sets the current rangefinder that is linked to this page
        /// </summary>
        /// <param name="rangefinder">rangefinder to be used</param>
        public void setRangefinder(Rangefinder rangefinder)
        {
            currentRangefinder = rangefinder;
            analogChannel.Value = currentRangefinder.analogChannel;
            Radius.Text = (-currentRangefinder.FOV).ToString();
            MinDist.Text = currentRangefinder.MinDist.ToString();
            MaxDist.Text = currentRangefinder.MaxDist.ToString();
        }

        /// <summary>
        /// Updates the analog channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void analogChannel_ValueChanged(object sender, EventArgs e)
        {
            currentRangefinder.analogChannel = (int)analogChannel.Value;
        }


        /// <summary>
        /// Updates the radius value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Radius_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(Radius.Text, out num))
            {
                currentRangefinder.FOV = (num > 0) ? -num : num; // want to store as negative number, but display as positive number
            }

            Radius.Text = currentRangefinder.FOV.ToString();
        }

        /// <summary>
        /// Updates the minimum distance value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinDist_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MinDist.Text, out num))
            {
                currentRangefinder.MinDist = (num > 0) ? num : -num;
            }

            MinDist.Text = currentRangefinder.MinDist.ToString();
        }

        /// <summary>
        /// Updates the maximum distance value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaxDist_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MaxDist.Text, out num))
            {
                currentRangefinder.MaxDist = (num > 0) ? num : -num;
            }

            MaxDist.Text = currentRangefinder.MaxDist.ToString();
        }

        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentRangefinder.Delete();
        }

        /// <summary>
        /// Close "Manage Robot" window and open "Attachment Selection" panel to position rangefinder in visual window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttachmentPose_Click(object sender, EventArgs e)
        {
            string modelConfig = currentRangefinder.ChildLink.VisualModel.swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig == null && !PluginSettings.DebugMode)
            {
                string msg = "Visual components must be selected for the child link before selecting the Rangefinder pose ";
                MessageBox.Show(msg, "Error:No visuals defined", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modelConfig != null && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "The Rangefinder pose must be set in the same configuration as the visual components of the Child Link";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AttachmentPMPage page = new AttachmentPMPage(currentRangefinder, currentRangefinder.ChildLink.swApp);
            page.RestoreManager = ((ManageRobot)this.FindForm()).ExternalSelect;
            this.FindForm().Hide();
        }

        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = PluginSettings.SetToolTips();

            unitToolTip.SetToolTip(this.AttachmentPose, "Click to select objects in Solidworks window.");
            
            unitToolTip.SetToolTip(this.Radius, "Radius of FOV at 1 m distance. Must be positive (meters)");
            unitToolTip.SetToolTip(this.MaxDist, "Must be positive (meters)");
            unitToolTip.SetToolTip(this.MinDist, "Must be positive (meters)");
        } 
    } 
}
