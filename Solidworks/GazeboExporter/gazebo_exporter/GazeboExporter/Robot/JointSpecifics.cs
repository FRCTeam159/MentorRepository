using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;
using GazeboExporter.Storage;
using System.Windows.Forms;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using GazeboExporter.Export;
using GazeboExporter.UI;
using System.Xml;


namespace GazeboExporter.Robot
{
    /// <summary>
    /// Abstract class representing the specific properties of a joint
    /// This class will be extened to create each different type of joint
    /// </summary>
    public abstract class JointSpecifics:SelectionObserver,ButtonObserver,CheckboxObserver,SwManipulatorHandler2
    {
        protected readonly RobotModel robot;
        protected readonly Joint joint;
        protected readonly SldWorks swApp;
        protected readonly ModelDoc2 modelDoc;
        protected readonly StorageModel swData;
        protected readonly string path;

        MathVector xVector;
        MathVector yVector;

        Manipulator swManip;
        TriadManipulator originTriad;

        bool inPropertyPage = false;

        //The origin and rotation of the joint
        public OriginPoint OriginValues;

        /// <summary>
        /// The origin point of the joint
        /// </summary>
        public object OriginPt { 
            get
            {
                return swData.GetObject(path + "/originPt");
            }
            set
            {
                object oldPoint = OriginPt;
                if(value != null)
                    OriginValues.Point = GetPointFromEntity(value);
                swData.SetObject(path + "/originPt", value);
                if(value != oldPoint)
                    UpdatePreviews();
            } 
        }

        /// <summary>
        /// Face, Edge, RefPlane, or RefAxis determining the X-axis direction
        /// </summary>
        public object XAxis
        {
            get
            {
                return swData.GetObject(path + "/xAxis");
            }
            set
            {
                swData.SetObject(path + "/xAxis", value);
            } 
        }

        

        /// <summary>
        /// Boolean to flip the direction of the x-axis vector
        /// </summary>
        public bool FlipXAxisDir
        {
            get
            {
                return swData.GetDouble(path + "/flipXAxisDir") == 1;
            }
            set
            {
                swData.SetDouble(path + "/flipXAxisDir", value ? 1 : 0);
                if(xVector != null)
                    xVector = xVector.Scale(-1);
            }
        }

        /// <summary>
        /// Face, Edge, RefPlane, or RefAxis determining the X-axis direction
        /// </summary>
        public object YAxis
        {
            get
            {
                return swData.GetObject(path + "/yAxis");
            }
            set
            {
                swData.SetObject(path + "/yAxis", value);
            } 
        }

        /// <summary>
        /// Boolean to flip the direction of the x-axis vector
        /// </summary>
        public bool FlipYAxisDir
        {
            get
            {
                return swData.GetDouble(path + "/flipYAxisDir") == 1;
            }
            set
            {
                swData.SetDouble(path + "/flipYAxisDir", value ? 1 : 0);
                if(yVector != null)
                    yVector = yVector.Scale(-1);
            }
        }

        /// <summary>
        /// Boolean to align the xaxis of the joint to the rotation axis
        /// </summary>
        public bool AlignToXaxis
        {
            get
            {
                return swData.GetDouble(path + "/AlignToXaxis") == 1;
            }
            set
            {
                swData.SetDouble(path + "/AlignToXaxis", value ? 1 : 0);
                
            }
        }

        private List<object> labels;
        private Dictionary<int, PropertyManagerPageSelectionbox> selBoxes;
        private int[] selBoxIDs;
        private Dictionary<int, PropertyManagerPageBitmapButton> flipButtons;
        private int[] flipButtonIds;
        private Dictionary<int, PropertyManagerPageCheckbox> checkBoxes;
        private int[] checkboxIds;
        PropertyManagerPageGroup groupBox;



