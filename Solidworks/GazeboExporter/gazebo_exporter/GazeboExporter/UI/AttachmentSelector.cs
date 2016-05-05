using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GazeboExporter.Robot;

namespace GazeboExporter.UI
{
    public partial class AttachmentSelector : UserControl
    {


        private Rectangle clientArea;
        public const int totalHeight = 165; //in Pixels
        private int totalWidth; // in Pixels

        private Image dragImage;

        public const int attachmentOptionWidth = 85; // CommandManager.MotorPic.Width + 11; //All attachment options things are the same size (74) plus border // in Pixels

        Cursor attAddCursor;

        public AttachmentSelector()
        {
            InitializeComponent();

            clientArea = new Rectangle(0, 0, this.Width, this.Height);
            
            attachmentMap = new Dictionary<Rectangle, AttachmentDescriptor>();

            int xpos = 0;
            foreach (AttachmentDescriptor ad in AttachmentFactory.allAttachmentDescriptors)
            {
                attachmentMap.Add(new Rectangle(xpos, 5, 80, totalHeight), ad);
                xpos += attachmentOptionWidth;
            }

            Refresh();
        }


        public override void Refresh()
        {
            totalWidth = attachmentOptionWidth*AttachmentFactory.allAttachmentDescriptors.Count;//Attachments.count;
            clientArea.Width = this.Width;

            //System.Diagnostics.Debug.WriteLine("height: " + this.Height);
            //System.Diagnostics.Debug.WriteLine(clientArea.Width);


            if (totalWidth < clientArea.Width)
            {
                hScrollBar1.Enabled = false;
                clientArea.X = 0;
            }
            else
            {
                hScrollBar1.Enabled = true;
                hScrollBar1.Maximum = ((int)totalWidth - clientArea.Width) / 10 + hScrollBar1.LargeChange;
            }


            panel.Invalidate();
        }

        private Dictionary<Rectangle, AttachmentDescriptor> attachmentMap;

        private static Pen graphOutline = new Pen(Brushes.Black, 3);

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            AttachmentDescriptor descriptor;
            foreach (Rectangle r in attachmentMap.Keys)
            {
                int xPos = r.X - clientArea.X;

                descriptor = attachmentMap[r];
                g.DrawRectangle(graphOutline, new Rectangle(xPos,r.Y,r.Width,r.Height));
                g.DrawImage(descriptor.icon, xPos + 3, r.Y + 3, r.Width - 6, r.Width - 6);
                StringFormat formater = new StringFormat();
                formater.Alignment = StringAlignment.Center;
                formater.LineAlignment = StringAlignment.Center;
                RectangleF textRect = new RectangleF(xPos, r.Y + r.Width, r.Width, r.Height - r.Width);
                g.DrawString(descriptor.name, new Font("Times New Roman", 10), Brushes.Black, textRect, formater);

            }
        }

        private void panel_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Rectangle r in attachmentMap.Keys)
            {
                if (r.Contains(e.X + clientArea.X, e.Y))
                {
                    dragImage = attachmentMap[r].icon;
                    using (Bitmap bmp = new Bitmap(dragImage))
                    {

                        bmp.MakeTransparent(Color.White);
                        attAddCursor = new Cursor(bmp.GetHicon());
                        SetCursor();

                        panel.DoDragDrop(attachmentMap[r], DragDropEffects.Copy | DragDropEffects.Move);
                    }

                }
            }
        }

        private void panel_Resize(object sender, EventArgs e)
        {

            
            clientArea.Width = this.Width;
            Refresh();

        }

        private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {
            clientArea.X = e.NewValue * 10;
            panel.Invalidate();

            panel.Refresh();
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            factor.Height = 1;
            base.ScaleControl(factor, specified);

        }

        private void AttachmentSelector_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            SetCursor();
        }

        private void SetCursor()
        {

            Cursor.Current = attAddCursor;
        }
    }
}
