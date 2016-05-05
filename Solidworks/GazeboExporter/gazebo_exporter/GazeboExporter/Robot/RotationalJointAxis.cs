using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using GazeboExporter.Export;
using GazeboExporter.UI;
using System.Diagnostics;

namespace GazeboExporter.Robot
{
    public class RotationalJointAxis: IJointAxis,ButtonObserver
    {
        private Body2[] bodies;

        const double DefaultAngle = 270;
        const double DefaultWidth = .02;
        const double DefaultRadius = .1;
        const double DefaultHeadTheta = 10;//angle between the base of the head and the point from the center of the arrow

        public const int DefaultArrow1Color = 0x2B39C0;
        public const int DefaultArrow2Color = 0x60AE27;
        public const int DefaultArrow3Color = 0xFFFFFF;
        public const int DefaultOuterLimitColor = 0xBD005F;//purple to match the sw selection color
        public const int DefaultInnerLimitColor = 0x895CB5;//pink to match the sw selection color

        public int ArrowColor { get; set; }

        private Dictionary<int,PropertyManagerPageBitmapButton> LimitButtons;
        private int[] LimitButtonIds; //order is lower then upper
        private List<object> limitLabels;
        MathVector[] limits;

        /// <summary>
        /// true if the axis is continous
        /// </summary>
        public bool IsContinuous
        {
            get
            {
                return swData.GetDouble(path + "/IsContinuous") == 1;
            }
            set
            {
                swData.SetDouble(path + "/IsContinuous", value ? 1 : 0);
            }
        }

        private DoubleStorageArray FlipVectors;

        public RotationalJointAxis(string path, Joint current) :
            base(path, current)
        {
            FlipVectors = new DoubleStorageArray(swData, path + "/FlipVectors");
            if (FlipVectors.Count == 0)
            {
                for (int i = 0; i < 4; i++)
                    FlipVectors.AddItem(0);
            }

        }

