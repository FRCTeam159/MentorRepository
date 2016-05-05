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
    public partial class PotentiometerProperties : UserControl,AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for potentiometer attachments
        /// </summary>
        public PotentiometerProperties()
        {
            InitializeComponent();
        }

        Potentiometer currentPot;

        /// <summary>
        /// Sets the current potentiometer that is linked to this page
        /// </summary>
        /// <param name="pot">poentiometer to be used</param>
        public void setPot(Potentiometer pot)
        {
            currentPot = pot;
            analogChannel.Value = currentPot.AnalogChannel;
            JointList.Items.Clear();
            JointList.Text = "";
            foreach (Joint j in currentPot.ChildLink.UpperJoints)
            {
                JointList.Items.Add(j.Name);
            }
            if (currentPot.Joint != null)
            {
                JointList.SelectedItem = currentPot.Joint.Name;
            }
        }

        /// <summary>
        /// Updates the analog channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void analogChannel_ValueChanged(object sender, EventArgs e)
        {
            currentPot.AnalogChannel = (int)analogChannel.Value;
        }
        
        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentPot.Delete();
        }

        /// <summary>
        /// updates which joint this attachment is attached to when the dropdown box changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JointList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = JointList.SelectedIndex;
            if (index != -1)
            {
                currentPot.SetJoint(currentPot.ChildLink.UpperJoints[index]);
            }
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
