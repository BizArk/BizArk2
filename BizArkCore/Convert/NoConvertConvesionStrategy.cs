using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Strategy used to do no conversion at all. Just returns the value that was sent in.
    /// </summary>
    public class NoConvertConversionStrategy
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
            return value;
        }

    }
}
