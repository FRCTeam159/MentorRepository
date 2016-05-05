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
    /// This class represents a Motor actuator
    /// </summary>
    public class SimpleMotor : JointedAttachment
    {

        #region Visible Properties

        /// <summary>
        /// The PWM channel this motor is connected to
        /// </summary>
        public int PwmChannel 
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/pwmChannel");
            }
            set
            {
                SwData.SetDouble(Path + "/pwmChannel", value);
            }
        }

        /// <summary>
        /// The multiplier for this motor
        /// </summary>
        public double Multiplier
        {
            get
            {
                return SwData.GetDouble(Path + "/multiplier");
            }
            set
            {
                if (Multiplier < 0)
                {
                    SwData.SetDouble(Path + "/multiplier", 0);
                }
                else
                {
                    SwData.SetDouble(Path + "/multiplier", value);
                }
                
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="ChildLink">Link that contains this attachment</param>
        public SimpleMotor(StorageModel swData, string path, Link ChildLink) : base(swData, path, ChildLink)
        {
            Icon = CommandManager.MotorPic;
            Name = ChildLink.Name + " "+ GetName();
        }


        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static SimpleMotorProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void setupPropertyEditor()
        {
            propertyEditorControl = new SimpleMotorProperties();
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
            return "Simple Motor";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                setupPropertyEditor();
            propertyEditorControl.setSimpleMotor(this);
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
            if (Multiplier == 0)
                log.WriteWarning("No multiplier in " + Name);
            if (Joint == null)
                log.WriteError(Name + " must be associated with a joint.");
            return true; 
        }

        /// <summary>
        /// Writes the Attachment to the URDF file
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="owner">The XMLWriter to use to write the values</param>
        public override void WriteElements(ProgressLogger log, XmlWriter owner) {
            owner.WriteStartElement("gazebo");
            owner.WriteStartElement("plugin");
            owner.WriteAttributeString("name", Name);
            owner.WriteAttributeString("filename", "libdc_motor.so");

            owner.WriteStartElement("joint");
            owner.WriteString(Joint.Name);
            owner.WriteEndElement();

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/pwm/" + PwmChannel.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("multiplier");
            owner.WriteString(Multiplier.ToString());
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
                owner.WriteAttributeString("name", Joint.Name + "_motor");
                owner.WriteAttributeString("filename", "libdc_motor.so");
                SDFExporter.writeSDFElement(owner, "joint", Joint.Name.Replace(" ", "_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/pwm/" + this.PwmChannel.ToString());
                SDFExporter.writeSDFElement(owner, "multiplier", Multiplier.ToString());
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter output)
        {
        }
        #endregion
    }
}
