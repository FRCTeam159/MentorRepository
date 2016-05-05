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
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;

namespace GazeboExporter.Export
{
    /// <summary>
    /// This class is used to export parts of a model into STL files
    /// </summary>
    public class STLExporter
    {
        /// <summary>
        /// number of operations to export 1 stl
        /// </summary>
        public const int NumOps = 3;
        SldWorks iSwApp;
        AssemblyDoc asm;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iSwApp">Solidworks app</param>
        /// <param name="asm">The assembly document the export takes place in</param>
        public STLExporter(SldWorks iSwApp, AssemblyDoc asm)
        {
            this.iSwApp = iSwApp;
            this.asm = asm;

            saveUserPreferences();
            setSTLExportPreferences();
        }


        /// <summary>
        /// Exports the .stl model for one link
        /// The model must already be switched to the correct configuration and all parts must be hidden before this method is called
        /// </summary>
        /// <param name="link">Link to be exported</param>
        /// <param name="configuration">The configuration that this link should be exported from</param>
        /// <param name="path">The path to save the .stl file</param>
        /// <param name="log">The logger to write messages to</param>
        public void ExportLink(Link link ,ModelConfiguration configuration, String path, ProgressLogger log)
        {
            log.WriteMessage("Exporting link " + link.Name + " as an STL. Configuration: " + ((ModelConfiguration.ModelConfigType)configuration.Type) + "; Path: " + path, false);

            int errors = 0;
            int warnings = 0;

            ModelDoc2 ActiveDoc = (ModelDoc2)asm;

            IsolateLink(link, configuration);
            

            int saveOptions = (int)swSaveAsOptions_e.swSaveAsOptions_Silent;

            ActiveDoc.Extension.SaveAs(path, (int)swSaveAsVersion_e.swSaveAsCurrentVersion, saveOptions, null, ref errors, ref warnings);
            
            HideLink(link);

            log.WriteMessage("Correcting STL header");
            CorrectSTLMesh(path);
            log.WriteMessage("Finished exporting STL.");  
        }

        

        /// <summary>
        /// Should be called after all exports are done
        /// resets the user preferences
        /// </summary>
        public void close()
        {
            resetUserPreferences();
        }

        #region Export Preference Manipulation
        //Adapted from old exporter

        private bool mBinary;
        private bool mshowInfo;
        private bool mSTLPreview;
        private bool mTranslateToPositive;
        private bool mSaveComponentsIntoOneFile;
        private int mSTLUnits;
        private int mSTLQuality;
        private double mHideTransitionSpeed;