        /// <summary>
        /// Determines if the inputted selection is valid. 
        /// </summary>
        /// <param name="SelType">The type of the selection<param>
        /// <param name="Selection">The Selected object</param>
        /// <param name="selBox">The Selection box that the selected object is in</param>
        /// <returns>returns true if the object is a valid limit selection</returns>
        public override bool IsValidLimitSelection(int SelType, object Selection, IPropertyManagerPageControl selBox)
        {
            if (Axis == null)//if there is no rotation axis then no selections can be valid
            {
                if (((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 2) != null)
                {
                    Axis = ((SelectionMgr)modelDoc.SelectionManager).GetSelectedObject6(1, 2);
                }
                else
                {
                    selBox.ShowBubbleTooltip("Error: no joint axis", "Select the first axis for this joint before selecting other features", "");
                    return false;
                }

            }

            string colinearErrorMsgTitle = "Error: Selection can not be Colinear with joint axis";
            string colinearErrorMsg = "The selected feature must no be colinear to the axis of rotation";
            string perpErrorMsgTitle = "Error: Selection must be perpendicular to joint axis";
            string perpErrorMsg = "The selected feature must be perpindicular to the axis of rotation";

            MathTransform componentTransform = null;
            if ((Component2)((IEntity)Selection).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
            {
                componentTransform = ((Component2)((IEntity)Selection).GetComponent()).Transform2;
            }
            double[] tempAxisNormalVector = { AxisX, AxisY, AxisZ };
            MathVector axisNormalVector = mathUtil.CreateVector(tempAxisNormalVector).Normalise();//creates a vector to represent the aXis of movment
            
            MathVector featureNormalVector = null;
            MathPoint jointOrigin = mathUtil.CreatePoint(Point);
            double[] tempArray;
            switch ((swSelectType_e)SelType)
            {
                case swSelectType_e.swSelDATUMAXES:
                    double[] refAxisPoints = ((IRefAxis)((IFeature)Selection).GetSpecificFeature2()).GetRefAxisParams();
                    tempArray = new double[] { refAxisPoints[3] - refAxisPoints[0], refAxisPoints[4] - refAxisPoints[1], refAxisPoints[5] - refAxisPoints[2] };
                    MathPoint refAxisPoint = VectorCalcs.CreatePoint(componentTransform,refAxisPoints.Take(3).ToArray());
                    featureNormalVector = VectorCalcs.CreateVector(componentTransform,tempArray);
                    if (VectorCalcs.IsParallel(featureNormalVector, axisNormalVector))
                    {
                        if (VectorCalcs.IsPointOnLine(featureNormalVector, refAxisPoint, jointOrigin))
                        {
                            selBox.ShowBubbleTooltip(colinearErrorMsgTitle, colinearErrorMsg, "");
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return true;

                case swSelectType_e.swSelDATUMPLANES:
                    tempArray = new double[] { 0, 0, 1 };//temp vector used to find normal vector of plane
                    MathTransform tempTransform = ((IRefPlane)((IFeature)Selection).GetSpecificFeature2()).Transform; //represents the transform of the plane from being the xy plane
                    MathVector planeNormalVector = mathUtil.CreateVector(tempArray);
                    if (componentTransform != null)//finds normal vecor of plane and transforms it if needed
                    {
                        featureNormalVector = planeNormalVector.MultiplyTransform(tempTransform).MultiplyTransform(componentTransform).Normalise();
                    }
                    else
                    {
                        featureNormalVector = planeNormalVector.MultiplyTransform(tempTransform).Normalise();
                    }
                    if (!VectorCalcs.IsPerpendicular(axisNormalVector,featureNormalVector))
                    {
                        selBox.ShowBubbleTooltip(perpErrorMsgTitle, perpErrorMsg, "");
                        return false;
                    }
                    else
                        return true;

                case swSelectType_e.swSelDATUMPOINTS:
                    MathPoint refPoint = ((IRefPoint)((IFeature)Selection).GetSpecificFeature2()).GetRefPoint();
                    if (componentTransform != null)//if the point is part of a sub component, it is transformed to 3d space
                    {
                        refPoint = refPoint.MultiplyTransform(componentTransform);
                    }
                        
                    if(VectorCalcs.IsPointOnLine(axisNormalVector,jointOrigin,refPoint))
                    {
                        selBox.ShowBubbleTooltip(colinearErrorMsgTitle, colinearErrorMsg, "");
                        return false;
                    }
                    else
                        return true;

                case swSelectType_e.swSelEDGES:
                    object[] faces = ((IEdge)Selection).GetTwoAdjacentFaces2();
                    if (faces[0] == null || faces[1] == null)
                    {
                        break;
                    }
                    MathVector faceVector1 = VectorCalcs.CreateVector(componentTransform,((IFace2)faces[0]).Normal);//gets normal vectors for each bordering face
                    MathVector faceVector2 = VectorCalcs.CreateVector(componentTransform,((IFace2)faces[1]).Normal);

                    double[] edgeCurveParams = null;
                    if (!(((IEdge)Selection).GetCurve().LineParams is DBNull))
                    {
                        edgeCurveParams = ((IEdge)Selection).GetCurve().LineParams;
                    }
                    //check if both adjacent faces are planar. If both are then the edge must be a line
                    if (edgeCurveParams != null)
                    {
                        tempArray = edgeCurveParams.Take(3).ToArray();
                        MathPoint edgePoint = VectorCalcs.CreatePoint(componentTransform,tempArray);
                        tempArray = new double[] { edgeCurveParams[3], edgeCurveParams[4], edgeCurveParams[5] };
                        featureNormalVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector representing the linear edge
                        if (VectorCalcs.IsParallel(featureNormalVector, axisNormalVector))//if the edgevector and the axis are perpindicular it is valid
                        {
                            if(VectorCalcs.IsPointOnLine(featureNormalVector,edgePoint,jointOrigin))
                            {
                                selBox.ShowBubbleTooltip(colinearErrorMsgTitle, colinearErrorMsg, "");
                                return false;
                            }
                            else
                                return true;
                        }
                        else
                            return true;
                    }
                    else
                    {
                        if(faceVector1.GetLength()>0)
                            if(!VectorCalcs.IsPerpendicular(faceVector1,axisNormalVector))
                            {
                                selBox.ShowBubbleTooltip(perpErrorMsgTitle, perpErrorMsg, "");
                                return false;
                            }
                            else
                                return true;
                        if(faceVector2.GetLength() > 0)
                            if (!VectorCalcs.IsPerpendicular(faceVector2, axisNormalVector))
                            {
                                selBox.ShowBubbleTooltip(perpErrorMsgTitle, perpErrorMsg, "");
                                return false;
                            }
                            else
                                return true;

                        return false;
                    }

                case swSelectType_e.swSelFACES:
                    MathVector faceVector = mathUtil.CreateVector(((IFace2)Selection).Normal);
                    if (faceVector.GetLength() < errorVal)//makes sure face is valid
                    {
                        break;
                    }
                    featureNormalVector = (faceVector.MultiplyTransform(componentTransform)).Normalise();
                    if (!VectorCalcs.IsPerpendicular(featureNormalVector, axisNormalVector))
                    {
                        selBox.ShowBubbleTooltip(perpErrorMsgTitle, perpErrorMsg, "");
                        return false;
                    }
                    else
                        return true;

                case swSelectType_e.swSelVERTICES:
                    MathPoint vertPoint = VectorCalcs.CreatePoint(componentTransform, ((Vertex)Selection).GetPoint());
                        
                    if(VectorCalcs.IsPointOnLine(axisNormalVector,jointOrigin,vertPoint))
                    {
                        selBox.ShowBubbleTooltip(colinearErrorMsgTitle, colinearErrorMsg, "");
                        return false;
                    }
                    else
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Calculates the limit values of the axis. 
        /// </summary>
        /// <param name="limitObjs">Array of the 4 limit selections in order of lowerLimitStop,lowerLimitEdge,upperLimitStop,upperLimitEdge. 
        /// If any spot is null the current stored value will be used. if limitObjs is null all current values will be used</param>
        /// <returns>True if the limits where succesfully calculated</returns>
        public override bool CalcLimits(int index,object obj)
        {
            if (Axis == null)
            {
                return false;
            }
            if (UseCustomMovementLimits || IsContinuous)
            {
                return false;
            }
            bool successfulCalculation = true;

            limits = new MathVector[4]; //contains the vectors for each limit axis. for revolute joints the angle is represented and all vectors are normalised, for prismatic all vectors are parallel and the length represents the limit
            object[] limitObjs = { LowerLimitStop, LowerLimitEdge, UpperLimitStop, UpperLimitEdge };
            if (index != -1)
            {
                limitObjs[index] = obj;
            }
            
            MathVector axisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ }); //the vector of the rotation/translation axis
            MathPoint jointOrigin;
            if (owner.jointSpecifics.OriginPt != null)
                jointOrigin = mathUtil.CreatePoint(owner.jointSpecifics.OriginValues.Point);
            else
                jointOrigin = mathUtil.CreatePoint(Point);
            double[] tempArray;

            for (int i = 0; i < limitObjs.Length; i++)
            {
                if (limitObjs[i] == null)//checks that a limit is stored
                {
                    successfulCalculation = false;
                    continue;
                }
                MathTransform componentTransform = null;
                Component2 limComp = (Component2)((IEntity)limitObjs[i]).GetComponent();
                if (limComp != null)
                {
                    componentTransform = limComp.Transform2; //stores the component transform if there is one. this will be used to convert the edge vectors to the global coordinate system
                }
                if (limitObjs[i] is IEdge) //if this limit is an edge
                {
                    object[] faces = ((IEdge)limitObjs[i]).GetTwoAdjacentFaces2();
                    MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);//the normal vectors for both adjacent faces to this edge
                    MathVector faceVector2 = mathUtil.CreateVector(((IFace2)faces[1]).Normal);
                    if (!(((IEdge)limitObjs[i]).GetCurve().LineParams is DBNull)) //if both vectors are non zero, the edge must be a line
                    {
                        MathPoint startPoint = VectorCalcs.CreatePoint(componentTransform, ((IEdge)limitObjs[i]).GetStartVertex().getPoint());
                        MathPoint endPoint = VectorCalcs.CreatePoint(componentTransform, ((IEdge)limitObjs[i]).GetEndVertex().getPoint());
                        MathVector edgeVector = VectorCalcs.GetVectorBetweenPoints(endPoint, startPoint).Normalise();
                        if (VectorCalcs.IsParallel(edgeVector,axisVector))//if the edge is parallel to the axis then the limit vector will be perpendicular to both vectors and intersect both
                        {
                            edgeVector = VectorCalcs.GetVectorBetweenPoints(endPoint, jointOrigin).Normalise();//draws a vector between a point on the edge and a point on the axis
                        }
                        if (VectorCalcs.IsPerpendicular(edgeVector,axisVector))//if the edge vector is perpindicular to the axis, it is the correct vector for the limit
                        {
                            limits[i] = edgeVector.Normalise();
                        }
                        else //otherwise it is made parallel by crossing it with the axis, then crossing that result with the axis again
                        {
                            limits[i] = axisVector.Cross(axisVector.Cross(edgeVector)).Normalise();
                        }
                    }
                    else//if 1 face vector is zero, then the angle to the face that has a non zero normal vector is the limit vector
                    {
                        if (faceVector1.GetLength() > 0)
                        {
                            limits[i] = faceVector1.MultiplyTransform(componentTransform).Cross(axisVector).Normalise();//crosses the face normal vector with the axis vector to get the limit vector, which is perpendicular to both
                        }
                        else
                        {
                            limits[i] = faceVector2.MultiplyTransform(componentTransform).Cross(axisVector).Normalise();
                        }
                        if (AxisIsFlipped)
                            limits[i] = limits[i].Scale(-1);
                    }
                }
                else if (limitObjs[i] is IFace2)//if the limit object is a face
                {
                    limits[i] = mathUtil.CreateVector(((IFace2)limitObjs[i]).Normal).MultiplyTransform(componentTransform).Cross(axisVector).Normalise();//converts the face normal vector to global space, then crosses the face normal vector with the axis vector to get the limit vector, which is perpendicular to both
                    if (AxisIsFlipped)
                        limits[i] = limits[i].Scale(-1);
                }
                else if (limitObjs[i] is IVertex)//if the limit object is a vertex
                {
                    MathPoint vertex = VectorCalcs.CreatePoint(componentTransform,((IVertex)limitObjs[i]).GetPoint());
                    MathVector vertVect = VectorCalcs.GetVectorBetweenPoints(vertex,jointOrigin).Normalise();//creates a vector between the vertex and a point on the axis
                    if (VectorCalcs.IsPerpendicular(vertVect,axisVector))// if the vector to the vertex is perpindicular to the axis then it is the limit vector
                    {
                        limits[i] = vertVect.Normalise();
                    }
                    else //otherwise it is crossed with the axis twice to make it perpindicular to the axis
                    {
                        limits[i] = axisVector.Cross(axisVector.Cross(vertVect)).Normalise();
                    }
                }
                else
                {
                    object tempObject = ((IFeature)limitObjs[i]).GetSpecificFeature2();
                    if (tempObject is IRefAxis)//if the limit is a reference axis
                    {
                        double[] points = ((IRefAxis)tempObject).GetRefAxisParams();
                        tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                        MathVector refAxisVector = VectorCalcs.CreateVector(componentTransform,tempArray);
                        if (VectorCalcs.IsParallel(refAxisVector,axisVector))//if the axis is parallel to the refAxis vector then the limit vector will be between the two axis and perpendicular to both
                        {
                            tempArray = new double[] { points[3],points[4] , points[5] };
                            MathPoint tempPoint = VectorCalcs.CreatePoint(componentTransform, tempArray);
                            refAxisVector = VectorCalcs.GetVectorBetweenPoints(tempPoint, jointOrigin).Normalise();
                        }
                        if (VectorCalcs.IsPerpendicular(refAxisVector,axisVector))//if the refAxisVector is perpendicular then it is the limit vector, otherwise it is made perpendicular by crossing it twice with the axis vector
                        {
                            limits[i] = refAxisVector;
                        }
                        else
                        {
                            limits[i] = axisVector.Cross(axisVector.Cross(refAxisVector)).Normalise();
                        }
                    }
                    else if (tempObject is IRefPlane)//if the limit is a reference plane
                    {
                        MathVector planeNormal = mathUtil.CreateVector(new double[] { 0, 0, 1 });//used to find the normal vector of the plane
                        if (componentTransform != null)//if the plane is part of a subcomponent, it is converted to global space. Then, its normal vector is found and crossed with the axis to find the limit vector, which is perpendicualr to both
                        {
                            limits[i] = planeNormal.MultiplyTransform(((IRefPlane)tempObject).Transform).MultiplyTransform(componentTransform).Cross(axisVector).Normalise();
                        }
                        else
                        {
                            limits[i] = planeNormal.MultiplyTransform(((IRefPlane)tempObject).Transform).Cross(axisVector).Normalise();
                        }
                        if (AxisIsFlipped)
                            limits[i] = limits[i].Scale(-1);
                    }
                    else if (tempObject is IRefPoint)//if the limit is a reference point
                    {
                        MathPoint point;
                        if (componentTransform != null)//if the point is part of a sub component, it is transformed to 3d space
                        {
                            point = ((IRefPoint)tempObject).GetRefPoint().MultiplyTransform(componentTransform);
                        }
                        else
                        {
                            point = ((IRefPoint)tempObject).GetRefPoint();
                        }
                        MathVector refPointVector = VectorCalcs.GetVectorBetweenPoints(point, jointOrigin);// a vector is created between the point and a point on the axis
                        if (VectorCalcs.IsPerpendicular(refPointVector,axisVector))//if the point vector is perpendicular to the axis it is the limit vector
                        {
                            limits[i] = refPointVector.Normalise();
                        }
                        else//otherwise it is made perpendicular by crossing it with the axis twice.
                        {
                            limits[i] = axisVector.Cross(axisVector.Cross(refPointVector)).Normalise();
                        }

                    }
                }
                double t = FlipVectors[i];
                if (FlipVectors[i] == 1)
                    limits[i] = limits[i].Scale(-1);
                //System.Diagnostics.Debug.WriteLine((double)limits[i].ArrayData[0] + " " + (double)limits[i].ArrayData[1] + " " + (double)limits[i].ArrayData[2]);
            }

            if (limits[0] != null && limits[1] != null)
            {
                double dotProduct = limits[0].Dot(limits[1]);
                double lowerAngle;
                if (dotProduct >= 1)
                    lowerAngle = 0;
                else if (dotProduct <= -1)
                    lowerAngle = Math.PI;
                else
                    lowerAngle = Math.Acos(dotProduct);

                if (VectorCalcs.IsSameDirection(limits[1].Cross(limits[0]), axisVector))
                    lowerAngle = 2 * Math.PI - lowerAngle;
                LowerLimit = -lowerAngle;
            }
            else
                LowerLimit = 0;

            if (limits[2] != null && limits[3] != null)
            {
                double dotProduct = limits[2].Dot(limits[3]);
                double upperAngle;
                if (dotProduct >= 1)
                    upperAngle = 0;
                else if (dotProduct <= -1)
                    upperAngle = Math.PI;
                else
                    upperAngle = Math.Acos(dotProduct);

                if (VectorCalcs.IsSameDirection(limits[2].Cross(limits[3]), axisVector))
                    upperAngle = 2 * Math.PI - upperAngle;

                UpperLimit = upperAngle;
            }
            else
                UpperLimit = 0;
            return successfulCalculation;
        }

        /// <summary>
        /// Adds the limit Selection Boxes to the page
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="mark"></param>
        /// <param name="page"></param>
        public override void AddLimitsToPage(ref int Id, ref int mark, JointPMPage page)
        {
            LimitSelBoxIds = new int[4];
            LimitButtonIds = new int[4];
            LimitButtons = new Dictionary<int, PropertyManagerPageBitmapButton>();
            limitLabels = new List<object>();

            List<SelectData> marks = new List<SelectData>();
            List<object> selectionObjects = new List<object>();

            short labelCT = (int)swPropertyManagerPageControlType_e.swControlType_Label;
            short selectionboxCT = (int)swPropertyManagerPageControlType_e.swControlType_Selectionbox;
            short checkboxCT = (int)swPropertyManagerPageControlType_e.swControlType_CheckableBitmapButton;
            // align
            short indentAlign = (int)swPropertyManagerPageControlLeftAlign_e.swControlAlign_Indent;
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
            PropertyManagerPageLabel jointLinkLowerEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Lower Link Edge", indentAlign, controlOptions, "Select a lower edge for this joint");
            limitLabels.Add(jointLinkLowerEdgeLabel);

            //setup and create joint lower edge selection box
            PropertyManagerPageSelectionbox jointLinkLowerEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Lower Link Edge", indentAlign, controlOptions, "Use this box to select the lower edge of the link");
            jointLinkLowerEdgeSelectionbox.AllowSelectInMultipleBoxes = true;
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
                selectionObjects.Add(LowerLimitEdge);
                marks.Add(limitData);
                //((IEntity)LowerLimitEdge).Select4(true, limitData);
            }

            SelBoxes.Add(Id, jointLinkLowerEdgeSelectionbox);
            LimitSelBoxIds[1] = Id;
            mark = mark << 1;
            Id++;

            PropertyManagerPageBitmapButton
                flipLowerLimitEdgeDirectionButton = (PropertyManagerPageBitmapButton)jointMovementLimitsGroup.AddControl(Id, checkboxCT, "flipAxisDirection", leftAlign, controlOptions, "Check to flip the direction of movement for this joint");
            flipLowerLimitEdgeDirectionButton.Checked = FlipVectors[1]==1;
            flipLowerLimitEdgeDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            LimitButtons.Add(Id, flipLowerLimitEdgeDirectionButton);
            LimitButtonIds[1] = Id;

            ((PropertyManagerPageControl)flipLowerLimitEdgeDirectionButton).Top = 20;
            ((PropertyManagerPageControl)jointLinkLowerEdgeSelectionbox).Top = 20;
            Id++;

            //Setup and create joint lower limit stop label
            PropertyManagerPageLabel jointLowerLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Lower Motion Limit", indentAlign, controlOptions, "Select a lower limit for this joint");
            limitLabels.Add(jointLowerLimitStopLabel);
            //setup and create joint lower limit selection box
            PropertyManagerPageSelectionbox jointLowerLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "LowerMotionLimit", indentAlign, controlOptions, "Use this box to select the lower limit of the link");
            jointLowerLimitStopSelectionbox.AllowSelectInMultipleBoxes = true;
            jointLowerLimitStopSelectionbox.SingleEntityOnly = true;
            jointLowerLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointLowerLimitStopSelectionbox.Mark = mark;
            jointLowerLimitStopSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3;
            if (LowerLimitStop != null)
            {
                if (selectionObjects.Contains(LowerLimitStop))
                {
                    int index = selectionObjects.IndexOf(LowerLimitStop);
                    marks[index].Mark |= mark;
                }
                else
                {
                    SelectData limitData = selMgr.CreateSelectData();
                    limitData.Mark = mark;
                    selectionObjects.Add(LowerLimitStop);
                    marks.Add(limitData);
                }

                //((IEntity)LowerLimitStop).Select4(true, limitData);
            }
            SelBoxes.Add(Id, jointLowerLimitStopSelectionbox);
            LimitSelBoxIds[0] = Id;
            mark = mark << 1;
            Id++;

            PropertyManagerPageBitmapButton
                flipLowerLimitDirectionButton = (PropertyManagerPageBitmapButton)jointMovementLimitsGroup.AddControl(Id, checkboxCT, "flipAxisDirection", leftAlign, controlOptions, "Check to flip the direction of movement for this joint");
            flipLowerLimitDirectionButton.Checked = FlipVectors[0] == 1; ;
            flipLowerLimitDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            LimitButtons.Add(Id, flipLowerLimitDirectionButton);
            LimitButtonIds[0] = Id;

            ((PropertyManagerPageControl)flipLowerLimitDirectionButton).Top = 120;
            ((PropertyManagerPageControl)jointLowerLimitStopSelectionbox).Top = 120;

            Id++;

            //Setup and create joint upper edgelabel
            PropertyManagerPageLabel jointLinkUpperEdgeLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Upper Link Edge", indentAlign, controlOptions, "Select a Upper edge for this joint");
            limitLabels.Add(jointLinkUpperEdgeLabel);
            //setup and create joint Upper edge selection box
            PropertyManagerPageSelectionbox jointLinkUpperEdgeSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Upper Link Edge", indentAlign, controlOptions, "Use this box to select the upper edge of the link");
            jointLinkUpperEdgeSelectionbox.AllowSelectInMultipleBoxes = true;
            jointLinkUpperEdgeSelectionbox.SingleEntityOnly = true;
            jointLinkUpperEdgeSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2);
            jointLinkUpperEdgeSelectionbox.Mark = mark;
            jointLinkUpperEdgeSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem2;
            if (UpperLimitEdge != null)
            {
                if (selectionObjects.Contains(UpperLimitEdge))
                {
                    int index = selectionObjects.IndexOf(UpperLimitEdge);
                    marks[index].Mark |= mark;
                }
                else
                {
                    SelectData limitData = selMgr.CreateSelectData();
                    limitData.Mark = mark;
                    selectionObjects.Add(UpperLimitEdge);
                    marks.Add(limitData);
                }
            }
            SelBoxes.Add(Id, jointLinkUpperEdgeSelectionbox);
            LimitSelBoxIds[3] = Id;
            mark = mark << 1;
            Id++;

            PropertyManagerPageBitmapButton
                flipUpperLimitEdgeDirectionButton = (PropertyManagerPageBitmapButton)jointMovementLimitsGroup.AddControl(Id, checkboxCT, "flipAxisDirection", leftAlign, controlOptions, "Check to flip the direction of movement for this joint");
            flipUpperLimitEdgeDirectionButton.Checked = FlipVectors[3] == 1; ;
            flipUpperLimitEdgeDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            LimitButtons.Add(Id, flipUpperLimitEdgeDirectionButton);
            LimitButtonIds[3] = Id;

            ((PropertyManagerPageControl)flipUpperLimitEdgeDirectionButton).Top = 220;
            ((PropertyManagerPageControl)jointLinkUpperEdgeSelectionbox).Top = 220;

            //Setup and create joint Upper limit stop label
            PropertyManagerPageLabel jointUpperLimitStopLabel = (PropertyManagerPageLabel)jointMovementLimitsGroup.AddControl(Id++, labelCT, "Upper Motion Limit", indentAlign, controlOptions, "Select an Upper limit for this joint");
            limitLabels.Add(jointUpperLimitStopLabel);
            //Setup and create joint upper limit selectionbox
            PropertyManagerPageSelectionbox jointUpperLimitStopSelectionbox = (PropertyManagerPageSelectionbox)jointMovementLimitsGroup.AddControl(Id, selectionboxCT, "Upper Motion Limit", indentAlign, controlOptions, "Use this box to select the upper limit of the joint");
            jointUpperLimitStopSelectionbox.AllowSelectInMultipleBoxes = true;
            jointUpperLimitStopSelectionbox.SingleEntityOnly = true;
            jointUpperLimitStopSelectionbox.SetSelectionColor(true, (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3);
            jointUpperLimitStopSelectionbox.Mark = mark;
            jointUpperLimitStopSelectionbox.SetSelectionFilters(allfilter);

            selMgr.SelectionColor[mark] = (int)swUserPreferenceIntegerValue_e.swSystemColorsSelectedItem3;
            if (UpperLimitStop != null)
            {
                if (selectionObjects.Contains(UpperLimitStop))
                {
                    int index = selectionObjects.IndexOf(UpperLimitStop);
                    marks[index].Mark |= mark;
                }
                else
                {
                    SelectData limitData = selMgr.CreateSelectData();
                    limitData.Mark = mark;
                    selectionObjects.Add(UpperLimitStop);
                    marks.Add(limitData);
                }
            }
            SelBoxes.Add(Id, jointUpperLimitStopSelectionbox);
            LimitSelBoxIds[2] = Id;
            mark = mark << 1;
            Id++;

            PropertyManagerPageBitmapButton
                flipUpperLimitDirectionButton = (PropertyManagerPageBitmapButton)jointMovementLimitsGroup.AddControl(Id, checkboxCT, "flipAxisDirection", leftAlign, controlOptions, "Check to flip the direction of movement for this joint");
            flipUpperLimitDirectionButton.Checked = FlipVectors[2] == 1; ;
            flipUpperLimitDirectionButton.SetStandardBitmaps((int)swPropertyManagerPageBitmapButtons_e.swBitmapButtonImage_reverse_direction);
            LimitButtons.Add(Id, flipUpperLimitDirectionButton);
            LimitButtonIds[2] = Id;

            ((PropertyManagerPageControl)flipUpperLimitDirectionButton).Top = 320;
            ((PropertyManagerPageControl)jointUpperLimitStopSelectionbox).Top = 320;

            Id++;

            for (int i = 0; i < selectionObjects.Count;i++ )
            {
                ((IEntity)selectionObjects[i]).Select4(true, marks[i]);
            }

            //((PropertyManagerPageControl)Buttons[ButtonIds[0]]).Enabled = false;
        }

        /// <summary>
        /// Draws the preview arrows for the joint
        /// Will update all preview bodies
        /// </summary>
        public override void DrawAxisPreview()
        {
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = false;
            ClearPreviews();
            if (Axis == null)
                return;
            
            MathPoint jointOrigin = mathUtil.CreatePoint(owner.jointSpecifics.OriginValues.Point);
            MathPoint axisPoint = mathUtil.CreatePoint(Point);
            MathVector AxisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });
            if (jointOrigin == null || (!VectorCalcs.IsPointOnLine(AxisVector, axisPoint, jointOrigin) && owner.jointSpecifics.OriginPt == null))
                jointOrigin = axisPoint;
            if (UseCustomMovementLimits || limits == null || limits.All(x => x==null))
            {
                bodies = new Body2[3];
                Body2[] tempBodies = DrawArrowAtLoc(DefaultArrow2Color, AxisVector.ArrayData, jointOrigin.ArrayData, DefaultAngle, DefaultRadius, null);
                bodies[0] = tempBodies[0];
                bodies[1] = tempBodies[1];
                bodies[2] = DrawReferenceLine(ArrowColor,AxisVector.ArrayData, jointOrigin.ArrayData);
            }
            else 
            {
                bodies = new Body2[9];
                bodies[0] = DrawReferenceLine(DefaultArrow1Color,AxisVector.ArrayData, jointOrigin.ArrayData);
                for (int i = 0; i < limits.Length; i++)
                {
                    DrawAxisPreview(i,false);

                }

            }
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = true;

            
           
        }

