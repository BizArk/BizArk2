using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using BizArk.Core;
using BizArk.DB.Adapters;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.ObjectExt;
using BizArk.Core.Util;
using BizArk.DB.ORM;
using System.Collections.Specialized;
using System.Diagnostics;

namespace BizArk.DB
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
        /// <param name="adapter"></param>
        public Database(IDbAdapter adapter)
        {
            if (adapter == null) throw new ArgumentNullException("adapter");
            Adapter = adapter;
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

                if (mTrans != null)
                {
                    mTrans.Rollback();
                    mTrans.Dispose();
                    mTrans = null;
                }

                if (mConn != null)
                {
                    if (mConn.State == ConnectionState.Open)
                        mConn.Close();
                    mConn.Dispose();
                    mConn = null;
                }

                // There are no unmanaged resources to release, but
                // if we add them, they need to be released here.
            }
            Disposed = true;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the DbAdapter object associated with this Database.
        /// </summary>
        public IDbAdapter Adapter { get; private set; }

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
                    mConn = Adapter.CreateConnection();
                    mConn.Open();
                }
                return mConn;
            }
        }

        private RemoteDateTime mServerUtc;
        /// <summary>
        /// Gets the current UTC DateTime from the server.
        /// </summary>
        public DateTime NowUtc
        {
            get
            {
                if (mServerUtc == null)
                {
                    var dt = Adapter.NowUtc(this);
                    mServerUtc = new RemoteDateTime(dt);
                }
                return mServerUtc.Now;
            }
        }

        /// <summary>
        /// Gets the current DateTime from the server.
        /// </summary>
        public DateTime Now
        {
            get
            {
                var utc = NowUtc;
                return utc.ToLocalTime();
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
            return Adapter.CreateCommand(sql, parameters);
        }

        /// <summary>
        /// All database queries should go through this method.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="execute"></param>
        private void ExecuteCommand(DbCommand cmd, Action execute)
        {
            if (cmd.Connection == null)
                cmd.Connection = Connection;

            if (cmd.Transaction == null)
                cmd.Transaction = mTrans;

            DebugWriteCommand(cmd);

            execute();
        }

        /// <summary>
        /// Writes the command to the debug window including pseudocode for the parameters.
        /// </summary>
        /// <param name="cmd"></param>
        private void DebugWriteCommand(DbCommand cmd)
        {
            foreach (DbParameter param in cmd.Parameters)
            {
                var value = param.Value;
                if (value == DBNull.Value)
                    value = "NULL";
                else
                {
                    switch (param.DbType)
                    {
                        case DbType.AnsiString:
                        case DbType.Date:
                        case DbType.DateTime:
                        case DbType.DateTime2:
                        case DbType.DateTimeOffset:
                        case DbType.Guid:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Time:
                        case DbType.Xml:
                            value = "'{0}'".Fmt(value);
                            break;
                        case DbType.Binary:
                            value = "[BINARY]";
                            break;
                    }
                }
                Debug.WriteLine("DECLARE {0} AS {1} = {2}".Fmt(param.ParameterName, param.DbType, value));
            }
            Debug.WriteLine(cmd.CommandText);
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

        /// <summary>
        /// Adds the record to the database. If the table includes an auto-incrementing identity, the value is returned.
        /// </summary>
        /// <param name="tableName">The name of the table where to put the new record.</param>
        /// <param name="data">The data to insert into the table.</param>
        public void Insert(string tableName, object data)
        {
            Insert(tableName, data, null);
        }

        /// <summary>
        /// Adds the record to the database. If the table includes an auto-incrementing identity, the value is returned.
        /// </summary>
        /// <param name="tableName">The name of the table where to put the new record.</param>
        /// <param name="data">The data to insert into the table.</param>
        /// <param name="identityFieldName">The name of the identity field. Leave null if the record doesn't have an identity.</param>
        /// <returns>If identityFieldName contains a value, returns the identity of the new record. Otherwise null.</returns>
        public int? Insert(string tableName, object data, string identityFieldName)
        {
            return Adapter.Insert(this, tableName, data.GetNameValues(), identityFieldName);
        }

        /// <summary>
        /// Updates the record with the given key in the database.
        /// </summary>
        /// <param name="tableName">The name of the table that has the records we are updating.</param>
        /// <param name="key">The key to the record to update.</param>
        /// <param name="data">The data to update.</param>
        /// <returns>The number of records effected.</returns>
        public int Update(string tableName, object key, object data)
        {
            return Adapter.Update(this, tableName, key.GetNameValues(), data.GetNameValues());
        }

        /// <summary>
        /// Removes the record with the given key from the database.
        /// </summary>
        /// <param name="tableName">The name of the table that has the records we are deleting.</param>
        /// <param name="key">The key to the record to delete.</param>
        /// <returns>The number of records effected.</returns>
        public int Delete(string tableName, object key)
        {
            return Adapter.Delete(this, tableName, key.GetNameValues());
        }

        #endregion

        #region DbObject Methods

        /// <summary>
        /// Executes a command and returns the first row as an object. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public T SelectSingle<T>(DbCommand cmd) where T : class, ISupportDbState, new()
        {
            T obj = null;
            ExecuteDataReader(cmd, (row) => { obj = LoadDbObject<T>(row); });
            return obj;
        }

        /// <summary>
        /// Executes a command and returns the rows as an array of objects. The class must have a default constructor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public T[] Select<T>(DbCommand cmd) where T : class, ISupportDbState, new()
        {
            var objs = new List<T>();
            ExecuteDataReader(cmd, (row) =>
            {
                var obj = LoadDbObject<T>(row);
                if (obj != null) objs.Add(obj);
            });
            return objs.ToArray();
        }

        private T LoadDbObject<T>(IDataReader row) where T : class, ISupportDbState, new()
        {
            var obj = new T();
            var state = obj.DbState;
            for (var i = 0; i < row.FieldCount; i++)
            {
                var fldName = row.GetName(i);
                var fld = state[fldName];
                if (fld == null) continue;
                fld.SetValue(row[i]);
            }
            state.SetOriginals();
            return obj;
        }

        /// <summary>
        /// Saves a database object to the database.
        /// </summary>
        /// <param name="obj"></param>
        public void Save(ISupportDbState obj)
        {
            var state = obj.DbState;
            if (state.IsDeleted)
                Delete(obj);
            else if (state.IsNew)
                Insert(obj);
            else if (state.IsModified)
                Update(obj);
        }

        private void Delete(ISupportDbState obj)
        {
            // Don't need to delete new objects since they aren't in the database yet.
            if (obj.DbState.IsNew) return;

            var key = GetKey(obj);

            var count = Delete(obj.DbState.TableName, key);
            if (count != 1) throw new InvalidOperationException("Failed to properly delete the db object {0}. Expected to update only 1 record but actually deleted {1}.".Fmt(obj, count));
        }

        private void Insert(ISupportDbState obj)
        {
            // Don't need to insert objects that aren't modified.
            // Don't set IsNew to false since this is still a new object (it just can't be saved).
            if (!obj.DbState.IsModified) return;

            var values = GetChangedValues(obj);
            var id = Insert(obj.DbState.TableName, values, obj.DbState.Identity == null ? null : obj.DbState.Identity.FieldName);

            if (obj.DbState.Identity != null)
            {
                if (id == null)
                    throw new InvalidOperationException("Error inserting the db object {0}. Expected an identity value to be returned.".Fmt(obj));
                else
                    obj.DbState.Identity.Value = id.Value;
            }

            // The object is now saved. Set it so it's not saved again (unless modified)
            obj.DbState.SetOriginals();
            obj.DbState.IsNew = false;
        }

        private void Update(ISupportDbState obj)
        {
            var key = GetKey(obj);
            var values = GetChangedValues(obj);

            var count = Update(obj.DbState.TableName, key, values);
            if (count != 1) throw new InvalidOperationException("Failed to properly update the db object {0}. Expected to update only 1 record but actually updated {1}.".Fmt(obj, count));

            // The object is now saved. Set it so it's not saved again (unless modified)
            obj.DbState.SetOriginals();
        }

        private NameValue[] GetKey(ISupportDbState obj)
        {
            var key = new List<NameValue>();

            var id = obj.DbState.Identity;
            if (id != null)
            {
                key.Add(new NameValue(id.FieldName, id.GetValue(), id.GetValueType()));
            }
            else
            {
                foreach (var fld in obj.DbState.Key)
                    key.Add(new NameValue(fld.FieldName, fld.GetValue(), fld.GetValueType()));
            }

            return key.ToArray();
        }

        private NameValue[] GetChangedValues(ISupportDbState obj)
        {
            var values = new List<NameValue>();

            foreach (var fld in obj.DbState.Fields)
            {
                if (fld == obj.DbState.Identity) continue;
                if (!fld.IsModified) continue;
                values.Add(new NameValue(fld.FieldName, fld.GetValue(), fld.GetValueType()));
            }

            return values.ToArray();
        }

        #endregion

        #region Transaction Methods

        private DbTransaction mTrans;

        /// <summary>
        /// Gets a value that determines if a transaction has been started. If true, all commands will be run through the transaction.
        /// </summary>
        public bool InTransaction
        {
            get { return mTrans != null; }
        }

        /// <summary>
        /// Starts a dataqbase transaction.
        /// </summary>
        /// <returns></returns>
        public BizArkTransaction BeginTransaction()
        {
            if (mTrans != null) throw new InvalidOperationException("A transaction has already been started.");
            mTrans = Connection.BeginTransaction();
            return new BizArkTransaction(this, mTrans);
        }

        /// <summary>
        /// Starts a dataqbase transaction.
        /// </summary>
        /// <param name="isolationLevel"></param>
        /// <returns></returns>
        public BizArkTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (mTrans != null) throw new InvalidOperationException("A transaction has already been started.");
            mTrans = Connection.BeginTransaction(isolationLevel);
            return new BizArkTransaction(this, mTrans);
        }

        /// <summary>
        /// Commits a database transaction.
        /// </summary>
        public void CommitTransaction()
        {
            if (mTrans == null) throw new InvalidOperationException("Their is no pending transaction. Commit failed.");
            mTrans.Commit();
            mTrans.Dispose();
            mTrans = null;
        }

        /// <summary>
        /// Rollsback a database transaction.
        /// </summary>
        public void RollbackTransaction()
        {
            if (mTrans == null) throw new InvalidOperationException("Their is no pending transaction. Rollback failed.");
            mTrans.Rollback();
            mTrans.Dispose();
            mTrans = null;
        }

        #endregion

        #region Adapter Registration

        private static Dictionary<string, IDbAdapter> sAdapters = new Dictionary<string, IDbAdapter>();

        /// <summary>
        /// Register a named adapter. Allows a database to be created using Database.CreateDB(name).
        /// </summary>
        /// <param name="name"></param>
        /// <param name="adapter"></param>
        /// <remarks>Register adapters in the startup of your application so you don't need to pass connection strings around.</remarks>
        public static void RegisterAdapter(string name, IDbAdapter adapter)
        {
            if (name.IsEmpty()) throw new ArgumentNullException("name");

            if (adapter == null)
                sAdapters.Remove(name);
            else if (sAdapters.ContainsKey(name))
                sAdapters[name] = adapter;
            else
                sAdapters.Add(name, adapter);
        }

        /// <summary>
        /// Removes a registered adapter.
        /// </summary>
        /// <param name="name"></param>
        public static void RemoveAdapter(string name)
        {
            RegisterAdapter(name, null);
        }

        /// <summary>
        /// Gets a registered adapter by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IDbAdapter GetAdapter(string name)
        {
            if (name.IsEmpty()) throw new ArgumentNullException("name");
            if (sAdapters.ContainsKey(name))
                return sAdapters[name];
            else
                return null;
        }

        /// <summary>
        /// Creates a new instance of Database with the named adapter.
        /// </summary>
        /// <param name="adapterName"></param>
        /// <returns></returns>
        public static Database CreateDB(string adapterName)
        {
            if (adapterName.IsEmpty()) throw new ArgumentNullException("adapterName");
            var adapter = GetAdapter(adapterName);
            if (adapter == null) throw new ArgumentException("'{0}' is not a valid named adapter.".Fmt(adapterName), "adapterName");
            return new Database(adapter);
        }

        #endregion

    }

}
