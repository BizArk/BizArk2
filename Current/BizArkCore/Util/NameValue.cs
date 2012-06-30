using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizArk.Core.Extensions.FormatExt;

namespace BizArk.Core.Util
{

    /// <summary>
    /// Represents a named value.
    /// </summary>
    public class NameValue
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of NameValue.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="valueType"></param>
        public NameValue(string name, object value, Type valueType)
        {
            Name = name;
            Value = value;
            ValueType = valueType;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the name for the pair.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value for the pair.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets the expected type for the value.
        /// </summary>
        public Type ValueType { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Provides a debug friendly string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var str = Value as string;
            if (Value == null)
                str = "[NULL]";
            else if (str == null)
                str = ConvertEx.ToString(Value);
            else
                str = "\"" + str + "\"";
            return "{0}={1}".Fmt(Name, str);
        }

        #endregion

    }

}
