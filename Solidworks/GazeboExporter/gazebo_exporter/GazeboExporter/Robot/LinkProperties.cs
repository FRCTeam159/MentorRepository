using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace GazeboExporter.UI
{
    public partial class LinkProperties : UserControl,SwManipulatorHandler2
    {
        //the manipulator for the COM
        TriadManipulator triadManip;
        Manipulator swManip;
        //starting origin of mainulator at the begining of the drag
        MathPoint startOrigin;
        //flag for whether or not to reset the start origin of the triad
        bool updateStartOrigin = true;


        /// <summary>
        /// The property page to be used for links
        /// </summary>
        public LinkProperties()
        {
            InitializeComponent();

        }

        Link currentLink;
        ManageRobot robotManager;

        /// <summary>
        /// Sets the current link of this page
        /// </summary>
        /// <param name="l">link to be used</param>
        public void setLink(Link l)
        {
            
            currentLink = l;
            nameTextBox.Text = currentLink.Name;
            changeColor.BackColor = Color.FromArgb(currentLink.ColorRed, currentLink.ColorGreen, currentLink.ColorBlue);

            Mu1Textbox.Text = currentLink.Mu1.ToString();
            Mu2Textbox.Text = currentLink.Mu2.ToString();
            KpTextbox.Text = currentLink.Kp != 0? currentLink.Kp.ToString() : "";
            KdTextbox.Text = currentLink.Kd != 0? currentLink.Kd.ToString() : "";
            LinearDampingTextbox.Text = currentLink.LinearDamping != 0 ? currentLink.LinearDamping.ToString() : "";
            AngularDampingTextbox.Text = currentLink.AngularDamping != 0 ? currentLink.AngularDamping.ToString() : "";
            ToggleCollisionValues();


            selfCollideCheckbox.Checked = currentLink.SelfCollide;
            CustomMassValCheckbox.Checked = currentLink.UseCustomInertial;
            toggleCOMTextboxes();
            InertiaValuesUpdateTextbox();

            PhysicalButton.Enabled = !CustomMassValCheckbox.Checked;

            RemoveButton.Enabled = !currentLink.isBaseLink;


        }

        /// <summary>
        /// update fields in JointProperties panel when change joints. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Parent_Changed(object sender, EventArgs e)
        {
            if (Parent != null)
                robotManager = (ManageRobot)this.Parent.FindForm();
        }

        public void HideProperty()
        {
            swManip = null;
            triadManip = null;
            GC.Collect();
        }

        /// <summary>
        /// Updates the name value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentLink != null)
                currentLink.Name = this.nameTextBox.Text;
        }

        /// <summary>
        /// Updates the name value if it is unique when leave the name textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nameTextBox_LostFocus(object sender, EventArgs e)
        {
            Link[] arrayoflinks = robotManager.robot.GetLinksAsArray();
            if (currentLink == null || arrayoflinks == null)
                return;

            {
                //check that link does not have same name as any other link
                foreach (Link L in arrayoflinks)
                {
                    if (L != currentLink && nameTextBox.Text == L.Name)
                    {
                        MessageBox.Show(this, "Link must have a unique name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        nameTextBox.Text = currentLink.Name;
                        return;
                    }
                }
            
                //if unique name, save it in Link
                currentLink.Name = this.nameTextBox.Text;
            }
        }

        /// <summary>
        /// Updates the color value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeColor_Click(object sender, EventArgs e)
        {
            colorPicker.Color = changeColor.BackColor;
            if (colorPicker.ShowDialog() == DialogResult.OK)
            {
                currentLink.ColorRed = colorPicker.Color.R;
                currentLink.ColorGreen = colorPicker.Color.G;
                currentLink.ColorBlue = colorPicker.Color.B;
                changeColor.BackColor = colorPicker.Color;
            }
        }

        /// <summary>
        /// Updates the mu 1 value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mu1Textbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Mu1Textbox.Text, out num))
            {
                currentLink.Mu1 = num;
            }
        }

        /// <summary>
        /// Updates the mu 2 value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Mu2Textbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(Mu2Textbox.Text, out num))
            {
                currentLink.Mu2 = num;
            }
        }

        /// <summary>
        /// Updates the K_p value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KpTextBox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(KpTextbox.Text, out num))
            {
                currentLink.Kp = num;
            }
            else if (String.IsNullOrWhiteSpace(KpTextbox.Text))
                currentLink.Kp = 0;
        }

        /// <summary>
        /// Updates the K_d value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KdTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(KdTextbox.Text, out num))
            {
                currentLink.Kd = num;
            }
            else if (String.IsNullOrWhiteSpace(KdTextbox.Text))
                currentLink.Kd = 0;
        }

        /// <summary>
        /// Updates the linear damping values when it changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinearDampningTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(LinearDampingTextbox.Text, out num))
            {
                currentLink.LinearDamping = num;
            }
            else if (String.IsNullOrWhiteSpace(LinearDampingTextbox.Text))
                currentLink.LinearDamping = 0;
        }

        /// <summary>
        /// Updates the angular damping value when it changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AngularDampingTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(AngularDampingTextbox.Text, out num))
            {
                currentLink.AngularDamping = num;
            }
            else if (String.IsNullOrWhiteSpace(AngularDampingTextbox.Text))
                currentLink.AngularDamping = 0;
        }

        /// <summary>
        /// Toggles custom value textboxes when checkbox changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CustomMassValCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentLink.UseCustomInertial = CustomMassValCheckbox.Checked;
            PhysicalButton.Enabled = !CustomMassValCheckbox.Checked;
            toggleCOMTextboxes();

            if (CustomMassValCheckbox.Checked)
            {
                swManip = currentLink.modelDoc.ModelViewManager.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, this);
                triadManip = swManip.GetSpecificManipulator();
                triadManip.DoNotShow = (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYRING | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZRING 
                                        | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXRING | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYPlane |
                                        (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZPlane | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXPlane;
                triadManip.Origin = currentLink.swApp.GetMathUtility().CreatePoint(new double[] { currentLink.ComX, currentLink.ComY, currentLink.ComZ });
                startOrigin = triadManip.Origin;
                triadManip.UpdatePosition();
                swManip.Show(currentLink.modelDoc);
            }
            else
            {
                InertiaButton_Click(null, null);

                HideProperty();
            }
        }

        private void ToggleCollisionValues()
        {
            bool enabled = !currentLink.CollisionModel.EmptyModel;
            Mu1Textbox.Enabled = enabled;
            Mu2Textbox.Enabled = enabled;
            KpTextbox.Enabled = enabled;
            KdTextbox.Enabled = enabled;
        }
        
        /// <summary>
        /// Updates textboxes to enable or disable depending on whether the checkbox is checked
        /// </summary>
        private void toggleCOMTextboxes()
        {
            ComXLabel.Enabled = CustomMassValCheckbox.Checked;
            ComXTextbox.Enabled = CustomMassValCheckbox.Checked;
            ComYLabel.Enabled = CustomMassValCheckbox.Checked;
            ComYTextbox.Enabled = CustomMassValCheckbox.Checked;
            ComZLabel.Enabled = CustomMassValCheckbox.Checked;
            ComZTextbox.Enabled = CustomMassValCheckbox.Checked;
            MassLabel.Enabled = CustomMassValCheckbox.Checked;
            MassTextbox.Enabled = CustomMassValCheckbox.Checked;
            IxxLabel.Enabled = CustomMassValCheckbox.Checked;
            IxxTextbox.Enabled = CustomMassValCheckbox.Checked;
            IxyLabel.Enabled = CustomMassValCheckbox.Checked;
            IxyTextbox.Enabled = CustomMassValCheckbox.Checked;
            IxzLabel.Enabled = CustomMassValCheckbox.Checked;
            IxzTextbox.Enabled = CustomMassValCheckbox.Checked;
            IyyLabel.Enabled = CustomMassValCheckbox.Checked;
            IyyTextbox.Enabled = CustomMassValCheckbox.Checked;
            IyzLabel.Enabled = CustomMassValCheckbox.Checked;
            IyzTextbox.Enabled = CustomMassValCheckbox.Checked;
            IzzLabel.Enabled = CustomMassValCheckbox.Checked;
            IzzTextbox.Enabled = CustomMassValCheckbox.Checked;
        }

        /// <summary>
        /// Updates the inertia value textboxes. Called after calculating values when custom checkbox not checked.  
        /// </summary>
        private void InertiaValuesUpdateTextbox()
        {
            int decimals = 8;
            ComXTextbox.Text = currentLink.ComX.ToString();
            ComYTextbox.Text = currentLink.ComY.ToString();
            ComZTextbox.Text = currentLink.ComZ.ToString();
            MassTextbox.Text = currentLink.Mass.ToString();
            IxxTextbox.Text = currentLink.MomentIxx.ToString("N" + decimals);
            IxyTextbox.Text = currentLink.MomentIxy.ToString("N" + decimals);
            IxzTextbox.Text = currentLink.MomentIxz.ToString("N" + decimals);
            IyyTextbox.Text = currentLink.MomentIyy.ToString("N" + decimals);
            IyzTextbox.Text = currentLink.MomentIyz.ToString("N" + decimals);
            IzzTextbox.Text = currentLink.MomentIzz.ToString("N" + decimals);
        }



        /// <summary>
        /// Updates the I_xx value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IxxTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if(Double.TryParse(IxxTextbox.Text,out num))
            {
                currentLink.MomentIxx = num;
            }
        }

        /// <summary>
        /// Updates the I_xy value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IxyTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(IxyTextbox.Text, out num))
            {
                currentLink.MomentIxy = num;
            }
        }

        /// <summary>
        /// Updates the I_xz value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IxzTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(IxzTextbox.Text, out num))
            {
                currentLink.MomentIxz = num;
            }
        }

        /// <summary>
        /// Updates the I_yy value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IyyTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(IyyTextbox.Text, out num))
            {
                currentLink.MomentIyy = num;
            }
        }

        /// <summary>
        /// Updates the I_yz value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IyzTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(IyzTextbox.Text, out num))
            {
                currentLink.MomentIyz = num;
            }
        }

        /// <summary>
        /// Updates the I_zz value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IzzTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(IzzTextbox.Text, out num))
            {
                currentLink.MomentIzz = num;
            }
        }

        /// <summary>
        /// Updates the mass value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MassTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(MassTextbox.Text, out num))
            {
                currentLink.Mass = num;
            }
        }

        /// <summary>
        /// Updates the center of mass (x) value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComXTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(ComXTextbox.Text, out num))
            {
                currentLink.ComX = num;
                if (triadManip != null)
                {
                    double[] newPoint = triadManip.Origin.ArrayData;
                    newPoint[0] = num;
                    triadManip.Origin = currentLink.swApp.GetMathUtility().CreatePoint(newPoint);
                    if(updateStartOrigin)
                        startOrigin = triadManip.Origin;
                    triadManip.UpdatePosition();
                }
                
            }
        }

        /// <summary>
        /// Updates the center of mass (y) value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComYTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(ComYTextbox.Text, out num))
            {
                currentLink.ComY = num;
                if (triadManip != null)
                {
                    double[] newPoint = triadManip.Origin.ArrayData;
                    newPoint[1] = num;
                    triadManip.Origin = currentLink.swApp.GetMathUtility().CreatePoint(newPoint);
                    if(updateStartOrigin)
                        startOrigin = triadManip.Origin;
                    triadManip.UpdatePosition();
                }
                
            }
        }

        /// <summary>
        /// Updates the center of mass (z) value when it changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComZTextbox_TextChanged(object sender, EventArgs e)
        {
            double num;
            if (Double.TryParse(ComZTextbox.Text, out num))
            {
                currentLink.ComZ = num;
                if (triadManip != null)
                {
                    double[] newPoint = triadManip.Origin.ArrayData;
                    newPoint[2] = num;
                    triadManip.Origin = currentLink.swApp.GetMathUtility().CreatePoint(newPoint);
                    if(updateStartOrigin)
                        startOrigin = triadManip.Origin;
                    triadManip.UpdatePosition();
                }
                
            }
        }

        

        /// <summary>
        /// Called when the update button is clicked. This method will recalculate the inertia values of the link and update the text boxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InertiaButton_Click(object sender, EventArgs e)
        {
            if (this.FindForm() == null)
                return; // cannot calculate before newly-selected link has been fully loaded
             
            Configuration currentConfig = currentLink.modelDoc.ConfigurationManager.ActiveConfiguration;
            String currentDisplayState = currentConfig.GetDisplayStates()[0];
            currentLink.CalcInertia(null, overrideCustomInertial: true);
            InertiaValuesUpdateTextbox();
            currentLink.modelDoc.ShowConfiguration(currentConfig.Name);
            currentConfig.ApplyDisplayState(currentDisplayState);
        }

        #region triad handlers
        public bool OnDelete(object pManipulator)
        {
            return true;
        }

        public void OnDirectionFlipped(object pManipulator)
        {
            
        }

        public bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double Value)
        {
            //System.Diagnostics.Debug.WriteLine("handle: " + handleIndex + " value: " + Value);
            double[] originPt = new double[3];
            updateStartOrigin = false;
            switch (handleIndex)
            {
                case (int)swTriadManipulatorControlPoints_e.swTriadManipulatorXAxis:
                    ((double[])startOrigin.ArrayData).CopyTo(originPt, 0);
                    originPt[0] += Value;
                    ComXTextbox.Text = originPt[0].ToString();
                    break;
                case (int)swTriadManipulatorControlPoints_e.swTriadManipulatorYAxis:
                    ((double[])startOrigin.ArrayData).CopyTo(originPt,0);
                    originPt[1] += Value;
                    ComYTextbox.Text = originPt[1].ToString();
                    break;
                case (int)swTriadManipulatorControlPoints_e.swTriadManipulatorZAxis:
                    ((double[])startOrigin.ArrayData).CopyTo(originPt, 0);
                    originPt[2] += Value;
                    ComZTextbox.Text = originPt[2].ToString();
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void OnEndDrag(object pManipulator, int handleIndex)
        {
            startOrigin = ((TriadManipulator)pManipulator).Origin;
            double[] originPt = ((TriadManipulator)pManipulator).Origin.ArrayData;
            ComXTextbox.Text = originPt[0].ToString();
            ComYTextbox.Text = originPt[1].ToString();
            ComZTextbox.Text = originPt[2].ToString();
            currentLink.ComX = originPt[0];
            currentLink.ComY = originPt[1];
            currentLink.ComZ = originPt[2];
            updateStartOrigin = true;
        }

        public void OnEndNoDrag(object pManipulator, int handleIndex)
        {
            
        }

        public bool OnHandleLmbSelected(object pManipulator)
        {
            return true;
        }

        public void OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
            
        }

        public void OnHandleSelected(object pManipulator, int handleIndex)
        {
           
        }

        public void OnItemSetFocus(object pManipulator, int handleIndex)
        {
            
        }

        public bool OnStringValueChanged(object pManipulator, int handleIndex, ref string Value)
        {
            return true;
        }

        public void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
            
        }
        #endregion


        /// <summary>
        /// Press button to delete this link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            RobotInfo.WriteToLogFile("\nClicking Delete Link button (LinkProperties)");
            try
            {
                currentLink.Delete();
                ((ManageRobot)this.FindForm()).ExternalSelect(null);
                RobotInfo.WriteToLogFile("Successfully ran Delete Link button (LinkProperties)");
            }
            catch
            {
                //catch any errors in assigning/removing children
                RobotInfo.WriteToLogFile("Error in clicking Delete Link button (LinkProperties)");
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

            unitToolTip.SetToolTip(this.PhysicalButton, "Click to physical components in Solidworks window.");
            unitToolTip.SetToolTip(this.CollisionButton, "Click to select collision components in Solidworks window.");
            unitToolTip.SetToolTip(this.VisualButton, "Click to select visual comnponents in Solidworks window.");
            
            unitToolTip.SetToolTip(this.nameTextBox, "Pick a unique link name");
            unitToolTip.SetToolTip(this.Mu1Textbox, "Friction coefficient. Must be positive (unitless)");
            unitToolTip.SetToolTip(this.Mu2Textbox, "Friction coefficient. Must be positive (unitless)");
            unitToolTip.SetToolTip(this.KpTextbox, "Contact Stiffness. Must be positive (Newton per meter)");
            unitToolTip.SetToolTip(this.KdTextbox, "Contact Damping. Must be positive");
            unitToolTip.SetToolTip(this.LinearDampingTextbox, "Linear Damping. Must be positive");
            unitToolTip.SetToolTip(this.LinearDampingTextbox, "Angular Damping. Must be positive");
            unitToolTip.SetToolTip(this.MassTextbox, "Must be positive (kilograms)");
            unitToolTip.SetToolTip(this.ComXTextbox, "COM location (meters)");
            unitToolTip.SetToolTip(this.ComYTextbox, "COM location (meters)");
            unitToolTip.SetToolTip(this.ComZTextbox, "COM location (meters)");
            //units for inertia?
        }

        private void selfCollideCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            currentLink.SelfCollide = selfCollideCheckbox.Checked;
        }

        

        private void VisualButton_Click(object sender, EventArgs e)
        {
            string modelConfig = currentLink.LinkModels[(int)ModelConfiguration.ModelConfigType.Visual].swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig != null  && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "Visual Components are already defined in a different configuration";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentLink.OpenLinkEditorPage(manager.robot, manager, (int)ModelConfiguration.ModelConfigType.Visual);
            this.FindForm().Hide();
        }

        private void COllisionButton_Click(object sender, EventArgs e)
        {
            string modelConfig = currentLink.LinkModels[(int)ModelConfiguration.ModelConfigType.Collision].swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig != null && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "Collision Components are already defined in a different configuration";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentLink.OpenLinkEditorPage(manager.robot, manager, (int)ModelConfiguration.ModelConfigType.Collision);
            this.FindForm().Hide();
        }

        /// <summary>
        /// when click SelectButton, go to SW window and left panel to visually select components
        /// of the model selected in the combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PhysicalButton_Click(object sender, EventArgs e)
        {
            string modelConfig = currentLink.LinkModels[(int)ModelConfiguration.ModelConfigType.Physical].swConfiguration;
            string[] configs = RobotInfo.ModelDoc.GetConfigurationNames();
            if (modelConfig != null && configs.Contains(modelConfig) && !modelConfig.Equals(RobotInfo.ModelDoc.ConfigurationManager.ActiveConfiguration.Name))
            {
                string msg = "Physical Components are already defined in a different configuration";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ManageRobot manager = ((ManageRobot)this.FindForm());
            currentLink.OpenLinkEditorPage(manager.robot, manager, (int)ModelConfiguration.ModelConfigType.Physical);
            this.FindForm().Hide();
        }
        
         
    }
}
