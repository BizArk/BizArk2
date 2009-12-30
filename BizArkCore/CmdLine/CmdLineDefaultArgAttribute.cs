using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.CmdLine
{
    /// <summary>
    /// Apply this attribute to the command-line class in order to define a default argument.
    /// This is the property that will be set if the first argument in the command-line 
    /// isn't an argument name. For example, if you want process a file name sent from
    /// Windows.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CmdLineDefaultArgAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of CmdLineDefaultArgAttribute.
        /// </summary>
        /// <param name="DefaultArgName"></param>
        public CmdLineDefaultArgAttribute(string DefaultArgName)
        {
            this.DefaultArgName = DefaultArgName;
        }

        /// <summary>
        /// The name of the property for the default argument.
        /// </summary>
        public string DefaultArgName { get; set; }
    }
}
