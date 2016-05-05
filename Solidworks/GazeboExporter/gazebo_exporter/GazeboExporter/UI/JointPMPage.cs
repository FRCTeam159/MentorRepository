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
using System.Windows.Forms;

namespace GazeboExporter.UI
{

    public delegate void RobotManagerCallback(ISelectable s);
    /// <summary>
    /// This creates a SW proprty manager page to edit joint properties
    /// </summary>
    public class JointPMPage : PropertyManagerPage2Handler9
    {
        #region Fields and Properties
        

        public RobotManagerCallback RestoreManager;

        //Object to interface with this Solidworks property manager page
        public PropertyManagerPage2 page;


        //Active robot model
        private RobotModel robot;
        //Allows access to the current assembly document
        private AssemblyDoc assemblyDoc;
        //Allows access to the current model document 
        private ModelDoc2 modelDoc;
        //Allows access to the current solidworks application instance
        private SldWorks swApp;
        //The joint that is currently being edited
        private Joint currentJoint;

        public int PrevSelectId;

        IMathUtility mathUtil;

        const double errorVal = .00000001;

        public List<SelectionObserver> SelectionObservers{get; private set;}

        public List<ButtonObserver> ButtonObservers { get; private set; }

        public List<CheckboxObserver> CheckboxObservers { get; private set; }


        public int currentId = 1;
        public int currentMark = 1;

        #endregion

        #region Constructor and Setup
        /// <summary>
        /// Creates a new joint editor proptery manager page that allows the user to edit joint properties.
        /// </summary>
        /// <param name="robot">Active robot model</param>
        /// <param name="joint">Joint to be edited</param>
        /// <param name="document">Active assembly document</param>
        /// <param name="swApp">Solidworks application environment</param>
        public JointPMPage(RobotModel robot, Joint joint, AssemblyDoc document, SldWorks swApp)
        {
            //Validate parameters
            if (robot == null)
                throw new ProgramErrorException("Tried to create a JointPMPage with a null robot model.");
            if (document == null)
                throw new ProgramErrorException("Tried to create a JointPMPage with a null assembly document.");
            if (swApp == null)
                throw new ProgramErrorException("Tried to create a JointPMPage with a null solidworks application interface.");

            //Initialize fields
            this.robot = robot;
            this.swApp = swApp;
            currentJoint = joint;
            mathUtil = ((IMathUtility)swApp.GetMathUtility());

            //AssemblyDoc inherits ModelDoc2 but the relationship dosn't carry through the COM interface
            //Having two fields prevents having to cast half of the time
            modelDoc = (ModelDoc2)document;
            assemblyDoc = document;

            //Setup the page, its controls and their visual layout
            SetupPage();

            SelectionObservers = new List<SelectionObserver>();
            ButtonObservers = new List<ButtonObserver>();
            CheckboxObservers = new List<CheckboxObserver>();

        }

        /// <summary>
        /// Creates the property manager page and populates it with controls
        /// </summary>
        private void SetupPage()
        {
            int errors = 0;

            //Create a property manager page and throw exceptions if an error occurs
            page = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Edit Joint: " + currentJoint.Name,
                                                                                   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
                                                                                   this, ref errors);
            if (errors == -1) //Creation failed
                throw new InternalSolidworksException("SldWorks::CreatePropertyManagerPage", "Failed to create property manager page");
            else if (errors == -2) //No open document
                throw new ProgramErrorException("Tried to open a property manager page when no document was open");
            else if (errors == 1) //Invlaid hanlder
                throw new ProgramErrorException("Tried to pass in and invalid page hanlder when creating a property manager page");


        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show this property manager page
        /// </summary>
        public void Show()
        {
            currentJoint.jointSpecifics.AddJointPoseToPMPage(this);
            page.Show();
        }


        #endregion

        #region Joint Selection
        /// <summary>
        /// Saves the currently selected joint's information back into the robot model
        /// </summary>
        private void SaveCurrentJointSelection()
        {
            foreach (SelectionObserver o in SelectionObservers)
            {
                o.SaveSelections();
            }
            SelectionObservers.Clear();
        }

        

