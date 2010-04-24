using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.ComponentModel;

namespace Redwerb.BizArk.Core.Data
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
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        int Insert(string tableName, object values);

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="values"></param>
        /// <returns>The number of rows affected.</returns>
        int Update(string tableName, object key, object values);

        /// <summary>
        /// Removes a record from the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <returns>The number of rows affected.</returns>
        int Delete(string tableName, object key);

    }

    /// <summary>
    /// Provides basic method definitions for Ansi-standard databases.
    /// </summary>
    public abstract class AnsiSqlDbInfo
        : IDbInfo
    {

        #region Methods

        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        public int Insert(string tableName, object values)
        {
            var props = TypeDescriptor.GetProperties(values);
            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();
                var fields = new List<string>();
                foreach (PropertyDescriptor prop in props)
                {
                    fields.Add(prop.Name);
                    AddParameter(cmd, prop, values);
                }
                if (fields.Count == 0) return 0;

                cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})", tableName, string.Join(", ", fields.ToArray()), string.Join(", @", fields.ToArray()));

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="values"></param>
        /// <returns>The number of rows affected.</returns>
        public int Update(string tableName, object key, object values)
        {
            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();
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

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(string tableName, object key)
        {
            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();
                var fields = new List<string>();
                var criteria = new List<string>();
                var props = TypeDescriptor.GetProperties(key);
                foreach (PropertyDescriptor prop in props)
                {
                    fields.Add(string.Format("{0} = @{0}", prop.Name));
                    AddParameter(cmd, prop, key);
                }

                if (criteria.Count == 0)
                    cmd.CommandText = string.Format("DELETE FROM {0}", tableName);
                else
                    cmd.CommandText = string.Format("DELETE FROM {0} WHERE {1}", tableName, string.Join(" AND ", criteria.ToArray()));

                conn.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private void AddParameter(DbCommand cmd, PropertyDescriptor prop, object obj)
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

        #endregion

    }

}
