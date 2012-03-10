using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizArk.Core.FormatExt;
using BizArk.Core.ArrayExt;
using System.Collections;

namespace BizArk.Core.CmdLine
{

    /// <summary>
    /// Interface for custom property validators for the BizArk command-line parser.
    /// </summary>
    public interface ICustomValidator
    {

        /// <summary>
        /// Gets or sets the error message. Include a {0} where you want the name of the property.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the formatted error message.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns></returns>
        string FormatErrorMessage(string name);

        /// <summary>
        /// Checks to make sure the value is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        bool IsValid(object value);

    }

    /// <summary>
    /// Base class for custom property validators for the BizArk command-line parser.
    /// </summary>
    public abstract class CustomValidator : ICustomValidator
    {

        /// <summary>
        /// Gets or sets the error message. Include a {0} where you want the name of the property.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets the formatted error message.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <returns></returns>
        public virtual string FormatErrorMessage(string name)
        {
            return ErrorMessage.Fmt(name);
        }

        /// <summary>
        /// Checks to make sure the value is correct.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool IsValid(object value);

    }

    /// <summary>
    /// Ensures the value is contained within the set of values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SetValidator<T> : CustomValidator
    {
        
        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of SetValidator.
        /// </summary>
        /// <param name="values"></param>
        public SetValidator(params T[] values)
            : this(null, values)
        {
        }

        /// <summary>
        /// Creates an instance of SetValidator.
        /// </summary>
        /// <param name="comparer">The comparer to use to compare the values.</param>
        /// <param name="values"></param>
        public SetValidator(IEqualityComparer comparer, params T[] values)
        {
            Comparer = comparer;
            ErrorMessage = "The field {0} must be one of these values: {1}.";
            Values = values;
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
        public T[] Values { get; private set; }

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
            return ErrorMessage.Fmt(name, displayVals.Join(", "));
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
