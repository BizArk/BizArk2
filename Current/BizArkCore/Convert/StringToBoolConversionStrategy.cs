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

        static StringToBoolConversionStrategy()
        {
            TrueValues = new List<string>() { "true", "t", "yes" };
        }

        /// <summary>
        /// Gets the list of values that will equate to True. Everything else is false.
        /// </summary>
        public static List<string> TrueValues { get; private set; }

        /// <summary>
        /// Converts the string to a boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            string strVal = value as string;
            if (strVal == null) return false;
            
            int i;
            if (int.TryParse(strVal, out i))
                return i != 0; // only false if i == 0.

            foreach (var trueVal in TrueValues)
            {
                if (trueVal.Equals(strVal, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }

            // Everything else is false.
            return false;
        }

    }
}
