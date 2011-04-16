using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text;
using BizArk.Core;
using BizArk.Core.AttributeExt;
using BizArk.Core.TypeExt;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Represents a property on an entity. Properties are defined once per entity type (not per instance).
    /// </summary>
    public class EntityProperty
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates a new instance of TfEntityProperty.
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="prop"></param>
        /// <param name="colAtt"></param>
        internal EntityProperty(Type entityType, PropertyInfo prop, ColumnAttribute colAtt)
        {
            EntityType = entityType;
            Property = prop;

            ColumnName = colAtt.Name;
            IsIdentity = colAtt.IsIdentity;
            IsReadOnly = colAtt.IsReadOnly;
            IsKey = colAtt.IsKey;
            var reqAtt = prop.GetAttribute<RequiredAttribute>(true);
            if (reqAtt == null)
            {
                AllowNull = prop.PropertyType.IsNullable();
                IsRequired = false;
            }
            else
            {
                AllowNull = false;
                IsRequired = true;
                RequiredAtt = reqAtt;
            }
            if (colAtt.DbType < 0)
                DbType = ConvertEx.ChangeType<DbType>(prop.PropertyType);
            else
                DbType = colAtt.DbType;

            var sizeAtt = prop.GetAttribute<StringLengthAttribute>(true);
            if (sizeAtt != null && sizeAtt.MaximumLength > 0)
                Size = sizeAtt.MaximumLength;

            var displayAtt = prop.GetAttribute<DisplayAttribute>(true);
            if (displayAtt != null)
                DisplayName = displayAtt.GetName();
            if (string.IsNullOrEmpty(DisplayName))
                DisplayName = Name;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get { return Property.Name; } }

        /// <summary>
        /// Gets the name of the column in the database to map this property to.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Gets the name of the property for display purposes. Comes from the DisplayAttribute on the property.
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the Type for the entity that declares this value.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets the type for the property.
        /// </summary>
        public Type PropertyType { get { return Property.PropertyType; } }

        /// <summary>
        /// Gets the PropertyInfo object.
        /// </summary>
        public PropertyInfo Property { get; private set; }

        /// <summary>
        /// Gets a value that determines if DbNull values are allowed in the database.
        /// </summary>
        public bool AllowNull { get; private set; }

        /// <summary>
        /// Gets a value that determines the size of the field in the database (for db types that allow variable lengths).
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Gets a value that determines if this is the identity property (auto-increment).
        /// </summary>
        public bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets a value that determines if this is part of the key.
        /// </summary>
        public bool IsKey { get; private set; }

        /// <summary>
        /// Gets a value that determines if the property will be inserted/updated in the database when the entity is saved.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets the data type for the database field.
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Gets a value that indicates if the value is required in the database. Comes from the RequiredAttribute on the property.
        /// </summary>
        public bool IsRequired { get; private set; }

        /// <summary>
        /// Used to get message for missing required fields.
        /// </summary>
        internal RequiredAttribute RequiredAtt { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of TfEntityProperty.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1}.{2}", Property.PropertyType.GetCSharpName(), EntityType.Name, Name);
            if (!string.IsNullOrEmpty(ColumnName))
                sb.AppendFormat(" ({0})", ColumnName);
            return sb.ToString();
        }

        #endregion

    }

}
