using System;
using System.ComponentModel;
using System.Linq.Expressions;
using BizArk.Core.Util;

namespace BizArk.Core.ObjectExt
{
    /// <summary>
    /// Extends the Object class.
    /// </summary>
    public static class ObjectExt
    {
        /// <summary>
        /// Converts the value to the specified type. 
        /// Checks for a TypeConverter, conversion methods, 
        /// and the IConvertible interface. Uses <see cref="BizArk.Core.ConvertEx.ChangeType(object, Type, IFormatProvider)"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="obj">The value to convert from.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">This conversion is not supported. -or-value is null and conversionType is a value type.</exception>
        /// <exception cref="System.ArgumentNullException">conversionType is null.</exception>
        public static T Convert<T>(this object obj)
        {
            return ConvertEx.ChangeType<T>(obj);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static object GetValue(this object obj, string propertyName)
        {
            var prop = TypeDescriptor.GetProperties(obj).Find(propertyName, false);
            if (prop == null)
                throw new ArgumentException(propertyName + " is not a valid property on " + obj.GetType().FullName);
            return prop.GetValue(obj);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object obj, string propertyName)
        {
            return (T)GetValue(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static int GetInt(this object obj, string propertyName)
        {
            return GetValue<int>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static int Getint(this object obj, string propertyName)
        {
            return GetValue<int>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static string GetString(this object obj, string propertyName)
        {
            return GetValue<string>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static bool GetBoolean(this object obj, string propertyName)
        {
            return GetValue<bool>(obj, propertyName);
        }

        /// <summary>
        /// Gets the value for the given property name. This works for any object that uses CustomTypeDescriptor.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static decimal GetDecimal(this object obj, string propertyName)
        {
            return GetValue<decimal>(obj, propertyName);
        }

        /// <summary>
        /// Gets the name of the property based on a Linq expression.
        /// </summary>
        /// <typeparam name="TObject"></typeparam>
        /// <param name="type"></param>
        /// <param name="propertyRefExpr"></param>
        /// <returns></returns>
        public static string GetPropertyName<TObject>(this TObject type, Expression<Func<TObject, object>> propertyRefExpr)
        {
            return PropertyUtil.GetNameCore(propertyRefExpr.Body);
        }

    }
}
