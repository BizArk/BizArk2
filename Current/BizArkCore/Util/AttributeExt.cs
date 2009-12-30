﻿/* Author: Brian Brewder
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
using System.ComponentModel;
using System.Reflection;

namespace Redwerb.BizArk.Core.AttributeExt
{
    /// <summary>
    /// Provides extension methods for PropertyDescriptor.
    /// </summary>
    public static class AttributeExt
    {
        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this PropertyDescriptor prop) where T : Attribute
        {
            foreach (Attribute att in prop.Attributes)
            {
                var tAtt = att as T;
                if (tAtt != null) return tAtt;
            }
            return null;
        }

        /// <summary>
        /// Gets the specified attribute from the type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Type type, bool inherit) where T : Attribute
        {
            var atts = type.GetCustomAttributes(typeof(T), inherit);
            if (atts.Length == 0) return null;
            return atts[0] as T;
        }

        /// <summary>
        /// Gets the specified attribute for the assembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="asm"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Assembly asm) where T : Attribute
        {
            if (asm == null) return null;

            var atts = asm.GetCustomAttributes(typeof(T), false);
            if (atts == null) return null;
            if (atts.Length == 0) return null;

            return (T)atts[0];
        }

        /// <summary>
        /// Gets the specified attribute from the PropertyDescriptor.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this object obj, bool inherit) where T : Attribute
        {
            if (obj == null) return null;
            return obj.GetType().GetAttribute<T>(inherit);
        }

        /// <summary>
        /// Gets the specified attribute for the assembly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <returns></returns>
        public static T GetAttribute<T>(this Enum val) where T : Attribute
        {
            var fi = val.GetType().GetField(val.ToString());
            var atts = fi.GetCustomAttributes(typeof(T), false);
            if (atts.Length == 0) return null;
            return (T)atts[0];
        }

        /// <summary>
        /// Gets the description for the enumerated value.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum e)
        {
            var att = GetAttribute<DescriptionAttribute>(e);
            if (att == null) return "";
            return att.Description;
        }

    }
}
