using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.Convert
{
    /// <summary>
    /// Strategy used to throw an InvalidCastException.
    /// </summary>
    public class InvalidConversionStrategy
        : IConvertStrategy
    {
        /// <summary>
        /// Creates an instance of InvalidConversionStrategy.
        /// </summary>
        /// <param name="message"></param>
        public InvalidConversionStrategy(string message)
        {
            Message = message;
        }

        /// <summary>
        /// Gets the message used in the exception.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            throw new InvalidCastException(Message);
        }

    }
}
