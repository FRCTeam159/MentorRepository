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
    public partial class PistonProperties : UserControl, AttachmentProperties
    {

        /// <summary>
        /// The property page to be used for pistons attachments
        /// </summary>
        public PistonProperties()
        {
            InitializeComponent();
            
            foreach (String s in Piston.Directions)
            {
                direction.Items.Add(s); //Combo Box Options
            }
        }

        Piston currentPiston;

        /// <summary>
        /// Sets the current piston that is linked to this page
        /// </summary>
        /// <param name="piston">piston to be used</param>
        public void setPiston(Piston piston)
        {
            currentPiston = piston;
            dioChannel.Value = currentPiston.dioChannel;
            direction.SelectedIndex = currentPiston.Direction;
            forwardForce.Text = currentPiston.ForwardForce.ToString();
            reverseForce.Text = currentPiston.ReverseForce.ToString();
            JointList.Items.Clear();
            JointList.Text = "";
            foreach (Joint j in currentPiston.ChildLink.UpperJoints)
            {
                JointList.Items.Add(j.Name);
            }
            if (currentPiston.Joint != null)
            {
                JointList.SelectedItem = currentPiston.Joint.Name;
            }
        }

        /// <summary>
        /// Updates the channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void channel_ValueChanged(object sender, EventArgs e)
        {
            currentPiston.dioChannel = (int)dioChannel.Value;
        }


        /// <summary>
        /// Updates the piston direction when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void direction_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPiston.Direction = direction.SelectedIndex;
        }


        /// <summary>
        /// Updates the forward force value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void forwardForce_TextChanged(object sender, EventArgs e)
        {
            double num;

            // check that input is a number
            if (Double.TryParse(forwardForce.Text, out num))
            {
                currentPiston.ForwardForce = (num > 0) ? num : -num;
            }

            forwardForce.Text = currentPiston.ForwardForce.ToString();
        }


        /// <summary>
        /// Updates the reverse force value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reverseForce_TextChanged(object sender, EventArgs e)
        {
            double num;

            // check that input is a number
            if (Double.TryParse(reverseForce.Text, out num))
            {
                currentPiston.ReverseForce = (num > 0) ? num : -num;
            }

            reverseForce.Text = currentPiston.ReverseForce.ToString();
        }

        
        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentPiston.Delete();
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
                currentPiston.SetJoint(currentPiston.ChildLink.UpperJoints[index]);
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

            unitToolTip.SetToolTip(this.forwardForce, "Must be a positive number (Newtons)");
            unitToolTip.SetToolTip(this.reverseForce, "Must be a positive number (Newtons)");
        }
    }
}
