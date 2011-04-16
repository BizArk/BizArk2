using System;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Allows an entity to have a custom manager class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class EntityManagerAttribute : Attribute
    {

        /// <summary>
        /// Creates an instance of EntityManagerAttribute.
        /// </summary>
        /// <param name="customManager">A class that is derived from TfEntityManager that should be used instead of the default manager..</param>
        public EntityManagerAttribute(Type customManager)
        {
            CustomManager = customManager;
        }

        /// <summary>
        /// Gets the custom manager for this entity.
        /// </summary>
        public Type CustomManager { get; private set; }

    }

}
