using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizArk.Core.Extensions.StringExt;

namespace BizArk.DB.ORM
{

    /// <summary>
    /// Interface for database objects.
    /// </summary>
    public interface ISupportDbState
    {
        /// <summary>
        /// The database state for the current object.
        /// </summary>
        DbObjectState DbState { get; }
    }

    /// <summary>
    /// Base class for ISupportDbState.
    /// </summary>
    public class DbObject : ISupportDbState
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DbObject.
        /// </summary>
        /// <param name="tableName">If null, uses the name of the class.</param>
        public DbObject(string tableName = null)
        {
            if (tableName.HasValue())
                mTableName = tableName;
            else
                mTableName = this.GetType().Name;
        }

        #endregion

        #region Fields and Properties

        private string mTableName;

        private DbObjectState mState;
        /// <summary>
        /// Gets the current state for the object.
        /// </summary>
        public DbObjectState DbState
        {
            get
            {
                // Don't create the state unless it's needed.
                if (mState == null)
                    mState = new DbObjectState(this, mTableName);
                return mState;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a textual description of the DB object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(mTableName);
            var id = DbState.Identity;
            var key = DbState.Key;
            if (id != null)
                sb.AppendFormat(": {0}", id.ToString());
            else if (key != null && key.Length > 0)
            {
                var flds = new List<string>();
                foreach (var fld in key)
                    flds.Add(fld.ToString());
                sb.AppendFormat(": {0}", string.Join(", ", flds.ToArray()));
            }
            return sb.ToString();
        }

        #endregion

    }

}
