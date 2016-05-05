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
    public partial class InternalLimitSwitchProperties : UserControl, AttachmentProperties
    {

        /// <summary>
        /// The property page to be used for Internal Limit Switch attachments
        /// </summary>
        public InternalLimitSwitchProperties()
        {
            InitializeComponent();
            foreach (String s in InternalLimitSwitch.AngleUnits)
            {
                units.Items.Add(s); //Combo Box Options
            }
        }

        InternalLimitSwitch currentIntLimSwitch;

        /// <summary>
        /// Sets the current internal limit switch that is linked to this page
        /// </summary>
        /// <param name="intLimitSwitch">internal limit switch to be used</param>
        public void setInternalLimSwitch(InternalLimitSwitch intLimSwitch)
        {
            currentIntLimSwitch = intLimSwitch;
            dioChannel.Value = currentIntLimSwitch.dioChannel;
            units.SelectedIndex = currentIntLimSwitch.units;
            maximum.Text = currentIntLimSwitch.MaxLimit.ToString();
            minimum.Text = currentIntLimSwitch.MinLimit.ToString();
            JointList.Items.Clear();
            JointList.Text = "";
            foreach (Joint j in currentIntLimSwitch.ChildLink.UpperJoints)
            {
                JointList.Items.Add(j.Name);
            }
            if (currentIntLimSwitch.Joint != null)
            {
                JointList.SelectedItem = currentIntLimSwitch.Joint.Name;
            }
            if (currentIntLimSwitch.Joint.jointSpecifics is SingleAxisJoint)
                units.Visible = ((SingleAxisJoint)currentIntLimSwitch.Joint.jointSpecifics).Axis1 is RotationalJointAxis;
            
        }

        /// <summary>
        /// Updates the digital channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dioChannel_ValueChanged(object sender, EventArgs e)
        {
            currentIntLimSwitch.dioChannel = (int)dioChannel.Value;
        }


        /// <summary>
        /// Updates the units when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void units_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentIntLimSwitch.units = units.SelectedIndex;
        }


        /// <summary>
        /// Updates the minimum limit value when exit the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void minimum_LostFocus(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(minimum.Text, out num))//try to read the input
            {
                if (num <= currentIntLimSwitch.MaxLimit) //if it is a number, check that number is valid
                    currentIntLimSwitch.MinLimit = num;
            }

            minimum.Text = currentIntLimSwitch.MinLimit.ToString(); //update with most recent valid value
            
        }


        /// <summary>
        /// Updates the maximum limit value exit the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void maximum_LostFocus(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(maximum.Text, out num))//try to read the input
            {
                if (num >= 0 && num >= currentIntLimSwitch.MinLimit) //if it is a number, check that number is valid
                currentIntLimSwitch.MaxLimit = num;
            }

            maximum.Text = currentIntLimSwitch.MaxLimit.ToString(); //revert to last valid value
        }

        
        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentIntLimSwitch.Delete();
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
                currentIntLimSwitch.SetJoint(currentIntLimSwitch.ChildLink.UpperJoints[index]);
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

            string selectedunits = "deg or rad";
            if (units.SelectedIndex != -1)
                selectedunits = units.Text;
            unitToolTip.SetToolTip(this.maximum, "Maximum angle to trigger the limit switch. Must be a real number (" + selectedunits + ")");
            unitToolTip.SetToolTip(this.minimum, "Minimum angle to trigger the limit switch. Must be a real number (" + selectedunits + ")");

        } 
    }
}
