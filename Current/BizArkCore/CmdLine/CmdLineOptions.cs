using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BizArk.Core.AttributeExt;
using BizArk.Core.Template;

namespace BizArk.Core.CmdLine
{

    /// <summary>
    /// Command-line options.
    /// </summary>
    public class CmdLineOptions
    {
        
        #region Fields and Properties

        /// <summary>
        /// Gets or sets the title for the application. Shown at the top of the help text. Defaults to {AssemblyTitleAttribute} ver. {EntryAssembly.Version}
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the text that shows how to use the command-line. Shown in help.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets the long description for the console application. Shown in help.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the names/aliases of the default properties for the command-line.
        /// </summary>
        public string[] DefaultArgNames { get; set; }

        /// <summary>
        /// Gets or sets the string used to identify argument names.
        /// </summary>
        public string ArgumentPrefix { get; set; }

        /// <summary>
        /// Gets or sets the boolean property used to determine if the application should wait before exiting. Only used in ConsoleApplication.RunProgram().
        /// </summary>
        public string WaitArgName { get; set; }

        /// <summary>
        /// Gets or sets a value used to determine if the application should wait before exiting. Only used in ConsoleApplication.RunProgram(). If WaitProperty is set, this value will be set during initialization.
        /// </summary>
        public bool Wait { get; set; }

        #endregion

    }
}
