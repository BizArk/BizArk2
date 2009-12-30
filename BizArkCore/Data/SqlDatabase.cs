using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using My = Redwerb.BizArk.Core.Properties;
using System.Data;
using System.ComponentModel;

namespace Redwerb.BizArk.Core.Data
{

    /// <summary>
    /// Provides some utility functions for accessing a Sql Server database.
    /// </summary>
    public class SqlDatabase
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDatabase.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="connStr"></param>
        public SqlDatabase(string name, string connStr)
        {
            mName = name;
            mConnStr = connStr;
        }

        #endregion

        #region Fields and Properties

        private static Dictionary<string, SqlDatabase> sDatabases = new Dictionary<string, SqlDatabase>();

        private string mName;
        /// <summary>
        /// Gets the name of this database. Used for registration.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        private string mConnStr;
        /// <summary>
        /// Gets the connection string for this database.
        /// </summary>
        public string ConnStr
        {
            get { return mConnStr; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the database so that it can be retrieved later.
        /// </summary>
        /// <param name="db"></param>
        public static void RegisterDatabase(SqlDatabase db)
        {
            sDatabases.Add(db.Name, db);
        }

        /// <summary>
        /// Gets the named database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SqlDatabase GetDatabase(string name)
        {
            return sDatabases[name];
        }

        private static SqlDatabase sDefaultDatabase;
        /// <summary>
        /// Gets or sets the default database to use.  If this property is not set explicitly, a default one will be created automatically based on the DefaultConnStr connection string defined in Redwerb.BizArk.Core.dll.config.
        /// </summary>
        public static SqlDatabase DefaultDatabase
        {
            get
            {
                if (sDefaultDatabase == null)
                    sDefaultDatabase = new SqlDatabase("Default", My.Settings.Default.DefaultConnStr);
                return sDefaultDatabase;
            }
            set { sDefaultDatabase = value; }
        }

        /// <summary>
        /// Creates a new sql connection but does not connect. You are responsible for disposing the connection.
        /// </summary>
        /// <returns></returns>
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(mConnStr);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(SqlCommand cmd)
        {
            var dflt = (T)ConvertEx.GetDefaultEmptyValue(typeof(T));
            return ExecuteScalar<T>(cmd, dflt);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(SqlCommand cmd, T dflt)
        {
            T value = dflt;

            ExecuteCommand(cmd, () =>
                {
                    var res = cmd.ExecuteScalar();
                    if (res != DBNull.Value)
                        value = ConvertEx.ChangeType<T>(res);
                });

            return value;
        }

        /// <summary>
        /// Executes the command against the database.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(SqlCommand cmd)
        {
            int count = 0;
            ExecuteCommand(cmd, () =>
            {
                count = cmd.ExecuteNonQuery();
            });

            return count;
        }

        /// <summary>
        /// Processes a SqlDataReader calling the processRow delegate for each row.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        public void ExecuteDataReader(SqlCommand cmd, ProcessDataRow processRow)
        {
            ExecuteCommand(cmd, () =>
                {
                    var dr = cmd.ExecuteReader();
                    while (dr.Read())
                        processRow(dr);
                });
        }

        /// <summary>
        /// Loads a DataTable based on the command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataTable GetDataTable(SqlCommand cmd)
        {
            DataTable tbl = null;
            ExecuteCommand(cmd, () =>
            {
                var dr = cmd.ExecuteReader();
                tbl = new DataTable();
                tbl.Load(dr);
            });

            return tbl;
        }

        /// <summary>
        /// Returns the first row of the DataTable. Null if no rows.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public DataRow GetDataRow(SqlCommand cmd)
        {
            var tbl = GetDataTable(cmd);
            if (tbl.Rows.Count == 0) return null;
            return tbl.Rows[0];
        }

        /// <summary>
        /// Sets up and tears down the connection and command object for execution. Calles the execute delegate to perform the actual execution. This is the only method that executes commands.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="execute"></param>
        protected void ExecuteCommand(SqlCommand cmd, ExecuteDelegate execute)
        {
            var conn = CreateConnection();
            conn.Open();
            try
            {
                cmd.Connection = conn;
                execute();
            }
            finally
            {
                cmd.Connection = null;
                conn.Close();
                conn.Dispose();
                conn = null;
            }
        }

        #endregion

        #region Support

        /// <summary>
        /// Delegate used to execute a command.
        /// </summary>
        protected delegate void ExecuteDelegate();

        /// <summary>
        /// Processes a single row of a data reader.
        /// </summary>
        /// <param name="dr"></param>
        public delegate void ProcessDataRow(SqlDataReader dr);

        #endregion

    }

}
