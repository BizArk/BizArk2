using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.Convert
{
    /// <summary>
    /// Strategy used to just return null;
    /// </summary>
    public class NullConversionStrategy
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
            return null;
        }
    }
}
