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
    public partial class QuadEncoderProperties : UserControl,AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for quadrature encoder attachments
        /// </summary>
        public QuadEncoderProperties()
        {
            InitializeComponent();
        }

        QuadEncoder currentQuadEnc;

        /// <summary>
        /// Sets the current quadrature encoder that is linked to this page
        /// </summary>
        /// <param name="enc">encoder to be used</param>
        public void setQuadEncoder(QuadEncoder enc)
        {
            currentQuadEnc = enc;
            dioChannelA.Value = currentQuadEnc.DioPinA;
            dioChannelB.Value = currentQuadEnc.DioPinB;
            pulsesPerRev.Text = currentQuadEnc.TicksPerRev.ToString();
            JointList.Items.Clear();
            JointList.Text = "";
            foreach (Joint j in currentQuadEnc.ChildLink.UpperJoints)
            {
                JointList.Items.Add(j.Name);
            }
            if (currentQuadEnc.Joint != null)
            {
                JointList.SelectedItem = currentQuadEnc.Joint.Name;
            }
        }

        /// <summary>
        /// Updates the digital IO A channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dioChannelA_ValueChanged(object sender, EventArgs e)
        {
            currentQuadEnc.DioPinA = (int)dioChannelA.Value;
        }

        /// <summary>
        /// Updates the digital IO B channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dioChannelB_ValueChanged(object sender, EventArgs e)
        {
            currentQuadEnc.DioPinB = (int)dioChannelB.Value;
        }
        
        /// <summary>
        /// Updates the pulses per revolution value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pulsesPerRev_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(pulsesPerRev.Text, out num))
            {
                currentQuadEnc.TicksPerRev = (num > 0) ? num : -num;
            }

            pulsesPerRev.Text = currentQuadEnc.TicksPerRev.ToString();
        }

        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentQuadEnc.Delete();
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
                currentQuadEnc.SetJoint(currentQuadEnc.ChildLink.UpperJoints[index]);
            }
        }

        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = PluginSettings.SetToolTips();

            unitToolTip.SetToolTip(this.pulsesPerRev, "Must be positive (Pulses per rev/min)");

        } 
    }
}
