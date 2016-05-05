using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using GazeboExporter.GazeboException;
using GazeboExporter.Robot;


namespace GazeboExporter.UI
{
    /// <summary>
    /// PropertyManager page for directional attachments
    /// </summary>
    public class AttachmentPMPage: PropertyManagerPage2Handler9 //,SwManipulatorHandler2
    {
        public RobotManagerCallback RestoreManager;
        //The current attachment being edited
        DirectionalAttachment currentAttachment;
        //the axis of viewing for the attachment
        double[] axis={1,0,0};
        //the axis defined by the feature
        double[] featureAxis = { 1, 0, 0 };
        //the preview body
        Body2[] previewBodies = new Body2[1];
        //Modeler used to model the preview
        Modeler swModeler;
        //the origin point
        MathPoint originPt = null;
        //Manipulator swManip;
        //TriadManipulator rotManipulator;

        SldWorks swApp;
        ModelDoc2 modelDoc;

        PropertyManagerPage2 page;

        //PMPage controls
        PropertyManagerPageGroup selectionGroup;                private const int selectionGroupID = 1;
        PropertyManagerPageLabel originLabel;                   private const int originLabelID = 2;
        PropertyManagerPageSelectionbox originSelectionbox;     private const int originSelectionboxID = 3;
        PropertyManagerPageLabel axisLabel;                     private const int axisLabelID = 4;
        PropertyManagerPageSelectionbox axisSelectionbox;       private const int axisSelectionboxID = 5;
        PropertyManagerPageBitmapButton flipAxisButton;         private const int flipAxisButtonID = 6;
        PropertyManagerPageLabel rollLabel;                     private const int rollLabelID = 7;
        PropertyManagerPageTextbox rollTextbox;                 private const int rollTextboxID = 8;
        PropertyManagerPageLabel pitchLabel;                    private const int pitchLabelID = 9;
        PropertyManagerPageTextbox pitchTextbox;                private const int pitchTextboxID = 10;
        PropertyManagerPageLabel yawLabel;                      private const int yawLabelID = 11;
        PropertyManagerPageTextbox yawTextbox;                  private const int yawTextboxID = 12;
        PropertyManagerPageCheckbox manualAngleCheckbox;        private const int manualAngleCheckboxID = 13;
        PropertyManagerPageSlider FOVSlider;                    private const int FOVSliderID = 14;
        PropertyManagerPageLabel FOVLabel;                      private const int FOVLabelID = 15;

        
        

        /*bool firstRotPoint;
        MathPoint startPoint;
        MathPoint endPoint;
        */

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="att">The attachment to be used</param>
        /// <param name="swApp">The Solidworks App</param>
        public AttachmentPMPage(DirectionalAttachment att, SldWorks swApp)
        {
            currentAttachment = att;
            this.swApp = swApp;
            modelDoc = swApp.ActiveDoc;
            swModeler = swApp.GetModeler();
            SetupPage();
            
            
        }

