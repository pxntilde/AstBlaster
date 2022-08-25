using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AstBlaster.Utils.Exceptions
{
    public class Error : Exception
    {
        public Error(String message) : base(message)
        {
            GD.PrintErr(message);
        }
    }

    /// <summary>
    /// Error when argument is out of the allowable range by the function
    /// </summary>
    public class ArgumentOutOfRangeError : ArgumentOutOfRangeException
    {
        public ArgumentOutOfRangeError(String argument) : base(argument)
        {
            GD.PrintErr(argument);
        }

        public ArgumentOutOfRangeError(String argument, String message) : base(argument, message)
        {
            GD.PrintErr($"{argument} {message}");
        }
    }
}