        /// <summary>
        /// Creates a new Specific Joint object
        /// </summary>
        /// <param name="joint">The joint that this joint is in</param>
        /// <param name="path">The storage path for this joint</param>
        protected JointSpecifics(Joint joint, string path)
        {
            this.robot = RobotInfo.Robot;
            this.joint = joint;
            this.swApp = RobotInfo.SwApp;
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;

            OriginValues = new OriginPoint();
            if(OriginPt != null)
                OriginValues.Point = GetPointFromEntity(OriginPt);
        }


        /// <summary>
        /// Calculates the rotations of the joints reference frame
        /// </summary>
        private void CalcJointRotation()
        {
            MathVector xAxisVector = RobotInfo.mathUtil.CreateVector(new double[] { 1, 0, 0 });
            MathVector yAxisVector = RobotInfo.mathUtil.CreateVector(new double[] { 0, 1, 0 });
            MathVector zAxisVector = RobotInfo.mathUtil.CreateVector(new double[] { 0, 0, 1 });

            MathVector xCoordVector;
            MathVector yCoordVector;
            MathVector zCoordVector;

            if (xVector != null)
            {
                xCoordVector = xVector;
                if (yVector != null)
                {
                    yCoordVector = yVector;
                    zCoordVector = xVector.Cross(yVector).Normalise();
                }
                else
                {
                    MathVector tempAxis = zAxisVector.Cross(xVector);
                    if (tempAxis.GetLength() < VectorCalcs.errorVal)
                        tempAxis = yAxisVector;
                    else
                        tempAxis = tempAxis.Normalise();
                    yCoordVector = tempAxis;
                    zCoordVector = xVector.Cross(tempAxis).Normalise();
                }
            }
            else
            {
                xCoordVector = xAxisVector;
                yCoordVector = yAxisVector;
                zCoordVector = zAxisVector;
            }

            double pitch = -Math.Asin(xCoordVector.ArrayData[2]);
            double yaw = Math.Atan2(xCoordVector.ArrayData[1],xCoordVector.ArrayData[2]);
            double roll = Math.Atan2(yCoordVector.ArrayData[2], zCoordVector.ArrayData[2]);
            if (Math.Abs(Math.Cos(pitch)) < VectorCalcs.errorVal)
            {
                roll = 0;
                yaw = Math.Asin(zCoordVector.ArrayData[1]) * (pitch > 0 ? 1 : -1);
            }
            else if (Math.Abs(xCoordVector.ArrayData[0] + 1) < VectorCalcs.errorVal)
                yaw = Math.PI;

            if (roll < 0)
                roll += 2 * Math.PI;
            if (pitch < 0)
                pitch += 2 * Math.PI;
            if (yaw < 0)
                yaw += 2 * Math.PI;

            OriginValues.Roll = roll;
            OriginValues.Pitch = pitch;
            OriginValues.Yaw = yaw;

        }

        /// <summary>
        /// Gets the Property panel for this joint
        /// </summary>
        /// <returns>Returns the newly created property panel</returns>
        public virtual UserControl CreatePropertiesPanel()
        {
            return new JointSpecificProperties(joint);
        }

        /// <summary>
        /// Creates and fills a new solidworks property page
        /// </summary>
        /// <returns>The newly created proprerty page</returns>
        public virtual JointPMPage FillPropertyPage()
        {
            JointPMPage page = new JointPMPage(robot, joint, (AssemblyDoc)modelDoc, swApp);
            return page;
        }

