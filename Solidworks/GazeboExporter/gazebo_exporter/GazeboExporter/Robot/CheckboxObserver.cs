using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.Robot
{
    /// <summary>
    /// A CheckboxObserver is an object that watches for a SolidWorks PMPage checkbox to be checked or unchecked
    /// </summary>
    public interface CheckboxObserver
    {
        /// <summary>
        /// Called when any checkbox is changed
        /// </summary>
        /// <param name="Id">Id of the checkbox that changed</param>
        /// <param name="isChecked">The new state of the checkbox</param>
        void CheckboxChanged(int Id, bool isChecked);
    }
}
