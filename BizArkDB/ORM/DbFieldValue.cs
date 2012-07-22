using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core;
using BizArk.Core.Convert.Strategies;
using BizArk.Core.Extensions.FormatExt;

namespace BizArk.DB.ORM
{

    /// <summary>
    /// Provides basic properties/methods for a DbValue.
    /// </summary>
    public abstract class DbFieldValue
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DbValue.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="key"></param>
        /// <param name="identity"></param>
        public DbFieldValue(string fieldName, bool key, bool identity)
        {
            FieldName = fieldName;
            IsKey = key;
            IsIdentity = identity;
            FieldType = DbTypeMap.ToDbType(GetValueType());
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the object that this value is for.
        /// </summary>
        public ISupportDbState Owner { get; internal set; }

        /// <summary>
        /// Gets the name of the database field.
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets a value that determines if this value has been initialized. If false, attempting to get the value will throw an exception.
        /// </summary>
        public bool IsInitialized { get; internal set; }

        /// <summary>
        /// Gets a value that determines if the original value has been initialized. If false, attempting to get the original value will throw an exception.
        /// </summary>
        public bool IsOriginalInitialized { get; internal set; }

        /// <summary>
        /// Gets a value that determines if this field is part of the key.
        /// </summary>
        public bool IsKey { get; private set; }

        /// <summary>
        /// Gets a value that determines if this field is a auto-incremented identity value. If true, the value will be retrieved automatically when a new record is saved to the DB.
        /// </summary>
        public bool IsIdentity { get; private set; }

        /// <summary>
        /// Gets or sets the datatype to use in the database. Defaults to the DbType that maps to values type.
        /// </summary>
        public DbType FieldType { get; set; }

        /// <summary>
        /// Gets or sets the maximum size of the value. Defaults to the size of the string or byte array or 0.
        /// </summary>
        public int FieldSize { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the value of the field.
        /// </summary>
        /// <param name="value"></param>
        public abstract void SetValue(object value);

        /// <summary>
        /// Gets the value of the field.
        /// </summary>
        /// <returns></returns>
        public abstract object GetValue();

        /// <summary>
        /// Gets the expected type for the value.
        /// </summary>
        /// <returns></returns>
        public abstract Type GetValueType();

        /// <summary>
        /// Sets the original value to the current value.
        /// </summary>
        public abstract void SetOriginal();

        /// <summary>
        /// Gets a value that determines if the value is different than the original value. True if the value has been set but the original value hasn't been. False if the value has not been set.
        /// </summary>
        public abstract bool IsModified { get; }

        /// <summary>
        /// Displays the field name.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            object val;
            if (IsInitialized)
            {
                val = GetValue();
                if (val == null)
                {
                    val = "[NULL]";
                }
                else
                {
                    var str = val as string;
                    if (str != null) val = "\"" + str + "\"";
                }
            }
            else
            {
                val = "[NOT INITIALIZED]";
            }
            return "{0}={1}{2}".Fmt(FieldName, val, IsModified ? "*" : "");
        }

        #endregion

    }

    /// <summary>
    /// Strongly typed value object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DbFieldValue<T> : DbFieldValue
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DbValue.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="key">Sets this field as a key field.</param>
        public DbFieldValue(string fieldName, bool key = false)
            : base(fieldName, key, false)
        {
        }

        /// <summary>
        /// Creates an instance of DbValue.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="key"></param>
        /// <param name="identity"></param>
        internal DbFieldValue(string fieldName, bool key, bool identity)
            : base(fieldName, key, identity)
        {
        }

        #endregion

        #region Fields and Properties

        private T mValue;
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <exception cref="DbFieldValueNotInitializedException">Thrown if trying to get a value that hasn't been initialized.</exception>
        public virtual T Value
        {
            get
            {
                if (!IsInitialized) throw new DbFieldValueNotInitializedException(this);
                return mValue;
            }
            set
            {
                mValue = value;
                IsInitialized = true;
            }
        }

        private T mOriginalValue;
        /// <summary>
        /// Gets or sets the original value. Only set if SetOriginal is called.
        /// </summary>
        public T OriginalValue
        {
            get
            {
                if (!IsOriginalInitialized) throw new DbFieldValueNotInitializedException(this, true);
                return mOriginalValue;
            }
            set
            {
                mOriginalValue = value;
                IsOriginalInitialized = true;
            }
        }

        /// <summary>
        /// Gets a value that determines if the value is different than the original value. True if the value has been set but the original value hasn't been. False if the value has not been set.
        /// </summary>
        public override bool IsModified
        {
            get
            {
                if (!IsInitialized) return false;
                if (!IsOriginalInitialized) return true;
                return !object.Equals(Value, OriginalValue);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the value of the field. Uses ConvertEx to convert the value if necessary.
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(object value)
        {
            Value = ConvertEx.ChangeType<T>(value);
        }

        /// <summary>
        /// Gets the value of the field.
        /// </summary>
        /// <returns></returns>
        public override object GetValue()
        {
            return Value;
        }

        /// <summary>
        /// Gets the expected type for the value.
        /// </summary>
        /// <returns></returns>
        public override Type GetValueType()
        {
            return typeof(T);
        }

        /// <summary>
        /// Sets the original value to the current value.
        /// </summary>
        public override void SetOriginal()
        {
            OriginalValue = Value;
        }

        #endregion

    }

    /// <summary>
    /// Represents a auto-incrementing integer field. This value will be automatically populated when a new DbObject record is inserted into the database.
    /// </summary>
    public class DbIdentityFieldValue : DbFieldValue<int>
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DbValue.
        /// </summary>
        /// <param name="fieldName"></param>
        public DbIdentityFieldValue(string fieldName)
            : base(fieldName, false, true)
        {
        }

        #endregion

    }

}
