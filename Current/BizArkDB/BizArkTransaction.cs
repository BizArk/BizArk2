using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace BizArk.DB
{

    /// <summary>
    /// Wraps a database transaction so that it can be managed by the BizArk Database class.
    /// </summary>
    public class BizArkTransaction : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of BizArkTransaction.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="trans"></param>
        internal BizArkTransaction(Database db, DbTransaction trans)
        {
            Database = db;
        }

        /// <summary>
        /// Rollsback the transaction.
        /// </summary>
        public void Dispose()
        {
            // Dispose should be called even if Commit or Rollback were already called.
            // Check for a transaction to avoid an exception.
            if (Database.InTransaction)
                Rollback();
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the database associated with this transaction.
        /// </summary>
        public Database Database { get; private set; }

        /// <summary>
        /// Gets the raw database transaction. Shouldn't call methods on it directly or it might cause problems.
        /// </summary>
        public DbTransaction Transaction { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Commits the transaction.
        /// </summary>
        public void Commit()
        {
            Database.CommitTransaction();
        }

        /// <summary>
        /// Rolls the transaction back.
        /// </summary>
        public void Rollback()
        {
            Database.RollbackTransaction();
        }

        #endregion

    }
}
