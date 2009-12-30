using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Uses the IConvertible interface to convert the value.
    /// </summary>
    public class ImageToByteArrayConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertibleConversionStrategy.
        /// </summary>
        public ImageToByteArrayConversionStrategy()
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
            var img = value as Image;
            if (img == null) return null;

            using (MemoryStream ms = new MemoryStream())
            {
                if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
                    img.Save(ms, ImageFormat.Bmp);
                else
                    img.Save(ms, img.RawFormat);
                return ms.ToArray();
            }
        }

    }
}
