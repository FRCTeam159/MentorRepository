using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    public partial class JointProperties2 : UserControl
    {
        public JointProperties2()
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
                
                if (currentJoint.Axes[0].IsContinuous)
                {
                    MotionUpperTextBox.Text = "no motion limit";
                    MotionLowerTextBox.Text = "no motion limit";
                }
                else
                {
                    MotionUpperTextBox.Text = currentJoint.Axes[0].UpperLimit.ToString();
                    MotionLowerTextBox.Text = currentJoint.Axes[0].LowerLimit.ToString();
                }
                    
                    
                EffortTextBox.Text = currentJoint.Axes[0].EffortLimit.ToString();
                VelocityTextBox.Text = currentJoint.Axes[0].VelocityLimit.ToString();
                FrictionTextBox.Text = currentJoint.Axes[0].Friction.ToString();
                DampingTextBox.Text = currentJoint.Axes[0].Damping.ToString();

                Effort2TextBox.Text = currentJoint.Axes[1].EffortLimit.ToString();
                Velocity2TextBox.Text = currentJoint.Axes[1].VelocityLimit.ToString();
                Friction2TextBox.Text = currentJoint.Axes[1].Friction.ToString();
                Damping2TextBox.Text = currentJoint.Axes[1].Damping.ToString();

                GearboxRatioTextbox.Text = currentJoint.GearboxRatio.ToString();
                ThreadPitchTextbox.Text = currentJoint.ThreadPitch.ToString();

            }
            else
            {
                MotionUpperTextBox.Text = "";
                MotionLowerTextBox.Text = "";
                EffortTextBox.Text = "";
                VelocityTextBox.Text = "";
                GearboxRatioTextbox.Text = "";
                ThreadPitchTextbox.Text = "";
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

            if ((currentJoint.Type == (int)Joint.JointType.Revolute 
                || currentJoint.Type == (int)Joint.JointType.Revolute2 
                || currentJoint.Type == (int)Joint.JointType.Gearbox) && 
                (!currentJoint.UseCustomMovementLimits && !currentJoint.Axes[0].IsContinuous))
            {
                currentJoint.UseCustomMovementLimits = true;
            }

            NameLabel2.Text = j.Name;
            
            if (j.Axes[0] != null)
            {
                DampingTextBox.Text = j.Axes[0].Damping.ToString();
                FrictionTextBox.Text = j.Axes[0].Friction.ToString();

            }
            if (j.Axes[1] != null)
            {
                Damping2TextBox.Text = j.Axes[1].Damping.ToString();
                Friction2TextBox.Text = j.Axes[1].Friction.ToString();

            }

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

            // NameLabel, NameLabel2, jointType, jointTypeLabel,  are always visible = true;

            Axis1Label.Visible = true;          //false for Ball
            JointPose.Visible = true;           //false for Ball
            JointGroupBox.Visible = true;       //false for Ball
            ExtraGroupBox.Visible = false;      //true for Screw and Gearbox
            
            {
                //Velocity limits NOT implemented in Gazebo!
                PhysicalGroupBox.Visible = false;   
                VelocityLabel.Visible = false;
                VelocityTextBox.Visible = false;
                Velocity2TextBox.Visible = false;
            }


            switch(currentJoint.Type)
            {                
                case (int)Joint.JointType.Prismatic:
                    //allow to select limits visually but not set limits as continuous
                    JointPose.Text = "Axis and Limits Pose";

                    ShowLimitOptions(true, true, false);
                    ShowAxis2Options(false);
                    currentJoint.Axes[0].IsContinuous = false;
                    break;

                case (int)Joint.JointType.Screw:
                    // Thread Pitch option
                    ExtraGroupBox.Visible = true;
                    MimicJointLabel.Visible = false;
                    MimicJointCombobox.Visible = false;
                    GearboxRatioLabel.Visible = false;
                    GearboxRatioTextbox.Visible = false;
                    ThreadPitchLabel.Visible = true;
                    ThreadPitchTextbox.Visible = true;
                    currentJoint.Axes[0].IsContinuous = false;

                    //allow to select limits visually but not set limits as continuous
                    JointPose.Text = "Axis and Limits Pose";

                    ShowLimitOptions(true, true, false);
                    ShowAxis2Options(false);
                    break;

                case (int)Joint.JointType.Gearbox:
                    JointPose.Text = "Axis Pose";              
                    // mimic selection and gearbox ratio property
                    ExtraGroupBox.Visible = true;
                    MimicJointLabel.Visible = true;
                    MimicJointCombobox.Visible = true;
                    setMimicOptions();
                    GearboxRatioLabel.Visible = true;
                    GearboxRatioTextbox.Visible = true;
                    ThreadPitchLabel.Visible = false;
                    ThreadPitchTextbox.Visible = false;

                    JointGroupBox.Visible = true;

                    ShowLimitOptions(false, false, false);
                    ShowAxis2Options(false);
                    currentJoint.Axes[0].IsContinuous = true;
                    //ContinuousCheckbox.Checked = false; // mimic other joint
                    break;

                case (int)Joint.JointType.Revolute:
                    //Axis limit properties for 1 rotational axis
                    MotionGroupBox.Text = "Axis Motion Limit Properties";
                    JointPose.Text = "Axis Pose";

                    ShowLimitOptions(false, false, true);
                    ShowAxis2Options(false);
                    break;

                case (int)Joint.JointType.Revolute2:
                    JointPose.Text = "Axes Pose";

                    //currentJoint.UseCustomMovementLimits = true;
                    ShowLimitOptions(false, false, true); 
                    ShowAxis2Options(true);
                    break;

                case (int)Joint.JointType.Universal:
                    JointPose.Text = "Axes Pose";    

                    ShowLimitOptions(false, false, false);
                    ShowAxis2Options(true);
                    currentJoint.Axes[0].IsContinuous = true;

                    break;
                case (int)Joint.JointType.Ball:
                    JointPose.Text = "Origin Pose";
                    JointGroupBox.Visible = true;
                    Axis1Label.Visible = false;
                    JointGroupBox.Visible = false;
            
                    ShowLimitOptions(false, false, false);
                    ShowAxis2Options(false);
                    break;
                default:
                    break;
            }

        }
        /// <summary>
        /// helper function to be used in jointType_SelectedIndexChanged to show/hide a set of fields
        /// </summary>
        /// <param name="show">true if there are axis 2 options to set; false otherwise</param>
        private void ShowAxis2Options(bool show)
        {
            Axis2Label.Visible = show;
            Damping2TextBox.Visible = show;
            Friction2TextBox.Visible = show;
            Effort2TextBox.Visible = show;
            

            if (show) //clarify which axis the limits apply to 
            {
                MotionGroupBox.Text = "Axis 1 Motion Limit Properties";
            }
            else
            {
                MotionGroupBox.Text = "Axis Motion Limit Properties";
            }
        }
        /// <summary>
        /// helper function to be used in jointType_SelectedIndexChanged to show/hide a set of fields
        /// </summary>
        /// <param name="showButton">true if selecting limits in visual SW window is an option</param>
        /// <param name="showManualCheckbox">true if inputting limit values manually is an option</param>
        /// <param name="showContinuousCheckbox">true if axis1 can be continuous</param>
        private void ShowLimitOptions(bool showButton, bool showManualCheckbox, bool showContinuousCheckbox)
        {
            //Prismatic and screw = visual select OR manual values
            //Revolute = manual values OR continuous
            //Revolute 2 = manual values only 
            //Gearbox, Universal = None

            if (!showButton && !showManualCheckbox && !showContinuousCheckbox) //Universal and ball = limits defined automatically not by user
            {
                MotionGroupBox.Visible = false;
                MotionCheckbox.Checked = false;
                return;
            }

            if (showButton) //Prismatic and screw = visual select OR manual values
            {

                LimitsPose.Visible = true;
                MotionCheckbox.Visible = true;
                ContinuousCheckbox.Visible = false;
            }
            else if (showContinuousCheckbox) //Revolute and gearbox = manual values OR continuous
            {
                LimitsPose.Visible = false;
                MotionCheckbox.Visible = false;
                ContinuousCheckbox.Visible = true;
            }
            else //Revolute 2 = manual values only 
            {
                LimitsPose.Visible = false;
                MotionCheckbox.Visible = false;
                MotionCheckbox.Checked = true;
                ContinuousCheckbox.Visible = false;
            }

            //allow to input manual limit values
            MotionGroupBox.Visible = true;
            MotionUpperTextBox.Visible = true;
            MotionUpperLabel.Visible = true;
            MotionLowerTextBox.Visible = true;
            MotionLowerLabel.Visible = true;

            bool setManually = currentJoint.UseCustomMovementLimits;
            LimitsPose.Enabled = !setManually;
            MotionCheckbox.Checked = setManually;
            MotionUpperTextBox.Enabled = setManually;
            MotionUpperLabel.Enabled = setManually;
            MotionLowerTextBox.Enabled = setManually;
            MotionLowerLabel.Enabled = setManually;

            ContinuousCheckbox.Checked = currentJoint.Axes[0].IsContinuous;
        }


        #region Joint/limit property handlers
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
                currentJoint.Axes[0].Damping = num;
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
                currentJoint.Axes[0].Friction = num;
            }
        }

        /// <summary>
        /// Updates the damping value for axis 2 when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Damping2TextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Damping2TextBox.Text, out num))
            {
                currentJoint.Axes[1].Damping = num;
            }
        }

        /// <summary>
        /// Updates the friction value for axis 2 when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Friction2TextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Friction2TextBox.Text, out num))
            {
                currentJoint.Axes[1].Friction = num;
            }
        }


        /// <summary>
        /// Updates the multiplier value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GearboxRatioTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(GearboxRatioTextbox.Text, out num) && num > 0)
            {
                currentJoint.GearboxRatio = num;
            }
        }

        /// <summary>
        /// Updates the offset value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThreadPitchTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(ThreadPitchTextbox.Text, out num))
            {
                currentJoint.ThreadPitch = num;
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

            MotionUpperLabel.Enabled = MotionCheckbox.Checked;
            MotionLowerLabel.Enabled = MotionCheckbox.Checked;
            MotionUpperTextBox.Enabled = MotionCheckbox.Checked;
            MotionLowerTextBox.Enabled = MotionCheckbox.Checked;
            
            LimitsPose.Enabled = !MotionCheckbox.Checked;
            //ContinuousCheckbox.Enabled = !MotionCheckbox.Checked;

            if (MotionCheckbox.Checked)
            {
                MotionUpperTextBox.Text = currentJoint.Axes[0].UpperLimit.ToString();
                MotionLowerTextBox.Text = currentJoint.Axes[0].LowerLimit.ToString();
            }
            else
            {
                MotionUpperTextBox.Clear();
                MotionLowerTextBox.Clear();
            }
        }

        /// <summary>
        /// Updates the limit values and property page if ContinuousCheckbox is changed
        /// A continuous joint has no motion limits (denoted by null)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinuousCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentJoint.Axes[0].IsContinuous = ContinuousCheckbox.Checked;
            currentJoint.UseCustomMovementLimits = !ContinuousCheckbox.Checked;

            MotionUpperLabel.Enabled = !ContinuousCheckbox.Checked;
            MotionLowerLabel.Enabled = !ContinuousCheckbox.Checked;
            MotionUpperTextBox.Enabled = !ContinuousCheckbox.Checked;
            MotionLowerTextBox.Enabled = !ContinuousCheckbox.Checked;

            //LimitsPose.Enabled = !ContinuousCheckbox.Checked;
            //MotionCheckbox.Enabled = !ContinuousCheckbox.Checked;

            if (ContinuousCheckbox.Checked)
            {
                MotionUpperTextBox.Text = "no limits";
                MotionLowerTextBox.Text = "no limits";
                
            }
            else
            {
                MotionUpperTextBox.Text = currentJoint.Axes[0].UpperLimit.ToString();
                MotionLowerTextBox.Text = currentJoint.Axes[0].LowerLimit.ToString();
                //MotionCheckbox.Checked = false;
            }
        }

        /// <summary>
        /// Updates the upper motion limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionUpperTextBox_LostFocus(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MotionUpperTextBox.Text, out num))
                currentJoint.Axes[0].UpperLimit = (num > 0) ? num : -num; //upper limit must be positive

            MotionUpperTextBox.Text = currentJoint.Axes[0].UpperLimit.ToString();
        }

        /// <summary>
        /// Updates the lower motion limit value when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MotionLowerTextBox_LostFocus(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MotionLowerTextBox.Text, out num))
                currentJoint.Axes[0].LowerLimit = (num < 0) ? num : -num; //lower limit must be negative

            MotionLowerTextBox.Text = currentJoint.Axes[0].LowerLimit.ToString();
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
                currentJoint.Axes[0].EffortLimit = num;
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
                currentJoint.Axes[0].VelocityLimit = (num > 0) ? num : -num ;
            }

            VelocityTextBox.Text = currentJoint.Axes[0].VelocityLimit.ToString();
        }

        /// <summary>
        /// Updates the effort limit value for axis 2 when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Effort2TextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Effort2TextBox.Text, out num))
            {
                currentJoint.Axes[1].EffortLimit = num;
            }
        }

        /// <summary>
        /// Updates the velocity limit value for axis 2 when the textbox is changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Velocity2TextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Velocity2TextBox.Text, out num))
            {
                currentJoint.Axes[1].VelocityLimit = num;
            }
        }

        #endregion

        /// <summary>
        /// Switches to the Robot Editor panel when button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JointPose_Click(object sender, EventArgs e)
        {
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentJoint.OpenJointEditorPage(manager.robot, manager);
            this.FindForm().Hide();
        }

        /// <summary>
        /// Switches to the Robot Editor panel when button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitsPose_Click(object sender, EventArgs e)
        {
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentJoint.OpenJointEditorPage(manager.robot, manager);
            this.FindForm().Hide();
        }


        #region Mimic functions/handlers
        /// <summary>
        /// for gearbox joints, updates gearbox reference when combobox changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MimicJointCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MimicJointCombobox.SelectedIndex != -1)
            {
                    foreach (Joint J in currentJoint.Parent.ChildJoints)
                        if (J.Name == MimicJointCombobox.Text) 
                        {
                            currentJoint.GearboxReference = J;
                            return;
                        }
                    foreach (Joint J in currentJoint.Parent.ParentJoints)
                        if (J.Name == MimicJointCombobox.Text) 
                        {
                            currentJoint.GearboxReference = J;
                            return;
                        }
            }
        }

        /// <summary>
        /// finds all joints that the current joint can mimic 
        //conditions: 
        //- Valid option must be a joint connected to parent link
        //- Valid option must be rotational joint along an axis
        //- Valid option must have axis parallel to this gearbox joint axis
        /// </summary>
        public void setMimicOptions()
        {
            MimicJointCombobox.Items.Clear();
            MimicJointCombobox.Text = "";

            if (currentJoint.Axes[0].Axis == null)
            {
                MimicJointCombobox.Text = "No axis for this joint. Choose an axis first";
                return;
            }

            //find jointoptions joint options
            List<Joint> jointoptions = new List<Joint>();

            try
            {
                foreach (Joint j in currentJoint.Parent.ParentJoints)
                {
                    if ((j.Type == (int)Joint.JointType.Revolute ||
                            j.Type == (int)Joint.JointType.Gearbox ) &&
                        AreParallelAxes(j.Axes[0], currentJoint.Axes[0]))
                        jointoptions.Add(j);
                }
                foreach (Joint j in currentJoint.Parent.ChildJoints)
                {
                    if (!j.Equals(currentJoint) &&
                        (j.Type == (int)Joint.JointType.Revolute || 
                            j.Type == (int)Joint.JointType.Gearbox ) &&
                        AreParallelAxes(j.Axes[0], currentJoint.Axes[0]) )
                        jointoptions.Add(j);
                }

                // Add valid options to combobox
                foreach (Joint j in jointoptions)
                    MimicJointCombobox.Items.Add(j.Name);
            }
            catch (InvalidOperationException e)
            {
            }
            if (jointoptions.Count == 0)
            {
                MimicJointCombobox.Text = "No valid options with parallel axis";
                return;
            }
            if (jointoptions.Contains(currentJoint.GearboxReference))
            {
                MimicJointCombobox.SelectedIndex = jointoptions.IndexOf(currentJoint.GearboxReference);
            }
        }

        /// <summary>
        /// determine if axis of two Joints are parallel
        /// </summary>
        /// <param name="axis1"></param>
        /// <param name="axis2"></param>
        /// <returns></returns>
        private static bool AreParallelAxes(JointAxis axis1, JointAxis axis2)
        {
            Vector3D v1 = new Vector3D(axis1.AxisX, axis1.AxisY, axis1.AxisZ);
            Vector3D v2 = new Vector3D(axis2.AxisX, axis2.AxisY, axis2.AxisZ);

            double crossProduct = Vector3D.CrossProduct(v1, v2).Length;
            return (Math.Abs(crossProduct) <= 0.0000001 ); //if cross product = 0;
        }


        #endregion
        
        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = new ToolTip();
            unitToolTip.AutoPopDelay = 2000;
            unitToolTip.InitialDelay = 500;
            unitToolTip.ReshowDelay = 500;

            unitToolTip.SetToolTip(this.JointPose, "Click to select objects in Solidworks window.");
            unitToolTip.SetToolTip(this.LimitsPose, "Click to select objects in Solidworks window.");
            
            unitToolTip.SetToolTip(this.DampingTextBox, "Must be positive (Newton-seconds)");
            unitToolTip.SetToolTip(this.Damping2TextBox, "Must be positive (Newton-seconds)");
            unitToolTip.SetToolTip(this.Friction2TextBox, "Must be positive (Newtons)");
            unitToolTip.SetToolTip(this.FrictionTextBox, "Must be positive (Newtons)");

            string effortunits;
            string limitunits;
            switch (jointType.SelectedIndex)
            {
                case (int)Joint.JointType.Prismatic:
                case (int)Joint.JointType.Screw:
                    limitunits = "meters";
                    effortunits = "Newtons";
                    break;
                case (int)Joint.JointType.Revolute:
                case (int)Joint.JointType.Gearbox:
                case (int)Joint.JointType.Revolute2:
                case (int)Joint.JointType.Universal:
                case (int)Joint.JointType.Ball:
                    limitunits = "radians";
                    effortunits = "Newton-meters";
                    break;
                case -1:
                default:
                    limitunits = "meters or radians";
                    effortunits = "Newtons or Newton-meters";
                    break;
            }
            unitToolTip.SetToolTip(this.EffortTextBox, "Max effort that can be applied. Must be positive (" + effortunits + ")");
            unitToolTip.SetToolTip(this.Effort2TextBox, "Max effort that can be applied. Must be positive (" + effortunits + ")");

            unitToolTip.SetToolTip(this.MotionUpperTextBox, "Must be positive (" + limitunits + ")");
            unitToolTip.SetToolTip(this.MotionLowerTextBox, "Must be negative (" + limitunits + ")");

            unitToolTip.SetToolTip(this.GearboxRatioTextbox, "Must be positive (unitless)");
            unitToolTip.SetToolTip(this.ThreadPitchTextbox, "Must be positive (radians per meter)");
        } 

    }
}
