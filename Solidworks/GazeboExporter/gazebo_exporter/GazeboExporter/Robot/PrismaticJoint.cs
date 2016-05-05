using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using GazeboExporter.Export;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{

    /// <summary>
    /// A Prismatic joint is a joint that has only one DOF.
    /// It will move linearly along it's axis of motion.
    /// Its limits are defined as distances along the axis in meters.
    /// </summary>
    public class PrismaticJoint : SingleAxisJoint
    {
        const string displayName = "Linear";

        const string exportName = "prismatic";

        /// <summary>
        /// Constructs a new Prismatic joint
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        public PrismaticJoint(Joint joint, string path)
            : base(joint, path,new LinearJointAxis(path+"/axes/axis1",joint))
        {
            
        }

        /// <summary>
        /// Adds the properties to the solidworks page
        /// </summary>
        /// <param name="page">The page to be added to</param>
        /// <param name="id">The starting Id for the controls. Will be updated to the next avalible id</param>
        /// <param name="mark">The starting mark for the selection boxes.Will be updated to the next avalible mark</param>
        public override void AddPropertiesToSWPage(UI.JointPMPage page, ref int id, ref int mark)
        {
            modelDoc.ClearSelection2(true);
            Axis1.AddAxisToPage(ref id, ref mark, page, ref AxisSelBoxID);
            if (!Axis1.UseCustomMovementLimits)
                Axis1.AddLimitsToPage(ref id, ref mark, page);
        }

        public override string GetJointTypeName()
        {
            return exportName;
        }

        static PrismaticJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(PrismaticJoint),1);
        }

    }
}
