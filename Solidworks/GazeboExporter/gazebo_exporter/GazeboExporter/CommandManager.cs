using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks; 
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace GazeboExporter
{
    public class CommandManager
    {
        SldWorks iSwApp;
        ICommandManager iCmdMgr = null;
        CommandTab cmdTab;
        CommandTabBox cmdBox;
        CommandGroup cmdGroup;
        int createRobotItemID = 0;
        int autoDetectItemID = 1;
        int editLinksItemID = 2;
        int settingsItemID = 3;
        int manageRobotItemID = 4;
        int exportItemID = 5;
        int cmdGroupID = 6;
        public static Image GazeboLogo;
        public static Image MotorPic;
        public static Image EncoderPic;
        public static Image PotPic;
        public static Image GyroPic;
        public static Image RangefinderPic;
        public static Image CameraPic;
        public static Image ExtLimitSwitchPic;
        public static Image IntLimitSwitchPic;
        public static Image PistonPic;
        public static Image NewLinkPic;
        public static Image NewJointPic;

        public CommandManager(SldWorks iSwApp)
        {
            this.iSwApp = iSwApp;
            this.iCmdMgr = iSwApp.GetCommandManager(SwAddin.addinID);
            SetupCommands();
        }

        ~CommandManager()
        {
            
            cmdTab.RemoveCommandTabBox(cmdBox);
            iCmdMgr.RemoveCommandTab(cmdTab);
            iCmdMgr.RemoveCommandGroup(cmdGroupID);
            
            System.Runtime.InteropServices.Marshal.ReleaseComObject(iCmdMgr);
            iCmdMgr = null;
        }

        public void SetupCommands()
        {
            BitmapHandler imageManager = new BitmapHandler();
            Assembly thisAssembly;
            int cmdIndex0, cmdIndex1, cmdIndex2, cmdIndex3, cmdIndex4, cmdIndex5;
            string Title = "Gazebo", ToolTip = "Tools for exporting this assembly as a robot for simulation in Gazebo";


            int cmdGroupErr = 0;
            bool ignorePrevious = false;

            object registryIDs;
            //get the ID information stored in the registry
            bool getDataResult = iCmdMgr.GetGroupDataFromRegistry(cmdGroupID, out registryIDs);

            int[] knownIDs = new int[3] {settingsItemID, manageRobotItemID, exportItemID};

            if (getDataResult)
            {
                if (!CompareIDs((int[])registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }
            ignorePrevious = true;

            //Get a reference to the current assembly and load bitmaps form it into a new command group
            thisAssembly = System.Reflection.Assembly.GetAssembly(this.GetType());
            cmdGroup = iCmdMgr.CreateCommandGroup2(cmdGroupID, Title, ToolTip, "", -1, ignorePrevious, ref cmdGroupErr);
            cmdGroup.LargeIconList = imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.ToolbarLarge.bmp", thisAssembly);
            cmdGroup.SmallIconList = imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.ToolbarSmall.bmp", thisAssembly);
            cmdGroup.LargeMainIcon = imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.MainIconLarge.bmp", thisAssembly);
            cmdGroup.SmallMainIcon = imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.MainIconSmall.bmp", thisAssembly);
            GazeboLogo = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.Gazebo.png", thisAssembly));
            MotorPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.motor.png", thisAssembly));
            EncoderPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.QuadratureEncoder.png", thisAssembly));
            PotPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.AnalogPotentiometer.png", thisAssembly));
            GyroPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.Gyro.png", thisAssembly));
            RangefinderPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.Ultrasonic.png", thisAssembly));
            CameraPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.Camera.png", thisAssembly));
            ExtLimitSwitchPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.LimitSwitch_E.png", thisAssembly));
            IntLimitSwitchPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.LimitSwitch_I.png", thisAssembly));
            PistonPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.Piston.png", thisAssembly));
            NewLinkPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.NewLinkIcon.png", thisAssembly));
            NewJointPic = Image.FromFile(imageManager.CreateFileFromResourceBitmap("GazeboExporter.Images.NewJointIcon.png", thisAssembly));
            

            int menuToolbarOption = (int)(swCommandItemType_e.swMenuItem | swCommandItemType_e.swToolbarItem);
            cmdIndex3 = cmdGroup.AddCommandItem2("Exporter Settings", -1, "Change exporter plugin settings", "Exporter settings", 0, "settingsCB", "settingsEN", settingsItemID, menuToolbarOption);
            cmdIndex4 = cmdGroup.AddCommandItem2("Manage Robot", -1, "Manage robot paramters", "Manage Robot", 1, "manageRobotCB", "manageRobotEN", manageRobotItemID, menuToolbarOption);
            cmdIndex5 = cmdGroup.AddCommandItem2("Export", -1, "Save this robot as a Gazebo package", "Export", 2, "exportCB", "exportEN", exportItemID, menuToolbarOption);

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();
        

            cmdTab = iCmdMgr.GetCommandTab((int)swDocumentTypes_e.swDocASSEMBLY, Title);

            if (cmdTab != null & !getDataResult | ignorePrevious)//if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
            {
                bool res = iCmdMgr.RemoveCommandTab(cmdTab);
                cmdTab = null;
            }

            //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
            if (cmdTab == null)
            {
                cmdTab = iCmdMgr.AddCommandTab((int)swDocumentTypes_e.swDocASSEMBLY, Title);

                cmdBox = cmdTab.AddCommandTabBox();

                int[] cmdIDs = new int[3];
                int[] TextType = new int[3];

                cmdIDs[0] = cmdGroup.get_CommandID(cmdIndex3);
                cmdIDs[1] = cmdGroup.get_CommandID(cmdIndex4);
                cmdIDs[2] = cmdGroup.get_CommandID(cmdIndex5);

                TextType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
                TextType[1] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
                TextType[2] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
                /*int[] cmdIDs = new int[2];
                int[] TextType = new int[2];

                cmdIDs[0] = cmdGroup.get_CommandID(cmdIndex4);
                cmdIDs[1] = cmdGroup.get_CommandID(cmdIndex5);

                TextType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;
                TextType[1] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow;*/
                
                cmdBox.AddCommands(cmdIDs, TextType);
            }

            thisAssembly = null;
            imageManager.Dispose();
        }


        public bool CompareIDs(int[] storedIDs, int[] addinIDs)
        {
            List<int> storedList = new List<int>(storedIDs);
            List<int> addinList = new List<int>(addinIDs);

            addinList.Sort();
            storedList.Sort();

            if (addinList.Count != storedList.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < addinList.Count; i++)
                {
                    if (addinList[i] != storedList[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void refreshCommands()
        {
            cmdGroup.Activate();
        }

    }


    
}
