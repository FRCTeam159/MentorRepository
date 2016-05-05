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
    public partial class JointProperties : UserControl
    {
        public JointProperties()
        {
            InitializeComponent();
            jointType.Items.AddRange(Joint.JointTypes);
            this.ParentChanged += new EventHandler(this.Parent_Changed);
        }

        /// <summary>
        /// update fields in JointProperties panel when change joints. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Parent_Changed(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                MimicJointCombobox.Items.Clear();
                foreach (Link l in ((ManageRobot)this.Parent.FindForm()).robot.Links.Values.ToArray())
                {
                    /*if (l.ParentConnection != null && 
                        l.ParentConnection.Type == currentJoint.Type && 
                        !l.ParentConnection.Name.Equals(currentJoint.Name))
                        MimicJointCombobox.Items.Add(l.ParentConnection.Name);*/
                }
                if (currentJoint.IsMimic)
                {
                    MimicJointCombobox.SelectedItem = currentJoint.MimicJoint;

                    MimicMultiplierTextbox.Text = currentJoint.MimicMultiplier.ToString();
                    MimicOffsetTextbox.Text = currentJoint.MimicOffset.ToString();

                    MotionUpperTextBox.Text = "";
                    MotionLowerTextBox.Text = "";
                    EffortTextBox.Text = "";
                    VelocityTextBox.Text = "";
                }
                else
                {
                    MimicJointCombobox.SelectedIndex = -1;
                    MimicMultiplierTextbox.Text = "";
                    MimicOffsetTextbox.Text = "";

                    if (currentJoint.Type == (int)Joint.JointType.Revolute || currentJoint.Type == (int)Joint.JointType.Prismatic)
                    {
                        EffortTextBox.Text = currentJoint.EffortLimit.ToString();
                        VelocityTextBox.Text = currentJoint.VelocityLimit.ToString();
                        
                        if (currentJoint.UseCustomMovementLimits)
                        {
                            MotionUpperTextBox.Text = currentJoint.UpperLimit.ToString();
                            MotionLowerTextBox.Text = currentJoint.LowerLimit.ToString();
                        }
                    }
                }
                FrictionTextBox.Text = currentJoint.Friction.ToString();
                DampingTextBox.Text = currentJoint.Damping.ToString();
                            
            }
            
            
        }

        Joint currentJoint;



        /// <summary>
        /// Sets the current joint that is linked to this page
        /// </summary>
        /// <param name="j">joint to be used</param>
        public void setJoint(Joint j)
        {
            currentJoint = j;
            jointType.SelectedIndex = j.Type;

            DampingTextBox.Text = j.Damping.ToString();
            FrictionTextBox.Text = j.Friction.ToString();
            MimicCheckbox.Checked = currentJoint.IsMimic;
            MimicGroupBox.Enabled = MimicCheckbox.Checked;

            MotionCheckbox.Checked = currentJoint.UseCustomMovementLimits;
            jointType_SelectedIndexChanged(null, null);


        }

        /// <summary>
        /// Updates joint type, 
        /// which fields of JointProperties panel are visible,
        /// and combobox options of joints to mimic 
        /// when change joint type in dropdown 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void jointType_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentJoint.Type = jointType.SelectedIndex;
            NameLabel2.Text = currentJoint.Name;

            // jointType, jointTypeLabel, JointGroupBox, MimicCheckbox, MimicGroupBox, JointPose, MotionGroupBox, and PhysicalGroupBox always visible = true;

            if (currentJoint.Type == (int)Joint.JointType.Prismatic)
            {
                bool setManually = MotionCheckbox.Checked;

                MotionCheckbox.Visible = true;
                JointPose.Visible = !setManually;
                JointPose.Text = "Axis and Limits Pose"; //Axis and limits
                
                MotionUpperTextBox.Visible = setManually;
                MotionUpperLabel.Visible = setManually;
                MotionLowerTextBox.Visible = setManually;
                MotionLowerLabel.Visible = setManually;
            }
            else if (currentJoint.Type == (int)Joint.JointType.Screw)
            {
                bool setManually = MotionCheckbox.Checked;

                MotionCheckbox.Visible = true;
                JointPose.Text = "Axis and Limits Pose"; //Axis and limits

                MotionUpperTextBox.Visible = setManually;
                MotionUpperLabel.Visible = setManually;
                MotionLowerTextBox.Visible = setManually;
                MotionLowerLabel.Visible = setManually;
            }
            else if (currentJoint.Type == (int)Joint.JointType.Revolute ||
                    currentJoint.Type == (int)Joint.JointType.Revolute2 ||
                    currentJoint.Type == (int)Joint.JointType.Gearbox ||
                    currentJoint.Type == (int)Joint.JointType.Universal)
            {
                MotionCheckbox.Visible = false;
                JointPose.Text = "Axes Pose";

                MotionUpperTextBox.Visible = true;
                MotionUpperLabel.Visible = true;
                MotionLowerTextBox.Visible = true;
                MotionLowerLabel.Visible = true;
            }
            else if (currentJoint.Type == (int)Joint.JointType.Ball)
            {
                MotionCheckbox.Visible = false;
                JointPose.Text = "Origin Pose";

                MotionUpperTextBox.Visible = false;
                MotionUpperLabel.Visible = false;
                MotionLowerTextBox.Visible = false;
                MotionLowerLabel.Visible = false;
            }
            /*
            switch (currentJoint.Type)
            {
                case (int)Joint.JointType.Prismatic:
                    //DampingLabel.Visible = true;
                    //DampingTextBox.Visible = true;
                    //FrictionLabel.Visible = true;
                    //FrictionTextBox.Visible = true;
                    JointGroupBox.Visible = true;

                    MimicCheckbox.Visible = true;
                    MimicGroupBox.Visible = true;

                    MotionCheckbox.Visible = true;
                    MotionGroupBox.Visible = true;
                    bool setManually = MotionCheckbox.Checked;
                    JointPose.Visible = !setManually;
                    JointPose.Text = "Limits Pose"; //Axis and limits
                    MotionUpperTextBox.Visible = setManually;
                    MotionUpperLabel.Visible = setManually;
                    MotionLowerTextBox.Visible = setManually;
                    MotionLowerLabel.Visible = setManually;

                    PhysicalGroupBox.Visible = true;
                    break;
                case (int)Joint.JointType.Screw:
                    
                    JointPose.Visible = true;
                    JointPose.Text = "Axis Pose"; //Axis and limits
                    MotionUpperTextBox.Visible = true;
                    MotionUpperLabel.Visible = true;
                    MotionLowerTextBox.Visible = true;
                    MotionLowerLabel.Visible = true;
                    break;
                case (int)Joint.JointType.Revolute:
                    JointGroupBox.Visible = true;

                    MimicCheckbox.Visible = true;
                    MimicGroupBox.Visible = true;

                    MotionCheckbox.Visible = false;
                    MotionGroupBox.Visible = true;
                    JointPose.Visible = true;
                    JointPose.Text = "Axis Pose";
                    MotionUpperTextBox.Visible = true;
                    MotionUpperLabel.Visible = true;
                    MotionLowerTextBox.Visible = true;
                    MotionLowerLabel.Visible = true;

                    PhysicalGroupBox.Visible = true;
                    break;

                case (int)Joint.JointType.Gearbox:
                    break;
                case (int)Joint.JointType.Revolute2:
                    break;
                case (int)Joint.JointType.Universal:
                    break;
                case (int)Joint.JointType.Ball:
                    break;
                default:
                    break;
            
            }
 */
        }

        /// <summary>
        /// Updates the damping value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DampingTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(DampingTextBox.Text, out num))
            {
                currentJoint.Damping = num;
            }
        }

        /// <summary>
        /// Updates the friction value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrictionTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(FrictionTextBox.Text, out num))
            {
                currentJoint.Friction = num;
            }
        }

        /// <summary>
        /// Updates the mimic property and the visible fields of the JointProperty page when the MimicCheckbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MimicCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentJoint.IsMimic = MimicCheckbox.Checked;
            
            MimicGroupBox.Enabled = MimicCheckbox.Checked;
            MotionCheckbox.Enabled = !MimicCheckbox.Checked;
            MotionGroupBox.Enabled = !MimicCheckbox.Checked;
            PhysicalGroupBox.Enabled = !MimicCheckbox.Checked; 

            if (MimicCheckbox.Checked)
            {
                MimicMultiplierTextbox.Text = currentJoint.MimicMultiplier.ToString();
                MimicOffsetTextbox.Text = currentJoint.MimicOffset.ToString();

                MotionUpperTextBox.Text = currentJoint.UpperLimit.ToString();
                MotionLowerTextBox.Text = currentJoint.LowerLimit.ToString();
                EffortTextBox.Text = currentJoint.EffortLimit.ToString();
                VelocityTextBox.Text = currentJoint.VelocityLimit.ToString();
                ///NEED TO GET MIMIC TO UPDATE OTHER FIELDS: HELPER FUNCTION!
            }
            else
            {
                MimicMultiplierTextbox.Clear();
                MimicOffsetTextbox.Clear();
                MimicJointCombobox.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Updates which joint is mimicked when the combobox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MimicJointCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentJoint.MimicJoint = (string)MimicJointCombobox.SelectedItem;
        }

        /// <summary>
        /// Updates the multiplier value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MimicMultiplierTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MimicMultiplierTextbox.Text, out num))
            {
                currentJoint.MimicMultiplier = num;
            }
        }

        /// <summary>
        /// Updates the offset value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MimicOffsetTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MimicOffsetTextbox.Text, out num))
            {
                currentJoint.MimicOffset = num;
            }
        }


        /// <summary>
        /// Updates the custom motion limits property and the visible fields of the JointProperty page when the MotionCheckbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentJoint.UseCustomMovementLimits = MotionCheckbox.Checked;
            JointPose.Visible = !MotionCheckbox.Checked;
            MotionUpperTextBox.Visible = MotionCheckbox.Checked;
            MotionLowerTextBox.Visible = MotionCheckbox.Checked;
            MotionUpperLabel.Visible = MotionCheckbox.Checked;
            MotionLowerLabel.Visible = MotionCheckbox.Checked;

            if (MotionCheckbox.Checked)
            {
                MotionUpperTextBox.Text = currentJoint.UpperLimit.ToString();
                MotionLowerTextBox.Text = currentJoint.LowerLimit.ToString();
            }
            else
            {
                MotionUpperTextBox.Clear();
                MotionLowerTextBox.Clear();
            }
        }

        /// <summary>
        /// Updates the upper motion limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionUpperTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MotionUpperTextBox.Text, out num))
            {
                currentJoint.UpperLimit = num;
            }
        }

        /// <summary>
        /// Updates the lower motion limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionLowerTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MotionLowerTextBox.Text, out num))
            {
                currentJoint.LowerLimit = num;
            }
        }
        
        /// <summary>
        /// Updates the effort limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EffortTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(EffortTextBox.Text, out num))
            {
                currentJoint.EffortLimit = num;
            }
        }
        
        /// <summary>
        /// Updates the velocity limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VelocityTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(VelocityTextBox.Text, out num))
            {
                currentJoint.VelocityLimit = num;
            }
        }



        /// <summary>
        /// Switches to the Robot Editor panel when button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JointPose_Click(object sender, EventArgs e)
        {
            
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentJoint.openJointEditorPage(manager.robot, manager);
            this.FindForm().Hide();



        }
    }
}
