using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using GazeboExporter.GazeboException;
using GazeboExporter.Robot;
using GazeboExporter;

namespace GazeboExporter.UI
{

    /// <summary>
    /// A class to display a Solidworks property manager page to
    /// edit the links of the current robot.
    /// </summary>
    public class LinkPMPage : PropertyManagerPage2Handler9
    {
        #region Fields and Properties
        public RobotManagerCallback RestoreManager;

        //Object to interface with this Solidworks property manager page
        PropertyManagerPage2 page;
        //property controls for the page
        PropertyManagerPageGroup linkPropertiesGroup; private const int linkPropertiesGroupID = 1;
        PropertyManagerPageCheckbox linkColorCheckbox; private const int linkColorCheckboxID = 2;
        PropertyManagerPageLabel linkNameLabel; private const int linkNameLabelID = 3;
        PropertyManagerPageTextbox linkNameTextbox; private const int linkNameTextboxID = 4;

        PropertyManagerPageLabel modelTypeLabel; private const int modelTypeLabelID = 7;
        PropertyManagerPageTextbox modelTypeTextbox; private const int modelTypeTextboxID = 8;
        PropertyManagerPageLabel swConfigLabel; private const int swConfigLabelID = 9;
        PropertyManagerPageTextbox swConfigTextbox; private const int swConfigTextboxID = 10;

        PropertyManagerPageLabel linkComponentsLabel; private const int linkComponentsLabelID = 5;
        PropertyManagerPageSelectionbox linkComponentsSelectionbox; private const int linkComponentsSelectionboxID = 6;

        PropertyManagerPageCheckbox isEmptyCheckbox; private const int isEmptyCheckboxID = 11;

        //Active robot model
        private RobotModel robot;
        //Currently selected link
        private Link currentLink;
        //currently selected model config type
        private int currentModelConfig; //0 = collision; 1 = visual
        //number of components in the current model configuation
        private int numComponents = 0;
        //Allows access to the current assembly document
        private AssemblyDoc assemblyDoc;
        //Allows access to the current model document 
        private ModelDoc2 modelDoc;
        //Allows access to the current solidworks application instance
        private SldWorks swApp;
        //Display state active when page was opened
        private String savedDisplayState;
        private const String colorDisplayState = "GazeboLinkColors";
        //True if the LinkColors display state is active
        private bool linksColored;
        //Info message
        private String infoMessage = "A link is a rigid part of a robot made up of a set of components that do not move relative to eachother. " +
                                    "Use this pane to add links, remove links and select which components belong to which links. " +
                                    "When a link is selected in the box below, its components will be highleted in your enviorment's selection color, " +
                                    "and you may select or de-select components that are part of that link by clicking on them in the feature tree or model.";

        #endregion

        #region Constructor and Setup

        /// <summary>
        /// Constructor for setting up the link property manager page
        /// </summary>
        /// <param name="robot">The robot model</param>
        /// <param name="link">The link that will be edited</param>
        /// <param name="document">Active assembly document</param>
        /// <param name="swApp">The solidworks app</param>
        public LinkPMPage(RobotModel robot, AssemblyDoc document, SldWorks swApp, int type)
        {
            //Validate parameters
            if (robot == null)
                throw new ProgramErrorException("Tried to create a LinkEditorPMPage with a null robot model.");
            if (document == null)
                throw new ProgramErrorException("Tried to create a LinkEditorPMPage with a null assembly document.");
            if (swApp == null)
                throw new ProgramErrorException("Tried to create a LinkEditorPMPage with a null solidworks application interface.");

            //Initialize fields
            this.robot = robot;
            this.swApp = swApp;
            this.linksColored = false;
            currentModelConfig = type;
            


            //AssemblyDoc inherits ModelDoc2 but the relationship dosn't carry through the COM interface
            //Having two fields prevents having to cast half of the time
            modelDoc = (ModelDoc2)document;
            assemblyDoc = document;

            //Setup the page, its controls and their visual layout
            SetupPage();
        }

        /// <summary>
        /// Creates the property manager page and populates it with controls
        /// </summary>
        private void SetupPage()
        {
            int errors = 0;

            //Create a property manager page and throw exceptions if an error occurs
            page = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Edit Links",
                                                                                   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
                                                                                   this, ref errors);
           
