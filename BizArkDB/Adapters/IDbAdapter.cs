using System;
using System.Data.Common;
using BizArk.Core.Util;

namespace BizArk.DB.Adapters
{

    /// <summary>
    /// This interface is used to initialize a Database instance with a specific provider.
    /// </summary>
    public interface IDbAdapter
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

        /// <summary>
        /// Gets the current DateTime from the server.
        /// </summary>
        /// <param name="db">The Database instance that is calling this method.</param>
        /// <remarks>This should go to the database every time. BizArk will handle performance issues.</remarks>
        DateTime NowUtc(Database db);

        /// <summary>
        /// Creates a command that will add the record to the database. If the table includes an auto-incrementing identity, the value should be returned as a scalar value (single row, single field).
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table where to put the new record.</param>
        /// <param name="data">The data to insert into the table.</param>
        /// <param name="identityFieldName">The name of the identity field. Leave null if the record doesn't have an identity.</param>
        /// <returns>If identityFieldName contains a value, returns the identity of the new record. Otherwise null.</returns>
        int? Insert(Database db, string tableName, NameValue[] data, string identityFieldName = null);

        /// <summary>
        /// Updates the record with the given key in the database.
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table that has the records we are updating.</param>
        /// <param name="key">The key to the record to update.</param>
        /// <param name="data">The data to update.</param>
        /// <returns>The number of records effected.</returns>
        int Update(Database db, string tableName, NameValue[] key, NameValue[] data);

        /// <summary>
        /// Creates a command that will removes the record with the given key from the database.
        /// </summary>
        /// <param name="db">The database that is making the request.</param>
        /// <param name="tableName">The name of the table that has the records we are deleting.</param>
        /// <param name="key">The key to the record to delete.</param>
        /// <returns>The number of records effected.</returns>
        int Delete(Database db, string tableName, NameValue[] key);

    }

}
