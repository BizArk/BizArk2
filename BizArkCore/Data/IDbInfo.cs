using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;

namespace BizArk.Core.Data
{

    /// <summary>
    /// This interface is used to initialize a Database instance with a specific provider.
    /// </summary>
    public interface IDbInfo
    {

        /// <summary>
        /// Creates an instance of DbConnection. The connection is NOT opened.
        /// </summary>
        /// <returns></returns>
        DbConnection CreateConnection();

        /// <summary>
        /// Creates an instance of a DbCommand. Populates the command if the parameters are supplied.
        /// </summary>
        /// <param name="commandText">The command text for the command.</param>
        /// <param name="parameters">An object that contains properties to use as parameters for the command. The property name must match a parameter in the sql or it will be ignored.</param>
        /// <returns></returns>
        DbCommand CreateCommand(string commandText = null, object parameters = null);
    }

    /// <summary>
    /// Provides methods for working with a Sql Server database.
    /// </summary>
    public class SqlDbInfo : IDbInfo
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDbInfo.
        /// </summary>
        /// <param name="connStr"></param>
        public SqlDbInfo(string connStr)
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
        public DbConnection CreateConnection()
        {
            return new SqlConnection(ConnStr);
        }

        /// <summary>
        /// Creates an instance of a SqlCommand. Populates the command if the parameters are supplied.
        /// </summary>
        /// <param name="sql">The T-SQL for the command.</param>
        /// <param name="parameters">An object that contains properties to use as parameters for the command. The property name must match a parameter in the sql or it will be ignored.</param>
        /// <returns></returns>
        public DbCommand CreateCommand(string sql = null, object parameters = null)
        {
            var cmd = new SqlCommand();
            cmd.CommandText = sql;
            if (parameters == null) return cmd;

            var props = TypeDescriptor.GetProperties(parameters);
            foreach (PropertyDescriptor prop in props)
            {
                var paramName = "@" + prop.Name;
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

        #endregion

    }

}
