using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.GazeboException
{
    /// <summary>
    /// An exception class to handle exceptions caused by Solidworks
    /// doing somehting it's not supposed to or throwing an undefined error.
    /// These exceptions will be hard to fix and debug.
    /// </summary>
    public class InternalSolidworksException : System.Exception
    {

        //The solidowrks method called that threw an unexpected/unrecoverable error
        //Use the form InterfaceName::MethodName
        public String swMethod { get; private set; }
        //Description of the thrown error
        public String reason { get; private set; }

        /// <summary>
        /// Creates an InternalSolidworksException object with the given 
        /// error method and reason
        /// </summary>
        /// <param name="swMethod">Solidworks method called that threw an unexpected/unrecoverable error</param>
        /// <param name="reason">Reason swMethod threw the error</param>
        public InternalSolidworksException(String swMethod, String reason)
        {
            //Initialize properties
            this.swMethod = swMethod;
            this.reason = reason;
        }

    }
}
