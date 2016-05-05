using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.sldworks;
using GazeboExporter.Storage;
using System.Xml;
using GazeboExporter.Export;

namespace GazeboExporter.Robot
{

    /// <summary>
    /// A linear joint Axis is an axis that moves linearly along the axis of motion
    /// </summary>
    public class LinearJointAxis : IJointAxis
    {

        private Body2[] bodies;

        const double DefaultLength = .1;
        const double DefaultRatio = 1 / 6.0;

        public const int DefaultArrow1Color = 0x2B39C0;
        public const int DefaultArrow2Color = 0x60AE27;


        /// <summary>
        /// Constructs a new Linear Axis
        /// </summary>
        /// <param name="path">Path to this joint's storage</param>
        /// <param name="current">The joint that this axis is contained in</param>
        public LinearJointAxis( string path, Joint current) :
            base( path, current)
        {

        }

        #region Limits

        /// <summary>
        /// Determines if the inputted selection is valid
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

            MathTransform componentTransform = null;
            if ((Component2)((IEntity)Selection).GetComponent() != null)//gets the component transform if one exists. this allows for conversion between the components local space and the global space
            {
                componentTransform = ((Component2)((IEntity)Selection).GetComponent()).Transform2;
            }
            double[] tempAxisNormalVector = { AxisX, AxisY, AxisZ };
            MathVector axisNormalVector = mathUtil.CreateVector(tempAxisNormalVector).Normalise();//creates a vector to represent the aXis of movment
            
            MathVector featureNormalVector = null;
            double[] tempArray;
            switch ((swSelectType_e)SelType)
            {
                //Selection is a reference axis. If refAxis is perpendicular to the joint axis it is a valid selection
                case swSelectType_e.swSelDATUMAXES:

                    double[] points = ((IRefAxis)((IFeature)Selection).GetSpecificFeature2()).GetRefAxisParams();
                    tempArray = new double[] { points[3] - points[0], points[4] - points[1], points[5] - points[2] };
                    featureNormalVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector between the 2 points on the reference axis and transforms to global space if nessacary

                    if (VectorCalcs.IsPerpendicular(featureNormalVector, axisNormalVector))//if the joint axis and the referance axis are perpindicular then the selection is valid
                    {
                        return true;
                    }
                    break;

                //Selection is a reference point. All reference points are valid.
                case swSelectType_e.swSelDATUMPOINTS:
                    return true;
                    break;
                //Selection is a reference plane. If the plane's normal vector is the same as the joint axis it is valid.
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
                    if (VectorCalcs.IsParallel(featureNormalVector, axisNormalVector))//if the plane normal vector and the joint axis are parallel the selection is valid
                    {
                        return true;
                    }
                    break;

                //Selection is an edge. If the edge is a line, the line must be perpindicular to the joint axis to be valid. 
                //If the edge is not a line but is still planar the normal vector of the plane that the edge is in it must be the same as the axis of the joint 
                case swSelectType_e.swSelEDGES:
                    object[] faces = ((IEdge)Selection).GetTwoAdjacentFaces2();
                    if (faces[0] == null || faces[1] == null)
                    {
                        break;
                    }
                    MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);//gets normal vectors for each bordering face
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
                        featureNormalVector = VectorCalcs.CreateVector(componentTransform, tempArray);//creates a vector representing the linear edge
                        if (VectorCalcs.IsPerpendicular(featureNormalVector, axisNormalVector))//if the edgevector and the axis are perpindicular it is valid
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
                        if (VectorCalcs.IsParallel(featureNormalVector, axisNormalVector))
                        {
                            return true;
                        }
                    }
                    break;
                //Selection is a face. The normal vector of the face must be the same as the joint axis to be valid
                case swSelectType_e.swSelFACES:
                    MathVector faceVector = mathUtil.CreateVector(((IFace2)Selection).Normal);
                    if (faceVector.GetLength() < errorVal)//makes sure face is valid
                    {
                        break;
                    }
                    featureNormalVector = (faceVector.MultiplyTransform(componentTransform)).Normalise(); //converts the face vector to the corect coordinate orientation

