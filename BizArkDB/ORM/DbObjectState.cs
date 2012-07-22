using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using BizArk.Core.Extensions.TypeExt;

namespace BizArk.DB.ORM
{

    /// <summary>
    /// State for a DbObject.
    /// </summary>
    public class DbObjectState
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DbObjectState.
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="tableName"></param>
        public DbObjectState(ISupportDbState owner, string tableName)
        {
            Owner = owner;
            TableName = tableName;
        }

        #endregion

        #region Fields and Properties

        private Dictionary<string, DbFieldValue> mFieldsX;

        /// <summary>
        /// Gets the field with the given name.
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public DbFieldValue this[string fieldName]
        {
            get
            {
                var flds = GetFields();
                if (flds.ContainsKey(fieldName))
                    return flds[fieldName];
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets the fields that make up the primary key.
        /// </summary>
        public DbFieldValue[] Key { get; private set; }

        /// <summary>
        /// Gets the identity field for this record. If set, the key is ignored.
        /// </summary>
        public DbIdentityFieldValue Identity { get; set; }

        /// <summary>
        /// Gets the ISupportDbState that this is the state of.
        /// </summary>
        public ISupportDbState Owner { get; private set; }

        /// <summary>
        /// Gets the name of the table this class represents.
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// Gets the fields associated with the DbObject.
        /// </summary>
        public ICollection<DbFieldValue> Fields
        {
            get
            {
                return GetFields().Values;
            }
        }

        /// <summary>
        /// Gets a value that determines if any of the fields have been modified.
        /// </summary>
        public bool IsModified
        {
            get
            {
                foreach (var fld in GetFields().Values)
                {
                    if (fld.IsModified)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if this object is deleted. If true and the object is in the database, it will be deleted on save.
        /// </summary>
        public bool IsDeleted { get; set; }

        private bool mIsNew = false;
        /// <summary>
        /// Gets or sets a value that determines if this object is new. If true and the object is not deleted, it will be inserted on save.
        /// </summary>
        /// <remarks>If their is an identity field for this object, this value will be true if the identity has not been initialized, otherwise false.</remarks>
        public bool IsNew
        {
            get
            {
                if (Identity != null) return !Identity.IsInitialized;
                return mIsNew;
            }
            set { mIsNew = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets all the initialized fields original value.
        /// </summary>
        public void SetOriginals()
        {
            foreach (var fld in GetFields().Values)
            {
                if (fld.IsInitialized)
                    fld.SetOriginal();
            }
        }

        private Dictionary<string, DbFieldValue> GetFields()
        {
            if (mFieldsX == null)
            {
                mFieldsX = new Dictionary<string, DbFieldValue>();
                var key = new List<DbFieldValue>();
                var flds = Owner.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var fld in flds)
                {
                    var fldval = fld.GetValue(Owner) as DbFieldValue;
                    if (fldval != null)
                    {
                        mFieldsX.Add(fldval.FieldName, fldval);
                        if (fldval.IsKey)
                            key.Add(fldval);
                        if (Identity == null)
                            Identity = fldval as DbIdentityFieldValue;
                    }
                }

                Key = key.ToArray();
            }
            return mFieldsX;
        }

        #endregion

    }

}
