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
    /// <summary>
    /// A panel to use to create new links
    /// </summary>
    public partial class LinkSelectorPanel : UserControl
    {
        private static int width = 80;
        private static int height = 165;

        Cursor LinkAditionCursor;

        /// <summary>
        /// Creates a new link selection panel
        /// </summary>
        public LinkSelectorPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Triggered when a mouse selects the panel
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">the event arguments</param>
        private void LinkSelectorPanel_MouseDown(object sender, MouseEventArgs e)
        {
            using (Bitmap bmp = new Bitmap(CommandManager.NewLinkPic))
            {

                bmp.MakeTransparent(Color.White);
                LinkAditionCursor = new Cursor(bmp.GetHicon());
                SetCursor();
                //this.Cursor = cur;
                DoDragDrop(new Link("null", 0), DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private static Pen boxOutline = new Pen(Brushes.Black, 3);

        private static Pen DividerLine = new Pen(Brushes.DarkGray, 5);

        /// <summary>
        /// Paints the panel
        /// </summary>
        /// <param name="sender">Sending object</param>
        /// <param name="e">Paint arguments</param>
        private void LinkSelectorPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            Rectangle outline = new Rectangle(7, 7, width, height);

            g.DrawRectangle(boxOutline, outline);
            g.DrawImage(CommandManager.NewLinkPic, outline.X + 3, outline.Y + 3, outline.Width - 6, outline.Width - 6);
            StringFormat formater = new StringFormat();
            formater.Alignment = StringAlignment.Center;
            formater.LineAlignment = StringAlignment.Center;
            RectangleF textRect = new RectangleF(outline.X, outline.Y + outline.Width, outline.Width, outline.Height - outline.Width);
            g.DrawString("Add Link", new Font("Times New Roman", 10), Brushes.Black, textRect, formater);

            g.DrawLine(DividerLine, this.Width-2, 0, this.Width-2, outline.Height+15);

        }

        /// <summary>
        /// Changes the cursor to the desired image when dragging
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LinkSelectorPanel_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            SetCursor();
        }

        /// <summary>
        /// Sets the cursor to the desired image
        /// </summary>
        private void SetCursor()
        {

            Cursor.Current = LinkAditionCursor;
        }

        /// <summary>
        /// MAkes sure that the panel does not scale
        /// </summary>
        /// <param name="factor"></param>
        /// <param name="specified"></param>
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            factor.Height = 1;
            factor.Width = 1;
            base.ScaleControl(factor, specified);

        }
    }
}
