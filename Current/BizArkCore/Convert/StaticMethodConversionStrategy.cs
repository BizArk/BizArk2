using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace BizArk.Core.Convert
{

    /// <summary>
    /// Uses a typed constructor to convert the value.
    /// </summary>
    public class StaticMethodConversionStrategy
         : IConvertStrategy
    {

        /// <summary>
        /// Creates an instance of ConvertMethodConversionStrategy.
        /// </summary>
        /// <param name="mi"></param>
        public StaticMethodConversionStrategy(MethodInfo mi)
        {
            Method = mi;
        }

        /// <summary>
        /// Gets the method used to convert the value.
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return Method.Invoke(null, new object[] { value });
        }

    }
}
