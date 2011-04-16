using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace BizArk.Core.Data
{
    /// <summary>
    /// Field value used for generating sql commands.
    /// </summary>
    public class FieldValue
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of FieldValue.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public FieldValue(string name = null, object value = null)
        {
            Name = name;
            Value = value;
            DbType = (DbType)(-1);
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the field in the database. If this is empty, uses the name property.
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets a value that determines if the value is DBNull.
        /// </summary>
        public bool IsDBNull
        {
            get { return Value == DBNull.Value; }
        }

        /// <summary>
        /// Gets or sets the database type. Set to (SqlDbType)(-1) to use the default.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// Gets or sets the size of the field in the database for fields that accept a size parameter.
        /// </summary>
        public int Size { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the value to DBNull.
        /// </summary>
        public void SetDBNull()
        {
            Value = DBNull.Value;
        }

        #endregion

    }
}
