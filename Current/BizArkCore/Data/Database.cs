using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;

namespace BizArk.Core.Data
{

    /// <summary>
    /// Represents a database and provides some simple methods that can be used to query it.
    /// </summary>
    public class Database : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of Database.
        /// </summary>
        /// <param name="dbInfo"></param>
        public Database(IDbInfo dbInfo)
        {
            if (dbInfo == null) throw new ArgumentNullException("dbInfo");
            DbInfo = dbInfo;
        }

        /// <summary>
        /// Closes the database connection.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the Database.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                if (mConn != null && mConn.State == ConnectionState.Open)
                    mConn.Close();
                mConn = null;

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            Disposed = true;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the DbInfo object associated with this Database.
        /// </summary>
        public IDbInfo DbInfo { get; private set; }

        /// <summary>
        /// Gets a value that determines if this Database has been disposed.
        /// </summary>
        public bool Disposed { get; private set; }

        private DbConnection mConn;
        /// <summary>
        /// Gets an open connection for the database. Call Database.Dispose to close and release the connection.
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                if (Disposed) throw new InvalidOperationException("The database has been disposed.");
                if (mConn == null)
                {
                    mConn = DbInfo.CreateConnection();
                    mConn.Open();
                }
                return mConn;
            }
        }

        #endregion

        #region Basic Command Methods

        /// <summary>
        /// Creates a database command.
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public DbCommand CreateCommand(string sql = null, object parameters = null)
        {
            return DbInfo.CreateCommand(sql, parameters);
        }

        private void ExecuteCommand(DbCommand cmd, Action execute)
        {
            if (cmd.Connection == null)
                cmd.Connection = Connection;
            execute();
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="parameters">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(string sql, object parameters = null)
        {
            var cmd = CreateCommand(sql, parameters);
            return ExecuteScalar<T>(cmd);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public T ExecuteScalar<T>(DbCommand cmd)
        {
            var dflt = (T)ConvertEx.GetDefaultEmptyValue(typeof(T));
            return ExecuteScalar<T>(cmd, dflt);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="dflt">The value to return if the database value is DBNull.</param>
        /// <returns></returns>
        public T ExecuteScalar<T>(DbCommand cmd, T dflt)
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
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="parameters">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string sql, object parameters = null)
        {
            var cmd = CreateCommand(sql, parameters);
            return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes the command against the database.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(DbCommand cmd)
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
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="parameters">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        public void ExecuteDataReader(string sql, object parameters, Action<IDataReader> processRow)
        {
            var cmd = CreateCommand(sql, parameters);
            ExecuteDataReader(cmd, processRow);
        }

        /// <summary>
        /// Processes a SqlDataReader calling the processRow delegate for each row.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        public void ExecuteDataReader(DbCommand cmd, Action<IDataReader> processRow)
        {
            ExecuteCommand(cmd, () =>
            {
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    processRow(dr);
            });
        }

        #endregion

        #region Object Command Methods

        /// <summary>
        /// Executes a command and returns the first row as an object. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">A function to convert the row to the object.</param>
        /// <returns></returns>
        public T SelectSingle<T>(DbCommand cmd, Func<IDataReader, T> process) where T : class
        {
            T obj = null;
            ExecuteDataReader(cmd, (row) => { obj = process(row); });
            return obj;
        }

        /// <summary>
        /// Executes a command and returns the rows as an array of objects. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">A function to convert the row to the object.</param>
        /// <returns></returns>
        public T[] Select<T>(DbCommand cmd, Func<IDataReader, T> process) where T : class
        {
            var objs = new List<T>();
            ExecuteDataReader(cmd, (row) =>
            {
                var obj = process(row);
                if (obj != null) objs.Add(obj);
            });
            return objs.ToArray();
        }

        #endregion

    }

}
