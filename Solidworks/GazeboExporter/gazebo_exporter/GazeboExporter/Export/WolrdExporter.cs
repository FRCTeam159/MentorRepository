using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GazeboExporter.Export
{
    /// <summary>
    /// A class that is used to create the world file for the robot
    /// </summary>
    public static class WorldExporter
    {
        /// <summary>
        /// Creates a world file for a given robot
        /// </summary>
        /// <param name="name">The name of the robot to be exported</param>
        /// <param name="path">the file path to where the export folder is</param>
        public static void WriteWorldFile(String name, String path, String[] otherModels)
        {
            name = name.Replace(" ", "_");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            System.Diagnostics.Debug.WriteLine(path);
            XmlWriter worldWriter = XmlWriter.Create(path + name + ".world", settings);
            worldWriter.WriteStartDocument();

            worldWriter.WriteStartElement("sdf");
            worldWriter.WriteAttributeString("version", "1.4");

                worldWriter.WriteStartElement("world");
                worldWriter.WriteAttributeString("name", "default");

                    //add global light source 
                    worldWriter.WriteStartElement("include");
                        worldWriter.WriteStartElement("uri");
                        worldWriter.WriteString("model://sun");
                        worldWriter.WriteEndElement();
                    worldWriter.WriteEndElement();

                    //add ground plane
                    worldWriter.WriteStartElement("include");
                        worldWriter.WriteStartElement("uri");
                        worldWriter.WriteString("model://ground_plane");
                        worldWriter.WriteEndElement();
                    worldWriter.WriteEndElement();

                    //add robot model
                    worldWriter.WriteStartElement("include");
                        worldWriter.WriteStartElement("uri");
                        worldWriter.WriteString("model://" + name);
                        worldWriter.WriteEndElement();
                        worldWriter.WriteStartElement("pose");
                        worldWriter.WriteString("0 0 0 0 0 0");
                        worldWriter.WriteEndElement();
                    worldWriter.WriteEndElement();

                    //write other models, ie frc field
                    foreach (string s in otherModels)
                    {
                        worldWriter.WriteStartElement("include");
                        worldWriter.WriteStartElement("uri");
                        worldWriter.WriteString(s);
                        worldWriter.WriteEndElement();
                        worldWriter.WriteStartElement("pose");
                        worldWriter.WriteString("0 0 0 0 0 0");
                        worldWriter.WriteEndElement();
                        worldWriter.WriteEndElement();
                    }

                worldWriter.WriteEndElement();
            worldWriter.WriteEndElement();

            worldWriter.WriteEndDocument();
            worldWriter.Close();
        }
    }
}
