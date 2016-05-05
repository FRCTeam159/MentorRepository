using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{
    public partial class ExternalLimitSwitchProperties : UserControl, AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for External Limit Switch attachments
        /// </summary>
        public ExternalLimitSwitchProperties()
        {
            InitializeComponent();
        }

        ExternalLimitSwitch currentExtLimSwitch;

        /// <summary>
        /// Sets the current external limit switch that is linked to this page
        /// </summary>
        /// <param name="extLimitSwitch">external limit switch to be used</param>
        public void setExternalLimSwitch(ExternalLimitSwitch extLimSwitch)
        {
            currentExtLimSwitch = extLimSwitch;
            dioChannel.Value = currentExtLimSwitch.dioChannel;
        }

        /// <summary>
        /// Updates the digital input-output channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dioChannel_ValueChanged(object sender, EventArgs e)
        {
            currentExtLimSwitch.dioChannel = (int)dioChannel.Value;
        }


        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
           currentExtLimSwitch.Delete();
        }

        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadToolTips(object sender, EventArgs e)
        {

        }
    }
}
