using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GazeboExporter.UI
{
    public class FancyPanel : Panel
    {
        public FancyPanel()
        {
            DoubleBuffered = true;
            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            System.Diagnostics.Debug.WriteLine("Fancy Panel Constructor");
            System.Diagnostics.Debug.WriteLine("Done.");
        }
    }
}
