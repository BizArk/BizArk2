﻿using System;
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
        public void ExecuteDataReader(SqlCommand cmd, ProcessRowDelegate processRow)
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

        /// <summary>
        /// Executes a command and returns the first row as the given object. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T GetObject<T>(SqlCommand cmd, ProcessRowDelegate<T> process = null) where T : class
        {
            if (process == null)
                process = new ProcessRowDelegate<T>(ProcessRow<T>);

            T obj = null;
            ExecuteDataReader(cmd, (row) => { obj = process(row); return false; });
            return obj;
        }

        /// <summary>
        /// Executes a command and returns the rows as an array of objects. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T[] GetObjects<T>(SqlCommand cmd, ProcessRowDelegate<T> process = null) where T : class
        {
            if (process == null)
                process = new ProcessRowDelegate<T>(ProcessRow<T>);

            var objs = new List<T>();
            ExecuteDataReader(cmd, (row) =>
                {
                    var obj = process(row); 
                    if(obj != null) objs.Add(obj); 
                    return true;
                });
            return objs.ToArray();
        }

        /// <summary>
        /// Inserts the given object into the given table. The properties are expected to match the field names. Executes SCOPE_IDENTITY() and returns the value.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="values"></param>
        /// <returns>The identity value. int.MinValue if not available.</returns>
        public int Insert(string tableName, object values)
        {
            return 0;
        }

        /// <summary>
        /// Updates a given record identified by the key.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <param name="values"></param>
        /// <returns>The number of rows affected.</returns>
        public int Update(string tableName, object key, object values)
        {
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(string tableName, object key)
        {
            return 0;
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
        /// <returns>True to continue, false to stop processing rows.</returns>
        public delegate bool ProcessRowDelegate(SqlDataReader dr);

        /// <summary>
        /// Processes a data row and returns the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public delegate T ProcessRowDelegate<T>(IDataReader dr);

        /// <summary>
        /// Implements ProcessRowDelegate[T].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        private T ProcessRow<T>(IDataReader row)
        {
            var obj = Activator.CreateInstance<T>();
            var props = TypeDescriptor.GetProperties(obj);
            for (int i = 0; i < row.FieldCount; i++)
            {
                var fieldName = row.GetName(i);
                var prop = props.Find(fieldName, true);
                if (prop != null) prop.SetValue(obj, ConvertEx.ChangeType(row[i], prop.PropertyType));
            }
            return obj;
        }

        #endregion

    }

}
