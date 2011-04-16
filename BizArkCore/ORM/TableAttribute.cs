using System;

namespace BizArk.Core.ORM
{

    /// <summary>
    /// Defines the mapping between an entity and a database table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of TableAttribute.
        /// </summary>
        /// <param name="name"></param>
        public TableAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the name of the table in the database for this entity.
        /// </summary>
        public string Name { get; private set; }

        #endregion

    }

}
