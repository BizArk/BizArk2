using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using BizArk.Core.AttributeExt;
using BizArk.Core.ArrayExt;
using BizArk.Core.StringExt;
using System.Xml;
using io = System.IO;
using BizArk.Core.Web;

namespace BizArk.Core.CmdLine
{
    /// <summary>
    /// Represents an object that can be initialized via command-line arguments.
    /// </summary>
    /// <remarks>
    /// <para>The CmdLineObject class can be inherited from to allow the 
    /// properties of a class to be initialized from command-line arguments.
    /// The properties can be any type that can be converted to and from a string 
    /// using the <see cref="BizArk.Core.ConvertEx.ChangeType(object, Type)"/> 
    /// method.</para>
    /// <para>Only properties that have the CmdLineArgAttribute applied to them
    /// can be initialized from the command-line. To make a property the default
    /// property, apply the CmdLineDefaultArgAttribute to the class and specify
    /// the name of the property.</para>
    /// <para>This class will automatically produce command-line help to let
    /// the user know what arguments are available from the command-line and
    /// how to use them. If you want to customize the usage text, override the
    /// GetUsage method. If you want to customize the title of the application,
    /// override the GetTitle method. The usage for properties can be set in the
    /// CmdLineArgAttribute and the description can be set by applying the
    /// System.ComponentModel.DescriptionAttribute to the property.</para>
    /// </remarks>
    public abstract class CmdLineObject
    {

        #region Initialization and Destruction

        /// <summary>
        /// Instantiates CmdLineObject.
        /// </summary>
        public CmdLineObject()
        {
            ArgumentPrefix = "/";
            Caption = "Command-line Help";
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets a value that determines if help should be displayed.
        /// </summary>
        [CmdLineArg(Alias = "?", ShowInUsage = DefaultBoolean.True)]
        [Description("Displays command-line usage information.")]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool Help { get; set; }

        /// <summary>
        /// Gets or sets the caption associated with these command-line arguments. Used in the command-line help dialog.
        /// </summary>
        [Browsable(false)]
        public string Caption { get; set; }

        /// <summary>
        /// Gets the list of command-line properties.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public CmdLinePropertyList Properties { get; private set; }

        /// <summary>
        /// Gets or sets the default property for the command-line.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public CmdLineProperty DefaultProperty { get; set; }

        /// <summary>
        /// Gets or sets the string used to identify argument names.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        protected string ArgumentPrefix { get; set; }

        private List<string> mErrors;
        /// <summary>
        /// Gets the error text for the command-line object.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public string ErrorText
        {
            get
            {
                return string.Join(Environment.NewLine, mErrors.ToArray());
            }
        }

        /// <summary>
        /// Makes sure the command-line object is valid. 
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Browsable(false)]
        public bool IsValid()
        {
            if (Properties == null) throw new InvalidOperationException("The command-line object has not been initialized yet.");
            mErrors.Clear();

            mErrors.AddRange(Validate());

            if (mErrors.Count == 0)
                return true;
            else
                return false;
        }

