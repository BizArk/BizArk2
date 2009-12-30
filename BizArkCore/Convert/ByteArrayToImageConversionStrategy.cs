using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ByteArrayToImageConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertibleConversionStrategy.
        /// </summary>
        public ByteArrayToImageConversionStrategy()
        {
        }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            var imgBytes = value as byte[];
            if (imgBytes == null) return null;

            MemoryStream ms = new MemoryStream(imgBytes, 0, imgBytes.Length);
            ms.Write(imgBytes, 0, imgBytes.Length);
            return Image.FromStream(ms, true);
        }

    }
}
