using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Export
{
    /// <summary>
    /// Interface to allow for other classes to acess the logger
    /// </summary>
    public interface ProgressLogger
    {
        /// <summary>
        /// Writes a message to the logger
        /// </summary>
        /// <param name="message">The message to be written</param>
        /// <param name="isOp">If the message should make the progress bar increment</param>
        void WriteMessage(String message, bool isOp = true);

        /// <summary>
        /// Writes a warning message to the logger
        /// </summary>
        /// <param name="warning">The warning to be written</param>
        /// <param name="isOp">If the warning should increment the progress bar</param>
        void WriteWarning(String warning, bool isOp = false);

        /// <summary>
        /// Writes an error to the logger
        /// </summary>
        /// <param name="error">The error to be written</param>
        /// <param name="isOp">If the error should increment the progress bar</param>
        void WriteError(String error, bool isOp = false);
    }
}
