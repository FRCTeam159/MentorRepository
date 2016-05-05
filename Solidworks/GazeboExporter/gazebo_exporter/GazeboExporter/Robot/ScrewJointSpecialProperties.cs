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
    public partial class ScrewJointSpecialProperties : UserControl
    {
        private ScrewJointAxis axis;

        public ScrewJointSpecialProperties(ScrewJointAxis axis)
        {
            InitializeComponent();
            this.axis = axis;
            ThreadPitchTextBox.Text = axis.ThreadPitch.ToString();
        }

        private void ThreadPitchTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axis.ThreadPitch = num;
            }
        }
        
        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadToolTips(object sender, EventArgs e)
        {
            ToolTip unitToolTip = PluginSettings.SetToolTips();

            unitToolTip.SetToolTip(this.ThreadPitchTextBox, "Must be positive (radians per meters).");

        }
    }
}
