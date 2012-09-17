﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BizArk.Core.Convert
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