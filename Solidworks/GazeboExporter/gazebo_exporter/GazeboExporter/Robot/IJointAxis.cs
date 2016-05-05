using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Storage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using GazeboExporter.Export;
using GazeboExporter.UI;
using SolidWorks.Interop.swpublished;
using System.Xml;
using System.Windows.Media.Media3D;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Delagate to cause the origin to be recalculated whenever the axis changes
    /// </summary>
    public delegate void ReCalcOrigin();

    /// <summary>
    /// Abstract class to represent the axis of movement in a joint
    /// </summary>
    public abstract class IJointAxis : ButtonObserver, SelectionObserver
    {
        #region Fields and Properties
        //The storage model that this link is saved with
        protected readonly StorageModel swData;

        //Storage path of this joint in the StorageModel
        protected readonly String path;
        //Allows access to this joint's model document
        protected readonly ModelDoc2 modelDoc;

        protected readonly SldWorks swApp;

        public readonly Joint owner;

        protected readonly RobotModel robot;

        protected readonly MathUtility mathUtil;

        protected MathVector[] limitVectors;

        protected const double errorVal = .0000001;

        protected const double DefaultArrowLength = .025;

        public ReCalcOrigin CalcOriginHandler;


        /// <summary>
        /// The axis that the joint is on
        /// </summary>
        public object Axis
        {
            get
            {
                return swData.GetObject(path + "/axis");
            }
            set
            {
                
                
                if (value != null)
                {
                    object oldAxis = Axis;
                    swData.SetObject(path + "/axis", value);
                    MathVector oldVect = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });
                    CalcAxisVector();
                    MathVector newVect = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });
                    if (!UseCustomMovementLimits && !VectorCalcs.IsParallel(oldVect, newVect))
                        ClearLimits();

                    if (CalcOriginHandler != null)
                    {
                        CalcOriginHandler();
                    }
                    if(!value.Equals(oldAxis))
                        DrawAxisPreview();
                }
                else
                {
                    if(!UseCustomMovementLimits)
                        ClearLimits();
                    swData.SetObject(path + "/axis", value);
                }
                
                    

            }
        }

        /// <summary>
        /// true if the axis of movment/rotation should be flipped
        /// </summary>
        public bool AxisIsFlipped
        {
            get
            {
                return swData.GetDouble(path + "/AxisIsFlipped") == 1;
            }
            set
            {
                if (value != AxisIsFlipped)
                {
                    AxisX = -AxisX;
                    AxisY = -AxisY;
                    AxisZ = -AxisZ;
                }
                if (CalcOriginHandler != null)
                {
                    CalcOriginHandler();
                }
                swData.SetDouble(path + "/AxisIsFlipped", value ? 1 : 0);
            }
        }

        /// <summary>
        /// Whether the joint will use custom movment limits
        /// </summary>
        public bool UseCustomMovementLimits
        {
            get
            {
                return swData.GetDouble(path + "/useCustomMovments") == 1;
            }
            set
            {
                swData.SetDouble(path + "/useCustomMovments", value ? 1 : 0);
            }
        }

        #region Limits
        /// <summary>
        /// Lower limit of the joint (m or rad)
        /// This is the distance that the link can travel backwards. Should be a negative number
        /// </summary>
        public double LowerLimit
        {
            get
            {
                return swData.GetDouble(path + "/lowerLimit");
            }
            set
            {
                if (value > 0)
                {
                    swData.SetDouble(path + "/lowerLimit", 0);
                }
                else
                {
                    swData.SetDouble(path + "/lowerLimit", value);
                }

            }
        }


        /// <summary>
        /// Upper limit of the joint (m or rad)
        /// This is the distance that the link can travel forwards
        /// </summary>
        public double UpperLimit
        {
            get
            {
                return swData.GetDouble(path + "/upperLimit");
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/upperLimit" , 0);
                }
                else
                {
                    swData.SetDouble(path + "/upperLimit", value);
                }

            }
        }


        /// <summary>
        /// The point on the link that will be used to define its lower limit
        /// </summary>
        public object LowerLimitEdge
        {
            get
            {
                return swData.GetObject(path + "/lowerLimitEdge");
            }
            set
            {
                swData.SetObject(path + "/lowerLimitEdge", value);
            }
        }

        /// <summary>
        /// A point not on the link that the lowerLinkEdge will reach at its furthest extent
        /// </summary>
        public object LowerLimitStop
        {
            get
            {
                return swData.GetObject(path + "/lowerLimitStop");
            }
            set
            {
                swData.SetObject(path + "/lowerLimitStop", value);
            }
        }

        /// <summary>
        /// The point on the link that will be used to define its upper limit
        /// </summary>
        public object UpperLimitEdge
        {
            get
            {
                return swData.GetObject(path + "/upperLimitEdge");
            }
            set
            {
                swData.SetObject(path + "/upperLimitEdge", value);
            }
        }

        /// <summary>
        /// A point not on the link that the upperLinkEdge will reach at its furthest extent
        /// </summary>
        public object UpperLimitStop
        {
            get
            {
                return swData.GetObject(path + "/upperLimitStop");
            }
            set
            {
                swData.SetObject(path + "/upperLimitStop", value);
            }
        }
        #endregion

        /// <summary>
        /// Effort limit of the joint (N or Nm)
        /// </summary>
        public double EffortLimit
        {
            get
            {
                return swData.GetDouble(path + "/effortLimit");
            }
            set
            {
                swData.SetDouble(path + "/effortLimit", value);
            }
        }

        /// <summary>
        /// The damping that will be in the joint
        /// </summary>
        public double Damping
        {
            get
            {
                return swData.GetDouble(path + "/damping");
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/damping", 0);
                }
                else
                {
                    swData.SetDouble(path + "/damping", value);
                }

            }
        }

        /// <summary>
        /// The friction in the joint
        /// </summary>
        public double Friction
        {
            get
            {
                return swData.GetDouble(path + "/friction");
            }
            set
            {
                if (value < 0)
                {
                    swData.SetDouble(path + "/friction", 0);
                }
                else
                {
                    swData.SetDouble(path + "/friction", value);
                }

            }
        }

        /// <summary>
        /// Represents a point on the axis
        /// </summary>
        public double[] Point { get; private set; }


        //Axis of motion
        public double AxisX { get; private set; }
        public double AxisY { get; private set; }
        public double AxisZ { get; private set; }


        protected Dictionary<int, PropertyManagerPageSelectionbox> SelBoxes;
        protected Dictionary<int, PropertyManagerPageBitmapButton> Buttons;

        protected int[] LimitSelBoxIds;//in the order lowerLimitStop,lowerLimitEdge,upperLimitStop,upperLimitEdge.
        protected int[] ButtonIds;//currently only has 1 button, but leaves room for more
        protected int AxisSelBoxId;//Id of the axis SelectionBox

        protected List<object> PMPgroups = new List<object>();//just used to keep groups from being garbage collected
        protected List<object> labels = new List<object>();
        #endregion

        /// <summary>
        /// Constructs a new IJointAxis
        /// </summary>
        /// <param name="path">Path to this joint's storage</param>
        /// <param name="current">The joint that this axis is contained in</param>
        public IJointAxis(string path, Joint current)
        {
            this.swApp = RobotInfo.SwApp;
            this.modelDoc = RobotInfo.ModelDoc;
            this.swData = RobotInfo.SwData;
            this.path = path;
            this.owner = current;
            this.robot = RobotInfo.Robot;


            if (EffortLimit == 0)
            {
                this.EffortLimit = 1;
            }

            if (Axis != null)
            {
                CalcAxisVector();
            }
            mathUtil = (MathUtility)swApp.GetMathUtility();
        }

        /// <summary>
        /// Calculates the axis vector and a point on the axis line and fills in the variables
        /// </summary>
        private void CalcAxisVector()
        {
            object Axis = this.Axis;
            if (Axis == null)
                return;
            
            MathVector axisVector;
            double[] points = { 0, 0, 0 };
            IMathUtility matUtil = RobotInfo.mathUtil;
            MathTransform compTransform = null;
            
            Component2 axisComp = ((IEntity)Axis).GetComponent();
            if ( axisComp != null)
                compTransform = axisComp.Transform2;
            if (Axis is IFeature)
            {
                IRefAxis axis = ((IFeature)Axis).GetSpecificFeature2();
                points = axis.GetRefAxisParams();
                double[] dispVector = { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                if (compTransform != null)
                {
                    axisVector = matUtil.CreateVector(dispVector).MultiplyTransform(compTransform).Normalise();
                    points = matUtil.CreatePoint(new double[] { points[0], points[1], points[2] }).MultiplyTransform(compTransform).ArrayData;
                }
                else
                {
                    axisVector = matUtil.CreateVector(dispVector).Normalise();
                    points = points.Take(3).ToArray();
                }
            }
            else if (Axis is IEdge)
            {
                IEdge axis = (IEdge)Axis;
                double[] startPoint = axis.GetStartVertex().GetPoint();
                double[] endPoint = axis.GetEndVertex().GetPoint();
                double[] dispVector =  { endPoint[0] - startPoint[0], endPoint[1] - startPoint[1], endPoint[2] - startPoint[2] };
                axisVector = matUtil.CreateVector(dispVector).MultiplyTransform(compTransform).Normalise();
                points = matUtil.CreatePoint(startPoint).MultiplyTransform(compTransform).ArrayData;
            }
            else
            {
                axisVector = matUtil.CreateVector(new double[] { 1, 0, 0 }).Normalise();
            }
            double[] tempArr = axisVector.ArrayData;

            Point = points;
            if (AxisIsFlipped)
            {
                AxisX = -tempArr[0];
                AxisY = -tempArr[1];
                AxisZ = -tempArr[2];
            }
            else
            {
                AxisX = tempArr[0];
                AxisY = tempArr[1];
                AxisZ = tempArr[2];
            }
        }

        /// <summary>
        /// Clears all limits for this axis
        /// </summary>
        public virtual void ClearLimits()
        {
            LowerLimitEdge = null;
            LowerLimitStop = null;
            UpperLimitEdge = null;
            UpperLimitStop = null;

            LowerLimit = 0;
            UpperLimit = 0;

            SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;
            

            int mark = 0;
            if(LimitSelBoxIds != null)
                foreach(int index in LimitSelBoxIds)
                {
                    mark = SelBoxes[index].Mark;
                    int numSelections = swSelMgr.GetSelectedObjectCount2(mark);
                    for (int i = 1; i <= numSelections; i++)
                    {
                        swSelMgr.DeSelect2(i, mark);
                    }
                }
            
            
            
        }

        /// <summary>
        /// Determnines if the inputed selection is a linear axis
        /// </summary>
        /// <param name="selection">The selection to be checked</param>
        /// <returns>True of the axis is linear</returns>
        public static bool IsLinearAxis(object selection)
        {
            if (selection is IEdge)
            {
                if (((IEdge)selection).GetCurve().LineParams is DBNull)
                    return false;
                else
                    return true;
            }
            else if (selection is IFace)
            {
                MathVector normal = RobotInfo.mathUtil.CreateVector(((IFace)selection).Normal);
                if ( normal.GetLength() < VectorCalcs.errorVal)
                    return false;
                else
                    return true;
            }
            else if (selection is IFeature)
            {
                if (((IFeature)selection).GetSpecificFeature2() is IRefAxis)
                    return true;
                else if (((IFeature)selection).GetSpecificFeature2() is IRefPlane)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Verifyies the joint for export
        /// </summary>
        /// <param name="axisName">THe name of the axis, should be in form "jointName Axis x"</param>
        /// <param name="log"></param>
        public virtual void Verify(String axisName, ProgressLogger log)
        {
            CalcAxisVector();
            RobotInfo.WriteToLogFile("Axis Vector Calculated");
            CalcLimits(-1,null);
            RobotInfo.WriteToLogFile("Limits Calculated");
            if (Axis == null)
                log.WriteError("No axis defined in joint " + axisName);
            
            if (Friction == 0)
                log.WriteWarning("No friction in joint " + axisName);
            if (Damping == 0)
                log.WriteWarning("No damping in joint " + axisName);
        }

        /// <summary>
        /// Gets the limits Panel for this axis
        /// </summary>
        /// <returns>The limits panel</returns>
        public UserControl GetLimitsPanel()
        {
            return new AxialJointLimitsProperties(this);
        }

        /// <summary>
        /// Gets the physical properties for this axis
        /// </summary>
        /// <returns>The properties panel</returns>
        public UserControl GetPropertiesPanel()
        {
            return new SingleAxisJointPhysicalProperties(this);
        }

        /// <summary>
        /// Gets a physical properties page for this page and a second axis
        /// </summary>
        /// <param name="axis">The second axis oin the page</param>
        /// <returns>The newly created properties panel</returns>
        public UserControl GetPropertiesPanel(IJointAxis axis)
        {
            return new DoubleAxisJointPhysicalProperties(new IJointAxis[] { this, axis });
        }

        /// <summary>
        /// Creates an error message popup at the given selection box
        /// </summary>
        /// <param name="Id">Id of the selection box</param>
        /// <param name="title">The title of the error msg</param>
        /// <param name="msg">The error msg</param>
        public void DisplaySelectionboxError(int Id,string title, string msg)
        {
            if (SelBoxes.ContainsKey(Id))
            {
                ((PropertyManagerPageControl)SelBoxes[Id]).ShowBubbleTooltip(title, msg, null);
            }
        }

        /// <summary>
        /// Adds the axis selections to the page
        /// </summary>
        /// <param name="Id">Starting ID for the box. Will be incremented to the next avalible ID after</param>
        /// <param name="mark">Starting mark to be used, will be incremented to next avalible mark afterwards</param>
        /// <param name="page">Joint page to be added to</param>
        /// <param name="refAxisSelBoxId">The Id of the axis selection box</param>
        public void AddAxisToPage(ref int Id, ref int mark, JointPMPage page, ref int refAxisSelBoxId)
        {

            SelBoxes = new Dictionary<int, PropertyManagerPageSelectionbox>();
            Buttons = new Dictionary<int, PropertyManagerPageBitmapButton>();
            labels = new List<object>();
            ButtonIds = new int[1];


            //Helper variables for setup options
            //control type
            short labelCT = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            short selectionboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            short checkboxCT = (int)swPropertyManagerPageControlType_e.swControlType_CheckableBitmapButton;
            // align
            short leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            short shiftedAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
            // options
            int groupboxOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible; // for group box
            int controlOptions = (int)swAddControlOptions_e.swControlOptions_Enabled |
                                 (int)swAddControlOptions_e.swControlOptions_Visible; // for controls
            int[] linefilter = { (int)swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES };

            //Setup and create the joint properties group
            PropertyManagerPageGroup jointPropertiesGroup = (PropertyManagerPageGroup)page.page.AddGroupBox(Id++, "Joint Properties", groupboxOptions);

            PMPgroups.Add(jointPropertiesGroup);
            //Setup and create the label for the joint axis 1 selection box
            PropertyManagerPageLabel jointAxis1Label = (PropertyManagerPageLabel)jointPropertiesGroup.AddControl(Id++, labelCT, "Axis", shiftedAlign, controlOptions, "Select an axis for this joint");
            labels.Add(jointAxis1Label);
            //Setup and create the joint axis 1 selectionbox
            PropertyManagerPageSelectionbox jointAxis1Selectionbox = (PropertyManagerPageSelectionbox)jointPropertiesGroup.AddControl(Id, selectionboxCT, "Axis 1", shiftedAlign, controlOptions, "Use this box to select the axis of movment of this joint");
            jointAxis1Selectionbox.AllowSelectInMultipleBoxes = false;
            jointAxis1Selectionbox.SingleEntityOnly = true;
            jointAxis1Selectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem1);
            jointAxis1Selectionbox.Mark = mark;

            jointAxis1Selectionbox.SetSelectionFilters(linefilter);
            SelectionMgr selMgr = modelDoc.SelectionManager;
            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem1;

            if (Axis != null)
            {
                SelectData axisData = selMgr.CreateSelectData();
                axisData.Mark = mark;
                ((IEntity)Axis).Select4(true, axisData);
            }
            page.PrevSelectId = Id;
            

            SelBoxes.Add(Id, jointAxis1Selectionbox);
            AxisSelBoxId = Id;
            refAxisSelBoxId = Id;
            page.SelectionObservers.Add(this);
            mark = mark << 1;
            Id++;

            //Setup and create the axis flip checkbox
            PropertyManagerPageBitmapButton 
                flipAxisDirectionButton = (PropertyManagerPageBitmapButton)jointPropertiesGroup.AddControl(Id, checkboxCT, "flipAxisDirection", leftAlign, controlOptions, "Check to flip the direction of movement for this joint");
            flipAxisDirectionButton.Checked = AxisIsFlipped;
            flipAxisDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);

            ((PropertyManagerPageControl)flipAxisDirectionButton).Top = 20;
            ((PropertyManagerPageControl)jointAxis1Selectionbox).Top = 20;

            Buttons.Add(Id, flipAxisDirectionButton);
            ButtonIds[0] = Id;
            page.ButtonObservers.Add(this);
            Id++;
            LimitSelBoxIds = null;
        }

        /// <summary>
        /// Adds the limit Selection Boxes to the page
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mark"></param>
        /// <param name="page"></param>
        public virtual void AddLimitsToPage(ref int Id, ref int mark, JointPMPage page)
        {
            LimitSelBoxIds = new int[4];
            short labelCT = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            short selectionboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            // align
            short leftAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_LeftEdge;
            // options
            int groupboxOptions = (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Expanded |
                                  (int)swAddGroupBoxOptions_e.swGroupBoxOptions_Visible; // for group box
            int controlOptions = (int)swAddControlOptions_e.swControlOptions_Enabled |
                                 (int)swAddControlOptions_e.swControlOptions_Visible; // for controls
            int[] allfilter = new int[] { (int)swSelectType_e.swSelDATUMAXES, (int)swSelectType_e.swSelEDGES, (int)swSelectType_e.swSelFACES, (int)swSelectType_e.swSelVERTICES, (int)swSelectType_e.swSelDATUMPLANES, (int)swSelectType_e.swSelDATUMPOINTS };

            PropertyManagerPageGroup jointMovementLimitsGroup = (PropertyManagerPageGroup)page.page.AddGroupBox(Id++, "Joint Movement Limits", groupboxOptions);

            PMPgroups.Add(jointMovementLimitsGroup);
            //Setup and create joint lower edgelabel
            PropertyManagerPageLabel jointLinkLowerEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Lower Link Edge", leftAlign, controlOptions, "Select a lower edge for this joint");
            labels.Add(jointLinkLowerEdgeLabel);

            //setup and create joint lower edge selection box
            PropertyManagerPageSelectionbox jointLinkLowerEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Lower Link Edge", leftAlign, controlOptions, "Use this box to select the lower edge of the link");
            jointLinkLowerEdgeSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLinkLowerEdgeSelectionbox.SingleEntityOnly = true;
            jointLinkLowerEdgeSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointLinkLowerEdgeSelectionbox.Mark = mark;
            jointLinkLowerEdgeSelectionbox.SetSelectionFilters(allfilter);

            SelectionMgr selMgr = modelDoc.SelectionManager;
            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2;

            if (LowerLimitEdge != null)
            {
                SelectData limitData = selMgr.CreateSelectData();
                limitData.Mark = mark;
                ((IEntity)LowerLimitEdge).Select4(true, limitData);
            }

            SelBoxes.Add(Id, jointLinkLowerEdgeSelectionbox);
            LimitSelBoxIds[1] = Id;
            mark = mark << 1;
            Id++;

            //Setup and create joint lower limit stop label
            PropertyManagerPageLabel jointLowerLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Lower Motion Limit", leftAlign, controlOptions, "Select a lower limit for this joint");
            labels.Add(jointLowerLimitStopLabel);

            //setup and create joint lower limit selection box
            PropertyManagerPageSelectionbox jointLowerLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "LowerMotionLimit", leftAlign, controlOptions, "Use this box to select the lower limit of the link");
            jointLowerLimitStopSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLowerLimitStopSelectionbox.SingleEntityOnly = true;
            jointLowerLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointLowerLimitStopSelectionbox.Mark = mark;
            jointLowerLimitStopSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3;
            if (LowerLimitStop != null)
            {
                SelectData limitData = selMgr.CreateSelectData();
                limitData.Mark = mark;
                ((IEntity)LowerLimitStop).Select4(true, limitData);
            }
            SelBoxes.Add(Id, jointLowerLimitStopSelectionbox);
            LimitSelBoxIds[0] = Id;
            mark = mark << 1;
            Id++;

            //Setup and create joint upper edgelabel
            PropertyManagerPageLabel jointLinkUpperEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Upper Link Edge", leftAlign, controlOptions, "Select a Upper edge for this joint");
            labels.Add(jointLinkUpperEdgeLabel);

            //setup and create joint Upper edge selection box
            PropertyManagerPageSelectionbox jointLinkUpperEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Upper Link Edge", leftAlign, controlOptions, "Use this box to select the upper edge of the link");
            jointLinkUpperEdgeSelectionbox.AllowSelectInMultipleBoxes = false;
            jointLinkUpperEdgeSelectionbox.SingleEntityOnly = true;
            jointLinkUpperEdgeSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointLinkUpperEdgeSelectionbox.Mark = mark;
            jointLinkUpperEdgeSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2;
            if (UpperLimitEdge != null)
            {
                SelectData limitData = selMgr.CreateSelectData();
                limitData.Mark = mark;
                ((IEntity)UpperLimitEdge).Select4(true, limitData);
            }
            SelBoxes.Add(Id, jointLinkUpperEdgeSelectionbox);
            LimitSelBoxIds[3] = Id;
            mark = mark << 1;
            Id++;

            //Setup and create joint Upper limit stop label
            PropertyManagerPageLabel jointUpperLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Upper Motion Limit", leftAlign, controlOptions, "Select an Upper limit for this joint");
            labels.Add(jointUpperLimitStopLabel);

            //Setup and create joint upper limit selectionbox
            PropertyManagerPageSelectionbox jointUpperLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Upper Motion Limit", leftAlign, controlOptions, "Use this box to select the upper limit of the joint");
            jointUpperLimitStopSelectionbox.AllowSelectInMultipleBoxes = false;
            jointUpperLimitStopSelectionbox.SingleEntityOnly = true;
            jointUpperLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointUpperLimitStopSelectionbox.Mark = mark;
            jointUpperLimitStopSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3;
            if (UpperLimitStop != null)
            {
                SelectData limitData = selMgr.CreateSelectData();
                limitData.Mark = mark;
                ((IEntity)UpperLimitStop).Select4(true, limitData);
            }
            SelBoxes.Add(Id, jointUpperLimitStopSelectionbox);
            LimitSelBoxIds[2] = Id;
            mark = mark << 1;
            Id++;

            ((PropertyManagerPageControl)Buttons[ButtonIds[0]]).Enabled = false;
        }

        /// <summary>
        /// Called when a button gets pressed in the property page
        /// </summary>
        /// <param name="Id">Id of the Button</param>
        /// <param name="isChecked">The state of the button</param>
        public virtual void ButtonChanged(int Id)
        {
            int index = Array.IndexOf(ButtonIds, Id);
            if (index == -1)
                return;
            switch (index)
            {
                case 0:
                    AxisIsFlipped = Buttons[Id].Checked;
                    DrawAxisPreview();
                    break;
            }
        }

        /// <summary>
        /// Saves all selections in this object
        /// </summary>
        public void SaveSelections()
        {
            int mark;
            if(LimitSelBoxIds != null)
                for (int i = 0;i<LimitSelBoxIds.Length;i++)
                {
                    mark = SelBoxes[LimitSelBoxIds[i]].Mark;
                    if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObjectCount2(mark) > 0)
                        switch (i)
                        {
                            case 0:
                                LowerLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                                break;
                            case 1:
                                LowerLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                                break;
                            case 2:
                                UpperLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                                break;
                            case 3:
                                
                                UpperLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                                break;
                        }
                }
            mark = SelBoxes[AxisSelBoxId].Mark;
            Axis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark); ;
            ClearPreviews();
        }

        /// <summary>
        /// Saves the selection that corresponds to the ID of the selection box
        /// </summary>
        /// <param name="Id">Id of selection box that should be saved</param>
        public void SaveSpecificSelection(int Id)
        {
            if (LimitSelBoxIds != null)
            {
                int index = Array.IndexOf(LimitSelBoxIds, Id);
                if (index != -1)
                {
                    int mark = SelBoxes[Id].Mark;
                    switch (index)
                    {
                        case 0:
                            LowerLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                            break;
                        case 1:
                            LowerLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                            break;
                        case 2:
                            UpperLimitStop = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                            break;
                        case 3:
                            UpperLimitEdge = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark);
                            break;
                    }
                }
            }
            
            if (Id == AxisSelBoxId)
            {
                int mark = SelBoxes[AxisSelBoxId].Mark;
                Axis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, mark); ;
            }
        }

        /// <summary>
        /// Checks if the Selection is valid, and if it is it draws a preview if possible
        /// </summary>
        /// <param name="isValid">Returns true if the selection is valid. False if not a vlid selection or the id is not applicable to this object</param>
        /// <param name="Id">The Id of the selection box</param>
        /// <param name="SelType">The type of the selection</param>
        /// <param name="selection">The selected object</param>
        public void CheckValidSelection(out bool isValid, int Id, int SelType, object selection)
        {
            if (LimitSelBoxIds == null || !LimitSelBoxIds.Contains(Id))
            {
                
                isValid = false;
                return;

            }
            IPropertyManagerPageControl selBox = (IPropertyManagerPageControl)SelBoxes[Id];
            if (IsValidLimitSelection(SelType, selection, selBox))
            {
                isValid = true;
                int index = Array.IndexOf(LimitSelBoxIds, Id);
                CalcLimits(index,selection);
                DrawAxisPreview(index);
            }
            else
            {
                isValid = false;
            }
            
        }

        /// <summary>
        /// Updates the specified preview
        /// </summary>
        /// <param name="Id">The Id of the Selection box that coresponds to the preveiw</param>
        /// <param name="selection">The new selection</param>
        public void UpdatePreviews(int Id, object selection)
        {

        }

        /// <summary>
        /// Called when a selection box is completly cleared
        /// </summary>
        /// <param name="Id">Id of the selection box</param>
        public void NoSelections(int Id)
        {
            if (LimitSelBoxIds == null || !LimitSelBoxIds.Contains(Id))// || (UpperLimit == 0 && LowerLimit == 0))
                return;
            int index = Array.IndexOf(LimitSelBoxIds, Id);
            CalcLimits(index, null);
            if (UpperLimit == 0 && LowerLimit == 0)
                DrawAxisPreview();
            else
                DrawAxisPreview(index);
        }

        /// <summary>
        /// Writes the SDF tags for this Axis
        /// </summary>
        /// <param name="log">logger to use to print messages</param>
        /// <param name="writer">the Writer to use to write the SDF</param>
        /// <param name="index">Index of the axis</param>
        public virtual void WriteSDF(ProgressLogger log, XmlWriter writer,int index)
        {
            if(index == 1)
                writer.WriteStartElement("axis");
            else
                writer.WriteStartElement("axis" + index);
            {
                Vector3D axisVector = new Vector3D(AxisX, AxisY, AxisZ);
                Matrix3D transform = owner.jointSpecifics.OriginValues.OriginTrasform;
                transform.Invert();
                axisVector = Vector3D.Multiply(axisVector, transform);
                string xyzvalue = axisVector.X.ToString() + " " + axisVector.Y.ToString() + " " + axisVector.Z.ToString();
                SDFExporter.writeSDFElement(writer, "xyz", xyzvalue);

                //writeSDFElement(writer, "use_parent_model_frame", );

                //dynamics
                writer.WriteStartElement("dynamics");

                SDFExporter.writeSDFElement(writer, "damping", Damping.ToString());
                SDFExporter.writeSDFElement(writer, "friction", Friction.ToString());

                writer.WriteEndElement();
                WriteLimits(log, writer);
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the SDF dynamics tags for this axis
        /// </summary>
        /// <param name="log">logger to use to print messages</param>
        /// <param name="writer">writer to use to write the SDF</param>
        protected virtual void WriteDynamics(ProgressLogger log, XmlWriter writer)
        {
            writer.WriteStartElement("dynamics");

            SDFExporter.writeSDFElement(writer, "damping", Damping.ToString());
            SDFExporter.writeSDFElement(writer, "friction", Friction.ToString());

            writer.WriteEndElement();
        }

        
        

        #region AbstractMethods
        /// <summary>
        /// Determines if the inputted selection is valid
        /// </summary>
        /// <param name="SelType">The type of the selection<param>
        /// <param name="Selection">The Selected object</param>
        /// <param name="selBox">The Selection box that the selected object is in</param>
        /// <returns>returns true if the object is a valid limit selection</returns>
        public abstract bool IsValidLimitSelection(int SelType, object Selection, IPropertyManagerPageControl selBox);

        /// <summary>
        /// Calculates the limits for the joint
        /// </summary>
        /// <param name="limitObjs">Array of the 4 limit selections in order of lowerLimitStop,lowerLimitEdge,upperLimitStop,upperLimitEdge. If any spot is null the current stored value will be used. if limitObjs is null all current values will be used</param>
        /// <returns>True if the limits where succesfully calculated</returns>
        public abstract bool CalcLimits(int index, object obj);

        /// <summary>
        /// Draws A preview for the axis
        /// </summary>
        public abstract void DrawAxisPreview();

        /// <summary>
        /// Updates only the preview relating to the specified axis
        /// </summary>
        /// <param name="limitIndex"></param>
        public abstract void DrawAxisPreview(int limitIndex, bool update = true);

        /// <summary>
        /// Clears the preview drawing
        /// </summary>
        public abstract void ClearPreviews();

        /// <summary>
        /// Writes the SDF Limits tags for this axis
        /// </summary>
        /// <param name="log">logger to use to print messages</param>
        /// <param name="writer">writer to use to write the SDF</param>
        protected abstract void WriteLimits(ProgressLogger log, XmlWriter writer);



        #endregion

        #region HelperMethods

        /// <summary>
        /// Sets an arrow to the give locations
        /// </summary>
        /// <param name="point">Origin point of the arrow</param>
        /// <param name="vector">Direction Vector of the arrow</param>
        /// <param name="length">length of the arrow</param>
        /// <param name="arrow">The arrow to be edited</param>
        protected void SetArrowAtLoc(double[] point, double[] vector, double length, DragArrowManipulator arrow)
        {
            MathPoint mPoint = mathUtil.CreatePoint(point);
            MathVector mVector = mathUtil.CreateVector(vector);
            SetArrowAtLoc(mPoint, mVector, length, arrow);
        }

        /// <summary>
        /// Sets an arrow to the give locations
        /// </summary>
        /// <param name="point">Origin point of the arrow</param>
        /// <param name="vector">Direction Vector of the arrow</param>
        /// <param name="length">length of the arrow</param>
        /// <param name="arrow">The arrow to be edited</param>
        protected void SetArrowAtLoc(MathPoint point, MathVector vector, double length, DragArrowManipulator arrow)
        {
            arrow.ShowRuler = false;
            arrow.FixedLength = true;
            arrow.ShowOppositeDirection = false;
            arrow.AllowFlip = false;
            arrow.Length = length;
            arrow.Direction = vector;
            arrow.Origin = point;
        }

        

        /// <summary>
        /// Gets the transformation for the axis
        /// </summary>
        /// <param name="axis">The axis that corresponds to the new x-axis</param>
        /// <param name="offset">The translation from the origin</param>
        /// <returns>The transform matrix</returns>
        protected MathTransform GetAxisTransform(double[] axis, double[] offset)
        {
            MathVector xAxis = mathUtil.CreateVector(axis);
            MathVector yAxisOrig = mathUtil.CreateVector(new double[] { 0, 1, 0 });
            MathVector zAxis;
            MathVector yAxis;
            if (VectorCalcs.IsParallel(xAxis, yAxisOrig))
                zAxis = mathUtil.CreateVector(new double[] { 0, 0, 1 });
            else
                zAxis = xAxis.Cross(yAxisOrig).Normalise();
            yAxis = zAxis.Cross(xAxis).Normalise();
            MathVector offsetVect = mathUtil.CreateVector(offset);
            MathTransform t = mathUtil.ComposeTransform(xAxis, yAxis, zAxis, offsetVect, 1);
            return t;
            
        }

        /// <summary>
        /// Gets the transformation for the axis
        /// </summary>
        /// <param name="xaxis">The axis that corresponds to the new x-axis</param>
        /// <param name="zaxis">The axis that corresponds to the new z-axis</param>
        /// <param name="offset">The translation from the origin</param>
        /// <returns>The transform matrix</returns>
        protected MathTransform GetAxisTransform(double[] xaxis, double[] zaxis, double[] offset)
        {
            MathVector xAxis = mathUtil.CreateVector(xaxis);
            MathVector zAxis = mathUtil.CreateVector(zaxis);
            MathVector yAxis = zAxis.Cross(xAxis).Normalise();

            MathVector offsetVect = mathUtil.CreateVector(offset);

            return mathUtil.ComposeTransform(xAxis, yAxis, zAxis, offsetVect, 1);

        }

        

        #endregion

        
    }
}
