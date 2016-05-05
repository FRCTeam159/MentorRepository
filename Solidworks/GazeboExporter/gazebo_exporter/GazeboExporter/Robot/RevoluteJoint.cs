using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using System.Windows.Forms;
using GazeboExporter.UI;

namespace GazeboExporter.Robot
{
    public class RevoluteJoint: SingleAxisJoint
    {
        const string displayName = "Rotational";

        const string exportName = "revolute"; 

        /// <summary>
        /// Constructs a new Prismatic joint
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        public RevoluteJoint(Joint joint, string path)
            : base(joint, path,new RotationalJointAxis(path+"/axes/axis1",joint))
        {
            ((RotationalJointAxis)Axis1).ArrowColor = RotationalJointAxis.DefaultArrow1Color; 
        }



        public override void AddPropertiesToSWPage(JointPMPage page, ref int id, ref int mark)
        {
            modelDoc.ClearSelection2(true);
            Axis1.AddAxisToPage(ref id, ref mark, page, ref AxisSelBoxID);
            if(!Axis1.UseCustomMovementLimits && !((RotationalJointAxis)Axis1).IsContinuous)
                Axis1.AddLimitsToPage(ref id, ref mark, page);
        }

        public override string GetJointTypeName()
        {
            return exportName;
        }

        static RevoluteJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(RevoluteJoint),2);
        }
    }
}
