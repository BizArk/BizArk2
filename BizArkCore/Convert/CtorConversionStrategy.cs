using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Redwerb.BizArk.Core.Convert
{

    /// <summary>
    /// Uses a typed constructor to convert the value.
    /// </summary>
    public class CtorConversionStrategy
         : IConvertStrategy
    {
    
        /// <summary>
        /// Creates an instance of ConvertMethodConversionStrategy.
        /// </summary>
        /// <param name="mi"></param>
        public CtorConversionStrategy(ConstructorInfo mi)
        {
            Ctor = mi;
        }

        /// <summary>
        /// Gets the method used to convert the value.
        /// </summary>
        public ConstructorInfo Ctor { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return Ctor.Invoke(new object[] { value });
        }

}
}
