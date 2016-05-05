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

namespace GazeboExporter.UI
{

    public delegate void Launcher(ISelectable select);

    /// <summary>
    /// A class to display a Solidworks property manager page to
    /// edit the links of the current robot.
    /// </summary>
    public class RobotEditorPMPage : PropertyManagerPage2Handler9, SwManipulatorHandler2
    {

        public Launcher RobotManagerLauncher;
        
        #region Fields and Properties

        //Object to interface with this Solidworks property manager page
        PropertyManagerPage2 page;

        //Property manager page controls (configured in the setupPage() method) and unique IDs for them

        //Tabs
        PropertyManagerPageTab linkTab;                                 private const int linkTabID = 32;
        PropertyManagerPageTab jointTab;                                private const int jointTabID = 33;
        //Link selector group
        PropertyManagerPageGroup selectGroup;                           private const int selectGroupID = 0;
        PropertyManagerPageCheckbox linkColorCheckbox;                  private const int linkColorCheckboxID = 1;
        PropertyManagerPageWindowFromHandle linkSelectorHandle;         private const int linkSelectorHandleID = 2;
        //Link properties group
        PropertyManagerPageGroup linkPropertiesGroup;                   private const int linkPropertiesGroupID = 3;
        PropertyManagerPageLabel linkNameLabel;                         private const int linkNameLabelID = 4;
        PropertyManagerPageTextbox linkNameTextbox;                     private const int linkNameTextboxID = 5;
        PropertyManagerPageLabel linkComponentsLabel;                   private const int linkComponentsLabelID = 6;
        PropertyManagerPageSelectionbox linkComponentsSelectionbox;     private const int linkComponentsSelectionboxID = 7;
        //Joint properties group
        PropertyManagerPageGroup jointPropertiesGroup;                  private const int jointPropertiesGroupID = 8;
        PropertyManagerPageLabel jointTypeLabel;                        private const int jointTypeLabelID = 9;
        PropertyManagerPageCombobox jointTypeCombobox;                  private const int jointTypeComboboxID = 10;
        PropertyManagerPageLabel jointAxis1Label;                       private const int jointAxis1LabelID = 11;
        PropertyManagerPageSelectionbox jointAxis1Selectionbox;         private const int jointAxis1SelectionboxID = 12;
        PropertyManagerPageLabel jointAxis2Label;                       private const int jointAxis2LabelID = 13;
        PropertyManagerPageSelectionbox jointAxis2Selectionbox;         private const int jointAxis2SelectionboxID = 14;
     
        PropertyManagerPageLabel jointEffortLimitLabel;                 private const int jointEffortLimitLabelID = 15;
        PropertyManagerPageTextbox jointEffortLimitTextbox;             private const int jointEffortLimitTextboxID = 16;
        PropertyManagerPageLabel jointVelocityLimitLabel;               private const int jointVelocityLimitLabelID = 17;
        PropertyManagerPageTextbox jointVelocityLimitTextbox;           private const int jointVelocityLimitTextboxID = 18;

        //Movment Limits sub group
        PropertyManagerPageGroup jointMovementLimitsGroup;              private const int jointMovementLimitsGroupID = 19;
        PropertyManagerPageGroup jointPhysicalPropertiesGroup;          private const int jointPhysicalPropertiesGroupID = 34;
        //textboxs
        PropertyManagerPageLabel jointLowerLimitLabel;                  private const int jointLowerLimitLabelID = 20;
        PropertyManagerPageTextbox jointLowerLimitTextbox;              private const int jointLowerLimitTextboxID = 21;
        PropertyManagerPageLabel jointUpperLimitLabel;                  private const int jointUpperLimitLabelID = 22;
        PropertyManagerPageTextbox jointUpperLimitTextbox;              private const int jointUpperLimitTextboxID = 23;
        //selectionboxes
        PropertyManagerPageLabel jointLowerLimitStopLabel;              private const int jointLowerLimitStopLabelID = 24;
        PropertyManagerPageSelectionbox jointLowerLimitStopSelectionbox;private const int jointLowerLimitStopSelectionboxID = 25;
        PropertyManagerPageLabel jointLinkLowerEdgeLabel;               private const int jointLinkLowerEdgeLabelID = 26;
        PropertyManagerPageSelectionbox jointLinkLowerEdgeSelectionbox; private const int jointLinkLowerEdgeSelectionboxID = 27;
        PropertyManagerPageLabel jointUpperLimitStopLabel;              private const int jointUpperLimitStopLabelID = 28;
        PropertyManagerPageSelectionbox jointUpperLimitStopSelectionbox;private const int jointUpperLimitStopSelectionboxID = 29;
        PropertyManagerPageLabel jointLinkUpperEdgeLabel;               private const int jointLinkUpperEdgeLabelID = 30;
        PropertyManagerPageSelectionbox jointLinkUpperEdgeSelectionbox; private const int jointLinkUpperEdgeSelectionboxID = 31;
        //checkbox
        PropertyManagerPageCheckbox jointManualLimitsCheckbox;          private const int jointManualLimitsCheckboxID = 35;
        PropertyManagerPageCheckbox flipAxisDirectionCheckbox;          private const int flipAxisDirectionCheckboxID = 36;


        //Control for selectin, adding, removing and editing links
        //private LinkSelector linkSelectorControl;
        //Active robot model
        private RobotModel robot;
        //Currently selected link
        private Link currentLink;
        //Allows access to the current assembly document
        private AssemblyDoc assemblyDoc;
        //Allows access to the current model document 
        private ModelDoc2 modelDoc;
        //Allows access to the current solidworks application instance
        private SldWorks swApp;
        //Display state active when page was opened
        private String savedDisplayState;
        //True if the LinkColors display state is active
        private bool linksColored;
        //Info message
        private String infoMessage = "A link is a rigid part of a robot made up of a set of components that do not move relative to eachother. " +
                                    "Use this pane to add links, remove links and select which components belong to which links. " +
                                    "When a link is selected in the box below, its components will be highleted in your enviorment's selection color, " +
                                    "and you may select or de-select components that are part of that link by clicking on them in the feature tree or model.";

        DragArrowManipulator[] DragArrows;
        Manipulator[] swManips;
        //id of the previously selected selectionbox
        int PrevSelectId=0;
        
        #endregion

        #region Constructor and Page Setup

        /// <summary>
        /// Creates a new link editor proptery manager page that allows the user to edit link components and properties.
        /// </summary>
        /// <param name="robot">Active robot model</param>
        /// <param name="document">Active assembly document</param>
        /// <param name="swApp">Solidworks application environment</param>
        public RobotEditorPMPage(RobotModel robot, AssemblyDoc document, SldWorks swApp)
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
            //Helper variables
            int options;
            short controlType, align;
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


