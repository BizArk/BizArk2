using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Redwerb.BizArk.Core.Convert
{

    /// <summary>
    /// Converts a .Net type to a SqlDBType.
    /// </summary>
    public class TypeToSqlDBTypeConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return DbTypeMap.ToSqlDbType((Type)value);
        }

    }

    /// <summary>
    /// Converts a SqlDbType to a .Net type.
    /// </summary>
    public class SqlDBTypeToTypeConversionStrategy
        : IConvertStrategy
    {

        /// <summary>
        /// Converts the value to the proper type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public object Convert(object value, IFormatProvider provider)
        {
            return DbTypeMap.ToNetType((SqlDbType)value);
        }

    }

    /// <summary>
    /// Map between different datatypes.
    /// </summary>
    public static class DbTypeMap
    {

        #region Initialization and Destruction

        static DbTypeMap()
        {
            sMap = new List<DbTypeMapEntry>();

            sMap.Add(new DbTypeMapEntry(typeof(bool), DbType.Boolean, SqlDbType.Bit));
            sMap.Add(new DbTypeMapEntry(typeof(byte), DbType.Double, SqlDbType.TinyInt));
            sMap.Add(new DbTypeMapEntry(typeof(byte[]), DbType.Binary, SqlDbType.Binary));
            sMap.Add(new DbTypeMapEntry(typeof(DateTime), DbType.DateTime, SqlDbType.DateTime));
            sMap.Add(new DbTypeMapEntry(typeof(Decimal), DbType.Decimal, SqlDbType.Decimal));
            sMap.Add(new DbTypeMapEntry(typeof(double), DbType.Double, SqlDbType.Float));
            sMap.Add(new DbTypeMapEntry(typeof(Guid), DbType.Guid, SqlDbType.UniqueIdentifier));
            sMap.Add(new DbTypeMapEntry(typeof(Int16), DbType.Int16, SqlDbType.SmallInt));
            sMap.Add(new DbTypeMapEntry(typeof(Int32), DbType.Int32, SqlDbType.Int));
            sMap.Add(new DbTypeMapEntry(typeof(Int64), DbType.Int64, SqlDbType.BigInt));
            sMap.Add(new DbTypeMapEntry(typeof(object), DbType.Object, SqlDbType.Variant));
            sMap.Add(new DbTypeMapEntry(typeof(string), DbType.String, SqlDbType.VarChar));
        }

        #endregion

        #region Fields and Properties

        private static List<DbTypeMapEntry> sMap;

        #endregion

        #region Methods

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("SqlDbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type ToNetType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("DbType.{0} cannot be converted to a .Net type.", type.ToString()));
            return entry.Type;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(SqlDbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static DbType ToDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a DbType.", type.ToString()));
            return entry.DbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(DbType type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        /// <summary>
        /// Converts the value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SqlDbType ToSqlDbType(Type type)
        {
            var entry = Find(type);
            if (entry == null)
                throw new InvalidCastException(string.Format("{0} cannot be converted to a SqlDbType.", type.ToString()));
            return entry.SqlDbType;
        }

        private static DbTypeMapEntry Find(Type type)
        {
            return sMap.Find((e) => { return e.Type == type; });
        }

        private static DbTypeMapEntry Find(DbType type)
        {
            return sMap.Find((e) => { return e.DbType == type; });
        }

        private static DbTypeMapEntry Find(SqlDbType type)
        {
            return sMap.Find((e) => { return e.SqlDbType == type; });
        }

        #endregion

    }

    /// <summary>
    /// Represents a map entry for conversion.
    /// </summary>
    public class DbTypeMapEntry
    {

        /// <summary>
        /// Creates an instance of DbTypeMapEntry.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="dbType"></param>
        /// <param name="sqlDbType"></param>
        public DbTypeMapEntry(Type type, DbType dbType, SqlDbType sqlDbType)
        {
            this.Type = type;
            this.DbType = dbType;
            this.SqlDbType = sqlDbType;
        }

        /// <summary>
        /// Gets the .Net type.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Gets the DbType.
        /// </summary>
        public DbType DbType { get; private set; }

        /// <summary>
        /// Gets the SqlDbType.
        /// </summary>
        public SqlDbType SqlDbType { get; private set; }

    }

}
