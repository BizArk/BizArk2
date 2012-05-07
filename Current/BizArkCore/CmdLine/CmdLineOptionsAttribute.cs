using System;

namespace BizArk.Core.CmdLine
{
    /// <summary>
    /// Apply this attribute to the command-line class in order to define options for 
    /// the command-line object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CmdLineOptionsAttribute : Attribute
    {

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
        /// Gets or sets the name/alias of the default property for the command-line. Setting this overwrites DefaultArgNames.
        /// </summary>
        public string DefaultArgName 
        {
            get
            {
                if (DefaultArgNames == null) return "";
                if (DefaultArgNames.Length == 0) return "";
                return DefaultArgNames[0];
            }
            set
            {
                DefaultArgNames = new string[] { value };
            }
        }

        /// <summary>
        /// Gets or sets the names/aliases of the default properties for the command-line. Setting this overwrites DefaultArgName.
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
        public bool? Wait { get; set; }

        /// <summary>
        /// Gets or sets the rule for comparing the names/aliases. By default this is set to 
        /// </summary>
        public StringComparison? Comparer { get; set; }

        /// <summary>
        /// Creates the options object.
        /// </summary>
        /// <returns></returns>
        internal CmdLineOptions CreateOptions()
        {
            var options = new CmdLineOptions();

            if (Title != null)
                options.Title = Title;
            if (ApplicationName != null)
                options.ApplicationName = ApplicationName;
            if (Usage != null)
                options.Usage = Usage;
            if (Description != null)
                options.Description = Description;
            if (DefaultArgNames != null)
                options.DefaultArgNames = DefaultArgNames;
            if (ArgumentPrefix != null)
                options.ArgumentPrefix = ArgumentPrefix;
            if (WaitArgName != null)
                options.WaitArgName = WaitArgName;
            if (Wait != null)
                options.Wait = (bool)Wait;
            if (Comparer != null)
                options.Comparer = Comparer.Value;

            return options;
        }

    }
}
