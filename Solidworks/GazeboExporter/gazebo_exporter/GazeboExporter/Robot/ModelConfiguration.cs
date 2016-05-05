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

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Struct representing a single component of a model configuration
    /// </summary>
    public struct modelComponent
    {

        #region Constructors
        /// <summary>
        /// lodas an exisiting modelcomponent at the given path
        /// </summary>
        /// <param name="path">path to store this component at</param>
        public modelComponent(string path)
        {
            this.swData = RobotInfo.SwData;
            this.path = path;
            comp = null;
            comp = Component;
            //ConfigType = 0; //default to physical model
        }

        /// <summary>
        /// Creates a new modelComponent
        /// </summary>
        /// <param name="path">path to stoer this component at</param>
        /// <param name="configname">Configuration that this compoent is in</param>
        /// <param name="configtype">The type of the configuration this component is in</param>
        /// <param name="component">The component that is being stored</param>
        /// <param name="pid">The persistant ID of the component</param>
        public modelComponent( string path, String configname, int configtype, Component2 component, String pid) 
        {
            this.swData = RobotInfo.SwData;
            this.path = path;
            comp = component;
            ConfigType = configtype;
            ConfigName = configname;
            ComponentPID = pid;
            
        }
        #endregion

        StorageModel swData;
        string path;
        Component2 comp;

        /// <summary>
        /// The name of teh config that this component is stored in
        /// </summary>
        public String ConfigName
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
        /// The type of the config this component is in
        /// physical = 0; visual = 1; collision = 2; in order of complex to simple see ModelConfiguration.ModelConfigType
        /// </summary>
        public int ConfigType 
        {
            get
            {
                return (int)swData.GetDouble(path + "/type");
            }
            set
            {
                swData.SetDouble(path + "/type", value);
            }
        }
        /// <summary>
        /// The componenet that is stored
        /// </summary>
        public Component2 Component
        {
            get
            {
                if (comp == null || comp.GetSuppression() == -1)//second checkis to deal with a component being suppressed and then unsurpressed
                {
                    string s = ConfigName;
                    int t = ConfigType;
                    if (this.ComponentPID == null)
                        throw new NullReferenceException();
                    int Errors;
                    byte[] pid = Convert.FromBase64String(this.ComponentPID);
                    Component2 temp = (Component2)RobotInfo.ModelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
                    comp = temp;
                    return temp;
                }
                else
                    return comp;
                
            }
        }

        /// <summary>
        /// The persistant Id of the component 
        /// </summary>
        public String ComponentPID
        {
            get
            {
                return swData.GetString(path + "/PID");
            }
            set
            {
                swData.SetString(path + "/PID", value);
                
            }
        }

        
    }


    /// <summary>
    /// This class represents a robot model configuration
    /// </summary>
    public class ModelConfiguration
    {
        #region Properties

        //storage model that this link is contained in
        StorageModel swData;
        //Allows access to this link's model document
        public ModelDoc2 modelDoc;
        //path to the storage location of this Link
        String path;

        public readonly Link Owner;

        /// <summary>
        /// Type of model configuration (either physical, visual, or collision)
        /// </summary>
        public int Type { get; set; }
        public enum ModelConfigType { Physical, Visual, Collision }; 

        /// <summary>
        /// List of all the components from any SW configuration that should be included in this model configuration
        /// </summary>
        public List<modelComponent> LinkComponents;

        /// <summary>
        /// array of all the PIDs for the components of this model configuration of the Link
        /// </summary>
        public StringStorageArray LinkComponentPIDs;


        private int nextComponentNumber;

        /// <summary>
        /// The solidworks configuration this model configuration is in
        /// </summary>
        public string swConfiguration
        {
            get
            {
                if (LinkComponents.Count > 0)
                    return LinkComponents[0].ConfigName;
                else
                    return null;
            }
        }

        /// <summary>
        /// Boolean representing if the configuration should be an empty configuration
        /// </summary>
        public bool EmptyModel
        {
            get
            {
                return swData.GetDouble(path + "/emptyConfig") == 1;
            }
            set
            {
                swData.SetDouble(path + "/emptyConfig", value ? 1 : 0);

            }
        }
        #endregion 

        /// <summary>
        /// Loads existing modelconfiguration from the swdata, 
        /// or creats new one if not already existing
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        public ModelConfiguration(String path, int type, Link owner)
        {
            //setup fields
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;
            this.Type = type;
            this.Owner = owner;
            LinkComponents = new List<modelComponent>();

            //If the link components data exists, load the components data 
            LinkComponentPIDs = new StringStorageArray(swData, path + "/components");
            nextComponentNumber = 0;

            modelComponent newComp;
            if (LinkComponentPIDs.Count != 0)
            {
                foreach (String s in LinkComponentPIDs)
                {
                    newComp = new modelComponent(path + "/component" + nextComponentNumber);
                    LinkComponents.Add(newComp);
                    nextComponentNumber++;
                }
            }

        }

        /// <summary>
        /// Resets the configuration to a blank configuration
        /// </summary>
        public void Reset()
        {
            LinkComponents.Clear();
            swData.DeleteAll(LinkComponentPIDs.getPath());

            nextComponentNumber = 0;
        }

        #region public methods
        /// <summary>
        /// given a pid, add/removes the component from LinkComponents and its pid from LinkComponentPIDs, as necessary
        /// </summary>
        /// <param name="newPID">pid of component to add to this model configuration</param>
        public void insertModelComp(String newPID)
        {
            //check that component not already in the list
            for (int i = 0; i < LinkComponents.Count; i++)
            {
                if (newPID == LinkComponents[i].ComponentPID)
                    return;
            }

            //make new component 
            String newPath = path + "/component" + nextComponentNumber;
            String newconfigname = modelDoc.ConfigurationManager.ActiveConfiguration.Name;
            int Errors;
            byte[] pid;
            pid = Convert.FromBase64String(newPID);
            Component2 newcomponent = (Component2)modelDoc.Extension.GetObjectByPersistReference3(pid, out Errors);
            modelComponent newComponent = new modelComponent(newPath, newconfigname, Type, newcomponent, newPID);
            nextComponentNumber++;
        }


        /// <summary>
        /// adds the given component to the list of link components if it not in the list already
        /// and adds the PID to LinkComponentPIDs
        /// </summary>
        /// <param name="component">component to add to this model configuration</param>
        public void insertModelComp(Component2 component)
        {
            if (component == null)
                return; 

            //check that component not already in the list
            for (int i = 0; i < LinkComponents.Count; i++)
            {
                if (component == LinkComponents[i].Component)
                    return;
            }

            //make new component 
            String newPath = path + "/component" + nextComponentNumber;
            String newconfigname = modelDoc.ConfigurationManager.ActiveConfiguration.Name;
            byte[] pid = modelDoc.Extension.GetPersistReference3(component); 
            String newpid = Convert.ToBase64String(pid);
            modelComponent newComponent = new modelComponent(newPath,newconfigname,Type,component,newpid);
            nextComponentNumber++;

            //insert in order: Collision, Visual, Physical
            //assume that LinkComponents already ordered as so
            for (int i = 0; i < LinkComponents.Count; i++)
            {
                if (newComponent.ConfigType < LinkComponents[i].ConfigType)
                {
                    LinkComponents.Insert(i, newComponent);
                    LinkComponentPIDs.AddItem(newpid);
                    break;
                }
            }
            if (!LinkComponents.Contains(newComponent)) // insert newComponent at end
            {
                LinkComponents.Add(newComponent);
                LinkComponentPIDs.AddItem(newpid);
            }
        }

        /// <summary>
        /// removes the component with the given persist id from the list of link components
        /// and removes the pid from LinkComponentPIDs
        /// </summary>
        /// <param name="PID"></param>
        public void removeModelComp(String PID)
        {
            //check that component in list
            for (int i = 0; i < LinkComponents.Count; i++)
            {
                if (PID == LinkComponents[i].ComponentPID)
                {
                    LinkComponentPIDs.RemoveItem(i);  
                    LinkComponents.Remove(LinkComponents[i]);
                    return;
                }
            }
        }

        /// <summary>
        /// Finds if the ModelConfiguration is completly empty but not set to be an empty config
        /// </summary>
        /// <returns>true if there are no stored components and is not an EmptyModel</returns>
        public bool IsEmpty()
        {
            return LinkComponents.Count == 0 && !EmptyModel;
        }
        #endregion
    }
}