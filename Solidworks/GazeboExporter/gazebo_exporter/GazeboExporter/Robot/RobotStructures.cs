using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Storage;
using System.Windows.Media.Media3D;
using SolidWorks.Interop.sldworks;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Struct representing the origin of an object as well as its transform
    /// </summary>
    public struct OriginPoint
    {
        public double X;
        public double Y;
        public double Z;

        public MathVector XAxis 
        { 
            get 
            {
                Matrix3D t = OriginTrasform;
                return RobotInfo.mathUtil.CreateVector(new double[]{t.M11, t.M12, t.M13});
            } 
        }
        public MathVector YAxis
        {
            get
            {
                Matrix3D t = OriginTrasform;
                return RobotInfo.mathUtil.CreateVector(new double[]{t.M21, t.M22, t.M23});
            }
        }
        public MathVector ZAxis
        {
            get
            {
                Matrix3D t = OriginTrasform;
                return RobotInfo.mathUtil.CreateVector(new double[]{t.M31, t.M32, t.M33});
            }
        }

        public double Roll;
        public double Pitch;
        public double Yaw;

        public double[] Point
        {
            get
            {
                return new double[] { X, Y, Z };
            }
            set
            {
                X = value[0];
                Y = value[1];
                Z = value[2];
            }
        }

        public Matrix3D OriginTrasform {
            get
            {
                Matrix3D mat = new Matrix3D();
                Vector3D offset = new Vector3D(-X, -Y, -Z);
                mat.Translate(offset);
                Vector3D xAxis = new Vector3D(1, 0, 0);
                Quaternion rollQ = new Quaternion(xAxis, Roll * 180 / Math.PI);
                Vector3D yAxis = new Vector3D(0, 1, 0);
                Quaternion pitchQ = new Quaternion(yAxis, Pitch * 180 / Math.PI);
                Vector3D zAxis = new Vector3D(0, 0, 1);
                Quaternion yawQ = new Quaternion(zAxis, Yaw * 180 / Math.PI);
                mat.Rotate(rollQ);
                mat.Rotate(pitchQ);
                mat.Rotate(yawQ);

                return mat;
            }
        }

        
    }

    /// <summary>
    /// Struct representing moment values of an object
    /// </summary>
    public struct MomentValues
    {
        StorageModel swData;
        string path;

        public MomentValues(StorageModel swData, string path)
        {
            this.swData = swData;
            this.path = path;
        }

        public MomentValues(StorageModel swData, string path,double ixx,double ixy,double ixz,double iyy,double iyz,double izz)
        {
            this.swData = swData;
            this.path = path;
            Ixx = ixx;
            Ixy = ixy;
            Ixz = ixz;
            Iyy = iyy;
            Iyz = iyz;
            Izz = izz;
        }

        public double Ixx
        {
            get
            {
                return swData.GetDouble(path + "/ixx");
            }
            set
            {
                swData.SetDouble(path + "/ixx", Math.Abs(value));
            }
        }

        public double Ixy
        {
            get
            {
                return swData.GetDouble(path + "/ixy");
            }
            set
            {
                swData.SetDouble(path + "/ixy", Math.Abs(value));
            }
        }

        public double Ixz
        {
            get
            {
                return swData.GetDouble(path + "/ixz");
            }
            set
            {
                swData.SetDouble(path + "/ixz", Math.Abs(value));
            }
        }

        public double Iyy
        {
            get
            {
                return swData.GetDouble(path + "/iyy");
            }
            set
            {
                swData.SetDouble(path + "/iyy", Math.Abs(value));
            }
        }
        public double Iyz
        {
            get
            {
                return swData.GetDouble(path + "/iyz");
            }
            set
            {
                swData.SetDouble(path + "/iyz", Math.Abs(value));
            }
        }

        public double Izz
        {
            get
            {
                return swData.GetDouble(path + "/izz");
            }
            set
            {
                swData.SetDouble(path + "/izz", Math.Abs(value));
            }
        }
    }
}
