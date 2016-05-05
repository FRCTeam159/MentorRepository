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
    public partial class GearBoxPanel : UserControl
    {
        private GearboxJoint joint;

        public GearBoxPanel(GearboxJoint joint)
        {
            InitializeComponent();
            this.joint = joint;
            joint.FillJointSelector(ReferenceJointCombobox);
            GearboxRatioTextbox.Text = joint.GearboxRatio.ToString();
        }

        private void ReferenceJointCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ReferenceJointCombobox.SelectedItem is Joint)
                joint.referenceJoint = (Joint)ReferenceJointCombobox.SelectedItem;
        }

        private void GearboxRatioTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;

            if (Double.TryParse(((TextBox)sender).Text, out num))
            {
                joint.GearboxRatio = num;
            }
        }
    }
}