        /// <summary>
        /// Adds the joint pose selections to the pmpage
        /// <param name="page">The page to add the boxes too</param>
        /// </summary>
        public void AddJointPoseToPMPage(JointPMPage page)
        {
            if (!page.CheckboxObservers.Contains(this))
                page.CheckboxObservers.Add(this);
            if (!page.SelectionObservers.Contains(this))
                page.SelectionObservers.Add(this);
            if (!page.ButtonObservers.Contains(this))
                page.ButtonObservers.Add(this);

            labels = new List<object>();
            selBoxes = new Dictionary<int, PropertyManagerPageSelectionbox>();
            selBoxIDs = new int[3];
            flipButtons = new Dictionary<int, PropertyManagerPageBitmapButton>();
            flipButtonIds = new int[2];
            checkBoxes = new Dictionary<int, PropertyManagerPageCheckbox>();
            checkboxIds = new int[2];


            //Helper variables for setup options
            //control type
            short labelCT = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            short selectionboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            short checkboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Checkbox;
            short buttonCT = (int)swPropertyManagerPageControlType_e.swControlType_CheckableBitmapButton;
            // align
            short leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            short shiftedAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            // options
            int groupboxOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible | 
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Checkbox; // for group box
            int controlOptions = (int)swAddControlOptions_e.swControlOptions_Enabled |
                                 (int)swAddControlOptions_e.swControlOptions_Visible; // for controls
            int[] selectionfilter = { (int)swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelFACES,(int)swSelectType_e.swSelDATUMPLANES };
            int[] originfilter = { (int)swSelectType_e.swSelDATUMPOINTS, (int)swSelectType_e.swSelVERTICES };

            //Setup and create the joint pose group
            groupBox = (PropertyManagerPageGroup)page.page.AddGroupBox(page.currentId++, "Joint Pose", groupboxOptions);
            groupBox.Expanded = groupBox.Checked;

            //Setup and create the label for the origin point selection box
            PropertyManagerPageLabel originPointLabel = (PropertyManagerPageLabel)groupBox.AddControl(
                page.currentId++, labelCT, "Joint origin point", shiftedAlign, controlOptions, "Select the origin point for this joint");
            labels.Add(originPointLabel);
            //Setup and create the origin point selectionbox
            PropertyManagerPageSelectionbox originPointSelectionbox = (PropertyManagerPageSelectionbox)groupBox.AddControl(
                page.currentId, selectionboxCT, "Joint origin point", shiftedAlign, controlOptions, "Use this box to select a point");
            originPointSelectionbox.AllowSelectInMultipleBoxes = false;
            originPointSelectionbox.SingleEntityOnly = true;
            originPointSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem1);
            originPointSelectionbox.Mark = page.currentMark;
            originPointSelectionbox.SetSelectionFilters(originfilter);

            selBoxes.Add(page.currentId, originPointSelectionbox);
            selBoxIDs[0] = page.currentId++;
            page.currentMark = page.currentMark << 1;

            //Setup and create the align to x-axis checkbox
            PropertyManagerPageCheckbox alignToXaxisCheckbox = (PropertyManagerPageCheckbox)groupBox.AddControl(
                page.currentId, checkboxCT, "Align X-axis to movement axis", leftAlign, controlOptions, "");
            checkBoxes.Add(page.currentId,alignToXaxisCheckbox);
            checkboxIds[0] = page.currentId++;

            //Setup and create the label for the x-axis selection box
            PropertyManagerPageLabel XAxisLabel = (PropertyManagerPageLabel)groupBox.AddControl(
                page.currentId++, labelCT, "X-axis", shiftedAlign, controlOptions, "Select X-axis for this joint");
            labels.Add(XAxisLabel);
            //create x-axis flipbutton
            PropertyManagerPageBitmapButton flipXAxisButton = (PropertyManagerPageBitmapButton)groupBox.AddControl(
                page.currentId, buttonCT, "Flip X-axis", leftAlign, controlOptions, "Check to flip the direction of the x-axis");
            flipXAxisButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            flipXAxisButton.Checked = false;
            flipButtons.Add(page.currentId, flipXAxisButton);
            flipButtonIds[0] = page.currentId++;
            //Setup and create the x-axis selectionbox
            PropertyManagerPageSelectionbox XAxisSelectionbox = (PropertyManagerPageSelectionbox)groupBox.AddControl(
                page.currentId, selectionboxCT, "X-axis", shiftedAlign, controlOptions, "Use this box to select an axis");
            XAxisSelectionbox.AllowSelectInMultipleBoxes = false;
            XAxisSelectionbox.SingleEntityOnly = true;
            XAxisSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            XAxisSelectionbox.Mark = page.currentMark;
            XAxisSelectionbox.SetSelectionFilters(selectionfilter);

