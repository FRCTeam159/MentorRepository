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
    /// This class represents an external Limit Switch sensor
    /// </summary>
    public class ExternalLimitSwitch : LimitSwitch
    {


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public ExternalLimitSwitch(StorageModel swData, string path, Link parentLink) 
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.ExtLimitSwitchPic;
            Name = parentLink.Name + " " + GetName();
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static ExternalLimitSwitchProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new ExternalLimitSwitchProperties();
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
            return "External Limit Switch";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setExternalLimSwitch(this);
            return propertyEditorControl;
        }

        #region Export


        /// <summary>
        /// Verifies that everything is good in the attachment
        /// </summary>
        /// <param name="log">Logger to output messages to</param>
        /// <returns>true if successfully verified</returns>
        public override bool Verify(ProgressLogger log)
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
            owner.WriteAttributeString("name", ChildLink.Name + " extlimit");
            owner.WriteAttributeString("filename", "liblimit_switch.so");

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/dio/" + dioChannel.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("type");
            owner.WriteString("external");
            owner.WriteEndElement();

            owner.WriteStartElement("sensor");
            owner.WriteString(ChildLink.Name);
            owner.WriteEndElement();

            owner.WriteEndElement();
            owner.WriteEndElement();

            base.WriteElements(log, owner);
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
                owner.WriteAttributeString("name", ChildLink.Name + "_extlimit");
                owner.WriteAttributeString("filename", "liblimit_switch.so");
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/dio/" + dioChannel.ToString());
                SDFExporter.writeSDFElement(owner, "type", "external");
                SDFExporter.writeSDFElement(owner, "sensor", ChildLink.Name.Replace(" ", "_") + "_contact");
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter owner)
        {
            owner.WriteStartElement("sensor");
            {
                owner.WriteAttributeString("name", ChildLink.Name.Replace(" ", "_") + "_contact");
                owner.WriteAttributeString("type", "contact");

                SDFExporter.writeSDFElement(owner, "always_on", "1");
                //SDFExporter.writeSDFElement(owner, "visualize", "true");

                owner.WriteStartElement("contact");
                {
                    SDFExporter.writeSDFElement(owner, "collision", ChildLink.Name.Replace(" ", "_")+"_collision");
                }
                owner.WriteEndElement();
            }
            owner.WriteEndElement();
        }
        #endregion

    }
}