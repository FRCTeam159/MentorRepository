using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Reflection;
using SolidWorks.Interop.sldworks; 
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GazeboExporter.Robot;
using GazeboExporter.UI;
using GazeboExporter.GazeboException;
using GazeboExporter.Export;
using System.Windows.Forms;


namespace GazeboExporter
{
    /// <summary>
    /// Solidworks add-in entry point.
    /// Solidworks will create an instance of this object and call ConnectToSW
    /// when Solidwoks loads this add-in.
    /// Solidworks will call DisconnectFromSW when Solidworks unloads this add-in.
    /// </summary>
    [Guid("f9e7f4e4-9a40-4767-b8cc-34dd7f6d06c5"), ComVisible(true)]
    [SwAddin(
        Description = "An add-in for exporting robot models to the gazebo simulator",
        Title = "GazeboExporter",
        LoadAtStartup = true
        )]
    public class SwAddin : ISwAddin
    {
        //Static properties for use by the entire Add-in
        private Robot.RobotModel currentRobot { get; set; }            //Robot structure for the robot in the current document (if one exists)
        private SldWorks iSwApp { get; set; }            //Interface to Solidworks
        private AssemblyDoc currentDoc { get; set; }                //Currently open document
        public static CommandManager cmdMgr { get; private set; }    //Object to manage Solidworks toolbar and menu commands for this add-in
        public static int addinID { get; private set; }                 //ID of this add-in
        ManageRobot manager;
        SettingsForm settings;
        Exporter exporter;


        #region SolidWorks Registration

        /// <summary>
        /// This funtion is called to register this assembly with COM. It
        /// creates registry keys that allow solidworks to find and load this 
        /// add in.
        /// </summary>
        /// <param name="t">Type variable used to identify this assmbly.</param>
        [ComRegisterFunctionAttribute]
        public static void RegisterFunction(Type t)
        {
            #region Get Custom Attribute: SwAddinAttribute
            //Reads the custon attribute used on this class that specifies the add in name, description
            //and whether it loads on startup.
            SwAddinAttribute SWattr = null;
            Type type = typeof(SwAddin);

            foreach (System.Attribute attr in type.GetCustomAttributes(false))
            {
                if (attr is SwAddinAttribute)
                {
                    SWattr = attr as SwAddinAttribute;
                    break;
                }
            }

            #endregion

            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                Microsoft.Win32.RegistryKey addinkey = hklm.CreateSubKey(keyname);
                addinkey.SetValue(null, 0);

                addinkey.SetValue("Description", SWattr.Description);
                addinkey.SetValue("Title", SWattr.Title);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                addinkey = hkcu.CreateSubKey(keyname);
                addinkey.SetValue(null, Convert.ToInt32(SWattr.LoadAtStartup), Microsoft.Win32.RegistryValueKind.DWord);
            }
            catch (System.NullReferenceException nl)
            {
                Console.WriteLine("There was a problem registering this dll: SWattr is null. \n\"" + nl.Message + "\"");
                System.Windows.Forms.MessageBox.Show("There was a problem registering this dll: SWattr is null.\n\"" + nl.Message + "\"");
            }

            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);

