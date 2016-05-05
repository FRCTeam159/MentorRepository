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
using System.Diagnostics;
using System.Windows.Forms;

namespace GazeboExporter.Export
{
    /// <summary>
    /// This class is used to export the robot model into the SDF format
    /// </summary>
    public class SDFExporter : RobotExporter
    {
        //estimated export operations for differnt parts
        private const int linkExportOps = 4;
        private const int jointExportOps = 4;
        private const int sensorExportOps = 4;
        private const int actuatorExportOps = 4;

        ProgressLogger log;
        string folderPath;
        string tempFilePath;
        string MeshesPath;
        string RobotPath;
        string robotName;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="robot">Robot model to be exported</param>
        /// <param name="iSwApp">Solidworks app</param>
        /// <param name="asm">Assembly that the robot model is in</param>
        public SDFExporter(RobotModel robot, SldWorks iSwApp, AssemblyDoc asm) : base(robot, iSwApp, asm)
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
            robotName = robot.Name.Replace(" ", "_");
            
            try
            {
                if (path.Equals(System.Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\wpilib\\simulation"))
                {
                    folderPath = path + "\\models\\" + robotName;
                    tempFilePath = folderPath + "\\";
                    MeshesPath = folderPath + "\\meshes\\";
                    RobotPath = folderPath + "\\robots\\";
                    if (Directory.Exists(folderPath))
                    {
                        ClearReadOnly(new DirectoryInfo(path));
                        Directory.Delete(folderPath, true);
                    }
                        
                    Directory.CreateDirectory(folderPath);
                    Directory.CreateDirectory(MeshesPath);
                    Directory.CreateDirectory(RobotPath);
                    folderPath = path + "\\worlds";
                    if(File.Exists(folderPath+"\\"+robotName+".world"))
                        File.Delete(folderPath+"\\"+robotName+".world");
                }
                else
                {
                    path += "\\" + robotName + "_Exported";
                    folderPath = path + "\\" + robotName;
                    tempFilePath = folderPath + "\\";
                    MeshesPath = folderPath + "\\meshes\\";
                    RobotPath = folderPath + "\\robots\\";
                    if (Directory.Exists(path))
                    {
                        ClearReadOnly(new DirectoryInfo(path));
                        Directory.Delete(path, true);
                    }
                    Directory.CreateDirectory(path);
                    Directory.CreateDirectory(folderPath);
                    Directory.CreateDirectory(MeshesPath);
                    Directory.CreateDirectory(RobotPath);
                    folderPath = path;
                }
                
            }
            catch (IOException e)
            {
                log.WriteMessage("Failed to create directory. Another application may be accesing the file");
                return;
            }
            

            
            try
            {
                log.WriteMessage("Setting up exporter", false);

                //Export STLs
                log.WriteMessage("Starting STL exports", false);
                ModelDoc2 ActiveDoc = (ModelDoc2)asm;
                
                //switches to the visual configuration and hides all components
                Configuration currentConfig = ActiveDoc.ConfigurationManager.ActiveConfiguration;
                //ActiveDoc.ShowConfiguration2(robot.VisualConfig);
                STLExporter meshMaker = new STLExporter(iSwApp, asm);
                Dictionary<string, List<ModelConfiguration>> VisualModels = new Dictionary<string, List<ModelConfiguration>>();
                Dictionary<string, List<ModelConfiguration>> CollisionModels = new Dictionary<string, List<ModelConfiguration>>();
                //Stopwatch watch = new Stopwatch();
                //watch.Start();
                //sort configurations
                foreach (Link l in robot.GetLinksAsArray())
                {
                    if(!l.VisualModel.EmptyModel)
                        if (VisualModels.ContainsKey(l.VisualModel.swConfiguration))
                            VisualModels[l.VisualModel.swConfiguration].Add(l.VisualModel);
                        else
                        {
                            List<ModelConfiguration> tempList = new List<ModelConfiguration>();
                            tempList.Add(l.VisualModel);
                            VisualModels.Add(l.VisualModel.swConfiguration, tempList);
                        }

                    if(!l.CollisionModel.EmptyModel)
                        if (CollisionModels.ContainsKey(l.CollisionModel.swConfiguration))
                            CollisionModels[l.CollisionModel.swConfiguration].Add(l.CollisionModel);
                        else
                        {
                            List<ModelConfiguration> tempList = new List<ModelConfiguration>();
                            tempList.Add(l.CollisionModel);
                            CollisionModels.Add(l.CollisionModel.swConfiguration, tempList);
                        }

                }
                //Debug.WriteLine("Finished sorting configs=" + watch.ElapsedMilliseconds);
                string[] configs = VisualModels.Keys.Union(CollisionModels.Keys).ToArray();

                //Export each config
                foreach (string config in configs)
                {
                    ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = false;
                    ActiveDoc.ShowConfiguration2(config);
                    //Debug.WriteLine("switched config " + config +"=" + watch.ElapsedMilliseconds);
                    ActiveDoc.ClearSelection2(true);
                    Component2[] hiddenComps = meshMaker.HideAllComponents();
                    //Debug.WriteLine("hid components=" + watch.ElapsedMilliseconds);
                    if(VisualModels.ContainsKey(config))
                        foreach (ModelConfiguration m in VisualModels[config])
                        {
                            Application.DoEvents();
                            if (!robot.ContinueExport)
                            {
                                meshMaker.UnhideComponents(hiddenComps);
                                meshMaker.close();
                                ActiveDoc.ShowConfiguration2(currentConfig.Name);
                                ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = true;
                                return;
                            }
                            ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = false;
                            meshMaker.ExportLink(m.Owner, m, MeshesPath + m.Owner.Name.Replace(" ", "_") + ".stl", log);
                            //Debug.WriteLine("exported visual=" + watch.ElapsedMilliseconds);
                        }
                    if(CollisionModels.ContainsKey(config))
                        foreach (ModelConfiguration m in CollisionModels[config])
                        {
                            Application.DoEvents();
                            if (!robot.ContinueExport)
                            {
                                meshMaker.UnhideComponents(hiddenComps);
                                meshMaker.close();
                                ActiveDoc.ShowConfiguration2(currentConfig.Name);
                                ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = true;
                                return;
                            }
                            ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = false;
                            meshMaker.ExportLink(m.Owner, m, MeshesPath + m.Owner.Name.Replace(" ", "_") + "_col.stl", log);
                           // Debug.WriteLine("exported col=" + watch.ElapsedMilliseconds);
                        }
                    meshMaker.UnhideComponents(hiddenComps);
                    //Debug.WriteLine("unhid comps=" + watch.ElapsedMilliseconds);
                }
                
                ActiveDoc.ShowConfiguration(currentConfig.Name);
                meshMaker.close();
                ((ModelView)ActiveDoc.ActiveView).EnableGraphicsUpdate = true;
                log.WriteMessage("Finished exporting STL files", false);

                //Make SDF
                log.WriteMessage("Creating SDF file", false);
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Encoding = new UTF8Encoding(false);
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                XmlWriter SDFwriter = XmlWriter.Create(RobotPath+robotName+".sdf", settings);
                SDFwriter.WriteStartDocument();
                WriteSDF(SDFwriter);
                SDFwriter.WriteEndDocument();
                SDFwriter.Close();
                log.WriteMessage("SDF file complete");
                CreateConfigFile();
                log.WriteMessage("Config file complete");
                log.WriteMessage("Export Complete");
            }
            catch (Exception e)
            {
                log.WriteError("Unhandled Exception thrown when exporting robot: "+e.Message);
            }
        }

        /// <summary>
        /// Clears any readonly attributes from subfiles
        /// </summary>
        /// <param name="parentDirectory"></param>
        private void ClearReadOnly(DirectoryInfo parentDirectory)
        {
            if (parentDirectory != null)
            {
                parentDirectory.Attributes = FileAttributes.Normal;
                foreach (FileInfo fi in parentDirectory.GetFiles())
                {
                    fi.Attributes = FileAttributes.Normal;
                    
                }
                foreach (DirectoryInfo di in parentDirectory.GetDirectories())
                {
                    ClearReadOnly(di);
                }
            }
        }

        public override int EstimateOperations()
        {
            return 500;
        }

        #region SDF Writing Methods

        /// <summary>
        /// Creates the .SDF file for the robot
        /// </summary>
        /// <param name="writer"> The Xml writer that is being used to create the .SDF file </param>
        private void WriteSDF(XmlWriter writer)
        {
            log.WriteMessage("Starting to write SDF data");

            writer.WriteStartElement("sdf");

            double versionSDF = 1.5; //newest is 1.5
            writer.WriteAttributeString("version", versionSDF.ToString());

            writer.WriteStartElement("model");
            {

                writer.WriteAttributeString("name", robot.Name);
                writeSDFElement(writer, "pose", "0 0 " + robot.GetBasePlaneOffset() + " 0 0 0");
                if (PluginSettings.UseFRCsim)
                {
                    writer.WriteStartElement("plugin");
                    {
                        writer.WriteAttributeString("name", "clock");
                        writer.WriteAttributeString("filename", "libclock.so");

                        writer.WriteStartElement("topic");
                        writer.WriteString("/gazebo/frc/time");
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
               

                

                //write each link and joint
                foreach (Link L in robot.GetLinksAsArray())
                {
                    WriteLink(L, writer);
                    foreach (Joint J in L.ParentJoints)
                    {
                        WriteJoint(J, writer);
                    }
                }
                //write each attachment
                if(PluginSettings.UseFRCsim)
                    foreach (Link L in robot.GetLinksAsArray())
                    {
                        foreach (Attachment att in L.GetAttachmentsAsArray())
                        {
                            WriteAttachmentSDF(att, L, writer);
                        }
                    }


            }
            List<string> otherModels = new List<string>();
            if(PluginSettings.UseFRCsim)
                switch (robot.FRCfield)
                {
                    case 2014:
                        otherModels.Add("model://frc_2014_field");
                        break;
                    case 2015:
                        otherModels.Add("model://frc_2015_field");
                        break;
                }
            WorldExporter.WriteWorldFile(robotName, folderPath +"\\",otherModels.ToArray());
            writer.WriteEndElement();
            log.WriteMessage("Finished writing SDF data");
        }

        /// <summary>
        /// Writes a Link to the .SDF file
        /// </summary>
        /// <param name="link"> The Link to be written </param>
        /// <param name="writer"> The writer to use to write the Link </param>
        private void WriteLink(Link link, XmlWriter writer)
        {
            //Link data
            String linkName = link.Name.Replace(" ", "_");

            writer.WriteStartElement("link");
            writer.WriteAttributeString("name", linkName);
            log.WriteMessage("Writing link " + linkName + " to SDF.");
            {
                //gravity, self_collide, kinematic, 
                writeSDFElement(writer, "gravity", "1"); // 1 true
                writeSDFElement(writer, "self_collide", link.SelfCollide ? "1" : "0"); // 0 false

                double[] transPoint = link.GetTransformedCoordinate();
                string linkpose =transPoint[0] + " " + transPoint[1] + " " + transPoint[2] + " " + link.OriginR + " " + link.OriginP + " " + link.OriginW;
                writeSDFElement(writer, "pose", linkpose);

                //if baselink true
                
                if (link.isBaseLink)
                    writeSDFElement(writer, "must_be_base_link", "1");
                else
                    writeSDFElement(writer, "must_be_base_link", "0");

                

                writer.WriteStartElement("velocity_decay");
                writeSDFElement(writer, "linear", link.LinearDamping.ToString());
                writeSDFElement(writer, "angular", link.AngularDamping.ToString());
                writer.WriteEndElement();


                //Inertial data
                writer.WriteStartElement("inertial");
                {
                    //mass data
                    writeSDFElement(writer, "mass", link.Mass.ToString());

                    //pose data
                    string link_xyzrpy = (link.ComX-link.OriginX) + " " + (link.ComY-link.OriginY) + " " + (link.ComZ-link.OriginZ) + " 0 0 0";
/*                    link_xyzrpy = (link.ParentConnection != null) ?
                        ((link.ComX - link.ParentConnection.OriginX) + " " + (link.ComY - link.ParentConnection.OriginY) + " " + (link.ComZ - link.ParentConnection.OriginZ)) :
                        (link.ComX + " " + link.ComY + " " + link.ComZ);
                    link_xyzrpy += link.OriginR + " " + link.OriginP + " " + link.OriginW;
*/                   writeSDFElement(writer, "pose", link_xyzrpy);

                    //Moment data
                    writer.WriteStartElement("inertia");
                    writeSDFElement(writer, "ixx", link.MomentIxx.ToString());
                    writeSDFElement(writer, "ixy", link.MomentIxy.ToString());
                    writeSDFElement(writer, "ixz", link.MomentIxz.ToString());
                    writeSDFElement(writer, "iyy", link.MomentIyy.ToString());
                    writeSDFElement(writer, "iyz", link.MomentIyz.ToString());
                    writeSDFElement(writer, "izz", link.MomentIzz.ToString());
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();

                //Collision Data
                if (!link.CollisionModel.EmptyModel)
                {
                    writer.WriteStartElement("collision");
                    string collision = linkName + "_collision"; //////////multiple visuals????
                    writer.WriteAttributeString("name", collision);
                    {
                        //origin (pose)
                        string collisionpose = -link.OriginX + " " + -link.OriginY + " " + -link.OriginZ + " 0 0 0";
                        /*                    collisionpose = (link.ParentConnection != null) ?
                                                (-link.ParentConnection.OriginX + " " + -link.ParentConnection.OriginY + " " + -link.ParentConnection.OriginZ) :
                                                (link.OriginXcollision + " " + link.OriginYcollision + " " + link.OriginZcollision);
                                            collisionpose += link.OriginRcollision + " " + link.OriginPcollision + " " + link.OriginWcollision;
                        */
                        writeSDFElement(writer, "pose", collisionpose);

                        //mesh (collision geometry)
                        writer.WriteStartElement("geometry");
                        writer.WriteStartElement("mesh");
                        writeSDFElement(writer, "scale", "1 1 1");
                        string visualuri = "model://" + robotName + "/meshes/" + linkName + "_col.STL";
                        writeSDFElement(writer, "uri", visualuri);
                        writer.WriteEndElement();
                        writer.WriteEndElement();

                        //surface(s)
                        writer.WriteStartElement("surface");
                        {
                            writer.WriteStartElement("contact");
                            writer.WriteStartElement("ode");
                            if (link.Kp > 0)
                                writeSDFElement(writer, "kp", link.Kp.ToString());
                            if (link.Kd > 0)
                                writeSDFElement(writer, "kd", link.Kd.ToString());
                            //writeSDFElement(writer, "max_vel", );
                            //writeSDFElement(writer, "max_depth", link.Kp.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndElement();

                            writer.WriteStartElement("friction");
                            writer.WriteStartElement("ode");
                            writeSDFElement(writer, "mu", link.Mu1.ToString());
                            writeSDFElement(writer, "mu2", link.Mu2.ToString());
                            //writeSDFElement(writer, "max_vel", );
                            //writeSDFElement(writer, "max_depth", link.Kp.ToString());
                            writer.WriteEndElement();
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }
                


                //Visual Data
                
                writer.WriteStartElement("visual");
                string visualname = linkName + "_visual"; //////////multiple visuals????
                writer.WriteAttributeString("name", visualname);
                {
                    //Visual pose
                    string visualpose = -link.OriginX + " " + -link.OriginY + " " + -link.OriginZ + " 0 0 0"; ;
                    /*                    visualpose = ( (link.ParentConnection != null) ?  //check for base link as the baselink will not have an offset) 
                                            (-link.ParentConnection.OriginX + " " + -link.ParentConnection.OriginY + " " + -link.ParentConnection.OriginZ) :
                                            (link.OriginXvisual + " " + link.OriginYvisual + " " + link.OriginZvisual));
                                        visualpose += link.OriginRvisual + " " + link.OriginPvisual + " " + link.OriginWvisual;
                    */
                    writeSDFElement(writer, "pose", visualpose);

                    //Color (material)
                    writer.WriteStartElement("material");
                    //writer.WriteStartElement("script");
                    //writeSDFElement(writer, "name", "Gazebo/" + link.color.ToString("X6"));
                    //writeSDFElement(writer, "uri", "__default__");
                    //writer.WriteEndElement();
                    writeSDFElement(writer, "ambient", link.ColorRed / 255.0 + " " + link.ColorBlue / 255.0 + " " + link.ColorGreen / 255.0 + " 1");
                    writeSDFElement(writer, "diffuse", link.ColorRed / 255.0 + " " + link.ColorBlue / 255.0 + " " + link.ColorGreen / 255.0 + " 1");
                    writer.WriteEndElement();

                    //Mesh (visual)
                    writer.WriteStartElement("geometry");

                    writer.WriteStartElement("mesh");
                    writeSDFElement(writer, "scale", "1 1 1");
                    string visualuri = "model://" + robotName + "/meshes/" + linkName + ".STL";
                    writeSDFElement(writer, "uri", visualuri);
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                
                
                foreach (Attachment att in link.GetAttachmentsAsArray())
                {
                    att.WriteSensor(log, writer);
                }

            }
            writer.WriteEndElement();


            log.WriteMessage("Finished writing link " + link.Name + " to SDF.");
        }

        /// <summary>
        /// Writes a joint to the .SDF file
        /// </summary>
        /// <param name="joint"> The joint to be written </param>
        /// <param name="writer"></param>
        private void WriteJoint(Joint joint, XmlWriter writer)
        {
            joint.WriteSDF(log, writer);
        }
   
        /// <summary>
        /// Writes an attachment of a Link to the .SDF file
        /// </summary>
        /// <param name="att"> Attachment to be written </param>
        /// <param name="parentLink"> The Link that contains this attachment </param>
        /// <param name="writer"> The writer that will be used to write the URDF</param>
        private void WriteAttachmentSDF(Attachment att, Link parentLink, XmlWriter writer)
        {
            att.WritePlugins(log, writer);
            log.WriteMessage("Finished writing attachment " + att.Name + " to SDF.");
        }
        
        /// <summary>
        /// Writes an attachment of a Joint to the .SDF file
        /// </summary>
        /// <param name="att"> Attachment to be written </param>
        /// <param name="parentJoint"> The joint that contains this attachment </param>
        /// <param name="writer"> The writer that will be used to write the URDF</param>
        private void WriteAttachmentSDF(Attachment att, Joint parentJoint, XmlWriter writer)
        {
            att.WriteElements(log, writer);
            log.WriteMessage("Finished writing attachment " + att.Name + " to SDF.");
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

            writeSDFElement(configWriter, "name", robotName);
            writeSDFElement(configWriter, "version", "1.0");
            writeSDFElement(configWriter, "sdf", "robots/" + robotName + ".sdf");
            writeSDFElement(configWriter, "description", "a robot");

            configWriter.WriteEndElement();

            configWriter.WriteEndDocument();
            configWriter.Close();
        }

        #endregion

        /// <summary>
        /// helper function writes a sdf line with a start tag, value, and end tag 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="element"></param>
        /// <param name="value"></param>
        public static void writeSDFElement(XmlWriter writer, string element, string value)
        {
            //if (value != "" && value != null)
            {
                writer.WriteStartElement(element);
                writer.WriteString(value);
                writer.WriteEndElement();
            }
        }

    }

}
