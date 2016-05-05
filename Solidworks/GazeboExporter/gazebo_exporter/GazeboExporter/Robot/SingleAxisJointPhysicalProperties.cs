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
    public partial class SingleAxisJointPhysicalProperties : UserControl
    {

        private IJointAxis axis;


        public SingleAxisJointPhysicalProperties(IJointAxis axis)
        {
            InitializeComponent();
            this.axis = axis;
            RefreshBoxes();
        }

        /// <summary>
        /// Refreshes the values in all boxes
        /// </summary>
        public void RefreshBoxes()
        {
            Axis1DampingTextbox.Text = axis.Damping.ToString();
            Axis1FrictionTextbox.Text = axis.Friction.ToString();
            Axis1EffortTextbox.Text = axis.EffortLimit.ToString();
        }

        private void Axis1DampingTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axis.Damping = num;
            }
        }

        private void Axis1FrictionTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axis.Friction = num;
            }
        }

        private void Axis1EffortTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axis.EffortLimit = num;
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

            unitToolTip.SetToolTip(this.Axis1FrictionTextbox, "Static friction. Must be positive (Newtons)");
            unitToolTip.SetToolTip(this.Axis1EffortTextbox, "Max force/torque load. Must be positive (Newtons or Newton-meters)");
            unitToolTip.SetToolTip(this.Axis1DampingTextbox, "Damping. Must be positive (Newton-seconds/meter)");
        }

    }
}
