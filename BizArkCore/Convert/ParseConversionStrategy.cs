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
using System.Reflection;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Calls the static parse method of a given type.
    /// </summary>
    public class ParseConversionStrategy
        : IConvertStrategy
    {
        /// <summary>
        /// Creates an instance of ParseConversionStrategy.
        /// </summary>
        /// <param name="mi"></param>
        public ParseConversionStrategy(MethodInfo mi)
        {
            ParseMethod = mi;
        }

        /// <summary>
        /// Gets the method used to parse the value.
        /// </summary>
        public MethodInfo ParseMethod { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            string s = value as string;
            if (s == null) return null;
            return ParseMethod.Invoke(null, new object[] { s });
        }
    }
}
