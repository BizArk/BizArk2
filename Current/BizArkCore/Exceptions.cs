/* Author: Brian Brewder
 * Website: http://redwerb.com
 * 
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.CmdLine
{
    /// <summary>
    /// Exception thrown when multiple command-line properties match a given argument name.
    /// </summary>
    public class AmbiguousCmdLineNameException
        : ApplicationException
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
