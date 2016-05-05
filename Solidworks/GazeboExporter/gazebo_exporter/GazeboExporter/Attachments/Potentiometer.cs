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
    /// This class represents a Potentiometer sensor
    /// </summary>
    public class Potentiometer : JointedAttachment
    {

        #region Visible Properties

        /// <summary>
        /// The analog channel the pot is on
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

        #endregion
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="ChildLink">Link that contains this attachment</param>
        public Potentiometer(StorageModel swData, string path, Link ChildLink)
            : base(swData, path, ChildLink)
        {
            Icon = CommandManager.PotPic;
            Name = ChildLink.Name + " " + GetName();
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static PotentiometerProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new PotentiometerProperties();
        }

        /// <summary>
        /// If this attachment is selected
        /// </summary>
        public override bool Selected { get; set; }

        // <summary>
        /// Gets the name of this attachment type
        /// </summary>
        /// <returns>Returns the name of the attachment type</returns>
        public override string GetName()
        {
            return "Potentiometer";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setPot(this);
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
            if (Joint == null)
                log.WriteError(Name + " must be associated with a joint.");
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
            owner.WriteAttributeString("filename", "libpotentiometer.so");

            owner.WriteStartElement("joint");
            owner.WriteString(Joint.Name);
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
                owner.WriteAttributeString("name", Joint.Name + "_pot");
                owner.WriteAttributeString("filename", "libpotentiometer.so");
                SDFExporter.writeSDFElement(owner, "joint", Joint.Name.Replace(" ","_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/analog/" + AnalogChannel.ToString());
                SDFExporter.writeSDFElement(owner, "units", "degrees");
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter output)
        {
        }
        #endregion
    }
}
