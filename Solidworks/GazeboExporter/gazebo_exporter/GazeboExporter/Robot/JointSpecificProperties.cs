using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using GazeboExporter.UI;

namespace GazeboExporter.Robot
{
    public partial class JointSpecificProperties : UserControl
    {

        private Joint joint;

        public JointSpecificProperties(Joint j)
        {
            InitializeComponent();
            joint = j;
            JointTypeCombobox.Items.AddRange(JointFactory.GetTypesList());
            RefreshBoxes();
        }

        /// <summary>
        /// Adds a control to the panel
        /// </summary>
        /// <param name="cont">Control to be added</param>
        public void AddUserControl(UserControl cont)
        {
            if (cont == null)
                return;
            int currentLevel = TopLayout.RowCount-2;
            TopLayout.SetRow(RemoveButton, TopLayout.GetRow(RemoveButton) + 1);
            TopLayout.RowCount++;
            TopLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            TopLayout.Controls.Add(cont, 0,currentLevel);
            cont.Dock = DockStyle.Fill;

        }

        /// <summary>
        /// Refreshes the boxes
        /// </summary>
        public void RefreshBoxes()
        {
            JointNameLabel.Text = joint.Name;
            JointTypeCombobox.SelectedItem = joint.Type;
        }

        private void JointPoseButton_Click(object sender, EventArgs e)
        {
            string modelConfig = joint.Child.VisualModel.swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig == null && !PluginSettings.DebugMode)
            {
                string msg = "Visual components must be selected for the child link before selecting Joint Poses ";
                MessageBox.Show(msg, "Error:No visuals defined", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (modelConfig != null && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "Joint Pose must be set in the same configuration as the visual components of the Child Link";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ManageRobot manager = (ManageRobot)this.FindForm(); 
            joint.OpenJointEditorPage(manager.robot, manager);
            manager.Hide();
        }

        private void JointTypeCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!joint.Type.Equals((string)JointTypeCombobox.SelectedItem))
            {
                joint.Type = (string)JointTypeCombobox.SelectedItem;
                ((ManageRobot)this.FindForm()).ExternalSelect(joint);
            }

        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RobotInfo.WriteToLogFile("\nClicking Delete Joint button (JointSpecificProperties)");
            joint.Delete();
            ((ManageRobot)this.FindForm()).ExternalSelect(null);
            RobotInfo.WriteToLogFile("Successfully ran Delete Joint button (JointSpecificProperties)");
        }
    }
}
