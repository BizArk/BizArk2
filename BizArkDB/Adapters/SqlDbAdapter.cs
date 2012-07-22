using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BizArk.Core;

namespace BizArk.DB.Adapters
{

    /// <summary>
    /// Provides methods for working with a Sql Server database.
    /// </summary>
    public class SqlDbAdapter : AnsiSqlDbAdapter
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDbInfo.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbAdapter(string connStr)
        {
            if (string.IsNullOrEmpty(connStr)) throw new ArgumentNullException("connStr");
            ConnStr = connStr;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the connection string to use for the database.
        /// </summary>
        public string ConnStr { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of SqlConnection. The connection is NOT opened.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new SqlConnection(ConnStr);
        }

        /// <summary>
        /// Creates an instance of SqlCommand.
        /// </summary>
        /// <returns></returns>
        protected override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        #endregion

    }

}
