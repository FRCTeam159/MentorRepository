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
using GazeboExporter.Robot;
using System.Xml;
using System.IO;
using System.IO.Packaging;

namespace GazeboExporter.Export
{
    /// <summary>
    /// This class is used to export the robot model into the URDF format
    /// </summary>
    public class URDFExporter : RobotExporter
    {
        //estimated export operations for different parts
        private const int linkExportOps = 4;
        private const int jointExportOps = 4;
        private const int sensorExportOps = 4;
        private const int actuatorExportOps = 4;

        ProgressLogger log;
        string folderPath;
        string tempFilePath;
        string visualSTLPath;
        string collisionSTLPath;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="robot">Robot model to be exported</param>
        /// <param name="iSwApp">Solidworks app</param>
        /// <param name="asm">Assembly that the robot model is in</param>
        public URDFExporter(RobotModel robot, SldWorks iSwApp, AssemblyDoc asm) : base(robot, iSwApp, asm)
        {
        }

        /// <summary>
        /// Exports the robot
        /// </summary>
        /// <param name="log"> Logger to be used to write messeges </param>
        /// <param name="path"> File path to export robot to </param>
        public override void Export(ProgressLogger log, String path)
        {
            this.log = log;
            path += "\\" + robot.Name +"_Exported";
            folderPath = path + "\\" + robot.Name;
            tempFilePath = folderPath +  "\\";
            visualSTLPath = folderPath + "\\visualSTL\\";
            collisionSTLPath = folderPath + "\\collisionSTL\\";
            if (Directory.Exists(path))
                Directory.Delete(path, true);
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(folderPath);
            Directory.CreateDirectory(visualSTLPath);
            Directory.CreateDirectory(collisionSTLPath);
            try
            {
                log.WriteMessage("Setting up exporter", false);

                //Export STLs
                log.WriteMessage("Starting STL exports", false);
                ModelDoc2 ActiveDoc = (ModelDoc2)asm;
                //switchs to the visual configuration and hides all components
                Configuration currentConfig = ActiveDoc.ConfigurationManager.ActiveConfiguration;
                ActiveDoc.ShowConfiguration2(robot.VisualConfig);
                STLExporter meshMaker = new STLExporter(iSwApp, asm);
                Component2[] hiddenComps = meshMaker.HideAllComponents();
                //exports each Link's visual models
                foreach (Link L in robot.GetLinksAsArray())
                {
                    if (!robot.ContinueExport)
                    {
                        meshMaker.UnhideComponents(hiddenComps);
                        meshMaker.close();
                        ActiveDoc.ShowConfiguration2(currentConfig.Name);
                        return;
                    }
                    meshMaker.ExportLink(L, robot.VisualConfig, visualSTLPath + L.Name + ".stl", log);
                    
                }
                //unhides all components, switches to collision configuration, then hides all components
                meshMaker.UnhideComponents(hiddenComps);
                ActiveDoc.ShowConfiguration2(robot.CollisionConfig);
                hiddenComps = meshMaker.HideAllComponents();
                //exports each Link's collision model
                foreach (Link L in robot.GetLinksAsArray())
                {
                    if (!robot.ContinueExport)
                    {
                        meshMaker.UnhideComponents(hiddenComps);
                        meshMaker.close();
                        ActiveDoc.ShowConfiguration2(currentConfig.Name);
                        return;
                    }
                    meshMaker.ExportLink(L, robot.CollisionConfig, collisionSTLPath + L.Name + ".stl", log);
                }
                //restores the model to its original configuration
                meshMaker.UnhideComponents(hiddenComps);
                ActiveDoc.ShowConfiguration(currentConfig.Name);
                meshMaker.close();
                log.WriteMessage("Finished exporting STL files", false);

                //Make URDF
                log.WriteMessage("Creating URDF file", false);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                XmlWriter URDFwriter = XmlWriter.Create(tempFilePath+robot.Name+".URDF", settings);
                URDFwriter.WriteStartDocument();
                WriteURDF(URDFwriter);
                URDFwriter.WriteEndDocument();
                URDFwriter.Close();
                log.WriteMessage("URDF file complete");
                CreateConfigFile();
                log.WriteMessage("Config file complete");
                log.WriteMessage("Export Complete");
            }
            catch (Exception e)
            {
                log.WriteError("Unhandled Exception thrown when exporting robot: "+e.Message);
            }
        }

