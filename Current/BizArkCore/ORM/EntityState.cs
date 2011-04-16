using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using BizArk.Core;
using System.ComponentModel.DataAnnotations;
using BizArk.Core.AttributeExt;
using BizArk.Core.Data;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Holds the state of an entity.
    /// </summary>
    public class EntityState
    {
        
        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of TfEntityState.
        /// </summary>
        /// <param name="owner"></param>
        internal EntityState(Entity owner)
        {
            Owner = owner;
            IsNew = true;
        }

        #endregion

        #region Fields and Properties

        private Dictionary<string, EntityValue> mValues = new Dictionary<string, EntityValue>();

        /// <summary>
        /// Gets the entity that this state is for.
        /// </summary>
        public Entity Owner { get; private set; }

        private EntityManager mMgr;
        /// <summary>
        /// Gets the entity manager associated with this entity.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public EntityManager Manager
        {
            get
            {
                if (mMgr == null)
                    mMgr = EntityManager.GetManager(Owner.GetType());
                return mMgr;
            }
        }

        /// <summary>
        /// Gets a value that determines if the entity needs to be inserted into the database.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNew { get; set; }

        /// <summary>
        /// Gets a value that determines if this is deleted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets a value that determines if any of the values has been modified.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsModified
        {
            get
            {
                if (mValues == null) return false;
                if (mValues.Count == 0) return false;
                foreach (var val in mValues.Values)
                {
                    if (val.IsModified)
                        return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets the key that can be used in sql statements.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public FieldValue[] DbKey
        {
            get
            {
                var key = new List<FieldValue>();
                var mgr = Manager;
                if (mgr.Identity != null)
                {
                    // Use the identity if we have one.
                    var val = GetValue(mgr.Identity.Name, false);
                    var fld = val.GetFieldValue();
                    key.Add(fld);
                }
                else
                {
                    // No identity, use the key fields.
                    foreach (var keyProp in mgr.Key)
                    {
                        var val = GetValue(keyProp.Name, false);
                        var fld = val.GetFieldValue();
                        key.Add(fld);
                    }
                }
                return key.ToArray();
            }
        }

        /// <summary>
        /// Gets the identity value for the entity.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the entity does not have an identity property.</exception>
        public int Identity
        {
            get
            {
                var prop = Manager.Identity;
                if (prop == null) throw new InvalidOperationException(string.Format("Entity {0} does not have an identity property associated with it.", Owner.GetType().Name));
                var val = GetValue_Internal(prop.Name);
                return ConvertEx.ChangeType<int>(val);
            }
        }

        #endregion

        #region Methods

        internal EntityValue[] GetInitializedValues()
        {
            return mValues.Values.ToArray();
        }

        /// <summary>
        /// Initializes the original values to the current values. Only processes values that have been set.
        /// </summary>
        public void SetUnmodified()
        {
            foreach (var val in mValues.Values)
                val.Original = val.Current;
        }

        /// <summary>
        /// Gets the current value of the named property.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal object GetValue_Internal(string name)
        {
            var val = GetValue(name, false);
            return val.Current;
        }

        /// <summary>
        /// Sets the current value of the named property.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        internal void SetValue_Internal(string name, object value)
        {
            var val = GetValue(name, true);
            val.Current = value;

            // It's a common mistake to forget to set IsNew = false.
            // If we are setting the identity field to a value greater than 0
            // this this entity is not new.
            if (val.Property.IsIdentity
                && value != null
                && value.GetType() == typeof(int)
                && (int)value > 0)
            {
                IsNew = false;
            }
        }

        /// <summary>
        /// Determines if the propery has a value.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsValueInitialized(string name)
        {
            if (!mValues.ContainsKey(name)) return false;
            var val = mValues[name];
            return val.IsCurrentIntialized;
        }

        /// <summary>
        /// Determines if the value has been modified. Returns false if it is not initialized, true if MyEntity.State.SetUnmodified() has not been called.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsValueModified(string name)
        {
            if (!mValues.ContainsKey(name)) return false;
            var val = mValues[name];
            return val.IsModified;
        }

        /// <summary>
        /// Provides a way to get a value from the entity. This will call the actual property to ensure that any logic is processed.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetValue(string name)
        {
            var prop = Manager.GetProperty(name);
            return prop.Property.GetValue(Owner, null);
        }

        private EntityValue GetValue(string name, bool create)
        {
            if (!mValues.ContainsKey(name))
            {
                if (!create)
                    throw new InvalidOperationException(string.Format("Property {0} of entity {1} has not been initialized.", name, Owner));

                var prop = Manager.GetProperty(name);
                if (prop == null) throw new InvalidOperationException(string.Format("Property {0} of entity {1} is not a valid column property. Make sure the ColumnAttribute is defined for it.", name, Owner));

                mValues.Add(name, new EntityValue(prop));
            }
            return mValues[name];
        }

        /// <summary>
        /// Gets the modified values for saving.
        /// </summary>
        /// <returns></returns>
        internal EntityValue[] GetValuesForSave()
        {
            var values = new List<EntityValue>();
            foreach (var val in mValues.Values)
            {
                if (val.Property.IsReadOnly) continue;
                if (val.Property.IsIdentity) continue;
                if (string.IsNullOrEmpty(val.Property.ColumnName)) continue;
                if (!val.IsModified) continue;
                values.Add(val);
            }
            return values.ToArray();
        }

        /// <summary>
        /// Validates the object based on the DataAnnotation attributes on the properties. Throws a ValidationException if the entity is not valid. Only validates initialized properties.
        /// </summary>
        public void Validate()
        {
            Owner.PreValidate();

            foreach (var prop in this.Manager.Properties)
            {
                if (!IsValueInitialized(prop.Name))
                {
                    if (IsNew && prop.IsRequired && !prop.IsIdentity)
                        // All required values must be initialized before inserting a record into the database.
                        // Identity properties are ignored since they will be set from the database after insert.
                        throw new ValidationException(prop.RequiredAtt.FormatErrorMessage(prop.DisplayName));
                }
                else
                {
                    var ctx = new ValidationContext(Owner, null, null)
                    {
                        DisplayName = prop.DisplayName,
                        MemberName = prop.Name
                    };
                    Validator.ValidateProperty(GetValue(prop.Name), ctx);
                }

            }
        }

        #endregion

    }

}
