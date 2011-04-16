using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using My = BizArk.Core.Properties;
using BizArk.Core.ORM;
using System.Linq;

namespace BizArk.Core.Data
{

    /// <summary>
    /// Provides some utility functions for accessing a Sql Server database.
    /// </summary>
    public class Database : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of Database. Uses the Database.DefaultDatabase. Throws an exception if DefaultDatabase is not set.
        /// </summary>
        public Database()
            : this(DefaultDatabase)
        {
        }

        /// <summary>
        /// Creates an instance of Database.
        /// </summary>
        /// <param name="dbInfo"></param>
        public Database(IDbInfo dbInfo)
        {
            if (dbInfo == null) throw new ArgumentNullException("dbInfo", "IDbInfo is required. If you called this with the default constructor, make sure Database.DefaultDatabase is set.");
            mDbInfo = dbInfo;
        }

        /// <summary>
        /// Destroys the Database. If a transaction has been started but not completed, automatically rolls it back.
        /// </summary>
        public void Dispose()
        {
            if (CurrentUpdate != null)
                CurrentUpdate.Dispose();
        }

        #endregion

        #region Database registration

        private static Dictionary<string, IDbInfo> sDatabases = new Dictionary<string, IDbInfo>();

        /// <summary>
        /// Registers a IDbInfo object so that new database can be created based on a name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="db"></param>
        public static void RegisterDatabase(string name, IDbInfo db)
        {
            if (sDatabases.ContainsKey(name))
                sDatabases[name] = db;
            else
                sDatabases.Add(name, db);
        }

        /// <summary>
        /// Instantiates a new Database using the IDbInfo that was registered with the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Database CreateDatabase(string name)
        {
            var db = sDatabases[name];
            if (db == null) throw new IndexOutOfRangeException(string.Format("'{0}' has not been registered.", name));
            return new Database(db);
        }

        /// <summary>
        /// Gets or sets the default database. This database will be used when the default constructor for Database is used.
        /// </summary>
        public static IDbInfo DefaultDatabase { get; set; }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the pending database update.
        /// </summary>
        public DatabaseUpdate CurrentUpdate { get; internal set; }

        private IDbInfo mDbInfo;
        /// <summary>
        /// Gets the database specific info object.
        /// </summary>
        public IDbInfo DbInfo
        {
            get { return mDbInfo; }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Creates a new sql connection but does not connect. You are responsible for disposing the connection.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DbConnection CreateConnection()
        {
            return mDbInfo.CreateConnection();
        }

        private DbCommand CreateCommand(string sql, object values)
        {
            var cmd = DbInfo.CreateCommand();
            cmd.CommandText = sql;
            if (values == null) return cmd;

            var props = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor prop in props)
            {
                var paramName = "@" + prop.Name;
                if (!sql.Contains(paramName)) continue;
                var parameter = cmd.CreateParameter();
                parameter.ParameterName = paramName;
                var value = prop.GetValue(values);
                if (ConvertEx.IsEmpty(value))
                    value = DBNull.Value;
                parameter.Value = value;
                cmd.Parameters.Add(parameter);
            }
            return cmd;
        }