            //Sets the page's message
            page.SetMessage3(infoMessage,
                                    (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
            linkTab = page.AddTab(linkTabID, "Links", "", 0);
            jointTab = page.AddTab(jointTabID, "Joint", "", 0);
            #region Select Group

            //Setup and create link selection group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            selectGroup = (PropertyManagerPageGroup)linkTab.AddGroupBox(selectGroupID, "Select Link", options);

            //Setup and create link colors checkbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            linkColorCheckbox = (PropertyManagerPageCheckbox)selectGroup.AddControl(linkColorCheckboxID, controlType, "Color components by link", align, options, "Check to change all components that are part of links to the color of the link");
            linkColorCheckbox.Checked = false;

            //Setup and create the link selector form handle
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_WindowFromHandle;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            linkSelectorHandle = (PropertyManagerPageWindowFromHandle)selectGroup.AddControl(linkSelectorHandleID, controlType, "Link Selector", align, options, "Select, add, remove, and edit links");
           // linkSelectorControl = new LinkSelector(robot);
            //linkSelectorControl.OnLinkSelectionChanged += this.OnLinkSelectionChanged;
            linkSelectorHandle.Height = 200;
            //linkSelectorHandle.SetWindowHandlex64(linkSelectorControl.Handle.ToInt64());
            //linkSelectorControl.OnEditLink += EditLink;

            #endregion

            #region Link Properties Group

            //Setup and create the link properties group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            linkPropertiesGroup = (PropertyManagerPageGroup)linkTab.AddGroupBox(linkPropertiesGroupID, "Link Properties", options);

            //Setup and create the label for the link name texbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            linkNameLabel = (PropertyManagerPageLabel)linkPropertiesGroup.AddControl(linkNameLabelID, controlType, "Link Name", align, options, "Change the name of this link");

            //Setup and create the link name textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled | 
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            linkNameTextbox = (PropertyManagerPageTextbox)linkPropertiesGroup.AddControl(linkNameTextboxID, controlType, "Link Name", align, options, "Enter the name for this link here");

            //Setup and create the label for the link components selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            linkComponentsLabel = (PropertyManagerPageLabel)linkPropertiesGroup.AddControl(linkComponentsLabelID, controlType, "Link Components", align, options, "Use this box to select components that are part of this link");

            //Setup and create the link components selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled | 
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            linkComponentsSelectionbox = (PropertyManagerPageSelectionbox)linkPropertiesGroup.AddControl(linkComponentsSelectionboxID, controlType, "Link Components", align, options, "Use this box to select components that are part of this link");
            linkComponentsSelectionbox.Height = 75;
            linkComponentsSelectionbox.Mark = 2;
            int[] filter = { (int)swSelectType_e.swSelCOMPONENTS };
            linkComponentsSelectionbox.SetSelectionFilters(filter);
            
            #endregion

            #region Joint Properties Group
            
            //Setup and create the joint properties group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            jointPropertiesGroup = (PropertyManagerPageGroup)jointTab.AddGroupBox(jointPropertiesGroupID, "Joint Properties", options);

            //Setup and create the label for the joint type combobox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointTypeLabel = (PropertyManagerPageLabel)jointPropertiesGroup.AddControl(linkNameLabelID, controlType, "Joint Type", align, options, "Change the name of this link");

            //Setup and create the label for the joint type combobox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Combobox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointTypeCombobox = (PropertyManagerPageCombobox)jointPropertiesGroup.AddControl(linkNameLabelID, controlType, "Joint Type", align, options, "Change the name of this link");
            jointTypeCombobox.AddItems(Joint.JointTypes);

            //Setup and create the label for the joint axis 1 selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointAxis1Label = (PropertyManagerPageLabel)jointPropertiesGroup.AddControl(linkNameLabelID, controlType, "Axis 1", align, options, "Select an axis for this joint");

            //Setup and create the joint axis 1 selectionbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointAxis1Selectionbox = (PropertyManagerPageSelectionbox)jointPropertiesGroup.AddControl(jointAxis1SelectionboxID, controlType, "Axis 1", align, options, "Use this box to select the axis of movment of this joint");
            jointAxis1Selectionbox.AllowSelectInMultipleBoxes = false;
            jointAxis1Selectionbox.SingleEntityOnly = true;
            jointAxis1Selectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointAxis1Selectionbox.Mark = 4;
            int[] filter2 = {(int) swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES };
            jointAxis1Selectionbox.SetSelectionFilters(filter2);

            //Setup and create the axis flip checkbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            flipAxisDirectionCheckbox = (PropertyManagerPageCheckbox)jointPropertiesGroup.AddControl(flipAxisDirectionCheckboxID, controlType, "flipAxisDirection", align, options, "Check to flip the direction of movement for this joint");
            flipAxisDirectionCheckbox.Checked = false;

            //Setup and create the joint Movement Limits properties group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            jointMovementLimitsGroup = (PropertyManagerPageGroup)jointTab.AddGroupBox(jointMovementLimitsGroupID, "Joint Movement Limits", options);

            //Setup and create joint lower edgelabel
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLinkLowerEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Lower Link Edge", align, options, "Select a lower edge for this joint");
            

            //setup and create joint lower edge selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLinkLowerEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(jointLinkLowerEdgeSelectionboxID, controlType, "Lower Link Edge", align, options, "Use this box to select the lower edge of the link");
            jointLinkLowerEdgeSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLinkLowerEdgeSelectionbox.SingleEntityOnly = true;
            jointLinkLowerEdgeSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointLinkLowerEdgeSelectionbox.Mark = 8;
            int[] filter3 = { (int)swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelFACES, (int)swSelectType_e.swSelVERTICES, (int)swSelectType_e.swSelDATUMPLANES, (int)swSelectType_e.swSelDATUMPOINTS };
            jointLinkLowerEdgeSelectionbox.SetSelectionFilters(filter3);
            

           

            //Setup and create joint lower limit stop label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLowerLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Lower Motion Limit", align, options, "Select a lower limit for this joint");

            //setup and create joint lower limit selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLowerLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(jointLowerLimitStopSelectionboxID, controlType, "LowerMotionLimit", align, options, "Use this box to select the lower limit of the link");
            jointLowerLimitStopSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLowerLimitStopSelectionbox.SingleEntityOnly = true;
            jointLowerLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointLowerLimitStopSelectionbox.Mark = 16;
            jointLowerLimitStopSelectionbox.SetSelectionFilters(filter3);
            
            //Setup and create joint upper edgelabel
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLinkUpperEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Upper Link Edge", align, options, "Select a Upper edge for this joint");

            //setup and create joint Upper edge selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLinkUpperEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(jointLinkUpperEdgeSelectionboxID, controlType, "Upper Link Edge", align, options, "Use this box to select the upper edge of the link");
            jointLinkUpperEdgeSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLinkUpperEdgeSelectionbox.SingleEntityOnly = true;
            jointLinkUpperEdgeSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointLinkUpperEdgeSelectionbox.Mark = 32;
            jointLinkUpperEdgeSelectionbox.SetSelectionFilters(filter3);

            //Setup and create joint Upper limit stop label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointUpperLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Upper Motion Limit", align, options, "Select an Upper limit for this joint");

            //Setup and create joint upper limit selectionbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointUpperLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(jointUpperLimitStopSelectionboxID, controlType, "Upper Motion Limit", align, options, "Use this box to select the upper limit of the joint");
            jointUpperLimitStopSelectionbox.AllowSelectInMultipleBoxes = false;
            jointUpperLimitStopSelectionbox.SingleEntityOnly = true;
            jointUpperLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointUpperLimitStopSelectionbox.Mark = 64;
            jointUpperLimitStopSelectionbox.SetSelectionFilters(filter3);

            //Setup and create manual limit checkbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointManualLimitsCheckbox = (PropertyManagerPageCheckbox)jointMovementLimitsGroup.AddControl(jointManualLimitsCheckboxID, controlType, "Set manual values for limits", align, options, "Check to manually set limit offsets from current position");
            jointManualLimitsCheckbox.Checked = false;

            //Setup and create joint lower limit label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLowerLimitLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Lower limit", align, options, "Select a lower limit for this joint");

            //Setup and create the joint lower limit Textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            jointLowerLimitTextbox = (PropertyManagerPageTextbox)jointMovementLimitsGroup.AddControl(jointLowerLimitTextboxID, controlType, "0.0", align, options, "Enter the lower limit of the joint");

            //Setup and create joint upper limit label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            jointUpperLimitLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(linkNameLabelID, controlType, "Upper limit", align, options, "Select an upper limit for this joint");

            //Setup and create the joint upper limit Textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            jointUpperLimitTextbox = (PropertyManagerPageTextbox)jointMovementLimitsGroup.AddControl(jointUpperLimitTextboxID, controlType, "0.0", align, options, "Enter the upper limit of the joint");

            //Create the joint physical properties group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            jointPhysicalPropertiesGroup = (PropertyManagerPageGroup)jointTab.AddGroupBox(jointPhysicalPropertiesGroupID, "Joint Physical Properties", options);

            //Setup and create joint effort limit label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointEffortLimitLabel = (PropertyManagerPageLabel)jointPhysicalPropertiesGroup.AddControl(linkNameLabelID, controlType, "Effort limit", align, options, "Select an effort limit for this joint");

            //Setup and create the joint effort limit textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointEffortLimitTextbox = (PropertyManagerPageTextbox)jointPhysicalPropertiesGroup.AddControl(jointEffortLimitTextboxID, controlType, "0.0", align, options, "Enter the effort limit of the joint");

            //Setup and create joint velocity limit label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointVelocityLimitLabel = (PropertyManagerPageLabel)jointPhysicalPropertiesGroup.AddControl(linkNameLabelID, controlType, "Velocity limit", align, options, "Select a velocity limit for this joint");

            //Setup and create the joint velocity limit selectionbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointVelocityLimitTextbox = (PropertyManagerPageTextbox)jointPhysicalPropertiesGroup.AddControl(jointVelocityLimitTextboxID, controlType, "0.0", align, options, "Enter the velocity limit of the joint");

            /*
            //Setup and create the label for the joint axis 2 selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            jointAxis2Label = (PropertyManagerPageLabel)jointPropertiesGroup.AddControl(linkNameLabelID, controlType, "Axis 2", leftAlign, options, "Select an axis for this joint");

            //Setup and create the joint axis 2 selection box
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            jointAxis2Selectionbox = (PropertyManagerPageSelectionbox)jointPropertiesGroup.AddControl(jointAxis2SelectionboxID, controlType, "Link Components", leftAlign, options, "Use this box to select components that are part of this link");
            jointAxis2Selectionbox.AllowSelectInMultipleBoxes = false;
            jointAxis2Selectionbox.SingleEntityOnly = true;
            jointAxis2Selectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem4);
            jointAxis2Selectionbox.Mark = 8;
            jointAxis2Selectionbox.SetSelectionFilters(filter2);
            */
            #endregion
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Colors the links in the model
        /// </summary>
        private void ColorLinks()
        {
            /*if (!linksColored)
            {
               Configuration config = modelDoc.ConfigurationManager.ActiveConfiguration;
               string[] displayStates = config.GetDisplayStates();
               savedDisplayState = displayStates[0];
               if (!displayStates.Contains("GazeboLinkColors"))
                   config.CreateDisplayState("GazeboLinkColors");
               config.ApplyDisplayState("GazeboLinkColors");
               robot.BaseLink.ColorLink();
               modelDoc.WindowRedraw();
               linksColored = true;
            }*/
        }

        /// <summary>
        /// reverts the links to their original colors
        /// </summary>
        private void RevertLinkColors()
        {
            if (linksColored)
            {
                Configuration config = modelDoc.ConfigurationManager.ActiveConfiguration;
                config.ApplyDisplayState(savedDisplayState);
                modelDoc.WindowRedraw();
                linksColored = false;
            }
        }

        /// <summary>
        /// Opens the robot manager window to allow for editing links
        /// </summary>
        /// <param name="select">The curently slected link</param>
        public void EditLink(ISelectable select)
        {
            if (RobotManagerLauncher != null)
                RobotManagerLauncher(select);
        }

        /// <summary>
        /// Show this property manager page
        /// </summary>
        public void Show()
        {
            //linkSelectorControl.SelectBase();
            page.Show();
        }

        /*
        public void showPage()
        {
            linkSelectorControl.linkSelectTree.ExpandAll();
         * linkSelectorControl.linkSelectTree.SelectedNode = linkSelectTree.Nodes[
         * page.Show();
         *             this.linkSelectTree.SelectedNode = linkSelectTree.Nodes[//how to select correct node?];
            
        }*/

        #endregion

        #region Link Selection

        /// <summary>
        /// Saves the currently selected link's information back into the robot model
        /// </summary>
        private void SaveCurrentLinkSelection()
        {
            Component2[] selectedComponents = new Component2[((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(2)];
            Component2 temp;
            for (int l = 1; l <= selectedComponents.Length; l++)
            {
                temp = (Component2)((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectsComponent4(l, 2);
                selectedComponents[l-1] = temp;
            }
            currentLink.LinkComponents = selectedComponents;
            /*if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(8) > 0)
                currentLink.ParentConnection.LowerLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 8);
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(16) > 0)
                currentLink.ParentConnection.LowerLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 16);
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(32) > 0)
                currentLink.ParentConnection.UpperLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 32);
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(64) > 0)
                currentLink.ParentConnection.UpperLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 64);
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(4) > 0)
                currentLink.ParentConnection.Axis1 = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);
            if (currentLink.ParentConnection != null)
            {
                currentLink.ParentConnection.CalcLimits(null);
            }*/
            swManips = null;
            DragArrows = null;
            GC.Collect();
        }

        /// <summary>
        /// Loads the currently selected link's information from the robot model
        /// </summary>
        private void LoadNewLinkSelection()
        {
            linkNameTextbox.Text = currentLink.Name;
            modelDoc.ClearSelection2(true);
            /*if (currentLink.ParentConnection != null)
            {
                ((IPropertyManagerPageControl)flipAxisDirectionCheckbox).Enabled = true;
                flipAxisDirectionCheckbox.Checked = currentLink.ParentConnection.Axis1IsFlipped;
                //load axis into selection box
                jointTypeCombobox.CurrentSelection = (short)currentLink.ParentConnection.Type;
                if (currentLink.ParentConnection.Axis1 != null)
                {
                    jointAxis1Selectionbox.SetSelectionFocus();
                    SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                    axisData.Mark = 4;
                    //((SelectionMgr)modelDoc.SelectionManager).AddSelectionListObject(currentLink.ParentConnection.Axis1, axisData);
                    //System.Diagnostics.Debug.WriteLine(currentLink.ParentConnection.Axis1==null);
                    ((IEntity)currentLink.ParentConnection.Axis1).Select4(true, axisData);
                    linkComponentsSelectionbox.SetSelectionFocus();
                }
                //load limit selection if needed
                //toggleLinkLimits(currentLink.parentConnection.type);
                if (currentLink.ParentConnection.Type == (int)Joint.JointType.Revolute || currentLink.ParentConnection.Type == (int)Joint.JointType.Prismatic)
                {
                    currentLink.ParentConnection.CalcLimits(null);
                    SelectLimits();
                    jointManualLimitsCheckbox.Checked = currentLink.ParentConnection.UseCustomMovementLimits;
                    ToggleManualLimits(jointManualLimitsCheckbox.Checked);
                    
                }
                
                

            }*/
            //else
            //jointPropertiesGroup.Visible = false;
            modelDoc.ClearSelection2(false);
            SelectData data = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
            data.Mark = 2;
            foreach (Component2 C in currentLink.LinkComponents)
                if (C != null)
                {
                    C.Select4(true, data, false);
                }
        }

        /// <summary>
        /// toggles which property managers should be visible for a specific joint
        /// </summary>
        /// <param name="linkType"> the type of the link</param>
        public void ToggleJointPropertyManagers(int linkType)
        {
            switch (linkType)
            {
                case (int)Joint.JointType.Prismatic:
                    ((IPropertyManagerPageControl)jointAxis1Label).Visible = true;
                    ((IPropertyManagerPageControl)jointLinkLowerEdgeLabel).Visible = true;
                    ((IPropertyManagerPageControl)jointLinkLowerEdgeSelectionbox).Visible = true;
                    ((IPropertyManagerPageControl)jointLowerLimitStopLabel).Visible = true;
                    ((IPropertyManagerPageControl)jointLowerLimitStopSelectionbox).Visible = true;
                    ((IPropertyManagerPageControl)jointLinkUpperEdgeLabel).Visible = true;
                    ((IPropertyManagerPageControl)jointLinkUpperEdgeSelectionbox).Visible = true;
                    ((IPropertyManagerPageControl)jointUpperLimitStopLabel).Visible = true;
                    ((IPropertyManagerPageControl)jointUpperLimitStopSelectionbox).Visible = true;
                    ((IPropertyManagerPageControl)jointManualLimitsCheckbox).Visible = true;
                    ((IPropertyManagerPageControl)jointAxis1Selectionbox).Visible = true;
                    ((IPropertyManagerPageControl)flipAxisDirectionCheckbox).Visible = true;
                    jointMovementLimitsGroup.Visible = true;
                    jointPhysicalPropertiesGroup.Visible = true;
                    //jointEffortLimitTextbox.Text = currentLink.ParentConnection.EffortLimit.ToString();
                    //jointVelocityLimitTextbox.Text = currentLink.ParentConnection.VelocityLimit.ToString();
                    break;
                case (int)Joint.JointType.Revolute:
                    jointMovementLimitsGroup.Visible = true;
                    ((IPropertyManagerPageControl)jointAxis1Label).Visible = true;
                    ((IPropertyManagerPageControl)jointAxis1Selectionbox).Visible = true;
                    ((IPropertyManagerPageControl)flipAxisDirectionCheckbox).Visible = true;
                    ((IPropertyManagerPageControl)jointLinkLowerEdgeLabel).Visible = false;
                    ((IPropertyManagerPageControl)jointLinkLowerEdgeSelectionbox).Visible = false;
                    ((IPropertyManagerPageControl)jointLowerLimitStopLabel).Visible = false;
                    ((IPropertyManagerPageControl)jointLowerLimitStopSelectionbox).Visible = false;
                    ((IPropertyManagerPageControl)jointLinkUpperEdgeLabel).Visible = false;
                    ((IPropertyManagerPageControl)jointLinkUpperEdgeSelectionbox).Visible = false;
                    ((IPropertyManagerPageControl)jointUpperLimitStopLabel).Visible = false;
                    ((IPropertyManagerPageControl)jointUpperLimitStopSelectionbox).Visible = false;
                    ((IPropertyManagerPageControl)jointManualLimitsCheckbox).Visible = false;
                    //currentLink.ParentConnection.UseCustomMovementLimits = true;
                    //jointPhysicalPropertiesGroup.Visible = true;
                    //jointEffortLimitTextbox.Text = currentLink.ParentConnection.EffortLimit.ToString();
                    //jointVelocityLimitTextbox.Text = currentLink.ParentConnection.VelocityLimit.ToString();
                    break;

                default:
                    break;
            }

                
            
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if the selected object would be a valid selection for the limit
        /// </summary>
        /// <param name="SelType"> The type of the Selected object, of form swSelectType_e </param>
        /// <param name="Selection"> The selected object </param>
        /// <param name="selBox"> The selection box that the object will be added to </param>
        /// <returns> returns true if the selected object is valid </returns>
        private bool IsValidLimitSelection(int SelType, object Selection, IPropertyManagerPageControl selBox)
        {
            /*if (currentLink.ParentConnection.Axis1 == null)//if there is no rotation axis then no selections can be valid
            {
                if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4) != null)
                {
                    currentLink.ParentConnection.Axis1 = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);
                }
                else
                {
                    selBox.ShowBubbleTooltip("Error: no joint axis", "Select an axis for this joint before selecting limits", "");
                    return false;
                }
                
            }
            IMathUtility mathUtil = ((IMathUtility)swApp.GetMathUtility());
            MathTransform componentTransform = null;
            if ((Component2)((IEntity)Selection).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
            {
                componentTransform = ((Component2)((IEntity)Selection).GetComponent()).Transform2;
            }
            double[] tempAxisNormalVector = { currentLink.ParentConnection.AxisX, currentLink.ParentConnection.AxisY, currentLink.ParentConnection.AxisZ };
            MathVector axisNormalVector = mathUtil.CreateVector(tempAxisNormalVector).Normalise();//creates a vector to represent the aXis of movment
            MathVector featureNormalVector = null;
            double errorVal = .00000001;
            double[] tempArray;
            //prismatic joint
            if (currentLink.ParentConnection.Type == (int)Joint.JointType.Prismatic)
            {

                switch (SelType)
                {
                    //Selection is a reference axis. If refAxis is perpendicular to the joint axis it is a valid selection
                    case (int)swSelectType_e.swSelDATUMAXES:

                        double[] points = ((IRefAxis)((IFeature)Selection).GetSpecificFeature2()).GetRefAxisParams();
                        tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                        if (componentTransform != null)//creates a vector between the 2 points on the reference axis and transforms to global space if nessacary
                        {
                            featureNormalVector = mathUtil.CreateVector(tempArray).MultiplyTransform(componentTransform).Normalise();
                        }
                        else
                        {
                            featureNormalVector = mathUtil.CreateVector(tempArray).Normalise();
                        }

                        if (Math.Abs(featureNormalVector.Dot(axisNormalVector)) < errorVal)//if the joint axis and the referance axis are perpindicular then the selection is valid
                        {
                            return true;
                        }
                        break;

                    //Selection is a reference point. All reference points are valid.
                    case (int)swSelectType_e.swSelDATUMPOINTS:
                        return true;
                        break;
                    //Selection is a reference plane. If the plane's normal vector is the same as the joint axis it is valid.
                    case (int)swSelectType_e.swSelDATUMPLANES:
                        tempArray = new double[] { 0, 0, 1 };//temp vector used to find normal vector of plane
                        MathVector planeNormalVector = mathUtil.CreateVector(tempArray);
                        if (componentTransform != null)//finds normal vecor of plane and transforms it if needed
                        {
                            featureNormalVector = planeNormalVector.MultiplyTransform(((IRefPlane)((IFeature)Selection).GetSpecificFeature2()).Transform).MultiplyTransform(componentTransform).Normalise();
                        }
                        else
                        {
                            featureNormalVector = planeNormalVector.MultiplyTransform(((IRefPlane)((IFeature)Selection).GetSpecificFeature2()).Transform).Normalise();
                        }
                        if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) < errorVal)//if the plane normal vector and the joint axis are parallel the selection is valid
                        {
                            return true;
                        }
                        break;

                    //Selection is an edge. If the edge is a line, the line must be perpindicular to the joint axis to be valid. 
                    //If the edge is not a line but is still planar the normal vector of the plane that the edge is in must be the same asw the axis of the joint 
                    case (int)swSelectType_e.swSelEDGES:
                        Object[] faces = ((IEdge)Selection).GetTwoAdjacentFaces2();
                        if (faces[0] == null || faces[1] == null)
                        {
                            break;
                        }
                        MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);//gets normal vectors for each bordering face
                        MathVector faceVector2 = mathUtil.CreateVector(((IFace2)faces[1]).Normal);

                        double[] edgeCurveParams =null;
                        if (!(((IEdge)Selection).GetCurve().LineParams is DBNull))
                        {
                            edgeCurveParams = ((IEdge)Selection).GetCurve().LineParams;
                        } 
                        //check if both adjacent faces are planar. If both are then the edge must be a line
                        if (edgeCurveParams!=null)
                        {
                            tempArray = new double[] { edgeCurveParams[3],edgeCurveParams[4],edgeCurveParams[5] };
                            featureNormalVector = mathUtil.CreateVector(tempArray).MultiplyTransform(componentTransform).Normalise();//creates a vector representing the linear edge
                            if (Math.Abs(featureNormalVector.Dot(axisNormalVector)) < errorVal)//if the edgevector and the axis are parallel it is valid
                            {
                                return true;
                            }
                            break;
                        }
                        else if (faceVector1.GetLength() > 0)
                        {
                            featureNormalVector = (faceVector1.MultiplyTransform(componentTransform)).Normalise();//converts the face vector to global space
                        }
                        else if (faceVector2.GetLength() > 0)
                        {
                            featureNormalVector = (faceVector2.MultiplyTransform(componentTransform)).Normalise();
                        }

                        if (featureNormalVector != null)//if at least one of the faces are planar and that face is normal to the plane, its valid
                        {
                            if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) < errorVal)
                            {
                                return true;
                            }
                        }
                        break;
                    //Selection is a face. The normal vector of the face must be the same as the joint axis to be valid
                    case (int)swSelectType_e.swSelFACES:
                        MathVector faceVector = mathUtil.CreateVector(((IFace2)Selection).Normal);
                        if (faceVector.GetLength() == 0)//makes sure face is valid
                        {
                            break;
                        }
                        featureNormalVector = (faceVector.MultiplyTransform(componentTransform)).Normalise(); //converts the face vector to the corect coordinate orientation

                        if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) < errorVal)
                        {
                            return true;
                        }
                        break;
                    //Selection is a vertice. All verticies are valid
                    case (int)swSelectType_e.swSelVERTICES:
                        return true;
                    default:
                        return false;
                }
                System.Diagnostics.Debug.WriteLine("AxisVector: " + String.Join(" ", (double[])axisNormalVector.ArrayData));
                System.Diagnostics.Debug.WriteLine("NormalVector: " + String.Join(" ", (double[])featureNormalVector.ArrayData));
                selBox.ShowBubbleTooltip("Error: Feature not normal to joint axis", "The selected feature must be normal to the axis of movement", "");
            }
            //if the joint is revolute
            else if (currentLink.ParentConnection.Type == (int)Joint.JointType.Revolute)
            {
                int errorCode = 0; //type of error that selection has. 0 is not coincident, 1 is colinear
                switch (SelType)
                {
                    //Selection is a reference axis. If refAxis is parallel to the joint axis or intersects it it is a valid selection
                    case (int)swSelectType_e.swSelDATUMAXES:

                        double[] points = ((IRefAxis)((IFeature)Selection).GetSpecificFeature2()).GetRefAxisParams();
                        tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                        if (componentTransform != null)
                        {
                            featureNormalVector = mathUtil.CreateVector(tempArray).MultiplyTransform(componentTransform).Normalise();
                        }
                        else
                        {
                            featureNormalVector = mathUtil.CreateVector(tempArray).Normalise();
                        }


                        MathVector pointVector;
                        if (componentTransform != null)
                        {
                            MathPoint arbPoint = mathUtil.CreatePoint(new double[] { points[0], points[1], points[2] }).MultiplyTransform(componentTransform);
                            pointVector = mathUtil.CreatePoint(new double[] { currentLink.ParentConnection.OriginX - arbPoint.ArrayData[0], currentLink.ParentConnection.OriginY - arbPoint.ArrayData[1], currentLink.ParentConnection.OriginZ - arbPoint.ArrayData[2] }).ConvertToVector().Normalise();
                        }
                        else
                        {
                            pointVector = mathUtil.CreatePoint(new double[] { currentLink.ParentConnection.OriginX - points[0], currentLink.ParentConnection.OriginY - points[1], currentLink.ParentConnection.OriginZ - points[2] }).ConvertToVector().Normalise();
                        }
                        //check if vectors are parallel
                        if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) < errorVal)
                        {
                            if (Math.Abs(pointVector.Cross(axisNormalVector).GetLength()) > errorVal)
                            {
                                return true;
                            }
                            else
                            {
                                errorCode = 1;
                                break;
                            }

                        }
                        //check if vectors intersect
                        if (Math.Abs(pointVector.Cross(axisNormalVector).GetLength()) < errorVal ||
                            (Math.Abs(featureNormalVector.Cross(axisNormalVector).Normalise().Cross(pointVector.Cross(axisNormalVector).Normalise()).GetLength()) < errorVal))
                        {
                            return true;
                        }
                        errorCode = 0;
                        break;
                    //Selection is a reference point. Reference points are valid if they do not lie on a the axis line.
                    case (int)swSelectType_e.swSelDATUMPOINTS:
                        double[] point;
                        if (componentTransform != null)
                        {
                            point = mathUtil.CreatePoint(((IRefPoint)((IFeature)Selection).GetSpecificFeature2()).GetRefPoint().ArrayData).MultiplyTransform(componentTransform).ArrayData;
                        }
                        else
                        {
                            point = ((IRefPoint)((IFeature)Selection).GetSpecificFeature2()).GetRefPoint().ArrayData;
                        }
                        featureNormalVector = mathUtil.CreateVector(new double[] { point[0] - currentLink.ParentConnection.OriginX, point[1] - currentLink.ParentConnection.OriginY, point[2] - currentLink.ParentConnection.OriginZ }).Normalise();
                        if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) > errorVal)
                        {
                            return true;
                        }
                        errorCode = 1;
                        break;
                    //Selection is a reference plane. If the plane intersects the joint axis and its normal vector is perpindicular it is valid.
                    case (int)swSelectType_e.swSelDATUMPLANES:
                        errorCode = 0;
                        tempArray = new double[] { 0, 0, 1 };
                        MathVector planeNormalVector = mathUtil.CreateVector(tempArray);
                        MathTransform planeTransform = ((IRefPlane)((IFeature)Selection).GetSpecificFeature2()).Transform;
                        if (componentTransform != null)
                        {
                            planeTransform = planeTransform.Multiply(componentTransform);
                        }
                        //System.Diagnostics.Debug.WriteLine("planeTransform: " + string.Join(" ", (double[])planeTransform.ArrayData));
                        featureNormalVector = planeNormalVector.MultiplyTransform(planeTransform).Normalise();
                        tempArray = new double[] { planeTransform.ArrayData[9] - currentLink.ParentConnection.OriginX, planeTransform.ArrayData[10] - currentLink.ParentConnection.OriginY, planeTransform.ArrayData[11] - currentLink.ParentConnection.OriginZ };
                        MathVector originVector = mathUtil.CreateVector(tempArray);
                        if (Math.Abs(featureNormalVector.Dot(axisNormalVector)) < errorVal && Math.Abs(featureNormalVector.Dot(originVector)) < errorVal)
                        {
                            return true;
                        }
                        break;
                    //Selection is an edge. If the edge is a line, the line must be parallel to the joint axis or intersect it to be valid. 
                    //If the edge is not a line but is still planar the normal vector of the plane that the edge is in must intersect the axis of the joint and its normalvector must be perpindicular to the axis 
                    case (int)swSelectType_e.swSelEDGES:
                        Object[] faces = ((IEdge)Selection).GetTwoAdjacentFaces2();
                        if (faces[0] == null || faces[1] == null)
                        {
                            break;
                        }
                        MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);
                        MathVector faceVector2 = mathUtil.CreateVector(((IFace2)faces[1]).Normal);
                        MathVector originVect;
                        //check if both adjacent faces are planar. If both are then the edge must be a line
                        if (faceVector1.GetLength() > 0 && faceVector2.GetLength() > 0)
                        {
                            double[] startPoint = mathUtil.CreateVector(((IEdge)Selection).GetStartVertex().getPoint()).MultiplyTransform(componentTransform).ArrayData;
                            double[] endPoint = mathUtil.CreateVector(((IEdge)Selection).GetEndVertex().getPoint()).MultiplyTransform(componentTransform).ArrayData;
                            tempArray = new double[] { endPoint[0] - startPoint[0], endPoint[1] - startPoint[1], endPoint[2] - startPoint[2] };
                            featureNormalVector = mathUtil.CreateVector(tempArray).Normalise;
                            MathVector pointVect = mathUtil.CreatePoint(new double[] { currentLink.ParentConnection.OriginX - startPoint[0], currentLink.ParentConnection.OriginY - startPoint[1], currentLink.ParentConnection.OriginZ - startPoint[2] }).ConvertToVector();

                            //check if edges are parallel
                            if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) < errorVal)
                            {
                                if (Math.Abs(pointVect.Cross(axisNormalVector).GetLength()) > errorVal)
                                {
                                    return true;
                                }
                                else
                                {
                                    errorCode = 1;
                                    break;
                                }
                            }

                            //check if vectors intersect
                            if (Math.Abs(pointVect.Cross(axisNormalVector).GetLength()) < errorVal ||
                                (Math.Abs(featureNormalVector.Cross(axisNormalVector).Normalise().Cross(pointVect.Cross(axisNormalVector).Normalise()).GetLength()) < errorVal))
                            {
                                return true;
                            }
                            errorCode = 0;
                            break;
                        }
                        else if (faceVector1.GetLength() > 0)
                        {
                            featureNormalVector = (faceVector1.MultiplyTransform(componentTransform)).Normalise();
                        }
                        else if (faceVector2.GetLength() > 0)
                        {
                            featureNormalVector = (faceVector2.MultiplyTransform(componentTransform)).Normalise();
                        }
                        tempArray = ((IEdge)Selection).GetClosestPointOn(currentLink.ParentConnection.OriginX, currentLink.ParentConnection.OriginY, currentLink.ParentConnection.OriginZ);
                        double[] facePoint = mathUtil.CreatePoint(tempArray).MultiplyTransform(componentTransform).ArrayData;
                        tempArray = new double[] { facePoint[0] - currentLink.ParentConnection.OriginX, facePoint[1] - currentLink.ParentConnection.OriginY, facePoint[2] - currentLink.ParentConnection.OriginZ };
                        originVect = mathUtil.CreateVector(tempArray);
                        if (featureNormalVector != null)
                        {
                            if (Math.Abs(featureNormalVector.Dot(axisNormalVector)) < errorVal && Math.Abs(featureNormalVector.Dot(originVect)) < errorVal)
                            {
                                return true;
                            }
                        }
                        errorCode = 0;
                        break;
                    //Selection is a face. The face must intersect the joint axis and its normal vector must be perpindicular to be valid
                    case (int)swSelectType_e.swSelFACES:
                        errorCode = 0;
                        MathVector faceVector = mathUtil.CreateVector(((IFace2)Selection).Normal);
                        featureNormalVector = (faceVector.MultiplyTransform(componentTransform)).Normalise(); //converts the face vector to the corect coordinate orientation
                        tempArray = ((IFace2)Selection).GetClosestPointOn(currentLink.ParentConnection.OriginX, currentLink.ParentConnection.OriginY, currentLink.ParentConnection.OriginZ);
                        double[] facePnt = mathUtil.CreatePoint(tempArray).MultiplyTransform(componentTransform).ArrayData;
                        tempArray = new double[] { facePnt[0] - currentLink.ParentConnection.OriginX, facePnt[1] - currentLink.ParentConnection.OriginY, facePnt[2] - currentLink.ParentConnection.OriginZ };
                        MathVector origVect = mathUtil.CreateVector(tempArray);
                        if (faceVector.GetLength() > errorVal && (Math.Abs(featureNormalVector.Dot(axisNormalVector)) < errorVal && Math.Abs(featureNormalVector.Dot(origVect)) < errorVal))
                        {
                            return true;
                        }
                        break;
                    //Selection is a vertice. Verticies are valid if they do not lie on a the axis line
                    case (int)swSelectType_e.swSelVERTICES:
                        errorCode = 1;
                        MathPoint vertPoint = mathUtil.CreatePoint(((IVertex)Selection).GetPoint()).MultiplyTransform(componentTransform); ;
                        featureNormalVector = mathUtil.CreateVector(new double[] { vertPoint.ArrayData[0] - currentLink.ParentConnection.OriginX, vertPoint.ArrayData[1] - currentLink.ParentConnection.OriginY, vertPoint.ArrayData[2] - currentLink.ParentConnection.OriginZ }).Normalise();
                        if (Math.Abs(featureNormalVector.Cross(axisNormalVector).GetLength()) > errorVal)
                        {
                            return true;
                        }
                        break;
                    default:
                        break;
                }
                switch (errorCode)
                {
                    case 0: //feature not coincident to axis
                        selBox.ShowBubbleTooltip("Error: Feature not coincident to joint axis", "The selected feature must be coincident to the axis of rotation", "");
                        break;
                    case 1: //feature is colinear to axis
                        selBox.ShowBubbleTooltip("Error: Feature is colinear to joint axis", "The selected feature must not be colinear to the axis of movement", "");
                        break;
                }

            }*/
            return false;
        }

        /// <summary>
        /// Selects all limiting objects
        /// </summary>
        private void SelectLimits()
        {

            /*jointLinkLowerEdgeSelectionbox.SetSelectionFocus();
            modelDoc.ClearSelection2(false);
            if (currentLink.ParentConnection.LowerLimitEdge != null)
            {
                SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                axisData.Mark = 8;
                ((IEntity)currentLink.ParentConnection.LowerLimitEdge).Select4(false, axisData);
            }

            jointLowerLimitStopSelectionbox.SetSelectionFocus();
            modelDoc.ClearSelection2(false);
            if (currentLink.ParentConnection.LowerLimitStop != null)
            {
                SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                axisData.Mark = 16;
                ((IEntity)currentLink.ParentConnection.LowerLimitStop).Select4(false, axisData);
            }

            jointLinkUpperEdgeSelectionbox.SetSelectionFocus();
            modelDoc.ClearSelection2(false);
            if (currentLink.ParentConnection.UpperLimitEdge != null)
            {
                SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                axisData.Mark = 32;
                ((IEntity)currentLink.ParentConnection.UpperLimitEdge).Select4(false, axisData);
            }

            jointUpperLimitStopSelectionbox.SetSelectionFocus();
            modelDoc.ClearSelection2(false);
            if (currentLink.ParentConnection.UpperLimitStop != null)
            {
                SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                axisData.Mark = 64;
                ((IEntity)currentLink.ParentConnection.UpperLimitStop).Select4(false, axisData);
            }

            jointLowerLimitTextbox.Text = currentLink.ParentConnection.LowerLimit.ToString();
            jointUpperLimitTextbox.Text = currentLink.ParentConnection.UpperLimit.ToString();
            linkComponentsSelectionbox.SetSelectionFocus();*/
        }

        /// <summary>
        /// Toggles whether the manual limits will be used
        /// </summary>
        /// <param name="useManual">true if manual limits should be used</param>
        public void ToggleManualLimits(bool useManual)
        {
            //currentLink.ParentConnection.UseCustomMovementLimits = useManual;
            //enables textboxes
            ((IPropertyManagerPageControl)jointLowerLimitLabel).Enabled = useManual;
            ((IPropertyManagerPageControl)jointLowerLimitTextbox).Enabled = useManual;
            ((IPropertyManagerPageControl)jointUpperLimitLabel).Enabled = useManual;
            ((IPropertyManagerPageControl)jointUpperLimitTextbox).Enabled = useManual;
            //disables selection boxes
            ((IPropertyManagerPageControl)jointLowerLimitStopLabel).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointLowerLimitStopSelectionbox).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointLinkLowerEdgeLabel).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointLinkLowerEdgeSelectionbox).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointUpperLimitStopLabel).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointUpperLimitStopSelectionbox).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointLinkUpperEdgeLabel).Enabled = !useManual;
            ((IPropertyManagerPageControl)jointLinkUpperEdgeSelectionbox).Enabled = !useManual;
            //enables flipping axis
            ((IPropertyManagerPageControl)flipAxisDirectionCheckbox).Enabled = useManual;
        }

        /// <summary>
        /// Draws preview arrows for the joint. limits should be recalculated before calling this method
        /// </summary>
        public void DrawJointPreview()
        {
            /*Joint currentJoint = currentLink.ParentConnection;
            if (currentJoint == null)
            {
                return;
            }
            //currentJoint.calcLimits(null);
            ModelViewManager swModViewMgr = modelDoc.ModelViewManager;
            IMathUtility mathUtil = ((IMathUtility)swApp.GetMathUtility());           
            MathVector axisVector =((IMathUtility)swApp.GetMathUtility()).CreateVector(new double[] { currentJoint.AxisX, currentJoint.AxisY, currentJoint.AxisZ });


            DragArrows = null;
            swManips = null;
            GC.Collect();
            switch (currentJoint.Type)
            {
                case (int)Joint.JointType.Revolute://revolute
                    
                case (int)Joint.JointType.Continuous://continuous
                    if (currentJoint.Axis1 == null)
                    {
                        return;
                    }
                    DragArrows = new DragArrowManipulator[2];//2 arrows, the axis and the rotation vector
                    swManips = new Manipulator[2];

                    swManips[0] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, this);
                    DragArrowManipulator axisArrow = (DragArrowManipulator)swManips[0].GetSpecificManipulator();
                    DragArrows[0] = axisArrow;

                    swManips[1] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, this);
                    DragArrowManipulator rotationArrow = (DragArrowManipulator)swManips[1].GetSpecificManipulator();
                    DragArrows[1] = rotationArrow;

                    axisArrow.ShowRuler = false;
                    axisArrow.FixedLength = true;
                    axisArrow.ShowOppositeDirection = false;
                    axisArrow.AllowFlip = false;
                    axisArrow.Length = .025;
                    axisArrow.Direction = axisVector;
                    axisArrow.Origin = mathUtil.CreatePoint(new double[] { currentJoint.OriginX, currentJoint.OriginY, currentJoint.OriginZ });
                    swManips[0].Show(modelDoc);
                    axisArrow.Update();

                    
                    //the rotation axis is found by assuming that the axis vector is equivlent to the x-axis of a rotated coordinate system.
                    //the vector of the rotation arrow is then defined as the z' axis, and its origin is found along the y' axis. 
                    //To find z' and y' thetaY and thetaZ are determined assuming that thetaY is the first rotation about the yaxis and then thetaZ is the second rotation about the z' axis

                    double thetaY; //the rotaions to create the axis vector
                    double thetaZ;
                    if (currentJoint.AxisX > 0)
                    {
                        thetaY = Math.Atan(currentJoint.AxisZ / currentJoint.AxisX);
                        thetaZ = Math.Atan(currentJoint.AxisY/(Math.Sqrt(Math.Pow(currentJoint.AxisX,2)+Math.Pow(currentJoint.AxisZ,2))));//thetaZ = Atan(y/sqrt(x^2+z^2))
                    }
                    else if (currentJoint.AxisX < 0)
                    {
                        thetaY = Math.Atan(currentJoint.AxisZ / -currentJoint.AxisX)+Math.PI;
                        thetaZ = Math.Atan(currentJoint.AxisY / (Math.Sqrt(Math.Pow(currentJoint.AxisX, 2) + Math.Pow(currentJoint.AxisZ, 2))));//thetaZ = Atan(y/sqrt(x^2+z^2))
                    }
                    else
                    {
                        if (currentJoint.AxisZ == 0)
                        {
                            thetaY = 0;
                            if (currentJoint.AxisY < 0)
                                thetaZ = -Math.PI / 2;
                            else
                                thetaZ = Math.PI/2;
                            
                        }
                        else
                        {
                            if (currentJoint.AxisZ > 0)
                            {
                                thetaY = Math.PI / 2;
                                thetaZ = Math.Atan(currentJoint.AxisY / (Math.Sqrt(Math.Pow(currentJoint.AxisX, 2) + Math.Pow(currentJoint.AxisZ, 2))));
                            }
                            else
                            {
                                thetaY = -Math.PI / 2;
                                thetaZ = Math.Atan(currentJoint.AxisY / (Math.Sqrt(Math.Pow(currentJoint.AxisX, 2) + Math.Pow(currentJoint.AxisZ, 2))));
                            }
                            
                            
                        }
                    }

                    //System.Diagnostics.Debug.WriteLine("tY=" + thetaY + " tZ=" + thetaZ);
                    //System.Diagnostics.Debug.WriteLine("axis=" + currentJoint.axisX + " " + currentJoint.axisY + " " + currentJoint.axisZ);
                    MathVector rotationArrowVector;
                    
                   rotationArrowVector = mathUtil.CreateVector(new double[] { -Math.Sin(thetaY), 0, Math.Cos(thetaY) });//vector representing the z axis after the coordinates have been transformed to match the axis
                    
                    
                    
                    MathVector rotationArrowOffset = mathUtil.CreateVector(new double[] { -Math.Sin(thetaZ) * Math.Cos(thetaY), Math.Cos(thetaZ), -Math.Sin(thetaZ) * Math.Sin(thetaY) });//vector representing the y axis after the coordinate system has been rotated
                    MathPoint rotationArrowOrigin = axisArrow.Origin.AddVector(rotationArrowOffset.Scale(.075));
                    rotationArrow.Direction = rotationArrowVector;
                    rotationArrow.Origin = rotationArrowOrigin;
                    rotationArrow.ShowOppositeDirection = false;
                    rotationArrow.ShowRuler = false;
                    rotationArrow.FixedLength = true;
                    rotationArrow.AllowFlip = false;
                    rotationArrow.Length = .025;
                    swManips[1].Show(modelDoc);
                    axisArrow.Update();
                    break;
                
                case (int)Joint.JointType.Prismatic://prismatic
                    if (currentJoint.Axis1 == null)
                    {
                        return;
                    }
                    if (currentJoint.UseCustomMovementLimits)
                    {
                        DragArrows = new DragArrowManipulator[1];//1 arrow for direction
                        swManips = new Manipulator[1];
                        swManips[0] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, this);
                        DragArrowManipulator movementArrow = (DragArrowManipulator)swManips[0].GetSpecificManipulator();
                        DragArrows[0] = movementArrow;
                        movementArrow.ShowRuler = false;
                        movementArrow.FixedLength = true;
                        movementArrow.ShowOppositeDirection = false;
                        movementArrow.AllowFlip = false;
                        movementArrow.Length = .025;
                        movementArrow.Direction = axisVector;
                        movementArrow.Origin = mathUtil.CreatePoint(new double[] { currentJoint.OriginX, currentJoint.OriginY, currentJoint.OriginZ });
                        swManips[0].Show(modelDoc);
                        movementArrow.Update();
                    }
                    else
                    {
                        DragArrows = new DragArrowManipulator[2];//2 arrows, the upper and lower movement arrows
                        swManips = new Manipulator[2];
                        if (currentJoint.LowerLimit < -.00000001)
                        {
                            swManips[0] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, this);
                            DragArrowManipulator lowerArrow = (DragArrowManipulator)swManips[0].GetSpecificManipulator();
                            DragArrows[0] = lowerArrow;

                            lowerArrow.ShowRuler = false;
                            lowerArrow.FixedLength = true;
                            lowerArrow.ShowOppositeDirection = false;
                            lowerArrow.AllowFlip = false;
                            lowerArrow.Length = -currentJoint.LowerLimit * .9;
                            lowerArrow.Direction = currentJoint.limitVectors[1].Subtract(currentJoint.limitVectors[0]).Normalise(); ;
                            lowerArrow.Origin = mathUtil.CreatePoint(new double[] { currentJoint.OriginX, currentJoint.OriginY, currentJoint.OriginZ }).AddVector(currentJoint.limitVectors[0]);
                            swManips[0].Show(modelDoc);
                            lowerArrow.Update();
                        }
                        if (currentJoint.UpperLimit > .00000001)
                        {
                            swManips[1] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swDragArrowManipulator, this);
                            DragArrowManipulator upperArrow = (DragArrowManipulator)swManips[1].GetSpecificManipulator();
                            DragArrows[1] = upperArrow;

                            upperArrow.ShowRuler = false;
                            upperArrow.FixedLength = true;
                            upperArrow.ShowOppositeDirection = false;
                            upperArrow.AllowFlip = false;
                            upperArrow.Length = currentJoint.UpperLimit * .9;
                            upperArrow.Direction = currentJoint.limitVectors[2].Subtract(currentJoint.limitVectors[3]).Normalise(); ;
                            upperArrow.Origin = mathUtil.CreatePoint(new double[] { currentJoint.OriginX, currentJoint.OriginY, currentJoint.OriginZ }).AddVector(currentJoint.limitVectors[3]);
                            swManips[1].Show(modelDoc);
                            upperArrow.Update();
                        }
                    }
                    
                    break;
            }*/
        }
        #endregion

        #region Handler Implementation

        /// <summary>
        /// update currentLink when new link selected 
        /// </summary>
        /// <param name="link"></param>
        public void OnLinkSelectionChanged(Link link)
        {
            if (currentLink != null)
                SaveCurrentLinkSelection();
            currentLink = link;
            LoadNewLinkSelection();
        }

        /// <summary>
        /// update link name or joint limits when a textbox changes
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Text"></param>
        public void OnTextboxChanged(int Id, string Text)
        {
            if (Id == linkNameTextboxID)
            {
                currentLink.Name = Text;
                //linkSelectorControl.Refresh();
            }
            else if (Id == jointUpperLimitTextboxID)
            {
                double num;
                if(Double.TryParse(Text,out num))
                {
                    //currentLink.ParentConnection.UpperLimit = num;
                }
            }
            else if (Id == jointLowerLimitTextboxID)
            {
                double num;
                if (Double.TryParse(Text, out num))
                {
                    //currentLink.ParentConnection.LowerLimit = num;
                }
            }
            else if (Id == jointEffortLimitTextboxID)
            {
                double num;
                if (Double.TryParse(Text, out num))
                {
                    //currentLink.ParentConnection.EffortLimit = num;
                }
            }
            else if (Id == jointVelocityLimitTextboxID)
            {
                double num;
                if (Double.TryParse(Text, out num))
                {
                    //currentLink.ParentConnection.VelocityLimit = num;
                }
            }
        }

        /// <summary>
        /// A listbox on this property manager page has had it's selection changed
        /// </summary>
        /// <param name="Id">User specified ID of the listbox in focus</param>
        /// <param name="Item">Index of the item selected</param>
        public void OnListboxSelectionChanged(int Id, int Item)
        {

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
            //linkSelectorControl.close();
            modelDoc.ClearSelection2(true);
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
            //Link components selectionbox is in focus
            if (Id == linkComponentsSelectionboxID)
            {
                if (SelType == (int)swSelectType_e.swSelCOMPONENTS)
                    return true;
            }
            else if (Id == jointAxis1SelectionboxID || Id == jointAxis2SelectionboxID)
            {
                if (SelType == (int)swSelectType_e.swSelDATUMAXES)
                {
                    //currentLink.ParentConnection.Axis1 = Selection;
                    //currentLink.ParentConnection.CalcLimits(null);
                    DrawJointPreview();
                    return true;
                }
                else if (SelType == (int)swSelectType_e.swSelEDGES)
                {
                    if (!(((IEdge)Selection).GetCurve().LineParams is DBNull))
                    {
                        //currentLink.ParentConnection.Axis1 = Selection;
                        //currentLink.ParentConnection.CalcLimits(null);
                        DrawJointPreview();
                        return true;
                    }
                }
                return false;
            }
            else if (Id == jointLinkUpperEdgeSelectionboxID || Id == jointLinkLowerEdgeSelectionboxID || Id == jointLowerLimitStopSelectionboxID || Id == jointUpperLimitStopSelectionboxID)
            {
                IPropertyManagerPageControl selBox=null;
                object[] limits = new object[4];
                switch (Id)
                {
                    case jointLinkUpperEdgeSelectionboxID:
                        selBox = (IPropertyManagerPageControl)jointLinkUpperEdgeSelectionbox;
                        limits[3]=Selection;
                        break;
                    case jointLinkLowerEdgeSelectionboxID:
                        selBox = (IPropertyManagerPageControl)jointLinkLowerEdgeSelectionbox;
                        limits[1]=Selection;
                        break;
                    case jointUpperLimitStopSelectionboxID:
                        selBox = (IPropertyManagerPageControl)jointUpperLimitStopSelectionbox;
                        limits[2]=Selection;
                        break;
                    case jointLowerLimitStopSelectionboxID:
                        selBox = (IPropertyManagerPageControl)jointLowerLimitStopSelectionbox;
                        limits[0]=Selection;
                        break;
                }
                if (IsValidLimitSelection(SelType, Selection, selBox))
                {
                    //currentLink.ParentConnection.CalcLimits(limits);
                    DrawJointPreview();
                    return true;
                }
            

            }
            return false;
        }

        /// <summary>
        /// display link or joint otions when click on corresponding tab
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public bool OnTabClicked(int Id)
        {
            if (Id == linkTabID)
            {
                linkTab.Activate();
                
                linkComponentsSelectionbox.SetSelectionFocus();
                page.SetMessage3(infoMessage,
                                    (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
                return true;
            }
            else if (Id == jointTabID)
            {
                /*if (currentLink.ParentConnection == null)
                {
                    page.SetMessage3("This Link has no parent joint, select a link with a parent joint to edit that joint's properties",
                                    (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
                    jointPropertiesGroup.Visible = false;
                    jointMovementLimitsGroup.Visible = false;
                    jointPhysicalPropertiesGroup.Visible = false;
                }
                else
                {
                    jointTab.Activate();

                    page.SetMessage3("",
                                    (int)swPropertyManagerPageMessageVisibility.swNoMessageBox,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
                    ToggleJointPropertyManagers(currentLink.ParentConnection.Type);
                    jointAxis1Selectionbox.SetSelectionFocus();
                    PrevSelectId = jointAxis1SelectionboxID;
                    //drawJointPreview();

                }*/
                return true;
            }
            return false;
        }


        /// <summary>
        /// updates type of joint and corresponding options when the dropdown box changes
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Item"></param>
        public void OnComboboxSelectionChanged(int Id, int Item)
        {
            SaveCurrentLinkSelection();
            /*if (currentLink.ParentConnection.Type != Item)
            {
               
                currentLink.ParentConnection.LowerLimitEdge = null;
                currentLink.ParentConnection.LowerLimitStop = null;
                currentLink.ParentConnection.UpperLimitEdge = null;
                currentLink.ParentConnection.UpperLimitStop = null;
            }
            currentLink.ParentConnection.Type = Item;
            ToggleJointPropertyManagers(currentLink.ParentConnection.Type);*/
            LoadNewLinkSelection();
        }

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
            else if (Id == jointManualLimitsCheckboxID)
            {
                //update joint 
                //currentLink.ParentConnection.UseCustomMovementLimits = Checked;
                
                ToggleManualLimits(Checked);
                DrawJointPreview();
            }
            else if (Id == flipAxisDirectionCheckboxID)
            {
                //currentLink.ParentConnection.Axis1IsFlipped = flipAxisDirectionCheckbox.Checked;
                DrawJointPreview();
            }
        }

       
            

        public void OnSelectionboxFocusChanged(int Id)
        {

            lock (page)
            {
                switch (PrevSelectId)
                {
                    case linkComponentsSelectionboxID:
                        Component2[] selectedComponents = new Component2[((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(2)];
                        Component2 temp;
                        for (int l = 1; l <= selectedComponents.Length; l++)
                        {
                            temp = (Component2)((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectsComponent4(l, 2);
                            selectedComponents[l - 1] = temp;
                        }
                        currentLink.LinkComponents = selectedComponents;
                        break;

                    case jointAxis1SelectionboxID:
                        if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(4) > 0)
                            //currentLink.ParentConnection.Axis1 = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);

                        DrawJointPreview();
                        SelectLimits();
                        jointLinkLowerEdgeSelectionbox.SetSelectionFocus();
                        break;

                    case jointUpperLimitStopSelectionboxID:
                        //if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(64) > 0)
                            //currentLink.ParentConnection.UpperLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 64);
                        break;

                    case jointLinkUpperEdgeSelectionboxID:
                        //if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(32) > 0)
                            //currentLink.ParentConnection.UpperLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 32);
                        break;

                    case jointLowerLimitStopSelectionboxID:
                        //if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(16) > 0)
                            //currentLink.ParentConnection.LowerLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 16);
                        break;

                    case jointLinkLowerEdgeSelectionboxID:
                        //if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(8) > 0)
                            //currentLink.ParentConnection.LowerLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 8);
                        break;
                }
                PrevSelectId = Id;
            }
        }
        
        #region Unused Hanlders

        public void OnButtonPress(int Id)
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

        

        public void OnSelectionboxListChanged(int Id, int Count)
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

        #region dragHandlers
        public bool OnDelete(object pManipulator)
        {
            return true;
        }

        public void OnDirectionFlipped(object pManipulator)
        {
            //System.Diagnostics.Debug.WriteLine("flipped: " + swDrag.AllowFlip);
        }

        public bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double Value)
        {
            return true;
        }

        public void OnEndDrag(object pManipulator, int handleIndex)
        {
        }

        public void OnEndNoDrag(object pManipulator, int handleIndex)
        {
        }

        public bool OnHandleLmbSelected(object pManipulator)
        {
            return true;
        }

        public void OnHandleRmbSelected(object pManipulator, int handleIndex)
        {
        }

        public void OnHandleSelected(object pManipulator, int handleIndex)
        {
        }

        public void OnItemSetFocus(object pManipulator, int handleIndex)
        {
        }

        public bool OnStringValueChanged(object pManipulator, int handleIndex, ref string Value)
        {
            return true;
        }

        public void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
        }
        #endregion
    }
}