                    if (VectorCalcs.IsParallel(featureNormalVector, axisNormalVector))
                    {
                        return true;
                    }
                    break;
                //Selection is a vertice. All verticies are valid
                case swSelectType_e.swSelVERTICES:
                    return true;
                default:
                    return false;
            }
            //System.Diagnostics.Debug.WriteLine("AxisVector: " + String.Join(" ", (double[])axisNormalVector.ArrayData));
            //System.Diagnostics.Debug.WriteLine("NormalVector: " + String.Join(" ", (double[])featureNormalVector.ArrayData));
            selBox.ShowBubbleTooltip("Error: Feature not normal to joint axis", "The selected feature must be normal to the axis of movement", "");
            return false;
        }

        /// <summary>
        /// Calculates the limit values of the axis
        /// </summary>
        /// <param name="limitObjs">Array of the 4 limit selections in order of lowerLimitStop,lowerLimitEdge,upperLimitStop,upperLimitEdge. If any spot is null the current stored value will be used. if limitObjs is null all current values will be used</param>
        /// <returns>True if the limits where succesfully calculated</returns>
        public override bool CalcLimits(int index, object obj)
        {
            if (Axis == null)
            {
                return false;
            }
            if (UseCustomMovementLimits)
            {
                return false;
            }
            bool successfulCalculation = true;
            
            MathVector[] limits = new MathVector[4]; //contains the vectors for each limit axis. for revolute joints the angle is represented and all vectors are normalised, for prismatic all vectors are parallel and the length represents the limit
            double[] Point;
            if (owner.jointSpecifics.OriginPt != null)
               Point = owner.jointSpecifics.OriginValues.Point;
            else
                Point = this.Point;
            
            object[] limitObjs = { LowerLimitStop, LowerLimitEdge, UpperLimitStop, UpperLimitEdge };
            if (index != -1)
            {
                limitObjs[index] = obj;
            }

            MathVector axisVector = mathUtil.CreateVector(new double[] { AxisX, AxisY, AxisZ }); //the vector of the rotation/translation axis
            double errorVal = .000001;
            for (int i = 0; i < limitObjs.Length; i++)
            {
                if (limitObjs[i] == null)//checks that a limit is stored
                {
                    successfulCalculation = false;
                    continue;
                }
                MathTransform componentTransform = null;
                Component2 limComp = (Component2)((IEntity)limitObjs[i]).GetComponent();
                if ( limComp != null)
                {
                    componentTransform = limComp.Transform2; //stores the component transform if there is one. this will be used to convert the edge vectors to the global coordinate system
                }
                if (limitObjs[i] is IEdge)//if the face is an edge
                    {
                        object[] faces = ((IEdge)limitObjs[i]).GetTwoAdjacentFaces2();
                        MathVector faceVector1 = mathUtil.CreateVector(((IFace2)faces[0]).Normal);//get normal vectors of each adjacent face, they will be 0 if no normal vector exists
                        MathVector faceVector2 = mathUtil.CreateVector(((IFace2)faces[1]).Normal);
                        if (faceVector1.GetLength() > 0 && faceVector2.GetLength() > 0)//if both exist, the edge is a line
                        {
                            double[] untransformedPoint = ((IEdge)limitObjs[i]).GetStartVertex().getPoint();
                            double[] startPoint = VectorCalcs.TransformPoint(componentTransform,untransformedPoint); //gets the start point of the edge and converts it to global space
                            limits[i] = VectorCalcs.GetScaledVector(axisVector,startPoint,Point);//projects the edge vector on to the axis vector
                        }
                        else
                        {
                            double[] closestPoint;
                            if (faceVector1.GetLength() > 0)//gets the closest point on the one face that exist
                            {
                                closestPoint = ((IFace2)faces[0]).GetClosestPointOn(Point[0], Point[1], Point[2]);
                                
                            }
                            else
                            {
                                closestPoint = ((IFace2)faces[1]).GetClosestPointOn(Point[0], Point[1], Point[2]);
                            }
                            double[] transformedPoint = VectorCalcs.TransformPoint(componentTransform, closestPoint);//transforms the closest point to global space
                            limits[i] = VectorCalcs.GetScaledVector(axisVector, transformedPoint,Point);
                        }
                    }
                    else if (limitObjs[i] is IFace2)//if limit is a face
                    {
                        double[] closestPoint = ((IFace2)limitObjs[i]).GetClosestPointOn(Point[0], Point[1], Point[2]);//finds the closest point on the face
                        double[] transformedPoint = VectorCalcs.TransformPoint(componentTransform, closestPoint);//transforms the point to global space
                        limits[i] = VectorCalcs.GetScaledVector(axisVector, transformedPoint,Point);
                    }
                    else if (limitObjs[i] is IVertex)//if limit is a vertex
                    {
                        double[] transformedPoint = VectorCalcs.TransformPoint(componentTransform, ((IVertex)limitObjs[i]).GetPoint());//transforms the vertex's coordinates to global space
                        limits[i] = VectorCalcs.GetScaledVector(axisVector, transformedPoint,Point);
                    }
                    else
                    {
                        object tempObject = ((IFeature)limitObjs[i]).GetSpecificFeature2();
                        if (tempObject is IRefAxis)//if limit is a referance axis
                        {
                            double[] refAxisPoints = ((IRefAxis)tempObject).GetRefAxisParams();
                            double[] transformedPoint;
                            if (componentTransform != null)//if the axis is part of a subcomponent transform its point to global space
                            {
                                transformedPoint = VectorCalcs.TransformPoint(componentTransform, refAxisPoints);
                            }
                            else
                            {
                                transformedPoint = new double[] { refAxisPoints[0], refAxisPoints[1], refAxisPoints[2] };
                            }

                            limits[i] = VectorCalcs.GetScaledVector(axisVector, transformedPoint,Point);
                        }
                        else if (tempObject is IRefPlane)//if limit is a reference plane
                        {
                            MathTransform planeTransform = ((IRefPlane)tempObject).Transform;//gets the transform of the plane
                            if (componentTransform != null)//if the plane is part of a sub component, transform it again into global space
                            {
                                planeTransform = planeTransform.Multiply(componentTransform);
                            }
                            double[] tempArr = { planeTransform.ArrayData[9], planeTransform.ArrayData[10], planeTransform.ArrayData[11] };
                            limits[i] = VectorCalcs.GetScaledVector(axisVector, tempArr,Point);


                        }
                        else if (tempObject is IRefPoint)//if limit is a reference point
                        {
                            double[] transformedPoint;
                            if (componentTransform != null)//gets point location and transforms to global space if the point is part of a sub component
                            {
                                transformedPoint = ((IRefPoint)tempObject).GetRefPoint().MultiplyTransform(componentTransform).ArrayData;
                            }
                            else
                            {
                                transformedPoint = ((IRefPoint)tempObject).GetRefPoint().ArrayData;
                            }

                            limits[i] = VectorCalcs.GetScaledVector(axisVector, transformedPoint,Point);
                        }
                    }
                }
            MathVector movementVector;
            
            movementVector = null;
            if (limitObjs[0] != null && limitObjs[1] != null)
            {
                LowerLimit = -limits[0].Subtract(limits[1]).GetLength();//length between the lower limits
                movementVector = limits[1].Subtract(limits[0]).Normalise();
            }
            else
            {
                LowerLimit = 0;
            }
            if (limitObjs[2] != null && limitObjs[3] != null)
            {
                UpperLimit = limits[2].Subtract(limits[3]).GetLength();//length between the upper limits
                movementVector = limits[2].Subtract(limits[3]).Normalise();
            }
            else
            {
                UpperLimit = 0;
            }
            if (movementVector != null)
            {
                AxisIsFlipped = (movementVector.Add(axisVector).GetLength() < errorVal) ^ AxisIsFlipped;//if the movment is in the opposite direction as the axis, the axis must be flipped
            }
            limitVectors = limits;
            return successfulCalculation;
        }

        
        #endregion

        /// <summary>
        /// Draws the preview for the linear axis
        /// </summary>
        public override void DrawAxisPreview()
        {
            MathPoint jointOrigin; 
            if(owner.jointSpecifics.OriginPt != null)
                jointOrigin = mathUtil.CreatePoint(owner.jointSpecifics.OriginValues.Point);
            else
                jointOrigin = mathUtil.CreatePoint(Point);
            MathVector AxisVector = mathUtil.CreateVector(new double[] {AxisX,AxisY,AxisZ});
            ClearPreviews();
            if (AxisVector.GetLength() < VectorCalcs.errorVal)
                return;
            
            if (UseCustomMovementLimits)
            {
                bodies = new Body2[2];
                bodies = DrawArrowAtLoc(DefaultArrow2Color,AxisVector.ArrayData, jointOrigin.ArrayData, DefaultLength, DefaultLength * DefaultRatio);
            }
            else
            {
                double ArrowWidth = UpperLimit * DefaultRatio;
                bodies = new Body2[4];

                if (LowerLimit < -.0001)
                {

                    double arrowLength = -LowerLimit;
                    if (-LowerLimit < UpperLimit || UpperLimit<.0001)
                        ArrowWidth = -LowerLimit * DefaultRatio;
                    MathVector direction = limitVectors[1].Subtract(limitVectors[0]).Normalise();
                    MathPoint origin;
                    if (VectorCalcs.IsSameDirection(direction, AxisVector))
                        origin = jointOrigin.AddVector(limitVectors[1]);
                    else
                        origin = jointOrigin.AddVector(limitVectors[0]);
                    Body2[] tempBodies = DrawArrowAtLoc(DefaultArrow1Color,AxisVector.Scale(-1).ArrayData,origin.ArrayData, arrowLength, ArrowWidth);
                    bodies[0] = tempBodies[0];
                    bodies[1] = tempBodies[1];
                }
                if (UpperLimit > .0001)
                {
                    double arrowLength = UpperLimit;
                    MathPoint origin = jointOrigin.AddVector(limitVectors[3]);
                    Body2[] tempBodies = DrawArrowAtLoc(DefaultArrow2Color,AxisVector.ArrayData, origin.ArrayData, arrowLength, ArrowWidth);
                    bodies[2] = tempBodies[0];
                    bodies[3] = tempBodies[1];
                }
               
            }
        }

        /// <summary>
        /// Updates the specified preview part. Willupdate both in this joint type
        /// </summary>
        /// <param name="limitIndex">the index of the limit</param>
        public override void DrawAxisPreview(int limitIndex, bool update = true)
        {
            DrawAxisPreview();
        }

        /// <summary>
        /// Draws an arrow based on the given values
        /// </summary>
        /// <param name="vector">The direction the arrow should point</param>
        /// <param name="startPoint">The point where the base of the arrow should start</param>
        /// <param name="length">The length of the arrow</param>
        /// <param name="width">The width of the arrow, note this must be less than the length;</param>
        /// <returns>The new wireframe body</returns>
        protected Body2[] DrawArrowAtLoc(int color, double[] vector, double[] startPoint, double length, double width)
        {
            Body2 wireBody = null;
            Modeler swModler = swApp.GetModeler();

            Curve[] lines = new Curve[7];
            double[][] vertices = {new double[]{0,-width/2,0},
                                  new double[]{0,width/2,0},
                                  new double[]{length-width,width/2,0},
                                  new double[]{length-width,width,0},
                                  new double[]{length,0,0},
                                  new double[]{length-width,-width,0},
                                  new double[]{length-width,-width/2,0}};




            int i = 0;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, 1, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i+1][0], vertices[i+1][1], vertices[i+1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 1, 0, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, 1, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { .70710678118, -.70710678118, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { -.70710678118, -.70710678118, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { 0, 1, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[i + 1][0], vertices[i + 1][1], vertices[i + 1][2]);
            i++;
            lines[i] = swModler.CreateLine(vertices[i], new double[] { -1, 0, 0 });
            lines[i] = lines[i].CreateTrimmedCurve2(vertices[i][0], vertices[i][1], vertices[i][2], vertices[0][0], vertices[0][1], vertices[0][2]);

            wireBody = swModler.CreateWireBody(lines, 0);

            Surface tempSurf = swModler.CreatePlanarSurface2(new double[] { 0, 0, 0 }, new double[] { 0, 0, 1 }, new double[] { 1, 0, 0 });
            Body2 fillBody = tempSurf.CreateTrimmedSheet4(lines, true);
            MathVector normal = mathUtil.CreateVector(new double[] { 0, 0, 1 });
            fillBody = swModler.CreateExtrudedBody(fillBody, normal, .0001);


            Component2 comp = RobotInfo.DispComp;

            MathTransform axisTransform = GetAxisTransform(vector, startPoint);
            axisTransform = axisTransform.Multiply(comp.Transform2.Inverse());
            wireBody.ApplyTransform(axisTransform);

            fillBody.ApplyTransform(axisTransform);

            wireBody.Display3(comp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            fillBody.Display3(comp, color, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);
            fillBody.MaterialPropertyValues2 = new double[] { (color & 0xFF) / 255.0, (color & 0xFF00) / 255.0, (color & 0xFF0000) / 255.0, 0, 0, 0, 0, .8, 0 };

            return new Body2[] { wireBody, fillBody };
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
        protected override void WriteLimits(ProgressLogger log, XmlWriter writer)
        {
            writer.WriteStartElement("limit");
            {
                SDFExporter.writeSDFElement(writer, "upper", UpperLimit.ToString());
                SDFExporter.writeSDFElement(writer, "lower", LowerLimit.ToString());
                SDFExporter.writeSDFElement(writer, "effort", EffortLimit.ToString());
            } 
            writer.WriteEndElement();
        }

        /// <summary>
        /// Verifies the axis is validfor export
        /// </summary>
        /// <param name="axisName">The name for this axis</param>
        /// <param name="log">The logger to write messages to</param>
        public override void Verify(string axisName, ProgressLogger log)
        {
            base.Verify(axisName, log);
            if (LowerLimit >= 0 && UpperLimit <= 0)
                log.WriteWarning("No movement defined in joint " + axisName);
        }
    }
}
