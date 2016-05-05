using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.UI;
using GazeboExporter.Export;
using SolidWorks.Interop.sldworks;
using System.Xml;
using SolidWorks.Interop.swconst;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Represents the preoperties relating to a ball joint. 
    /// Ball joints have no axis of motion and simply pivot around a point
    /// </summary>
    public class BallJoint : JointSpecifics 
    {
        const string displayName = "Ball";

        const string exportName = "ball";

        /// <summary>
        /// Creates a new ball joint
        /// </summary>
        /// <param name="joint">The joint that contains this</param>
        /// <param name="path">the storage path for this jointspecifics</param>
        public BallJoint(Joint joint, string path)
            : base(joint, path)
        {

        }

        /// <summary>
        /// Writes the sdfParameters specific to the joint
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">the writer to use to write the sdf</param>
        public override void WriteSDF(ProgressLogger log, XmlWriter writer)
        {
            return;
        }

        /// <summary>
        /// Verifyies the joint for export
        /// </summary>
        /// <param name="axisName">THe name of the axis, should be in form "jointName Axis x"</param>
        /// <param name="log"></param>
        protected override bool VerifySpecifics(Export.ProgressLogger log)
        {
            if (OriginPt == null)
                log.WriteError("No origin defined in joint " + joint.Name);
            return true;
        }


        /// <summary>
        /// Calculates an origin point that is closest to the given point and on the line
        /// </summary>
        /// <param name="origPoint">POint to be close to</param>
        /// <returns>the origin point that is created</returns>
        protected override double[] CalcOrigin(double[] point)
        {
            double[] originPoint = null;
            if (OriginPt is IVertex)
            {
                IVertex originFeature = (IVertex)OriginPt;
                MathPoint transformedPt = RobotInfo.mathUtil.CreatePoint(originFeature.GetPoint()).MultiplyTransform(((IEntity)originFeature).GetComponent().Transform2);
                originPoint = transformedPt.ArrayData;
            }
            else if (OriginPt is IFeature)
            {
                IRefPoint originFeature = ((IFeature)OriginPt).GetSpecificFeature2();
                MathPoint transformedPt;
                if (((IEntity)originFeature).GetComponent() != null)
                {
                    transformedPt = originFeature.GetRefPoint().MultiplyTransform(((IEntity)originFeature).GetComponent().Transform2);
                }
                else
                {
                    transformedPt = originFeature.GetRefPoint();
                }

                originPoint = transformedPt.ArrayData;
            }
            return originPoint;
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
        static BallJoint()
        {
            JointFactory.RegisterJointType(displayName, typeof(BallJoint),7);
        }

        /// <summary>
        /// Clears any values that need to be reset
        /// </summary>
        public override void ClearValues()
        {
        }

        /// <summary>
        /// Updates any previews of the joint
        /// </summary>
        public override void UpdatePreviews()
        {
        }
    }
}
