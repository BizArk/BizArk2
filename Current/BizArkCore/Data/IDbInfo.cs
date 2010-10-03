using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;

namespace BizArk.Core.Data
{

    /// <summary>
    /// Database specific methods.
    /// </summary>
    public interface IDbInfo
    {

        /// <summary>
        /// Create a connection to the database.
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConnection();

        /// <summary>
        /// Creates a command object that can be used to execute queries.
        /// </summary>
        /// <returns></returns>
        DbCommand CreateCommand();

        /// <summary>
        /// Sets up and tears down the connection and command object for execution. Calles the execute delegate to perform the actual execution. This is the only method that executes commands.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="execute"></param>
        void ExecuteCommand(DbCommand cmd, ExecuteDelegate execute);

        /// <summary>
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        int Insert(string tableName, object values, DbTransaction trans = null);

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        int Update(string tableName, object key, object values, DbTransaction trans = null);

        /// <summary>
        /// Removes a record from the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        int Delete(string tableName, object key, DbTransaction trans = null);

        /// <summary>
        /// Determines if the record exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns></returns>
        bool Exists(string tableName, object key, DbTransaction trans = null);

    }

    /// <summary>
    /// Delegate used to execute a command.
    /// </summary>
    public delegate void ExecuteDelegate();

    /// <summary>
    /// Base class for IDbInfo.
    /// </summary>
    public abstract class BaseDbInfo
        : IDbInfo
    {

        #region Methods

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Creates a command object that can be used to execute queries.
        /// </summary>
        /// <returns></returns>
        public abstract DbCommand CreateCommand();

        /// <summary>
        /// Sets up and tears down the connection and command object for execution. Calles the execute delegate to perform the actual execution. This is the only method that executes commands.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="execute"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ExecuteCommand(DbCommand cmd, ExecuteDelegate execute)
        {
            DbConnection conn = null;
            if (cmd.Connection == null)
            {
                // Create a temporary connection.
                conn = CreateConnection();
                conn.Open();
                cmd.Connection = conn;
            }

            try
            {
                execute();
            }
            finally
            {
                if (conn != null)
                {
                    cmd.Connection = null;
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
            }
        }

        /// <summary>
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        public abstract int Insert(string tableName, object values, DbTransaction trans = null);

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        public abstract int Update(string tableName, object key, object values, DbTransaction trans = null);

        /// <summary>
        /// Removes a record from the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        public abstract int Delete(string tableName, object key, DbTransaction trans = null);

        /// <summary>
        /// Determines if the record exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns></returns>
        public abstract bool Exists(string tableName, object key, DbTransaction trans = null);

        internal static void AddParameter(DbCommand cmd, PropertyDescriptor prop, object obj)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@" + prop.Name;
            var value = prop.GetValue(obj);
            if (ConvertEx.IsEmpty(value))
                value = DBNull.Value;
            parameter.Value = value;
            cmd.Parameters.Add(parameter);
        }

        #endregion

    }

    /// <summary>
    /// Provides basic method definitions for Ansi-standard databases.
    /// </summary>
    public abstract class AnsiSqlDbInfo
        : BaseDbInfo
    {

        #region Methods

        /// <summary>
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        public override int Insert(string tableName, object values, DbTransaction trans = null)
        {
            var cmd = CreateCommand();
            var props = TypeDescriptor.GetProperties(values);
            var fields = new List<string>();
            foreach (PropertyDescriptor prop in props)
            {
                fields.Add(prop.Name);
                AddParameter(cmd, prop, values);
            }
            if (fields.Count == 0) return 0;

            cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})", tableName, string.Join(", ", fields.ToArray()), string.Join(", @", fields.ToArray()));

            if (trans != null)
            {
                cmd.Connection = trans.Connection;
                cmd.Transaction = trans;
            }

            int count = 0;
            ExecuteCommand(cmd, () => { count = cmd.ExecuteNonQuery(); });
            return count;
        }

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="values"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        public override int Update(string tableName, object key, object values, DbTransaction trans = null)
        {
            var cmd = CreateCommand();
            var fields = new List<string>();
            var props = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor prop in props)
            {
                fields.Add(string.Format("{0} = @{0}", prop.Name));
                AddParameter(cmd, prop, values);
            }
            if (fields.Count == 0) return 0;

            var criteria = new List<string>();
            props = TypeDescriptor.GetProperties(key);
            foreach (PropertyDescriptor prop in props)
            {
                criteria.Add(string.Format("{0} = @{0}", prop.Name));
                AddParameter(cmd, prop, key);
            }

            if (criteria.Count == 0)
                cmd.CommandText = string.Format("UPDATE {0} SET {1}", tableName, string.Join(", ", fields.ToArray()));
            else
                cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, string.Join(", ", fields.ToArray()), string.Join(" AND ", criteria.ToArray()));

            if (trans != null)
            {
                cmd.Connection = trans.Connection;
                cmd.Transaction = trans;
            }

            int count = 0;
            ExecuteCommand(cmd, () => { count = cmd.ExecuteNonQuery(); });
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        public override int Delete(string tableName, object key, DbTransaction trans = null)
        {
            var cmd = CreateCommand();
            var criteria = new List<string>();
            var props = TypeDescriptor.GetProperties(key);
            foreach (PropertyDescriptor prop in props)
            {
                criteria.Add(string.Format("{0} = @{0}", prop.Name));
                AddParameter(cmd, prop, key);
            }

            if (criteria.Count == 0)
                cmd.CommandText = string.Format("DELETE FROM {0}", tableName);
            else
                cmd.CommandText = string.Format("DELETE FROM {0} WHERE {1}", tableName, string.Join(" AND ", criteria.ToArray()));

            if (trans != null)
            {
                cmd.Connection = trans.Connection;
                cmd.Transaction = trans;
            }

            int count = 0;
            ExecuteCommand(cmd, () => { count = cmd.ExecuteNonQuery(); });
            return count;
        }

        /// <summary>
        /// Determines if the record exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns></returns>
        public override bool Exists(string tableName, object key, DbTransaction trans = null)
        {
            var cmd = CreateCommand();
            var criteria = new List<string>();
            var props = TypeDescriptor.GetProperties(key);
            foreach (PropertyDescriptor prop in props)
            {
                criteria.Add(string.Format("{0} = @{0}", prop.Name));
                AddParameter(cmd, prop, key);
            }

            if (criteria.Count == 0)
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM {0}", tableName);
            else
                cmd.CommandText = string.Format("SELECT COUNT(*) FROM {0} WHERE {1}", tableName, string.Join(" AND ", criteria.ToArray()));

            if (trans != null)
            {
                cmd.Connection = trans.Connection;
                cmd.Transaction = trans;
            }

            int count = 0;
            ExecuteCommand(cmd, () => { count = (int)cmd.ExecuteScalar(); });
            if (count == 0)
                return false;
            else
                return true;
        }

        #endregion

    }

    /// <summary>
    /// IDbInfo object for Sql Server.
    /// </summary>
    public class SqlDbInfo
        : AnsiSqlDbInfo
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDbInfo.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbInfo(string connStr)
        {
            mConnStr = connStr;
        }

        #endregion

        #region Fields and Properties

        private string mConnStr;
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnStr
        {
            get { return mConnStr; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return new System.Data.SqlClient.SqlConnection(mConnStr);
        }

        /// <summary>
        /// Creates a command object that can be used to execute queries.
        /// </summary>
        /// <returns></returns>
        public override DbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        #endregion

    }

}