        #endregion


        #region Handler Implementation

        #region Side Panel handlers
        /// <summary>
        /// Called when this property manager page is closed
        /// </summary>
        /// <param name="Reason">Reason this page is closing</param>
        public void OnClose(int Reason)
        {
            if (currentJoint != null)
                SaveCurrentJointSelection();
            modelDoc.ClearSelection2(true);
            if (Reason != (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_ParentClosed && RestoreManager != null)
                RestoreManager(currentJoint);
        }

        /// <summary>
        /// Called to verify whether to submit a newly selected item into a selection box on this property manager page
        /// </summary>
        /// <param name="Id">Id of selection box in focus</param>
        /// <param name="Selection">Item selected</param>
        /// <param name="SelType">Type of item selected</param>
        /// <param name="ItemText">Text to enter into selectionbox for this item</param>
        /// <returns>true if valid selection</returns>
        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            bool isValid = false;
            foreach (SelectionObserver o in SelectionObservers)
            {
                o.CheckValidSelection(out isValid, Id, SelType, Selection);
                if (isValid)
                {
                    o.UpdatePreviews(Id,Selection);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// updates options for axis direction when a corresponding checkbox is toggled
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Checked"></param>
        public void OnCheckboxCheck(int Id, bool Checked)
        {
            foreach (CheckboxObserver o in CheckboxObservers)
            {
                o.CheckboxChanged(Id, Checked);
            }
        }

        public void OnButtonPress(int Id)
        {
            foreach (ButtonObserver o in ButtonObservers)
            {
                o.ButtonChanged(Id);
            }
        }

        /// <summary>
        /// when use selection box: updates values, draws previews, and selects limits if applicable 
        /// </summary>
        /// <param name="Id"></param>
        public void OnSelectionboxFocusChanged(int Id)
        {
            foreach (SelectionObserver o in SelectionObservers)
            {
                o.SaveSpecificSelection(PrevSelectId);
            }
            PrevSelectId = Id;
             
        }

        /// <summary>
        /// Called when selectionbox list changes. used to detect unselecting
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Count"></param>
        public void OnSelectionboxListChanged(int Id, int Count)
        {
            if (Count == 0)
            {
                foreach (SelectionObserver o in SelectionObservers)
                {
                    o.NoSelections(Id);
                    o.SaveSpecificSelection(Id);
                }
            }
        }
        #endregion 

        #region Unused Hanlders

        public void OnComboboxSelectionChanged(int Id, int Item)
        {
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        public void OnTextboxChanged(int Id, string Text)
        {

        }

        

        public void AfterActivation()
        {
        }

        public void AfterClose()
        {
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            return 0;
        }


        public void OnComboboxEditChanged(int Id, string Text)
        {
        }



        public void OnGainedFocus(int Id)
        {
        }

        public void OnGroupCheck(int Id, bool Checked)
        {
        }

        public void OnGroupExpand(int Id, bool Expanded)
        {
        }

        public bool OnHelp()
        {
            return false;
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return false;
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
        }

        public void OnLostFocus(int Id)
        {
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
        }

        public bool OnNextPage()
        {
            return false;
        }

        public void OnNumberboxChanged(int Id, double Value)
        {
        }

        public void OnOptionCheck(int Id)
        {
        }

        public void OnPopupMenuItem(int Id)
        {
        }

        public void OnPopupMenuItemUpdate(int Id, ref int retval)
        {
        }

        public bool OnPreview()
        {
            return false;
        }

        public bool OnPreviousPage()
        {
            return false;
        }

        public void OnRedo()
        {
        }

        public void OnSelectionboxCalloutCreated(int Id)
        {
        }

        public void OnSelectionboxCalloutDestroyed(int Id)
        {
        }





        public void OnSliderPositionChanged(int Id, double Value)
        {
        }

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
        }



        public void OnUndo()
        {
        }

        public void OnWhatsNew()
        {
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 0;
        }

        #endregion


        #endregion


    
        
    }
}
