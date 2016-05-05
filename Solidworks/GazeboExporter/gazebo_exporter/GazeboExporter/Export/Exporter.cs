  using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GazeboExporter.Robot;
using System.Windows.Forms;
using System.Threading;

namespace GazeboExporter.Export
{
    public class Exporter
    {
        //the logger that export messages should be sent to
        ProgressLogForm logger;
        //the model of the robot
        RobotModel currentRobot;

        FolderBrowserDialog dialog;

        /// <summary>
        /// Constructs the exporter
        /// </summary>
        /// <param name="robot"> Robot model to be exported</param>
        public Exporter(RobotModel robot)
         {
             //Thread loggerThread = new Thread(new ThreadStart(LaunchLogger));
             //loggerThread.Start();
             this.currentRobot = robot;
             //Thread.Sleep(1000);
             LaunchLogger();
         }

        private void LaunchLogger()
        {
            logger = new ProgressLogForm();
            logger.ButtonClicked += LoggerButtonClicked;
            logger.CancelButtonClicked += LoggerCancelButtonClicked;
            RobotInfo.WriteToLogFile("Export Logger Launched (Exporter)");
        }


        /// <summary>
        /// Begins the export in a new thread
        /// </summary>
        public void RunExportSequence()
        {
            RobotInfo.WriteToLogFile("RunExportSequence from Exporter executing (Exporter)");
            currentRobot.ContinueExport = true;
            logger.Show();
            /*Thread verify = new Thread(new ThreadStart(RunVerify));
            verify.Start();*/
            //while (true) ;
            RunVerify();
        }

        /// <summary>
        /// EventHandler for starting the export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoggerButtonClicked(object sender, EventArgs e)
        {
            /*Thread export = new Thread(new ThreadStart(RunExport));
            export.SetApartmentState(ApartmentState.STA);
            export.Start();*/
            RunExport();
        }

        /// <summary>
        /// EventHandler for canceling the export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoggerCancelButtonClicked(object sender, EventArgs e)
        {
            RobotInfo.WriteToLogFile("Canceling export (Exporter)");
            logger.WriteMessage("Canceling Export", false);
            currentRobot.ContinueExport = false;
            logger.CancelingExport = true;
            logger.setButtonText("Close");
            logger.setTitle("Canceling Export");
            logger.setOpText("Canceling...");
            logger.EnableButton(false);
        }

        /// <summary>
        /// runs the actual export
        /// </summary>
        private void RunExport()
        {
            logger.EnableButton(false);
            logger.makeTopMost(false);
            /*if (dialog != null)
            {
                dialog.Dispose();
                dialog = null;
            }
            dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.Desktop;
            dialog.SelectedPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\wpilib\\simulation";
            

            DialogResult result = dialog.ShowDialog();*/
            DialogResult result;
            String path = logger.GetFolderPath(out result);//dialog.SelectedPath;
            logger.EnableButton(true);
            if (result == DialogResult.Cancel || result == DialogResult.Abort){
                logger.Invoke((MethodInvoker)delegate
                {
                    logger.Close();
                });
            } else {
                logger.makeTopMost(true);
                logger.setTitle("Exporting");
                logger.setButtonText("Cancel");
                logger.reset(currentRobot.EstimateExportOps());
                if (currentRobot.Export(logger, path))
                {
                    logger.setTitle("Export Complete");
                    logger.setButtonText("Close");
                    logger.setOpText("Export Successful");
                }
                else
                {
                    logger.Invoke((MethodInvoker)delegate
                    {
                        logger.Close();
                        logger.Dispose();
                    });
                }
            }
            RobotInfo.WriteToLogFile("Run Export" + logger + " (Exporter)");
        }

        /// <summary>
        /// Verifies the model
        /// This ensures the model is valid for export, as well as recalculates properties like mass and moment of inertia
        /// </summary>
        private void RunVerify()
        {
            RobotInfo.WriteToLogFile("RunVerify from Exporter executing (Exporter)");
            logger.reset(currentRobot.EstimateVerifyOps());
            logger.setTitle("Verifying Model");
            logger.setButtonText("Cancel");
            if (currentRobot.Verify(logger))
            {
                logger.setOpText(logger.Errors + " Errors, " + logger.Warnings + " Warnings");
                if (logger.Errors == 0 || PluginSettings.DebugMode)
                {
                    logger.setButtonText("Export");
                    logger.setTitle("Ready to Export");
                }
                else
                {
                    logger.setButtonText("Close");
                    logger.setTitle("Errors Detected");
                }
                
            }
            else
            {
                if(!logger.IsDisposed)
                logger.Invoke((MethodInvoker)delegate
                {
                    logger.Close();
                    logger.Dispose();
                });
            }
             
        }

        /// <summary>
        /// Brings the logging window to the front
        /// </summary>
        public void BringToFront()
        {
            logger.BringToFront();
        }

        /// <summary>
        /// Finds if the exporter has been closed
        /// </summary>
        /// <returns>True if the Exporter was closed</returns>
        public bool IsClosed()
        {
            return logger.IsDisposed;
        }

        /// <summary>
        /// Closes the Logger window
        /// </summary>
        public void CloseExporter()
        {
            logger.Close();
        }
    }
}
