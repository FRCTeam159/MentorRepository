using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using GazeboExporter.Robot;
using GazeboExporter.Storage;
using GazeboExporter.UI;


namespace GazeboExporter.Robot
{
    /// <summary>
    /// This Class represents any attachment that has a specific location or direction in the robot model
    /// this can include sensors like rangefinders and cameras
    /// </summary>
    public abstract class DirectionalAttachment:Attachment
    {

        /// <summary>
        /// X origin of the attachment
        /// </summary>
        public double OriginX
        {
            get
            {
                return SwData.GetDouble(Path + "/originX");
            }
            set
            {
                SwData.SetDouble(Path + "/originX", value);
            }
        }

        /// <summary>
        /// Y origin of the attachment
        /// </summary>
        public double OriginY
        {
            get
            {
                return SwData.GetDouble(Path + "/originY");
            }
            set
            {
                SwData.SetDouble(Path + "/originY", value);
            }
        }

        /// <summary>
        /// Z origin of the attachment
        /// </summary>
        public double OriginZ
        {
            get
            {
                return SwData.GetDouble(Path + "/originZ");
            }
            set
            {
                SwData.SetDouble(Path + "/originZ", value);
            }
        }

        /// <summary>
        /// rotation about the X axis
        /// </summary>
        public double RotRoll
        {
            get
            {
                return SwData.GetDouble(Path + "/RotR");
            }
            set
            {
                SwData.SetDouble(Path + "/RotR", value);
            }
        }

        /// <summary>
        /// rotation about the y axis
        /// </summary>
        public double RotPitch
        {
            get
            {
                return SwData.GetDouble(Path + "/RotP");
            }
            set
            {
                SwData.SetDouble(Path + "/RotP", value);
            }
        }

        /// <summary>
        /// rotation about the Z axis
        /// </summary>
        public double RotYaw
        {
            get
            {
                return SwData.GetDouble(Path + "/RotY");
            }
            set
            {
                SwData.SetDouble(Path + "/RotY", value);
            }
        }

        /// <summary>
        /// the origin point of the attachment. Will be a vertex or reference point
        /// </summary>
        public object OriginPoint
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(SwData.GetString(Path + "/OriginPoint"));
                return ChildLink.modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    SwData.Delete(Path + "/OriginPoint");
                }
                else
                {
                    byte[] pid = ChildLink.modelDoc.Extension.GetPersistReference3(value);
                    SwData.SetString(Path + "/OriginPoint", Convert.ToBase64String(pid));
                    
                }
                
            }
        }

        /// <summary>
        /// the direction of the attachment. Will be an edge or an axis
        /// </summary>
        public object DirectionAxis
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(SwData.GetString(Path + "/DirAxis"));
                return ChildLink.modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    SwData.Delete(Path + "/DirAxis");
                }
                else
                {
                    byte[] pid = ChildLink.modelDoc.Extension.GetPersistReference3(value);
                    SwData.SetString(Path + "/DirAxis", Convert.ToBase64String(pid));
                }
            }
        }

        /// <summary>
        /// true if the axis should be flipped
        /// </summary>
        public bool FlipAxis
        {
            get
            {
                return SwData.GetDouble(Path + "/flipAxis")==1;
            }
            set
            {
                SwData.SetDouble(Path + "/flipAxis", value ? 1 : 0);
            }
        }

        /// <summary>
        /// True if manually defined angles should be used
        /// </summary>
        public bool UseManualAngles
        {
            get
            {
                return SwData.GetDouble(Path + "/ManualAngles") == 1;
            }
            set
            {
                SwData.SetDouble(Path + "/ManualAngles", value ? 1 : 0);
            }
        }

        /// <summary>
        /// the field of view for the attachment
        /// if positive, it represents the angle of vision in degrees
        /// if negative it represents the radius of a cylinder representing the vision
        /// </summary>
        public double FOV
        {
            get
            {
                return SwData.GetDouble(Path + "/FOV");
            }
            set
            {
                SwData.SetDouble(Path + "/FOV",value);
            }
        }

        /// <summary>
        /// the property manager for this attachment
        /// </summary>
        public static AttachmentProperties PropertyPage { get; set; }

        /// <summary>
        /// Constructor for directional attachments. Directional attachments are attachments that have a location and direction, like cameras and rangefinders
        /// </summary>
        /// <param name="swData">The storage model that will be used for storage</param>
        /// <param name="path">The storage path for this attachment's storage</param>
        /// <param name="parentLink">The link that contains this attachment</param>
        protected DirectionalAttachment(StorageModel swData, string path, Link parentLink):base(swData,path,parentLink)
        {

        }

        /// <summary>
        /// Calculates the rotaions that would be needed to rotate to the given axis
        /// </summary>
        /// <param name="axis">Axis that the attachment will be on</param>
        /// <returns>An array containing the 3 rotations in degrees in the order roll, pitch, yaw</returns>
        public abstract double[] CalcRotations(double[] axis);

        /// <summary>
        /// Calculates the axis that would be created after a given rotation
        /// </summary>
        /// <param name="rotations">Rotations in radians to be performed in the order roll, pitch, yaw</param>
        /// <returns>An array containg the xyz components of the generated axis</returns>
        public abstract double[] CalcAxisFromRotations(double[] rotations);
    }
}
