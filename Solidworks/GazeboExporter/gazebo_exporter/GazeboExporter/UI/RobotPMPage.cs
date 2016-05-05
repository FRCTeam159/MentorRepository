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

    /// <summary>
    /// This creates a SW proprty manager page to select robot frame of reference objects
    /// </summary>
    public class RobotPMPage : PropertyManagerPage2Handler9, SwManipulatorHandler2
    {
        #region Fields and Properties
        

        public RobotManagerCallback RestoreManager;

        //Object to interface with this Solidworks property manager page
        PropertyManagerPage2 page;

        //Property manager page controls (configured in the setupPage() method) and unique IDs for them

        PropertyManagerPageGroup robotPropertiesGroup;                      private const int robotPropertiesGroupID = 0;

        PropertyManagerPageLabel originPointLabel;                          private const int originPointLabelID = 1;
        PropertyManagerPageSelectionbox originPointSelectionbox;            private const int originPointSelectionboxID = 2;
        PropertyManagerPageLabel basePlaneLabel;                            private const int basePlaneLabelID = 3;
        PropertyManagerPageSelectionbox basePlaneSelectionbox;              private const int basePlaneSelectionboxID = 4;
        PropertyManagerPageBitmapButton flipPlaneDirectionButton;           private const int flipPlaneDirectionButtonID = 5;
        PropertyManagerPageLabel directionAxisLabel;                        private const int directionAxisLabelID = 6;
        PropertyManagerPageSelectionbox directionAxisSelectionbox;          private const int directionAxisSelectionboxID = 7;
        PropertyManagerPageBitmapButton flipAxisDirectionButton;            private const int flipAxisDirectionButtonID = 8;


        //Active robot model
        private RobotModel robot;
        //Allows access to the current assembly document
        private AssemblyDoc assemblyDoc;
        //Allows access to the current model document 
        private ModelDoc2 modelDoc;
        //Allows access to the current solidworks application instance
        private SldWorks swApp;

        private int PrevSelectId = originPointSelectionboxID;
        
        DragArrowManipulator[] DragArrows;
        TriadManipulator triadManip;
        Manipulator[] swManips;

        IMathUtility mathUtil;

        const double errorVal = .00000001;
        
        #endregion

        #region Constructor and Setup
        /// <summary>
        /// Creates a new robot editor proptery manager page that allows the user to select robot frame-of-reference objects.
        /// </summary>
        /// <param name="robot">Active robot model</param>
        /// <param name="document">Active assembly document</param>
        /// <param name="swApp">Solidworks application environment</param>
        public RobotPMPage(RobotModel robot, AssemblyDoc document, SldWorks swApp)
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
            mathUtil = ((IMathUtility)swApp.GetMathUtility());

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
            page = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Edit Robot Frame of Reference",
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
            short buttonCT = (int)swPropertyManagerPageControlType_e.swControlType_CheckableBitmapButton; 
            // align
            short leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            short buttonAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            // options
            int groupboxOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible; // for group box
            int controlOptions = (int)swAddControlOptions_e.swControlOptions_Enabled |
                                 (int)swAddControlOptions_e.swControlOptions_Visible; // for controls
            // object filters
            int[] linefilter = new int[] { (int)swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES };
            int[] pointfilter = new int[] { (int)swSelectType_e.swSelDATUMPOINTS, (int)swSelectType_e.swSelVERTICES };
            int[] planefilter = new int[] { (int)swSelectType_e.swSelDATUMPLANES, (int)swSelectType_e.swSelFACES };

            
            //Setup and create the robot properties group
            robotPropertiesGroup = (PropertyManagerPageGroup)page.AddGroupBox(
                robotPropertiesGroupID, "Robot Frame of Reference Properties", groupboxOptions);
            //Setup and create the label for the origin point selection box
            originPointLabel = (PropertyManagerPageLabel)robotPropertiesGroup.AddControl(
                originPointLabelID, labelCT, "Robot origin point", leftAlign, controlOptions, "Select the origin point for this robot");
            //Setup and create the origin point selectionbox
            originPointSelectionbox = (PropertyManagerPageSelectionbox)robotPropertiesGroup.AddControl(
                originPointSelectionboxID, selectionboxCT, "Robot origin point", leftAlign, controlOptions, "Use this box to select a point");
            originPointSelectionbox.AllowSelectInMultipleBoxes = false;
            originPointSelectionbox.SingleEntityOnly = true;
            originPointSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem1);
            originPointSelectionbox.Mark = 2;
            originPointSelectionbox.SetSelectionFilters(pointfilter);
            //Setup and create the label for the base plane selection box
            basePlaneLabel = (PropertyManagerPageLabel)robotPropertiesGroup.AddControl(
                basePlaneLabelID, labelCT, "Base plane", leftAlign, controlOptions, "Select the base plane for this robot");
            //create baseplane flip checkbox
            flipPlaneDirectionButton = (PropertyManagerPageBitmapButton)robotPropertiesGroup.AddControl(
                flipPlaneDirectionButtonID, buttonCT, "Flip plane normal", buttonAlign, controlOptions, "Check to flip the direction of the normal of the base plane");
            flipPlaneDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            flipPlaneDirectionButton.Checked = false;
            //Setup and create the base plane selectionbox
            basePlaneSelectionbox = (PropertyManagerPageSelectionbox)robotPropertiesGroup.AddControl(
                basePlaneSelectionboxID, selectionboxCT, "Base plane", leftAlign, controlOptions, "Use this box to select a plane");
            basePlaneSelectionbox.AllowSelectInMultipleBoxes = false;
            basePlaneSelectionbox.SingleEntityOnly = true;
            basePlaneSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            basePlaneSelectionbox.Mark = 4;
            basePlaneSelectionbox.SetSelectionFilters(planefilter);

            ((PropertyManagerPageControl)basePlaneSelectionbox).Top = 75;
            ((PropertyManagerPageControl)flipPlaneDirectionButton).Top = (short)(((PropertyManagerPageControl)basePlaneSelectionbox).Top+3);
            
            //Setup and create the label for the direction axis selection box
            directionAxisLabel = (PropertyManagerPageLabel)robotPropertiesGroup.AddControl(
                directionAxisLabelID, labelCT, "Direction axis", leftAlign, controlOptions, "Select the axis along which the robot faces");
            //Setup and create the axis flip checkbox
            flipAxisDirectionButton = (PropertyManagerPageBitmapButton)robotPropertiesGroup.AddControl(
                flipAxisDirectionButtonID, buttonCT, "Flip axis direction", buttonAlign, controlOptions, "Check to flip the direction of the axis to the direction that the robot is facing");
            flipAxisDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            flipAxisDirectionButton.Checked = false;
            //Setup and create the direction axis selection box
            directionAxisSelectionbox = (PropertyManagerPageSelectionbox)robotPropertiesGroup.AddControl(
                directionAxisSelectionboxID, selectionboxCT, "Direction axis", leftAlign, controlOptions, "Use this box to select an axis");
            directionAxisSelectionbox.AllowSelectInMultipleBoxes = false;
            directionAxisSelectionbox.SingleEntityOnly = true;
            directionAxisSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            directionAxisSelectionbox.Mark = 8;
            directionAxisSelectionbox.SetSelectionFilters(linefilter);

            ((PropertyManagerPageControl)directionAxisSelectionbox).Top = 125;
            ((PropertyManagerPageControl)flipAxisDirectionButton).Top = (short)(((PropertyManagerPageControl)directionAxisSelectionbox).Top + 3);
            

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
        /// update robot when selected 
        /// </summary>
        /// <param name="robot"></param>
        public void OnRobotSelectionChanged(RobotModel robot_)
        {
            if (robot != null) 
                SaveCurrentSelection();
            robot = robot_;
            LoadSelections();
        }
        #endregion

        #region Object selection
        /// <summary>
        /// Saves the currently selected robot's information back into the robot model
        /// </summary>
        private void SaveCurrentSelection()
        {
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(2) > 0)
            {
                robot.OriginPt = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 2);
            }
            if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(4) > 0)
            {
                robot.BasePlane = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);

                if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(8) > 0)
                    robot.Direction = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 8);
                robot.AxisIsFlipped = flipAxisDirectionButton.Checked;
            }

            robot.ConfigName = modelDoc.ConfigurationManager.ActiveConfiguration.Name;

            swManips = null;
            DragArrows = null;
            GC.Collect();
        }

        /// <summary>
        /// Loads the currently selected robot's information from the robot model
        /// </summary>
        private void LoadSelections()
        {
            modelDoc.ClearSelection2(true);

            //Load robot origin point into selection box
            if (robot.OriginPt != null)
            {
                originPointSelectionbox.SetSelectionFocus();
                SelectData ptData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                ptData.Mark = 2;
                ((IEntity)robot.OriginPt).Select4(true, ptData);
            }
            //Load base plane into selection box
            if (robot.BasePlane != null)
            {
                basePlaneSelectionbox.SetSelectionFocus();
                SelectData planeData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                planeData.Mark = 4;
                ((IEntity)robot.BasePlane).Select4(true, planeData);

                //Load direction axis into selection box
                if (robot.Direction != null)
                {
                    directionAxisSelectionbox.SetSelectionFocus();
                    SelectData axisData = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                    axisData.Mark = 8;
                    ((IEntity)robot.Direction).Select4(true, axisData);
                }
            }
            flipAxisDirectionButton.Checked = robot.AxisIsFlipped;
            flipPlaneDirectionButton.Checked = robot.BaseIsFlipped;

        }


        #endregion

        #region Helper Methods
        /// <summary>
        /// checks that the axis is valid. 
        /// if a axis is defined, it must be parallel to new/current base plane
        /// </summary>
        /// <param name="SelType"> The type of the Selected object, of form swSelectType_e </param>
        /// <param name="Selection"> The selected object </param>
        /// <param name="selBox"> The selection box that the object will be added to </param>
        /// <returns> returns true if the axis is valid </returns>
        private bool IsValidAxisSelection(int SelType, object Selection, IPropertyManagerPageControl selBox)
        {
            if (selBox == basePlaneSelectionbox)
            {
                if (robot.Direction != null)
                {
                    double[] directionVectorArray = { robot.AxisX, robot.AxisY, robot.AxisZ };
                    MathVector directionVector = mathUtil.CreateVector(directionVectorArray).Normalise();//creates a vector to represent the aXis of direction

                    MathVector normalVector = CalcNormalVector(robot.BasePlane);

                    if (!VectorCalcs.IsPerpendicular(directionVector, normalVector))
                    {
                        
                        directionAxisSelectionbox.SetSelectionFocus();
                        modelDoc.ClearSelection2(true);
                        ((IEntity)robot.OriginPt).SelectByMark(true, originPointSelectionbox.Mark);
                        robot.Direction = null;
                        
                        ((IPropertyManagerPageControl)directionAxisSelectionbox).ShowBubbleTooltip(
                            "Error: axis not parallel to base plane", "Select a direction axis that is parallel to the base plane for this robot", "");
                        return false;
                    }
                }
                return true;
            }


            if (selBox == directionAxisSelectionbox)
            {
                // if baseplane == null, error in setting axis
                if (robot.BasePlane == null)
                {
                    selBox.ShowBubbleTooltip("Error: no base plane", "Select the base plane for this robot before selecting the directional axis", "");
                    return false; //cannot select direction axis before base plane
                }
                // else if axis not perpendicular to the normal vector of the plane, error in picking axis


                MathVector normalVector = CalcNormalVector(robot.BasePlane); // get normal vector of plane as MathVector
                MathVector directionVector = null; // get selected axis as MathVector

                double[] tempArray; //used to store the three values to make the MathVector

                MathTransform componentTransform = null;
                if ((Component2)((IEntity)Selection).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
                {
                    componentTransform = ((Component2)((IEntity)Selection).GetComponent()).Transform2;
                }

                //Selection is a reference axis. If refAxis is perpendicular to the normal of the plane, then the axis in parallel to the plane (valid)
                if (SelType == (int)swSelectType_e.swSelDATUMAXES)
                {
                    double[] points = ((IRefAxis)((IFeature)Selection).GetSpecificFeature2()).GetRefAxisParams();
                    tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                    directionVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector between the 2 points on the reference axis and transforms to global space if nessacary
                    if (VectorCalcs.IsPerpendicular(directionVector, normalVector))
                        return true;//if axis and the normal to the plane are perpendicular then axis is parallel to plane
                    else
                    {
                        selBox.ShowBubbleTooltip("Error: axis not parallel to base plane", "Select a direction axis that is parallel to the base plane for this robot", "");
                        return false; //cannot select direction axis before base plane
                    }
                }
                //Selection is an edge. If the edge is a line, the line must be perpindicular to the joint axis to be valid. 
                //If the edge is not a line but is still planar the normal vector of the plane that the edge is in it must be the same as the axis of the joint 
                if (SelType == (int)swSelectType_e.swSelEDGES)
                {
                    Object[] faces = ((IEdge)Selection).GetTwoAdjacentFaces2();
                    if (!(faces[0] == null || faces[1] == null))
                    {   //get normal vectors for each bordering face
                        MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);
                        MathVector faceVector2 = mathUtil.CreateVector(((IFace2)faces[1]).Normal);

                        double[] edgeCurveParams = null;
                        if (!(((IEdge)Selection).GetCurve().LineParams is DBNull))
                        {
                            edgeCurveParams = ((IEdge)Selection).GetCurve().LineParams;
                        }
                        //check if both adjacent faces are planar. If both are then the edge must be a line
                        if (edgeCurveParams != null)
                        {
                            tempArray = new double[] { edgeCurveParams[3], edgeCurveParams[4], edgeCurveParams[5] };
                            directionVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector representing the linear edge/the axis
                            if (VectorCalcs.IsPerpendicular(directionVector, normalVector))
                                return true;//if axis and the normal to the plane are perpendicular then axis is parallel to plane
                            else
                            {
                                selBox.ShowBubbleTooltip("Error: axis not parallel to base plane", "Select a direction axis that is parallel to the base plane for this robot", "");
                                return false; //cannot select direction axis before base plane
                            }
                        }

                        //if at least one of the faces are planar and that face is normal to the plane, its valid
                        MathVector faceNormalVector = null;
                        if (faceVector1.GetLength() > 0)
                        {
                            faceNormalVector = (faceVector1.MultiplyTransform(componentTransform)).Normalise();//converts the face vector to global space
                        }
                        else if (faceVector2.GetLength() > 0)
                        {
                            faceNormalVector = (faceVector2.MultiplyTransform(componentTransform)).Normalise();
                        }
                        if (faceNormalVector != null)
                        {
                            if (VectorCalcs.IsParallel(faceNormalVector, normalVector))
                                return true;
                            else
                            {
                                selBox.ShowBubbleTooltip("Error: axis not parallel to base plane", "Select a direction axis that is parallel to the base plane for this robot", "");
                                return false; //cannot select direction axis before base plane
                            }

                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Draws preview arrows for the coordinate system. Axes should be calculated before hand.
        /// </summary>
        public void DrawOriginPreview()
        {
            triadManip = null;
            swManips = null;
            GC.Collect();
            if (robot == null || robot.OriginPt == null || robot.Direction == null || robot.BasePlane == null) 
            {
                return;
            }

            triadManip = null;
            swManips = null;
            GC.Collect();

            ModelViewManager swModViewMgr = modelDoc.ModelViewManager;
            MathPoint robotOrigin = mathUtil.CreatePoint(new double[] { robot.OriginX, robot.OriginY, robot.OriginZ });

            swManips = new Manipulator[1];
            MathVector[] axisVectors = new MathVector[3];
            axisVectors[0] = mathUtil.CreateVector(new double[] { robot.AxisX, robot.AxisY, robot.AxisZ });
            axisVectors[1] = CalcNormalVector(robot.BasePlane);                //calculate vertical axis as normal of plane
            axisVectors[2] = axisVectors[1].Cross(axisVectors[0]);  //calculate horizontal axis as cross product between plane normal and direction axis.

            swManips[0] = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, this);
            triadManip = (TriadManipulator)swManips[0].GetSpecificManipulator();
            triadManip.DoNotShow = (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYRING | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZRING
                                        | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXRING | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYPlane |
                                        (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZPlane | (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXPlane;
            triadManip.XAxis = axisVectors[0];
            triadManip.YAxis = axisVectors[2];
            triadManip.ZAxis = axisVectors[1];
            triadManip.Origin = robotOrigin;
            triadManip.UpdatePosition();
            swManips[0].Show(modelDoc);
        }

        /// <summary>
        /// short helper function to set some visual properties of the DragArrowManipulator
        /// called in DrawJointPreview
        /// </summary>
        /// <param name="arrow"></param>
        private void setArrowLooks(DragArrowManipulator arrow)
        {
            arrow.ShowRuler = false;
            arrow.FixedLength = true;
            arrow.ShowOppositeDirection = false;
            arrow.AllowFlip = false;
            arrow.Length = .025;
        }

        /// <summary>
        /// given an object as a plane, calculates a MathVector representing the normal vector 
        /// </summary>
        /// <param name="plane">the plane to find the normal vector of</param>
        /// <returns>a MathVector representing the normal vector to the given plane</returns>
        private MathVector CalcNormalVector(object plane)
        {
            MathTransform componentTransform = null;

            //stores the component transform if there is one. this will be used to convert the vectors to the global coordinate system
            if ((Component2)((IEntity)plane).GetComponent() != null)
            {
                componentTransform = ((Component2)((IEntity)robot.BasePlane).GetComponent()).Transform2;
            }

            //if base plane is a face
            if (plane is IFace2)
            {
                MathVector tempVect = mathUtil.CreateVector(((IFace2)plane).Normal);
                tempVect = tempVect.MultiplyTransform(componentTransform);
                if(robot.BaseIsFlipped)
                    tempVect = tempVect.Scale(-1);
                return tempVect;
            }
            //if limit is a reference plane
            else
            {
                object tempObject = ((IFeature)plane).GetSpecificFeature2();
                if (tempObject is IRefPlane)
                {
                    MathTransform planeTransform = ((IRefPlane)tempObject).Transform;//gets the transform of the plane
                    if (componentTransform != null)//if the plane is part of a sub component, transform it again into global space
                    {
                        planeTransform = planeTransform.Multiply(componentTransform);
                    }
                    //return normalVector = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9] - robot.OriginX, planeTransform.ArrayData[10] - robot.OriginY, planeTransform.ArrayData[11] - robot.OriginZ });//create vector between joint origin and point on plane
                    MathVector tempVect = mathUtil.CreateVector(new double[] { planeTransform.ArrayData[9], planeTransform.ArrayData[10], planeTransform.ArrayData[11] });;
                    if (robot.BaseIsFlipped)
                        tempVect = tempVect.Scale(-1);
                    return tempVect;
                }
            }

            return null; //parameter is invalid if neither a face nor a plane
        }

        private MathVector VectorProduct(object axis1, object axis2)
        {
            return null;
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
            if (PrevSelectId != null)
                OnSelectionboxFocusChanged(PrevSelectId);
            swManips = null;
            triadManip = null;
            GC.Collect();

            if (robot != null)
                SaveCurrentSelection();
            modelDoc.ClearSelection2(true);
            if (Reason != (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_ParentClosed && RestoreManager != null)
                RestoreManager(robot);
        }

        /// <summary>
        /// Called to verify whether to submit a newly selected item into a selection box on this property manager page
        /// and sets objects to the selection if valid
        /// </summary>
        /// <param name="Id">Id of selection box in focus</param>
        /// <param name="Selection">Item selected</param>
        /// <param name="SelType">Type of item selected</param>
        /// <param name="ItemText">Text to enter into selectionbox for this item</param>
        /// <returns>true if valid selection</returns>
        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            switch(Id)
            {
                
                case originPointSelectionboxID:
                    if (SelType == (int)swSelectType_e.swSelDATUMPOINTS || SelType == (int)swSelectType_e.swSelVERTICES)
                    {
                        robot.OriginPt = Selection;
                        DrawOriginPreview();
                        return true; //all points are valid
                    }
                    break;
                case basePlaneSelectionboxID:
                    if (SelType == (int)swSelectType_e.swSelDATUMPLANES ||
                        SelType == (int)swSelectType_e.swSelFACES)
                    {
                        robot.BasePlane = Selection;
                        if (robot.Direction != null)
                            IsValidAxisSelection(SelType, Selection, (IPropertyManagerPageControl)basePlaneSelectionbox);
                        DrawOriginPreview();
                        return true;
                    }
                    break;
                case directionAxisSelectionboxID:
                    if (SelType == (int)swSelectType_e.swSelDATUMAXES ||
                        (SelType == (int)swSelectType_e.swSelEDGES && !(((IEdge)Selection).GetCurve().LineParams is DBNull)))
                    {
                        if (robot.BasePlane != null && IsValidAxisSelection(SelType, Selection, (IPropertyManagerPageControl)directionAxisSelectionbox))
                        {
                            robot.Direction = Selection;
                            DrawOriginPreview();
                            return true;
                        }
                    }
                    break;
                default:
                    break;
            }
            return false;
        }


        public void OnButtonPress(int Id)
        {
            if (Id == flipAxisDirectionButtonID)
            {
                //flipAxisDirectionButton.Checked = !flipAxisDirectionButton.Checked;
                robot.AxisIsFlipped = flipAxisDirectionButton.Checked;
            }
            if (Id == flipPlaneDirectionButtonID)
            {
                //flipPlaneDirectionButton.Checked = true;
                robot.BaseIsFlipped = flipPlaneDirectionButton.Checked;
            }

            DrawOriginPreview();
        }

        /// <summary>
        /// when use selection box: updates values and draws previews
        /// </summary>
        /// <param name="Id"></param>
        public void OnSelectionboxFocusChanged(int Id)
        {
            lock (page)
            {
                switch (PrevSelectId)
                {
                    case originPointSelectionboxID:
                        if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(2) > 0)
                            robot.OriginPt = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 2);
                        break;

                    case basePlaneSelectionboxID:
                        if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(4) > 0)
                            robot.BasePlane = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);
                        break;
                    case directionAxisSelectionboxID:
                        if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(8) > 0)
                            robot.Direction = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 8);
                        break;
                    default:
                        break;

                }

                DrawOriginPreview();
                PrevSelectId = Id;
            }
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            if (Count == 0)
            {
                switch (Id)
                {
                    case originPointSelectionboxID:
                        robot.OriginPt = null;
                        break;
                    case basePlaneSelectionboxID:
                        robot.BasePlane = null;
                        break;
                    case directionAxisSelectionboxID:
                        robot.Direction = null;
                        break;
                }
                DrawOriginPreview();
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

        public void OnCheckboxCheck(int Id, bool Checked)
        {

        }

        #endregion

        #region dragHandlers (unused)
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

        #endregion


    
        
    }
}
