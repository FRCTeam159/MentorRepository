using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GazeboExporter.UI
{
    public partial class SettingsForm : Form
    {
        ManageRobot manager;

        public SettingsForm(ManageRobot manager)
        {
            InitializeComponent();
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.manager = manager;
            useFRCsimCheckbox.Checked = PluginSettings.UseFRCsim;
            DebugCheckbox.Checked = PluginSettings.DebugMode;
            logCheckbox.Checked = PluginSettings.Log;
        }

        private void useFRCsimCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            PluginSettings.UseFRCsim = useFRCsimCheckbox.Checked;
            manager.toggleFRCsim(useFRCsimCheckbox.Checked);
        }

        private void DebugCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            PluginSettings.DebugMode = DebugCheckbox.Checked;
            RobotInfo.WriteToLogFile("Toggling Debug Mode in Exporter Settings");
        }

        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void logCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            PluginSettings.Log = logCheckbox.Checked;
        }
    }
}
