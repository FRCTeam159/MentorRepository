using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using GazeboExporter.GazeboException;
using System.Xml;
using GazeboExporter.Storage;
using System.Windows.Forms;
using GazeboExporter.Export;
using System.Threading;
using GazeboExporter.UI;
using System.Drawing;
using System.Windows.Media.Media3D;

namespace GazeboExporter.Robot
{

    /// <summary>
    /// This class represents a robot link, a piece of the robot that represents
    /// a single rigid body in Gazebo.
    /// </summary>
    public class Link : ISelectable
    {

        #region Visible Properties

        /// <summary>
        /// Name of this link
        /// </summary>
        public String Name
        {
            get
            {
                return swData.GetString(path + "/name");
            }
            set
            {
                swData.SetString(path + "/name", value);
            }
        }
    
        /// <summary>
        /// Color of this link in the form 0xBBGGRR
        /// </summary>
        public int color
        {
            get
            {
                return (int)swData.GetDouble(path + "/color");
            }
            set
            {
                swData.SetDouble(path + "/color", value);
            }
        }

        /// <summary>
        /// The red component of the link color
        /// </summary>
        public int ColorRed
        {
            get
            {
                return (color & 0xFF);
            }
            set
            {
                color &= 0xFFFF00;
                color |= value;
            }
        }

        /// <summary>
        /// The green component of the link color
        /// </summary>
        public int ColorGreen
        {
            get
            {
                return (color >> 8) & 0xFF;
            }
            set
            {
                color &= 0xFF00FF;
                color |= (value << 8);
            }
        }

        /// <summary>
        /// The blue component of the link color
        /// </summary>
        public int ColorBlue
        {
            get
            {
                return (color >> 16) & 0xFF;
            }
            set
            {
                color &= 0x00FFFF;
                color |= (value << 16);
            }
        }

        /// <summary>
        /// Default colors for links
        /// </summary>
        private static int[] DefaultColors = {0x0000FF, //red
                                              0x00FF00, //green
                                              0xFF0000, //blue
                                              0x00FFFF, //yellow
                                              0xFFFF00, //cyan
                                              0xFF00FF, //magenta
                                              0x0080FF, //orange
                                              0x808080, //grey
                                              0x000080, //dark red
                                              0x008000, //dark green
                                              0x800080, //dark blue
                                              0x008080, //dark yellow
                                              0x808000, //dark cyan
                                              0x800080, //dark magenta
                                              0x004080, //brown
                                              0xC0C0C0}; //light grey


        

        //Origin and rotation of this link in global frame
        public double OriginX { get; set; }
        public double OriginY { get; set; }
        public double OriginZ { get; set; }
        public double OriginR { get; set; }
        public double OriginP { get; set; }
        public double OriginW { get; set; }

        //Visual mesh origin and rotation
        public double OriginXvisual { get; set; }
        public double OriginYvisual { get; set; }
        public double OriginZvisual { get; set; }
        public double OriginRvisual { get; set; }
        public double OriginPvisual { get; set; }
        public double OriginWvisual { get; set; }

        //Colision mesh origin and rotation
        public double OriginXcollision { get; set; }
        public double OriginYcollision { get; set; }
        public double OriginZcollision { get; set; }
        public double OriginRcollision { get; set; }
        public double OriginPcollision { get; set; }
        public double OriginWcollision { get; set; }

        /// <summary>
        /// This link's mass and moment of inertia matrix
        /// </summary>
        public bool UseCustomInertial
        {
            get
            {
                return (swData.GetDouble(path + "/customInertia") == 1);
            }
            set
            {
                swData.SetDouble(path + "/customInertia", value ? 1 : 0);
            }
        }

        //origin of the center of mass of the link
        public double ComX
        {
            get
            {
                return swData.GetDouble(path + "/ComX");
            }
            set
            {
                swData.SetDouble(path + "/ComX",value);
            }
        }
        public double ComY
        {
            get
            {
                return swData.GetDouble(path + "/ComY");
            }
            set
            {
                swData.SetDouble(path + "/ComY", value);
            }
        }
        public double ComZ
        {
            get
            {
                return swData.GetDouble(path + "/ComZ");
            }
            set
            {
                swData.SetDouble(path + "/ComZ", value);
            }
        }

