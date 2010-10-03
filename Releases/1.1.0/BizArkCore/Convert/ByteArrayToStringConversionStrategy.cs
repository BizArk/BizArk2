using System;
using System.Text;

namespace BizArk.Core.Convert
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ByteArrayToStringConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertibleConversionStrategy.
        /// </summary>
        public ByteArrayToStringConversionStrategy(Encoding encoding)
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
            var strBytes = value as byte[];
            if (strBytes == null) return null;

            return mEncoding.GetString(strBytes);
        }

    }
}
