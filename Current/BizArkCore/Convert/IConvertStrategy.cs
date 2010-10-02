using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.Convert
{
    /// <summary>
    /// Interface for defining conversion strategies. Used in ConvertEx. Each strategy object should be used to convert from exactly one type to another.
    /// </summary>
    public interface IConvertStrategy
    {
        /// <summary>
        /// Changes the type of the value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        object Convert(object value, IFormatProvider provider);
    }
}
