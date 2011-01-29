﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.TypeExt
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
        /// Determines if the type is an instance of a generic type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsDerivedFromGenericType(this Type type, Type genericType)
        {
            var typeTmp = type;
            while (typeTmp != null)
            {
                if (typeTmp.IsGenericType && typeTmp.GetGenericTypeDefinition() == genericType)
                    return true;

                typeTmp = typeTmp.BaseType;
            }
            return false;
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

        /// <summary>
        /// Gets a value that determines if the type allows instances with a null value.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            if (type == null) return false;
            if (!type.IsValueType) return true;
            return type.IsDerivedFromGenericType(typeof(Nullable<>));
        }

        /// <summary>
        /// Gets the C# name of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetCSharpName(this Type type)
        {
            var name = type.Name;

            if (type == typeof(bool)) name = "bool";
            else if (type == typeof(byte)) name = "byte";
            else if (type == typeof(sbyte)) name = "sbyte";
            else if (type == typeof(char)) name = "char";
            else if (type == typeof(short)) name = "short";
            else if (type == typeof(ushort)) name = "ushort";
            else if (type == typeof(int)) name = "int";
            else if (type == typeof(uint)) name = "uint";
            else if (type == typeof(long)) name = "long";
            else if (type == typeof(ulong)) name = "ulong";
            else if (type == typeof(float)) name = "float";
            else if (type == typeof(double)) name = "double";
            else if (type == typeof(decimal)) name = "decimal";
            else if (type == typeof(string)) name = "string";

            if (type.IsValueType && type.IsNullable()) name += "?";

            return name;
        }

    }
}