        private bool mIsInitialized = false;
        /// <summary>
        /// Gets a value that determines if the CmdLineObject is ready to use.
        /// </summary>
        public bool IsInitialized
        {
            get { return mIsInitialized; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the command-line object. 
        /// </summary>
        private void Initialize_Internal()
        {
            mErrors = new List<string>();

            if (Properties == null)
            {
                // Get the list of properties that can be set from the command-line.
                // We must perform this step after construction which is why Initialize
                // is a separate step.
                Properties = new CmdLinePropertyList(this);
            }

            if (DefaultProperty == null)
            {
                var att = this.GetType().GetAttribute<CmdLineDefaultArgAttribute>(false);
                if (att != null)
                    DefaultProperty = Properties[att.DefaultArgName];
            }

        }

        /// <summary>
        /// Initializes the CmdLineObject.
        /// </summary>
        public void Initialize()
        {
            if (Application.ClickOnceDeployed)
            {
                var url = Application.ClickOnceUrl;
                if (string.IsNullOrEmpty(url))
                    InitializeEmpty();
                else
                {
                    var uri = new Uri(url);
                    var queryStr = uri.Query;
                    InitializeFromQueryString(queryStr);
                }
            }
            else
            {
                var args = Environment.GetCommandLineArgs();
                args = args.Shrink(1);
                InitializeFromCmdLine(args);
            }
        }

        /// <summary>
        /// Initializes the command-line object, but does not populate it.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void InitializeEmpty()
        {
            BeginInit();
            Initialize_Internal();
            EndInit();
        }

        /// <summary>
        /// Initializes the command-line args based on a query string. Used for 
        /// </summary>
        /// <param name="queryStr"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void InitializeFromQueryString(string queryStr)
        {
            BeginInit();
            Initialize_Internal();
            if (Properties.Count == 0) return;

            // the query string is empty so just return.
            if (string.IsNullOrEmpty(queryStr)) return;

            var ps = new UrlParamList(queryStr);
            foreach (var p in ps)
            {
                var prop = Properties[p.Name];
                if (prop == null) continue;
                prop.Value = p.Value;
            }
            EndInit();
        }

        /// <summary>
        /// Initializes the object with the given arguments.
        /// </summary>
        /// <param name="args">The command-line args. Make sure to shrink the array if the first element contains the path to the application (as in Environment.GetCommandLineArgs()) or the default parameter won't get set correctly.</param>
        /// <example>
        /// using BizArk.Core.ArrayExt;
        /// var args = Environment.GetCommandLineArgs().Shrink(1);
        /// </example>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void InitializeFromCmdLine(params string[] args)
        {
            BeginInit();
            Initialize_Internal();
            if (Properties.Count == 0) return;

            // There aren't any args so just return.
            if (args.Length == 0) return;

            int startIndex = 0;
            if (DefaultProperty != null && !args[0].StartsWith(ArgumentPrefix))
            {
                var argValues = GetArgValues(args);
                DefaultProperty.Value = argValues;
                startIndex = argValues.Length; // We already took care of the first set of arguments so start with the next.
            }

            for (int i = startIndex; i < args.Length; i++)
            {
                if (!args[i].StartsWith(ArgumentPrefix)) continue;

                string argName = args[i].Substring(1);
                if (argName == "") continue;

                var prop = Properties[argName];
                if (prop == null) continue;

                string[] argValues;
                argValues = GetArgValues(args.Shrink(i + 1));
                i = i + argValues.Length; // Move to the next valid argument.

                if (prop.PropertyType == typeof(bool))
                {
                    // Boolean arguments don't require an argument value (/A and /A- can be used for true/false).
                    if (argName.EndsWith("-"))
                        prop.Value = false;
                    else
                    {
                        bool bVal = true;

                        if (argValues.Length > 0)
                        {
                            try
                            {
                                bVal = ConvertEx.ToBoolean(argValues[0]);
                            }
                            catch (Exception)
                            {
                                // The command-line argument value couldn't be converted to a 
                                // bool so assume it wasn't intended to be.
                            }
                        }

                        prop.Value = bVal;
                    }
                }
                else if (argValues.Length > 0)
                {
                    prop.Value = argValues;
                }
                else
                {
                    // No value was set so leave the value.
                }

            }
            EndInit();
        }

        private string[] GetArgValues(string[] args)
        {
            List<string> argValues = new List<string>();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith(ArgumentPrefix)) return argValues.ToArray();
                argValues.Add(args[i]);
            }

            return argValues.ToArray();
        }

        private void SetValue(PropertyDescriptor prop, string argName, params string[] argValues)
        {
        }

