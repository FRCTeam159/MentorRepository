using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using GazeboExporter.Export;
using System.Xml;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This class represents a Gyro sensor
    /// Gyros measure rotataion about either the pitch, yaw, or roll axis
    /// </summary>
    public class Gyro : Attachment
    {

        #region Visible Properties

        /// <summary>
        /// The analog channel that this gyro is connected to
        /// </summary>
        public int AnalogChannel
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
        /// The axis of rotation for the gyro
        /// </summary>
        public int Axis
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/axis");
            }
            set
            {
                SwData.SetDouble(Path + "/axis", value);
            }
        }

        /// <summary>
        /// the different axes that the gyro can rotate on
        /// </summary>
        public static string[] Axes = { "pitch", "yaw", "roll" };
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public Gyro(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.GyroPic;
            Name = parentLink.Name + " " + GetName();
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static GyroProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new GyroProperties();
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
            return "Gyro";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.SetGyro(this);
            return propertyEditorControl;
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
            owner.WriteAttributeString("filename", "libgyro.so");

            owner.WriteStartElement("link");
            owner.WriteString(ChildLink.Name);
            owner.WriteEndElement();

            owner.WriteStartElement("axis");
            owner.WriteString(Axes[Axis]);
            owner.WriteEndElement();

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/analog/" + AnalogChannel.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("units");
            owner.WriteString("degrees");
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
                owner.WriteAttributeString("name", ChildLink.Name + "_gyro");
                owner.WriteAttributeString("filename", "libgyro.so");
                SDFExporter.writeSDFElement(owner, "link", ChildLink.Name.Replace(" ", "_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/analog/" + AnalogChannel.ToString());
                SDFExporter.writeSDFElement(owner, "units", "degrees");
                SDFExporter.writeSDFElement(owner, "axis", Axes[Axis]);
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter output)
        {
            
        }
        #endregion
    }
}
