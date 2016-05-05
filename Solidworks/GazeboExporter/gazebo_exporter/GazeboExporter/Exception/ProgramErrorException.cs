using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.GazeboException
{
    /// <summary>
    /// An exception class to handle errors or bugs in the add in program.
    /// This add-in should ideally never throw one of these exceptions.
    /// </summary>
    public class ProgramErrorException : System.Exception
    {

        public String error { get; private set; }

        public ProgramErrorException(String error)
        {
            this.error = error;
        }

        
    }
}
