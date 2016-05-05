using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Export;
using System.Xml;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Represents a screw joint. a screw joint is like a linear joint 
    /// except it's limits are expressed in terms of rotations based on the thread pitch
    /// </summary>
    public class ScrewJointAxis: LinearJointAxis
    {
        /// <summary>
        /// The thread pitch for the axis in Radians/Meter
        /// </summary>
        public double ThreadPitch
        {
            get
            {
                return swData.GetDouble(path + "/threadPitch");
            }
            set
            {
                swData.SetDouble(path + "/threadPitch", value);
            }
        }

        /// <summary>
        /// Constructs a new Screw Joint axis
        /// </summary>
        /// <param name="path">The path to store this axis at</param>
        /// <param name="current">The joint that this axis is contained in</param>
        public ScrewJointAxis(string path, Joint current) :
            base(path, current)
        {

        }

        /// <summary>
        /// Writes the limits to the sdf file
        /// </summary>
        /// <param name="log">logger to write messages to</param>
        /// <param name="writer">the writer to write the file with</param>
        protected override void WriteLimits(ProgressLogger log, XmlWriter writer)
        {
            writer.WriteStartElement("limit");
            {
                SDFExporter.writeSDFElement(writer, "upper", (UpperLimit*ThreadPitch).ToString());
                SDFExporter.writeSDFElement(writer, "lower", (LowerLimit*ThreadPitch).ToString());
                SDFExporter.writeSDFElement(writer, "effort", EffortLimit.ToString());
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the SDF tags for this Axis
        /// </summary>
        /// <param name="log">logger to use to print messages</param>
        /// <param name="writer">the Writer to use to write the SDF</param>
        /// <param name="index">Index of the axis</param>
        public override void WriteSDF(ProgressLogger log, XmlWriter writer, int index)
        {
            base.WriteSDF(log, writer, index);
            SDFExporter.writeSDFElement(writer, "thread_pitch", ThreadPitch.ToString());
        }

    }
}
