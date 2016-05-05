using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using GazeboExporter.UI;
using GazeboExporter.Storage;
using System.Xml;
using GazeboExporter.Export;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// This class represents an Attachment for the robot
    /// Attachments are some sort of actuator or sensor that is to be added to the robot
    /// to allow for simulation in FRCsim
    /// </summary>
    public abstract class Attachment : ISelectable
    {
        /// <summary>
        /// name of this attachment
        /// </summary>
        public String Name { get; protected set; }

        /// <summary>
        /// The Icon to be used to represent this attachment
        /// </summary>
        public Image Icon { get; protected set; }

        /// <summary>
        /// The storage model that stores this attachment
        /// </summary>
        protected StorageModel SwData { get; private set; }

        /// <summary>
        /// The location of this attachment in the storage model
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The link that contains this Attachment
        /// </summary>
        public Link ChildLink { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="swData">The storage model to store values in</param>
        /// <param name="path">The location of this attachment in the storage model</param>
        /// <param name="parentLink">The link that contains this attachment</param>
        protected Attachment(StorageModel swData, string path, Link parentLink)
        {
            this.SwData = swData;
            this.Path = path;
            this.ChildLink = parentLink;
        }

        /// <summary>
        /// Deletes the attachment
        /// </summary>
        public void Delete()
        {
            ChildLink.RemoveAttachment(this);
            SwData.DeleteAll(Path);
        }

        #region GUI

        private static Pen selectedOutline = new Pen(Brushes.Blue, 3);
        private static Pen graphOutline = new Pen(Brushes.Black, 3);

        public void DrawOnGraph(Graphics g, Rectangle r, bool selected)
        {
            if (selected)
            {
                selectedOutline.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(selectedOutline, r);
            }
            else
                g.DrawRectangle(graphOutline, r);
            if (Icon != null)
                g.DrawImage(Icon, r.X + 1, r.Y + 1, r.Width - 2, r.Height -2);
        }
        #endregion 

        #region ISelectable Implementation

        public virtual bool Selected { get; set; }

        public virtual string GetName()
        {
            return "Attachment";
        }

        public virtual Control GetEditorControl()
        {
            return null;
        }

        #endregion

        #region Export
        /// <summary>
        /// Verifies the attachment
        /// </summary>
        /// <param name="log">Logger to print messages to</param>
        /// <returns>returns true if verified successfully</returns>
        public abstract bool Verify(ProgressLogger log);
        
        /// <summary>
        /// Writes the Attachment to the URDF file
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="output">The XMLWriter to use to write the values</param>
        public abstract void WriteElements(ProgressLogger log, XmlWriter output);

        public abstract void WritePlugins(ProgressLogger log, XmlWriter output);

        public abstract void WriteSensor(ProgressLogger log, XmlWriter output);

        /// <summary>
        /// Writes the Attachment to the SDF file
        /// </summary>
        /// <param name="log">Logger to write messages to</param>
        /// <param name="output">The XMLWriter to use to write the values</param>
        //public abstract void WriteElementsSDF(ProgressLogger log, XmlWriter output);
        /*
            owner.WriteStartElement("sensor");
            {
                owner.WriteAttributeString("name", ParentLink + "Sensor");
                owner.WriteAttributeString("type", "sensor");
        
                SDFExporter.writeSDFElement(owner, "always_on", "true");

                owner.WriteStartElement("sensor");
                {

                    SDFExporter.writeSDFElement(owner, "", "");
                }
                owner.WriteEndElement();
        
                owner.WriteStartElement("");
                {

                }
                owner.WriteEndElement();
            }
            owner.WriteEndElement();
         */


        #endregion
    }
}
