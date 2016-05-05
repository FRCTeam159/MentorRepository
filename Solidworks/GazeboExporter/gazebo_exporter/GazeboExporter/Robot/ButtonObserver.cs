using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// Button observers are objects that want to watch for a SolidWorks PMPage button to be pressed
    /// </summary>
    public interface ButtonObserver
    {
        /// <summary>
        /// Called when a button is pressed
        /// </summary>
        /// <param name="Id">The Id of the button that was pressed</param>
        void ButtonChanged(int Id);
    }
}