        /// <summary>
        /// sets up all the controls on the page
        /// </summary>
        public void SetupPage()
        {
            //Helper variables
            int options;
            short controlType, align;
            int errors = 0;
            System.Diagnostics.Debug.WriteLine(this is PropertyManagerPage2Handler9);
            //Create a property manager page and throw exceptions if an error occurs
            page = (PropertyManagerPage2)swApp.CreatePropertyManagerPage("Attachment Selection",
                                                                                   (int)swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton,
                                                                                   this, ref errors);
            if (errors == -1) //Creation failed
                throw new InternalSolidworksException("SldWorks::CreatePropertyManagerPage", "Failed to create property manager page");
            else if (errors == -2) //No open document
                throw new ProgramErrorException("Tried to open a property manager page when no document was open");
            else if (errors == 1) //Invlaid hanlder
                throw new ProgramErrorException("Tried to pass in and invalid page hanlder when creating a property manager page");


            //Sets the page's message
            page.SetMessage3("Select the origin and direction for this attachment.",
                                    (int)swPropertyManagerPageMessageVisibility.swMessageBoxVisible,
                                    (int)swPropertyManagerPageMessageExpanded.swMessageBoxExpand,
                                    "Message");
            //create the selection group
            options = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                      (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible;
            selectionGroup = (PropertyManagerPageGroup)page.AddGroupBox(selectionGroupID, "Attachment Selections", options);

            //create the Origin selection Label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            originLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(originLabelID, controlType, "Attachment Origin", align, options, "Select the orgin point of the Attachment");

            //create Orgin selectionbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            originSelectionbox = (PropertyManagerPageSelectionbox)selectionGroup.AddControl(originSelectionboxID, controlType, "Attachment Origin", align, options, "Select the origin point of the Attachment");
            originSelectionbox.AllowSelectInMultipleBoxes = false;
            originSelectionbox.SingleEntityOnly = true;
            originSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            originSelectionbox.Mark = 2;
            int[] filter = { (int)swSelectType_e.swSelVERTICES, (int)swSelectType_e.swSelDATUMPOINTS};
            originSelectionbox.SetSelectionFilters(filter);

            //create the axis selection Label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            axisLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(axisLabelID, controlType, "Attachment Axis", align, options, "Select the viewing axis of the Attachment");

            //create axis selectionbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                    (int)swAddControlOptions_e.swControlOptions_Visible;
            axisSelectionbox = (PropertyManagerPageSelectionbox)selectionGroup.AddControl(axisSelectionboxID, controlType, "Attachment Axis", align, options, "Select the viewing axis of the Attachment");
            axisSelectionbox.AllowSelectInMultipleBoxes = false;
            axisSelectionbox.SingleEntityOnly = true;
            axisSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            axisSelectionbox.Mark = 4;
            filter = new int[] { (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelDATUMAXES};
            axisSelectionbox.SetSelectionFilters(filter);

            //Setup and create axis flip checkbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_CheckableBitmapButton;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            flipAxisButton = (PropertyManagerPageBitmapButton)selectionGroup.AddControl(flipAxisButtonID, controlType, "Flip direction axis", align, options, "Check to flip the direction of viewing");
            flipAxisButton.Checked = currentAttachment.FlipAxis;
            flipAxisButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);

            ((PropertyManagerPageControl)flipAxisButton).Top = 75;
            ((PropertyManagerPageControl)axisSelectionbox).Top = 75;

            //create FOV label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            FOVLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(FOVLabelID, controlType, "Field of View", align, options, "Select Field of View");

            //create FOV slider
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Slider;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            FOVSlider = (PropertyManagerPageSlider)selectionGroup.AddControl(FOVSliderID, controlType, "FOV Slider", align, options, "Slide to adjust FOV");
            if (currentAttachment.FOV > 0)
            {
                FOVSlider.SetRange(5,175); //corresponds to radius of 5 to 175 meters
                FOVSlider.TickFrequency = 5;
                FOVSlider.Position = (int)currentAttachment.FOV;
            }
            else
            {
                FOVSlider.SetRange(5, 100); //after conversion corresponds to radius of 5 to 100 centimeters
                FOVSlider.TickFrequency = 5;
                FOVSlider.Position = (int)(-currentAttachment.FOV * 100);
            }
            FOVSlider.Style = (int)swPropMgrPageSliderStyle_e.swPropMgrPageSliderStyle_AutoTicks | (int)swPropMgrPageSliderStyle_e.swPropMgrPageSliderStyle_BottomLeftTicks | (int)swPropMgrPageSliderStyle_e.swPropMgrPageSliderStyle_NotifyWhileTracking;

            //create manual angle checkbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            manualAngleCheckbox = (PropertyManagerPageCheckbox)selectionGroup.AddControl(manualAngleCheckboxID, controlType, "Manual Rotations", align, options, "Check to manually define angle rotations");
            manualAngleCheckbox.Checked = currentAttachment.UseManualAngles;

            //create the roll text Label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            rollLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(rollLabelID, controlType, "Roll", align, options, "Rotation about the X axis");

            //create roll textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            rollTextbox = (PropertyManagerPageTextbox)selectionGroup.AddControl(rollTextboxID, controlType, "0.0", align, options, "Enter the rotation about the X axis");

            //create the pitch text Label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            pitchLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(pitchLabelID, controlType, "Pitch", align, options, "Rotation about the Y axis");

            //create pitch textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            pitchTextbox = (PropertyManagerPageTextbox)selectionGroup.AddControl(pitchTextboxID, controlType, "0.0", align, options, "Enter the rotation about the Y axis");

            //create the yaw text Label
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Enabled |
                       (int)swAddControlOptions_e.swControlOptions_Visible;
            yawLabel = (PropertyManagerPageLabel)selectionGroup.AddControl(yawLabelID, controlType, "Yaw", align, options, "Rotation about the Z axis");

            //create roll textbox
            controlType = (int)swPropertyManagerPageControlType_e.swControlType_Textbox;
            align = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            options = (int)swAddControlOptions_e.swControlOptions_Visible;
            yawTextbox = (PropertyManagerPageTextbox)selectionGroup.AddControl(yawTextboxID, controlType, "0.0", align, options, "Enter the rotation about the Z axis");

           

            ToggleManualAngles(currentAttachment.UseManualAngles);

            page.Show();

            //ModelViewManager swModViewMgr = modelDoc.ModelViewManager;
            //IMathUtility mathUtil = ((IMathUtility)swApp.GetMathUtility());
            //swManip = swModViewMgr.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, this);
            //rotManipulator = (TriadManipulator)swManip.GetSpecificManipulator();

            //rotManipulator.DoNotShow =  (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYPlane 
                //+ (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZPlane + (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXPlane;// 127;//hides all exept for rotations


            SelectOrigin();
            rollTextbox.Text = (currentAttachment.RotRoll * 180 / Math.PI).ToString();
            pitchTextbox.Text = (currentAttachment.RotPitch * 180 / Math.PI).ToString();
            yawTextbox.Text = (currentAttachment.RotYaw * 180 / Math.PI).ToString();

           

        }

        /// <summary>
        /// Selects the axis and origin if one has already been selected
        /// </summary>
        public void SelectOrigin()
        {
            modelDoc.ClearSelection2(true);
            
            if (currentAttachment.OriginPoint != null)
            {
                originSelectionbox.SetSelectionFocus();
                SelectData data = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                data.Mark = 2;
                ((IEntity)currentAttachment.OriginPoint).Select4(false, data);
                MathTransform originTransform = null;

                if (((IEntity)currentAttachment.OriginPoint).GetComponent() != null)
                {
                    originTransform = ((IEntity)currentAttachment.OriginPoint).GetComponent().Transform2;
                }


                if (currentAttachment.OriginPoint is IVertex)
                {
                    originPt = swApp.GetMathUtility().CreatePoint(((IVertex)currentAttachment.OriginPoint).GetPoint()).MultiplyTransform(originTransform);
                }
                else if (currentAttachment.OriginPoint is IFeature)
                {
                    if (originTransform != null)
                    {
                        originPt = ((IFeature)currentAttachment.OriginPoint).GetSpecificFeature2().GetRefPoint().MultiplyTransform(originTransform);
                    }
                    else
                    {
                        originPt = ((IFeature)currentAttachment.OriginPoint).GetSpecificFeature2().GetRefPoint();
                    }
                }

                /*rotManipulator.Origin = ((IMathUtility)swApp.GetMathUtility()).CreatePoint(new double[]{currentAttachment.OriginX, currentAttachment.OriginY, currentAttachment.OriginZ});
                //convert current rotations to transforms and apply them to the triad
                IMathUtility mathUtil = (IMathUtility)swApp.GetMathUtility();
                MathTransform transR = mathUtil.CreateTransform(new double[] { 1, 0, 0, 
                                                                            0, Math.Cos(currentAttachment.RotR), Math.Sin(currentAttachment.RotR), 
                                                                            0, -Math.Sin(currentAttachment.RotR), Math.Cos(currentAttachment.RotR), 
                                                                            0, 0, 0, 1, 0, 0, 0 });
                MathTransform transP = mathUtil.CreateTransform(new double[] { Math.Cos(currentAttachment.RotP), 0, -Math.Sin(currentAttachment.RotP), 
                                                                            0, 1, 0, 
                                                                            Math.Sin(currentAttachment.RotP), 0, Math.Cos(currentAttachment.RotP), 
                                                                            0, 0, 0, 1, 0, 0, 0 });
                MathTransform transY = mathUtil.CreateTransform(new double[] { Math.Cos(currentAttachment.RotY),Math.Sin(currentAttachment.RotY),0,
                                                                            -Math.Sin(currentAttachment.RotY),Math.Cos(currentAttachment.RotY),0,
                                                                            0,0,1,
                                                                            0,0,0,1,0,0,0 });
                MathTransform rotMatrix = transR.Multiply(transP).Multiply(transY);

                MathVector xAxis = mathUtil.CreateVector(new double[] { 1, 0, 0 });
                xAxis = xAxis.MultiplyTransform(rotMatrix);
                MathVector yAxis = mathUtil.CreateVector(new double[] { 0, 1, 0 });
                yAxis = yAxis.MultiplyTransform(rotMatrix);
                MathVector zAxis = mathUtil.CreateVector(new double[] { 0, 0, 1 });
                zAxis = zAxis.MultiplyTransform(rotMatrix);

                rotManipulator.XAxis = xAxis;
                rotManipulator.YAxis = yAxis;
                rotManipulator.ZAxis = zAxis;

                rotManipulator.UpdatePosition();
                swManip.Show(modelDoc);*/
            }
            if (currentAttachment.DirectionAxis!=null)
            {
                //modelDoc.ClearSelection2(false);
                axisSelectionbox.SetSelectionFocus();
                
                
                SelectData data = ((SelectionMgr)modelDoc.SelectionManager).CreateSelectData();
                data.Mark = 4;
                ((IEntity)currentAttachment.DirectionAxis).Select4(true, data);
                originSelectionbox.SetSelectionFocus();
            }
            
            
        }

        /// <summary>
        /// Calculates the rotations in degrees and fill in textboxes
        /// </summary>
        /// <param name="axis">The normalised axis to find the rotation of</param>
        private void CalcRotations()
        {
            double[] rotations = currentAttachment.CalcRotations(axis);

            rollTextbox.Text = rotations[0].ToString();
            pitchTextbox.Text = rotations[1].ToString();
            yawTextbox.Text = rotations[2].ToString();
        }

        /// <summary>
        /// saves all the attachment data
        /// </summary>
        public void SaveAttachment()
        {
            object originObj =((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 2);
            currentAttachment.OriginPoint = originObj;
            if (originObj is IVertex)
            {
                MathTransform vertTransform = ((IEntity)originObj).GetComponent().Transform2;
                double[] tempPoint = ((IVertex)originObj).GetPoint();
                double[] originPt = swApp.GetMathUtility().CreatePoint(tempPoint).MultiplyTransform(vertTransform).ArrayData;
                currentAttachment.OriginX = originPt[0];
                currentAttachment.OriginY = originPt[1];
                currentAttachment.OriginZ = originPt[2];
            }
            else if (originObj is IFeature)
            {
                MathPoint tempPoint = ((IFeature)originObj).GetSpecificFeature2().GetRefPoint();
                double[] originPt;
                if(((IEntity)originObj).GetComponent() != null)
                {
                    MathTransform vertTransform = ((IEntity)originObj).GetComponent().Transform2;
                    originPt = swApp.GetMathUtility().CreatePoint(tempPoint).MultiplyTransform(vertTransform).ArrayData;
                }
                else
                {
                    originPt = tempPoint.ArrayData;
                }
                currentAttachment.OriginX = originPt[0];
                currentAttachment.OriginY = originPt[1];
                currentAttachment.OriginZ = originPt[2];
            }
            currentAttachment.DirectionAxis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 4);
            double num;
            if (Double.TryParse(rollTextbox.Text, out num))
            {
                currentAttachment.RotRoll = num * Math.PI / 180;
            }
            if (Double.TryParse(pitchTextbox.Text, out num))
            {
                currentAttachment.RotPitch = num * Math.PI / 180;
            }
            if (Double.TryParse(yawTextbox.Text, out num))
            {
                currentAttachment.RotYaw = num * Math.PI / 180;
            }

        }

        /// <summary>
        /// toggles whether manual angles are enabled
        /// </summary>
        /// <param name="manualEnabled">true if manual angles should be eliminated</param>
        public void ToggleManualAngles(bool manualEnabled)
        {
            ((PropertyManagerPageControl)axisLabel).Enabled = !manualEnabled;
            ((PropertyManagerPageControl)axisSelectionbox).Enabled = !manualEnabled;
            ((PropertyManagerPageControl)flipAxisButton).Enabled = !manualEnabled;
            ((PropertyManagerPageControl)rollTextbox).Enabled = manualEnabled;
            ((PropertyManagerPageControl)pitchTextbox).Enabled = manualEnabled;
            ((PropertyManagerPageControl)yawTextbox).Enabled = manualEnabled;
        }

        /// <summary>
        /// Draws a preview of the sensor's view
        /// Will draw a square prism if the FOV is a positive number (Camera), and a cone if the FOV is negative (RangeFinder)
        /// </summary>
        public void DrawViewPreview()
        {
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = false;
            if (previewBodies != null)
            {
                foreach (Body2 b in previewBodies)
                    if (b != null)
                        b.Hide(RobotInfo.DispComp);
            }
            if (originPt != null)
            {
                if (currentAttachment.FOV > 0)
                {
                    double viewDist = 1;
                    double scaleFactor = 3.0 / 4;
                    double horzAngle = currentAttachment.FOV;

                    MathUtility matUtil = swApp.GetMathUtility();
                    
                    double width = Math.Tan(horzAngle * Math.PI / 360) * viewDist;
                    double height = width * scaleFactor;

                    double[] origin = {0,0,0};
                    double[] upperLeftCorner = { viewDist, width / 2, height / 2 };
                    double[] lowerLeftCorner = { viewDist, width / 2, -height / 2};
                    double[] upperRightCorner = { viewDist, - width / 2, height / 2};
                    double[] lowerRightCorner = { viewDist , - width / 2, -height / 2};


                    Component2 firstComp = RobotInfo.DispComp;
                    if (previewBodies[0] != null)//clear out old bodies
                    {
                        for (int i = 0; i < previewBodies.Length; i++)
                        {
                            previewBodies[i].Hide(firstComp.GetModelDoc2());
                        }
                    }
                    
                    previewBodies = new Body2[3];
                    //create front box
                    ICurve[] boxEdges = new ICurve[4];
                    boxEdges[0] = swModeler.CreateLine(upperLeftCorner, new double[] { 0, -1, 0 });
                    boxEdges[0] = boxEdges[0].CreateTrimmedCurve2(upperLeftCorner[0], upperLeftCorner[1], upperLeftCorner[2], upperRightCorner[0], upperRightCorner[1], upperRightCorner[2]);
                    boxEdges[1] = swModeler.CreateLine(upperRightCorner, new double[] { 0, 0, -1 });
                    boxEdges[1] = boxEdges[1].CreateTrimmedCurve2(upperRightCorner[0], upperRightCorner[1], upperRightCorner[2], lowerRightCorner[0], lowerRightCorner[1], lowerRightCorner[2]);
                    boxEdges[2] = swModeler.CreateLine(lowerRightCorner, new double[] { 0, 1, 0 });
                    boxEdges[2] = boxEdges[2].CreateTrimmedCurve2(lowerRightCorner[0], lowerRightCorner[1], lowerRightCorner[2], lowerLeftCorner[0], lowerLeftCorner[1], lowerLeftCorner[2]);
                    boxEdges[3] = swModeler.CreateLine(lowerLeftCorner, new double[] { 0, 0, 1 });
                    boxEdges[3] = boxEdges[3].CreateTrimmedCurve2(lowerLeftCorner[0], lowerLeftCorner[1], lowerLeftCorner[2],upperLeftCorner[0], upperLeftCorner[1], upperLeftCorner[2]);

                    previewBodies[0] = swModeler.CreateWireBody(boxEdges, 0);

                    //create top face
                    ICurve[] topEdges = new ICurve[3];
                    topEdges[0] = swModeler.CreateLine(upperLeftCorner, new double[] { -upperLeftCorner[0], -upperLeftCorner[1], -upperLeftCorner[2] });
                    topEdges[0] = topEdges[0].CreateTrimmedCurve2(upperLeftCorner[0], upperLeftCorner[1], upperLeftCorner[2], 0,0,0);
                    topEdges[1] = swModeler.CreateLine(new double[] { 0, 0, 0 }, upperRightCorner);
                    topEdges[1] = topEdges[1].CreateTrimmedCurve2(0, 0, 0, upperRightCorner[0], upperRightCorner[1], upperRightCorner[2]);
                    topEdges[2] = swModeler.CreateLine(upperRightCorner, new double[] {0,1,0});
                    topEdges[2] = topEdges[2].CreateTrimmedCurve2(upperRightCorner[0], upperRightCorner[1], upperRightCorner[2],upperLeftCorner[0],upperLeftCorner[1],upperLeftCorner[2]);

                    previewBodies[1] = swModeler.CreateWireBody(topEdges, 0);
                    
                    //create bottom face
                    ICurve[] bottomEdges = new ICurve[2];
                    bottomEdges[0] = swModeler.CreateLine(lowerLeftCorner, new double[] { -lowerLeftCorner[0], -lowerLeftCorner[1], -lowerLeftCorner[2] });
                    bottomEdges[0] = bottomEdges[0].CreateTrimmedCurve2(lowerLeftCorner[0], lowerLeftCorner[1], lowerLeftCorner[2], 0, 0, 0);
                    bottomEdges[1] = swModeler.CreateLine(lowerRightCorner, new double[] { -lowerRightCorner[0], -lowerRightCorner[1], -lowerRightCorner[2] });
                    bottomEdges[1] = bottomEdges[1].CreateTrimmedCurve2(lowerRightCorner[0], lowerRightCorner[1], lowerRightCorner[2], 0, 0, 0);
                    
                    previewBodies[2] = swModeler.CreateWireBody(bottomEdges, 0);

                    double[] transposedOrigin = originPt.ArrayData;

                    MathTransform dispTransform = matUtil.CreateTransform(new double[] {1,0,0,
                                                                                        0,1,0,
                                                                                        0,0,1,
                                                                                        transposedOrigin[0],transposedOrigin[1],transposedOrigin[2],
                                                                                        1,0,0,0});

                    MathTransform rollTransform = matUtil.CreateTransform(new double[] {1,0,0,
                                                                                        0,Math.Cos(Math.PI/180*Double.Parse(rollTextbox.Text)),Math.Sin(Math.PI/180*Double.Parse(rollTextbox.Text)),
                                                                                        0,-Math.Sin(Math.PI/180*Double.Parse(rollTextbox.Text)),Math.Cos(Math.PI/180*Double.Parse(rollTextbox.Text)),
                                                                                        0,0,0,
                                                                                        1,0,0,0});

                    MathTransform pitchTransform = matUtil.CreateTransform(new double[] {Math.Cos(Math.PI/180*Double.Parse(pitchTextbox.Text)),0,-Math.Sin(Math.PI/180*Double.Parse(pitchTextbox.Text)),
                                                                                        0,1,0,
                                                                                        Math.Sin(Math.PI/180*Double.Parse(pitchTextbox.Text)),0,Math.Cos(Math.PI/180*Double.Parse(pitchTextbox.Text)),
                                                                                        0,0,0,
                                                                                        1,0,0,0});

                    MathTransform yawTransform = matUtil.CreateTransform(new double[] {Math.Cos(Math.PI/180*Double.Parse(yawTextbox.Text)),Math.Sin(Math.PI/180*Double.Parse(yawTextbox.Text)),0,
                                                                                        -Math.Sin(Math.PI/180*Double.Parse(yawTextbox.Text)),Math.Cos(Math.PI/180*Double.Parse(yawTextbox.Text)),0,
                                                                                        0,0,1,
                                                                                        0,0,0,
                                                                                        1,0,0,0});
                    MathTransform completeTransform = rollTransform.Multiply(pitchTransform).Multiply(yawTransform).Multiply(dispTransform);

                    foreach (Body2 b in previewBodies)
                    {
                        b.ApplyTransform(completeTransform);
                        b.ApplyTransform(firstComp.Transform2.Inverse());
                        
                        b.Display3(firstComp, 0x0000FF, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
                    }
                    
    
                }
                else // Preview for Rangefinder
                {                    
                    double coneDist = 1;
                    double coneRadius = Math.Abs(currentAttachment.FOV);

                    MathUtility matUtil = swApp.GetMathUtility();

                    MathVector axisVector = matUtil.CreateVector(new double[] { axis[0], axis[1], axis[2] }).Scale(coneDist);

                    double[] coneOrigin = originPt.AddVector(axisVector).ArrayData;

                    double[] coneArr = { coneOrigin[0], coneOrigin[1], coneOrigin[2], -axis[0], -axis[1], -axis[2], coneRadius, 0, coneDist };

                    Component2 firstComp = RobotInfo.DispComp;
                    if (previewBodies[0] != null)//clear out old bodies
                    {
                        for (int i = 0; i < previewBodies.Length; i++)
                        {
                            previewBodies[i].Hide(firstComp.GetModelDoc2());
                        }
                    }
                    previewBodies = new Body2[] {swModeler.CreateBodyFromCone(coneArr)};
                    
                    previewBodies[0].ApplyTransform(firstComp.Transform2.Inverse());
                    previewBodies[0].Display3(firstComp, 0xFF00, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
                    previewBodies[0].MaterialPropertyValues2 = new double[] { 1, 0, 0, 0, 0, 0, 0, .9, 0 };

                }
                
            }
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = true;
            
        }


        #region handlers



        /// <summary>
        /// Called when the attachment property manager page is closed
        /// </summary>
        /// <param name="Reason">Reason this page is closing</param>
        public void OnClose(int Reason)
        {
            SaveAttachment();
            Component2 firstComp = RobotInfo.DispComp;
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = false;
            for (int i = 0; i < previewBodies.Length; i++)
            {
                System.Diagnostics.Debug.WriteLine(firstComp.Name2);
                if (previewBodies[i] != null) // check if a point has been selected
                    previewBodies[i].Hide((PartDoc)(firstComp.GetModelDoc2()));
                //Marshal.FinalReleaseComObject(previewBodies[i]);
            }
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = true;
            previewBodies = null;
            GC.Collect();
            GC.Collect();
            modelDoc.GraphicsRedraw2();
            modelDoc.ClearSelection2(true);
            if (Reason != (int)swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_ParentClosed && RestoreManager != null)
                RestoreManager(currentAttachment);
            
            
        }

        /// <summary>
        /// Called to verify whether to submit a newly selected item into a selection box on attachment property manager page
        /// </summary>
        /// <param name="Id">Id of selection box in focus</param>
        /// <param name="Selection">Item selected</param>
        /// <param name="SelType">Type of item selected</param>
        /// <param name="ItemText">Text to enter into selectionbox for this item</param>
        /// <returns>true if valid selection</returns>
        public bool OnSubmitSelection(int Id, object Selection, int SelType, ref string ItemText)
        {
            MathTransform componentTransform = null;
            if ((Component2)((IEntity)Selection).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
            {
                componentTransform = ((Component2)((IEntity)Selection).GetComponent()).Transform2;
            }
            MathUtility matUtil = swApp.GetMathUtility();
            switch (Id)
            {
                case originSelectionboxID:
                    MathTransform originTransform = null;

                    if (((IEntity)Selection).GetComponent() != null)
                    {
                        originTransform = ((IEntity)Selection).GetComponent().Transform2;
                    }

                    
                    if (Selection is IVertex)
                    {
                        originPt = matUtil.CreatePoint(((IVertex)Selection).GetPoint()).MultiplyTransform(originTransform);
                    }
                    else if (Selection is IFeature)
                    {
                        if (originTransform != null)
                        {
                            originPt = ((IFeature)Selection).GetSpecificFeature2().GetRefPoint().MultiplyTransform(originTransform);
                        }
                        else
                        {
                            originPt = ((IFeature)Selection).GetSpecificFeature2().GetRefPoint();
                        }
                    }
                    
                    break;
                case axisSelectionboxID:
                    if (SelType == (int)swSelectType_e.swSelDATUMAXES)
                    {
                        IRefAxis tempAxis = ((IFeature)Selection).GetSpecificFeature2();
                        double[] refPoints = tempAxis.GetRefAxisParams();
                        double[] axis;
                        if (currentAttachment.FlipAxis)
                        {
                            axis = new double[] { refPoints[0] - refPoints[3], refPoints[1] - refPoints[4], refPoints[2] - refPoints[5] };
                        }
                        else
                        {
                            axis = new double[] { refPoints[3] - refPoints[0], refPoints[4] - refPoints[1], refPoints[5] - refPoints[2] };
                        }
                        
                        if (componentTransform != null)
                        {
                            MathVector transformedAxis = matUtil.CreateVector(axis).MultiplyTransform(componentTransform);
                            this.axis = transformedAxis.Normalise().ArrayData;
                            featureAxis = this.axis;
                        }
                        else
                        {
                            this.axis=matUtil.CreateVector(axis).Normalise().ArrayData;
                            featureAxis = this.axis;
                        }
                        
                        
                    }
                    else if (SelType == (int)swSelectType_e.swSelEDGES)
                    {
                        if (((IEdge)Selection).GetCurve().LineParams is DBNull)
                        {
                            return false;
                        }
                        
                        double[] tempArr = ((IEdge)Selection).GetCurve().LineParams;
                        double[] axis;
                        if (currentAttachment.FlipAxis)
                        {
                            axis = new double[] { -tempArr[3], -tempArr[4], -tempArr[5] };
                        }
                        else
                        {
                            axis = new double[] { tempArr[3], tempArr[4], tempArr[5] };
                        }
                        
                            
                        MathVector transformedAxis = matUtil.CreateVector(axis).MultiplyTransform(componentTransform);
                        this.axis=transformedAxis.ArrayData;
                        featureAxis = this.axis;
                            
                    }
                    break;
            }
            CalcRotations();
            DrawViewPreview();
            return true;
        }

        public void OnSelectionboxListChanged(int Id, int Count)
        {
            if(Count == 0)
                switch (Id)
                {
                    case originSelectionboxID:
                        currentAttachment.OriginPoint = null;
                        originPt = null;
                        DrawViewPreview();
                        break;
                    case axisSelectionboxID:
                        currentAttachment.DirectionAxis = null;
                        axis = new double[] { 1, 0, 0 };
                        CalcRotations();
                        DrawViewPreview();
                        break;
                }
        }


        public void OnTextboxChanged(int Id, string Text)
        {
            double num;
            if (Double.TryParse(Text, out num))
            {
                double origNum = num;
                num = num%360;
                if(num < 0)
                {
                    num+=360;
                }
                if (origNum != num)
                {
                    switch (Id)
                    {
                        case rollTextboxID:
                            rollTextbox.Text = num.ToString();
                            break;
                        case pitchTextboxID:
                            pitchTextbox.Text = num.ToString();
                            break;
                        case yawTextboxID:
                            yawTextbox.Text = num.ToString();
                            break;
                    }
                }
                double[] rots = new double[3];
                rots[0] = Double.Parse(rollTextbox.Text) * Math.PI / 180;
                rots[1] = Double.Parse(pitchTextbox.Text) * Math.PI / 180;
                rots[2] = Double.Parse(yawTextbox.Text) * Math.PI / 180;
                axis = currentAttachment.CalcAxisFromRotations(rots);
                System.Diagnostics.Debug.WriteLine(String.Join(" ", axis));
                DrawViewPreview();

            }
            /*else if(!Text.Equals("-") && !Text.Equals(""))
            {
                switch (Id)
                {
                    case rollTextboxID:
                        rollTextbox.Text = "0";
                        break;
                    case pitchTextboxID:
                        pitchTextbox.Text = "0";
                        break;
                    case yawTextboxID:
                        yawTextbox.Text = "0";
                        break;
                }
            }*/
        }

        public void OnLostFocus(int Id)
        {
            double num;
            switch (Id)
            {
                case rollTextboxID:
                    if (!Double.TryParse(rollTextbox.Text, out num))
                    {
                        rollTextbox.Text = "0";
                    }
                    break;
                case pitchTextboxID:
                    if (!Double.TryParse(pitchTextbox.Text, out num))
                    {
                        pitchTextbox.Text = "0";
                    }
                    break;
                case yawTextboxID:
                    if (!Double.TryParse(yawTextbox.Text, out num))
                    {
                        yawTextbox.Text = "0";
                    }
                    break;
            }
        }

        public void OnCheckboxCheck(int Id, bool Checked)
        {
            switch (Id)
            {
                
                case manualAngleCheckboxID:
                    ToggleManualAngles(Checked);
                    currentAttachment.UseManualAngles = Checked;
                    if (!Checked)
                    {
                        axis = featureAxis;
                        CalcRotations();
                        DrawViewPreview();
                    }
                    break;
            }
        }

        public void OnButtonPress(int Id)
        {
            if (Id == flipAxisButtonID)
            {
                currentAttachment.FlipAxis = flipAxisButton.Checked;
                axis[0] *= -1;
                axis[1] *= -1;
                axis[2] *= -1;
                CalcRotations();
                DrawViewPreview();
            }
            
        }

        public void OnSliderPositionChanged(int Id, double Value)
        {
            if (currentAttachment.FOV < 0)
            {
                currentAttachment.FOV = -Value / 100;
                DrawViewPreview();
            }
            else
            {
                currentAttachment.FOV = Value;
                DrawViewPreview();
            }
            
        }
        #endregion

        #region unused handlers
        //pmp handlers
        public void AfterActivation()
        {
            
        }

        public void AfterClose()
        {
            
        }

        public int OnActiveXControlCreated(int Id, bool Status)
        {
            return 1;
        }

        

        

        

        public void OnComboboxEditChanged(int Id, string Text)
        {
        }

        public void OnComboboxSelectionChanged(int Id, int Item)
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
            return true;
        }

        public bool OnKeystroke(int Wparam, int Message, int Lparam, int Id)
        {
            return true;
        }

        public void OnListboxRMBUp(int Id, int PosX, int PosY)
        {
        }

        public void OnListboxSelectionChanged(int Id, int Item)
        {
        }

        

        public bool OnNextPage()
        {
            return true;
        }

        public void OnNumberBoxTrackingCompleted(int Id, double Value)
        {
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
            return true;
        }

        public bool OnPreviousPage()
        {
            return true;
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

        public void OnSelectionboxFocusChanged(int Id)
        {
        }



        

        public void OnSliderTrackingCompleted(int Id, double Value)
        {
        }

        

        public bool OnTabClicked(int Id)
        {
            return true;
        }

        

        public void OnUndo()
        {
        }

        public void OnWhatsNew()
        {
        }

        public int OnWindowFromHandleControlCreated(int Id, bool Status)
        {
            return 1;
        }

        //manipuloator handlers
        /*public bool OnDelete(object pManipulator)
        {
            return false;
        }

        public void OnDirectionFlipped(object pManipulator)
        {
            throw new NotImplementedException();
        }

        public bool OnDoubleValueChanged(object pManipulator, int handleIndex, ref double Value)
        {
            //System.Diagnostics.Debug.WriteLine(Value);
            return true;
        }

        public void OnEndDrag(object pManipulator, int handleIndex)
        {
            MathVector startVector=null;
            MathVector endVector=null;
            IMathUtility mathUtil = (IMathUtility)swApp.GetMathUtility();
            switch (handleIndex)
            {
                case 7://around z
                    double[] tempArr = rotManipulator.Origin.ArrayData;
                    startVector = mathUtil.CreateVector(new double[] { startPoint.ArrayData[0] - tempArr[0], startPoint.ArrayData[1] - tempArr[1], 0 }).Normalise();
                    endVector = mathUtil.CreateVector(new double[] { endPoint.ArrayData[0] - tempArr[0], endPoint.ArrayData[1] - tempArr[1], 0 }).Normalise();
                    System.Diagnostics.Debug.WriteLine(Math.Acos(startVector.Dot(endVector))*180/Math.PI);
                    break;
                case 8://around x
                    break;
                case 9://around y
                    break;
            }
            rotManipulator.UpdatePosition();
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
            System.Diagnostics.Debug.WriteLine(handleIndex);
            firstRotPoint = true;
        }

        public void OnItemSetFocus(object pManipulator, int handleIndex)
        {
        }

        public bool OnStringValueChanged(object pManipulator, int handleIndex, ref string Value)
        {
            //System.Diagnostics.Debug.WriteLine(Value);
            return true;
        }

        public void OnUpdateDrag(object pManipulator, int handleIndex, object newPosMathPt)
        {
            //System.Diagnostics.Debug.WriteLine(String.Join(" ", (double[])((TriadManipulator)pManipulator).XAxis.ArrayData) + String.Join(" ", (double[])((TriadManipulator)pManipulator).YAxis.ArrayData) + String.Join(" ", (double[])((TriadManipulator)pManipulator).ZAxis.ArrayData));
            //System.Diagnostics.Debug.WriteLine(String.Join(" ", (double[])((MathPoint)newPosMathPt).ArrayData));
            //rotManipulator.Origin = (MathPoint)newPosMathPt;
            if (firstRotPoint)
            {
                startPoint = ((IMathUtility)swApp.GetMathUtility()).CreatePoint(((MathPoint)newPosMathPt).ArrayData);
                firstRotPoint = false;
            }
            else
            {
                endPoint = ((IMathUtility)swApp.GetMathUtility()).CreatePoint(((MathPoint)newPosMathPt).ArrayData);
            }
            //rotManipulator.UpdatePosition();
        }*/
        #endregion

        
    }
}
