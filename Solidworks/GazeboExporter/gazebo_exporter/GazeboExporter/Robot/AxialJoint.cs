using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using SolidWorks.Interop.sldworks;
using GazeboExporter.UI;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// An Axial joint is a joint that has at least one axis that defines it's movement
    /// </summary>
    public abstract class AxialJoint: JointSpecifics
    {
        protected readonly MathUtility matUtil;

        /// <summary>
        /// Creates a new Axial Joint
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        public AxialJoint(Joint joint, string path):
            base(joint,path)
        {
            this.matUtil = (MathUtility)swApp.GetMathUtility();
        }

        /// <summary>
        /// recalculates the origin point
        /// </summary>
        public override void UpdateOriginPoint()
        {
            base.UpdateOriginPoint();
            double[] COM = { joint.Child.ComX, joint.Child.ComY, joint.Child.ComZ };
            OriginValues.Point = CalcOrigin(COM);
        }

        /// <summary>
        /// Gets teh vector of the primary axis for the joint
        /// </summary>
        /// <returns>The direction vector of the primary axis</returns>
        public abstract MathVector GetAxisVector();
        
        /// <summary>
        /// Adds the properties of this joint to the solidwoprks page
        /// </summary>
        /// <param name="page">Page to add the properties to</param>
        /// <param name="id">Id to start numbering new controls with. Will be returned with the next avaliable ID</param>
        /// <param name="mark">The mark to start with for any new selection boxes. Will be returned with the next avalible mark</param>
        public abstract void AddPropertiesToSWPage(JointPMPage page, ref int id, ref int mark);



    }
}
