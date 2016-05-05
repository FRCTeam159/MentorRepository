using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using GazeboExporter.Export;
using System.Windows.Forms;
using GazeboExporter.UI;
using System.Xml;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// DoubleAxisJoints are joints that rotate around two Axes
    /// </summary>
    public abstract class DoubleAxisJoint:AxialJoint
    {
        /// <summary>
        /// 2 long array of the axes in this joint
        /// </summary>
        protected readonly IJointAxis[] Axes;

        /// <summary>
        /// Ids of the Axes selection boxes
        /// </summary>
        protected int[] AxisSelBoxIDs;

        /// <summary>
        /// Constructs a new double axis joint
        /// </summary>
        /// <param name="joint">The JOint that this joint specific is in</param>
        /// <param name="path">The path to store data to</param>
        /// <param name="axes">2 long array of the axes in this joint</param>
        public DoubleAxisJoint(Joint joint, string path, IJointAxis[] axes)
            : base(joint, path)
        {
            this.Axes = axes;
            foreach (IJointAxis axis in Axes)
            {
                axis.CalcOriginHandler = new ReCalcOrigin(UpdateOriginPoint);
            }
            AxisSelBoxIDs = new int[2];
            
        }

        /// <summary>
        /// Creates the PropertyPanel for the joint
        /// </summary>
        /// <returns>Returns the created control for this joint</returns>
        public override UserControl CreatePropertiesPanel()
        {
            JointSpecificProperties page = (JointSpecificProperties)base.CreatePropertiesPanel();
            DoubleAxisJointPhysicalProperties physPanel = (DoubleAxisJointPhysicalProperties)Axes[0].GetPropertiesPanel(Axes[1]);
            page.AddUserControl(physPanel);
            AddLimitProperties(page);
            return page;
        }

        /// <summary>
        /// Adds the limit properties to the manage robot window
        /// </summary>
        /// <param name="page">Page to add the properties to</param>
        public abstract void AddLimitProperties(JointSpecificProperties page);

        /// <summary>
        /// Creates and fills the solidworks property page
        /// </summary>
        /// <returns>Returns the newly created property page</returns>
        public override JointPMPage FillPropertyPage()
        {
            JointPMPage page = base.FillPropertyPage();
            if (!page.SelectionObservers.Contains(this))
                page.SelectionObservers.Add(this);
            AddPropertiesToSWPage(page, ref page.currentId, ref page.currentMark);
            UpdateOriginPoint();
            foreach (IJointAxis axis in Axes)
            {
                axis.CalcLimits(-1,null);
                axis.DrawAxisPreview();
            }
            return page;
        }


        /// <summary>
        /// Calculates the origin of this joint. The origin will be the point where the 2 axis intersect
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        protected override double[] CalcOrigin(double[] point)
        {
            if (OriginPt != null)
                return OriginValues.Point;
            if (Axes[0].Axis == null || Axes[1].Axis == null)
                return new double[] { 0, 0, 0 };
            MathVector axis1Vector = matUtil.CreateVector(new double[] { Axes[0].AxisX, Axes[0].AxisY, Axes[0].AxisZ });
            MathVector axis2Vector = matUtil.CreateVector(new double[] { Axes[1].AxisX, Axes[1].AxisY, Axes[1].AxisZ });
            double[] point1 = Axes[0].Point;
            double[] point2 = Axes[1].Point;
            MathVector pointVector = matUtil.CreateVector(new double[] { point1[0] - point2[0], point1[1] - point2[1], point1[2] - point2[2] });
            double dist = pointVector.GetLength();
            pointVector = pointVector.Normalise();
            int sign = 1;
            if (VectorCalcs.IsParallel(pointVector, axis2Vector))
            {
                if (Math.Abs(pointVector.Subtract(axis2Vector).GetLength()) > VectorCalcs.errorVal)
                    sign = -1;
            }
            else
            {
                MathVector a2Xp = axis2Vector.Cross(pointVector).Normalise();
                MathVector a2Xa1 = axis2Vector.Cross(axis1Vector).Normalise();
                if (Math.Abs(a2Xp.Subtract(a2Xa1).GetLength()) < VectorCalcs.errorVal)
                    sign = -1;
            }

            
            double t = dist * sign * (Math.Sqrt(1 - Math.Pow(axis1Vector.Dot(pointVector), 2))) / Math.Sqrt(1 - Math.Pow(axis1Vector.Dot(axis2Vector), 2));
            double[] origin = new double[3];
            origin[0] = Axes[1].AxisX * t + point2[0];
            origin[1] = Axes[1].AxisY * t + point2[1];
            origin[2] = Axes[1].AxisZ * t + point2[2];
            return origin;
        }

        /// <summary>
        /// Checks if the given object is a valid selection
        /// </summary>
        /// <param name="isValid">Retruns if the selection is valid. Will be false if the selection does not apply to this object or if it is not valid</param>
        /// <param name="Id">The Id of the slection box</param>
        /// <param name="SelType">The type of the selection</param>
        /// <param name="selection">The object that is selected</param>
        public override void CheckValidSelection(out bool isValid, int Id, int SelType, object selection)
        {
            for (int i = 0;i<AxisSelBoxIDs.Length;i++)
            {
                if (Id == AxisSelBoxIDs[i])
                {
                    if (IJointAxis.IsLinearAxis(selection))
                    {
                        int otherAxisIndex = 1 - i;
                        if (OriginPt == null && Axes[otherAxisIndex].Axis != null)
                        {
                            double[] tempArr = { Axes[otherAxisIndex].AxisX, Axes[otherAxisIndex].AxisY, Axes[otherAxisIndex].AxisZ };
                            MathVector otherAxisVect = RobotInfo.mathUtil.CreateVector(tempArr);
                            MathPoint otherAxisPoint = RobotInfo.mathUtil.CreatePoint(Axes[otherAxisIndex].Point);
                            MathVector SelAxisVect;
                            MathPoint SelAxisPoint;
                            GetAxisFromObject(out SelAxisVect, out SelAxisPoint, selection, SelType);
                            if (VectorCalcs.AxisIntersect(SelAxisVect, SelAxisPoint, otherAxisVect, otherAxisPoint) 
                                && !VectorCalcs.IsParallel(SelAxisVect,otherAxisVect))
                            {
                                isValid = true;
                                return;
                            }
                            else
                            {
                                Axes[i].DisplaySelectionboxError(Id, "Error: Axes are not valid", "The axes must both intersect each other and not be parallel");
                                isValid = false;
                                return;
                            }
                                
                        }
                        else
                        {
                            isValid = true;
                            Axes[i].Axis = selection;
                            return;
                        }
                        
                    }

                   
                }
            }
            isValid = false;
            base.CheckValidSelection(out isValid, Id, SelType, selection);
        }

        /// <summary>
        /// Updates the joint previews
        /// </summary>
        /// <param name="Id">The Id of the selection box the preview coresponds to</param>
        /// <param name="selection">The object that is selected</param>
        public override void UpdatePreviews(int Id, object selection)
        {
            for (int i = 0; i < AxisSelBoxIDs.Length; i++)
            {
                if (Id == AxisSelBoxIDs[i])
                {
                    Axes[i].Axis = selection;
                    int otherAxisIndex = 1 - i;
                    Axes[otherAxisIndex].DrawAxisPreview();
                }
            }
            base.UpdatePreviews(Id, selection);
        }

        /// <summary>
        /// Called when no items are currently selected in a selection box
        /// </summary>
        /// <param name="Id">Id of the selection box</param>
        public override void NoSelections(int Id)
        {
            base.NoSelections(Id);
            for (int i = 0; i < AxisSelBoxIDs.Length; i++)
                if (Id == AxisSelBoxIDs[i])
                {
                    Axes[i].Axis = null;
                    Axes[i].DrawAxisPreview();
                }
        }
        
        /// <summary>
        /// Gets the primary axis vector for the joint
        /// </summary>
        /// <returns></returns>
        public override MathVector GetAxisVector()
        {
            return RobotInfo.mathUtil.CreateVector(new double[] { Axes[0].AxisX, Axes[0].AxisY, Axes[0].AxisZ });
        }

        /// <summary>
        /// Writes this joint to the SDF file
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">The writer to use to write the SDF file</param>
        public override void WriteSDF(ProgressLogger log, XmlWriter writer)
        {
            Axes[0].WriteSDF(log, writer, 1);
            Axes[1].WriteSDF(log, writer, 2);
        }

        /// <summary>
        /// Verifies that the joint is valid for export
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        protected override bool VerifySpecifics(ProgressLogger log)
        {
            Axes[0].Verify(joint.Name + " Axis1", log);
            Axes[1].Verify(joint.Name + " Axis2", log);
            RobotInfo.WriteToLogFile("Successfully Verified both Double Axis");
            return true;
        }

        /// <summary>
        /// Clears all limits in all axes
        /// </summary>
        public override void ClearValues()
        {
            foreach (IJointAxis a in Axes)
                a.ClearLimits();
            
        }

        /// <summary>
        /// Updates the previews for all joint axes
        /// </summary>
        public override void UpdatePreviews()
        {
            foreach (IJointAxis j in Axes)
            {
                j.CalcLimits(-1, null);
                j.DrawAxisPreview();
            }
                
        }
    }
}
