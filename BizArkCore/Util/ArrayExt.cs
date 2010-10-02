﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.Core.ArrayExt
{
    /// <summary>
    /// Provides extension methods for string arrays.
    /// </summary>
    public static class ArrayExt
    {
        #region Shrink

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static Array Shrink(this Array arr, int startIndex, int endIndex)
        {
            if (arr == null) return null;
            if (startIndex >= arr.Length) return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (endIndex < startIndex) return Array.CreateInstance(arr.GetType().GetElementType(), 0);
            if (startIndex < 0) startIndex = 0;

            int length = (endIndex - startIndex) + 1;
            Array retArr = Array.CreateInstance(arr.GetType().GetElementType(), length);
            for (int i = startIndex; i <= endIndex; i++)
                retArr.SetValue(arr.GetValue(i), i - startIndex);

            return retArr;
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static string[] Shrink(this string[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as string[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex)
        {
            return Shrink((Array)arr, startIndex, arr.Length - 1) as int[];
        }

        /// <summary>
        /// Creates a new array with just the specified elements.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public static int[] Shrink(this int[] arr, int startIndex, int endIndex)
        {
            return Shrink((Array)arr, startIndex, endIndex) as int[];
        }

        #endregion

        #region Convert

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static T[] Convert<T>(this Array arr)
        {
            return (T[])Convert(arr, typeof(T));
        }

        /// <summary>
        /// Converts the array to a different type.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="elementType"></param>
        /// <returns></returns>
        public static Array Convert(this Array arr, Type elementType)
        {
            if (arr.GetType().GetElementType() == elementType)
                return arr.Copy();

            Array retArr = Array.CreateInstance(elementType, arr.Length);
            for (int i = 0; i < arr.Length; i++)
                retArr.SetValue(ConvertEx.ChangeType(arr.GetValue(i), elementType), i);
            return retArr;
        }

        #endregion

        #region RemoveEmpties

        /// <summary>
        /// Creates a new array that contains the non-empty elements of the given array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static string[] RemoveEmpties(this string[] arr)
        {
            return RemoveEmpties((Array)arr) as string[];
        }

        /// <summary>
        /// Creates a new array that contains the non-empty elements of the given array.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array RemoveEmpties(this Array arr)
        {
            var vals = new List<object>();
            for (int i = 0; i < arr.Length; i++)
            {
                var val = arr.GetValue(i);
                if (!ConvertEx.IsEmpty(val))
                    vals.Add(val);
            }

            Array retArr = Array.CreateInstance(arr.GetType().GetElementType(), vals.Count);
            for (int i = 0; i < vals.Count; i++)
                retArr.SetValue(vals[i], i);

            return retArr;
        }

        #endregion

        /// <summary>
        /// Searches for the specified object and returns the index of the first occurrence
        /// within the entire one-dimensional System.Array.
        /// </summary>
        /// <param name="arr">The one-dimensional System.Array to search.</param>
        /// <param name="val">The object to locate in array.</param>
        /// <returns>
        /// The index of the first occurrence of value within the entire array, if found;
        /// otherwise, the lower bound of the array minus 1.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">arr is null</exception>
        /// <exception cref="System.RankException">arr is multidimensional.</exception>
        public static int IndexOf(this Array arr, object val)
        {
            return Array.IndexOf(arr, val);
        }

        /// <summary>
        /// Determines if the array contains the given value.
        /// </summary>
        /// <param name="arr">The one-dimensional System.Array to search.</param>
        /// <param name="val">The object to locate in array.</param>
        /// <returns>
        /// </returns>
        /// <exception cref="System.ArgumentNullException">arr is null</exception>
        /// <exception cref="System.RankException">arr is multidimensional.</exception>
        public static bool Contains(this Array arr, object val)
        {
            if (Array.IndexOf(arr, val) < arr.GetLowerBound(0))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Copies the array to a new array of the same type.
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public static Array Copy(this Array arr)
        {
            var newArr = Array.CreateInstance(arr.GetType().GetElementType(), arr.Length);
            arr.CopyTo(newArr, 0);
            return newArr;
        }
    }
}