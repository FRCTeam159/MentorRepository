using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Factory to create new jointspecifics
    /// </summary>
    public static class JointFactory
    {
        /// <summary>
        /// Dictionary of string names of joints and their corresponding types
        /// </summary>
        static Dictionary<string, Type> jointTypes;

        static Dictionary<string, int> jointWeights;

        static int nextIndex = 0;

        /// <summary>
        /// The default Type of a new joint
        /// </summary>
        public static string DefaultJointType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a new jointSpecifics object
        /// </summary>
        /// <param name="type">String name of the joint type</param>
        /// <param name="path">The path to store the new joint at</param>
        /// <param name="joint">The joint that this specifics corresponds to</param>
        /// <returns>The newly created jointSpecifics object</returns>
        public static JointSpecifics GetSpecificJoint(string type, string path, Joint joint)
        {
            if (PluginSettings.DebugMode)
            {
                if (!jointTypes.Keys.Contains(type))
                    type = GetTypesList()[0];
            }
            Type t = jointTypes[type];
            Type[] paramTypes = { typeof(Joint),typeof(string)};
            object[] parameters = { joint, path };
            JointSpecifics tempObj = (JointSpecifics) t.GetConstructor(paramTypes).Invoke(parameters);
            tempObj.UpdateOriginPoint();            
            return tempObj;

        }

        /// <summary>
        /// Registers the joint type in the factory
        /// </summary>
        /// <param name="str"></param>
        /// <param name="t"></param>
        public static void RegisterJointType(string str, Type t, int weight)
        {
            jointTypes.Add(str, t);
            jointWeights.Add(str,weight);
            nextIndex++;
            if (DefaultJointType.Equals("") || weight < jointWeights[DefaultJointType])
                DefaultJointType = str;

        }

        /// <summary>
        /// Gets a list of all the jointspecific types
        /// </summary>
        /// <returns></returns>
        public static string[] GetTypesList()
        {
            var jointList = jointWeights.Keys.OrderBy(x => jointWeights[x]).ToList();
            //var jointList= from entry in jointTypes orderby entry.Value ascending select entry;
            List<string> tempList = new List<string>();
            foreach(var str in jointList)
            {
                tempList.Add(str);
            }
            return tempList.ToArray();
        }

        /// <summary>
        /// Initiallizes the factory
        /// </summary>
        static JointFactory()
        {
            jointTypes = new Dictionary<string, Type>();
            jointWeights = new Dictionary<string, int>();
            DefaultJointType = "";
        }
    }
}
