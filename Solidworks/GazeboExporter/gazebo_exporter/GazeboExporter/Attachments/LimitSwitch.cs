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
    /// This class represents a Limit Switch sensor
    /// </summary>
    public abstract class LimitSwitch : Attachment
    {

        #region Visible Properties

        /// <summary>
        /// The digital IO channel that this limit switch is connected to
        /// </summary>
        public int dioChannel
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/dioChannel");
            }
            set
            {
                SwData.SetDouble(Path + "/dioChannel", value);
            }
        }

        /// <summary>
        /// The type of limit switch
        /// </summary>
        public int type
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/type");
            }
            set
            {
                SwData.SetDouble(Path + "/type", value);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public LimitSwitch(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.EncoderPic;
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static LimitSwitchProperties propertyEditorControl;


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
            return "Limit Switch";
        }

     


        #region Export

        /// <summary>
        /// Verifies that everything is good in the attachment
        /// </summary>
        /// <param name="log">Logger to output messages to</param>
        /// <returns>true if successfully verified</returns>
        public override bool Verify(Export.ProgressLogger log)
        {
            throw new NotImplementedException();
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
            owner.WriteAttributeString("name", ChildLink.Name + "Contact");
            owner.WriteAttributeString("type", "contact");

            owner.WriteStartElement("always_on");
            owner.WriteString("true");
            owner.WriteEndElement();

            owner.WriteStartElement("contact");
            owner.WriteStartElement("collision");
            owner.WriteString(ChildLink.Name + "_collision");
            owner.WriteEndElement();
            owner.WriteEndElement();

            owner.WriteEndElement();
            owner.WriteEndElement();
        }

        public override void WritePlugins(ProgressLogger log, XmlWriter output)
        {
            throw new NotImplementedException();
        }

        #endregion


    }

}