using System;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.Core.CmdLine
{

    /// <summary>
    /// Command-line options.
    /// </summary>
    public class CmdLineOptions
    {
        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of CmdLineOptions.
        /// </summary>
        public CmdLineOptions()
        {
            ArgumentPrefix = "/";
	        AssignmentDelimiter = ' ';
            if (Application.Title.IsEmpty())
                Title = "Command-line options.";
            else if (Application.Version == null)
                Title = Application.Title;
            else
                Title = string.Format("{0} ver. {1}", Application.Title, Application.Version);
            ApplicationName = Application.ExeName;
            Comparer = StringComparison.OrdinalIgnoreCase;
            ArraySeparator = ",";
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the title for the application. Shown at the top of the help text. Defaults to {AssemblyTitleAttribute} ver. {EntryAssembly.Version}
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the name of the application for use in the usage text. Defaults to the name of the exe that is running.
        /// </summary>
        public string ApplicationName { get; set; }

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

        /// <summary>
        /// Gets or sets the rule for comparing the names/aliases. By default this is set to 
        /// </summary>
        public StringComparison Comparer { get; set; }

		/// <summary>
		/// Gets or sets the delimetert between the argument name and its value
		/// </summary>
	    public char AssignmentDelimiter { get; set; }

        /// <summary>
        /// Gets or sets array elements separator, default value is ","
        /// </summary>
        public string ArraySeparator { get; set; }

        #endregion

    }
}
