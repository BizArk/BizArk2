using System;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Converts to enumeration values.
    /// </summary>
    public class EnumConversionStrategy
        : IConvertStrategy
    {
        /// <summary>
        /// Creates an instance of ParseConversionStrategy.
        /// </summary>
        public EnumConversionStrategy(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new ArgumentException("enumType must be an Enumeration.");
            EnumType = enumType;
        }

        /// <summary>
        /// Gets the method used to parse the value.
        /// </summary>
        public Type EnumType { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            string s = value as string;
            if (s != null) return Enum.Parse(EnumType, s);

            return Enum.ToObject(EnumType, value);
        }
    }
}
