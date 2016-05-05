using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// A screw joint is a joint that has linear movment along an azis as well as having rotation along that axis
    /// the roataion and translation are coupled using the thread pitch paramter.
    /// </summary>
    public class ScrewJoint : SingleAxisJoint
    {
        const string displayName = "Screw";

        const string exportName = "screw";

        /// <summary>
        /// Constructs a new Screw joint
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        public ScrewJoint(Joint joint, string path): base(joint, path, new ScrewJointAxis(path+ "/axes/axis1",joint))
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

        // <summary>
        /// Creates the PropertyPanel for the joint
        /// </summary>
        /// <returns>Returns the created control for this joint</returns>
        public override UserControl CreatePropertiesPanel()
        {
            JointSpecificProperties page = (JointSpecificProperties)base.CreatePropertiesPanel();
            page.AddUserControl(new ScrewJointSpecialProperties((ScrewJointAxis)Axis1));
            return page;
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
        static ScrewJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(ScrewJoint),3);
        }
    }
}