            selBoxes.Add(page.currentId, XAxisSelectionbox);
            selBoxIDs[1] = page.currentId++;
            page.currentMark = page.currentMark << 1;

            ((PropertyManagerPageControl)XAxisSelectionbox).Top = 110;
            ((PropertyManagerPageControl)flipXAxisButton).Top = (short)(((PropertyManagerPageControl)XAxisSelectionbox).Top + 3);

            //Setup and create the label for the y-axis selection box
            PropertyManagerPageLabel YAxisLabel = (PropertyManagerPageLabel)groupBox.AddControl(
                page.currentId++, labelCT, "Y-axis", shiftedAlign, controlOptions, "Select the Y-axis of the joint");
            labels.Add(YAxisLabel);
            //Setup and create the y-axis flipbutton
            PropertyManagerPageBitmapButton flipYAxisButton = (PropertyManagerPageBitmapButton)groupBox.AddControl(
                page.currentId, buttonCT, "Flip axis direction", leftAlign, controlOptions, "Check to flip the direction of the axis to the direction of the x-axis");
            flipYAxisButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            flipYAxisButton.Checked = false;
            flipButtons.Add(page.currentId, flipYAxisButton);
            flipButtonIds[1] = page.currentId++;
            //Setup and create the y-axis selection box
            PropertyManagerPageSelectionbox YAxisSelectionbox = (PropertyManagerPageSelectionbox)groupBox.AddControl(
                page.currentId, selectionboxCT, "Y-axis", shiftedAlign, controlOptions, "Use this box to select an axis");
            YAxisSelectionbox.AllowSelectInMultipleBoxes = false;
            YAxisSelectionbox.SingleEntityOnly = true;
            YAxisSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem4);
            YAxisSelectionbox.Mark = page.currentMark;
            YAxisSelectionbox.SetSelectionFilters(selectionfilter);
            selBoxes.Add(page.currentId, YAxisSelectionbox);
            selBoxIDs[2] = page.currentId++;
            page.currentMark = page.currentMark << 1;