        //Saves the preferences that the user had setup so that I can change them and revert back to their configuration
        public void saveUserPreferences()
        {
            mBinary = iSwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLBinaryFormat);
            mTranslateToPositive = iSwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLDontTranslateToPositive);
            mSTLUnits = iSwApp.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swExportStlUnits);
            mSTLQuality = iSwApp.GetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swSTLQuality);
            mshowInfo = iSwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLShowInfoOnSave);
            mSTLPreview = iSwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLPreview);
            mHideTransitionSpeed = iSwApp.GetUserPreferenceDoubleValue((int)swUserPreferenceDoubleValue_e.swViewTransitionHideShowComponent);
            mSaveComponentsIntoOneFile = iSwApp.GetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLComponentsIntoOneFile);
        }

        //This is how the STL export preferences need to be to properly export
        public void setSTLExportPreferences()
        {
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLBinaryFormat, true);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLDontTranslateToPositive, true);
            iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swExportStlUnits, 2);
            iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swSTLQuality, (int)swSTLQuality_e.swSTLQuality_Coarse);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLShowInfoOnSave, false);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLPreview, false);
            iSwApp.SetUserPreferenceDoubleValue((int)swUserPreferenceDoubleValue_e.swViewTransitionHideShowComponent, 0);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLComponentsIntoOneFile, true);
        }

        //This resets the user preferences back to what they were.
        public void resetUserPreferences()
        {
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLBinaryFormat, mBinary);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLDontTranslateToPositive, mTranslateToPositive);
            iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swExportStlUnits, mSTLUnits);
            iSwApp.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swSTLQuality, mSTLQuality);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLShowInfoOnSave, mshowInfo);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLPreview, mSTLPreview);
            iSwApp.SetUserPreferenceDoubleValue((int)swUserPreferenceDoubleValue_e.swViewTransitionHideShowComponent, mHideTransitionSpeed);
            iSwApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSTLComponentsIntoOneFile, mSaveComponentsIntoOneFile);
        }

        #endregion

        #region Private Helper Methods

        //Adapted from old exporter
        //Writes an empty header to the STL to get rid of the BS that SolidWorks adds to a binary STL file
        private void CorrectSTLMesh(string filename)
        {
            FileStream fileStream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            byte[] emptyHeader = new byte[80];
            fileStream.Write(emptyHeader, 0, emptyHeader.Length);
            fileStream.Close();
        }

        /// <summary>
        /// Isolates the selceted link by hiding all other components
        /// </summary>
        /// <param name="l"> Link to be isolated</param>
        private void IsolateLink(Link l, ModelConfiguration config)
        {
            List<Component2> comps = new List<Component2>();
            foreach (modelComponent m in config.LinkComponents)
            {
                Component2 c = m.Component;
                if (c == null )
                    continue;

                if (((object[])c.GetChildren()).Length > 0)
                {
                    GetSubComponents(c, comps);
                }
                else
                {
                    comps.Add(c);
                    //System.Diagnostics.Debug.WriteLine(c.Name2);
                }
            }
            DispatchWrapper[] dispComps = Array.ConvertAll(comps.ToArray(),element=>new DispatchWrapper(element));

            SelectionMgr manager = ((ModelDoc2)asm).SelectionManager;
            SelectData data = manager.CreateSelectData();
            data.Mark = -1;
            manager.SuspendSelectionList();
            manager.AddSelectionListObjects(comps.ToArray(), data);
            
            ((ModelDoc2)asm).ShowComponent2();
            manager.ResumeSelectionList();
        }

        /// <summary>
        /// gets all subComponents if a component is a subassembly
        /// </summary>
        /// <param name="comp">The component to find its subComponents of</param>
        /// <param name="comps">A List to store the subComponents in</param>
        private void GetSubComponents(Component2 comp, List<Component2> comps)
        {
            object[] childrenComps = (object[])comp.GetChildren(); 
            if (childrenComps.Length>0)
            {
                foreach (Component2 c in childrenComps)
                {
                    if (((object[])c.GetChildren()).Length > 0)
                    {
                        GetSubComponents(c, comps);
                    }
                    else
                    {
                        comps.Add(c);
                    }
                }
            }
        }

        /// <summary>
        /// Hides all components in the link
        /// </summary>
        /// <param name="l">Link to be hidden</param>
        private void HideLink(Link l)
        {
            ((ModelDoc2)asm).Extension.SelectAll();
            int temp = ((SelectionMgr)((ModelDoc2)asm).SelectionManager).GetSelectedObjectCount2(-1);//this fixes stuff. no idea why
            ((ModelDoc2)asm).HideComponent2();//since all componets are still selected from the isolate link method, they can simply be rehidden
            ((ModelDoc2)asm).ClearSelection2(true);
        }
        #endregion

        #region Model Manipulation 

        /// <summary>
        /// Hides all components in the assembly
        /// </summary>
        /// <returns>returns an array containing all the components that were originally hidden</returns>
        public Component2[] HideAllComponents()
        {
            LinkedList<Component2> hiddenComps = new LinkedList<Component2>();
            object[] obs = asm.GetComponents(false);
            SelectionMgr manager = ((ModelDoc2)asm).SelectionManager;
            SelectData data = manager.CreateSelectData();
            data.Mark = -1;
            ((ModelDoc2)asm).ClearSelection2(true);
            foreach (object o in obs)
            {
                if (((Component2)o).IsHidden(true))
                {
                    hiddenComps.AddLast((Component2)o);
                }
            }

            ((ModelDoc2)asm).Extension.SelectAll();
            ((ModelDoc2)asm).HideComponent2();
            ((ModelDoc2)asm).ClearSelection2(true);
            return hiddenComps.ToArray();
        }

        /// <summary>
        /// unhides all components in the assembly
        /// </summary>
        /// <param name="excludeds">Components that should remain hidden</param>
        public void UnhideComponents(Component2[] excludeds)
        {
            object[] obs = asm.GetComponents(false);
            SelectionMgr manager = ((ModelDoc2)asm).SelectionManager;
            SelectData data = manager.CreateSelectData();
            data.Mark = -1;

            

            DispatchWrapper[] dispComps = Array.ConvertAll(obs, e => new DispatchWrapper(e));
            manager.AddSelectionListObjects(dispComps, data);

            ((ModelDoc2)asm).ShowComponent2();
            ((ModelDoc2)asm).ClearSelection2(true);

            ((ModelDoc2)asm).Extension.MultiSelect2(excludeds, false, data);
            ((ModelDoc2)asm).HideComponent2();
            ((ModelDoc2)asm).ClearSelection2(true);
        }
        #endregion
    }
}
