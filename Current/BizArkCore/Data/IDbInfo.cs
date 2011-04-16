using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Collections;
using BizArk.Core.AttributeExt;
using BizArk.Core.ORM;
using System.ComponentModel.DataAnnotations;
using System.Data;

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
        /// Inserts the given object into the given table. The properties of the parameter values are expected to match the field names or should be an IEnumarable of FieldValue objects. Executes SCOPE_IDENTITY() and returns the value.
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

        internal static DbParameter AddParameter(DbCommand cmd, PropertyDescriptor prop, object obj)
        {
            var fld = GetFieldFromProperty(prop, obj);
            return AddParameter(cmd, fld);
        }

        internal static DbParameter AddParameter(DbCommand cmd, FieldValue fld)
        {
            var parameter = cmd.CreateParameter();
            parameter.ParameterName = "@" + fld.Name;
            var value = fld.Value;
            if (ConvertEx.IsEmpty(value))
                value = DBNull.Value;
            parameter.Value = value;
            parameter.Size = fld.Size;
            cmd.Parameters.Add(parameter);


            var param = cmd.CreateParameter();
            param.ParameterName = "@" + fld.Name;

            if (fld.DbType < 0)
                param.Value = fld.Value;
            else
            {
                param.DbType = fld.DbType;
                if (fld.Size > 0) param.Size = fld.Size;

                if (ConvertEx.IsEmpty(fld.Value))
                    param.Value = DBNull.Value;
                else
                {
                    switch (fld.DbType)
                    {
                        case DbType.Int32:
                            // Useful for enums that are saved as ints.
                            param.Value = ConvertEx.ToInt32(fld.Value);
                            break;
                        case DbType.String:
                            // Useful for enums that are saved as strings.
                            param.Value = ConvertEx.ToString(fld.Value);
                            break;
                        default:
                            param.Value = fld.Value;
                            break;
                    }
                }
            }

            return param;
        }

        /// <summary>
        /// Gets a list of FieldValue objects out of the values parameter.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected FieldValue[] GetFields(object values)
        {
            var fields = new List<FieldValue>();

            var flds = values as IEnumerable;
            if (flds != null)
            {
                foreach (var fld in flds)
                {
                    var fldval = fld as FieldValue;
                    if (fldval == null) throw new ArgumentException("If values is of type IEnumerable, all the items must be of type FieldValue.");
                    fields.Add(fldval);
                }
            }
            else
            {
                var props = TypeDescriptor.GetProperties(values);
                foreach (PropertyDescriptor prop in props)
                {
                    var fld = GetFieldFromProperty(prop, values);
                    fields.Add(fld);
                }
            }

            return fields.ToArray();
        }

        internal static FieldValue GetFieldFromProperty(PropertyDescriptor prop, object obj)
        {
            var fld = new FieldValue(prop.Name, prop.GetValue(obj));

            var colAtt = prop.GetAttribute<ColumnAttribute>(true);
            if (colAtt != null)
            {
                fld.DbName = colAtt.Name;
                fld.DbType = colAtt.DbType;
            }

            var sizeAtt = prop.GetAttribute<StringLengthAttribute>(true);
            if (sizeAtt != null && sizeAtt.MaximumLength > 0)
                fld.Size = sizeAtt.MaximumLength;

            return fld;
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
            var fields = GetFields(values);
            var fieldNames = new List<string>();
            foreach (var fld in fields)
            {
                fieldNames.Add(fld.Name);
                AddParameter(cmd, fld);
            }
            if (fieldNames.Count == 0) return 0;

            cmd.CommandText = string.Format("INSERT INTO {0} ({1}) VALUES (@{2}); SELECT SCOPE_IDENTITY();", tableName, string.Join(", ", fieldNames.ToArray()), string.Join(", @", fieldNames.ToArray()));

            if (trans != null)
            {
                cmd.Connection = trans.Connection;
                cmd.Transaction = trans;
            }

            int id = int.MinValue;
            ExecuteCommand(cmd, () => { var val = cmd.ExecuteScalar(); if (val != null) id = ConvertEx.ToInt32(val); });
            return id;
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
            var fields = GetFields(values);
            var fieldNames = new List<string>();
            foreach (var fld in fields)
            {
                fieldNames.Add(string.Format("{0} = @{0}", fld.Name));
                AddParameter(cmd, fld);
            }
            if (fieldNames.Count == 0) return 0;

            var criteria = new List<string>();
            fields = GetFields(key);
            foreach (var fld in fields)
            {
                criteria.Add(string.Format("{0} = @{0}", fld.Name));
                AddParameter(cmd, fld);
            }

            if (criteria.Count == 0)
                cmd.CommandText = string.Format("UPDATE {0} SET {1}", tableName, string.Join(", ", fieldNames.ToArray()));
            else
                cmd.CommandText = string.Format("UPDATE {0} SET {1} WHERE {2}", tableName, string.Join(", ", fieldNames.ToArray()), string.Join(" AND ", criteria.ToArray()));

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
        /// Removes the given record from the database.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <param name="trans">Optional: The transaction that the command should be a part of. The transaction should contain the connection to use.</param>
        /// <returns>The number of rows affected.</returns>
        public override int Delete(string tableName, object key, DbTransaction trans = null)
        {
            var cmd = CreateCommand();
            var criteria = new List<string>();
            var fields = GetFields(key);
            foreach (var fld in fields)
            {
                criteria.Add(string.Format("{0} = @{0}", fld.Name));
                AddParameter(cmd, fld);
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
            var fields = GetFields(key);
            foreach (var fld in fields)
            {
                criteria.Add(string.Format("{0} = @{0}", fld.Name));
                AddParameter(cmd, fld);
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
