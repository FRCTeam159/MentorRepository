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
    public partial class CameraProperties : UserControl,AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for Camera attachments
        /// </summary>
        public CameraProperties()
        {
            InitializeComponent();
        }

        Camera currentCamera;

        /// <summary>
        /// Sets the current camera that is linked to this page
        /// </summary>
        /// <param name="camera">camera to be used</param>
        public void setCamera(Camera camera)
        {
            currentCamera = camera;
            FOVbox.Text = currentCamera.FOV.ToString();
        }

        /// <summary>
        /// Updates the FOV value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FOV_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(FOVbox.Text, out num))
            {
                currentCamera.FOV = (num > 0) ? num : -num; 
            }

            FOVbox.Text = currentCamera.FOV.ToString();
        }

        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentCamera.Delete();
        }

        /// <summary>
        /// Close "Manage Robot" window and open "Attachment Selection" panel to position camera in visual window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttachmentPose_Click(object sender, EventArgs e)
        {
            string modelConfig = currentCamera.ChildLink.VisualModel.swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig == null && !PluginSettings.DebugMode)
            {
                string msg = "Visual components must be selected for the child link before selecting the Camera pose ";
                MessageBox.Show(msg, "Error:No visuals defined", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modelConfig != null && configs.Contains(modelConfig) &&!modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "The Camera pose must be set in the same configuration as the visual components of the Child Link";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AttachmentPMPage page = new AttachmentPMPage(currentCamera, currentCamera.ChildLink.swApp);
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
            unitToolTip.SetToolTip(this.FOVbox, "Angle range of FOV. Must be positive (degrees)");
            
        } 
    }
}
