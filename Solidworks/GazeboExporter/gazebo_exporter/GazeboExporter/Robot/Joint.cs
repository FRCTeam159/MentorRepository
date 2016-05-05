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

namespace GazeboExporter.Robot
{

    /// <summary>
    /// This class represents a robot joint, a connection between two links
    /// that allows them to move relative to eachother with zero, one or multiple
    /// degrees of freedom
    /// </summary>
    public class Joint : ISelectable
    {

        #region Fields and Properties

        //The storage model that this link is saved with
        StorageModel swData;

        /// <summary>
        /// The link that is the parent of the joint
        /// </summary>
        public Link Parent { 
            get 
            {
                return parentLink; 
            } 
            private set 
            {
                parentLink = value;
                parentId = value.Id;
            } 
        }

        private Link parentLink;

        /// <summary>
        /// The Id of the parent link
        /// </summary>
        private int parentId
        {
            get
            {
                return (int)swData.GetDouble(path + "/parentId");
            }
            set
            {
                swData.SetDouble(path + "/parentId", value);
            }
        }

        /// <summary>
        /// the link that is the child of the joint. This is the link that will contain the joint
        /// </summary>
        public Link Child
        {
            get
            {
                return childLink;
            }
            private set
            {
                childLink = value;
                childId = value.Id;
            }
        }

        private Link childLink;

        /// <summary>
        /// The Id of the child link
        /// </summary>
        private int childId
        {
            get
            {
                return (int)swData.GetDouble(path + "/childId");
            }
            set
            {
                swData.SetDouble(path + "/childId", value);
            }
        }

        /// <summary>
        /// Name of this joint in form "parentLink-childLink"
        /// </summary>
        public String Name { 
            get
            {
                return Parent.Name + "-" + Child.Name;
            }
        }

        /// <summary>
        /// Type of this joint
        /// </summary>
        public string Type
        {
            get
            {
                return swData.GetString(path + "/type") ?? "";
            }
            set
            {
                if (JointFactory.GetTypesList().Contains(value))
                {
                    
                    jointSpecifics = JointFactory.GetSpecificJoint(value, path, this);
                    if(!value.Equals(Type))
                        jointSpecifics.ClearValues();
                    swData.SetString(path + "/type", value);
                }
                
            }
        }

        public JointSpecifics jointSpecifics { get; private set; }


        

        
        //public enum JointType { Fixed, Floating, Continuous, Revolute, Prismatic };
        //public static String[] JointTypes = { "fixed", "floating", "continuous", "revolute", "prismatic" };
        public static String[] JointAxis1Names = { null, null, "Axis of Rotation:", "Axis of Rotation:", "Axis of Translation" };
        public static String[] JointAxis2Names = { null, null, null, null, null };



        //Storage path of this joint in the StorageModel
        public String path;
        //Allows access to this joint's assembly document
        AssemblyDoc asmDoc;
        //Allows access to this joint's model document
        ModelDoc2 modelDoc;

        SldWorks swApp;

        RobotModel robot;

        


        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new joint
        /// </summary>
        /// <param name="path">Path to this joint in the StorageModel</param>
        /// <param name="owner">The Link that owns this joint (child Link in the joint)</param>
        public Joint(string path, Link parent, Link child)
        {
            this.swApp = RobotInfo.SwApp;
            this.asmDoc = RobotInfo.AssemDoc;
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;
            this.robot = RobotInfo.Robot;
            this.Parent = parent;
            Parent.ChildJoints.Add(this);
            this.Child = child;
            this.Selected = false;
            this.Type = JointFactory.DefaultJointType;

            RobotInfo.WriteToLogFile("Getting Joint Specifics (Joint)");
            jointSpecifics = JointFactory.GetSpecificJoint(Type,path,this);
            RobotInfo.WriteToLogFile("Successfully created Joint Specifics (Joint)");

            if (swData.GetDouble(path) == 0)
            {
                swData.SetDouble(path, 1);
            }

            
        }
            