        public override int EstimateOperations()
        {
            return 500;
        }

        #region URDF Writing Methods

        /// <summary>
        /// Creates the .URDF file for the robot
        /// </summary>
        /// <param name="writer"> The Xml writer that is being used to create the .URDF file </param>
        private void WriteURDF(XmlWriter writer)
        {
            writer.WriteStartElement("robot");
            writer.WriteAttributeString("name", robot.Name);
            log.WriteMessage("Starting to write URDF data");
            
            //write each link
            foreach (Link L in robot.GetLinksAsArray())
                WriteLink(L, writer);

            //write each joint
            foreach (Link L in robot.GetLinksAsArray())
            {
                if(L.ParentConnection!=null){
                    WriteJoint(L.ParentConnection, writer);
                }
            }

            writer.WriteStartElement("gazebo");
                writer.WriteStartElement("plugin");
                    writer.WriteAttributeString("name", "clock");
                    writer.WriteAttributeString("filename", "libclock.so");

                    writer.WriteStartElement("topic");
                    writer.WriteString("/gazebo/frc/time");
                    writer.WriteEndElement();
                writer.WriteEndElement();
            writer.WriteEndElement();

            //write each attachment
            
            foreach (Link L in robot.GetLinksAsArray())
            {
                foreach (Attachment att in L.GetAttachmentsAsArray())
                {
                    WriteAttachment(att, L, writer);
                }
            }
            writer.WriteEndElement();
            log.WriteMessage("Finished writing URDF data");
        }

