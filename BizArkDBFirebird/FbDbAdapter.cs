using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BizArk.Core;
using BizArk.DB.Adapters;
using FirebirdSql.Data.FirebirdClient;

namespace BizArk.DB.Firebird
{

    /// <summary>
    /// Provides methods for working with a Sql Server database.
    /// </summary>
    public class FbDbAdapter : AnsiSqlDbAdapter
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDbInfo.
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="tzi">The timezone for the server. If null, uses the local timezone.</param>
        public FbDbAdapter(string connStr, TimeZoneInfo tzi = null)
        {
            if (string.IsNullOrEmpty(connStr)) throw new ArgumentNullException("connStr");
            ConnStr = connStr;
            ServerTimeZone = tzi ?? TimeZoneInfo.Local;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the connection string to use for the database.
        /// </summary>
        public string ConnStr { get; private set; }

        /// <summary>
        /// Gets the timezone for the server.
        /// </summary>
        public TimeZoneInfo ServerTimeZone { get; private set; }

        #endregion

        #region Methods

        private static DbCommand sNowCmd;
        /// <summary>
        /// Gets the DateTime from the database server.
        /// </summary>
        public override DateTime NowUtc(Database db)
        {
            if (sNowCmd == null)
            {
                sNowCmd = CreateCommand();
                sNowCmd.CommandText = "SELECT current_timestamp";
            }
            var dt = db.ExecuteScalar<DateTime>(sNowCmd);
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        /// <summary>
        /// Creates an instance of SqlConnection. The connection is NOT opened.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new FbConnection(ConnStr);
        }

        /// <summary>
        /// Creates an instance of FbCommand.
        /// </summary>
        /// <returns></returns>
        protected override DbCommand CreateCommand()
        {
            return new FbCommand();
        }

        /// <summary>
        /// Override to handle getting the identity value. This is only called if identityFieldName has a value.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="identityFieldName"></param>
        /// <returns></returns>
        protected override int InsertIdentity(Database db, DbCommand cmd, string identityFieldName)
        {
            cmd.CommandText += " returning " + identityFieldName;
            return db.ExecuteScalar<int>(cmd);
        }

        #endregion

    }

}
