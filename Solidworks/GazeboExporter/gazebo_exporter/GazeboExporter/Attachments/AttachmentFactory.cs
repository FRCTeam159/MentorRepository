using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using System.Drawing;
using System.Reflection;
using System.IO;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This class generates attachments as well as manages the descriptors for the attachments
    /// </summary>
    public static class AttachmentFactory
    {

        private const int SimpleMotorID = 1;
        private const int QuadEncoderID = 2;
        private const int PotentiometerID = 3;
        private const int GyroID = 4;
        private const int RangefinderID = 5;
        private const int CameraID = 6;
        private const int ExtLimitSwitchID = 7;
        private const int IntLimitSwitchID = 8;
        private const int PistonID = 9;
        private const int CanMotorID = 10;

        /// <summary>
        /// Does the initial setup of the factory
        /// this creates an AttachmentDescriptor for each type of implemented attachment
        /// </summary>
        public static void InitializeFactory()
        {      
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Simple Motor", CommandManager.MotorPic, SimpleMotorID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("CAN Motor", CommandManager.MotorPic, CanMotorID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Quad Encoder", CommandManager.EncoderPic, QuadEncoderID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Potentiometer", CommandManager.PotPic, PotentiometerID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Gyro", CommandManager.GyroPic, GyroID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Range finder", CommandManager.RangefinderPic, RangefinderID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Camera", CommandManager.CameraPic, CameraID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("External Limit Switch", CommandManager.ExtLimitSwitchPic, ExtLimitSwitchID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Internal Limit Switch", CommandManager.IntLimitSwitchPic, IntLimitSwitchID));
            allAttachmentDescriptors.AddLast(new AttachmentDescriptor("Piston", CommandManager.PistonPic, PistonID));
 
        }

        /// <summary>
        /// List of all the AttachmentDescriptors
        /// </summary>
        public static LinkedList<AttachmentDescriptor> allAttachmentDescriptors;

        /// <summary>
        /// initializes the array of AttachmentDescriptors
        /// </summary>
        static AttachmentFactory()
        {
            allAttachmentDescriptors = new LinkedList<AttachmentDescriptor>();
        }

        /// <summary>
        /// Generates a new attachment
        /// </summary>
        /// <param name="swData">The storage model to store the attachment in</param>
        /// <param name="path">The path to the attachment in the storage model</param>
        /// <param name="parentLink">The link that contains the attachment</param>
        /// <returns>The newly generated attachment</returns>
        public static Attachment GenerateAttachment(string path, Link parentLink)
        {
            StorageModel swData = RobotInfo.SwData;
            int id = (int)swData.GetDouble(path);

            if (parentLink.isBaseLink &&
                (id == SimpleMotorID || id == QuadEncoderID || id == PotentiometerID || id == IntLimitSwitchID || id == PistonID))
            {
                string msg = "This attachment must be associated with a link that has at least one parent joint.";
                MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            switch (id)
            {
                case SimpleMotorID:
                    return new SimpleMotor(swData, path, parentLink );
                case CanMotorID:
                    return new CanMotor(swData, path, parentLink);
                case QuadEncoderID:
                    return new QuadEncoder(swData, path, parentLink);
                case PotentiometerID:
                    return new Potentiometer(swData, path, parentLink);
                case GyroID:
                    return new Gyro(swData, path, parentLink);
                case RangefinderID:
                    return new Rangefinder(swData, path, parentLink);
                case CameraID:
                    return new Camera(swData, path, parentLink);
                case ExtLimitSwitchID:
                    return new ExternalLimitSwitch(swData, path, parentLink);
                case IntLimitSwitchID:
                    return new InternalLimitSwitch(swData, path, parentLink);
                case PistonID:
                    return new Piston(swData, path, parentLink);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates an new attachment of the specified type
        /// </summary>
        /// <param name="swData">The storage model to store the attachment in</param>
        /// <param name="path">The path to the attachment in the storage model</param>
        /// <param name="id">The type of the attachment</param>
        /// <param name="parentLink">The link that contains the attachment</param>
        /// <returns>The newly created attachment</returns>
        public static Attachment CreateNewAttachment(string path, int id, Link parentLink)
        {
            RobotInfo.SwData.SetDouble(path, id);
            return GenerateAttachment(path, parentLink);
        }
    }
}
