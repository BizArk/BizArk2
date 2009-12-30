/* Author: Brian Brewder
 * Website: http://redwerb.com
 * 
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. 
 */

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

    }
}
