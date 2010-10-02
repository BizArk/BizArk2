using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.CmdLine
{
    /// <summary>
    /// Apply this attribute to a property in order to allow the CmdLineProcessor to set the value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class CmdLineArgAttribute : Attribute
    {
        /// <summary>
        /// Creates an instance of CmdLineArgAttribute.
        /// </summary>
        public CmdLineArgAttribute()
        {
            Required = false;
            Usage = "";
            ShowInUsage = DefaultBoolean.Default;
            AllowSave = true;
            Aliases = new string[] { };
        }

        /// <summary>
        /// Gets or sets the alias for the command-line argument. It is recommended that this be a single character. This will overwrite Aliases.
        /// </summary>
        public string Alias 
        {
            get
            {
                if (Aliases == null) return "";
                if (Aliases.Length == 0) return "";
                return Aliases[0];
            }
            set
            {
                Aliases = new string[] { value };
            }
        }

        /// <summary>
        /// Gets or sets the aliases for the command-line argument. Aliases cannot conflict with one another. It is recommended that the first alias be a single character. This will overwrite Alias.
        /// </summary>
        public string[] Aliases { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the command-line argument must be specified.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the short description that should be used in the usage description.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the property should be saved.
        /// </summary>
        public bool AllowSave { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the argument should be displayed in the usage. By default, only required arguments and help are displayed in the usage in order to save space when printing the usage.
        /// </summary>
        public DefaultBoolean ShowInUsage { get; set; }
    }
}
