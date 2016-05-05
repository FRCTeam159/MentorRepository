using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using GazeboExporter.Export;
using SolidWorks.Interop.sldworks;
using System.Xml;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This class represents a Rangefinder sensor
    /// rangefinders measure distance in a cylinder
    /// </summary>
    public class Rangefinder : DirectionalAttachment
    {

        #region Visible Properties

        /// <summary>
        /// The analog channel this rangefinder is attached to
        /// </summary>
        public int analogChannel
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/analogChannel");
            }
            set
            {
                SwData.SetDouble(Path + "/analogChannel", value);
            }
        }

        /// <summary>
        /// Minimum distance the rangefinder can see
        /// </summary>
        public double MinDist
        {
            get
            {
                return SwData.GetDouble(Path + "/minDist");
            }
            set
            {
                SwData.SetDouble(Path + "/minDist", value);
            }
        }

        /// <summary>
        /// maximum distance the rangefinder can see
        /// </summary>
        public double MaxDist
        {
            get
            {
                return SwData.GetDouble(Path + "/maxDist");
            }
            set
            {
                SwData.SetDouble(Path + "/maxDist", value);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public Rangefinder(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.RangefinderPic;
            Name = parentLink.Name + " " + GetName();
            if (FOV == 0)
            {
                FOV = -.05;
            }
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static RangefinderProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void setupPropertyEditor()
        {
            propertyEditorControl = new RangefinderProperties();
            PropertyPage = propertyEditorControl;
        }

        /// <summary>
        /// If this attachment is selected
        /// </summary>
        public override bool Selected { get; set; }

        /// <summary>
        /// Gets the name of this attachment type
        /// </summary>
        /// <returns>Returns the name of the attachment type</returns>
        public override string GetName()
        {
            return "Rangefinder";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                setupPropertyEditor();
            propertyEditorControl.setRangefinder(this);
            return propertyEditorControl;
        }

        /// <summary>
        /// Calculates the rotations in degrees of a given axis
        /// </summary>
        /// <param name="axis">The normalised axis to find the rotation of</param>
        public override double[] CalcRotations(double[] axis)
        {
            double[] rotations = { 0, 0, 0 };//roll,pitch,yaw

            rotations[0] = Math.Acos(-axis[2]) * 180 / Math.PI;

            IMathUtility matUtil = ChildLink.swApp.GetMathUtility();
            MathVector xyProjection = matUtil.CreateVector(new double[] { axis[0], axis[1], 0 }).Normalise();
            MathVector yAxis = matUtil.CreateVector(new double[] { 0, 1, 0 });
            rotations[2] = Math.Acos(xyProjection.Dot(yAxis));
            if (axis[0] > 0)
            {
                rotations[2] = 2 * Math.PI - rotations[2];
            }
            if (rotations[2] < 0)
            {
                rotations[2] += 2 * Math.PI;
            }

            rotations[2] *= 180 / Math.PI;

            return rotations;
        }


        /// <summary>
        /// Calculates the normalized axis of a given set of rotations in radians 
        /// </summary>
        /// <param name="rotations">The rotations (in radians) to find the axis of</param>
        public override double[] CalcAxisFromRotations(double[] rotations)
        {
            double[] tempAxis = { 1, 0, 0 };
            tempAxis[0] = -(Math.Cos(rotations[2]) * Math.Sin(rotations[1]) * Math.Cos(rotations[0]) + Math.Sin(rotations[2]) * Math.Sin(rotations[0]));//x = cos(y)sin(p)cos(r)+sin(y)sin(r)
            tempAxis[1] = -(Math.Sin(rotations[2]) * Math.Sin(rotations[1]) * Math.Cos(rotations[0]) - Math.Cos(rotations[2]) * Math.Sin(rotations[0]));//y = sin(y)sin(p)cos(r)-cos(y)sin(r)
            tempAxis[2] = -Math.Cos(rotations[1]) * Math.Cos(rotations[0]);//z = cos(p)cos(r)
            return tempAxis;
        }

        #region export
        /// <summary>
        /// Verifies that everything is good in the attachment
        /// </summary>
        /// <param name="log">Logger to output messages to</param>
        /// <returns>true if successfully verified</returns>
        public override bool Verify(Export.ProgressLogger log)
        {
            log.WriteMessage("Verifying " + Name);
            if (OriginPoint == null)
                log.WriteError("No camera location set for " + Name);
            if (MaxDist < MinDist)
                log.WriteError("Minimum distance is greater than maximum distance for " + Name);
            if (MaxDist == 0)
                log.WriteWarning(Name + " has no viewing distance");
            return true; 
        }

        /// <summary>
        /// Writes the Attachment to the URDF file
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="owner">The XMLWriter to use to write the values</param>
        public override void WriteElements(ProgressLogger log, XmlWriter owner)
        {
            owner.WriteStartElement("gazebo");
            owner.WriteStartElement("plugin");
            owner.WriteAttributeString("name", Name);
            owner.WriteAttributeString("filename", "librangefinder.so");

                owner.WriteStartElement("sensor");
                owner.WriteString(ChildLink.Name+"Ultrasonic");
                owner.WriteEndElement();

                owner.WriteStartElement("topic");
                owner.WriteString("/gazebo/frc/simulator/analog/" + analogChannel.ToString());
                owner.WriteEndElement();
                owner.WriteEndElement();
            owner.WriteEndElement();

            owner.WriteStartElement("gazebo");
            owner.WriteAttributeString("reference", ChildLink.Name);

                owner.WriteStartElement("sensor");
                owner.WriteAttributeString("name", ChildLink + "Ultrasonic");
                owner.WriteAttributeString("type", "sonar");

                    owner.WriteStartElement("always_on");
                    owner.WriteString("true");
                    owner.WriteEndElement();

                    owner.WriteStartElement("visualize");
                    owner.WriteString("true");
                    owner.WriteEndElement();

                    owner.WriteStartElement("pose");
                    owner.WriteString(OriginX + " " + OriginY + " " + OriginZ + " " + RotRoll + " " + RotPitch + " " + RotYaw);
                    owner.WriteEndElement();

                    owner.WriteStartElement("sonar");

                        owner.WriteStartElement("min");
                        owner.WriteString(MinDist.ToString());
                        owner.WriteEndElement();

                        owner.WriteStartElement("max");
                        owner.WriteString(MaxDist.ToString());
                        owner.WriteEndElement();

                        owner.WriteStartElement("radius");
                        owner.WriteString((-FOV).ToString());
                        owner.WriteEndElement();

                    owner.WriteEndElement();
                owner.WriteEndElement();
            owner.WriteEndElement();
        }
        /// <summary>
        /// Writes the Attachment to the SDF file
        /// </summary>
        /// <param name="log"></param>
        /// <param name="owner"></param>
        public override void WritePlugins(ProgressLogger log, XmlWriter owner)
        {
            owner.WriteStartElement("plugin");
            {
                owner.WriteAttributeString("name", ChildLink.Name.Replace(" ","_") + "_rangefinder");
                owner.WriteAttributeString("filename", "librangefinder.so");
               // SDFExporter.writeSDFElement(owner, "link", ChildLink.Name);
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/analog/" + analogChannel.ToString());
                SDFExporter.writeSDFElement(owner, "sensor", ChildLink.Name.Replace(" ", "_") + "Ultrasonic");
            }
            owner.WriteEndElement();

        }

        public override void WriteSensor(ProgressLogger log, XmlWriter owner)
        {
            owner.WriteStartElement("sensor");
            {
                owner.WriteAttributeString("name", ChildLink.Name.Replace(" ","_") + "Ultrasonic");
                owner.WriteAttributeString("type", "sonar");

                SDFExporter.writeSDFElement(owner, "always_on", "1");
                SDFExporter.writeSDFElement(owner, "visualize", "1");
                string pose = (OriginX - ChildLink.OriginX) + " " + (OriginY - ChildLink.OriginY) + " " + (OriginZ - ChildLink.OriginZ) + " " + RotRoll + " " + RotPitch + " " + RotYaw;
                SDFExporter.writeSDFElement(owner, "pose", pose);
                owner.WriteStartElement("sonar");
                {
                    
                    SDFExporter.writeSDFElement(owner, "min", MinDist.ToString());
                    SDFExporter.writeSDFElement(owner, "max", MaxDist.ToString());
                    SDFExporter.writeSDFElement(owner, "radius", (-FOV).ToString());

                }
                owner.WriteEndElement();
            }
            owner.WriteEndElement();
        }
        #endregion
    }
}
