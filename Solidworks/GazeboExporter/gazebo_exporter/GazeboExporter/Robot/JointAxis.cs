using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using GazeboExporter.Export;
using GazeboExporter.GazeboException;
using GazeboExporter.Storage;
using GazeboExporter.UI;



namespace GazeboExporter.Robot
{
    /// <summary>
    /// Summary description for Class1
    /// </summary>
    public class JointAxis
    {
        

        #region Fields and Properties
        public static String[] JointAxis1Names = { null, null, "Axis of Rotation:", "Axis of Rotation:", "Axis of Translation" };
        public static String[] JointAxis2Names = { null, null, null, null, null };

        public int AxisIndex; //1 or 2

        //The storage model that this link is saved with
        StorageModel swData;

        //Storage path of this joint in the StorageModel
        public String path;
        //Allows access to this joint's assembly document
        AssemblyDoc asmDoc;
        //Allows access to this joint's model document
        ModelDoc2 modelDoc;

        SldWorks swApp;

        Joint owner;

        RobotModel robot;

        /// <summary>
        /// The axis that the joint is on
        /// </summary>
        public object Axis
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(swData.GetString(path + "/axis" + AxisIndex.ToString()));
                object axis = modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
                int Errors2;
                object axis2 = modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors2);//called twice due to some inconsistant behaviour in the getObjectByPersustReference3 method
                if (Errors == (int)swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok && Errors2 == (int)swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok)
                {
                    return axis;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value == null)
                {
                    swData.Delete(path + "/axis" + AxisIndex.ToString());
                }
                else
                {
                    byte[] pid = modelDoc.Extension.GetPersistReference3(value);
                    swData.SetString(path + "/axis" + AxisIndex.ToString(), Convert.ToBase64String(pid));
                    CalcAxisVectors();

                }
            }
        }

        /// <summary>
        /// true if the axis of movment/rotation should be flipped
        /// </summary>
        public bool AxisIsFlipped
        {
            get
            {
                return swData.GetDouble(path + "/AxisIsFlipped" + AxisIndex.ToString()) == 1;
            }
            set
            {
                if (value != AxisIsFlipped)
                {
                    AxisX = -AxisX;
                    AxisY = -AxisY;
                    AxisZ = -AxisZ;
                }
                swData.SetDouble(path + "/AxisIsFlipped" + AxisIndex.ToString(), value ? 1 : 0);
            }
        }

        #region Motion limit properties
        /// <summary>
        /// Whether the axis is continuous (no limits)
        /// </summary>
        public bool IsContinuous
        {
            get
            {
                return swData.GetDouble(path + "/isContinuous" + AxisIndex.ToString()) == 1;
            }
            set
            {
                swData.SetDouble(path + "/isContinuous" + AxisIndex.ToString(), value ? 1 : 0);
            }
        }

        /// <summary>
        /// The point on the link that will be used to define its lower limit
        /// </summary>
        public object LowerLimitEdge
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(swData.GetString(path + "/lowerLimitEdge" + AxisIndex.ToString()));
                return modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    swData.Delete(path + "/lowerLimitEdge" + AxisIndex.ToString());
                }
                else
                {
                    byte[] pid = modelDoc.Extension.GetPersistReference3(value);
                    swData.SetString(path + "/lowerLimitEdge" + AxisIndex.ToString(), Convert.ToBase64String(pid));
                }
                
            }
        }

        /// <summary>
        /// A point not on the link that the lowerLinkEdge will reach at its furthest extent
        /// </summary>
        public object LowerLimitStop
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(swData.GetString(path + "/lowerLimitStop" + AxisIndex.ToString()));
                return modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    swData.Delete(path + "/lowerLimitStop" + AxisIndex.ToString());
                }
                else
                {
                    byte[] pid = modelDoc.Extension.GetPersistReference3(value);
                    swData.SetString(path + "/lowerLimitStop" + AxisIndex.ToString(), Convert.ToBase64String(pid));
                }
            }
        }

        /// <summary>
        /// The point on the link that will be used to define its upper limit
        /// </summary>
        public object UpperLimitEdge
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(swData.GetString(path + "/upperLimitEdge" + AxisIndex.ToString()));
                return modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    swData.Delete(path + "/upperLimitEdge" + AxisIndex.ToString());
                }
                else
                {
                    byte[] pid = modelDoc.Extension.GetPersistReference3(value);
                    swData.SetString(path + "/upperLimitEdge" + AxisIndex.ToString(), Convert.ToBase64String(pid));
                }
            }
        }

        /// <summary>
        /// A point not on the link that the upperLinkEdge will reach at its furthest extent
        /// </summary>
        public object UpperLimitStop
        {
            get
            {
                int Errors;
                byte[] pid = Convert.FromBase64String(swData.GetString(path + "/upperLimitStop" + AxisIndex.ToString()));
                return modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            }
            set
            {
                if (value == null)
                {
                    swData.Delete(path + "/upperLimitStop" + AxisIndex.ToString());
                }
                else
                {
                    byte[] pid = modelDoc.Extension.GetPersistReference3(value);
                    swData.SetString(path + "/upperLimitStop" + AxisIndex.ToString(), Convert.ToBase64String(pid));
                }
            }
        }

        /// <summary>
        /// Array of the vectors that represent each of the limits
        /// </summary>
        public MathVector[] limitVectors { get; set; }

        /// <summary>
        /// Lower limit of the joint (m or rad)
        /// This is the distance that the link can travel backwards. Should be a negative number
        /// </summary>
        public double LowerLimit
        {
            get
            {
                return swData.GetDouble(path + "/lowerLimit" + AxisIndex.ToString());
            }
            set
            {
                if (value > 0)
                {
                    swData.SetDouble(path + "/lowerLimit" + AxisIndex.ToString(), 0);
                }
                else
                {
                    swData.SetDouble(path + "/lowerLimit" + AxisIndex.ToString(), value);
                }
                
            }
        }

        

        /// <summary>
        /// Upper limit of the joint (m or rad)
        /// This is the distance that the link can travel forwards
        /// </summary>
        public double UpperLimit
        {
            get
            {
                return swData.GetDouble(path + "/upperLimit" + AxisIndex.ToString());
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/upperLimit" + AxisIndex.ToString(), 0);
                }
                else
                {
                    swData.SetDouble(path + "/upperLimit" + AxisIndex.ToString(), value);
                }
                
            }
        }
        #endregion

        /// <summary>
        /// velocity limit of the joint (m/s or rad/s)
        /// </summary>
        public double VelocityLimit
        {
            get
            {
                return swData.GetDouble(path + "/velocityLimit" + AxisIndex.ToString());
            }
            set
            {
                swData.SetDouble(path + "/velocityLimit" + AxisIndex.ToString(), value);
            }
        }
        /// <summary>
        /// Effort limit of the joint (N or Nm)
        /// </summary>
        public double EffortLimit
        {
            get
            {
                return swData.GetDouble(path + "/effortLimit" + AxisIndex.ToString());
            }
            set
            {
                swData.SetDouble(path + "/effortLimit" + AxisIndex.ToString(), value);
            }
        }

        /// <summary>
        /// The damping that will be in the joint
        /// </summary>
        public double Damping
        {
            get
            {
                return swData.GetDouble(path + "/damping" + AxisIndex.ToString());
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/damping" + AxisIndex.ToString(), 0);
                }
                else
                {
                    swData.SetDouble(path + "/damping" + AxisIndex.ToString(), value);
                }
                
            }
        }

        /// <summary>
        /// The friction in the joint
        /// </summary>
        public double Friction
        {
            get
            {
                return swData.GetDouble(path + "/friction" + AxisIndex.ToString());
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/friction" + AxisIndex.ToString(), 0);
                }
                else
                {
                    swData.SetDouble(path + "/friction" + AxisIndex.ToString(), value);
                }
                
            }
        }

        /// <summary>
        /// Represents a point on the axis
        /// </summary>
        public double[] Point { get; set; }

    
        //Axis of motion
        public double AxisX { get; set; }
        public double AxisY { get; set; }
        public double AxisZ { get; set; }
    #endregion

    #region Constructors
        public JointAxis(SldWorks swApp, AssemblyDoc asm, StorageModel swData, string path, Joint current, int index, RobotModel robot)
        {
            this.swApp = swApp;
            this.asmDoc = asm;
            this.modelDoc = (ModelDoc2)asm;
            this.swData = swData;
            this.path = path;
            this.owner = current;
            this.robot = robot;

            this.AxisIndex = index;


            if (EffortLimit == 0)
            {
                this.EffortLimit = 1;
            }
            if (this.VelocityLimit == 0)
            {
                this.VelocityLimit = 1;
            }


            if (Axis != null)
            {
                CalcAxisVectors();
                //CalcLimits(null);
            }
            if (index == 2)
            {
                IsContinuous = true;
            }

        }




    #endregion 


    #region Public methods

        /// <summary>
        /// Verifies that the axis is ready for export
        /// </summary>
        /// <returns>Returns true if succesfully verified</returns>
        public void Verify(Joint joint, ProgressLogger log)
        {
            CalcAxisVectors();
            if (Axis == null)
                log.WriteError("No axis " + AxisIndex + " defined in joint " + joint.Name);
            if (!IsContinuous && LowerLimit == UpperLimit)
                log.WriteWarning("No movement defined in joint " + joint.Name + " axis" + AxisIndex);
            if (Friction == 0)
                log.WriteWarning("No friction in joint " + joint.Name + " axis" + AxisIndex);
            if (Damping == 0)
                log.WriteError("No damping in joint " + joint.Name + " axis" + AxisIndex);
        }
    #endregion
    #region Calculate



        public void CalcAxisVectors()
        {
            MathVector axisVector;
            double[] points = { 0, 0, 0 };
            IMathUtility matUtil = (((IMathUtility)swApp.GetMathUtility()));
            if (Axis is IFeature)
            {
                IRefAxis axis = ((IFeature)Axis).GetSpecificFeature2();
                points = axis.GetRefAxisParams();
                if (((IEntity)Axis).GetComponent() != null)
                {
                    axisVector = matUtil.CreateVector(new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] }).MultiplyTransform(((Component2)((IEntity)Axis).GetComponent()).Transform2).Normalise();
                    points = matUtil.CreatePoint(new double[] { points[0], points[1], points[2] }).MultiplyTransform(((Component2)((IEntity)Axis).GetComponent()).Transform2).ArrayData;
                }
                else
                {
                    axisVector = matUtil.CreateVector(new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] }).Normalise();
                }
            }
            else if (Axis is IEdge)
            {
                IEdge axis = (IEdge)Axis;
                MathTransform compTrans = ((Component2)((IEntity)Axis).GetComponent()).Transform2;
                double[] startPoint = axis.GetStartVertex().GetPoint();
                double[] endPoint = axis.GetEndVertex().GetPoint();
                axisVector = matUtil.CreateVector(new double[] { endPoint[0] - startPoint[0], endPoint[1] - startPoint[1], endPoint[2] - startPoint[2] }).MultiplyTransform(((Component2)((IEntity)axis).GetComponent()).Transform2).Normalise();
                points = matUtil.CreatePoint(startPoint).MultiplyTransform(((Component2)((IEntity)axis).GetComponent()).Transform2).ArrayData;
            }
            else
            {
                axisVector = matUtil.CreateVector(new double[] { 1, 0, 0 }).Normalise();
            }
            double[] tempArr = axisVector.ArrayData;

            Point = points;
            if (AxisIsFlipped)
            {
                AxisX = -tempArr[0];
                AxisY = -tempArr[1];
                AxisZ = -tempArr[2];
            }
            else
            {
                AxisX = tempArr[0];
                AxisY = tempArr[1];
                AxisZ = tempArr[2];
            }
        }
        //public bool CalcLimits(object[] limitObjs)

    #endregion




}
}