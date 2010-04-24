using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ConvertibleConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertibleConversionStrategy.
        /// </summary>
        /// <param name="toType"></param>
        public ConvertibleConversionStrategy(Type toType)
        {
            ConversionType = toType;
            //Provider = provider;
        }

        //private const int cEmptyIndex = 0;
        private const int cObjectIndex = 1;
        private const int cDBNullIndex = 2;
        private const int cBoolIndex = 3;
        private const int cCharIndex = 4;
        private const int cSbyteIndex = 5;
        private const int cByteIndex = 6;
        private const int cShortIndex = 7;
        private const int cUshortIndex = 8;
        private const int cIntIndex = 9;
        private const int cUintIndex = 10;
        private const int cLongIndex = 11;
        private const int cUlongIndex = 12;
        private const int cFloatIndex = 13;
        private const int cDoubleIndex = 14;
        private const int cDecimalIndex = 15;
        private const int cDateTimeIndex = 16;
        //private const int cObject2Index = 17;
        private const int cStringIndex = 18;

        private static Type[] ConvertTypes = new Type[] 
        { 
            typeof(object), // not used but needed as a place holder in the array (was Empty).
            typeof(object), 
            typeof(DBNull), 
            typeof(bool), 
            typeof(char), 
            typeof(sbyte), 
            typeof(byte), 
            typeof(short), 
            typeof(ushort), 
            typeof(int), 
            typeof(uint), 
            typeof(int), 
            typeof(uint), 
            typeof(float), 
            typeof(double), 
            typeof(decimal), 
            typeof(DateTime), 
            typeof(object), // not used but needed as a place holder in the array.
            typeof(string)
        };

        /// <summary>
        /// Gets the type of conversion to perform.
        /// </summary>
        public Type ConversionType { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            IConvertible convertible = value as IConvertible;
            if (convertible == null)
                throw new InvalidCastException(String.Format("Invalid cast. {0} does not implement the IConvertible interface.", value));

            if (ConversionType == ConvertTypes[cBoolIndex])
                return convertible.ToBoolean(provider);
            if (ConversionType == ConvertTypes[cCharIndex])
                return convertible.ToChar(provider);
            if (ConversionType == ConvertTypes[cSbyteIndex])
                return convertible.ToSByte(provider);
            if (ConversionType == ConvertTypes[cByteIndex])
                return convertible.ToByte(provider);
            if (ConversionType == ConvertTypes[cShortIndex])
                return convertible.ToInt16(provider);
            if (ConversionType == ConvertTypes[cUshortIndex])
                return convertible.ToUInt16(provider);
            if (ConversionType == ConvertTypes[cIntIndex])
                return convertible.ToInt32(provider);
            if (ConversionType == ConvertTypes[cUintIndex])
                return convertible.ToUInt32(provider);
            if (ConversionType == ConvertTypes[cLongIndex])
                return convertible.ToInt64(provider);
            if (ConversionType == ConvertTypes[cUlongIndex])
                return convertible.ToUInt64(provider);
            if (ConversionType == ConvertTypes[cFloatIndex])
                return convertible.ToSingle(provider);
            if (ConversionType == ConvertTypes[cDoubleIndex])
                return convertible.ToDouble(provider);
            if (ConversionType == ConvertTypes[cDecimalIndex])
                return convertible.ToDecimal(provider);
            if (ConversionType == ConvertTypes[cDateTimeIndex])
                return convertible.ToDateTime(provider);
            if (ConversionType == ConvertTypes[cStringIndex])
                return convertible.ToString(provider);
            if (ConversionType == ConvertTypes[cObjectIndex])
                return value;
            return convertible.ToType(ConversionType, provider);
        }

        /// <summary>
        /// Determines if IConvertible can convert to the given type.
        /// </summary>
        /// <param name="toType"></param>
        /// <returns></returns>
        public static bool CanConvertTo(Type toType)
        {
            foreach (var cType in ConvertTypes)
                if (toType == cType) return true;
            return false;
        }

    }
}
