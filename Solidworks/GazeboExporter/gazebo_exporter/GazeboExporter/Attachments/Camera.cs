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
    /// This class represents a Camera sensor
    /// rangefinders measure distance in a cylinder
    /// </summary>
    public class Camera : DirectionalAttachment
    {

        #region Visible Properties

       
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public Camera(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.CameraPic;
            Name = parentLink.Name + " " + GetName();
            if (FOV == 0)
            {
                FOV = 90;
            }
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static CameraProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void setupPropertyEditor()
        {
            propertyEditorControl = new CameraProperties();
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
            return "Camera";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                setupPropertyEditor();
            propertyEditorControl.setCamera(this);
            return propertyEditorControl;
        }

        /// <summary>
        /// Calculates the rotations in degrees of a given axis
        /// </summary>
        /// <param name="axis">The normalised axis to find the rotation of</param>
        public override double[] CalcRotations(double[] axis)
        {
            double[] rotations = { 0, 0, 0 };//roll,pitch,yaw

            rotations[1] = Math.Asin(-axis[2]) * 180 / Math.PI;
            if (rotations[1] < 0)
            {
                rotations[1] += 360;
            }

            if (axis[0] == 0 && axis[1] == 0)
            {
                rotations[2] = 0;
            }
            else
            {
                IMathUtility matUtil = ChildLink.swApp.GetMathUtility();
                 MathVector xyProjection = matUtil.CreateVector(new double[] { axis[0], axis[1], 0 }).Normalise();
                MathVector xAxis = matUtil.CreateVector(new double[] { 1, 0, 0 });
                rotations[2] = Math.Acos(xyProjection.Dot(xAxis));
                if (axis[1] < 0)
                {
                   rotations[2]= 2*Math.PI-rotations[2];
                }
                if (rotations[2] < 0)
                {
                    rotations[2] += 2 * Math.PI;
                }

                rotations[2] *= 180 / Math.PI;
            }

            return rotations;
        }


        /// <summary>
        /// Calculates the normalized axis of a given set of rotations in radians 
        /// </summary>
        /// <param name="rotations">The rotations (in radians) to find the axis of</param>
        public override double[] CalcAxisFromRotations(double[] rotations)
        {
            double[] tempAxis = { 1, 0, 0 };
            tempAxis[0] = Math.Cos(rotations[1]) * Math.Cos(rotations[2]);//x = cos(p)cos(y)
            tempAxis[1] = Math.Cos(rotations[1]) * Math.Sin(rotations[2]);//y = cos(p)sin(y)
            tempAxis[2] = -Math.Sin(rotations[1]);//z = -sin(p)
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
            owner.WriteAttributeString("reference", ChildLink.Name);

                owner.WriteStartElement("sensor");
                owner.WriteAttributeString("name", ChildLink + "Camera");
                owner.WriteAttributeString("type", "camera");

                    owner.WriteStartElement("visualize");
                    owner.WriteString("true");
                    owner.WriteEndElement();

                    owner.WriteStartElement("pose");
                    owner.WriteString(OriginX + " " + OriginY + " " + OriginZ + " " + RotRoll + " " + RotPitch + " " + RotYaw);
                    owner.WriteEndElement();

                    owner.WriteStartElement("camera");

                        owner.WriteStartElement("horizontal_fov");
                        owner.WriteString((FOV*Math.PI/180).ToString()); 
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
            /*owner.WriteStartElement("plugin");
            {
                owner.WriteAttributeString("name", ChildLink.Name + "_" + this.GetName());
                owner.WriteAttributeString("filename", "lib" + this.GetName());
                //SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/analog/" + this..ToString());
                SDFExporter.writeSDFElement(owner, "visualize", "true");
                SDFExporter.writeSDFElement(owner, "pose", OriginX + " " + OriginY + " " + OriginZ + " " + RotRoll + " " + RotPitch + " " + RotYaw);
                SDFExporter.writeSDFElement(owner, "horizontal_fov", (FOV * Math.PI / 180).ToString());                
            }
            owner.WriteEndElement();*/

        }

        /// <summary>
        /// Writes the Attachment to the SDF file
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="owner">The XMLWriter to use to write the values</param>
        public override void WriteSensor(ProgressLogger log, XmlWriter owner)
        {
            owner.WriteStartElement("sensor");
            {
                owner.WriteAttributeString("name", ChildLink.Name.Replace(" ", "_") + "Camera");
                owner.WriteAttributeString("type", "camera");
                SDFExporter.writeSDFElement(owner, "visualize", "1");
                SDFExporter.writeSDFElement(owner, "always_on", "1");

                SDFExporter.writeSDFElement(owner, "pose", (OriginX - ChildLink.OriginX) + " " + (OriginY - ChildLink.OriginY) + " " + (OriginZ - ChildLink.OriginZ) + " " + RotRoll + " " + RotPitch + " " + RotYaw);
                owner.WriteStartElement("camera");
                {
                    owner.WriteAttributeString("name", ChildLink.Name.Replace(" ","_") + "camera");

                    

                    SDFExporter.writeSDFElement(owner, "horizontal_fov", (FOV * Math.PI / 180).ToString());

                    owner.WriteStartElement("image");
                    {
                        SDFExporter.writeSDFElement(owner, "width", "320");
                        SDFExporter.writeSDFElement(owner, "height", "240");
                    }
                    owner.WriteEndElement();

                    owner.WriteStartElement("clip");
                    {
                        SDFExporter.writeSDFElement(owner, "near", "0.1");
                        SDFExporter.writeSDFElement(owner, "far", "100");
                    }
                    owner.WriteEndElement();
                }
                owner.WriteEndElement();
            }
            owner.WriteEndElement();

        }
        #endregion
    }
}
