using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GazeboExporter.GazeboException
{

    /// <summary>
    /// An exception class to handle exceptions due to user error
    /// the addin should catch these exceptions and give the user an appopriate response.
    /// if not cauth, they should turn into ProgramErrorExcetpion
    /// </summary>
    public class UserErrorException : System.Exception
    {
    }
}