            ((PropertyManagerPageControl)YAxisSelectionbox).Top = 160;
            ((PropertyManagerPageControl)flipYAxisButton).Top = (short)(((PropertyManagerPageControl)YAxisSelectionbox).Top + 3);
            LoadPMPValues();

        }

        /// <summary>
        /// Loads all the values into the property manager
        /// </summary>
        private void LoadPMPValues()
        {
            inPropertyPage = true;
            SelectionMgr selMgr = modelDoc.SelectionManager;
            if (OriginPt != null)
            {
                SelectData axisData = selMgr.CreateSelectData();
                axisData.Mark = selBoxes[selBoxIDs[0]].Mark;
                ((IEntity)OriginPt).Select4(true, axisData);
            }
            if (XAxis != null)
            {
                SelectData axisData = selMgr.CreateSelectData();
                axisData.Mark = selBoxes[selBoxIDs[1]].Mark;
                ((IEntity)XAxis).Select4(true, axisData);
            }
            if (YAxis != null)
            {
                SelectData axisData = selMgr.CreateSelectData();
                axisData.Mark = selBoxes[selBoxIDs[2]].Mark;
                ((IEntity)YAxis).Select4(true, axisData);
            }
            if (this is AxialJoint)
            {
                checkBoxes[checkboxIds[0]].Checked = AlignToXaxis;
                xVector = ((AxialJoint)this).GetAxisVector();
            }
            else
                ((IPropertyManagerPageControl)checkBoxes[checkboxIds[0]]).Enabled = false;
            flipButtons[flipButtonIds[0]].Checked = FlipXAxisDir;
            flipButtons[flipButtonIds[1]].Checked = FlipYAxisDir;

            if (!(this is AxialJoint) || OriginPt != null || XAxis != null || YAxis != null || AlignToXaxis)
            {
                groupBox.Checked = true;
                groupBox.Expanded = true;
                DrawOriginPreview();
            }
           
        }

        /// <summary>
        /// Verifies that the joint is valid for export
        /// </summary>
        /// <param name="log">logger to write messages to</param>
        /// <returns>returns true if the joint succesfully verfied</returns>
        public bool Verify(ProgressLogger log)
        {
            RobotInfo.WriteToLogFile("Verifying joint of type: " + this.GetJointTypeName());
            CalcJointRotation();
            RobotInfo.WriteToLogFile("Successfully Calculated Joint Rotation");
            return VerifySpecifics(log);
        }

        /// <summary>
        /// Writes the SDF tags for the joint
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">the writer to use to write the sdf</param>
        public virtual void WriteJointSDF(ProgressLogger log, XmlWriter writer)
        {
            string ParentName;
            string ChildName = joint.Child.Name.Replace(" ", "_");
            ParentName = joint.Parent.Name.Replace(" ", "_");


            string jointName = joint.Name.Replace(" ", "_");
            writer.WriteStartElement("joint");
            writer.WriteAttributeString("name", jointName);
            writer.WriteAttributeString("type", GetJointTypeName());
            log.WriteMessage("Writing joint " + joint.Name + " to SDF.");
            {
                SDFExporter.writeSDFElement(writer, "parent", ParentName); //Parent Link
                SDFExporter.writeSDFElement(writer, "child", ChildName);   //Child Link
                string jointpose = (OriginValues.X - joint.Child.OriginX) + " " + (OriginValues.Y - joint.Child.OriginY) + " " + (OriginValues.Z - joint.Child.OriginZ) + " " + OriginValues.Roll + " " + OriginValues.Pitch + " " + OriginValues.Yaw;
                SDFExporter.writeSDFElement(writer, "pose", jointpose);
                WriteSDF(log, writer);

            }
            writer.WriteEndElement();

            
        }

        /// <summary>
        /// Writes the sdfParameters specific to the joint
        /// </summary>
        /// <param name="log">The logger to write messages to</param>
        /// <param name="writer">the writer to use to write the sdf</param>
        public abstract void WriteSDF(ProgressLogger log, XmlWriter writer);

        #region Abstract Methods

        /// <summary>
        /// Verifies that the joint is valid for export
        /// </summary>
        /// <param name="log">logger to write messages to</param>
        /// <returns>returns true if the joint succesfully verfied</returns>
        protected abstract bool VerifySpecifics(ProgressLogger log);

        /// <summary>
        /// Calculates an origin point that is closest to the given point and on the line
        /// </summary>
        /// <param name="origPoint">POint to be close to</param>
        /// <returns>the origin point that is created</returns>
        protected abstract double[] CalcOrigin(double[] point);

        /// <summary>
        /// Gets the name of the joint type for export
        /// </summary>
        /// <returns>The name that will be used when the joint is exported</returns>
        public abstract string GetJointTypeName();

        /// <summary>
        /// Clears any values that need to be cleared inthe joint
        /// </summary>
        public abstract void ClearValues();

        /// <summary>
        /// Updates any previews for the joint
        /// </summary>
        public abstract void UpdatePreviews();

       
        #endregion

        #region Observer Handlers
        /// <summary>
        /// Saves all of the selected values
        /// </summary>
        public virtual void SaveSelections()
        {
            CalcJointRotation();
            OriginPt = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[0]].Mark);
            XAxis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[1]].Mark);
            YAxis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[2]].Mark);
            ClearOriginPreview();
            inPropertyPage = false;
        }

        /// <summary>
        /// Checks if the given object is a valid selection
        /// </summary>
        /// <param name="isValid">returns true if valid, false if not or does not aply to this observer</param>
        /// <param name="Id">Id of the selection box</param>
        /// <param name="SelType">Type of the selection</param>
        /// <param name="selection">The selected object</param>
        public virtual void CheckValidSelection(out bool isValid, int Id, int SelType, object selection)
        {
            if (selBoxes.ContainsKey(Id))
            {
                if (Id == selBoxIDs[0])
                {
                    isValid = true;
                    OriginPt = selection;
                }
                else
                {
                    if (IJointAxis.IsLinearAxis(selection))
                    {
                        if(Id == selBoxIDs[1])
                        {
                            isValid = true;
                            MathVector xAxisVector;
                            MathPoint xAxisPoint;
                            GetAxisFromObject(out xAxisVector, out xAxisPoint, selection, SelType);
                            xVector = xAxisVector;
                            if (yVector != null && !VectorCalcs.IsPerpendicular(xAxisVector, yVector))
                            {
                                YAxis = null;
                                SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;
                                int mark = selBoxes[selBoxIDs[2]].Mark;
                                swSelMgr.DeSelect2(1, mark);
                            }
                                
                        }
                        else
                        {
                            if (xVector == null)
                            {
                                ((IPropertyManagerPageControl)selBoxes[Id]).ShowBubbleTooltip("Error: must set X-Axis first", "Set the X-Axis before setting the Y-axis", null);
                                isValid = false;
                            }
                            else
                            {
                                MathVector yAxisVector;
                                MathPoint yAxisPoint;
                                GetAxisFromObject(out yAxisVector, out yAxisPoint, selection, SelType);
                                if (VectorCalcs.IsPerpendicular(yAxisVector, xVector))
                                {
                                    isValid = true;
                                    yVector = yAxisVector;
                                }
                                else
                                {
                                    isValid = false;
                                    ((IPropertyManagerPageControl)selBoxes[Id]).ShowBubbleTooltip("Error: Axes must be perpindicular", "The Y-Axis must be perpindicular to the X-Axis", null);
                                }

                            }
                        }
                    }
                    else
                        isValid = false;
                }
                DrawOriginPreview();
                return;
                    
                
            }
            isValid = false;
        }

        /// <summary>
        /// Updates the previews for the selected object
        /// </summary>
        /// <param name="Id">Id of the selection box</param>
        /// <param name="selection">Selected object</param>
        public virtual void UpdatePreviews(int Id, object selection)
        {

        }

        /// <summary>
        /// Saves the selected item
        /// </summary>
        /// <param name="Id">Id of the selectionbox to save</param>
        public virtual void SaveSpecificSelection(int Id)
        {
            int index = Array.IndexOf(selBoxIDs, Id);
            if (index != -1)
            {
                switch (index)
                {
                    case 0:
                        OriginPt = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[0]].Mark);
                        break;
                    case 1:
                        XAxis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[1]].Mark);
                        break;
                    case 2:
                        YAxis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, selBoxes[selBoxIDs[2]].Mark);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when a selection box is cleared of all values
        /// </summary>
        /// <param name="Id">The Id of the selection box</param>
        public virtual void NoSelections(int Id)
        {
            if (Id == selBoxIDs[0])
            {
                double[] COM = { joint.Child.ComX, joint.Child.ComY, joint.Child.ComZ };
                OriginValues.Point = CalcOrigin(COM);
                UpdatePreviews();
                DrawOriginPreview();
            }
            else if (Id == selBoxIDs[1])
            {
                xVector = null;
                yVector = null;
                SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;
                int mark = selBoxes[selBoxIDs[2]].Mark;
                swSelMgr.DeSelect2(1, mark);
            }
            else if (Id == selBoxIDs[2])
            {
                yVector = null;
            }

        }

        /// <summary>
        /// Handles events when a button is pressed
        /// </summary>
        /// <param name="Id">Id of the button that was pressed</param>
        public virtual void ButtonChanged(int Id)
        {
            int index = Array.IndexOf(flipButtonIds,Id);
            if(index != -1)
                switch (index)
                {
                    case 0:
                        FlipXAxisDir = flipButtons[Id].Checked;
                        break;
                    case 1:
                        FlipYAxisDir = flipButtons[Id].Checked;
                        break;
                }
            DrawOriginPreview();
        }

        /// <summary>
        /// Handles when a checkbox is checked or unchecked
        /// </summary>
        /// <param name="Id">Id of the checkbox</param>
        /// <param name="isChecked">The new state of the checkbox</param>
        public virtual void CheckboxChanged(int Id, bool isChecked)
        {
            int index = Array.IndexOf(checkboxIds, Id);
            if (index != -1)
                switch (index)
                {
                    case 0:
                        AlignToXaxis = isChecked;
                        ((PropertyManagerPageControl)selBoxes[selBoxIDs[1]]).Enabled = !isChecked;
                        ((PropertyManagerPageControl)flipButtons[flipButtonIds[0]]).Enabled = !isChecked;
                        if (isChecked)
                        {
                            yVector = null;
                            SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;
                            int mark = selBoxes[selBoxIDs[1]].Mark;
                            swSelMgr.DeSelect2(1, mark);
                            xVector = ((AxialJoint)this).GetAxisVector();
                        }
                        else
                        {
                            xVector = null;
                            yVector = null;
                            SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;
                            int mark = selBoxes[selBoxIDs[2]].Mark;
                            swSelMgr.DeSelect2(1, mark);
                        }
                        DrawOriginPreview();
                        
                        break;
                }
        }
        #endregion

        /// <summary>
        /// Gets the axis vector for a line or edge. Assumes the edge is linear
        /// </summary>
        /// <param name="axisVector">The calculated axis vector</param>
        /// <param name="axisPoint">The calculated point on the line</param>
        /// <param name="obj">The axis object</param>
        /// <param name="objType">The typeof the object</param>
        protected void GetAxisFromObject(out MathVector axisVector, out MathPoint axisPoint, object obj, int objType)
        {
            MathTransform componentTransform = null;
            if ((Component2)((IEntity)obj).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
            {
                componentTransform = ((Component2)((IEntity)obj).GetComponent()).Transform2;
            }
            double[] tempArray;
            if (objType == (int)swSelectType_e.swSelEDGES)
            {
                double[] edgeCurveParams = ((IEdge)obj).GetCurve().LineParams;
                tempArray = new double[] { edgeCurveParams[3], edgeCurveParams[4], edgeCurveParams[5] };
                axisVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector representing the linear edge
                tempArray = new double[] { edgeCurveParams[0], edgeCurveParams[1], edgeCurveParams[2] };//point on line
                axisPoint = VectorCalcs.CreatePoint(componentTransform, tempArray); // point transformed to global space
            }
            else if (objType == (int)swSelectType_e.swSelFACES)
            {
                axisVector = VectorCalcs.CreateVector(componentTransform,((IFace)obj).Normal);
                axisPoint = VectorCalcs.CreatePoint(componentTransform, ((IFace)obj).GetClosestPointOn(0, 0, 0));
            }
            else if (objType == (int)swSelectType_e.swSelDATUMAXES)
            {
                double[] points = ((IRefAxis)((IFeature)obj).GetSpecificFeature2()).GetRefAxisParams();
                tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                axisVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector between the 2 points on the reference axis and transforms to global space if nessacary
                tempArray = new double[] { points[0], points[1], points[2] };//point on line
                axisPoint = VectorCalcs.CreatePoint(componentTransform, tempArray); // point transformed to global space
            }
            else if (objType == (int)swSelectType_e.swSelDATUMPLANES)
            {
                MathVector tempVector = RobotInfo.mathUtil.CreateVector(new double[] { 0, 0, 1 });
                MathPoint tempPoint = RobotInfo.mathUtil.CreatePoint(new double[] { 0, 0, 0 });
                MathTransform tempTransform = ((IRefPlane)((IFeature)obj).GetSpecificFeature2()).Transform;
                axisVector = tempVector.MultiplyTransform(tempTransform).MultiplyTransform(componentTransform);
                axisPoint = tempPoint.MultiplyTransform(tempTransform).MultiplyTransform(componentTransform);
            }
            else
            {
                axisPoint = null;
                axisVector = null;
            }
        }

        /// <summary>
        /// Draws the preview triad for the origin
        /// </summary>
        private void DrawOriginPreview()
        {
            if (!inPropertyPage || groupBox.Expanded == false)
                return;
            MathVector xAxisVector = RobotInfo.mathUtil.CreateVector(new double[]{1,0,0});
            MathVector yAxisVector = RobotInfo.mathUtil.CreateVector(new double[]{0,1,0});
            MathVector zAxisVector = RobotInfo.mathUtil.CreateVector(new double[]{0,0,1});
            if (swManip == null)
            {
                swManip = modelDoc.ModelViewManager.CreateManipulator((int)swManipulatorType_e.swTriadManipulator, this);
                originTriad = swManip.GetSpecificManipulator();
            }
                
            if (xVector != null)
            {
                originTriad.XAxis = xVector;
                if (yVector != null)
                {
                    originTriad.YAxis = yVector;
                    originTriad.ZAxis = xVector.Cross(yVector).Normalise();
                }
                else
                {
                    MathVector tempAxis = zAxisVector.Cross(xVector);
                    if(tempAxis.GetLength() < VectorCalcs.errorVal)
                        tempAxis = yAxisVector;
                    else
                        tempAxis = tempAxis.Normalise();
                    originTriad.YAxis = tempAxis;
                    originTriad.ZAxis = xVector.Cross(tempAxis).Normalise();
                }
            }
            else
            {
                originTriad.XAxis = xAxisVector;
                originTriad.YAxis = yAxisVector;
                originTriad.ZAxis = zAxisVector;
            }

            

            originTriad.Origin = RobotInfo.mathUtil.CreatePoint(OriginValues.Point);
            originTriad.UpdateScale(80);
            originTriad.DoNotShow = (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYPlane |
                (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowXYRING |
                (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZPlane |
                (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowYZRING |
                (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXPlane |
                (int)swTriadManipulatorDoNotShow_e.swTriadManipulatorDoNotShowZXRING;

            originTriad.UpdatePosition();
            if (!swManip.Visible)
            {
                swManip.Show(modelDoc);
            }


        }

        /// <summary>
        /// Clears the origin previews
        /// </summary>
        public void ClearOriginPreview()
        {
            if (swManip != null)
            {
                swManip.Visible = false;
                swManip.Remove();
                swManip = null;
                originTriad = null;
            }
            
        }

        /// <summary>
        /// Gets a 3d point that is the location of the given object
        /// </summary>
        /// <param name="obj">The entity that should have the point found of. Must be a Vertex or RefPoint</param>
        /// <returns>double array of the coordinates</returns>
        protected double[] GetPointFromEntity(object obj)
        {
            
            MathTransform componentTransform = null;
            if ((Component2)((IEntity)obj).GetComponent() != null)
            {
                componentTransform = ((Component2)((IEntity)obj).GetComponent()).Transform2; //stores the component transform if there is one. this will be used to convert the edge vectors to the global coordinate system
            }
            if (obj is IVertex)
            {
                MathPoint vertex = VectorCalcs.CreatePoint(componentTransform, ((IVertex)obj).GetPoint());
                return vertex.ArrayData;
            }
            else if (obj is IFeature)
            {
                IRefPoint tempObject = ((IFeature)obj).GetSpecificFeature2();
                MathPoint point;
                if (componentTransform != null)//if the point is part of a sub component, it is transformed to 3d space
                {
                    point = tempObject.GetRefPoint().MultiplyTransform(componentTransform);
                }
                else
                {
                    point = tempObject.GetRefPoint();
                }
                return point.ArrayData;
            }
            return null;
        }

        /// <summary>
        /// Updates the location of the origin point
        /// </summary>
        public virtual void UpdateOriginPoint()
        {
            if (AlignToXaxis)
            {
                xVector = ((AxialJoint)this).GetAxisVector();
                DrawOriginPreview();
            }
                
        }


        #region Drag Handlers (unused)
        public bool OnDelete(object pManipulator)
        {
            throw new NotImplementedException();
        }

        public void OnDirectionFlipped(object pManipulator)
        {
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