        /// <summary>
        /// Updates the specified preview part
        /// </summary>
        /// <param name="limitIndex">the index of the limit</param>
        public override void DrawAxisPreview(int limitIndex, bool update = true)
        {
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = false;
            if (Axis == null)
                return;


            Component2 comp = RobotInfo.DispComp;
            MathPoint jointOrigin = mathUtil.CreatePoint(owner.jointSpecifics.OriginValues.Point);
            MathPoint axisPoint = mathUtil.CreatePoint(Point);
            MathVector AxisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });

            if (jointOrigin == null || (!VectorCalcs.IsPointOnLine(AxisVector, axisPoint, jointOrigin)&&owner.jointSpecifics.OriginPt == null))
                jointOrigin = axisPoint;

            if (bodies.Length < 9)
            {
                ClearPreviews();
                bodies = new Body2[9];
                bodies[0] = DrawReferenceLine(DefaultArrow1Color, AxisVector.ArrayData, jointOrigin.ArrayData);
            }

            switch (limitIndex)
            {
                case 0:
                    if(bodies[1] != null)
                        bodies[1].Hide(comp);
                    if (limits[0] != null)
                        bodies[1] = DrawReferenceLine(DefaultOuterLimitColor, limits[0].ArrayData, jointOrigin.ArrayData, DefaultRadius * 2);
                    break;
                case 1:
                    if(bodies[2] != null)
                        bodies[2].Hide(comp);
                    if (limits[1] != null)
                        bodies[2] = DrawReferenceLine(DefaultInnerLimitColor, limits[1].ArrayData, jointOrigin.ArrayData, DefaultRadius * 2);
                    break;
                case 2:
                    if(bodies[5] != null)
                        bodies[5].Hide(comp);
                    if (limits[2] != null)
                        bodies[5] = DrawReferenceLine(DefaultOuterLimitColor, limits[2].ArrayData, jointOrigin.ArrayData, DefaultRadius * 2);
                    break;
                case 3:
                    if(bodies[6] != null)
                        bodies[6].Hide(comp);
                    if (limits[3] != null)
                        bodies[6] = DrawReferenceLine(DefaultInnerLimitColor, limits[3].ArrayData, jointOrigin.ArrayData, DefaultRadius * 2);
                    break;
            }

            if (limitIndex == 0 || limitIndex == 1)
            {
                if (bodies[3] != null)
                    bodies[3].Hide(comp);
                if (bodies[4] != null)
                    bodies[4].Hide(comp);
                if (limits[0] != null && limits[1] != null)
                {
                    Body2[] arrowBodies = DrawArrowAtLoc(DefaultArrow1Color, AxisVector.Scale(-1).ArrayData, jointOrigin.ArrayData, -LowerLimit * 180 / Math.PI, DefaultRadius, limits[0].ArrayData);
                    bodies[3] = arrowBodies[0];
                    bodies[4] = arrowBodies[1];
                }
            }
            else if (limitIndex == 2 || limitIndex == 3)
            {
                if (bodies[7] != null)
                    bodies[7].Hide(comp);
                if (bodies[8] != null)
                    bodies[8].Hide(comp);

                if (limits[2] != null && limits[3] != null)
                {
                    double radius = DefaultRadius;
                    if (ArrowsOverLap())
                        radius *= 1.5;
                    Body2[] arrowBodies = DrawArrowAtLoc(DefaultArrow2Color, AxisVector.ArrayData, jointOrigin.ArrayData, UpperLimit * 180 / Math.PI, radius, limits[2].ArrayData);
                    bodies[7] = arrowBodies[0];
                    bodies[8] = arrowBodies[1];
                }
            }
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = update;
            
        }

        /// <summary>
        /// Draws an arrow based on the given values
        /// </summary>
        /// <param name="vector">The direction the arrow should point</param>
        /// <param name="originPoint">The point where the center of the arrow is</param>
        /// <param name="angle">The angle the arrow covers in degrees</param>
        /// <param name="radius">The radius of the inside of the arrow</param>
        /// <param name="offsetAxis">The axis that the arrow should end at, null if it should end at the default location</param>
        /// <returns>The new wireframe body</returns>
        protected Body2[] DrawArrowAtLoc(int color,double[] vector, double[] originPoint, double angle,double radius, double[] offsetAxis)
        {
            if (angle <= DefaultHeadTheta)
                return new Body2[2];
            Body2 wireBody = null;
            Modeler swModler = swApp.GetModeler();

            Curve[] lines = new Curve[7];
            double headSin = Math.Sin(DefaultHeadTheta*Math.PI/180);
            double headCos = Math.Cos(DefaultHeadTheta * Math.PI / 180);

            double endSin = Math.Sin(angle * Math.PI / 180);
            double endCos = Math.Cos(angle * Math.PI / 180);

            double[][] vertices = {new double[]{0,-radius*headSin,-radius*headCos},
                                  new double[]{0,-(radius-DefaultWidth/2)*headSin,-(radius-DefaultWidth/2)*headCos},
                                  new double[]{0,0,-(DefaultWidth/2+radius)},
                                  new double[]{0,-(radius+DefaultWidth*1.5)*headSin,-(radius+DefaultWidth*1.5)*headCos},
                                  new double[]{0,-(radius+DefaultWidth)*headSin,-(radius+DefaultWidth)*headCos},
                                  new double[]{0,-(radius+DefaultWidth)*endSin,-(radius+DefaultWidth)*endCos},
                                  new double[]{0,-radius*endSin,-radius*endCos}};


            int i = 0;
            MathVector tempVect;
            double[] tempArr;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, headSin, headCos });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            tempArr = new double[] { 0, vertices[i + 1][1] - vertices[i][1], vertices[i + 1][2] - vertices[i][2] };
            tempVect = mathUtil.CreateVector(tempArr).Normalise();
            lines[i] = swModler.CreateLine(vertices[i], tempVect.ArrayData);
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            tempArr = new double[] { 0, vertices[i + 1][1] - vertices[i][1], vertices[i + 1][2] - vertices[i][2] };
            tempVect = mathUtil.CreateVector(tempArr).Normalise();
            lines[i] = swModler.CreateLine(vertices[i], tempVect.ArrayData);
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, headSin, headCos });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateArc(new double[] { 0, 0, 0 }, new double[] { -1, 0, 0 }, radius+DefaultWidth,vertices[i + 1], vertices[i] );
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, endSin, endCos });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateArc(new double[] { 0, 0, 0 }, new double[] { 1, 0, 0 }, radius, vertices[i], vertices[0]);
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[0][0], vertices[0][1], vertices[0][2]);



            wireBody = swModler.CreateWireBody(lines, 0);
            Surface tempSurf = swModler.CreatePlanarSurface2(new double[] { 0, 0, 0 }, new double[] { 1, 0, 0 }, new double[] { 0, 1, 0 });
            Body2 fillBody = tempSurf.CreateTrimmedSheet4(lines, true);
            MathVector normal = mathUtil.CreateVector(new double[]{1,0,0});
            fillBody = swModler.CreateExtrudedBody(fillBody, normal, .0001);
            Component2 comp = RobotInfo.DispComp;
            MathTransform axisTransform;
            if(offsetAxis == null)
                axisTransform = GetAxisTransform(vector, originPoint);
            else
            {
                MathVector tempVector = mathUtil.CreateVector(offsetAxis);
                tempVector = tempVector.Scale(-1);
                axisTransform = GetAxisTransform(vector, tempVector.ArrayData, originPoint);
            }
            axisTransform = axisTransform.Multiply(comp.Transform2.Inverse());
            wireBody.ApplyTransform(axisTransform);
            fillBody.ApplyTransform(axisTransform);

            wireBody.Display3(comp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            fillBody.Display3(comp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            fillBody.MaterialPropertyValues2 = new double[] { (color & 0xFF) / 255.0, (color & 0xFF00) / 255.0, (color & 0xFF0000) / 255.0, 0, 0, 0, 0, .8, 0 };
            return new Body2[]{wireBody,fillBody};
        }

        /// <summary>
        /// Draws a line along the axis of rotation
        /// </summary>
        /// <param name="vector">The direction of the line </param>
        /// <param name="originPoint">the centerpoint of theline</param>
        /// <returns>The created wireframe body</returns>
        protected Body2 DrawReferenceLine(int color, double[] vector, double[] originPoint)
        {
            Component2 comp = RobotInfo.DispComp;
            Modeler swModler = swApp.GetModeler();
            Curve tempLine = swModler.CreateLine(new double[] { 0, 0, 0 }, new double[] { 1, 0, 0 });
            tempLine = tempLine.CreateTrimmedCurve2(-1, 0, 0, 1, 0, 0);

            Body2 b2 = swModler.CreateWireBody(new Curve[] { tempLine }, 0);
            b2.ApplyTransform(GetAxisTransform(vector, originPoint));
            b2.ApplyTransform(comp.Transform2.Inverse());

            b2.Display3(RobotInfo.DispComp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            return b2;
        }

        /// <summary>
        /// Draws a line along the axis of rotation
        /// </summary>
        /// <param name="vector">The direction of the line </param>
        /// <param name="originPoint">the start of theline</param>
        /// <param name="length">The length of the line</param>
        /// <returns>The created wireframe body</returns>
        protected Body2 DrawReferenceLine(int color, double[] vector, double[] originPoint, double length)
        {
            Component2 comp = RobotInfo.DispComp;//getComponentForDisplay(null);
            Modeler swModler = swApp.GetModeler();
            Curve tempLine = swModler.CreateLine(originPoint, vector);
            MathVector dispVector = mathUtil.CreateVector(vector).Scale(length);
            MathPoint endPoint = mathUtil.CreatePoint(originPoint);
            endPoint = endPoint.AddVector(dispVector);
            double[] tempArr = endPoint.ArrayData;
            tempLine = tempLine.CreateTrimmedCurve2(originPoint[0], originPoint[1], originPoint[2], tempArr[0], tempArr[1], tempArr[2]);
            
            Body2 b2 = swModler.CreateWireBody(new Curve[] { tempLine }, 0);
            b2.ApplyTransform(comp.Transform2.Inverse());
            b2.Display3(RobotInfo.DispComp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            return b2;
        }

        /// <summary>
        /// Gets if the preview arrows overlap
        /// </summary>
        /// <returns></returns>
        private bool ArrowsOverLap()
        {
            if (limits == null || limits[0] == null || limits[3] == null)
                return false;
            double theta = Math.Acos(limits[0].Dot(limits[3]));
            MathVector axisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ });
            if (VectorCalcs.IsSameDirection(limits[3].Cross(limits[0]), axisVector))
                theta = 2 * Math.PI - theta;

            bool lowerOverlap = theta < -LowerLimit && Math.Abs(theta + LowerLimit) > VectorCalcs.errorVal;
            bool upperOverlap = (2 * Math.PI - theta)<UpperLimit && Math.Abs((2 * Math.PI - theta)- UpperLimit) > VectorCalcs.errorVal;

            return lowerOverlap || upperOverlap;
        }


        /// <summary>
        /// helper function to calculate the rotation angles to create the axis vector
        /// called in DrawJointPreview
        /// </summary>
        /// <param name="thetaY">rotation angle of axis vector around y'</param>
        /// <param name="thetaZ">rotation angle of axis vector around z'</param>
        private void CalculateThetas(out double thetaY, out double thetaZ)
        {
            //the rotation axis is found by assuming that the axis vector is equivlent to the x-axis of a rotated coordinate system.
            //the vector of the rotation arrow is then defined as the z' axis, and its origin is found along the y' axis. 
            //To find z' and y' thetaY and thetaZ are determined assuming that thetaY is the first rotation about the yaxis and then thetaZ is the second rotation about the z' axis

            double x = AxisX;
            double y = AxisY;
            double z = AxisZ;

            double xsquared = Math.Pow(AxisX, 2);
            double zsquared = Math.Pow(AxisZ, 2);

            if (Math.Abs(x) < errorVal)
            {
                if (Math.Abs(z) < errorVal)
                {
                    thetaY = 0;
                    thetaZ = (y > 0) ? Math.PI / 2 : -Math.PI / 2;
                }
                else
                {
                    thetaY = (z > 0) ? Math.PI / 2 : -Math.PI / 2;
                    thetaZ = Math.Atan(y / (Math.Sqrt(xsquared + zsquared)));  //thetaZ = Atan(y/sqrt(x^2+z^2))
                }
            }
            else
            {
                thetaY = (x > 0) ? Math.Atan(z / x) : //thetaY = Atan(Z/X)
                    Math.Atan(z / -x) + Math.PI;      //thetaY = Atan(-Z/X) + PI
                thetaZ = Math.Atan(y / (Math.Sqrt(xsquared + zsquared)));      //thetaZ = Atan(y/sqrt(x^2+z^2))
            }

            //System.Diagnostics.Debug.WriteLine("tY=" + thetaY + " tZ=" + thetaZ);
            //System.Diagnostics.Debug.WriteLine("axis=" + currentJoint.axisX + " " + currentJoint.axisY + " " + currentJoint.axisZ);

        }

        public override void ClearLimits()
        {
            base.ClearLimits();
            limits = null;
            for (int i = 0; i < FlipVectors.Count;i++ )
                FlipVectors[i] = 0;
            if(LimitButtons != null)
                foreach (PropertyManagerPageBitmapButton b in LimitButtons.Values)
                    b.Checked = false;
        }

        /// <summary>
        /// Clears all joint previews
        /// </summary>
        public override void ClearPreviews()
        {
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = false;
            if (bodies != null)
            {
                Component2 comp = RobotInfo.DispComp;
                foreach (Body2 b in bodies)
                {
                    if (b != null)
                    {
                        b.Hide(comp);
                    }
                }
            }
            ((ModelView)modelDoc.ActiveView).EnableGraphicsUpdate = true;
        }

        /// <summary>
        /// Writes the SDF Limits tags for this axis
        /// </summary>
        /// <param name="log">logger to use to print messages</param>
        /// <param name="writer">writer to use to write the SDF</param>
        protected override void WriteLimits(Export.ProgressLogger log, System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("limit");
            {
                if (!IsContinuous)
                {
                    SDFExporter.writeSDFElement(writer, "upper", UpperLimit.ToString());
                    SDFExporter.writeSDFElement(writer, "lower", LowerLimit.ToString());
                }
                
                SDFExporter.writeSDFElement(writer, "effort", EffortLimit.ToString());
            }
            writer.WriteEndElement();
        }

        public override void ButtonChanged(int Id)
        {
            if (LimitButtons!= null && LimitButtons.ContainsKey(Id))
            {
                int index = Array.IndexOf(LimitButtonIds, Id);
                FlipVectors[index] = LimitButtons[Id].Checked ? 1:0;
                double t = FlipVectors[index];
                if (limits != null && limits[index] != null)
                {
                    limits[index] = limits[index].Scale(-1);
                    if (index <= 1)
                        LowerLimit = -(-LowerLimit + Math.PI) % (2 * Math.PI);
                    else
                        UpperLimit = (UpperLimit + Math.PI) % (2 * Math.PI);
                    DrawAxisPreview(index);
                }
                
            }
            else if (Buttons.ContainsKey(Id) && !UseCustomMovementLimits)
            {
                LowerLimit = -(2 * Math.PI + LowerLimit) % (2 * Math.PI);
                UpperLimit = (2 * Math.PI - UpperLimit) % (2 * Math.PI);
            }
            base.ButtonChanged(Id);
        }

        public override void Verify(string axisName, ProgressLogger log)
        {
            base.Verify(axisName, log);
            if (!IsContinuous && LowerLimit >= 0 && UpperLimit <= 0)
                log.WriteWarning("No movement defined in joint " + axisName);
        }
    }
}
