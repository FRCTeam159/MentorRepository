using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using GazeboExporter.Export;

namespace GazeboExporter.Robot
{
    public abstract class JointedAttachment : Attachment
    {

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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">Storage model</param>
        /// <param name="path">Path in the storage model</param>
        /// <param name="ChildLink">Link that contains this attachment</param>
        public JointedAttachment(StorageModel swData, string path, Link ChildLink) : base(swData, path, ChildLink)
        {

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
        /// Verifies that this attachment is ready to export
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public override bool Verify(ProgressLogger log)
        {
            if (Joint == null)
                log.WriteWarning("No joint for " + Name);
            return true;
        }

    }
}
