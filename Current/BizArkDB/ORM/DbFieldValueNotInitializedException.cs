using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizArk.Core.Extensions.FormatExt;

namespace BizArk.DB.ORM
{

    /// <summary>
    /// Exception thrown if a value is attempted to be retrieved that has not been initialized yet.
    /// </summary>
    public class DbFieldValueNotInitializedException : ApplicationException
    {

        /// <summary>
        /// Creates a new instance of DbFieldValueNotInitializedException.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="original"></param>
        public DbFieldValueNotInitializedException(DbFieldValue value, bool original = false)
            : base("The {0}value for {1} is not initialized.".Fmt(original ? "original " : "", value.FieldName))
        {
            Value = value;
        }

        /// <summary>
        /// Gets the field associated with this exception.
        /// </summary>
        public DbFieldValue Value { get; private set; }

    }
}
