using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using GazeboExporter.UI; 

namespace GazeboExporter.Robot
{
    public partial class RobotProperties : UserControl
    {
        /// <summary>
        /// The property page to be used for robots
        /// </summary>
        public RobotProperties()
        {
            InitializeComponent();
            URDFExportTypeButton.Enabled = false;
        }

        RobotModel robot;

        /// <summary>
        /// Sets the current robot that is linked to this page
        /// </summary>
        /// <param name="robot">robot to be used</param>
        /// <param name="model">model to be used</param>
        public void setRobot(RobotModel robot, ModelDoc2 model)
        {
            this.robot = robot;
            robotName.Text = robot.Name;

            string[] configurations = model.GetConfigurationNames();

            
            IncludeFRCfieldBox.Visible = PluginSettings.UseFRCsim;
            if (PluginSettings.UseFRCsim)
            {
                switch (robot.FRCfield)
                {
                    case 2014:
                        FRC2014FieldButton.Checked = true;
                        break;
                    case 2015:
                        FRC2015FieldButton.Checked = true;
                        break;
                    default:
                        NoFieldButton.Checked = true;
                        break;
                }
            }
            if (robot.ExportType == 0)
                SDFExportTypeButton.Checked = true;
            else
                URDFExportTypeButton.Checked = true;
        }

        /// <summary>
        /// Updates the name when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void robotName_TextChanged(object sender, EventArgs e)
        {
            robot.Name = robotName.Text;
        }

        

        /// <summary>
        /// when click SelectButton, go to SW window and left panel to visually select components
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectButton_Click(object sender, EventArgs e)
        {

            ManageRobot manager = ((ManageRobot)this.FindForm());
            robot.OpenRobotEditorPage(manager.robot, manager);
            this.FindForm().Hide();
        }

        /// <summary>
        /// set up tool tips 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = new ToolTip();
            unitToolTip.AutoPopDelay = 2000;
            unitToolTip.InitialDelay = 500;
            unitToolTip.ReshowDelay = 500;

            unitToolTip.SetToolTip(this.SelectButton, "Click to select objects in Solidworks window.");
        }

        private void ExportButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if(btn!= null && btn.Checked == true)
            {
               switch(btn.Name)
               {
                   case "SDFExportTypeButton":
                       robot.ExportType = 0;
                       break;
                   case "URDFExportTypeButton":
                       robot.ExportType = 1;
                       break;
                   default:
                       robot.ExportType = 0;
                       break;
               }
}
        }

        private void FieldButtons_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn != null && btn.Checked == true)
            {
                switch (btn.Name)
                {
                    case "FRC2015FieldButton":
                        robot.FRCfield = 2015;
                        break;
                    case "FRC2014FieldButton":
                        robot.FRCfield = 2014;
                        break;
                    case "NoFieldButton":
                        robot.FRCfield = 0;
                        break;
                    default:
                        robot.FRCfield = 0;
                        break;
                }
            }
        } 
    }
}
