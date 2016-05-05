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
    /// <summary>
    /// Control containing the limit properties of an axis
    /// </summary>
    public partial class AxialJointLimitsProperties : UserControl
    {

        IJointAxis axis;

        /// <summary>
        /// Creates a new property limit control for the desired axis
        /// </summary>
        /// <param name="axis">The axis this control should be referencing</param>
        public AxialJointLimitsProperties(IJointAxis axis)
        {
            InitializeComponent();
            this.axis = axis;
            if (axis is LinearJointAxis)
                ToggleContinuousCheckbox(false);
                
            else
                ToggleContinuousCheckbox(true);

            ToggleLimitButton(true);
            ManualLimitsCheckbox.Checked = axis.UseCustomMovementLimits;
            RefreshBoxes();
        }

        /// <summary>
        /// Toggles if the limit selection button should be visable
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleLimitButton(bool enabled)
        {
            LimitSelectionButton.Visible = enabled;
            ManualLimitsCheckbox.Visible = enabled;
        }

        /// <summary>
        /// Toggles if the continuous checkbox should be visable
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleContinuousCheckbox(bool enabled)
        {
            ContinousCheckbox.Visible = enabled;
            if (enabled)
                ContinousCheckbox.Checked = ((RotationalJointAxis)axis).IsContinuous;
        }

        /// <summary>
        /// Refreshes the values in all textboxes
        /// </summary>
        public void RefreshBoxes()
        {
            UpperLimitTextbox.Text = axis.UpperLimit.ToString();
            LowerLimitTextbox.Text = axis.LowerLimit.ToString();
            ToggleManualLimits(ManualLimitsCheckbox.Checked);
            if (ContinousCheckbox.Checked)
                ToggleContinuous(ContinousCheckbox.Checked);
        }

        /// <summary>
        /// Toggles whether manual limits hsould be enabled or not
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleManualLimits(bool enabled)
        {
            UpperLimitTextbox.Enabled = enabled;
            LowerLimitTextbox.Enabled = enabled;
            LimitSelectionButton.Enabled = !enabled;
        }

        /// <summary>
        /// toggle whether the joint is continous
        /// </summary>
        /// <param name="isCont"></param>
        private void ToggleContinuous(bool isCont)
        {
            UpperLimitTextbox.Enabled = !isCont;
            LowerLimitTextbox.Enabled = !isCont;
            LimitSelectionButton.Enabled = !isCont && !ManualLimitsCheckbox.Checked;
            ManualLimitsCheckbox.Enabled = !isCont;
            if (isCont)
            {
                UpperLimitTextbox.Text = "no limit";
                LowerLimitTextbox.Text = "no limit";
            }
            else
            {
                UpperLimitTextbox.Text = axis.UpperLimit.ToString();
                LowerLimitTextbox.Text = axis.LowerLimit.ToString();
            }
        }

        /// <summary>
        /// Called when the upper limit textbox changes. Assigns the new value if it valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpperLimitTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(UpperLimitTextbox.Text, out num))
            {
                axis.UpperLimit = num;
            }

            
        }

        /// <summary>
        /// Called when the lower limit textbox changes. Assigns the new value if it valid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LowerLimitTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(LowerLimitTextbox.Text, out num))
            {
                axis.LowerLimit = num;
            }
        }

        /// <summary>
        /// Toggles controls when the manual textbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualLimitsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            axis.UseCustomMovementLimits = ManualLimitsCheckbox.Checked;
            ToggleManualLimits(ManualLimitsCheckbox.Checked);
        }

        /// <summary>
        /// Toggles controls when the continous textbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContinousCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ((RotationalJointAxis)axis).IsContinuous = ContinousCheckbox.Checked;
            ToggleContinuous(ContinousCheckbox.Checked);
        }

        /// <summary>
        /// Launches the limit selection page when clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LimitSelectionButton_Click(object sender, EventArgs e)
        {
            string modelConfig = axis.owner.Child.VisualModel.swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig == null && !PluginSettings.DebugMode)
            {
                string msg = "Visual components must be selected for the child link before selecting Joint Limits ";
                MessageBox.Show(msg, "Error:No visuals defined", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modelConfig != null && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "Joint Limits must be set in the same configuration as the visual components of the Child Link";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ManageRobot manager = (ManageRobot)this.FindForm();
            axis.owner.OpenJointEditorPage(manager.robot, manager);
            manager.Hide();
        }

        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = PluginSettings.SetToolTips();

            unitToolTip.SetToolTip(this.UpperLimitTextbox, "Must be positive (meters or radians).");
            unitToolTip.SetToolTip(this.LowerLimitTextbox, "Must be negative (meters or radians).");

        }
    }
}
