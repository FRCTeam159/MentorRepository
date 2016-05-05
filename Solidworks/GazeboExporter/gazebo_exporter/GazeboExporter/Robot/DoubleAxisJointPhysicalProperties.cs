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
    public partial class DoubleAxisJointPhysicalProperties : UserControl
    {

        private IJointAxis[] axes;


        public DoubleAxisJointPhysicalProperties(IJointAxis[] axes)
        {
            InitializeComponent();
            this.axes = axes;
            RefreshBoxes();
        }

        /// <summary>
        /// Refreshes the values in all boxes
        /// </summary>
        public void RefreshBoxes()
        {
            Axis1DampingTextbox.Text = axes[0].Damping.ToString();
            Axis1FrictionTextbox.Text = axes[0].Friction.ToString();
            Axis1EffortTextbox.Text = axes[0].EffortLimit.ToString();
            Axis2DampingTextbox.Text = axes[1].Damping.ToString();
            Axis2FrictionTextbox.Text = axes[1].Friction.ToString();
            Axis2EffortTextbox.Text = axes[1].EffortLimit.ToString();
        }

        private void Axis1DampingTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[0].Damping = num;
            }
        }

        private void Axis1FrictionTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[0].Friction = num;
            }
        }

        private void Axis1EffortTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[0].EffortLimit = num;
            }
        }

        private void Axis2DampingTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[1].Damping = num;
            }
        }

        private void Axis2FrictionTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[1].Friction = num;
            }
        }

        private void Axis2EffortTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                axes[1].EffortLimit = num;
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
            unitToolTip.SetToolTip(this.Axis2FrictionTextbox, "Static friction. Must be positive (Newtons)");
            unitToolTip.SetToolTip(this.Axis2EffortTextbox, "Max force/torque load. Must be positive (Newtons or Newton-meters)");
            unitToolTip.SetToolTip(this.Axis2DampingTextbox, "Damping. Must be positive (Newton-seconds/meter)");
        }
    }
}
