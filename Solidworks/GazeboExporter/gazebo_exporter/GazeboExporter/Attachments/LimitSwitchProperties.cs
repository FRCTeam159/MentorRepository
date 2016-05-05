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
    public partial class LimitSwitchProperties : UserControl, AttachmentProperties
    {
        public LimitSwitchProperties()
        {
            InitializeComponent();
        }

        LimitSwitch currentLimSwitch;

        public void setLimSwitch(LimitSwitch limSwitch)
        {
            currentLimSwitch = limSwitch;
            dioChannel.Value = currentLimSwitch.dioChannel;

        }

        private void dioChannel_ValueChanged(object sender, EventArgs e)
        {
            currentLimSwitch.dioChannel = (int)dioChannel.Value;
        }

        public void RemoveAttachment()
        {
            currentLimSwitch.Delete();
        }

        public void LoadToolTips(object sender, EventArgs e)
        {

        }

    }
}
