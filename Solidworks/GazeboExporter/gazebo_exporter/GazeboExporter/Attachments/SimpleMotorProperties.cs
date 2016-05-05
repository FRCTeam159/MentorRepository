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
    public partial class SimpleMotorProperties : UserControl, AttachmentProperties
    {
        /// <summary>
        /// The property page to be used for motor attachments
        /// </summary>
        public SimpleMotorProperties()
        {
            InitializeComponent();
        }

        SimpleMotor motor;

        /// <summary>
        /// Sets the current motor that is linked to this page
        /// </summary>
        /// <param name="motor">motor to be used</param>
        public void setSimpleMotor(SimpleMotor motor)
        {
            this.motor = motor;
            pwmChannel.Value = motor.PwmChannel;
            multiplier.Text = motor.Multiplier.ToString();
            JointList.Items.Clear();
            JointList.Text = "";
            foreach (Joint j in motor.ChildLink.UpperJoints)
            {
                JointList.Items.Add(j.Name);
            }
            if (motor.Joint != null)
            {
                JointList.SelectedItem = motor.Joint.Name;
            }
        }

        /// <summary>
        /// Updates the PWM channel value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pwmChannel_ValueChanged(object sender, EventArgs e)
        {
            motor.PwmChannel = (int)pwmChannel.Value;
        }

        /// <summary>
        /// Updates the multiplier value when exit the textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multiplier_LostFocus(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(multiplier.Text, out num))//try to read the input
            {
                motor.Multiplier = (num > 0) ? num : -num; //check that number is positive
            }

            multiplier.Text = motor.Multiplier.ToString(); //revert to last valid value
        }


        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            motor.Delete();
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
                motor.SetJoint(motor.ChildLink.UpperJoints[index]);
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

            unitToolTip.SetToolTip(this.multiplier, "Must be a positive number (unitless)");
        }
    }
}