        private void ExecuteCommand(DbCommand cmd, ExecuteDelegate execute)
        {
            var connSet = false;
            if (CurrentUpdate != null && cmd.Connection == null)
            {
                // Make sure the connection and transaction are set correctly 
                // when running within a transaction.
                cmd.Connection = CurrentUpdate.Connection;
                cmd.Transaction = CurrentUpdate.Transaction;
                connSet = true;
            }
            DbInfo.ExecuteCommand(cmd, execute);
            if (connSet)
            {
                cmd.Connection = null;
                cmd.Transaction = null;
            }
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public T ExecuteScalar<T>(string sql, object values)
        {
            var cmd = CreateCommand(sql, values);
            return ExecuteScalar<T>(cmd);
        }

        /// <summary>
        /// Executes the command and returns the first column of the first row.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public int ExecuteNonQuery(string sql, object values)
        {
            var cmd = CreateCommand(sql, values);
            return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes the command against the database.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void ExecuteDataReader(string sql, object values, ProcessRowDelegate processRow)
        {
            var cmd = CreateCommand(sql, values);
            ExecuteDataReader(cmd, processRow);
        }

        /// <summary>
        /// Processes a SqlDataReader calling the processRow delegate for each row.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="processRow">void ProcessDataRow(SqlDataReader dr)</param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataTable GetDataTable(string sql, object values)
        {
            var cmd = CreateCommand(sql, values);
            return GetDataTable(cmd);
        }

        /// <summary>
        /// Loads a DataTable based on the command.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
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
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataRow GetDataRow(string sql, object values)
        {
            var cmd = CreateCommand(sql, values);
            return GetDataRow(cmd);
        }

        /// <summary>
        /// Returns the first row of the DataTable. Null if no rows.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DataRow GetDataRow(DbCommand cmd)
        {
            var tbl = GetDataTable(cmd);
            if (tbl.Rows.Count == 0) return null;
            return tbl.Rows[0];
        }

        /// <summary>
        /// Sets the writable properties of an object from the IDataReader.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="obj"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void FillObject(IDataReader row, object obj)
        {

            var props = TypeDescriptor.GetProperties(obj);
            for (int i = 0; i < row.FieldCount; i++)
            {
                var fieldName = row.GetName(i);
                var prop = props.Find(fieldName, true);
                if (prop != null) prop.SetValue(obj, ConvertEx.ChangeType(row[i], prop.PropertyType));
            }
        }

        /// <summary>
        /// Creates a DbCommand object for a stored procedure.
        /// </summary>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public DbCommand CreateProcCommand(string procName, object values)
        {
            var cmd = DbInfo.CreateCommand();
            cmd.CommandText = procName;
            cmd.CommandType = CommandType.StoredProcedure;

            var props = TypeDescriptor.GetProperties(values);
            foreach (PropertyDescriptor prop in props)
                BaseDbInfo.AddParameter(cmd, prop, values);

            return cmd;
        }

        /// <summary>
        /// Allows database updates to occur within a transaction. All commands will run through the update transaction until it is completed (unless a connection is explicity set on the DbCommand object).
        /// </summary>
        /// <returns></returns>
        public DatabaseUpdate BeginUpdate()
        {
            if (CurrentUpdate != null) throw new InvalidOperationException("An update has already been started.");
            CurrentUpdate = new DatabaseUpdate(this, CreateConnection());
            return CurrentUpdate;
        }

        #endregion

        #region Object Methods

        /// <summary>
        /// Executes a command and returns the first row as the given object. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T SelectSingle<T>(string sql, object values, ProcessRowDelegate<T> process = null) where T : class
        {
            var cmd = CreateCommand(sql, values);
            return SelectSingle<T>(cmd, process);
        }

        /// <summary>
        /// Executes a command and returns the first row as the given object. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T SelectSingle<T>(DbCommand cmd, ProcessRowDelegate<T> process = null) where T : class
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
        /// <param name="sql">Parameterized sql string.</param>
        /// <param name="values">Object that contains parameters. Property names must match parameters in sql.</param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T[] Select<T>(string sql, object values, ProcessRowDelegate<T> process = null) where T : class
        {
            var cmd = CreateCommand(sql, values);
            return Select<T>(cmd, process);
        }

        /// <summary>
        /// Executes a command and returns the rows as an array of objects. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="process">If not set, sets properties based on the field name returned from the command.</param>
        /// <returns></returns>
        public T[] Select<T>(DbCommand cmd, ProcessRowDelegate<T> process = null) where T : class
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
        /// Executes a stored procedure and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public T ExecuteProcScalar<T>(string procName, object values)
        {
            var dflt = (T)ConvertEx.GetDefaultEmptyValue<T>();
            return ExecuteProcScalar<T>(procName, values, dflt);
        }

        /// <summary>
        /// Executes a stored procedure and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public T ExecuteProcScalar<T>(string procName, object values, T dflt)
        {
            var cmd = CreateProcCommand(procName, values);
            return ExecuteScalar<T>(cmd, dflt);
        }

        /// <summary>
        /// Executes a stored procedure and returns a scalar value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public int ExecuteProcNonQuery<T>(string procName, object values)
        {
            var cmd = CreateProcCommand(procName, values);
            return ExecuteNonQuery(cmd);
        }

        /// <summary>
        /// Executes a stored procedure and returns a single result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public T ExecuteProcSingle<T>(string procName, object values, ProcessRowDelegate<T> process = null) where T : class
        {
            var cmd = CreateProcCommand(procName, values);
            return SelectSingle<T>(cmd, process);
        }

        /// <summary>
        /// Executes a stored procedure and returns a list of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="procName"></param>
        /// <param name="values"></param>
        /// <param name="process"></param>
        /// <returns></returns>
        public T[] ExecuteProc<T>(string procName, object values, ProcessRowDelegate<T> process = null) where T : class
        {
            var cmd = CreateProcCommand(procName, values);
            return Select<T>(cmd, process);
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
            if (CurrentUpdate == null)
                return mDbInfo.Insert(tableName, values);
            else
                return mDbInfo.Insert(tableName, values, CurrentUpdate.Transaction);
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
            if (CurrentUpdate == null)
                return mDbInfo.Update(tableName, key, values);
            else
                return mDbInfo.Update(tableName, key, values, CurrentUpdate.Transaction);
        }

        /// <summary>
        /// Removes a record from the table.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key">Uses AND when building the criteria.</param>
        /// <returns>The number of rows affected.</returns>
        public int Delete(string tableName, object key)
        {
            if (CurrentUpdate == null)
                return mDbInfo.Delete(tableName, key);
            else
                return mDbInfo.Delete(tableName, key, CurrentUpdate.Transaction);
        }

        /// <summary>
        /// Determines if the record exists.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string tableName, object key)
        {
            if (CurrentUpdate == null)
                return mDbInfo.Exists(tableName, key);
            else
                return mDbInfo.Exists(tableName, key, CurrentUpdate.Transaction);
        }

        #endregion

        #region Entity Methods

        private T ProcessEntityRow<T>(IDataReader row, EntityManager mgr) where T : Entity
        {
            var entity = Activator.CreateInstance<T>();
            var props = mgr.Properties;
            for (int i = 0; i < row.FieldCount; i++)
            {
                var fldName = row.GetName(i);
                var prop = props.FirstOrDefault(p => { return p.ColumnName == fldName; });
                if (prop == null) continue;
                // Make sure the value is compatible with the property type.
                var val = ConvertEx.ChangeType(row[i], prop.PropertyType);
                entity.State.SetValue_Internal(prop.Name, val);
            }

            entity.State.SetUnmodified();
            entity.State.IsNew = false;
            return entity;
        }

        /// <summary>
        /// Inserts, updates, or deletes the entity, depending on the state of the entity. Will not issue sql updates if not necessary. Updates the identifier property if defined. Initializes the original values once it is done.
        /// </summary>
        /// <param name="entity"></param>
        public void Save(Entity entity)
        {
            if (entity == null) return;

            // No need to issue a sql statement if we are deleting an entity that is not in the database yet.
            if (entity.State.IsNew && entity.State.IsDeleted) return;

            // Need to validate the entity before saving it to the database.
            entity.State.Validate();

            DatabaseUpdate update = null;
            if (CurrentUpdate == null)
                update = BeginUpdate();
            var updateOK = false;
            try
            {
                if (entity.State.IsNew)
                    Insert(entity);
                else if (entity.State.IsDeleted)
                    Delete(entity);
                else if (entity.State.IsModified)
                    Update(entity);

                if (update != null)
                    update.Commit();
                updateOK = true;
            }
            finally
            {
                if (update != null)
                {
                    if (!updateOK)
                        update.Rollback();
                    update.Dispose();
                }
            }
        }

        /// <summary>
        /// Inserts, updates, or deletes the entity, depending on the state of the entity. Will not issue sql updates if not necessary. Updates the identifier property if defined. Initializes the original values once it is done.
        /// </summary>
        /// <param name="entities"></param>
        public void Save<T>(IEnumerable<T> entities) where T : Entity
        {
            foreach (var entity in entities)
                Save(entity);
        }

        private void Insert(Entity entity)
        {
            var mgr = entity.State.Manager;
            var fields = new List<FieldValue>();
            foreach (var val in entity.State.GetValuesForSave())
            {
                var fld = val.GetFieldValue();
                fields.Add(fld);
            }
            var id = Insert(mgr.TableName, fields);
            if (mgr.Identity != null)
                entity.State.SetValue_Internal(mgr.Identity.Name, id);

            entity.State.IsNew = false;
            entity.State.SetUnmodified();
        }

        private void Update(Entity entity)
        {
            var mgr = entity.State.Manager;
            var fields = new List<FieldValue>();
            foreach (var val in entity.State.GetValuesForSave())
            {
                var fld = val.GetFieldValue();
                fields.Add(fld);
            }
            Update(mgr.TableName, entity.State.DbKey, fields);
            entity.State.SetUnmodified();
        }

        private void Delete(Entity entity)
        {
            var mgr = entity.State.Manager;
            var count = this.Delete(mgr.TableName, entity.State.DbKey);
            if (count > 1) throw new InvalidOperationException(string.Format("Multiple rows were changed in the database when deleting {0}.", entity));
        }

        #endregion

        #region Support

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
            FillObject(row, obj);
            return obj;
        }

        #endregion

    }

