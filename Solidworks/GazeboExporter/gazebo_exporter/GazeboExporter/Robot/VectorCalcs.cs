using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolidWorks.Interop.sldworks;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Utility class for doing vector calculations
    /// </summary>
    public static class VectorCalcs
    {
        public const double errorVal = .0000001;

        /// <summary>
        /// Finds if the vectors are parallel
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        /// <returns>True if the vectors are parallel</returns>
        public static bool IsParallel(MathVector vec1, MathVector vec2)
        {
            return vec1.Cross(vec2).GetLength() < errorVal;
        }

        /// <summary>
        /// Finds if the vectors are perpendicular
        /// </summary>
        /// <param name="vec1">Vector 1</param>
        /// <param name="vec2">Vector 2</param>
        /// <returns>True if the vectors are perpendicular</returns>
        public static bool IsPerpendicular(MathVector vec1, MathVector vec2)
        {
            return Math.Abs(vec1.Dot(vec2)) < errorVal;
        }

        /// <summary>
        /// Creates a vector in the global frame that was in the specified direction in the given component's frame
        /// </summary>
        /// <param name="compTransform">The transfrom that the orginal vector was in. If null the vector will be based in the global frame</param>
        /// <param name="vector">The direction of the vector to be created</param>
        /// <returns>The transformed MathVector</returns>
        public static MathVector CreateVector(MathTransform compTransform, double[] vector)
        {
            if (compTransform != null)
                return RobotInfo.mathUtil.CreateVector(vector).MultiplyTransform(compTransform).Normalise();
            else
                return RobotInfo.mathUtil.CreateVector(vector).Normalise();
        }

        /// <summary>
        /// Creates a Point in the global frame that was in the specified direction in the given component's frame
        /// </summary>
        /// <param name="compTransform">The transfrom that the orginal vector was in. If null the vector will be based in the global frame</param>
        /// <param name="point">The local location of the point to be created</param>
        /// <returns>The transformed MathPoint</returns>
        public static MathPoint CreatePoint(MathTransform compTransform, double[] point)
        {
            if (compTransform != null)
                return RobotInfo.mathUtil.CreatePoint(point).MultiplyTransform(compTransform);
            else
                return RobotInfo.mathUtil.CreatePoint(point);
        }

        /// <summary>
        /// Projects the vector between the inputed point and a point on the axis line and then scales
        /// The axis to the projected length.
        /// </summary>
        /// <param name="axis">The axis vector that will be projected onto</param>
        /// <param name="point">the point on the other axis</param>
        /// <returns>A vector scaled to the projection</returns>
        public static MathVector GetScaledVector(MathVector axis, double[] point1, double[] point2)
        {
            double[] tempArray = { point1[0] - point2[0], point1[1] - point2[1], point1[2] - point2[2] };
            MathVector pointVector = RobotInfo.mathUtil.CreateVector(tempArray);//creates a vector between the point on the line and the origin of the joint
            return axis.Scale(pointVector.Dot(axis));
        }

        /// <summary>
        /// Transforms the inputed array by the inputted transform.
        /// </summary>
        /// <param name="trans">Transform to move the point by</param>
        /// <param name="Pt">Point to be Transformed</param>
        /// <returns>The transformed point</returns>
        public static double[] TransformPoint(MathTransform trans, double[] Pt)
        {
            double[] newPoint;
            if (Pt.Length > 3)
                newPoint = Pt.Take(3).ToArray();
            else
                newPoint = Pt;
            return RobotInfo.mathUtil.CreatePoint(newPoint).MultiplyTransform(trans).ArrayData;
        }

        /// <summary>
        /// Calcualtes if the two vectors intersect
        /// </summary>
        /// <param name="axis1Dir">The direction of the first axis</param>
        /// <param name="axis1Point">a point on the first axis</param>
        /// <param name="axis2Dir">The direction of the second axis</param>
        /// <param name="axis2Point">a point on the second axis</param>
        /// <returns></returns>
        public static bool AxisIntersect(MathVector axis1Dir, MathPoint axis1Point, MathVector axis2Dir, MathPoint axis2Point)
        {

            MathVector pointVect = GetVectorBetweenPoints(axis1Point, axis2Point);
            if (pointVect.GetLength() < VectorCalcs.errorVal || VectorCalcs.IsParallel(pointVect.Cross(axis1Dir), pointVect.Cross(axis2Dir)))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Finds if the given point lies on the given line
        /// </summary>
        /// <param name="dir">The direction of the line</param>
        /// <param name="pointOnLine">A point somewhere on the line</param>
        /// <param name="otherPoint">The point that is being checked</param>
        /// <returns>Returns true if the point is is on the line</returns>
        public static bool IsPointOnLine(MathVector dir, MathPoint pointOnLine, MathPoint otherPoint)
        {
            MathVector PointVector = GetVectorBetweenPoints(pointOnLine, otherPoint).Normalise();
            return (IsParallel(dir, PointVector));
        }

        /// <summary>
        /// Gets the nonnormalised vector between the 2 points
        /// </summary>
        /// <param name="p1">Thefirst point</param>
        /// <param name="p2">The second point</param>
        /// <returns>returns the vector from point 1 to point 2</returns>
        public static MathVector GetVectorBetweenPoints(MathPoint p1, MathPoint p2)
        {
            double[] point1 = p1.ArrayData;
            double[] point2 = p2.ArrayData;
            double[] pointDisp = { point2[0] - point1[0], point2[1] - point1[1], point2[2] - point1[2] };
            return RobotInfo.mathUtil.CreateVector(pointDisp);
        }

        /// <summary>
        /// Finds if the 2 vectors are in the same direction
        /// </summary>
        /// <param name="v1">THe first vector</param>
        /// <param name="v2">The second vector</param>
        /// <returns>whether the 2 vectors have the same direction</returns>
        public static bool IsSameDirection(MathVector v1, MathVector v2)
        {
            v1 = v1.Normalise();
            v2 = v2.Normalise();
            return v1.Subtract(v2).GetLength() < errorVal;
        }
    }
}
