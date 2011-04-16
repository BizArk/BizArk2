using System.Linq;
using System.Text;
using BizArk.Core;
//using System.Web.Script.Serialization;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Base class for entities. 
    /// </summary>
    public abstract class Entity
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of Entity.
        /// </summary>
        public Entity()
        {
            State = new EntityState(this);
        }

        #endregion

        #region Fields and Properties

        //todo: Add [ScriptIgnore] to prevent State from being serialized in JSON response.
        /// <summary>
        /// Gets the state of an entity.
        /// </summary>
        public EntityState State { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a property value from the entity state.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        protected virtual T GetValue<T>(string name)
        {
            if (name.StartsWith("get_")) name = name.Substring(4);
            var val = State.GetValue_Internal(name);
            return ConvertEx.ChangeType<T>(val);
        }

        /// <summary>
        /// Set a property value in the entity state.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        protected virtual void SetValue(string name, object value)
        {
            if (name.StartsWith("set_")) name = name.Substring(4);
            State.SetValue_Internal(name, value);
        }

        /// <summary>
        /// Gets a textual description of the entity based on the key.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("<{0}>: ", this.GetType().Name);
            var first = true;
            var values = State.GetInitializedValues();

            if (State.Manager.Identity != null)
            {
                sb.Append(GetValueDisplay(State.Manager.Identity, values));
                first = false;
            }

            foreach (var keyProp in State.Manager.Key)
            {
                // We've already displayed the identity above.
                if(keyProp.IsIdentity) continue;

                if (!first) sb.Append(", ");
                sb.Append(GetValueDisplay(keyProp, values));
                first = false;
            }
            return sb.ToString();
        }

        private string GetValueDisplay(EntityProperty prop, EntityValue[] values)
        {
            var val = values.FirstOrDefault(v => v.Property == prop);
            if (val == null)
                return string.Format("{0}=[NOT SET]", prop.Name);
            else
                return val.ToString();
        }

        /// <summary>
        /// Override this method to perform any custom validation on the object. This is run prior to running any other validation. If there is an error, this should throw a System.ComponentModel.DataAnnotations.ValidationException.
        /// </summary>
        internal protected virtual void PreValidate()
        {

        }

        #endregion

    }
}