        /// <summary>
        /// mass of the link (kg)
        /// </summary>
        public double Mass
        {
            get
            {
                return swData.GetDouble(path + "/mass");
            }
            set
            {
                swData.SetDouble(path + "/mass", value);
            }
        }

        //moments of inertia of the link
        #region moments
        public double MomentIxx
        {
            get
            {
                return swData.GetDouble(path + "/ixx");
            }
            set
            {
                swData.SetDouble(path + "/ixx", Math.Abs(value));
            }
        }
        public double MomentIxy
        {
            get
            {
                return swData.GetDouble(path + "/ixy");
            }
            set
            {
                swData.SetDouble(path + "/ixy", Math.Abs(value));
            }
        }
        public double MomentIxz
        {
            get
            {
                return swData.GetDouble(path + "/ixz");
            }
            set
            {
                swData.SetDouble(path + "/ixz", Math.Abs(value));
            }
        }
        public double MomentIyy
        {
            get
            {
                return swData.GetDouble(path + "/iyy");
            }
            set
            {
                swData.SetDouble(path + "/iyy", Math.Abs(value));
            }
        }
        public double MomentIyz
        {
            get
            {
                return swData.GetDouble(path + "/iyz");
            }
            set
            {
                swData.SetDouble(path + "/iyz", Math.Abs(value));
            }
        }
        public double MomentIzz
        {
            get
            {
                return swData.GetDouble(path + "/izz");
            }
            set
            {
                swData.SetDouble(path + "/izz", Math.Abs(value));
            }
        }
#endregion

        /// <summary>
        /// Friction value for link
        /// </summary>
        public double Mu1
        {
            get
            {
                return swData.GetDouble(path + "/mu1");
            }
            set
            {
                swData.SetDouble(path + "/mu1", Math.Abs(value));
            }
        }

        /// <summary>
        /// friction value for link
        /// </summary>
        public double Mu2
        {
            get
            {
                return swData.GetDouble(path + "/mu2");
            }
            set
            {
                swData.SetDouble(path + "/mu2", Math.Abs(value));
            }
        }

        /// <summary>
        /// contact stiffness for link
        /// </summary>
        public double Kp
        {
            get
            {
                return swData.GetDouble(path + "/kp");
            }
            set
            {
                swData.SetDouble(path + "/kp", value);
            }
        }

        /// <summary>
        /// contact dampening for link
        /// </summary>
        public double Kd
        {
            get
            {
                return swData.GetDouble(path + "/kd");
            }
            set
            {
                swData.SetDouble(path + "/kd", value);
            }
        }

        /// <summary>
        /// The linear velocity damping of the link
        /// </summary>
        public double LinearDamping
        {
            get
            {
                return swData.GetDouble(path + "/linearDamping");
            }
            set
            {
                swData.SetDouble(path + "/linearDamping", Math.Abs(value));
            }
        }

        /// <summary>
        /// The angular velocity damping of the link
        /// </summary>
        public double AngularDamping
        {
            get
            {
                return swData.GetDouble(path + "/angularDamping");
            }
            set
            {
                swData.SetDouble(path + "/angularDamping", Math.Abs(value));
            }
        }

        /// <summary>
        /// True if the link will collide with other parts of the model
        /// </summary>
        public bool SelfCollide
        {
            get
            {
                return swData.GetDouble(path + "/selfCollide") == 1;
            }
            set
            {
                swData.SetDouble(path + "/selfCollide", value ? 1 : 0);
            }
        }


        /// <summary>
        /// Array of collision and visual model configurations
        /// </summary>
        public ModelConfiguration[] LinkModels;

        /// <summary>
        /// Gets the Physical model to use for export.
        /// This should only be used when reading physical components, not if any will be set
        /// </summary>
        public ModelConfiguration PhysicalModel
        {
            get
            {
                if (LinkModels[(int)ModelConfiguration.ModelConfigType.Physical].IsEmpty())
                    return LinkModels[(int)ModelConfiguration.ModelConfigType.Visual];
                else
                    return LinkModels[(int)ModelConfiguration.ModelConfigType.Physical];
            }
        }

