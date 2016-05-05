using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This interface represents the property page that will correspond to each attachment
    /// </summary>
    public interface AttachmentProperties
    {
        /// <summary>
        /// remove the attachment from the robot
        /// </summary>
       void RemoveAttachment();
    }
}
