using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// A universal joint is a joint with 2 axis at 90 degrees that both spin infinitely
    /// </summary>
    public class UniversalJoint:Revolute2Joint
    {
        const string displayName = "Universal";

        const string exportName = "universal";

        /// <summary>
        /// Creates a new Universal joint
        /// </summary>
        /// <param name="joint">joint that contains this joint specifics</param>
        /// <param name="path">The path to store this joint at</param>
        public UniversalJoint(Joint joint, string path):
            base(joint, path)
        {
            ((RotationalJointAxis)Axes[0]).IsContinuous = true;
        }

        /// <summary>
        /// Adds anylimit properties to the windows panel
        /// </summary>
        /// <param name="page">The panel that the limits should be added to</param>
        public override void AddLimitProperties(JointSpecificProperties page)
        {
            return;
        }


        public override void CheckValidSelection(out bool isValid, int Id, int SelType, object selection)
        {
            
            base.CheckValidSelection(out isValid, Id, SelType, selection);
            if(isValid)
            {
                for (int i = 0; i < AxisSelBoxIDs.Length; i++)
                {
                    if (Id == AxisSelBoxIDs[i])
                    {
                        int otherAxisIndex = 1 - i;
                        if (Axes[otherAxisIndex].Axis != null)
                        {
                            double[] tempArr = { Axes[otherAxisIndex].AxisX, Axes[otherAxisIndex].AxisY, Axes[otherAxisIndex].AxisZ };
                            MathVector otherAxisVect = RobotInfo.mathUtil.CreateVector(tempArr);
                            MathVector SelAxisVect;
                            MathPoint SelAxisPoint;
                            GetAxisFromObject(out SelAxisVect, out SelAxisPoint, selection, SelType);
                            if (VectorCalcs.IsPerpendicular(otherAxisVect, SelAxisVect))
                            {
                                isValid = true;
                                
                                return;
                            }
                            else
                            {
                                Axes[i].DisplaySelectionboxError(Id, "Error: Axes must be perpindicular", "The axis must be perpindicular to each other");
                                
                                isValid = false;
                                return;
                            }
                                
                        }
                        else
                        {
                            isValid = true;
                            return;
                        }
                    }
                }
            }
            else
            {
                isValid = false;
            }
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
        static UniversalJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(UniversalJoint),6);
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
        }
    }
}