        /// <summary>
        /// Loads a joint that has already been created
        /// </summary>
        /// <param name="swApp">Solidworks app</param>
        /// <param name="asm">Assembly document this joint and its robot belong to</param>
        /// <param name="swData">StorageModel that this joint is tored in</param>
        /// <param name="path">Path to this joint in the StorageModel</param>
        /// <param name="robot">The Robot model that the joint is in</param>
        public Joint(string path)
        {
            this.swApp = RobotInfo.SwApp;
            this.asmDoc = RobotInfo.AssemDoc;
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;
            this.robot = RobotInfo.Robot;
            this.Parent = robot.GetLink(parentId);
            Parent.ChildJoints.Add(this);
            this.Child = robot.GetLink(childId);
            this.Selected = false;
            if(this.Type == null || this.Type.Equals(""))
                this.Type = JointFactory.DefaultJointType;
            RobotInfo.WriteToLogFile("Loading existing joint (Joint)");
            jointSpecifics = JointFactory.GetSpecificJoint(Type, path, this);
            if (swData.GetDouble(path) == 0)
            {
                swData.SetDouble(path, 1);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Deletes this joint
        /// </summary>
        public void Delete()
        {
            RobotInfo.WriteToLogFile("Removing attachments (Joint)");
            Link tempLink;            
            if (Child.UpperJoints != null && Child.UpperJoints.Contains(this))
                tempLink = Child;
            else
                tempLink = Parent;
            Attachment[] attArr = tempLink.GetAttachmentsAsArray();
            foreach (Attachment att in attArr)
            {
                if (att is JointedAttachment && ((JointedAttachment)att).Joint.Equals(this))
                    att.Delete();
            }
            RobotInfo.WriteToLogFile("Removing joint from related links (Joint)");
            Parent.RemoveJoint(this);
            Child.RemoveJoint(this);
            swData.DeleteAll(path);
            RobotInfo.WriteToLogFile("Successfully removed joint (Joint)");
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// open Joint tab of JointEditor for given joint when click Joint Pose button from Manage Robot window
        /// call from JointProperties
        /// </summary>
        /// <param name="robot">current robot to edit</param>
        /// <param name="manager"></param>
        public void OpenJointEditorPage(RobotModel robot, ManageRobot manager)
        {
            JointPMPage editor = jointSpecifics.FillPropertyPage();//SetupPage();
            editor.RestoreManager += manager.ExternalSelect;
            editor.Show(); 
           // editor.OnJointSelectionChanged(this);            //calls SaveCurrentLinkSelection() and LoadCurrentLinkSelection();
            //const int jointTabID = 33; editor.OnTabClicked(jointTabID);            //calls ToggleJointPropertyManagers(this.Type);

        }

        #endregion



        #region Export

        /// <summary>
        /// Verifies that the joint is ready for export
        /// </summary>
        /// <returns>Returns true if succesfully verified</returns>
        public bool Verify(ProgressLogger log)
        {
            return jointSpecifics.Verify(log);
        }

        /// <summary>
        /// Writes the SDF file for the joint
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">The XMLwriter to write the sdf tags to</param>
        public void WriteSDF(ProgressLogger log, XmlWriter writer)
        {
            jointSpecifics.WriteJointSDF(log, writer);
            

        }
        #endregion 

        #region ISelectable Implementation and Property Editor Code
        //private static JointProperties2 propertyEditorControl;

        private static void setupPropertyEditor()
        {
            //propertyEditorControl = new JointProperties2();
        }


        public bool Selected { get; set; }

        public string GetName()
        {
            return "Joint";
        }

        public Control GetEditorControl()
        {
            /*if (propertyEditorControl == null)
                setupPropertyEditor();
            propertyEditorControl.setJoint(this);
            return propertyEditorControl;*/
            return jointSpecifics.CreatePropertiesPanel();
        }

        #endregion
    }
}
