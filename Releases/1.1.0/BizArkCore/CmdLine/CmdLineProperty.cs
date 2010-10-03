using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using BizArk.Core.AttributeExt;
using BizArk.Core.ArrayExt;
using System.Collections;

namespace BizArk.Core.CmdLine
{
    /// <summary>
    /// Represents a property that can be set via the command-line.
    /// </summary>
    public class CmdLineProperty
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of a CmdLineProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        public CmdLineProperty(CmdLineObject obj, PropertyDescriptor prop)
            : this(obj, prop, null)
        {
        }

        /// <summary>
        /// Creates an instance of a CmdLineProperty.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="claAtt"></param>
        internal CmdLineProperty(CmdLineObject obj, PropertyDescriptor prop, CmdLineArgAttribute claAtt)
        {
            Object = obj;
            mProperty = prop;
            DefaultValue = Value;
            if (prop.PropertyType == typeof(string))
                ShowDefaultValue = !string.IsNullOrEmpty((string)Value);
            else
                ShowDefaultValue = true;

            if (claAtt == null)
            {
                Required = false;
                Usage = "";
                ShowInUsage = DefaultBoolean.Default;
                AllowSave = true;
                Aliases = new string[] { };
            }
            else
            {
                Required = claAtt.Required;
                Usage = claAtt.Usage;
                ShowInUsage = claAtt.ShowInUsage;
                AllowSave = claAtt.AllowSave;
                Aliases = claAtt.Aliases;
            }
        }

        #endregion

        #region Fields and Properties

        private PropertyDescriptor mProperty;

        /// <summary>
        /// Gets the command-line object associated with this property.
        /// </summary>
        public CmdLineObject Object { get; private set; }

        /// <summary>
        /// The name of the command-line property.
        /// </summary>
        public string Name
        {
            get { return mProperty.Name; }
        }

        /// <summary>
        /// Gets the description associated with the property.
        /// </summary>
        public string Description
        {
            get { return mProperty.Description; }
        }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type PropertyType
        {
            get { return mProperty.PropertyType; }
        }

        /// <summary>
        /// Gets the aliases associated with this property.
        /// </summary>
        public string[] Aliases { get; private set; }

        /// <summary>
        /// Gets or sets a value that determines if this command-line argument is required.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets the short description that should be used in the usage description.
        /// </summary>
        public string Usage { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the argument should be displayed in the usage. By default, only required arguments and help are displayed in the usage in order to save space when printing the usage.
        /// </summary>
        public DefaultBoolean ShowInUsage { get; set; }

        /// <summary>
        /// Gets a value that determines if this property was set through the command-line or not.
        /// </summary>
        public bool PropertySet { get; private set; }

        /// <summary>
        /// Gets the default value for this property. Used in the command-line help description.
        /// </summary>
        public object DefaultValue { get; private set; }

        /// <summary>
        /// Gets or sets the current value for this property.
        /// </summary>
        public object Value
        {
            get
            {
                return mProperty.GetValue(Object);
            }
            set
            {
                var strs = value as string[];
                if (strs != null)
                {
                    if (mProperty.PropertyType.IsArray)
                        value = strs.Convert(PropertyType.GetElementType());
                    else
                        value = ConvertEx.ChangeType(strs[0], PropertyType);
                }
                mProperty.SetValue(Object, ConvertEx.ChangeType(value, PropertyType));
                PropertySet = true;
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if the default value should be displayed to the user in the usage.
        /// </summary>
        public bool ShowDefaultValue { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the property should be saved.
        /// </summary>
        public bool AllowSave { get; set; }

        #endregion

        #region Methods

        ///// <summary>
        ///// Sets the command-line property.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="values"></param>
        //internal void SetValue(CmdLineObject obj, params string[] values)
        //{
        //    object value;
        //    if (mProperty.PropertyType.IsArray)
        //        value = values.Convert(PropertyType.GetElementType());
        //    else
        //        value = ConvertEx.ChangeType(values[0], PropertyType);

        //    mProperty.SetValue(obj, value);
        //    PropertySet = true;
        //}

        ///// <summary>
        ///// Sets the command-line property.
        ///// </summary>
        ///// <param name="obj"></param>
        ///// <param name="value"></param>
        //public void SetValue(CmdLineObject obj, bool value)
        //{
        //    mProperty.SetValue(obj, value);
        //    PropertySet = true;
        //}

        #endregion

    }

    /// <summary>
    /// A list of CmdLineProperty objects.
    /// </summary>
    public class CmdLinePropertyList : IEnumerable<CmdLineProperty>
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of CmdLinePropertyList.
        /// </summary>
        public CmdLinePropertyList(CmdLineObject obj)
        {
            Object = obj;
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                var claAtt = prop.GetAttribute<CmdLineArgAttribute>();
                if (claAtt == null) continue;
                var cmdLineProp = new CmdLineProperty(obj, prop, claAtt);
                mProperties.Add(cmdLineProp);
                mPropertyDictionary.Add(prop.Name, cmdLineProp);
                foreach (var alias in cmdLineProp.Aliases)
                    mPropertyDictionary.Add(alias, cmdLineProp);
            }
        }

        #endregion

        #region Fields and Properties

        private List<CmdLineProperty> mProperties = new List<CmdLineProperty>();
        private Dictionary<string, CmdLineProperty> mPropertyDictionary = new Dictionary<string, CmdLineProperty>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the command-line property associated with this argument.
        /// </summary>
        /// <param name="argName">This can be the shortcut, full property name, or a partial property name that is unique.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the command-line property cannot be found.</exception>
        public CmdLineProperty this[string argName]
        {
            get
            {
                if (string.IsNullOrEmpty(argName)) return null;

                if (mPropertyDictionary.ContainsKey(argName))
                    return mPropertyDictionary[argName];

                // Search for a command-line property that starts with this name.
                List<CmdLineProperty> foundProps = new List<CmdLineProperty>();
                foreach (var prop in this)
                {
                    if (prop.Name.StartsWith(argName, StringComparison.InvariantCultureIgnoreCase))
                        foundProps.Add(prop);
                    foreach(var alias in prop.Aliases)
                        if (alias.StartsWith(argName, StringComparison.InvariantCultureIgnoreCase))
                            foundProps.Add(prop);
                }

                if (foundProps.Count == 0) return null;
                if (foundProps.Count == 1) return foundProps[0];
                // Multiple properties were found. We cannot process this argument.
                throw new AmbiguousCmdLineNameException(argName, foundProps.ToArray());
            }
        }

        /// <summary>
        /// Gets the number of properties in the list.
        /// </summary>
        public int Count { get { return mProperties.Count; } }

        /// <summary>
        /// Gets the command-line object for this list.
        /// </summary>
        public CmdLineObject Object { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the enumerator for the list.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CmdLineProperty> GetEnumerator()
        {
            return mProperties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

    }
}