        /// <summary>
        /// Gets The Collision model for export
        /// This should only be used when reading collision components, not if any will be set
        /// </summary>
        public ModelConfiguration CollisionModel
        {
            get
            {
                if (LinkModels[(int)ModelConfiguration.ModelConfigType.Collision].IsEmpty())
                    return LinkModels[(int)ModelConfiguration.ModelConfigType.Visual];
                else
                    return LinkModels[(int)ModelConfiguration.ModelConfigType.Collision];
            }
        }

        /// <summary>
        /// Gets The visual model for export
        /// This should only be used when reading collision components, not if any will be set
        /// </summary>
        public ModelConfiguration VisualModel
        {
            get
            {
                return LinkModels[(int)ModelConfiguration.ModelConfigType.Visual];
            }
        }


        /// <summary>
        /// Array containing all joints that connect this link to it's parents
        /// </summary>
        public List<Joint> ParentJoints { get; private set; }

        /// <summary>
        /// Array containing all joints that connect this link to it's children
        /// </summary>
        public List<Joint> ChildJoints {get; private set;}

        /// <summary>
        /// Storage array of all parent joints in this link
        /// </summary>
        private DoubleStorageArray parentJointNums;

        
        /// <summary>
        /// The index of the next joint to be created
        /// </summary>
        private int nextJointNum;

        //Array of all Attachments in this Link
        private List<Attachment> attachments;
        //Storage array of all attachments in this Link
        private DoubleStorageArray attachmentNums;

        //number of the next attachment to be added
        private int nextAttachmentNum;

        //storage model that this link is contained in
        StorageModel swData;

        //Allows access to this link's assembly document
        AssemblyDoc asmDoc;
        //Allows access to this link's model document
        public ModelDoc2 modelDoc;

        public SldWorks swApp;
        //path to the storage location of this Link
        String path;

        public RobotModel robot;

        public int Id { get; set; }

        /// <summary>
        /// Array of the joints that sit above this link in the gui.These will mainly be parent nodes but may be child nodes if there are loops
        /// </summary>
        public Joint[] UpperJoints { get; set; }

        public bool isBaseLink { get; set; }

        public bool MarkedForDeletion;

