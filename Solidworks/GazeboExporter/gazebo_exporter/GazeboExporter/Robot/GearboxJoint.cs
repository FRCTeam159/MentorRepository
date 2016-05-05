using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using GazeboExporter.Export;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// A gearbox joint is a revolute joint that's rotation is based on another joint
    /// </summary>
    public class GearboxJoint:RevoluteJoint
    {

        const string displayName = "Gearbox";

        const string exportName = "revolute";
        /// <summary>
        /// The joint this gearbox uses as the reference
        /// </summary>
        public Joint referenceJoint
        {
            get
            {
                int index = (int)swData.GetDouble(path + "/gearbox_reference_body") - 1;
                if (index < 0)
                    return null;
                return joint.Parent.GetJointFromLink(robot.Links[index]);
            }
            set
            {
                if (value.Parent.Equals(joint.Parent))
                    swData.SetDouble(path + "/gearbox_reference_body", value.Child.Id + 1);
                else
                    swData.SetDouble(path + "/gearbox_reference_body", value.Parent.Id + 1);
            }
        }

        /// <summary>
        /// The gearbox ratio of the joint
        /// </summary>
        public double GearboxRatio
        {
            get
            {
                return swData.GetDouble(path + "/gearboxRatio");
            }
            set
            {
                swData.SetDouble(path + "/gearboxRatio",value);
            }
        }

        /// <summary>
        /// Constructs a new gearbox joint
        /// </summary>
        /// <param name="joint">The joint that contains this jointSpecific</param>
        /// <param name="path">The path to store this joint at</param>
        public GearboxJoint(Joint joint, string path)
            : base(joint, path)
        {
            ((RotationalJointAxis)Axis1).IsContinuous = true;
        }

        /// <summary>
        /// Writes the SDF file for this joint
        /// </summary>
        /// <param name="log">The logger to use to write messages</param>
        /// <param name="writer">The writer to use to write the sdf file</param>
        public override void WriteJointSDF(ProgressLogger log,XmlWriter writer)
        {
            base.WriteJointSDF(log, writer);
            string ParentName;
            string ChildName = joint.Child.Name.Replace(" ", "_");
            if (referenceJoint.Parent.Equals(joint.Parent))
                ParentName = referenceJoint.Child.Name.Replace(" ", "_");
            else
                ParentName = referenceJoint.Parent.Name.Replace(" ", "_");

            string jointName = ParentName + "-" + ChildName + "_gearbox";
            writer.WriteStartElement("joint");
            writer.WriteAttributeString("name", jointName);
            writer.WriteAttributeString("type", "gearbox");
            log.WriteMessage("Writing joint " + jointName + " to SDF.");
            {
                SDFExporter.writeSDFElement(writer, "parent", ParentName); //Parent Link
                SDFExporter.writeSDFElement(writer, "child", ChildName);   //Child Link
                SDFExporter.writeSDFElement(writer, "gearbox_ratio", GearboxRatio.ToString());
                SDFExporter.writeSDFElement(writer, "gearbox_reference_body", joint.Parent.Name.Replace(" ", "_"));
                writer.WriteStartElement("axis");
                {
                    SDFExporter.writeSDFElement(writer, "xyz", Axis1.AxisX + " " + Axis1.AxisY + " " + Axis1.AxisZ);
                }
                writer.WriteEndElement();
                writer.WriteStartElement("axis2");
                {
                    SDFExporter.writeSDFElement(writer, "xyz", Axis1.AxisX + " " + Axis1.AxisY + " " + Axis1.AxisZ);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        protected override bool VerifySpecifics(ProgressLogger logger)
        {
            if (GearboxRatio == 0)
                logger.WriteError("No gear ratio defined in joint " + joint.Name);
            return base.VerifySpecifics(logger);
        }

        /// <summary>
        /// Creates the PropertyPanel for the joint
        /// </summary>
        /// <returns>Returns the created control for this joint</returns>
        public override UserControl CreatePropertiesPanel()
        {
            JointSpecificProperties page = (JointSpecificProperties)base.CreatePropertiesPanel();
            page.AddUserControl(new GearBoxPanel(this));
            return page;
        }


        public void FillJointSelector(ComboBox box)
        {
            box.Items.Clear();
            box.Text = "";
            if (Axis1.Axis == null)
            {
                box.Items.Add("No Axis selected");
            }
            else
            {
                MathVector axisVector = RobotInfo.mathUtil.CreateVector(new double[] {Axis1.AxisX,Axis1.AxisY,Axis1.AxisZ});
                foreach (Joint j in joint.Parent.ParentJoints)
                {
                    if (j.jointSpecifics is RevoluteJoint)
                    {
                        RotationalJointAxis tempAxis = (RotationalJointAxis)((RevoluteJoint)j.jointSpecifics).Axis1;
                        if (tempAxis.Axis == null)
                            continue;
                        MathVector jointVect = RobotInfo.mathUtil.CreateVector(new double[] { tempAxis.AxisX, tempAxis.AxisY, tempAxis.AxisZ });
                        if(VectorCalcs.IsParallel(axisVector,jointVect))
                            box.Items.Add(j);
                    }
                        
                }
                foreach (Joint j in joint.Parent.ChildJoints)
                {
                    if (j != joint && j.jointSpecifics is RevoluteJoint)
                    {
                        RotationalJointAxis tempAxis = (RotationalJointAxis)((RevoluteJoint)j.jointSpecifics).Axis1;
                        if (tempAxis.Axis == null)
                            continue;
                        MathVector jointVect = RobotInfo.mathUtil.CreateVector(new double[] { tempAxis.AxisX, tempAxis.AxisY, tempAxis.AxisZ });
                        if (VectorCalcs.IsParallel(axisVector, jointVect))
                            box.Items.Add(j);
                    }
                }
                if (box.Items.Count == 0)
                    box.Items.Add("No possible reference joints");
                if (referenceJoint == null)
                    box.SelectedIndex = -1;
                else
                    box.SelectedItem = referenceJoint;
            }
        }

        /// <summary>
        /// Gets the name of this joint type
        /// </summary>
        /// <returns></returns>
        public override string GetJointTypeName()
        {
            return exportName;
        }

        /// <summary>
        /// Reghisters the joint in the factory
        /// </summary>
        static GearboxJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(GearboxJoint),4);
        }
    }
}
