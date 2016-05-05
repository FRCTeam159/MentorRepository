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
    /// <summary>
    /// The property page to be used for Gyro attachments
    /// </summary>
    public partial class GyroProperties : UserControl,AttachmentProperties
    {
        /// <summary>
        /// Constructor, setups the page
        /// </summary>
        public GyroProperties()
        {
            InitializeComponent();
            foreach (String s in Gyro.Axes)
            {
                gyroAxis.Items.Add(s); //Combo Box Options
            }
        }

        private Gyro currentGyro;

        /// <summary>
        /// Sets the current gyro that is linked to this page
        /// </summary>
        /// <param name="gyro">Gyro to be used</param>
        public void SetGyro(Gyro gyro)
        {
            currentGyro = gyro;
            analogChannel.Value = currentGyro.AnalogChannel;
            gyroAxis.SelectedIndex = currentGyro.Axis;
        }

        /// <summary>
        /// Updates the analog channel when the value changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void analogChannel_ValueChanged(object sender, EventArgs e)
        {
            currentGyro.AnalogChannel = (int)analogChannel.Value;
        }

        /// <summary>
        /// Updates the gyro axis when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gyroAxis_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentGyro.Axis = gyroAxis.SelectedIndex;
        }

        /// <summary>
        /// removes the attachment from the robot 
        /// </summary>
        public void RemoveAttachment()
        {
            currentGyro.Delete();
        }

        /// <summary>
        /// set up tool tips to show units
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadToolTips(object sender, EventArgs e)
        {

        }
    }
}
