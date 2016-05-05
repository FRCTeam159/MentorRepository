using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Robot
{
    public class Revolute2Joint: DoubleAxisJoint
    {
        const string displayName = "2-Axis Rotational";

        const string exportName = "revolute2";

        public Revolute2Joint(Joint joint, string path): 
            base(joint, path,
            new IJointAxis[] { new RotationalJointAxis(path + "/axes/axis1", joint), new RotationalJointAxis(path + "/axes/axis2", joint) })
        {
            ((RotationalJointAxis)Axes[1]).IsContinuous = true;
            ((RotationalJointAxis)Axes[0]).ArrowColor = RotationalJointAxis.DefaultArrow1Color;
            ((RotationalJointAxis)Axes[1]).ArrowColor = RotationalJointAxis.DefaultArrow2Color;
        }

        /// <summary>
        /// Adds anylimit properties to the windows panel
        /// </summary>
        /// <param name="page">The panel that the limits should be added to</param>
        public override void AddLimitProperties(JointSpecificProperties page)
        {
            page.AddUserControl(new AxialJointLimitsProperties(Axes[0]));
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
            Axes[0].AddAxisToPage(ref id, ref mark, page, ref AxisSelBoxIDs[0]);
            Axes[1].AddAxisToPage(ref id, ref mark, page, ref AxisSelBoxIDs[1]);
            if (!Axes[0].UseCustomMovementLimits && !((RotationalJointAxis)Axes[0]).IsContinuous)
                Axes[0].AddLimitsToPage(ref id, ref mark, page);
        }

        /// <summary>
        /// Gets the name of this joint type
        /// </summary>
        /// <returns></returns>
        public override string GetJointTypeName()
        {
            return exportName;
        }

        /// <summary>
        /// Reghisters the joint in the factory
        /// </summary>
        static Revolute2Joint()
        {
            JointFactory.RegisterJointType(displayName, typeof(Revolute2Joint),5);
        }

    }
}