        /// <summary>
        /// Writes a Link to the .URDF file
        /// </summary>
        /// <param name="link"> The Link to be written </param>
        /// <param name="writer"> The writer to use to write the Link </param>
        private void WriteLink(Link link, XmlWriter writer)
        {
            //Link data
            writer.WriteStartElement("link");
            writer.WriteAttributeString("name", link.Name);
            log.WriteMessage("Writing link " + link.Name + " to URDF");

                //Inertial data
                writer.WriteStartElement("inertial");

                    //Origin data
                    writer.WriteStartElement("origin");
                    if (link.ParentConnection != null)
                    {
                        writer.WriteAttributeString("xyz", (link.ComX - link.ParentConnection.OriginX) + " " + (link.ComY - link.ParentConnection.OriginY) + " " + (link.ComZ - link.ParentConnection.OriginZ));
                        writer.WriteAttributeString("rpy", link.OriginR + " " + link.OriginP + " " + link.OriginW);
                    }
                    else
                    {
                        writer.WriteAttributeString("xyz", link.ComX + " " + link.ComY  + " " + link.ComZ );
                        writer.WriteAttributeString("rpy", link.OriginR + " " + link.OriginP + " " + link.OriginW);
                    }
                    writer.WriteEndElement();

                    //Mass data
                    writer.WriteStartElement("mass");
                    writer.WriteAttributeString("value", link.Mass.ToString());
                    writer.WriteEndElement();

                    //Moment data
                    writer.WriteStartElement("inertia");
                    writer.WriteAttributeString("ixx", link.MomentIxx.ToString());
                    writer.WriteAttributeString("ixy", link.MomentIxy.ToString());
                    writer.WriteAttributeString("ixz", link.MomentIxz.ToString());
                    writer.WriteAttributeString("iyy", link.MomentIyy.ToString());
                    writer.WriteAttributeString("iyz", link.MomentIyz.ToString());
                    writer.WriteAttributeString("izz", link.MomentIzz.ToString());
                    writer.WriteEndElement();

                writer.WriteEndElement();

                //Visual Data
                writer.WriteStartElement("visual");

                    //Visual Origin
                    writer.WriteStartElement("origin");
                    if (link.ParentConnection != null)//check for base link as the baselink will not have an offset
                    {
                        writer.WriteAttributeString("xyz", -link.ParentConnection.OriginX + " " + -link.ParentConnection.OriginY + " " + -link.ParentConnection.OriginZ);
                        writer.WriteAttributeString("rpy", link.OriginRvisual + " " + link.OriginPvisual + " " + link.OriginWvisual);
                    }
                    else
                    {
                        writer.WriteAttributeString("xyz", link.OriginXvisual + " " + link.OriginYvisual + " " + link.OriginZvisual);
                        writer.WriteAttributeString("rpy", link.OriginRvisual + " " + link.OriginPvisual + " " + link.OriginWvisual);
                    }
                    writer.WriteEndElement();

                    //Visual Mesh
                    writer.WriteStartElement("geometry");
                    writer.WriteStartElement("mesh");
                    writer.WriteAttributeString("filename", "package://" + robot.Name+"/visualSTL/" + link.Name + ".STL");
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                    //Color
                    writer.WriteStartElement("material");
                    writer.WriteAttributeString("name", link.color.ToString("X6"));
                    writer.WriteStartElement("color");
                    writer.WriteAttributeString("rgba", (link.ColorRed/255.0).ToString() + " " + (link.ColorGreen/255.0).ToString() + " " + (link.ColorBlue/255.0).ToString() + " 1");
                    writer.WriteEndElement();
                    writer.WriteEndElement();

                writer.WriteEndElement();

                //Collision Data
                writer.WriteStartElement("collision");

                    //Visual Mesh
                    writer.WriteStartElement("origin");
                    if (link.ParentConnection != null)
                    {
                        writer.WriteAttributeString("xyz", -link.ParentConnection.OriginX + " " + -link.ParentConnection.OriginY + " " + -link.ParentConnection.OriginZ);
                        writer.WriteAttributeString("rpy", link.OriginRcollision + " " + link.OriginPcollision + " " + link.OriginWcollision);
                    }
                    else
                    {
                        writer.WriteAttributeString("xyz", link.OriginXcollision + " " + link.OriginYcollision + " " + link.OriginZcollision);
                        writer.WriteAttributeString("rpy", link.OriginRcollision + " " + link.OriginPcollision + " " + link.OriginWcollision);
                    }
                    writer.WriteEndElement();

                    //Collision Mesh
                    writer.WriteStartElement("geometry");
                        writer.WriteStartElement("mesh");
                        writer.WriteAttributeString("filename", "package://" + robot.Name + "/collisionSTL/" + link.Name + ".STL");
                        writer.WriteEndElement();
                    writer.WriteEndElement();

                writer.WriteEndElement();

            writer.WriteEndElement();
            //write Gazebo properties of link
            if (link.Mu1 != 0 || link.Mu2 != 0 || link.Kp != 0 || link.Kd != 0)
            {
                writer.WriteStartElement("gazebo");
                writer.WriteAttributeString("reference", link.Name);
                    if(link.Mu1 != 0 || link.Mu2 != 0)
                    {
                        writer.WriteStartElement("mu1");
                        writer.WriteString(link.Mu1.ToString());
                        writer.WriteEndElement();
                        writer.WriteStartElement("mu2");
                        writer.WriteString(link.Mu2.ToString());
                        writer.WriteEndElement();
                    }
                    if (link.Kp != 0)
                    {
                        writer.WriteStartElement("kp");
                        writer.WriteString(link.Kp.ToString());
                        writer.WriteEndElement();
                    }
                    if (link.Kd != 0)
                    {
                        writer.WriteStartElement("kd");
                        writer.WriteString(link.Kd.ToString());
                        writer.WriteEndElement();
                    }
                writer.WriteEndElement();
            }
            /*writer.WriteStartElement("gazebo");
            writer.WriteAttributeString("reference", link.name);
                writer.WriteStartElement("material");
                writer.WriteStartElement("ambient");
                    writer.WriteString((link.colorRed / 255.0).ToString() + " " + (link.colorGreen / 255.0).ToString() + " " + (link.colorBlue / 255.0).ToString() + " 1");
                writer.WriteEndElement();
                writer.WriteEndElement();
            writer.WriteEndElement();
            log.writeMessage("Finished writing link " + link.name + " to URDF");*/
        }

