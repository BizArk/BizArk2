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
