using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using BizArk.Core.ArrayExt;
using BizArk.Core.AttributeExt;
using BizArk.Core.ExceptionExt;
using BizArk.Core.FormatExt;
using BizArk.Core.StringExt;
using BizArk.Core.TypeExt;

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
            Validators = new List<ValidationAttribute>();
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

            var reqAtt = prop.GetAttribute<RequiredAttribute>();
            if (reqAtt != null) Required = true;
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
                    {
                        try
                        {
                            value = strs.Convert(PropertyType.GetElementType());
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetDetails());
                            Error = "[{0}] is not valid. The value must be able to convert to an array of {2}.".Fmt(strs.Join(", "), Name, PropertyType.GetElementType().GetCSharpName());
                            return;
                        }
                    }
                    else
                    {
                        try
                        {
                            value = ConvertEx.ChangeType(strs[0], PropertyType);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.GetDetails());
                            if (PropertyType.IsEnum)
                            {
                                Error = "'{0}' is not valid. The argument must be one of these values: [{2}].".Fmt(strs[0], Name, Enum.GetValues(PropertyType).Join(", "));
                            }
                            else
                            {
                                var typeName = PropertyType.GetCSharpName();
                                Error = "'{0}' is not valid. The argument must be a{2} {3}.".Fmt(strs[0], Name, typeName[0].IsVowel() ? "n" : "", typeName);
                            }
                            return;
                        }
                    }
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

        /// <summary>
        /// Gets any errors associated with this property.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// Additional custom validators. Useful for adding validation outside of attributes.
        /// </summary>
        public IList<ValidationAttribute> Validators { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the textual representation of this command-line object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var value = new StringBuilder();
            var vals = Value as IEnumerable;
            if (vals != null)
            {
                var first = true;
                foreach (var val in vals)
                {
                    if (!first) value.Append(", ");
                    first = false;
                    value.Append(ConvertEx.ToString(val));
                }
            }
            else
                value.Append(ConvertEx.ToString(Value));
            return "{0}=[{1}]".Fmt(Name, value);
        }

        internal ValidationAttribute[] GetAllValidators()
        {
            var validators = new List<ValidationAttribute>();

            validators.AddRange(mProperty.GetAttributes<ValidationAttribute>());
            validators.AddRange(Validators);

            return validators.ToArray();
        }

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
            mCompare = obj.Options.Comparer;
            mPropertyDictionary = CreateDictionary(obj.Options.Comparer);
            Object = obj;
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(obj))
            {
                var claAtt = prop.GetAttribute<CmdLineArgAttribute>();
                if (claAtt == null) continue;
                var cmdLineProp = new CmdLineProperty(obj, prop, claAtt);
                mProperties.Add(cmdLineProp);
                Add(prop.Name, cmdLineProp);
                foreach (var alias in cmdLineProp.Aliases)
                    Add(alias, cmdLineProp);
            }
        }

        private Dictionary<string, CmdLineProperty> CreateDictionary(StringComparison compare)
        {
            switch (compare)
            {
                case StringComparison.CurrentCulture:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.CurrentCulture);
                case StringComparison.CurrentCultureIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.CurrentCultureIgnoreCase);
                case StringComparison.InvariantCulture:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.InvariantCulture);
                case StringComparison.InvariantCultureIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.InvariantCultureIgnoreCase);
                case StringComparison.Ordinal:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.Ordinal);
                case StringComparison.OrdinalIgnoreCase:
                    return new Dictionary<string, CmdLineProperty>(StringComparer.OrdinalIgnoreCase);
                default:
                    throw new ArgumentException("The comparison type '{0}' is not supported.".Fmt(compare));
            }
        }

        #endregion

        #region Fields and Properties

        private StringComparison mCompare;
        private List<CmdLineProperty> mProperties = new List<CmdLineProperty>();
        private Dictionary<string, CmdLineProperty> mPropertyDictionary;

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

                // Honor exact matches.
                if (mPropertyDictionary.ContainsKey(argName))
                    return mPropertyDictionary[argName];

                // Search for a command-line property that starts with this name.
                var foundProps = new List<CmdLineProperty>();
                foreach (var prop in this)
                {
                    if (prop.Name.StartsWith(argName, mCompare))
                        if (!foundProps.Contains(prop)) foundProps.Add(prop);
                    foreach (var alias in prop.Aliases)
                        if (alias.StartsWith(argName, mCompare))
                            if (!foundProps.Contains(prop)) foundProps.Add(prop);
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
        /// Adds a command-line property to the list keyed to the given name.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="prop"></param>
        public void Add(string name, CmdLineProperty prop)
        {
            if (mPropertyDictionary.ContainsKey(name))
                throw new CmdLineArgumentException("The command-line name '{0}' is already defined.".Fmt(name));
            mPropertyDictionary.Add(name, prop);
        }

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
