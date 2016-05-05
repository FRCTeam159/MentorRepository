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
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Windows.Media.Media3D;
using System.Drawing;
using System.Reflection;

namespace GazeboExporter.Robot
{

    /// <summary>
    /// This class represnts a robot that can be simulated in gazebo
    /// it stores parameters related to the assembly it's attached to
    /// that are used to export it to gazebo.
    /// </summary>
    public class RobotModel : ISelectable
    {

        #region Fields and the Properties

        /// <summary>
        /// Links in the robot
        /// </summary>
        public Dictionary<int,Link> Links { get; private set; }

        private DoubleStorageArray LinkNums { get; set; }

        /// <summary>
        /// Fields stored in attributes
        /// </summary>
        public String Name
        {
            get
            {
                return swData.GetString("robot/name");
            }
            set
            {
                swData.SetString("robot/name", value);
            }
        }

        /// <summary>
        /// The configuration that the robot pose is defined in
        /// </summary>
        public string ConfigName
        {
            get
            {
                return swData.GetString("robot/configName");
            }
            set
            {
                swData.SetString("robot/configName", value);
            }
        }
        
        /// <summary>
        /// The year of the frc field that should be inclued in the exported world. 0 means no field
        /// </summary>
        public int FRCfield
        {
            get
            {
                return (int)swData.GetDouble("robot/FRCfield");
            }
            set
            {
                swData.SetDouble("robot/FRCfield", value);
            }
        }

        /// <summary>
        /// Type of file to export as. 0 is SDF, 1 is URDF
        /// </summary>
        public int ExportType
        {
            get
            {
                return (int)swData.GetDouble("robot/ExportType");
            }
            set
            {
                swData.SetDouble("robot/ExportType", value);
            }
        }

        /// <summary>
        /// flag used for ending the export early
        /// </summary>
        public bool ContinueExport { get; set;}


        //Intefrace for interacting with SolidWorks
        SldWorks swApp;
        //Allows access to this robot's assembly document
        AssemblyDoc asmDoc;
        //Allows access to this robot's model document
        ModelDoc2 modelDoc;
        //Object for storing data inside the solidworks model document
        private StorageModel swData;

        private int nextLinkNum;


        #region frame of reference objects

        /// <summary>
        /// The origin point of the robot
        /// </summary>
        public object OriginPt
        {
            get
            {
                return swData.GetObject("robot/originPt");
            }
            set
            {
                swData.SetObject("robot/originPt",value);
                CalcOrigin();

            }
        }

        //Origin and rotation of the robot in absolute space
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double OriginZ { get; set; }
        public double OriginR { get; set; }
        public double OriginP { get; set; }
        public double OriginW { get; set; }

        /// <summary>
        /// The base plane on which the robot sits
        /// </summary>
        public object BasePlane
        {
            get
            {
                return swData.GetObject("robot/BasePlane");
            }
            set
            {
                swData.SetObject("robot/BasePlane", value);
            }
        }

        /// <summary>
        /// true if the direction axis should be flipped
        /// </summary>
        public bool BaseIsFlipped
        {
            get
            {
                return swData.GetDouble("robot/BaseIsFlipped") == 1;
            }
            set
            {
                swData.SetDouble("robot/BaseIsFlipped", value ? 1 : 0);
            }
        }

        /// <summary>
        /// The direction (axis) that the robot is facing
        /// </summary>
        public object Direction
        {
            get
            {
                return swData.GetObject("robot/direction");
            }
            set
            {
                swData.SetObject("robot/direction", value);
            }
        }

        //Axis of direction
        public double AxisX { get; set; }
        public double AxisY { get; set; }
        public double AxisZ { get; set; }

        /// <summary>
        /// true if the direction axis should be flipped
        /// </summary>
        public bool AxisIsFlipped
        {
            get
            {
                return swData.GetDouble("robot/AxisIsFlipped") == 1;
            }
            set
            {
                if (value != AxisIsFlipped)
                {
                    AxisX = -AxisX;
                    AxisY = -AxisY;
                    AxisZ = -AxisZ;
                    CalcRotations();
                }
                swData.SetDouble("robot/AxisIsFlipped", value ? 1 : 0);
            }
        }


        #endregion 

        #endregion

        #region Constructor

