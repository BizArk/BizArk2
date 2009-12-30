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
using System.ComponentModel;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Uses a TypeConverter to perform a conversion.
    /// </summary>
    public class TypeConverterConversionStrategy
        : IConvertStrategy
    {
        /// <summary>
        /// Creates an instance of TypeConverterConversionStrategy.
        /// </summary>
        /// <param name="converter"></param>
        public TypeConverterConversionStrategy(TypeConverter converter)
        {
            Converter = converter;
        }

        /// <summary>
        /// Creates an instance of TypeConverterConversionStrategy.
        /// </summary>
        /// <param name="converter"></param>
        /// <param name="destinationType"></param>
        public TypeConverterConversionStrategy(TypeConverter converter, Type destinationType)
        {
            Converter = converter;
            DestinationType = destinationType;
        }

        /// <summary>
        /// Gets the type converter used to convert the type.
        /// </summary>
        public TypeConverter Converter { get; private set; }

        /// <summary>
        /// Gets the type to convert to. If set, the Converter.ConvertTo method is used, otherwise, the Converter.ConvertFrom method is used.
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
            if (DestinationType == null)
                return Converter.ConvertFrom(value);
            else
                return Converter.ConvertTo(value, DestinationType);
        }
    }
}
