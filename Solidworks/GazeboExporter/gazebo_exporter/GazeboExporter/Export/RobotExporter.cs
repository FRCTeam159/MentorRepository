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

namespace GazeboExporter.Export
{
    /// <summary>
    /// Abstract class to represent an exporter
    /// Classes that extend this class should be able to export a robot model
    /// </summary>
    public abstract class RobotExporter
    {
        //the model of the robot
        protected RobotModel robot;
        //the solidworks app
        protected SldWorks iSwApp;
        //the assembly doc
        protected AssemblyDoc asm;

        /// <summary>
        /// Constructer
        /// </summary>
        /// <param name="robot">Robot model to be used for export</param>
        /// <param name="iSwApp">The solidworks app</param>
        /// <param name="asm">The assembly document</param>
        public RobotExporter(RobotModel robot, SldWorks iSwApp, AssemblyDoc asm)
        {
            this.robot = robot;
            this.iSwApp = iSwApp;
            this.asm = asm;
        }

        /// <summary>
        /// Exports the robot
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="path">The path to where the export should be stored</param>
        public abstract void Export(ProgressLogger log, String path);

        /// <summary>
        /// Estimate the number of steps to export the robot
        /// </summary>
        /// <returns>The number of steps needed to export the robot</returns>
        public abstract int EstimateOperations();
    }
}
