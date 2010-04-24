using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core.TypeExt
{
    /// <summary>
    /// Provides extension methods for Type.
    /// </summary>
    public static class TypeExt
    {
        /// <summary>
        /// Determines if the type implements the given interface.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="interfaceType"></param>
        /// <returns></returns>
        public static bool Implements(this Type type, Type interfaceType)
        {
            foreach (Type i in type.GetInterfaces())
                if (i == interfaceType) return true;
            return false;
        }

        /// <summary>
        /// Determines if the type is derived from the given base type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsDerivedFrom(this Type type, Type baseType)
        {
             return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object Instantiate(this Type type, params object[] args)
        {
            return ClassFactory.CreateObject(type, args);
        }

        public static bool IsNullable(this Type type)
        {
            if (type == null) return false;
            if (!type.IsGenericType) return false;
            if (type.GetGenericTypeDefinition() != typeof(Nullable<>)) return false;
            return true;
        }

    }
}
