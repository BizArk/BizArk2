using System;
using System.Data;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Defines the mapping of a property to a database field.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of ColumnAttribute.
        /// </summary>
        public ColumnAttribute()
        {
            IsIdentity = false;
            IsKey = false;
            DbType = (DbType)(-1); // We don't know what the type is yet, so give it an invalid value.
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the name of the field in the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the column is an identity field (int that increments in the database).
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the column is the key (or part of the key) in the database. This is useful for business keys as opposed to the identity. A key field is readonly when loaded from the database.
        /// </summary>
        public bool IsKey { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the column is readonly. The value can be set in code, but it will not be saved to the database.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets the data type for the database field.
        /// </summary>
        public DbType DbType { get; set; }
        
        #endregion

    }

}
