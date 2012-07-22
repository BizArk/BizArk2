using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using BizArk.Core;
using BizArk.Core.Extensions.ObjectExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.MathExt;
using BizArk.Core.Util;
using System.Data;
using BizArk.Core.Convert.Strategies;

namespace BizArk.DB.Adapters
{

    /// <summary>
    /// Provides methods for working with a Sql Server database.
    /// </summary>
    public abstract class AnsiSqlDbAdapter : IDbAdapter
    {

        #region Methods

        /// <summary>
        /// Creates an instance of DbConnection. The connection is NOT opened.
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Creates an instance of DbCommand.
        /// </summary>
        /// <returns></returns>
        protected abstract DbCommand CreateCommand();

        private static DbCommand sNowCmd;
        /// <summary>
        /// Gets the DateTime from the database server.
        /// </summary>
        public virtual DateTime NowUtc(Database db)
        {
            if (sNowCmd == null)
            {
                sNowCmd = CreateCommand();
                sNowCmd.CommandText = "SELECT GETUTCDATE()";
            }
            return db.ExecuteScalar<DateTime>(sNowCmd);
        }

        /// <summary>
        /// Creates an instance of a SqlCommand. Populates the command if the parameters are supplied.
        /// </summary>
        /// <param name="sql">The T-SQL for the command.</param>
        /// <param name="parameters">An object that contains properties to use as parameters for the command. The property name must match a parameter in the sql or it will be ignored.</param>
        /// <returns></returns>
        public virtual DbCommand CreateCommand(string sql = null, object parameters = null)
        {
            var cmd = CreateCommand();
            cmd.CommandText = sql;
            if (parameters == null) return cmd;

            var props = TypeDescriptor.GetProperties(parameters);
            foreach (PropertyDescriptor prop in props)
            {
                var paramName = GetParameterName(prop.Name);
                if (!sql.Contains(paramName)) continue;
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = paramName;
                var value = prop.GetValue(parameters);
                if (ConvertEx.IsEmpty(value))
                    value = DBNull.Value;
                parameter.Value = value;
                cmd.Parameters.Add(parameter);
            }
            return cmd;
        }

        /// <summary>
        /// Used to get the command parameter name based on a base name.
        /// </summary>
        /// <param name="baseName"></param>
        /// <returns></returns>
        protected virtual string GetParameterName(string baseName)
        {
            return "@" + baseName;
        }

        /// <summary>
        /// Creates a command that will add the record to the database. If the table includes an auto-incrementing identity, the value should be returned as a scalar value (single row, single field).
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table that has the records we are deleting.</param>
        /// <param name="data">The data to insert into the table.</param>
        /// <param name="identityFieldName">The name of the identity field. Leave null if the record doesn't have an identity.</param>
        /// <returns>If identityFieldName contains a value, returns the identity of the new record. Otherwise null.</returns>
        public virtual int? Insert(Database db, string tableName, NameValue[] data, string identityFieldName)
        {
            var cmd = CreateCommand();
            var fields = new List<string>();
            var parameters = new List<string>();
            foreach (var val in data)
            {
                var param = AddParameter(cmd, val);
                fields.Add(val.Name);
                parameters.Add(param.ParameterName);
            }

            var sb = new StringBuilder();
            sb.AppendFormat("INSERT INTO {0} ({1})\n", tableName, string.Join(", ", fields.ToArray()));
            sb.AppendFormat("\tVALUES({0})", string.Join(", ", parameters.ToArray()));
            cmd.CommandText = sb.ToString();

            if (identityFieldName.IsEmpty())
            {
                db.ExecuteNonQuery(cmd);
                return null;
            }
            else
            {
                return InsertIdentity(db, cmd, identityFieldName);
            }

        }

        /// <summary>
        /// Override to handle getting the identity value. This is only called if identityFieldName has a value.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="cmd"></param>
        /// <param name="identityFieldName"></param>
        /// <returns></returns>
        protected virtual int InsertIdentity(Database db, DbCommand cmd, string identityFieldName)
        {
            cmd.CommandText += ";SELECT SCOPE_IDENTITY();";
            return db.ExecuteScalar<int>(cmd);
        }

        /// <summary>
        /// Updates the record with the given key in the database.
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table that has the records we are deleting.</param>
        /// <param name="key">The key to the record to delete.</param>
        /// <param name="data">The data to insert into the table.</param>
        /// <returns>The number of records effected.</returns>
        public virtual int Update(Database db, string tableName, NameValue[] key, NameValue[] data)
        {
            var cmd = CreateCommand();
            var criteria = new List<string>();
            foreach (var val in key)
            {
                var param = AddParameter(cmd, val);
                criteria.Add("{0}={1}".Fmt(val.Name, param.ParameterName));
            }

            var fields = new List<string>();
            foreach (var val in data)
            {
                var param = AddParameter(cmd, val);
                fields.Add("{0}={1}".Fmt(val.Name, param.ParameterName));
            }

            var sb = new StringBuilder();
            sb.AppendFormat("UPDATE {0} SET\n\t\t", tableName);
            sb.AppendFormat(string.Join(",\n\t\t", fields.ToArray()));
            if (criteria.Count > 0)
                sb.AppendFormat("\n\tWHERE {1}", tableName, string.Join("\n\t\tAND ", criteria.ToArray()));
            cmd.CommandText = sb.ToString();

            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Creates a command that will removes the record with the given key from the database.
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table that has the records we are deleting.</param>
        /// <param name="key">The key to the record to delete.</param>
        /// <returns>The number of records effected.</returns>
        public virtual int Delete(Database db, string tableName, NameValue[] key)
        {
            var cmd = CreateCommand();
            var criteria = new List<string>();
            foreach (var val in key)
            {
                var param = AddParameter(cmd, val);
                criteria.Add("{0}={1}".Fmt(val.Name, param.ParameterName));
            }

            var sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0}", tableName);
            if (criteria.Count > 0)
                sb.AppendFormat(" WHERE {1}", tableName, string.Join(" AND ", criteria.ToArray()));
            cmd.CommandText = sb.ToString();

            return db.ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Adds the value as a parameter.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected virtual DbParameter AddParameter(DbCommand cmd, NameValue value)
        {
            var val = ConvertEx.IsEmpty(value.Value) ? DBNull.Value : value.Value;
            var baseParamName = GetParameterName(value.Name);
            var paramName = baseParamName;
            var dbType = DbTypeMap.ToDbType(value.ValueType);
            var size = GetSize(dbType, val);
            DbParameter param;
            var i = 1;

            while (cmd.Parameters.Contains(paramName))
            {
                param = cmd.Parameters[paramName];
                if (param.Value == val && param.DbType == dbType)
                {
                    // same name, same value, same parameter
                    param.Size = Math.Max(param.Size, size);
                    return param;
                }
                paramName = "{0}{1}".Fmt(baseParamName, i++); // increment value
            }

            param = cmd.CreateParameter();
            param.ParameterName = paramName;
            param.DbType = dbType;
            param.Value = val;
            param.Size = size;
            cmd.Parameters.Add(param);
            return param;
        }

        /// <summary>
        /// Gets the size for the value.
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        protected virtual int GetSize(DbType dbType, object val)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    var str = val as String;
                    if (str != null)
                        return str.Length;
                    else
                        return 0;
                case DbType.Binary:
                    var bin = val as byte[];
                    if (bin == null)
                        return 0;
                    else
                        return bin.Length;
            }
            return 0;
        }

        #endregion

    }

}
