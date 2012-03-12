using System;
using System.Collections.Generic;

namespace BizArk.Core.CmdLine
{

    /// <summary>
    /// Base class for exceptions used for command-line parsing.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    public class CmdLineException : ApplicationException
    {

        /// <summary>
        /// Creates an instance of CmdLineException.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CmdLineException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }

    }

    /// <summary>
    /// Exception thrown if there is a problem with a command line arugment definition.
    /// </summary>
    public class CmdLineArgumentException : CmdLineException
    {

        /// <summary>
        /// Creates an instance of CmdLineArgumentException.
        /// </summary>
        /// <param name="message"></param>
        public CmdLineArgumentException(string message)
            : base(message)
        {
        }

    }

    /// <summary>
    /// Exception thrown when multiple command-line properties match a given argument name.
    /// </summary>
    public class AmbiguousCmdLineNameException
        : CmdLineException
    {

        /// <summary>
        /// Creates an instance of AmbiguousCmdLineNameException.
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="props"></param>
        public AmbiguousCmdLineNameException(string argName, params CmdLineProperty[] props)
            : base(string.Format("The command-line argument name '{0}' matches the following command-line properties: {1}. You must disambiguate the argument name by using either the shortcut or include additional characters in the name.", argName, string.Join(", ", GetPropertyNames(props))))
        {
            ArgName = argName;
            AmbiguousProperties = props;
        }

        /// <summary>
        /// Gets the name of the invalid argument.
        /// </summary>
        public string ArgName { get; private set; }

        /// <summary>
        /// Gets the conflicting properties.
        /// </summary>
        public CmdLineProperty[] AmbiguousProperties { get; private set; }

        private static string[] GetPropertyNames(params CmdLineProperty[] props)
        {
            var names = new List<string>();
            foreach (var prop in props)
                names.Add(prop.Name);
            return names.ToArray();
        }
    }
}