        /// <summary>
        /// This method is intended to be overwritten in order to perform cmd-line validation.
        /// </summary>
        /// <returns></returns>
        protected virtual string[] Validate()
        {
            var errors = new List<string>();
            foreach (CmdLineProperty prop in Properties)
            {
                if (prop.Required && !prop.PropertySet)
                    errors.Add(string.Format("{0} is required.", prop.Name));
            }

            return errors.ToArray();
        }

        /// <summary>
        /// Gets the usage for the given property. If prop is null, gets the usage for the application.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual string GetUsage()
        {
            var usage = new StringBuilder();
            usage.Append(Application.ExeName);

            if (DefaultProperty != null)
                usage.Append(" <" + (string.IsNullOrEmpty(DefaultProperty.Usage) ? DefaultProperty.Name : DefaultProperty.Usage) + ">");

            // Display all the required properties first.
            foreach (CmdLineProperty prop in Properties)
            {
                if (prop.Required && prop.ShowInUsage != DefaultBoolean.False)
                    usage.Append(" " + GetPropertyUsage(prop));
            }

            // Display all the non-required properties after the required properties.
            foreach (CmdLineProperty prop in Properties)
            {
                if (!prop.Required && prop.ShowInUsage == DefaultBoolean.True)
                    usage.Append(" [" + GetPropertyUsage(prop) + "]");
            }

            return usage.ToString();
        }

        private string GetPropertyUsage(CmdLineProperty prop)
        {
            object id;
            if (prop.Aliases.Length == 0)
                id = prop.Name;
            else
                id = prop.Aliases[0];

            if (!string.IsNullOrEmpty(prop.Usage))
                return string.Format("{0}{1}=<{2}>", ArgumentPrefix, id, prop.Usage);

            if (prop.PropertyType == typeof(bool))
                return string.Format("{0}{1}[-]", ArgumentPrefix, id, prop.Name);
            else
                return string.Format("{0}{1}=<{2}>", ArgumentPrefix, id, prop.Name);
        }

        /// <summary>
        /// Gets the title associated with this command-line.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual string GetTitle()
        {
            var title = Application.Title;
            var version = Application.Version;
            var copyright = Application.Copyright;

            return string.Format("{0}, {1} - {2}", title, version, copyright);
        }

        /// <summary>
        /// Gets the full description for the command-line arguments.
        /// </summary>
        /// <returns></returns>
        public string GetHelpText(int maxWidth)
        {
            if (Properties == null) throw new InvalidOperationException("The command-line object has not been initialized yet.");

            var desc = new StringBuilder();

            if (mErrors != null && mErrors.Count > 0)
            {
                desc.AppendLine(("ERROR: " + mErrors[0]).Wrap(maxWidth));
                for (int i = 1; i < mErrors.Count; i++)
                    desc.AppendLine(mErrors[i].Wrap(maxWidth, "    "));
                desc.AppendLine();
            }

            desc.AppendLine(GetTitle());
            desc.AppendLine(GetAppDescription().Wrap(maxWidth));
            desc.AppendLine("Usage: " + GetUsage());
            desc.AppendLine();

            var maxNameWidth = GetMaxNameLength() + 6; // Add extra for ' (S): '
            var maxDescWidth = maxWidth - maxNameWidth;

            foreach (CmdLineProperty prop in Properties)
            {
                var propName = new StringBuilder();
                propName.Append(prop.Name);
                if (prop.Aliases.Length > 0)
                {
                    propName.Append(" (");
                    propName.Append(string.Join(", ", prop.Aliases));
                    propName.Append(")");
                }
                propName.Append(": ");
                desc.Append(propName.ToString().PadRight(maxNameWidth));
                var propDesc = prop.Description.Wrap(maxDescWidth);
                string[] lines = propDesc.Lines();
                if (lines.Length > 0)
                {
                    desc.AppendLine(lines[0]);
                    for (int i = 1; i < lines.Length; i++)
                        desc.AppendLine(new string(' ', maxNameWidth) + lines[i]);
                }
                else
                {
                    // If there isn't a description, we need to add a line break.
                    desc.AppendLine();
                }
                if (prop.Required)
                    desc.AppendLine(new string(' ', maxNameWidth) + "REQUIRED");
                else if (prop.ShowDefaultValue)
                    desc.AppendLine(new string(' ', maxNameWidth) + string.Format("Default Value: {0}", prop.DefaultValue));

            }

            return desc.ToString();
        }

