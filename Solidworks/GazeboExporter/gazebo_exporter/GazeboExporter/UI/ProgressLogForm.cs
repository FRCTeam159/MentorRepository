using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Export;

namespace GazeboExporter
{

    /// <summary>
    /// A form to show the user the progress of a long operation.
    /// </summary>
    public partial class ProgressLogForm : Form, ProgressLogger
    {
        public event EventHandler ButtonClicked;
        public event EventHandler CancelButtonClicked;

        /// <summary>
        /// number of warnings
        /// </summary>
        public int Warnings { get; set; }
        /// <summary>
        /// number of errors
        /// </summary>
        public int Errors { get; set; }

        /// <summary>
        /// true if export is being canceled
        /// </summary>
        public bool CancelingExport { get; set; }

        /// <summary>
        /// Initializeds a ProgressLog with the given window title.
        /// </summary>
        /// <param name="title">Title that will be displayed in the title bar of the form's window</param>
        public ProgressLogForm()
        {
            InitializeComponent();
            CancelingExport = false;
            this.TopMost = true;
        }

        internal void dump(object v)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Signifies the beginning of the operation.
        /// This should be called before any part of the operation starts.
        /// </summary>
        /// <param name="notice">Text to print in the log at the start of the operation</param>
        /// <param name="extimatedOps">Numer of sub-operations this operation needs (for progress bar)</param>
        public void reset(int extimatedOps)
        {
            this.Invoke((MethodInvoker)delegate
            {
                operationLog.Text = "";
                progressBar.Maximum = extimatedOps;
                progressBar.Value = 0;
            });
        }

        public void setTitle(String title)
        {
            //this.Invoke((MethodInvoker)delegate
            //{
                this.Text = title;
            //});
        }

        public void setButtonText(String text)
        {
            //this.Invoke((MethodInvoker)delegate
            //{
                this.button.Text = text;
            //});
        }

        public void setOpText(String text)
        {
            //this.Invoke((MethodInvoker)delegate
            //{
                this.currentOperation.Text = text;
            //});
        }


        public void WriteMessage(String message, bool isOp)
        {
            this.Invoke((MethodInvoker)delegate
            {
                RobotInfo.WriteToLogFile("Logger Message: " + message);
                if (CancelingExport)
                    return;
                
                //operationLog.ForeColor = Color.Black;
                operationLog.SelectionColor = Color.Black;
                operationLog.AppendText(message + "\n");
                operationLog.SelectionStart = operationLog.Text.Length;
                operationLog.ScrollToCaret();
                if (isOp)
                {
                    currentOperation.Text = message;
                    progressBar.PerformStep();
                }
                Refresh();
            });
        }


        public void WriteWarning(String warning, bool isOp)
        {
            this.Invoke((MethodInvoker)delegate
            {
                RobotInfo.WriteToLogFile("[WARNING] " + warning);
                if (CancelingExport)
                    return;
                if (isOp)
                {
                    progressBar.PerformStep();
                    currentOperation.Text = warning;
                }
                string s = "[WARNING]: " + warning + "\n";
                operationLog.SelectionColor = Color.Gold; 
                operationLog.AppendText(s);
                operationLog.SelectionStart = operationLog.Text.Length;
                operationLog.ScrollToCaret();
                Warnings++;
            });
        }

        public void WriteError(String error, bool isOp)
        {
            this.Invoke((MethodInvoker)delegate
            {
                RobotInfo.WriteToLogFile("[ERROR] " + error);
                if (CancelingExport)
                    return;
                if (isOp)
                {
                    progressBar.PerformStep();
                    currentOperation.Text = error;
                }
                string s = "[ERROR]: " + error + "\n";
                operationLog.SelectionColor = Color.Red;
                operationLog.AppendText(s);
                operationLog.SelectionStart = operationLog.Text.Length;
                operationLog.ScrollToCaret();
                Errors++;
            });
        }

        public void makeTopMost(bool isTop)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.TopMost = isTop;
            });
        }

        public void EnableButton(bool enabled)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.button.Enabled = enabled;
            });
        }

        /// <summary>
        /// Dump the log in its current state to a file with the specified path
        /// </summary>
        /// <param name="path">File path to write log to</param>
        public void dump(String path)
        {
            operationLog.SaveFile(path);
        }


        /// <summary>
        /// Event handler called when button 1 is clicked by the user
        /// This is usually an OK button
        /// </summary>
        /// <param name="sender">Object that invoked the event</param>
        /// <param name="e">Parameters relating to the event</param>
        private void button_Click_1(object sender, EventArgs e)
        {
            if (this.button.Text.Equals("Export"))
            {
                ButtonClicked(this, e);
            }
            else if (this.button.Text.Equals("Cancel"))
            {
                CancelButtonClicked(this, e);
            }
            else
            {
                this.Close();
            }
        }

        private void ProgressLogForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (this.button.Text.Equals("Cancel"))
                {
                    
                    e.Cancel = true;
                    CancelButtonClicked(this, e);

                }
                else if (!this.button.Enabled && this.button.Text.Equals("Close"))
                {
                    e.Cancel = true;
                }
            }
        }

        public string GetFolderPath(out DialogResult result)
        {
            string folderPath = null;
            DialogResult folderResult = DialogResult.Abort;
            this.Invoke((MethodInvoker)delegate
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.RootFolder = Environment.SpecialFolder.Desktop;
                dialog.SelectedPath = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%") + "\\wpilib\\simulation";


                folderResult = dialog.ShowDialog(this);
                folderPath =  dialog.SelectedPath;
            });
            result = folderResult;
            return folderPath;

        }


    }
}