        /// <summary>
        /// Loads a robot from an assembly document, if a robot dosn't already
        /// exist one will be created
        /// </summary>
        /// <param name="asm">Assembly document containing a robot model</param>
        /// <param name="swApp">Interface for interacting with Solidworks</param>
        public RobotModel(AssemblyDoc asm, SldWorks swApp)
        {
            
            RobotInfo.WriteToLogFile("Robot Created (Robot)");

            var assembly = typeof(JointSpecifics).Assembly;
            Type[] types = assembly.GetTypes().Where(
                t => t.IsSubclassOf(typeof(JointSpecifics)) && !t.IsAbstract).ToArray();
            foreach (Type t in types)
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(t.TypeHandle);
            }
            RobotInfo.WriteToLogFile("Initialized JointTypes = " + String.Join(", ",JointFactory.GetTypesList()) + " (Robot)");

            //Setup fields
            this.swApp = swApp;
            this.asmDoc = asm;
            this.modelDoc = (ModelDoc2)asm;
            swData = new StorageModel(modelDoc);
            Selected = false;

            RobotInfo.SetProperties(swApp, asmDoc, swData, this);
            RobotInfo.WriteToLogFile("Setup Fields Setup");
            //If the robot data dosn't exist yet, create it with default values
            if (swData.GetDouble("robot") == 0)
            {
                swData.SetDouble("robot", 1);
                /*PhysicalConfig = "Default";
                VisualConfig = "Default";
                CollisionConfig = "Default";*/
                Name = ((ModelDoc2)asm).GetTitle();
                RobotInfo.WriteToLogFile("Robot Data created with Default Values");
            }

            RobotInfo.WriteToLogFile("Robot Data created");

            LinkNums = new DoubleStorageArray(swData, "robot/linkNums");
            nextLinkNum = 0;
            RobotInfo.WriteToLogFile("LinkNums Storage Array Created");

            Links = new Dictionary<int,Link>();
            //Load link structure
            Link newLink;
            if (LinkNums.Count == 0)
            {
                LinkNums.AddItem(0);
                RobotInfo.WriteToLogFile("New Link added to LinkNums");
            }

            Configuration currentConfig = modelDoc.ConfigurationManager.ActiveConfiguration;

            foreach (double d in LinkNums)
            {
                newLink = new Link("robot/link" + (int)d, (int)d);
                RobotInfo.WriteToLogFile("New Link Created");
                Links.Add((int)d,newLink);
                if (d >= nextLinkNum)
                    nextLinkNum = (int)d + 1;
            }
            Links[0].isBaseLink = true;

            foreach (Link l in Links.Values.ToArray())
            {
                l.InitializeJoints();
                l.InitializeAttachments();
            }
            modelDoc.ShowConfiguration2(ConfigName);
            CalcAxisVectors();
            CalcOrigin();

            modelDoc.ShowConfiguration2(currentConfig.Name);
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a link to the robot
        /// </summary>
        public Link AddLink()
        {
            RobotInfo.WriteToLogFile("Creating new link (Robot)");
            Link tempLink = new Link("robot/link" + nextLinkNum, nextLinkNum);
            tempLink.Name = "NewLink" + nextLinkNum.ToString();
            RobotInfo.WriteToLogFile("Link created: " + tempLink.Name + " (Robot)");
            Links.Add(nextLinkNum, tempLink);            
            LinkNums.AddItem(nextLinkNum);
            nextLinkNum++;
            return tempLink;
        }

        /// <summary>
        /// returns the link with the given Id
        /// </summary>
        /// <param name="id"> The Id of the desired link</param>
        /// <returns> The link with the Id</returns>
        public Link GetLink(int id)
        {
            Link l;
            Links.TryGetValue(id, out l);
            return l;
        }

        /// <summary>
        /// Gets an array containg all links in the robot
        /// </summary>
        /// <returns>Array containing all links in the robot</returns>
        public Link[] GetLinksAsArray()
        {
            return Links.Values.ToArray();
        }

        /// <summary>
        /// creates a joint between the 2 specified links
        /// </summary>
        /// <param name="parent">The parent link of the joint</param>
        /// <param name="child">The child link of the joint</param>
        /// <returns>newly created joint</returns>
        public Joint AddConnection(Link parent, Link child)
        {           
            return child.AddParentJoint(parent);
        }

        public void DeleteLink(Link link)
        {
            Links.Remove(link.Id);
            LinkNums.RemoveItem(link.Id);
            RobotInfo.WriteToLogFile(link.Id + " has been deleted");
        }


        /// <summary>
        /// opens link editor page in solidworks window
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="manager"></param>
        public void OpenRobotEditorPage(RobotModel robot, ManageRobot manager)
        {
            RobotPMPage editor = new RobotPMPage(robot, asmDoc, swApp); //SetupPage();
            editor.Show();
            editor.RestoreManager += manager.ExternalSelect;
            editor.OnRobotSelectionChanged(this);            //calls SaveCurrentLinkSelection() and LoadCurrentLinkSelection();

        }

