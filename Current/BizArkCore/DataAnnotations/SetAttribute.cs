﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using BizArk.Core.StringExt;
using BizArk.Core.FormatExt;
using BizArk.Core.ArrayExt;
using System.Collections;

namespace BizArk.Core.DataAnnotations
{

    /// <summary>
    /// Ensures the value is contained within the set of values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SetAttribute : DataTypeAttribute
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SetAttribute.
        /// </summary>
        /// <param name="values"></param>
        public SetAttribute(params object[] values)
            : this(null, values)
        {
        }

        /// <summary>
        /// Creates an instance of SetAttribute.
        /// </summary>
        /// <param name="comparer">The comparer to use to compare the values.</param>
        /// <param name="values"></param>
        public SetAttribute(IEqualityComparer comparer, params object[] values)
            : base("set")
        {
            Comparer = comparer;
            ErrorMessage = "The field {0} must be one of these values: {1}.";
            Values = values;
        }

        /// <summary>
        /// Creates an instance of SetAttribute.
        /// </summary>
        /// <param name="ignoreCase"></param>
        /// <param name="values"></param>
        public SetAttribute(bool ignoreCase, params string[] values)
            : this(ignoreCase ? StringComparer.InvariantCultureIgnoreCase : null, values)
        {
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the comparer to use to compare the values. If not set, uses Values[x].Equals(value).
        /// </summary>
        public IEqualityComparer Comparer { get; private set; }

        /// <summary>
        /// Gets the set of valid values.
        /// </summary>
        public object[] Values { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Applies formatting to an error message, based on the data field where the error occurred.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override string FormatErrorMessage(string name)
        {
            var displayVals = Values.Convert(typeof(string));
            return ErrorMessageString.Fmt(name, displayVals.Join(", "));
        }

        /// <summary>
        /// Checks that the value of the data field is valid.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool IsValid(object value)
        {
            if (value == null) return true; // Use RequiredAttribute to validate for a value.

            foreach (var validVal in Values)
            {
                if (Comparer != null)
                {
                    if (Comparer.Equals(validVal, value))
                        return true;
                }
                else if (validVal.Equals(value)) 
                    return true;
            }
            return false;
        }

        #endregion

    }

}