        //GUI variables

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new link belonging to the given assembly
        /// </summary>
        /// <param name="swApp">The Solidworks App</param>
        /// <param name="asm"> The assembly that this Link is in</param>
        /// <param name="swData"> The Storage model that this Link is stored in </param>
        /// <param name="path"> The path to the StorageModel location of this link </param>
        /// <param name="baseLink"> Whether this Link is the base link of the model or not </param>
        public Link(String path, int id)
        {
            this.Selected = false;
            this.asmDoc = RobotInfo.AssemDoc;
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;
            this.swApp = RobotInfo.SwApp;
            this.Id = id;
            this.robot = RobotInfo.Robot;
            attachments = new List<Attachment>();
            ParentJoints = new List<Joint>();
            ChildJoints = new List<Joint>();
            nextAttachmentNum = 0;
            nextJointNum = 0;
            isBaseLink = false;
            

            if (swData.GetDouble(path) == 0)
            {
                swData.SetDouble(path, 1);
                Name = "NewLink";
                this.color = DefaultColors[0];
            }

            attachmentNums = new DoubleStorageArray(swData, path + "/attachmentNums");
            parentJointNums = new DoubleStorageArray(swData, path + "/jointNums");

            ModelConfiguration physical = new ModelConfiguration(path + "/physicalComps", (int)ModelConfiguration.ModelConfigType.Physical,this);
            ModelConfiguration visual = new ModelConfiguration(path + "/visualComps", (int)ModelConfiguration.ModelConfigType.Visual,this);
            ModelConfiguration collision = new ModelConfiguration(path + "/collisionComps", (int)ModelConfiguration.ModelConfigType.Collision,this);
            LinkModels = new ModelConfiguration[] {  physical, visual, collision };

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Intializes all the joints in the model
        /// </summary>
        public void InitializeJoints()
        {
            RobotInfo.WriteToLogFile("Initializing joints (Link)");
            string visualConfig = VisualModel.swConfiguration;
            if (visualConfig != null && !visualConfig.Equals(modelDoc.ConfigurationManager.ActiveConfiguration.Name))
                modelDoc.ShowConfiguration2(visualConfig);
            foreach (double d in parentJointNums)
            {

                ParentJoints.Add(new Joint(path + "/joint/" + (int)d));
                if (d >= nextJointNum)
                    nextJointNum = (int)d + 1;
            }

        }

        /// <summary>
        /// Initializes all attachments in this link
        /// </summary>
        public void InitializeAttachments()
        {
            RobotInfo.WriteToLogFile("Initializing links (Link)");
            string visualConfig = VisualModel.swConfiguration;
            if (visualConfig != null && !visualConfig.Equals(modelDoc.ConfigurationManager.ActiveConfiguration.Name))
                modelDoc.ShowConfiguration2(visualConfig);
            foreach (double d in attachmentNums)
            {
                attachments.Add(AttachmentFactory.GenerateAttachment(path + "/attachments/" + (int)d, this));
                if (d >= nextAttachmentNum)
                    nextAttachmentNum = (int)d + 1;
            }
        }

        /// <summary>
        /// Removes the selected joint from the link. Should only be called from the Joint class. Call joint.Delete() instead to delete a joint
        /// </summary>
        /// <param name="j">Joint to be removed</param>
        public void RemoveJoint(Joint j)
        {
            ///Remove all attachments from this joint

            if (ParentJoints.Contains(j))
            {
                ParentJoints.Remove(j);
                parentJointNums.RemoveItem(Convert.ToInt32(j.path.Substring(j.path.Length - 1)));
            }
            else if (ChildJoints.Contains(j))
            {
                ChildJoints.Remove(j);
            }
            if (ParentJoints.Count == 0 || !ConnectedToBaseLink())
                this.Delete();
        }

        /// <summary>
        /// Checks if the link is connected to the base link
        /// </summary>
        /// <param name="visited">All links that have already been visited by this method</param>
        /// <returns>if the link is connected to the base link</returns>
        public bool ConnectedToBaseLink(HashSet<Link> visited = null)
        {
            if(isBaseLink)
                return true;
            if(visited == null)
                visited = new HashSet<Link>();
            visited.Add(this);
            foreach (Joint j in ParentJoints)
            {
                Link parent = j.Parent;
                if (!visited.Contains(parent))
                    if (parent.isBaseLink)
                        return true;
                    else
                        if (parent.ConnectedToBaseLink(visited))
                            return true;
            }
            return false;
        }

        /// <summary>
        /// Deletes this link's attribute
        /// This object should be left to garbage collection after this
        /// method is called
        /// </summary>
        public void Delete()
        {
            /* the end of this function will result in the deletion of this link
            but before it reaches the end, it may call itself through other functions. 
            It should only run this function once at the top level, thus MarkedForDeletion 
            */
            if (MarkedForDeletion) 
                return;            
            MarkedForDeletion = true; 
            if (isBaseLink) // it is not permitted to have no links
                return;
            RobotInfo.WriteToLogFile("Deleting link joints and child links (Link)");
            Joint[] tempArr = ParentJoints.ToArray();
            foreach (Joint j in tempArr)
            {
                j.Delete();
            }
            tempArr = ChildJoints.ToArray();
            foreach (Joint j in tempArr)
            {
                j.Delete();
            }
            RobotInfo.WriteToLogFile("Deleting attachments and self. Joints have been deleted (Link)");
            Attachment[] atts = attachments.ToArray();
            foreach (Attachment a in atts)
            {
                a.Delete();
            }
            robot.DeleteLink(this);
            swData.DeleteAll(path);
            RobotInfo.WriteToLogFile("Successfully deleted link (Link)");
        }

        /// <summary>
        /// Retruns the name of the Link
        /// </summary>
        /// <returns> Returns the name of the Link /returns>
        public override string ToString()
        {
            return this.Name;
        }

        /// <summary>
        /// Adds to this link's list of parent joints and joint ids and incremenets nextJointNum
        /// </summary>
        /// <param name="parent">parent link of the new joint</param>
        /// <returns>newly created joint</returns>
        public Joint AddParentJoint(Link parent)
        {
            RobotInfo.WriteToLogFile("Creating new joint (Link)");
            Joint newJoint = new Joint(path + "/joint/" + nextJointNum, parent, this);
            RobotInfo.WriteToLogFile("Adding joint to parent list (Link)");
            ParentJoints.Add(newJoint);
            parentJointNums.AddItem(nextJointNum);
            nextJointNum++;
            return newJoint;
        }

        /// <summary>
        /// Gets all links that are a parent to this link
        /// </summary>
        /// <returns>Returns an array containing all links that are a parent to this link</returns>
        public Link[] GetParentLinks()
        {
            List<Link> parents = new List<Link>(); 
            foreach(Joint j  in ParentJoints)
            {
                parents.Add(j.Parent);
            }
            return parents.ToArray();
        }

        /// <summary>
        /// Makes a link, adds to robot, and joins to link as child
        /// </summary>
        public Link AddChild()
        {
            Link l = robot.AddLink();
            robot.AddConnection(this, l);
            return l;
        }

        /// <summary>
        /// Makes a link, adds to robot, and joins to link as parent
        /// </summary>
        public void AddParent()
        {
            Link l = robot.AddLink();
            robot.AddConnection(l, this);
        }

        /// <summary>
        /// makes joint and connects two given links
        /// </summary>
        /// <param name="parent">parent link of new joint</param>
        /// <param name="child">child link of new joint</param>
        /// <returns>newly created joint</returns>
        public Joint connectLinks(Link parent, Link child)
        {
            return robot.AddConnection(parent, child);
        }

        /// <summary>
        /// Gets all links that are a child to this link
        /// </summary>
        /// <returns>Returns an array containing all links that are a child to this link</returns>
        public Link[] GetChildLinks()
        {
            List<Link> children = new List<Link>();
            foreach (Joint j in ChildJoints)
            {
                children.Add(j.Parent);
            }
            return children.ToArray();
        }

        /// <summary>
        /// Gets the joint that connects this link to the specified link
        /// </summary>
        /// <param name="l">Link that this joint is connected to</param>
        /// <returns>The joint that connects this link and inputted link</returns>
        public Joint GetJointFromLink(Link l)
        {
            foreach (Joint j in ParentJoints)
            {
                if (j.Parent.Equals(l))
                {
                    return j;
                }
            }
            foreach (Joint j in ChildJoints)
            {
                if (j.Child.Equals(l))
                {
                    return j;
                }
            }
            return null;
        }

        /// <summary>
        /// opens link editor page in solidworks window
        /// </summary>
        /// <param name="robot"></param>
        /// <param name="manager"></param>
        public void OpenLinkEditorPage(RobotModel robot,ManageRobot manager, int modelType)
        {
            LinkPMPage editor = new LinkPMPage(robot, asmDoc, swApp, modelType); //SetupPage();
            editor.Show();
            editor.RestoreManager += manager.ExternalSelect;
            editor.OnLinkSelectionChanged(this);            //calls SaveCurrentLinkSelection() and LoadCurrentLinkSelection();
        
        }

        #endregion


        #region attachment methods (public)
        /// <summary>
        /// Gets the number of attachments in this Link
        /// </summary>
        /// <returns> Number of attachments in this Link</returns>
        public int GetNumAttachments()
        {
            return attachments.Count;
        }

        /// <summary>
        /// Gets the attachment at the specified index
        /// </summary>
        /// <param name="index"> Index of the wanted attachment </param>
        /// <returns> The attachemnt at the specified index </returns>
        public Attachment GetAttachment(int index)
        {
            return attachments.ElementAt(index);
        }

        /// <summary>
        /// Generates a new attachment and adds it to this Link
        /// </summary>
        /// <param name="id"> Type of attachment to be created </param>
        /// <returns> The attachment that was created </returns>
        public Attachment AddAttachment(int id)
        {
            Attachment att = AttachmentFactory.CreateNewAttachment(path + "/attachments/" + nextAttachmentNum, id, this);
            if (att == null) return null;
            RobotInfo.WriteToLogFile("Attachment created, adding to link (Link)");
            attachmentNums.AddItem(nextAttachmentNum);
            attachments.Add(att);
            nextAttachmentNum++;
            return att;
        }

        /// <summary>
        /// removes the inputed Attachment from this link
        /// </summary>
        /// <param name="att"> Attachment to be removed </param>
        public void RemoveAttachment(Attachment att)
        {
            RobotInfo.WriteToLogFile("Removing attachment (Link)");
            attachments.Remove(att);
            attachmentNums.RemoveItem(Convert.ToInt32(att.Path.Remove(0, path.Length + 13)));
            RobotInfo.WriteToLogFile("Successfully removed attachment from link (Link)");
        }

        /// <summary>
        /// Returns an array of all attachments in this Link
        /// </summary>
        /// <returns> Array containing all of the Attachments in this Link </returns>
        public Attachment[] GetAttachmentsAsArray()
        {
            return attachments.ToArray();
        }
        #endregion


        #region Export

        /// <summary>
        /// Estimates the number of steps it will take to verfiy this link and all subLinks
        /// </summary>
        /// <returns> Number of steps to verify this link and all subLinks </returns>
        public int EstimateVerifyOps()
        {
            int result = 1;
            result += attachments.Count;
            return result;
        }

        /// <summary>
        /// recalculates the mass properties and origins of this link and checks for any issues
        /// </summary>
        /// <param name="log"> ProgressLogger that messages should be printed to </param>
        /// <returns> Returns true if successfully verified link, otherwise returns false </returns>
        public bool Verify(ProgressLogger log)
        {
            log.WriteMessage("Verifying link " + Name);
            string currentConfig = modelDoc.ConfigurationManager.ActiveConfiguration.Name;

            //switch to physical config for inertia calcs
            if (!currentConfig.Equals(PhysicalModel.swConfiguration))
                modelDoc.ShowConfiguration2(PhysicalModel.swConfiguration);
            //recalculates center of mass and inertia
            CalcInertia(log,false);
            RobotInfo.WriteToLogFile("Calc Inertia Completed");
            //switch to visual config for axis calcs
            if (!modelDoc.ConfigurationManager.ActiveConfiguration.Name
                .Equals(LinkModels[(int)ModelConfiguration.ModelConfigType.Visual].swConfiguration))
                modelDoc.ShowConfiguration2(LinkModels[(int)ModelConfiguration.ModelConfigType.Visual].swConfiguration);
            //checks that model components are defined
            if (LinkModels[(int)ModelConfiguration.ModelConfigType.Visual].IsEmpty())
                log.WriteError("No Visual models for link " + Name);
            if (LinkModels[(int)ModelConfiguration.ModelConfigType.Collision].IsEmpty())
                log.WriteWarning("No unique collison models for link " + Name + ". Using visual models for collisions");

            
            foreach (Joint j in ParentJoints)
            {
                if (!robot.ContinueExport)
                    return false;
                j.Verify(log);
            }
            RobotInfo.WriteToLogFile("All Joints Verified");
            foreach (Attachment att in attachments)
            {
                if (!robot.ContinueExport)
                    return false;
                att.Verify(log);
            }
            RobotInfo.WriteToLogFile("All Attachments Verified");
            
            return true;
        }

        /// <summary>
        /// Calculates the mass and inertia values in the link and stores them
        /// </summary>
        /// <param name="log">The logger to write to. If null then nothing will be written</param>
        /// <param name="changeConfig">default: config changed inside method, false declares config already changed outside</param>
        /// <param name="overrideCustomInertial">default: if custom inertial declared, allows exception</param>
        public void CalcInertia(ProgressLogger log, bool changeConfig = true, bool overrideCustomInertial = false)
        {
            if (UseCustomInertial && !overrideCustomInertial)
                return;
            string matDB = "";
            string config = modelDoc.ConfigurationManager.ActiveConfiguration.Name;
            bool compsMissingMat = false;
            List<Body2> bodies = new List<Body2>();
            object obj;
            ModelConfiguration physConfig = PhysicalModel;
            if (changeConfig)
                modelDoc.ShowConfiguration(physConfig.swConfiguration);
            foreach (modelComponent M in physConfig.LinkComponents)
            {
                if (M.Component == null)
                    continue; 

                Component2 c = M.Component;
                if (c == null)
                    continue;
                //modelDoc.ShowConfiguration2(M.ConfigName);
                
                
                if (c.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed)
                {

                    //if (((object[])c.GetChildren()).Length > 0)
                    if (c.IGetChildrenCount() > 0)
                    {
                        GetSubBodies(bodies, c);
                    }
                    else
                    {
                        if (log != null)
                        {
                            string s = ((IPartDoc)c.GetModelDoc2()).GetMaterialPropertyName2(config, out matDB);
                            if (String.IsNullOrEmpty(s))
                            {
                                compsMissingMat = true;
                            }
                        }
                        if (c.GetBodies3((int)swBodyType_e.swSolidBody, out obj) != null)
                        {
                            foreach (Body2 b in c.GetBodies3((int)swBodyType_e.swSolidBody, out obj))
                            {

                                bodies.Add(b);
                            }
                        }
                    }
                }
            }
            

            if (compsMissingMat && log!=null)
                log.WriteWarning("Some components in link " + Name + " have no material assigned");
            if (bodies.Count > 0)
            {
                MassProperty massProps = ((ModelDocExtension)modelDoc.Extension).CreateMassProperty();
                massProps.AddBodies(bodies.ToArray());
                ComX = massProps.CenterOfMass[0];
                ComY = massProps.CenterOfMass[1];
                ComZ = massProps.CenterOfMass[2];
                Mass = massProps.Mass;
                double[] moments = massProps.GetMomentOfInertia((int)swMassPropertyMoment_e.swMassPropertyMomentAboutCenterOfMass);
                MomentIxx = moments[0];
                MomentIxy = moments[1];
                MomentIxz = moments[2];
                MomentIyy = moments[4];
                MomentIyz = moments[5];
                MomentIzz = moments[8];
            }
            else
            {
                if (log != null)
                    log.WriteError("No solid bodies in link " + Name);
                Mass = 0;
                ComX = 0;
                ComY = 0;
                ComZ = 0;
                MomentIxx = 0;
                MomentIxy = 0;
                MomentIxz = 0;
                MomentIyy = 0;
                MomentIyz = 0;
                MomentIzz = 0;
            }
            OriginX = ComX;
            OriginY = ComY;
            OriginZ = ComZ;
            OriginR = robot.OriginR;
            OriginP = robot.OriginP;
            OriginW = robot.OriginW;
            if(changeConfig)
                modelDoc.ShowConfiguration2(config);
        }

        /// <summary>
        /// Gets the transformed origin point of the link
        /// </summary>
        /// <returns>The transformed origin point for this link</returns>
        public double[] GetTransformedCoordinate()
        {
            Matrix3D transMat = robot.GetTransformMatrix();
            Point3D point = new Point3D(OriginX, OriginY, OriginZ);
            point = Point3D.Multiply(point, transMat);
            double[] arr = { point.X, point.Y, point.Z };
            return arr;
        }

        /// <summary>
        /// Adds all sub bodies in component comp to bodies
        /// </summary>
        /// <param name="bodies">List to add bodies to</param>
        /// <param name="comp">Component2 that has subcomponments</param>
        private void GetSubBodies(List<Body2> bodies, Component2 comp)
        {
            object[] childrenComps = (object[])comp.GetChildren(); 
            if (childrenComps.Length>0)
            {
                object obj;
                foreach (Component2 c in childrenComps)
                {
                    if (((object[])c.GetChildren()).Length>0)
                    {
                        GetSubBodies(bodies, c);
                    }
                    else
                    {
                        var tempBodies = c.GetBodies3((int)swBodyType_e.swSolidBody, out obj);
                        if (tempBodies != null) // handles for surface bodies
                        {
                            foreach (Object b in tempBodies)
                            {
                                bodies.Add((Body2)b);
                            }
                        }                        
                    }
                }
            }
        }

        /// <summary>
        /// Estimates the number of operations needed to export this link and all subLinks
        /// </summary>
        /// <returns> Number of operations to export this Link and all subLinks </returns>
        public int EstimateExportOps()
        {
            int result = 4 + ParentJoints.Count + (LinkModels[(int)ModelConfiguration.ModelConfigType.Collision].EmptyModel?0:2);//2 for collision stl, 2 for visual, 2 for sdf, one per joint
            result += attachments.Count;
            return result;

        }

        #endregion



        #region ISelectable implementation
        private static LinkProperties propertyEditorControl;

        private static void SetupPropertyEditor()
        {
            propertyEditorControl = new LinkProperties();
        }

        public bool Selected { get; set; }

        /// <summary>
        /// Colors the link components
        /// </summary>
        public void ColorLink(int currentModelConfig)
        {

            double[] materials;

            foreach (modelComponent m in LinkModels[currentModelConfig].LinkComponents)
            {
                if (m.ConfigName.Equals(modelDoc.ConfigurationManager.ActiveConfiguration.Name))
                {
                    Component2 c = m.Component;
                    if (c == null)
                        continue;
                    if (c.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed)
                    {
                        if (((object[])c.GetChildren()).Length == 0)
                        {

                            materials = ((ModelDoc2)c.GetModelDoc2()).MaterialPropertyValues;
                            materials[0] = ColorRed / 255.0;
                            materials[1] = ColorGreen / 255.0;
                            materials[2] = ColorBlue / 255.0;
                            System.Diagnostics.Debug.WriteLine(c.Name + " R:" + materials[0] + " G:" + materials[1] + " B:" + materials[2] + " " + this.Name);
                            c.MaterialPropertyValues = materials;
                            System.Diagnostics.Debug.WriteLine(" R:" + ((double[])c.MaterialPropertyValues)[0] + " G:" + ((double[])c.MaterialPropertyValues)[1] + " B:" + ((double[])c.MaterialPropertyValues)[2]);
                        }
                        else
                        {
                            List<Component2> comps = new List<Component2>();
                            GetSubComponents(c, comps);
                            foreach (Component2 c2 in comps)
                            {
                                if (c2.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed)
                                {

                                    materials = ((ModelDoc2)c2.GetModelDoc2()).MaterialPropertyValues;
                                    materials[0] = ColorRed / 255;
                                    materials[1] = ColorGreen / 255;
                                    materials[2] = ColorBlue / 255;
                                    System.Diagnostics.Debug.WriteLine(c2.Name + "sub " + c.Name + " R:" + materials[0] + " G:" + materials[1] + " B:" + materials[2] + " " + this.Name);
                                    c2.MaterialPropertyValues = materials;
                                    System.Diagnostics.Debug.WriteLine(" R:" + ((double[])c2.MaterialPropertyValues)[0] + " G:" + ((double[])c2.MaterialPropertyValues)[1] + " B:" + ((double[])c2.MaterialPropertyValues)[2]);
                                }
                            }
                        }
                    }
                }
            }
            
        }

        public string GetName()
        {
            return "Link"; 
        }

        public Control GetEditorControl()
        {
            if (propertyEditorControl == null)
                SetupPropertyEditor();
            propertyEditorControl.setLink(this);
            return propertyEditorControl;
        }

        #endregion

        /// <summary>
        /// Gets all subComponents of the specified component
        /// </summary>
        /// <param name="comp">Component to find children of</param>
        /// <param name="comps">List to add subcomponents to</param>
        private static void GetSubComponents(Component2 comp, List<Component2> comps)
        {
            object[] childrenComps = (object[])comp.GetChildren();
            if (childrenComps.Length > 0)
            {
                foreach (Component2 c in childrenComps)
                {
                    if (((object[])c.GetChildren()).Length > 0)
                    {
                        GetSubComponents(c, comps);
                    }
                    else
                    {
                        comps.Add(c);
                    }
                }
            }
        }
    }
}
