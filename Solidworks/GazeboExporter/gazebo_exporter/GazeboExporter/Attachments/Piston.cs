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
    /// This class represents a Piston actuator
    /// </summary>
    public class Piston : JointedAttachment
    {

        #region Visibile Properties
        /// <summary>
        /// The channel the piston is on
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
        /// direction
        /// </summary>
        public int Direction
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/direction");
            }
            set
            {
                SwData.SetDouble(Path + "/direction", (double)value);
            }
        }

        /// <summary>
        /// the different options for the Direction
        /// </summary>
        public static string[] Directions = { "forward", "reverse" };
        
        /// <summary>
        /// The force that the piston exerts when extending
        /// </summary>
        public double ForwardForce
        {
            get
            {
                return SwData.GetDouble(Path + "/forwardForce");
            }
            set
            {
                SwData.SetDouble(Path + "/forwardForce", value);
            }
        }

        /// <summary>
        /// The force that the piston exerts when retracting
        /// </summary>
        public double ReverseForce
        {
            get
            {
                return SwData.GetDouble(Path + "/reverseForce");
            }
            set
            {
                SwData.SetDouble(Path + "/reverseForce", value);
            }
        }


        #endregion

        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public Piston(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.PistonPic;
            Name = parentLink.Name + " " + GetName();
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static PistonProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new PistonProperties();
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
            return "Pneumatic Piston";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setPiston(this);
            return propertyEditorControl;
        }

        #region Export 

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
            owner.WriteAttributeString("filename", "libpneumatic_piston.so");

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/dio/" + dioChannel.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("joint");
            owner.WriteString(Joint.Name);
            owner.WriteEndElement();

            owner.WriteStartElement("direction");
            owner.WriteString(Directions[Direction]);
            owner.WriteEndElement();

            owner.WriteStartElement("forward-force");
            owner.WriteString(ForwardForce.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("reverse-force");
            owner.WriteString(ReverseForce.ToString());
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
                owner.WriteAttributeString("name", Joint.Name + "_piston");
                owner.WriteAttributeString("filename", "libpneumatic_piston.so");
                SDFExporter.writeSDFElement(owner, "joint", Joint.Name.Replace(" ", "_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/pneumatic/" + dioChannel.ToString() + "/" + dioChannel.ToString());
                SDFExporter.writeSDFElement(owner, "forward-force", ForwardForce.ToString());
                SDFExporter.writeSDFElement(owner, "reverse-force", ReverseForce.ToString());
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter output)
        {
        }
        #endregion 
    }
}
