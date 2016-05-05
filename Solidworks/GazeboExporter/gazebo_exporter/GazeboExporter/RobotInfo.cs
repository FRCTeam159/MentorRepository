 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using GazeboExporter.Storage;
using GazeboExporter.Robot;
using SolidWorks.Interop.swconst;
using System.IO;

namespace GazeboExporter
{
    /// <summary>
    /// Static class for the various singletons that the plugin needs to use
    /// </summary>
    public static class RobotInfo
    {
        /// <summary>
        /// The Solidworks application
        /// </summary>
        public static SldWorks SwApp { get; private set; }

        /// <summary>
        /// The currentlyactive assembly document
        /// </summary>
        public static AssemblyDoc AssemDoc { get; private set; }

        /// <summary>
        /// The currently active model document
        /// </summary>
        public static ModelDoc2 ModelDoc { get; private set; }

        /// <summary>
        /// The storage model fro the  robot
        /// </summary>
        public static StorageModel SwData { get; private set; }

        /// <summary>
        /// The robot object
        /// </summary>
        public static RobotModel Robot { get; private set; }

        /// <summary>
        /// The math utility to do math operations with
        /// </summary>
        public static MathUtility mathUtil { get; private set; }

        /// <summary>
        /// The component that should be used to display a body
        /// </summary>
        private static Component2 compForDisplay;

        /// <summary>
        /// Gets the component to use to display a preview body
        /// </summary>
        public static Component2 DispComp
        {
            get
            {
                if (compForDisplay == null || compForDisplay.IsSuppressed() || compForDisplay.GetSuppression() == -1)
                {
                    compForDisplay = getComponentForDisplay(null);
                    System.Diagnostics.Debug.WriteLine("gotComp");
                }
                return compForDisplay;
            }
        }

        /// <summary>
        /// Sets the various properties of the current robot
        /// </summary>
        /// <param name="swApp">The solidworks application</param>
        /// <param name="asmDoc">The current assembly</param>
        /// <param name="swData">The current storage model</param>
        /// <param name="robot">the current robot</param>
        public static void SetProperties(SldWorks swApp, AssemblyDoc asmDoc, StorageModel swData, RobotModel robot)
        {
            SwApp = swApp;
            ModelDoc = (ModelDoc2)asmDoc;
            AssemDoc = asmDoc;
            SwData = swData;
            Robot = robot;
            mathUtil = SwApp.GetMathUtility();
        }

        /// <summary>
        /// Gets the first component thatwould work as a display reference
        /// </summary>
        /// <param name="comp">component to work from</param>
        /// <returns>A component that would be valid for display</returns>
        private static Component2 getComponentForDisplay(Component2 comp)
        {
            object[] Comps;
            if (comp == null)
                Comps = ModelDoc.ConfigurationManager.ActiveConfiguration.GetRootComponent3(false).GetChildren();
            else
                Comps = comp.GetChildren();
            foreach (Component2 c in Comps)
            {
                if (c.GetChildren().Length <= 0 && c.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed)
                {
                    return c;
                }
            }
            foreach (Component2 c in Comps)
            {
                object[] subComps = c.GetChildren();
                if(c.GetChildren().Length > 0 && c.GetSuppression() != (int)swComponentSuppressionState_e.swComponentSuppressed)
                {
                    Component2 newComp = getComponentForDisplay(c);
                    if (newComp != null)
                        return newComp;
                }
            }
            return null;

        }


        public static void WriteToLogFile(string text)
        {
            if (!PluginSettings.Log)
                return;
            string loc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            loc += "\\GazeboExporter";
            if (!Directory.Exists(loc))
                Directory.CreateDirectory(loc);
            string logLoc = loc + "\\log.txt";
            StreamWriter writer = File.AppendText(logLoc);
            writer.WriteLine(text);
            writer.Close();
        }

        public static void ClearLog()
        {
            if (!PluginSettings.Log)
                return;
            string loc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            loc += "\\GazeboExporter";
            if (!Directory.Exists(loc))
                Directory.CreateDirectory(loc);
            string logLoc = loc + "\\log.txt";
            if (File.Exists(logLoc))
            {
                File.Delete(logLoc);
            }
        }

        internal static void WriteToLogFile(Action<object> dump)
        {
            throw new NotImplementedException();
        }
    }
}
