using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.Convert
{
    /// <summary>
    /// Converts from a string to a bool.
    /// </summary>
    public class StringToBoolConversionStrategy
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
            // Let's try a few common boolean variations. 
            string[] trueValues = new string[] { "true", "t", "yes", "1", "-1" };
            string[] falseValues = new string[] { "false", "f", "no", "0" };
            string strVal = ((string)value).ToLowerInvariant();
            if (trueValues.Contains(strVal)) return true;
            if (falseValues.Contains(strVal)) return false;
            return false;
        }

    }
}
