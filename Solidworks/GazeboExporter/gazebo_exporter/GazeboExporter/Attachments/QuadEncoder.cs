using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.UI;
using GazeboExporter.Storage;
using GazeboExporter.Export;
using System.Xml;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This class represents a Quadrature encoder sensor
    /// </summary>
    public class QuadEncoder : JointedAttachment
    {

        #region Visible Properties

        /// <summary>
        /// Number of ticks per revolution of the sensor
        /// </summary>
        public double TicksPerRev
        {
            get
            {
                return SwData.GetDouble(Path + "/ticksPerRev");
            }
            set
            {
                SwData.SetDouble(Path + "/ticksPerRev", value);
            }
        }

        /// <summary>
        /// Digital IO pin for the A channel
        /// </summary>
        public int DioPinA
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/dioPinA");
            }
            set
            {
                SwData.SetDouble(Path + "/dioPinA", value);
            }
        }

        /// <summary>
        /// Digital IO pin for the B channel
        /// </summary>
        public int DioPinB
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/dioPinB");
            }
            set
            {
                SwData.SetDouble(Path + "/dioPinB", value);
            }
        }


        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="ChildLink">Link that contains this attachment</param>
        public QuadEncoder(StorageModel swData, string path, Link ChildLink)
            : base(swData, path, ChildLink)
        {
            Icon = CommandManager.EncoderPic;
            Name = ChildLink.Name + " " + GetName();
        }


        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static QuadEncoderProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new QuadEncoderProperties();
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
            return "Quadrature Encoder";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setQuadEncoder(this);
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
            if (TicksPerRev == 0)
                log.WriteWarning("Tick number not specified in " + Name);
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
            owner.WriteAttributeString("filename", "libencoder.so");

            owner.WriteStartElement("joint");
            owner.WriteString(Joint.Name);
            owner.WriteEndElement();

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/dio/" + DioPinA.ToString() + "/" + DioPinB.ToString());
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
                owner.WriteAttributeString("name", Joint.Name + "_encoder");
                owner.WriteAttributeString("filename", "libencoder.so");
                SDFExporter.writeSDFElement(owner, "joint", Joint.Name.Replace(" ", "_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/dio/" + DioPinA.ToString() + "/" + DioPinB.ToString());
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
