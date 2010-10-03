using System;
using System.Text;

namespace BizArk.Core.Convert
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class StringToByteArrayConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertibleConversionStrategy.
        /// </summary>
        public StringToByteArrayConversionStrategy(Encoding encoding)
        {
            mEncoding = encoding;
        }

        private Encoding mEncoding = ASCIIEncoding.ASCII;
        /// <summary>
        /// Gets the encoding for the conversion.
        /// </summary>
        public Encoding Encoding
        {
            get { return mEncoding; }
        }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            var str = value as string;
            if (str == null) return null;

            return mEncoding.GetBytes(str);
        }

    }
}