        /// <summary>
        /// Writes a joint to the .URDF file
        /// </summary>
        /// <param name="joint"> The joint to be written </param>
        /// <param name="writer"></param>
        private void WriteJoint(Joint joint, XmlWriter writer)
        {
            //Joint Data
            writer.WriteStartElement("joint");
            writer.WriteAttributeString("name", joint.Name);

            writer.WriteAttributeString("type", Joint.JointTypes[joint.Type]);
            log.WriteMessage("Writing joint " + joint.Name+ " to URDF");

                //Joint origin
                writer.WriteStartElement("origin");
                if (joint.Owner.Parent.ParentConnection != null)
                {
                    writer.WriteAttributeString("xyz", (joint.OriginX - joint.Owner.Parent.ParentConnection.OriginX) + " " + (joint.OriginY - joint.Owner.Parent.ParentConnection.OriginY) + " " + (joint.OriginZ - joint.Owner.Parent.ParentConnection.OriginZ));
                    writer.WriteAttributeString("rpy", joint.OriginR + " " + joint.OriginP + " " + joint.OriginW);
                }
                else
                {
                    writer.WriteAttributeString("xyz", joint.OriginX + " " + joint.OriginY + " " + joint.OriginZ);
                    writer.WriteAttributeString("rpy", joint.OriginR + " " + joint.OriginP + " " + joint.OriginW);
                }
                writer.WriteEndElement();
            
                //Parent Link
                writer.WriteStartElement("parent");
                writer.WriteAttributeString("link", joint.Owner.Parent.Name);
                writer.WriteEndElement();

                //Child Link
                writer.WriteStartElement("child");
                writer.WriteAttributeString("link", joint.Owner.Name);
                writer.WriteEndElement();

                //Axis
                writer.WriteStartElement("axis");    
                writer.WriteAttributeString("xyz", joint.AxisX.ToString() + " " + joint.AxisY.ToString() + " " + joint.AxisZ);
                
                
                writer.WriteEndElement();

                //dynamics
                writer.WriteStartElement("dynamics");
                writer.WriteAttributeString("damping", joint.Damping.ToString());
                writer.WriteAttributeString("friction", joint.Friction.ToString());
                writer.WriteEndElement();

                //Limits if needed
                if (joint.Type == 3 || joint.Type == 4)
                {
                    writer.WriteStartElement("limit");
                    writer.WriteAttributeString("upper", joint.UpperLimit.ToString());
                    writer.WriteAttributeString("lower", joint.LowerLimit.ToString());
                    writer.WriteAttributeString("effort", joint.EffortLimit.ToString());
                    writer.WriteAttributeString("velocity", joint.VelocityLimit.ToString());
                    writer.WriteEndElement();
                }

                //mimic if needed
                if (joint.IsMimic)
                {
                    writer.WriteStartElement("mimic");
                    writer.WriteAttributeString("joint", joint.MimicJoint);
                    writer.WriteAttributeString("multiplier", joint.MimicMultiplier.ToString());
                    writer.WriteAttributeString("offset", joint.MimicOffset.ToString());
                    writer.WriteEndElement();
                }

            writer.WriteEndElement();
            log.WriteMessage("Finished writing joint " + joint.Name + " to URDF");
        }

        /// <summary>
        /// Writes an attachment to the .URDF file
        /// </summary>
        /// <param name="att"> Attachment to be written </param>
        /// <param name="parentLink"> The Link that contains this attachment </param>
        /// <param name="writer"> The writer that will be used to write the URDF</param>
        private void WriteAttachment(Attachment att, Link parentLink, XmlWriter writer)
        {
            att.WriteElements(log, writer);
            log.WriteMessage("Finished writing attachment " + att.Name + " to URDF.");

        }

        /// <summary>
        /// Creates the config file for the robot
        /// </summary>
        private void CreateConfigFile()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;
            settings.NewLineOnAttributes = false;
            XmlWriter configWriter = XmlWriter.Create(tempFilePath + "model.config", settings);
            configWriter.WriteStartDocument();
            configWriter.WriteStartElement("model");

            configWriter.WriteStartElement("name");
            configWriter.WriteString(robot.Name);
            configWriter.WriteEndElement();

            configWriter.WriteStartElement("version");
            configWriter.WriteString("1.0");
            configWriter.WriteEndElement();

            configWriter.WriteStartElement("sdf");
            configWriter.WriteString(robot.Name+".URDF");
            configWriter.WriteEndElement();

            configWriter.WriteStartElement("description");
            configWriter.WriteString("a robot");
            configWriter.WriteEndElement();

            configWriter.WriteEndElement();

            configWriter.WriteEndDocument();
            configWriter.Close();
        }

        #endregion
    }

}
