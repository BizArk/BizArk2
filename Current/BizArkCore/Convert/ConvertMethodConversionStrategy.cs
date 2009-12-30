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
    /// Uses a conversion method to convert the value.
    /// </summary>
    public class ConvertMethodConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertMethodConversionStrategy.
        /// </summary>
        /// <param name="mi"></param>
        public ConvertMethodConversionStrategy(MethodInfo mi)
        {
            ConversionMethod = mi;
        }

        /// <summary>
        /// Gets the method used to convert the value.
        /// </summary>
        public MethodInfo ConversionMethod { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return ConversionMethod.Invoke(value, null);
        }

    }
}
