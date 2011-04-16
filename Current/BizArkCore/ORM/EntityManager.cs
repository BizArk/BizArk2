using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BizArk.Core.AttributeExt;
using BizArk.Core.TypeExt;
using BizArk.Core.Data;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// The manager handles class-level entity management. The manager is only created once for the application.
    /// </summary>
    public class EntityManager
    {

        #region Initialization and Destruction

        /// <summary>
        /// TfEntityManager instances are created by the factory method TfEntityManager.GetManager([EntityType]).
        /// </summary>
        /// <param name="entityType"></param>
        protected EntityManager(Type entityType)
        {
            if (entityType == null) throw new ArgumentNullException("entityType");
            if (!typeof(Entity).IsAssignableFrom(entityType)) throw new InvalidOperationException(string.Format("Cannot create the requested entity manager because the type '{0}' does not inherit from TfEntity.", entityType.Name));

            EntityType = entityType;
            var tblAtt = entityType.GetAttribute<TableAttribute>(false);
            if (tblAtt == null) throw new InvalidOperationException(string.Format("The entity type {0} does not have a TableAttribute defined on it.", entityType.Name));
            TableName = tblAtt.Name;

            // Initialize the entity properties.
            var key = new List<EntityProperty>();
            mProperties = new Dictionary<string, EntityProperty>();
            foreach (var prop in EntityType.GetProperties())
            {
                var colAtt = prop.GetAttribute<ColumnAttribute>(false);
                if (colAtt == null) continue;

                var colProp = new EntityProperty(EntityType, prop, colAtt);
                mProperties.Add(prop.Name, colProp);

                if (colProp.IsIdentity)
                {
                    if (Identity != null) throw new InvalidOperationException(string.Format("The entity type {0} has multiple identity properties defined.", entityType.Name));
                    if (colProp.PropertyType != typeof(int)) throw new InvalidOperationException(string.Format("The identity property ({0}) for entity type {1} must be of type int.", colProp.Name, entityType.Name));
                    if (colProp.PropertyType.IsNullable()) throw new InvalidOperationException(string.Format("The identity property ({0}) for entity type {1} cannot be nullable.", colProp.Name, entityType.Name));
                    Identity = colProp;
                }

                if (colProp.IsKey)
                    key.Add(colProp);
            }

            // Make sure we have a key for the entity.
            if (key.Count == 0)
            {
                if (Identity != null)
                    // Use the identity if the key is not otherwise defined.
                    key.Add(Identity);
            }
            Key = key.ToArray();
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the type of the entity that is being managed.
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Gets the name of the table for the entity.
        /// </summary>
        public string TableName { get; private set; }

        private Dictionary<string, EntityProperty> mProperties;
        /// <summary>
        /// Gets the list of properties for the entity.
        /// </summary>
        public EntityProperty[] Properties
        {
            get { return mProperties.Values.ToArray(); }
        }

        /// <summary>
        /// Gets the identity property for this entity, if it has one.
        /// </summary>
        public EntityProperty Identity { get; private set; }

        /// <summary>
        /// Gets the properties that make up the key for the entity. This is typically just the Identity property, but it can be different.
        /// </summary>
        public EntityProperty[] Key { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the specified property.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EntityProperty GetProperty(string name)
        {
            return mProperties[name];
        }

        private static Dictionary<Type, EntityManager> mManagers = new Dictionary<Type, EntityManager>();
        /// <summary>
        /// Gets the single instance of the entity manager for this application.
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static EntityManager GetManager(Type entityType)
        {
            if (!mManagers.ContainsKey(entityType))
            {
                EntityManager mgr = null;

                var mgrAtt = entityType.GetAttribute<EntityManagerAttribute>(false);
                if (mgrAtt != null && mgrAtt.CustomManager != null)
                    mgr = (EntityManager)Activator.CreateInstance(mgrAtt.CustomManager, entityType);
                else
                    // Create the manager as we need it.
                    mgr = new EntityManager(entityType);

                mManagers.Add(entityType, mgr);
            }

            return mManagers[entityType];
        }

        /// <summary>
        /// Gets a string representation of TfEntityManager.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Manager for {0}", EntityType.Name);
            if (!string.IsNullOrEmpty(TableName))
                sb.AppendFormat(" ({0})", TableName);
            return sb.ToString();
        }
        
        #endregion

    }
}
