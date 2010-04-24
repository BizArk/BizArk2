using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using My = Redwerb.BizArk.Core.Properties;

namespace Redwerb.BizArk.Core.Data
{

    /// <summary>
    /// Provides some utility functions for accessing a Sql Server database.
    /// </summary>
    public class Database
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SqlDatabase.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dbInfo"></param>
        public Database(string name, IDbInfo dbInfo)
        {
            mName = name;
            mDbInfo = dbInfo;
        }

        #endregion

        #region Fields and Properties

        private static Dictionary<string, Database> sDatabases = new Dictionary<string, Database>();

        private string mName;
        /// <summary>
        /// Gets the name of this database. Used for registration.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        private IDbInfo mDbInfo;
        /// <summary>
        /// Gets the database specific info object.
        /// </summary>
        public IDbInfo DbInfo
        {
            get { return mDbInfo; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the database so that it can be retrieved later.
        /// </summary>
        /// <param name="db"></param>
        public static void RegisterDatabase(Database db)
        {
            sDatabases.Add(db.Name, db);
        }

        /// <summary>
        /// Gets the named database.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Database GetDatabase(string name)
        {
            return sDatabases[name];
        }

        private static Database sDefaultDatabase;
        /// <summary>
        /// Gets or sets the default database to use.  If this property is not set explicitly, a default one will be created automatically based on the DefaultConnStr connection string defined in Redwerb.BizArk.Core.dll.config.
        /// </summary>
        public static Database DefaultDatabase
        {
            get
            {
                if (sDefaultDatabase == null)
                    sDefaultDatabase = new Database("Default", new SqlDbInfo(My.Settings.Default.DefaultConnStr));
                return sDefaultDatabase;
            }
            set { sDefaultDatabase = value; }
        }

        /// <summary>
        /// Creates a new sql connection but does not connect. You are responsible for disposing the connection.
        /// </summary>
        /// <returns></returns>
        public DbConnection CreateConnection()
        {
            return mDbInfo.CreateConnection();
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
        /// <param name="dflt"></param>
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
        /// <param name="cmd"></param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        public void ExecuteDataReader(DbCommand cmd, ProcessRowDelegate processRow)
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
        public DataTable GetDataTable(DbCommand cmd)
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
        public DataRow GetDataRow(DbCommand cmd)
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
        protected void ExecuteCommand(DbCommand cmd, ExecuteDelegate execute)
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
        public T GetObject<T>(DbCommand cmd, ProcessRowDelegate<T> process = null) where T : class
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
        public T[] GetObjects<T>(DbCommand cmd, ProcessRowDelegate<T> process = null) where T : class
        {
            if (process == null)
                process = new ProcessRowDelegate<T>(ProcessRow<T>);

            var objs = new List<T>();
            ExecuteDataReader(cmd, (row) =>
                {
                    var obj = process(row);
                    if (obj != null) objs.Add(obj);
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
        /// <remarks>
        /// This method will create a new database record in the specified table with the values in the object. Only accepts value types (int, float, etc), enums, and strings.
        /// </remarks>
        public int Insert(string tableName, object values)
        {
            return mDbInfo.Insert(tableName, values);
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
            return mDbInfo.Update(tableName, key, values);
        }

        /// <summary>
        /// Removes a record from the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(string tableName, object key)
        {
            return mDbInfo.Delete(tableName, key);
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
        public delegate bool ProcessRowDelegate(IDataReader dr);

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