    /// <summary>
    /// Encapsulates the properties and methods for a database transaction.
    /// </summary>
    public class DatabaseUpdate : IDisposable
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DatabaseUpdate.
        /// </summary>
        /// <param name="database"></param>
        /// <param name="conn"></param>
        internal DatabaseUpdate(Database database, DbConnection conn)
        {
            Database = database;
            Connection = conn;
            if (Connection.State == ConnectionState.Closed)
                Connection.Open();
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// Destroys the DatabaseUpdate. If the transaction hasn't been completed, automatically rolls it back.
        /// </summary>
        public void Dispose()
        {
            if (Transaction != null)
                Rollback();
            Database.CurrentUpdate = null;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the database associated with the update.
        /// </summary>
        public Database Database { get; private set; }

        /// <summary>
        /// Gets the connection associated with the update.
        /// </summary>
        public DbConnection Connection { get; private set; }

        /// <summary>
        /// Gets the transaction associated with the update.
        /// </summary>
        public DbTransaction Transaction { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Commits the database updates.
        /// </summary>
        public void Commit()
        {
            if (Transaction == null) throw new InvalidOperationException("The transaction has already been completed.");

            Transaction.Commit();

            Transaction.Dispose();
            Transaction = null;
            Connection.Dispose();
            Connection = null;
            Database.CurrentUpdate = null;
        }

        /// <summary>
        /// Rollsback the database updates.
        /// </summary>
        public void Rollback()
        {
            if (Transaction == null) throw new InvalidOperationException("The transaction has already been completed.");

            Transaction.Rollback();

            Transaction.Dispose();
            Transaction = null;
            Connection.Dispose();
            Connection = null;
            Database.CurrentUpdate = null;
        }

        #endregion

    }

}
