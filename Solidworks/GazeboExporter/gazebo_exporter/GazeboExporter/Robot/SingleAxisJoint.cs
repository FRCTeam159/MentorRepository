using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using GazeboExporter.Storage;
using GazeboExporter.Export;
using System.Windows.Forms;
using GazeboExporter.UI;
using System.Xml;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// A single axis joint is a joint that has all of it's DOF defined by 1 axis
    /// </summary>
    public abstract class SingleAxisJoint : AxialJoint
    {
        

        //The Axis for this joint
        public readonly IJointAxis Axis1;

        protected int AxisSelBoxID;

        /// <summary>
        /// Constructs a new single axis joint
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        /// <param name="axis">The IJointAxis that defines this joint's DOF</param>
        public SingleAxisJoint(Joint joint, string path, IJointAxis axis):base(joint,path)
        {
            
            this.Axis1 = axis;
            axis.CalcOriginHandler = new ReCalcOrigin(UpdateOriginPoint);
            
        }

        /// <summary>
        /// Calculates an origin point that is closest to the given point and on the line
        /// </summary>
        /// <param name="origPoint">POint to be close to</param>
        /// <returns>the origin point that is created</returns>
        protected override double[] CalcOrigin(double[] origPoint)
        {
            if (OriginPt != null)
                return OriginValues.Point;
            if (Axis1.Axis == null)
                return new double[] { 0, 0, 0 };
            MathVector axisVector = matUtil.CreateVector(new double[] { Axis1.AxisX, Axis1.AxisY, Axis1.AxisZ });
            double[] points = Axis1.Point;
            double[] dispVect = { origPoint[0] - points[0], origPoint[1] - points[1], origPoint[2] - points[2] };
            MathVector tempVector = matUtil.CreateVector(dispVect);//vector between the first axis point and the component origin
            double t = axisVector.Dot(tempVector) / axisVector.Dot(axisVector);//finds displacment along the axis to move to get to the origin point

            double[] origin = new double[3];
           // the origin is the closest point to the first component's origin that is also on the line
            origin[0] = Axis1.AxisX * t + points[0];
            origin[1] = Axis1.AxisY * t + points[1];
            origin[2] = Axis1.AxisZ * t + points[2];
            return origin;
        }

        /// <summary>
        /// Verifies that the joint is valid to export
        /// </summary>
        /// <param name="logger">The logger to write messages to</param>
        /// <returns>Returns true if succesfully verified</returns>
        protected override bool VerifySpecifics(ProgressLogger logger)
        {
            Axis1.Verify(joint.Name + " Axis1", logger);
            RobotInfo.WriteToLogFile("Successfully Verified Single Axis Joint");
            return true;
        }

        /// <summary>
        /// Writes this joint to the SDF file
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">The writer to use to write the SDF file</param>
        public override void WriteSDF(ProgressLogger log, XmlWriter writer)
        {
            Axis1.WriteSDF(log,writer,1);
        }

        /// <summary>
        /// Creates the PropertyPanel for the joint
        /// </summary>
        /// <returns>Returns the created control for this joint</returns>
        public override UserControl CreatePropertiesPanel()
        {
            JointSpecificProperties page = (JointSpecificProperties)base.CreatePropertiesPanel();
            SingleAxisJointPhysicalProperties physPanel = (SingleAxisJointPhysicalProperties)Axis1.GetPropertiesPanel();
            page.AddUserControl(physPanel);
            page.AddUserControl(Axis1.GetLimitsPanel());
            return page;
        }

        /// <summary>
        /// Creates and fills the solidworks property page
        /// </summary>
        /// <returns>Returns the newly created property page</returns>
        public override JointPMPage FillPropertyPage()
        {
            JointPMPage page = base.FillPropertyPage();
            if(!page.SelectionObservers.Contains(this))
                page.SelectionObservers.Add(this);
            AddPropertiesToSWPage(page, ref page.currentId, ref page.currentMark);
            UpdateOriginPoint();
            Axis1.CalcLimits(-1,null);
            Axis1.DrawAxisPreview();
            //Axis1.AddAxisToPage
            return page;
        }

        

        /// <summary>
        /// Handler for saving all selections
        /// </summary>
        public override void SaveSelections()
        {
            base.SaveSelections();
            return;
        }

        /// <summary>
        /// Handler for saving selections from a specific selection box
        /// </summary>
        /// <param name="Id"></param>
        public override void SaveSpecificSelection(int Id)
        {
            base.SaveSpecificSelection(Id);
            return;
                
        }

        /// <summary>
        /// Called when a selection box is cleared of all entries
        /// </summary>
        /// <param name="Id">TheID of the selectionbox</param>
        public override void NoSelections(int Id)
        {
            base.NoSelections(Id);
            if (Id == AxisSelBoxID)
            {
                Axis1.Axis = null;
                Axis1.DrawAxisPreview();
            }
                
        } 

        /// <summary>
        /// Checks if the given selection is valid.
        /// Will return true if the selection is from the axis box and the axis is linear
        /// </summary>
        /// <param name="isValid">The value that will be returned based on the validity. will be true if the selection is valid, otherwise is false</param>
        /// <param name="Id">The Id of the selection box</param>
        /// <param name="SelType">The type of the selection</param>
        /// <param name="selection">The selected object</param>
        public override void CheckValidSelection(out bool isValid, int Id, int SelType, object selection)
        {
            if (Id == AxisSelBoxID)
            {
                isValid = IJointAxis.IsLinearAxis(selection);
                if(isValid)
                    Axis1.Axis = selection;
                return;
            }
            else
                isValid = false;
            base.CheckValidSelection(out isValid, Id, SelType, selection);
        }

        /// <summary>
        /// Clears all limits in the joint
        /// </summary>
        public override void ClearValues()
        {
            Axis1.ClearLimits();
        }

        /// <summary>
        /// Gets the primary axis direction
        /// </summary>
        /// <returns></returns>
        public override MathVector GetAxisVector()
        {
            return RobotInfo.mathUtil.CreateVector(new double[] { Axis1.AxisX, Axis1.AxisY, Axis1.AxisZ });
        }

        /// <summary>
        /// Updates the axis previews
        /// </summary>
        public override void UpdatePreviews()
        {
            Axis1.CalcLimits(-1,null);
            Axis1.DrawAxisPreview();
        }
    }
}