            if (errors == -1) //Creation failed
                throw new InternalSolidworksException("SldWorks::CreatePropertyManagerPage", "Failed to create property manager page");
            else if (errors == -2) //No open document
                throw new ProgramErrorException("Tried to open a property manager page when no document was open");
            else if (errors == 1) //Invlaid hanlder
                throw new ProgramErrorException("Tried to pass in and invalid page hanlder when creating a property manager page");


            //Helper variables for setup options
            //control type
            short labelCT = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            short selectionboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            short checkboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            short textboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            // align
            short leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            // options
            int groupboxOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible; // for group box
            int controlOptions = (int)swAddControlOptions_e.swControlOptions_Enabled |
                                 (int)swAddControlOptions_e.swControlOptions_Visible; // for controls
            int readonlyOptions = (int)swAddControlOptions_e.swControlOptions_Visible; 
            

            //Sets the page's message
            page.SetMessage3(infoMessage,
                                    (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
            //Setup and create the link properties group
            linkPropertiesGroup = (PropertyManagerPageGroup)page.
                AddGroupBox(linkPropertiesGroupID, "Link Properties", groupboxOptions);

            //Setup and create link colors checkbox
            linkColorCheckbox = (PropertyManagerPageCheckbox)linkPropertiesGroup.
                AddControl(linkColorCheckboxID, checkboxCT, "Color components by link", leftAlign, controlOptions, "Check to change all components that are part of links to the color of the link");
            linkColorCheckbox.Checked = false;


            //show Link name, Model type, and SW config to remind user
            //Setup and create the label for the link name texbox
            linkNameLabel = (PropertyManagerPageLabel)linkPropertiesGroup.
                AddControl(linkNameLabelID, labelCT, "Link Name:", leftAlign, controlOptions, "The name of this link");
            //Setup and create the link name textbox
            linkNameTextbox = (PropertyManagerPageTextbox)linkPropertiesGroup.
                AddControl(linkNameTextboxID, textboxCT, "Link Name", leftAlign, readonlyOptions, "The name of this link");
            //Setup and create the label for the model type texbox
            modelTypeLabel = (PropertyManagerPageLabel)linkPropertiesGroup.
                AddControl(modelTypeLabelID, labelCT, "Model:", leftAlign, controlOptions, "The current model type");
            //Setup and create the model type textbox
            modelTypeTextbox = (PropertyManagerPageTextbox)linkPropertiesGroup.
                AddControl(modelTypeTextboxID, textboxCT, "Model", leftAlign, readonlyOptions, "The current model type");
            //Setup and create the label for the sw configuration texbox
            swConfigLabel = (PropertyManagerPageLabel)linkPropertiesGroup.
                AddControl(swConfigLabelID, labelCT, "Solidworks Configuration:", leftAlign, controlOptions, "The current Solidworks configuration");
            //Setup and create the sw configuration textbox
            swConfigTextbox = (PropertyManagerPageTextbox)linkPropertiesGroup.
                AddControl(swConfigTextboxID, textboxCT, "Solidworks Configuration", leftAlign, readonlyOptions, "The current Solidworks configuration");

            isEmptyCheckbox = (PropertyManagerPageCheckbox)linkPropertiesGroup.
                AddControl(isEmptyCheckboxID, checkboxCT, "No " + ((ModelConfiguration.ModelConfigType)currentModelConfig) + " model for this link", leftAlign, controlOptions, "Check to make this model have no components");
            ((PropertyManagerPageControl)isEmptyCheckbox).Visible = (currentModelConfig == (int)ModelConfiguration.ModelConfigType.Collision);

            //Setup and create the label for the link components selection box
            linkComponentsLabel = (PropertyManagerPageLabel)linkPropertiesGroup.
                AddControl(linkComponentsLabelID, labelCT, "Link Components", leftAlign, controlOptions, "Use this box to select components that are part of this link");

            //Setup and create the link components selection box
            linkComponentsSelectionbox = (PropertyManagerPageSelectionbox)linkPropertiesGroup.
                AddControl(linkComponentsSelectionboxID, selectionboxCT, "Link Components", leftAlign, controlOptions, "Use this box to select components that are part of this link");
            linkComponentsSelectionbox.Height = 75;
            linkComponentsSelectionbox.Mark = 2;
            int[] filter = { (int)swSelectType_e.swSelCOMPONENTS };
            linkComponentsSelectionbox.SetSelectionFilters(filter);
            linkComponentsSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            //linkComponentsSelectionbox.SetSelectionColor(false, (int) null);

            
        
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Show this property manager page
        /// </summary>
        public void Show()
        {
            page.Show();
        }

        /// <summary>
        /// update currentLink when new link selected 
        /// </summary>
        /// <param name="link"></param>
        public void OnLinkSelectionChanged(Link link)
        {
            if (currentLink != null)
                SaveCurrentLinkSelection();
            //RevertLinkColors(); 
            
            currentLink = link;
            LoadNewLinkSelection();
        }
        #endregion

        #region Coloring Methods
        /// <summary>
        /// colors components of other links (for the same type of model configuration) with the colors specified in the Robot Manager graph
        /// </summary>
        private void ColorLinks()
        {
            if (!linksColored)
            {
                //save current display state and apply another display states
                Configuration config = modelDoc.ConfigurationManager.ActiveConfiguration;
                string[] displayStates = config.GetDisplayStates();
                if (!displayStates.Contains(colorDisplayState))
                    config.CreateDisplayState(colorDisplayState);
                savedDisplayState = (displayStates[0].Equals(colorDisplayState)) ?
                                        displayStates[1] : displayStates[0];
                //System.Diagnostics.Debug.Print(savedDisplayState);
                config.ApplyDisplayState(colorDisplayState);

                //color links
                foreach (Link l in robot.GetLinksAsArray().Except(new Link[]{currentLink})) // don't color current selections because will not de-color if deselected and because selections are automatically colored in Solidworks
                    l.ColorLink(currentModelConfig);

                modelDoc.WindowRedraw();
                linksColored = true;
            }
        }

        /// <summary>
        /// reverts the links to their original colors
        /// </summary>
        private void RevertLinkColors()
        {
            if (linksColored)
            {
                Configuration config = modelDoc.ConfigurationManager.ActiveConfiguration;
                string[] displayStates = config.GetDisplayStates();
                config.ApplyDisplayState(savedDisplayState);
                
                if (displayStates.Contains(colorDisplayState))
                    config.DeleteDisplayState(colorDisplayState);

                modelDoc.WindowRedraw();
                linksColored = false;
            }
        }
        #endregion

        #region Link Selection Methods
        /// <summary>
        /// Saves all currently selected components to the currently selected link's information in the robot model
        /// </summary>
        private void SaveCurrentLinkSelection()
        {

            SelectionMgr selectionManager = (SelectionMgr)modelDoc.SelectionManager;
            int numComponents = selectionManager.GetSelectedObjectCount2(2);
            currentLink.LinkModels[currentModelConfig].Reset();

            Component2 temp;
            for (int l = 1; l <= numComponents; l++)
            {
                temp = (Component2)((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectsComponent4(l, 2);
                currentLink.LinkModels[currentModelConfig].insertModelComp(temp);
            }
            //currentLink.LinkModels[currentModelConfig] = selectedComponents;

        }

        /// <summary>
        /// Loads the currently selected link's information from the robot model
        /// </summary>
        private void LoadNewLinkSelection()
        {

            modelDoc.ClearSelection2(true);
            
            SelectData data = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
            data.Mark = 2;
            List<modelComponent> temp = currentLink.LinkModels[currentModelConfig].LinkComponents;

            for (int i = 0; i < temp.Count; i++ )
            {
                if (temp[i].Component != null && temp[i].ConfigName == modelDoc.ConfigurationManager.ActiveConfiguration.Name)
                {
                    temp[i].Component.Select4(true, data, false);
                }
            }
                

            linkNameTextbox.Text = currentLink.Name;
            modelTypeTextbox.Text = ((ModelConfiguration.ModelConfigType)currentLink.LinkModels[currentModelConfig].Type).ToString() ;
            swConfigTextbox.Text = modelDoc.ConfigurationManager.ActiveConfiguration.Name;
            linkColorCheckbox.Checked = linksColored;
            numComponents = currentLink.LinkModels[currentModelConfig].LinkComponents.Count;
            isEmptyCheckbox.Checked = currentLink.LinkModels[currentModelConfig].EmptyModel;
            OnCheckboxCheck(isEmptyCheckboxID, isEmptyCheckbox.Checked);
        }
        #endregion

        #region Handler Implementation
        /// <summary>
        /// updates options for link color, manual limits, or axis direction when a corresponding checkbox is toggled
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Checked"></param>
        public void OnCheckboxCheck(int Id, bool Checked)
        {
            if (Id == linkColorCheckboxID)
            {
                if (Checked)
                    ColorLinks();
                else
                    RevertLinkColors();
            }
            else if (Id == isEmptyCheckboxID)
            {
                currentLink.LinkModels[currentModelConfig].EmptyModel = Checked;
                ((PropertyManagerPageControl)linkComponentsSelectionbox).Enabled = !Checked;
                ((PropertyManagerPageControl)linkComponentsLabel).Enabled = !Checked;
                if (Checked)
                {
                    currentLink.LinkModels[currentModelConfig].Reset();
                    modelDoc.ClearSelection2(true);
                }
                
            }
        }

        /// <summary>
        /// Called to verify whether to submit a newly selected item into a selection box on this property manager page
        /// only called if component is selected (not deselcted)
        /// </summary>
        /// <param name="Id">Id of selection box in focus</param>
        /// <param name="Selection">Item selected</param>
        /// <param name="SelType">Type of item selected</param>
        /// <param name="ItemText">Text to enter into selectionbox for this item</param>
        /// <returns>true if valid selection</returns>
        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            //Link components selectionbox is in focus
            if (Id == linkComponentsSelectionboxID)
            {
                if (!((PropertyManagerPageControl)linkComponentsSelectionbox).Enabled)
                    return false;
                if (SelType == (int)swSelectType_e.swSelCOMPONENTS)
                {
                    currentLink.LinkModels[currentModelConfig].insertModelComp((Component2)Selection);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Called after OnSubmitSelection when a component is either selected or deselected
        /// Updates which components are colored.
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Count"></param>
        public void OnSelectionboxListChanged(int Id, int Count)
        {

            //check if a component had been deselected
            //Count is the number of items currently in the selectionbox
            if (Count == currentLink.LinkModels[currentModelConfig].LinkComponents.Count)
            {
                SaveCurrentLinkSelection();
                RevertLinkColors();
            }

            //update numComponents
            numComponents = Count;
            
            // recolor if necessary
            if (linkColorCheckbox.Checked)
                ColorLinks();
       
        }

        /// <summary>
        /// Called when this property manager page is closed
        /// </summary>
        /// <param name="Reason">Reason this page is closing</param>
        public void OnClose(int Reason)
        {
            if (currentLink != null)
                SaveCurrentLinkSelection();
            RevertLinkColors();
            if (!currentLink.UseCustomInertial && currentLink.LinkModels[currentModelConfig].Equals(currentLink.PhysicalModel) )
                currentLink.CalcInertia(null);
            modelDoc.ClearSelection2(true);
            if (Reason != (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_ParentClosed && RestoreManager != null)
                RestoreManager(currentLink);
        }

        /// <summary>
        /// Called when leaving a propery manager page 
        /// </summary>
        public void AfterClose()
        {
            // if the PMPage is not closed properly, de-selected components will not be saved as not selected
            if (currentLink.LinkModels[currentModelConfig].LinkComponents.Count > numComponents && !PluginSettings.DebugMode)
            {
                string msg = "A new Link Property Page was opened before the previous one had properly closed. Component selections for link " + currentLink.Name + " may not be properly saved. Please check the components of link " + currentLink.Name + ".";
                MessageBox.Show(msg, "Warning:Components not saved", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            //close display state to make sure links are colored correctly in case a color is changed 
            Configuration config = modelDoc.ConfigurationManager.ActiveConfiguration;
            string[] displayStates = config.GetDisplayStates();
            if (displayStates.Contains(colorDisplayState))
                config.DeleteDisplayState(colorDisplayState);
        }

        #region Unused Handlers
        // NAME TEXTBOX DISABLED
        /// <summary>
        /// update link name when the textbox changes
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Text"></param>
        public void OnTextboxChanged(int Id, string Text)
        {
            if (Id == linkNameTextboxID)
            {
                currentLink.Name = Text;
            }
        }


        public void OnComboboxSelectionChanged(int Id, int Item)
        {
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
        }

        public void OnSelectionboxFocusChanged(int Id)
        {
        }

        public bool OnTabClicked(int Id)
        {
            return true;
        }
        public void OnButtonPress(int Id)
        {
        }

        public void AfterActivation()
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
