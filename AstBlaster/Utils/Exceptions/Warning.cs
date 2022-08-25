using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace AstBlaster.Utils.Exceptions
{
    /// <summary>
    /// Static warning functions
    /// </summary>
    public static class Warning
    {
        /// <summary>
        /// Prints a warning message to the console
        /// </summary>
        /// <param name="message">Message to print</param>
        public static void Print(String message)
        {
            GD.PushWarning(message);
        }

        /// <summary>
        /// Prints a warning message to the console
        /// </summary>
        /// <param name="node">Node emitting the warning</param>
        /// <param name="message">Message to print</param>
        public static void Print(Node node, String message)
        {
            GD.PushWarning($"{node} :: {message}");
        }
    }
}
