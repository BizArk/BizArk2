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
using System.Text.RegularExpressions;

namespace Redwerb.BizArk.Core.StringExt
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExt
    {
        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <returns></returns>
        public static string Wrap(this string str, int maxLength)
        {
            return Wrap(str, maxLength, "");
        }

        /// <summary>
        /// Forces the string to word wrap so that each line doesn't exceed the maxLineLength.
        /// </summary>
        /// <param name="str">The string to wrap.</param>
        /// <param name="maxLength">The maximum number of characters per line.</param>
        /// <param name="prefix">Adds this string to the beginning of each line.</param>
        /// <returns></returns>
        public static string Wrap(this string str, int maxLength, string prefix)
        {
            if (string.IsNullOrEmpty(str)) return "";
            if (maxLength <= 0) return prefix + str;

            var lines = new List<string>();

            // breaking the string into lines makes it easier to process.
            foreach (string line in str.Lines())
            {
                var remainingLine = line.Trim();
                do
                {
                    var newLine = GetLine(remainingLine, maxLength - prefix.Length);
                    lines.Add(newLine);
                    remainingLine = remainingLine.Substring(newLine.Length).Trim();
                    // Keep iterating as int as we've got words remaining 
                    // in the line.
                } while (remainingLine.Length > 0);
            }

            return string.Join(Environment.NewLine, lines.ToArray());
        }

        private static string GetLine(string str, int maxLength)
        {
            // The string is less than the max length so just return it.
            if (str.Length <= maxLength) return str;

            // Search backwords in the string for a whitespace char
            // starting with the char one after the maximum length
            // (if the next char is a whitespace, the last word fits).
            for (int i = maxLength; i >= 0; i--)
            {
                if (char.IsWhiteSpace(str[i]))
                    return str.Substring(0, i).TrimEnd();
            }

            // No whitespace chars, just break the word at the maxlength.
            return str.Substring(0, maxLength);
        }

        /// <summary>
        /// Splits the string into lines.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] Lines(this string str)
        {
            var lines = new List<string>();
            using (var sr = new System.IO.StringReader(str))
            {
                string line = sr.ReadLine();
                while (line != null)
                {
                    lines.Add(line);
                    line = sr.ReadLine();
                }
            }

            return lines.ToArray();
        }

        /// <summary>
        /// Splits the string into words (all white space is removed).
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] Words(this string str)
        {
            return str.Split(new char[] { }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Shortcut for ConvertEx.IsEmpty. Works because this is an extension method, not a real method.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string str)
        {
            return ConvertEx.IsEmpty(str);
        }

        /// <summary>
        /// Returns the string up to the maximum allowed.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Max(this string str, int length)
        {
            return str.Substring(0, length);
        }

        /// <summary>
        /// Returns an array split along the separator.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static T[] Split<T>(this string str, params char[] separator)
        {
            if (string.IsNullOrEmpty(str)) return new T[] { };
            var vals = str.Split(separator, StringSplitOptions.RemoveEmptyEntries);
            int i = -1;
            try
            {
                var retVals = new T[vals.Length];
                for (i = 0; i < vals.Length; i++)
                {
                    retVals[i] = ConvertEx.ChangeType<T>(vals[i].Trim());
                }
                return retVals;
            }
            catch (Exception ex)
            {
                if (i < 0) throw;
                throw new FormatException(string.Format("Cannot convert value '{0}'", vals[i]), ex);
            }
        }

    }
}
