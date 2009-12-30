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

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Converts from a string to a bool.
    /// </summary>
    public class StringToBoolConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            // Let's try a few common boolean variations. 
            string[] trueValues = new string[] { "true", "t", "yes", "1", "-1" };
            string[] falseValues = new string[] { "false", "f", "no", "0" };
            string strVal = ((string)value).ToLowerInvariant();
            if (trueValues.Contains(strVal)) return true;
            if (falseValues.Contains(strVal)) return false;
            return false;
        }

    }
}
