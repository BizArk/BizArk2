
using BizArk.Core.Data;
namespace BizArk.Core.ORM
{

    /// <summary>
    /// Stores the value for a entity instance.
    /// </summary>
    public class EntityValue
    {
        
        #region Initialization and Destruction

        /// <summary>
        /// Creates a new instance of EntityValue.
        /// </summary>
        /// <param name="prop"></param>
        public EntityValue(EntityProperty prop)
        {
            Property = prop;
            Original = Unitialized.Value;
            Current = Unitialized.Value;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the entity property associated with this value.
        /// </summary>
        public EntityProperty Property { get; private set; }

        /// <summary>
        /// Gets a value that determines if the value has been modified.
        /// </summary>
        public bool IsModified
        {
            get
            {
                // If the current value hasn't been initialized then this field is not modified.
                if (!IsCurrentIntialized) return false;

                // If the original hasn't been initialized then this field is modified.
                if (!IsOriginalIntialized) return true;

                if (Original == null && Current == null) return false;
                if (Original == null) return true;
                if (Current == null) return true;
                return !Original.Equals(Current);
            }
        }

        /// <summary>
        /// Gets a value that determines if the original value has been initialized.
        /// </summary>
        public bool IsOriginalIntialized { get { return Original != Unitialized.Value; } }

        /// <summary>
        /// Gets the original value for the property.
        /// </summary>
        public object Original { get; internal set; }

        /// <summary>
        /// Gets a value that determines if the original value has been initialized.
        /// </summary>
        public bool IsCurrentIntialized { get { return Current != Unitialized.Value; } }

        /// <summary>
        /// Gets the current value for the property.
        /// </summary>
        public object Current { get; internal set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a string representation of TfEntityValue.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            object val;

            if (Current == Unitialized.Value)
                val = "[NOT SET]";
            else if (Current == null)
                val = "[NULL]";
            else if (Current.GetType() == typeof(string))
                val = string.Format("\"{0}\"", Current);
            else
                val = Current;
            
            return string.Format("{0}={1}", Property.Name, val);
        }

        /// <summary>
        /// Gets the FieldValue that represents this entity value.
        /// </summary>
        /// <returns></returns>
        public FieldValue GetFieldValue()
        {
            var fld = new FieldValue(Property.ColumnName, Current);
            fld.DbType = Property.DbType;
            fld.Size = Property.Size;
            return fld;
        }

        #endregion

    }

    /// <summary>
    ///  Used to represent an uninitialized value.
    /// </summary>
    public sealed class Unitialized
    {
        /// <summary>
        /// Singleton.
        /// </summary>
        private Unitialized() { }

        private static Unitialized sValue = new Unitialized();
        /// <summary>
        /// Gets the singleton value for Unitialized.
        /// </summary>
        public static Unitialized Value { get { return sValue; } }

    }

}
