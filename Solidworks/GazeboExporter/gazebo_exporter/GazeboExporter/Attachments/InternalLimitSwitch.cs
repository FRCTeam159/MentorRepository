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
    public class InternalLimitSwitch : LimitSwitch
    {

        #region Visible Properties

        /// <summary>
        /// The units that this internal limit switch uses
        /// </summary>
        public int units
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/units");
            }
            set
            {
                SwData.SetDouble(Path + "/units", (double) value);
            }
        }

        /// <summary>
        /// the different units for angles
        /// </summary>
        public static string[] AngleUnits = {"degrees","radians"};
        
        /// <summary>
        /// Minimum angle limit (input in degrees) to trigger internal limit switch
        /// </summary>
        public double MinLimit
        {
            get
            {
                return SwData.GetDouble(Path + "/minLimit");
            }
            set
            {
                SwData.SetDouble(Path + "/minLimit", value);
            }
        }

        /// <summary>
        ///  Maximum angle limit (input in degrees) to trigger internal limit switch
        /// </summary>
        public double MaxLimit
        {
            get
            {
                return SwData.GetDouble(Path + "/MaxLimit");
            }
            set
            {
                SwData.SetDouble(Path + "/MaxLimit", value);
            }
        }

        /// <summary>
        /// The JOint this acts on
        /// </summary>
        public Joint Joint { get; set; }

        /// <summary>
        /// The Id of the parent link, -1 if none is set
        /// </summary>
        public int ParentLinkID
        {
            get
            {
                return (int)SwData.GetDouble(Path + "/parentID") - 1;
            }
            set
            {
                SwData.SetDouble(Path + "/parentID", value + 1);
            }
        }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="parentLink">Link that contains this attachment</param>
        public InternalLimitSwitch(StorageModel swData, string path, Link parentLink)
            : base(swData, path, parentLink)
        {
            Icon = CommandManager.IntLimitSwitchPic;
            Name = parentLink.Name + " " + GetName();
            if (ParentLinkID != -1)
                Joint = ChildLink.GetJointFromLink(ChildLink.robot.GetLink(ParentLinkID));
            else if (ChildLink.UpperJoints.Length == 1)
                SetJoint(ChildLink.GetJointFromLink(ChildLink.UpperJoints[0].Parent));
        }

        /// <summary>
        /// Sets the joint this attachment affects
        /// </summary>
        /// <param name="j">Joint that this attachment affects</param>
        public void SetJoint(Joint j)
        {

            if (j.Parent.Equals(ChildLink))
            {
                ParentLinkID = j.Child.Id;
                Joint = j;
            }
            else if (j.Child.Equals(ChildLink))
            {
                ParentLinkID = j.Parent.Id;
                Joint = j;
            }
        }

        /// <summary>
        /// the Property editor that corresponds to this attachment
        /// </summary>
        private static InternalLimitSwitchProperties propertyEditorControl;

        /// <summary>
        /// sets up the property editor for this attachment
        /// </summary>
        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new InternalLimitSwitchProperties();
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
            return "Internal Limit Switch";
        }

        /// <summary>
        /// gets the property editor that coresponds to this attachment
        /// </summary>
        /// <returns>The property editor that coresponds to this attachment</returns>
        public override Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setInternalLimSwitch(this);
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

            if (Joint != null)
            {
                if (Joint.jointSpecifics is SingleAxisJoint)
                {
                    if (MinLimit == MaxLimit)
                        log.WriteWarning("No range of movement defined in " + Name);
                    else if (MinLimit > MaxLimit)
                        log.WriteError("Minimum value must be less than maximum value in " + Name);

                    
                    //check that units of switch limits match those of joint limits
                    if (Joint.jointSpecifics is RevoluteJoint) // for revolute joints, convert to radians and check limit ranges
                    {
                        if (units == 0) // if degrees, convert to radians
                        {
                            MinLimit = MinLimit * Math.PI / 180;
                            MaxLimit = MaxLimit * Math.PI / 180;
                            units = 1;
                        }
                        
                    }
                    if (MinLimit < ((SingleAxisJoint)Joint.jointSpecifics).Axis1.LowerLimit)
                        log.WriteError("Minimum limit in " + Name + " is less than miniumum limit in joint " + Joint.Name);
                    if (MaxLimit > ((SingleAxisJoint)Joint.jointSpecifics).Axis1.UpperLimit)
                        log.WriteError("Maximum limit in " + Name + " is greater than maximum limit in joint " + Joint.Name);
                }
                else
                {
                    log.WriteError(Name + " must be applied to a revolute or prismatic joint");
                }
                
            }
            else
            {
                log.WriteError(Name + " must be associated with a joint.");
            }
           
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
            owner.WriteAttributeString("name", ChildLink.Name + " intlimit");
            owner.WriteAttributeString("filename", "liblimit_switch.so");

            owner.WriteStartElement("topic");
            owner.WriteString("/gazebo/frc/simulator/dio/" + dioChannel.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("type");
            owner.WriteString("internal");
            owner.WriteEndElement();

            owner.WriteStartElement("joint");
            owner.WriteString(Joint.Name);  
            owner.WriteEndElement();

            owner.WriteStartElement("units");
            owner.WriteString(AngleUnits[units]);
            owner.WriteEndElement();

            owner.WriteStartElement("min");
            owner.WriteString(MinLimit.ToString());
            owner.WriteEndElement();

            owner.WriteStartElement("max");
            owner.WriteString(MaxLimit.ToString());
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
                owner.WriteAttributeString("name", Joint.Name + "_intlimit");
                owner.WriteAttributeString("filename", "liblimit_switch.so");
                SDFExporter.writeSDFElement(owner, "joint", Joint.Name.Replace(" ", "_"));
                SDFExporter.writeSDFElement(owner, "topic", "/gazebo/frc/simulator/dio/" + dioChannel.ToString());
                SDFExporter.writeSDFElement(owner, "type", "internal");
                SDFExporter.writeSDFElement(owner, "units", AngleUnits[units]);
                SDFExporter.writeSDFElement(owner, "min", MinLimit.ToString());
                SDFExporter.writeSDFElement(owner, "max", MaxLimit.ToString());
            }
            owner.WriteEndElement();
        }

        public override void WriteSensor(ProgressLogger log, XmlWriter output)
        {
        }

        #endregion

    }
}