                System.Windows.Forms.MessageBox.Show("There was a problem registering the function: \n\"" + e.Message + "\"");
            }
        }

        /// <summary>
        /// This function is called to unregister this assembly with COM. It
        /// deletes registry kesy that were created when RegisterFunction(Type) 
        /// was called
        /// </summary>
        /// <param name="t">Type variable used to identify this assembly</param>
        [ComUnregisterFunctionAttribute]
        public static void UnregisterFunction(Type t)
        {
            try
            {
                Microsoft.Win32.RegistryKey hklm = Microsoft.Win32.Registry.LocalMachine;
                Microsoft.Win32.RegistryKey hkcu = Microsoft.Win32.Registry.CurrentUser;

                string keyname = "SOFTWARE\\SolidWorks\\Addins\\{" + t.GUID.ToString() + "}";
                hklm.DeleteSubKey(keyname);

                keyname = "Software\\SolidWorks\\AddInsStartup\\{" + t.GUID.ToString() + "}";
                hkcu.DeleteSubKey(keyname);
            }
            catch (System.NullReferenceException nl)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + nl.Message);
                System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: \n\"" + nl.Message + "\"");
            }
            catch (System.Exception e)
            {
                Console.WriteLine("There was a problem unregistering this dll: " + e.Message);
                System.Windows.Forms.MessageBox.Show("There was a problem unregistering this dll: \n\"" + e.Message + "\"");
            }
        }

        #endregion

        #region ISwAddin Implementation
        /// <summary>
        /// Called when this add in is loaded by solidworks.
        /// Allocates necessary resources for runtime.
        /// </summary>
        /// <param name="ThisSW">An instance of ISldWorks used to interact with the Solidworks application.</param>
        /// <param name="cookie">ID of this addin in the current Solidworks environment</param>
        /// <returns></returns>
        public bool ConnectToSW(object ThisSW, int cookie)
        {
            iSwApp = (SldWorks)ThisSW;
            addinID = cookie;
            RobotInfo.ClearLog();
            try
            {
                //Setup callbacks
                iSwApp.SetAddinCallbackInfo(0, this, addinID);

                Storage.StorageModel.DefineAttributes(iSwApp);

                //Setup the command managers
                cmdMgr = new CommandManager(iSwApp);

                AttachmentFactory.InitializeFactory();

                //Setup the document Manager
                iSwApp.ActiveDocChangeNotify += new DSldWorksEvents_ActiveDocChangeNotifyEventHandler(OnDocChange);
 
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        
        /// <summary>
        /// Called whin this add in is unloaded by solidworks.
        /// Disposes of all resources allocated during runtime.
        /// </summary>
        /// <returns></returns>
        public bool DisconnectFromSW()
        {

            try
            {
                //Release static pointers
                cmdMgr = null;
                currentRobot = null;
                currentDoc = null;

                //Release the interface to Solidworks
                System.Runtime.InteropServices.Marshal.ReleaseComObject(iSwApp);
                iSwApp = null;

                //Tell CLR to reclaim all resources used by this add in
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region Command Callbacks


        /// <summary>
        /// This method is called when the Edit Joints button is clicked by the user.
        /// </summary>
        /*public void editJointsCB()
        {
            //RobotEditorPMPage editor = new RobotEditorPMPage(currentRobot, currentDoc, iSwApp);
            //editor.RobotManagerLauncher += launchManageRobot;
            JointPMPage editor = new JointPMPage(currentRobot, currentRobot.GetLink(0).ChildJoints.ElementAt(0), currentDoc, iSwApp);
            editor.Show();
        }*/

        public void launchManageRobot(ISelectable select)
        {
            RobotInfo.WriteToLogFile("launchManageRobot from SwAddin executing (SwAddin)");
            manager.ExternalSelect(select);
            manager.Show();
            RobotInfo.WriteToLogFile("launchManageRobot from SwAddin complete (SwAddin)");
        }

        /// <summary>
        /// This method is called when the Solidworks command manager needs to determine wheter to enable the
        /// Edit Joints button.
        /// </summary>
        /// <returns>1 for enabled if there is a robot on the current model, 0 for disable if there is not robot on the current model</returns>
        public int settingsEN()
        {
            if (currentRobot != null && (exporter == null || exporter.IsClosed()))
                return 1;
            return 0;
        }

        public void settingsCB()
        {            
            RobotInfo.WriteToLogFile("\nSettings button pressed (SwAddin)");
            settings.WindowState = FormWindowState.Normal;
            settings.Show();
        }

        /// <summary>
        /// This method is called when the Manage Robot button is clicked by the user.
        /// </summary>
        public void manageRobotCB()
        {
            RobotInfo.WriteToLogFile("\nManage Robot Button Pressed (SwAddin)");
            System.Diagnostics.Debug.WriteLine("Manage Robot clicked");
            //System.Diagnostics.Debug.WriteLine("robot: " + manager.robot.Name);
            manager.WindowState = FormWindowState.Normal;
            manager.ExternalSelect(null);
            manager.Show();
            manager.BringToFront();
            System.Diagnostics.Debug.WriteLine("Done.");            
        }


        /// <summary>
        /// This method is called when the Solidworks command manager needs to determine wheter to enable the
        /// Manage Robot button.
        /// </summary>
        /// <returns>1 for enabled if there is a robot on the current model, 0 for disable if there is not robot on the current model</returns>
        public int manageRobotEN()
        {
            if (currentRobot != null && (exporter == null || exporter.IsClosed()))
                return 1;
            return 0;
        }

        /// <summary>
        /// This method is called when the Export button is clicked by the user.
        /// </summary>
        public void exportCB()
        {
            RobotInfo.WriteToLogFile("\nExport Button Pressed (SwAddin)");
            if (exporter == null || exporter.IsClosed())
            {
                exporter = new Exporter(currentRobot);
                exporter.RunExportSequence();
            }
            else
                exporter.BringToFront();                                    
        }

        /// <summary>
        /// This method is called when the Solidworks command manager needs to determine wheter to enable the
        /// Export button.
        /// </summary>
        /// <returns>1 for enabled if there is a robot on the current model, 0 for disable if there is not robot on the current model</returns>
        public int exportEN()
        {
            if (currentRobot != null)
                return 1;
            return 0;
        }

        #endregion

        public int OnDocChange()
        {
                currentRobot = null;
                currentDoc = null;
                ModelDoc2 activeDoc = iSwApp.ActiveDoc;
                if (activeDoc.GetType() == (int)swDocumentTypes_e.swDocASSEMBLY)
                {
                    currentDoc = (AssemblyDoc)activeDoc;
                    currentDoc.DestroyNotify2 += new DAssemblyDocEvents_DestroyNotify2EventHandler(OnDocClose);
                    currentRobot = new RobotModel(currentDoc, iSwApp);
                    if (manager != null && manager.Visible == true)
                    {
                        manager.Close();
                    }
                    if (settings!= null && settings.Visible == true)
                    {
                        settings.Close();
                    }
                    if (exporter != null && !exporter.IsClosed())
                    {
                        exporter.CloseExporter();
                        exporter = null;
                    }
                    manager = new ManageRobot(currentRobot);
                    settings = new SettingsForm(manager);
                }
            RobotInfo.WriteToLogFile("Doc changed (SwAddin)");
            return 0;
        }

        public int OnDocClose(int destroyType)
        {
            
            if (manager != null && manager.Visible == true)
            {
                manager.Close();
                manager = null;
            }
            if (settings.Visible == true)
            {
                settings.Close();
                settings = null;
            }
            if (exporter != null && !exporter.IsClosed())
            {
                exporter.CloseExporter();
                exporter = null;
            }
            currentRobot = null;

            RobotInfo.WriteToLogFile("Doc closed (SwAddin)\n");
            return 0;
        }

    }

}
