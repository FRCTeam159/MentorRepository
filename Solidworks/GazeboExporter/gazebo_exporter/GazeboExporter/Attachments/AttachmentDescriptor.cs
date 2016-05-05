using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Storage;
using System.Drawing;

namespace GazeboExporter.Robot
{
    
    [Serializable]
    public class AttachmentDescriptor
    {
        /// <summary>
        /// icon for this attachment descriptor
        /// </summary>
        public Image icon { get; private set; }

        /// <summary>
        /// name of this attachment descriptor
        /// </summary>
        public string name {get; private set;}

        /// <summary>
        /// id of this attachment descriptor
        /// </summary>
        public int id { get; private set; }

        /// <summary>
        /// Used to store the descritions for attachments in the attachment factory
        /// </summary>
        public AttachmentDescriptor(string name, Image icon, int id)
        {
            this.name = name;
            this.icon = icon;
            this.id = id;
        }
    }
}
