using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core
{
    /// <summary>
    /// The class factory for objects allows for objects to be changed at runtime.
    /// </summary>
    public static class ClassFactory
    {

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T CreateObject<T>(params object[] args)
        {
            return (T)CreateObject(typeof(T), args);
        }

        /// <summary>
        /// Creates an object of the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static object CreateObject(Type type, params object[] args)
        {
            if (sTypeReplacement.ContainsKey(type))
                return Activator.CreateInstance(sTypeReplacement[type], args);
            else
                return Activator.CreateInstance(type, args);
        }

        private static Dictionary<Type, Type> sTypeReplacement = new Dictionary<Type, Type>();

        /// <summary>
        /// Registers a replacement type. To clear a replacement, send in null.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="replacementType"></param>
        public static void RegisterTypeReplacement(Type type, Type replacementType)
        {
            if (sTypeReplacement.ContainsKey(type))
            {
                if (replacementType == null)
                    sTypeReplacement.Remove(type);
                else
                    sTypeReplacement[type] = replacementType;
            }
            else if (replacementType != null)
                sTypeReplacement.Add(type, replacementType);
        }

    }
}