        /// <summary>
        /// Override to provide a description of the application in the help text. The preferred method of providing a description is applying the DescriptionAttribute to the class.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetAppDescription()
        {
            var att = this.GetAttribute<DescriptionAttribute>(true);
            if (att == null) return "";
            return att.Description;
        }

        private int GetMaxNameLength()
        {
            int max = 0;
            foreach (CmdLineProperty prop in Properties)
                max = Math.Max(max, prop.Name.Length);
            return max;
        }

        /// <summary>
        /// Saves the settings to an xml file.
        /// </summary>
        /// <param name="path"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void SaveToXml(string path)
        {
            if (Properties == null)
                throw new InvalidOperationException("The command-line object has not been initialized.");

            var xml = new XmlDocument();
            xml.LoadXml("<CmdLineObject></CmdLineObject>");

            var typeNode = xml.CreateElement("Type");
            typeNode.InnerText = this.GetType().FullName;
            xml.DocumentElement.AppendChild(typeNode);

            var propsNode = xml.CreateElement("Properties");
            xml.DocumentElement.AppendChild(propsNode);

            foreach (var prop in Properties)
            {
                if (!prop.AllowSave) continue;

                var propNode = xml.CreateElement(prop.Name);

                if (prop.PropertyType.IsArray)
                {
                    var arr = prop.Value as Array;
                    if (arr != null)
                    {
                        foreach (var elementVal in arr)
                        {
                            var elementNode = xml.CreateElement("Element");
                            elementNode.InnerText = ConvertEx.ToString(elementVal);
                            propNode.AppendChild(elementNode);
                        }
                    }
                }
                else
                    propNode.InnerText = ConvertEx.ToString(prop.Value);

                propsNode.AppendChild(propNode);
            }

            xml.Save(path);
        }

        /// <summary>
        /// Restores the settings from an xml file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if the settings are restored from the file.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool RestoreFromXml(string path)
        {
            // No point in throwing an exception, just return;
            if (!io.File.Exists(path))
                return false;
            
            if (Properties == null)
                Initialize_Internal();

            var xml = new XmlDocument();
            xml.Load(path);

            var propsNode = xml.SelectSingleNode("CmdLineObject/Properties");
            foreach (XmlNode propNode in propsNode.ChildNodes)
            {
                var prop = Properties[propNode.Name];
                if (prop == null) continue;

                if (prop.PropertyType.IsArray)
                {
                    var vals = new List<object>();
                    var elementType = prop.PropertyType.GetElementType();
                    foreach (XmlNode elementNode in propNode.ChildNodes)
                        vals.Add(ConvertEx.ChangeType(elementNode.InnerXml, elementType));
                    var arr = Array.CreateInstance(elementType, vals.Count);
                    for (int i = 0; i < vals.Count; i++)
                        arr.SetValue(vals[i], i);
                    prop.Value = arr;
                }
                else
                    prop.Value = ConvertEx.ChangeType(propNode.InnerXml, prop.PropertyType);
            }

            return true;
        }

        private void BeginInit()
        {
        }

        private void EndInit()
        {
            mIsInitialized = true;
            Initialized();
        }

        /// <summary>
        /// This method is called after initialization is complete to allow for any validation.
        /// </summary>
        protected virtual void Initialized()
        {
        }

        #endregion

        #region Support

        private class Arg
        {
            public Arg(string name, string[] values)
            {
                mName = name;
                mValues = values;
            }

            private string mName;
            public string Name
            {
                get { return mName; }
            }

            private string[] mValues;
            public string[] Values
            {
                get { return mValues; }
            }

        }

        #endregion

    }

}