        /// <summary>
        /// Gets a transform matrix representing the transform of the robot model
        /// </summary>
        /// <returns></returns>
        public Matrix3D GetTransformMatrix()
        {
            Matrix3D mat = new Matrix3D();
            Vector3D offset = new Vector3D(-OriginX, -OriginY, -OriginZ);
            mat.Translate(offset);
            Vector3D xAxis = new Vector3D(1, 0, 0);
            Quaternion roll = new Quaternion(xAxis, OriginR * 180 / Math.PI);
            Vector3D yAxis = new Vector3D(0, 1, 0);
            Quaternion pitch = new Quaternion(yAxis, OriginP * 180 / Math.PI);
            Vector3D zAxis = new Vector3D(0, 0, 1);
            Quaternion yaw = new Quaternion(zAxis, OriginW * 180 / Math.PI);
            mat.Rotate(roll);
            mat.Rotate(pitch);
            mat.Rotate(yaw);

            RobotInfo.WriteToLogFile("Transform Matrix Created");
            return mat;
        }

        /// <summary>
        /// Gets the offset from the baseplane to the origin of the model
        /// </summary>
        /// <returns></returns>
        public double GetBasePlaneOffset()
        {
            if (BasePlane == null)
                return 0;
            MathUtility mathUtil = swApp.GetMathUtility();
            MathVector basePlaneNormal = null;
            MathTransform componentTransform = null;
            RobotInfo.WriteToLogFile("Base Plane Offset Obtained");
            //stores the component transform if there is one. this will be used to convert the vectors to the global coordinate system
            if ((Component2)((IEntity)BasePlane).GetComponent() != null)
            {
                componentTransform = ((Component2)((IEntity)BasePlane).GetComponent()).Transform2;
            }

            //if base plane is a face
            if (BasePlane is IFace2)
            {
                basePlaneNormal = mathUtil.CreateVector(((IFace2)BasePlane).Normal);
                if (BaseIsFlipped)
                    basePlaneNormal = basePlaneNormal.Scale(-1);
                double[] closestPoint = ((IFace2)BasePlane).GetClosestPointOn(OriginX, OriginY, OriginZ);//finds the closest point on the face
                double[] transformedPoint = mathUtil.CreatePoint(new double[] { closestPoint[0], closestPoint[1], closestPoint[2] }).MultiplyTransform(componentTransform).ArrayData;//transforms the point to global space
                MathVector pointVector = mathUtil.CreateVector(new double[] { transformedPoint[0] - OriginX, transformedPoint[1] - OriginY, transformedPoint[2] - OriginZ }); ;//creates a vector between the point and the joint origin
                return -pointVector.Dot(basePlaneNormal);//projects point vector on to axis
            }
            //if limit is a reference plane
            else
            {
                object tempObject = ((IFeature)BasePlane).GetSpecificFeature2();
                if (tempObject is IRefPlane)
                {
                    double[] tempArray = { 0, 0, 1 };//temp vector used to find normal vector of plane
                    MathVector planeNormalVector = mathUtil.CreateVector(tempArray);
                               
                    MathTransform planeTransform = ((IRefPlane)tempObject).Transform;//gets the transform of the plane
                    if (componentTransform != null)//if the plane is part of a sub component, transform it again into global space
                    {
                        planeTransform = planeTransform.Multiply(componentTransform);
                    }
                    basePlaneNormal = planeNormalVector.MultiplyTransform(planeTransform).Normalise();
                    //return normalVector = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9] - robot.OriginX, planeTransform.ArrayData[10] - robot.OriginY, planeTransform.ArrayData[11] - robot.OriginZ });//create vector between joint origin and point on plane
                    if (BaseIsFlipped)
                        basePlaneNormal = basePlaneNormal.Scale(-1);
                    MathVector pointVector = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9] - OriginX, planeTransform.ArrayData[10] - OriginY, planeTransform.ArrayData[11] - OriginZ });//create vector between joint origin and point on plane
                    return pointVector.Dot(basePlaneNormal);//project point vector on to axis
                }
            }
            return 0;
        }

        /// <summary>
        /// Calculates the orgin of this robot in absolute space based on the point selected
        /// </summary>
        public void CalcOrigin()
        {
            if (OriginPt == null)
                return;
            RobotInfo.WriteToLogFile("Origin Point Calculated");

            //convert origin from object to MathPoint
            MathUtility matUtil = swApp.GetMathUtility();
            MathPoint transformedPt = null;
            if (OriginPt is IVertex)
            {
                IVertex originFeature = (IVertex)OriginPt;
                transformedPt = matUtil.CreatePoint(originFeature.GetPoint()).MultiplyTransform(((IEntity)originFeature).GetComponent().Transform2);

            }
            else if (OriginPt is IFeature)
            {
                IRefPoint originFeature = ((IFeature)OriginPt).GetSpecificFeature2();
                if (((IEntity)originFeature).GetComponent() != null)
                {
                    transformedPt = originFeature.GetRefPoint().MultiplyTransform(((IEntity)originFeature).GetComponent().Transform2);
                }
                else
                {
                    transformedPt = originFeature.GetRefPoint();
                }
            }
            RobotInfo.WriteToLogFile("Origin Point converted from object to MathPoint");
            //save double values from the MathPoint in the robot fields
            double[] tempArr = transformedPt.ArrayData;
            OriginX = tempArr[0];
            OriginY = tempArr[1];
            OriginZ = tempArr[2];
            return;
        }

        /// <summary>
        /// Calculates the axis components of this robot in absolute space based on the axis selected
        /// </summary>
        public void CalcAxisVectors()
        {
            if (Direction == null)
                return;
            MathVector axisVector;
            double[] points = { 0, 0, 0 };
            IMathUtility mathUtil = (((IMathUtility)swApp.GetMathUtility()));
            if (Direction is IFeature)
            {
                IRefAxis axis = ((IFeature)Direction).GetSpecificFeature2();
                points = axis.GetRefAxisParams();
                if (((IEntity)Direction).GetComponent() != null)
                {
                    axisVector = mathUtil.CreateVector(new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] }).
                        MultiplyTransform(((Component2)((IEntity)Direction).GetComponent()).Transform2).Normalise();
                    points = mathUtil.CreatePoint(new double[] { points[0], points[1], points[2] }).
                        MultiplyTransform(((Component2)((IEntity)Direction).GetComponent()).Transform2).ArrayData;
                }
                else
                {
                    axisVector = mathUtil.CreateVector(new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] }).Normalise();
                }
            }
            else if (Direction is IEdge)
            {
                IEdge axis = (IEdge)Direction;
                MathTransform compTrans = ((Component2)((IEntity)Direction).GetComponent()).Transform2;
                double[] startPoint = axis.GetStartVertex().GetPoint();
                double[] endPoint = axis.GetEndVertex().GetPoint();
                axisVector = mathUtil.CreateVector(new double[] { endPoint[0] - startPoint[0], endPoint[1] - startPoint[1], endPoint[2] - startPoint[2] }).MultiplyTransform(((Component2)((IEntity)axis).GetComponent()).Transform2).Normalise();
                points = mathUtil.CreatePoint(startPoint).MultiplyTransform(((Component2)((IEntity)axis).GetComponent()).Transform2).ArrayData;
            }
            else
            {
                axisVector = mathUtil.CreateVector(new double[] { 1, 0, 0 }).Normalise();
            }
            double[] tempArr = axisVector.ArrayData;

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
            CalcRotations();
            RobotInfo.WriteToLogFile("Axis Components Calculated");
        }

        /// <summary>
        /// Calculates the rotation of the model
        /// </summary>
        public void CalcRotations()
        {
            if (Direction == null || BasePlane == null)
                return;
            MathUtility mathUtil = swApp.GetMathUtility();
            MathVector axisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });
            MathVector basePlaneNormal = null;
            MathTransform componentTransform = null;
            RobotInfo.WriteToLogFile("Model Rotation calculated");

            //stores the component transform if there is one. this will be used to convert the vectors to the global coordinate system
            if ((Component2)((IEntity)BasePlane).GetComponent() != null)
            {
                componentTransform = ((Component2)((IEntity)BasePlane).GetComponent()).Transform2;
                RobotInfo.WriteToLogFile("Component Transform was stored");
            }

            //if base plane is a face
            if (BasePlane is IFace2)
            {
                basePlaneNormal = mathUtil.CreateVector(((IFace2)BasePlane).Normal);
            }
            //if limit is a reference plane
            else
            {
                object tempObject = ((IFeature)BasePlane).GetSpecificFeature2();
                if (tempObject is IRefPlane)
                {
                    MathTransform planeTransform = ((IRefPlane)tempObject).Transform;//gets the transform of the plane
                    if (componentTransform != null)//if the plane is part of a sub component, transform it again into global space
                    {
                        planeTransform = planeTransform.Multiply(componentTransform);
                    }
                    //return normalVector = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9] - robot.OriginX, planeTransform.ArrayData[10] - robot.OriginY, planeTransform.ArrayData[11] - robot.OriginZ });//create vector between joint origin and point on plane
                    basePlaneNormal = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9], planeTransform.ArrayData[10], planeTransform.ArrayData[11] });
                }
            }
            if (BaseIsFlipped)
                basePlaneNormal = basePlaneNormal.Scale(-1);

            MathVector crossVector = basePlaneNormal.Cross(axisVector).Normalise();

            double yaw = Math.Atan2(crossVector.ArrayData[0], axisVector.ArrayData[0]);
            double[] basePlneArr = basePlaneNormal.ArrayData;
            double pitch = Math.Atan2(-basePlneArr[0], Math.Sqrt(Math.Pow(basePlneArr[1], 2) + Math.Pow(basePlneArr[2], 2)));
            double roll = Math.Atan2(basePlneArr[1], basePlneArr[2]);

            OriginR = roll;
            OriginW = yaw;
            OriginP = pitch;
        }

        #endregion

        #region Export

        /// <summary>
        /// Estimate the nuber of operations needed to verify the robot
        /// </summary>
        /// <returns> The number of operations that will be needed to verify the model </returns>
        public int EstimateVerifyOps()
        {
            int ops =1;
            
            foreach (Link l in GetLinksAsArray())
            {
                ops += l.EstimateVerifyOps();
            }
            return ops;
        }

        /// <summary>
        /// Recalculates the mass properties for the robot and checks for issues
        /// </summary>
        /// <param name="log"> The ProgressLogger that messages will be printed to </param>
        /// <returns> Returns true if no errors were found and the Export was not cancelled </returns>
        public bool Verify(ProgressLogger log)
        {
            log.WriteMessage("Verifying Link Names");
            modelDoc.Rebuild((int)swRebuildOptions_e.swRebuildAll);
            Configuration currentConfig = modelDoc.ConfigurationManager.ActiveConfiguration;
            String currentDisplayState = currentConfig.GetDisplayStates()[0];
            modelDoc.ShowConfiguration2(ConfigName);
            CalcAxisVectors();
            CalcOrigin();
            //modelDoc.ShowConfiguration(PhysicalConfig);
            if (String.IsNullOrEmpty(Name) || String.IsNullOrWhiteSpace(Name))
            {
                log.WriteError("No name set for robot");
            }
            if (OriginPt == null || Direction == null || BasePlane == null)
                log.WriteWarning("No pose set for robot. Defaulting to model frame");
            foreach (Link l in Links.Values.ToArray())
            {
                l.Verify(log);
                RobotInfo.WriteToLogFile("Verifying");
                Application.DoEvents();
                if (!ContinueExport)
                    break;
            }
            modelDoc.ShowConfiguration(currentConfig.Name);
            currentConfig.ApplyDisplayState(currentDisplayState);
            return ContinueExport;
        }

        /// <summary>
        /// Estimates the number of operations needed to export the robot
        /// </summary>
        /// <returns> Returns the number of operations it will take to export the robot </returns>
        public int EstimateExportOps()
        {
            int ops = 1;
            foreach (Link l in Links.Values.ToArray())
            {
                ops += l.EstimateExportOps();
            }
            return ops;
        }

        /// <summary>
        /// Exports the robot model
        /// </summary>
        /// <param name="log"> The Proggress logger messages will be sent to </param>
        /// <param name="path"> The file path that the model will be stored to </param>
        /// <returns> Returns true if the robot was successfully exported </returns>
        public bool Export(ProgressLogger log, String path)
        {
            
            /*RobotExporter exporter = new URDFExporter(this, swApp, asmDoc);

            exporter.Export(log, path);
            WorldExporter.WriteWorldFile(Name, path);*/
            RobotExporter exporter = new SDFExporter(this, swApp, asmDoc);

            exporter.Export(log, path);
            RobotInfo.WriteToLogFile("Exporting...");
            return ContinueExport;
        }



        #endregion

        #region ISelectable Implementation and Property Editor Code

        //Forms control for editing the properties of a robot in the robot manager
        private static RobotProperties propertyEditorControl;

        //Creates and sets up the RobotProperties control that will be used by all robot objects
        private static void setupPropertyEditor()
        {
            propertyEditorControl = new RobotProperties();
        }

        //True if this ISelectable is selected in the robot manager
        public bool Selected {get; set;}

        //Gets the name of this ISeletable object to be displayed at the top of the robot
        //manager's property editor
        public string GetName()
        {
	         return "Robot"; 
        }

        /// <summary>
        /// Returns a control that can be used to edit the properties of this ISeletable object
        /// </summary>
        /// <returns>a control for this object</returns>
        public Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                setupPropertyEditor();
            propertyEditorControl.setRobot(this, modelDoc);
            return propertyEditorControl;
        }

        public void Delete()
        {

        }
        #endregion

    }

}
