using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;
using System.Windows.Forms;

namespace GazeboExporter
{
    /// <summary>
    /// This class contains the settings of the plugin. These settings will be the same for all documents
    /// </summary>
    public static class PluginSettings
    {
        #region properties
        /// <summary>
        /// True if the plugin should use FRCsim components. Default is true
        /// </summary>
        public static bool UseFRCsim {
            get
            {
                XmlNodeList nodes = config.GetElementsByTagName(useFRCsimTag);
                return nodes.Item(0).InnerText.Equals("true") ? true : false;
            }
            set
            {
                XmlNodeList nodes = config.GetElementsByTagName(useFRCsimTag);
                nodes.Item(0).InnerText = value ? "true" : "false";
                config.Save(configLoc);
            }
        }

        /// <summary>
        /// true if the plugin will operate in debug mode. This will disable some saftey checks and show all tags on the side
        /// </summary>
        public static bool DebugMode
        {
            get
            {
                XmlNodeList nodes = config.GetElementsByTagName(debugModeTag);
                return nodes.Item(0).InnerText.Equals("true") ? true : false;
            }
            set
            {
                XmlNodeList nodes = config.GetElementsByTagName(debugModeTag);
                nodes.Item(0).InnerText = value ? "true" : "false";
                config.Save(configLoc);
                if (value)
                    RobotInfo.ClearLog();
            }
        }

        public static bool Log
        {
            get
            {
                XmlNodeList nodes = config.GetElementsByTagName(logTag);
                return nodes.Item(0).InnerText.Equals("true") ? true : false;
            }
            set
            {
                XmlNodeList nodes = config.GetElementsByTagName(logTag);
                nodes.Item(0).InnerText = value ? "true" : "false";
                config.Save(configLoc);
            }
        }
        #endregion

        const string useFRCsimTag = "usefrcsim";
        const string debugModeTag = "debugmode";
        const string logTag = "log";

        static XmlDocument config;
        static string configLoc;

        /// <summary>
        /// Loads the plugin settings from the config file or creates a new config file if none exists
        /// </summary>
        static PluginSettings()
        {
            //string loc = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string loc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            loc += "\\GazeboExporter";
            if (!Directory.Exists(loc))
                Directory.CreateDirectory(loc);
            configLoc = loc + "\\GazeboExporter.config";
            if (!File.Exists(configLoc))
                CreateNewConfig(configLoc);

            config = new XmlDocument();
            config.Load(configLoc);
            VerifyConfig();
            RobotInfo.WriteToLogFile("Settings File Loaded");
        }

        /// <summary>
        /// Creates a new config file. These settings are persistant across all documents
        /// </summary>
        /// <param name="path">The path to where the file should be located</param>
        private static void CreateNewConfig(string path)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            XmlWriter configWriter = XmlWriter.Create(path, settings);
            configWriter.WriteStartDocument();
            configWriter.WriteStartElement("configs");
            {
                configWriter.WriteElementString(useFRCsimTag, "true");
                configWriter.WriteElementString(debugModeTag, "false");
                configWriter.WriteElementString(logTag, "false");
            }
            configWriter.WriteEndElement();
            configWriter.WriteEndDocument();
            configWriter.Close();
            //RobotInfo.WriteToLogFile("Created config file");

        }

        /// <summary>
        /// Verified the config has all needed data
        /// </summary>
        private static void VerifyConfig()
        {
            bool failed = false;
            XmlNodeList nodes = config.GetElementsByTagName(useFRCsimTag);
            if (nodes.Count == 0)
                failed = true;
            nodes = config.GetElementsByTagName(debugModeTag);
            if (nodes.Count == 0)
                failed = true;
            nodes = config.GetElementsByTagName(logTag);
            if (nodes.Count == 0)
                failed = true;
            if (failed)
            {
                File.Delete(configLoc);
                CreateNewConfig(configLoc);
                config = new XmlDocument();
                config.Load(configLoc);
            }

        }

        /// <summary>
        /// set the settings of the tool tips to be used in properties pages
        /// </summary>
        /// <returns></returns>
        public static ToolTip SetToolTips()
        {
            ToolTip tooltip = new ToolTip();

            tooltip.AutoPopDelay = 2000;
            tooltip.InitialDelay = 500;
            tooltip.ReshowDelay = 500;
            tooltip.ShowAlways = true; // Force the ToolTip text to be displayed whether or not the form is active.

            return tooltip;

        }
    }
}
