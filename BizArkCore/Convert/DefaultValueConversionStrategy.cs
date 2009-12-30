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
    /// Strategy used to return the default value for a type;
    /// </summary>
    public class DefaultValueConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of DefaultValueConversionStrategy.
        /// </summary>
        /// <param name="destinationType"></param>
        public DefaultValueConversionStrategy(Type destinationType)
        {
            DestinationType = destinationType;
        }

        /// <summary>
        /// Gets the type of empty value to get.
        /// </summary>
        public Type DestinationType { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return ConvertEx.GetDefaultEmptyValue(DestinationType);
        }
    }
}
