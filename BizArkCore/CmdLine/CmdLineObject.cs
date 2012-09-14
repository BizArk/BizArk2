using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using BizArk.Core.Extensions.ArrayExt;
using BizArk.Core.Extensions.AttributeExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Extensions.ObjectExt;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Web;
using System.Linq;

namespace BizArk.Core.CmdLine
{
	public interface ICmdLineObject
	{
		/// <summary>
		/// Gets the options used for the handling the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		CmdLineOptions Options { get; }

		/// <summary>
		/// Gets or sets a value that determines if help should be displayed.
		/// </summary>
		[CmdLineArg(Alias = "?", ShowInUsage = DefaultBoolean.True)]
		[Description("Displays command-line usage information.")]
		[Browsable(false)]
		[DefaultValue(false)]
		bool Help { get; set; }

		/// <summary>
		/// Gets the list of command-line properties.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		CmdLinePropertyList Properties { get; }

		/// <summary>
		/// Gets the error text for the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		string ErrorText { get; }

		/// <summary>
		/// Gets a value that determines if the CmdLineObject is ready to use.
		/// </summary>
		bool IsInitialized { get; }

		/// <summary>
		/// Gets the default properties for the command-line.
		/// </summary>
		CmdLineProperty[] DefaultProperties { get; }

		/// <summary>
		/// Initializes the CmdLineObject.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Initializes the command-line object, but does not populate it.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeEmpty();

		/// <summary>
		/// Initializes the command-line args based on a query string. Used for 
		/// </summary>
		/// <param name="queryStr"></param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeFromQueryString(string queryStr);

		/// <summary>
		/// Initializes the object with the given arguments.
		/// </summary>
		/// <param name="args">The command-line args. Make sure to shrink the array if the first element contains the path to the application (as in Environment.GetCommandLineArgs()) or the default parameter won't get set correctly.</param>
		/// <example>
		/// using BizArk.Core.ArrayExt;
		/// var args = Environment.GetCommandLineArgs().Shrink(1);
		/// </example>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void InitializeFromCmdLine(params string[] args);

		/// <summary>
		/// Makes sure the command-line object is valid. 
		/// </summary>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		bool IsValid();

		/// <summary>
		/// Gets the full description for the command-line arguments.
		/// </summary>
		/// <param name="maxWidth">Determines the number of characters per line. Set this to Console.Width.</param>
		/// <returns></returns>
		string GetHelpText(int maxWidth);

		/// <summary>
		/// Saves the settings to an xml file.
		/// </summary>
		/// <param name="path"></param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		void SaveToXml(string path);

		/// <summary>
		/// Restores the settings from an xml file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns>True if the settings are restored from the file.</returns>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		bool RestoreFromXml(string path);

		/// <summary>
		/// Gets the usage for this command-line object.
		/// </summary>
		/// <returns></returns>
		string ToString();

		/// <summary>
		/// Initializes the object with the given command line taking into consideration the Assignment Delimeter. 
		/// </summary>
		/// <param name="fullCommandLine"> </param>
		/// <param name="fullCommandLineArgs"> </param>
		void InitializeFromFullCmdLine(string fullCommandLine, string[] fullCommandLineArgs);
	}

	/// <summary>
	/// Represents an object that can be initialized via command-line arguments.
	/// </summary>
	/// <remarks>
	/// <para>The CmdLineObject class can be inherited from to allow the 
	/// properties of a class to be initialized from command-line arguments.
	/// The properties can be any type that can be converted to and from a string 
	/// using the <see cref="BizArk.Core.ConvertEx.ChangeType(object, Type, IFormatProvider)"/> 
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
	public abstract class CmdLineObject : ICmdLineObject
	{
		#region Initialization and Destruction

		/// <summary>
		/// Instantiates CmdLineObject.
		/// </summary>
		public CmdLineObject()
			: this(null)
		{
		}

		/// <summary>
		/// Instantiates CmdLineObject.
		/// </summary>
		/// <param name="options"></param>
		public CmdLineObject(CmdLineOptions options)
		{
			IsInitialized = false;

			Options = options ?? CreateOptions();

			if (Options.Description == null)
			{
				var att = this.GetAttribute<DescriptionAttribute>(true);
				if (att == null)
					Options.Description = Application.Description;
				else
					Options.Description = att.Description;
			}

			if (Options.DefaultArgNames == null)
			{
				var att = GetType().GetAttribute<CmdLineDefaultArgAttribute>(false);
				if (att != null)
					Options.DefaultArgNames = new[] {att.DefaultArgName};
			}
		}

		private CmdLineOptions CreateOptions()
		{
			var att = GetType().GetAttribute<CmdLineOptionsAttribute>(false);
			if (att != null)
				return att.CreateOptions();
			else
				return new CmdLineOptions();
		}

		#endregion

		#region Fields and Properties

		private string[] mErrors;

		/// <summary>
		/// Gets the options used for the handling the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public CmdLineOptions Options { get; private set; }

		/// <summary>
		/// Gets or sets a value that determines if help should be displayed.
		/// </summary>
		[CmdLineArg(Alias = "?", ShowInUsage = DefaultBoolean.True)]
		[Description("Displays command-line usage information.")]
		[Browsable(false)]
		[DefaultValue(false)]
		public bool Help { get; set; }

		/// <summary>
		/// Gets the list of command-line properties.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public CmdLinePropertyList Properties { get; private set; }

		/// <summary>
		/// Gets the error text for the command-line object.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		[Browsable(false)]
		public string ErrorText
		{
			get { return string.Join(Environment.NewLine, mErrors); }
		}

		/// <summary>
		/// Gets a value that determines if the CmdLineObject is ready to use.
		/// </summary>
		public bool IsInitialized { get; private set; }

		/// <summary>
		/// Gets the default properties for the command-line.
		/// </summary>
		public CmdLineProperty[] DefaultProperties { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Initializes the command-line object. 
		/// </summary>
		private void Initialize_Internal()
		{
			if (Properties == null)
			{
				// Get the list of properties that can be set from the command-line.
				// We must perform this step after construction which is why Initialize
				// is a separate step.
				Properties = new CmdLinePropertyList(this);
			}

			if (Options.Usage == null)
				Options.Usage = CreateUsage();

			if (Options.DefaultArgNames != null)
			{
				var props = new List<CmdLineProperty>();
				foreach (string argName in Options.DefaultArgNames)
					props.Add(Properties[argName]);
				DefaultProperties = props.ToArray();
			}
		}

		/// <summary>
		/// Initializes the CmdLineObject.
		/// </summary>
		public void Initialize()
		{
			if (Application.ClickOnceDeployed)
			{
				string url = Application.ClickOnceUrl;
				if (string.IsNullOrEmpty(url))
					InitializeEmpty();
				else
				{
					var uri = new Uri(url);
					string queryStr = uri.Query;
					InitializeFromQueryString(queryStr);
				}
			}
			else
			{
				InitializeFromFullCmdLine(Environment.CommandLine, Environment.GetCommandLineArgs());
			}
		}

		/// <summary>
		/// Initializes the object with the given command line taking into consideration the Assignment Delimeter. 
		/// </summary>
		/// <param name="fullCommandLine"> </param>
		/// <param name="fullCommandLineArgs"> </param>
		public void InitializeFromFullCmdLine(string fullCommandLine, string[] fullCommandLineArgs)
		{
			string[] args;

			if (Options.AssignmentDelimiters == null || (
				Options.AssignmentDelimiters != null && 
				Options.AssignmentDelimiters.Length == 1 &&
				Options.AssignmentDelimiters[0] == ' ' ) 
				)
			{
				// the first parameter is the name of the application.
				args = fullCommandLineArgs.Shrink(1); 
			}
			else
			{
				var commandLine = fullCommandLine;
				commandLine = commandLine.Substring(fullCommandLineArgs[0].Length);
				var delimeters = Options.AssignmentDelimiters.ToList();
				delimeters.Add(' ');
				args = commandLine.Split(delimeters.ToArray(), StringSplitOptions.RemoveEmptyEntries);
			}
			InitializeFromCmdLine(args);
		}

		/// <summary>
		/// Initializes the command-line object, but does not populate it.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void InitializeEmpty()
		{
			InitializeBy(Initialize_Internal);
		}

		private void InitializeBy(Action action)
		{
			BeginInit();
			action();
			EndInit();
		}

		/// <summary>
		/// Initializes the command-line args based on a query string. Used for 
		/// </summary>
		/// <param name="queryStr"></param>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void InitializeFromQueryString(string queryStr)
		{
			InitializeBy(() =>
				             {
					             Initialize_Internal();
					             if (Properties.Count == 0) return;

					             // the query string is empty so just return.
					             if (string.IsNullOrEmpty(queryStr)) return;

					             var ps = new UrlParamList(queryStr);
					             foreach (UrlParam p in ps)
					             {
						             CmdLineProperty prop = Properties[p.Name];
						             if (prop == null) continue;
						             prop.Value = p.Value;
					             }
				             });
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
			InitializeBy(() =>
				             {
					             Initialize_Internal();
					             if (Properties.Count == 0) return;

					             // There aren't any args so just return.
					             if (args.Length == 0) return;

					             int startIndex = 0;
					             if (DefaultProperties != null && DefaultProperties.Length > 0 &&
					                 !args[0].StartsWith(Options.ArgumentPrefix))
					             {
						             string[] argValues = GetArgValues(args);
						             if (DefaultProperties.Length == 1)
							             DefaultProperties[0].Value = argValues;
						             else
						             {
							             for (int i = 0; i < argValues.Length && i < DefaultProperties.Length; i++)
								             DefaultProperties[i].Value = argValues[i];
						             }
						             startIndex = argValues.Length;
						             // We already took care of the first set of arguments so start with the next.
					             }

					             for (int i = startIndex; i < args.Length; i++)
					             {
						             if (!args[i].StartsWith(Options.ArgumentPrefix)) continue;

						             string argName = args[i].Substring(Options.ArgumentPrefix.Length);
						             if (argName == "") continue;

						             CmdLineProperty prop = Properties[argName.TrimEnd('-')];
						             if (prop == null) continue;

						             string[] argValues;
						             argValues = GetArgValues(args.Shrink(i + 1));
						             i = i + argValues.Length; // Move to the next valid argument.

						             if (prop.PropertyType == typeof (bool))
						             {
							             // Boolean arguments don't require an argument value (/A and /A- can be used for true/false).
							             if (argName.EndsWith("-"))
								             prop.Value = false;
							             else
							             {
								             bool bVal = true;

								             if (argValues.Length > 0)
								             {
									             bVal = ConvertEx.ToBoolean(argValues[0]);
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
					             }
				             });
		}

		private string[] GetArgValues(string[] args)
		{
			var argValues = new List<string>();

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i].StartsWith(Options.ArgumentPrefix)) return argValues.ToArray();
				argValues.Add(args[i]);
			}

			return argValues.ToArray();
		}

		/// <summary>
		/// Override this method to perform cmd-line validation. It is recommended to call the base method.
		/// </summary>
		/// <returns></returns>
		protected virtual string[] Validate()
		{
			var errors = new List<string>();
			foreach (CmdLineProperty prop in Properties)
			{
				if (!prop.Error.IsEmpty())
					errors.Add(string.Format("{0} has an error: {1}", prop.Name, prop.Error));
				if (prop.Required && !prop.PropertySet)
					errors.Add(string.Format("{0} is required.", prop.Name));

				foreach (ICustomValidator validator in prop.Validators)
				{
					if (!validator.IsValid(prop.Value))
						errors.Add(validator.FormatErrorMessage(prop.Name));
				}
			}

			ValidationResult[] results = ObjectExt.Validate(this);
			foreach (ValidationResult result in results)
			{
				if (result.ErrorMessage.HasValue())
					errors.Add(result.ErrorMessage);
			}

			return errors.ToArray();
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
			mErrors = Validate();

			if (mErrors.Length == 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the full description for the command-line arguments.
		/// </summary>
		/// <param name="maxWidth">Determines the number of characters per line. Set this to Console.Width.</param>
		/// <returns></returns>
		public virtual string GetHelpText(int maxWidth)
		{
			if (Properties == null) throw new InvalidOperationException("The command-line object has not been initialized yet.");

			var desc = new StringBuilder();

			if (mErrors != null && mErrors.Length > 0)
			{
				desc.AppendLine(("ERROR: " + mErrors[0]).Wrap(maxWidth));
				for (int i = 1; i < mErrors.Length; i++)
					desc.AppendLine(mErrors[i].Wrap(maxWidth, "    "));
				desc.AppendLine();
			}

			desc.AppendLine(Options.Title);
			desc.AppendLine(Options.Description.Wrap(maxWidth));
			desc.AppendLine("Usage: " + Options.Usage);
			desc.AppendLine();

			int maxNameWidth = GetMaxNameLength() + 6; // Add extra for ' (S): '
			int maxDescWidth = maxWidth - maxNameWidth;
			var indent = new string(' ', maxNameWidth);

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
				string propDesc = prop.Description.Wrap(maxDescWidth);
				string[] lines = propDesc.Lines();
				if (lines.Length > 0)
				{
					desc.AppendLine(lines[0]);
					for (int i = 1; i < lines.Length; i++)
						desc.AppendLine(indent + lines[i]);
				}
				else
				{
					// If there isn't a description, we need to add a line break.
					desc.AppendLine();
				}
				if (prop.Required)
					desc.AppendLine(indent + "REQUIRED");
				else if (prop.ShowDefaultValue)
				{
					object dflt = prop.DefaultValue;
					if (!ConvertEx.IsEmpty(dflt))
					{
						var arr = dflt as Array;
						if (arr != null)
						{
							string[] strs = arr.Convert<string>();
							if (dflt.GetType().GetElementType() == typeof (string))
								dflt = "[\"{0}\"]".Fmt(strs.Join("\", \""));
							else
								dflt = "[{0}]".Fmt(strs.Join(", "));
						}
						desc.AppendLine(indent + string.Format("Default Value: {0}", dflt));
					}
				}

				if (prop.PropertyType.IsEnum)
				{
					var enumVals = new StringBuilder();
					string[] enumNames = prop.PropertyType.GetEnumNames();
					enumVals.AppendFormat(indent + "Possible Values: [{0}]", enumNames.Join(", "));
					if (enumVals.Length < maxWidth)
						desc.AppendLine(enumVals.ToString());
				}

				foreach (ValidationAttribute validator in prop.GetValidationAtts())
				{
					string message = validator.FormatErrorMessage(prop.Name).Wrap(maxDescWidth);
					foreach (string line in message.Lines())
						desc.AppendLine(indent + line);
				}
				foreach (ICustomValidator validator in prop.Validators)
				{
					string message = validator.FormatErrorMessage(prop.Name).Wrap(maxDescWidth);
					foreach (string line in message.Lines())
						desc.AppendLine(indent + line);
				}
			}

			return desc.ToString();
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

			XmlElement typeNode = xml.CreateElement("Type");
			typeNode.InnerText = GetType().FullName;
			xml.DocumentElement.AppendChild(typeNode);

			XmlElement propsNode = xml.CreateElement("Properties");
			xml.DocumentElement.AppendChild(propsNode);

			foreach (CmdLineProperty prop in Properties)
			{
				if (!prop.AllowSave) continue;

				XmlElement propNode = xml.CreateElement(prop.Name);

				if (prop.PropertyType.IsArray)
				{
					var arr = prop.Value as Array;
					if (arr != null)
					{
						foreach (object elementVal in arr)
						{
							XmlElement elementNode = xml.CreateElement("Element");
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
			string xmlStr = GetXml(path);
			// No point in throwing an exception, just return;
			if (string.IsNullOrEmpty(xmlStr))
				return false;

			if (Properties == null)
				Initialize_Internal();

			var xml = new XmlDocument();
			xml.LoadXml(xmlStr);

			XmlNode propsNode = xml.SelectSingleNode("CmdLineObject/Properties");
			foreach (XmlNode propNode in propsNode.ChildNodes)
			{
				CmdLineProperty prop = Properties[propNode.Name];
				if (prop == null) continue;

				if (prop.PropertyType.IsArray)
				{
					var vals = new List<object>();
					Type elementType = prop.PropertyType.GetElementType();
					foreach (XmlNode elementNode in propNode.ChildNodes)
						vals.Add(ConvertEx.ChangeType(elementNode.InnerXml, elementType));
					Array arr = Array.CreateInstance(elementType, vals.Count);
					for (int i = 0; i < vals.Count; i++)
						arr.SetValue(vals[i], i);
					prop.Value = arr;
				}
				else
					prop.Value = ConvertEx.ChangeType(propNode.InnerXml, prop.PropertyType);
			}

			return true;
		}

		[SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		private string GetXml(string path)
		{
			try
			{
				var uri = new Uri(path);
				using (WebResponse response = WebRequest.Create(uri).GetResponse())
				using (Stream stream = response.GetResponseStream())
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
			catch
			{
				return "";
			}
		}

		private void BeginInit()
		{
		}

		private void EndInit()
		{
			if (!Options.WaitArgName.IsEmpty())
			{
				CmdLineProperty wait = Properties[Options.WaitArgName];
				if (wait == null)
					throw new CmdLineException("The Wait property '{0}' was not found.".Fmt(Options.WaitArgName));
				if (wait.PropertyType != typeof (bool))
					throw new CmdLineException("The Wait property must be a boolean.");
				Options.Wait = (bool) wait.Value;
			}

			IsInitialized = true;
			Initialized();
		}

		/// <summary>
		/// This method is called after initialization is complete to allow for any additional intialization.
		/// </summary>
		protected virtual void Initialized()
		{
		}

		/// <summary>
		/// Gets the usage for this command-line object.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Options.Usage;
		}

		private string CreateUsage()
		{
			var usage = new StringBuilder();
			usage.Append(Options.ApplicationName);

			if (DefaultProperties != null && DefaultProperties.Length > 0)
			{
				foreach (CmdLineProperty prop in DefaultProperties)
					usage.Append(" <" + (string.IsNullOrEmpty(prop.Usage) ? prop.Name : prop.Usage) + ">");
			}

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
				return string.Format("{0}{1} <{2}>", Options.ArgumentPrefix, id, prop.Usage);

			if (prop.PropertyType == typeof (bool))
				return string.Format("{0}{1}[-]", Options.ArgumentPrefix, id);
			else
				return string.Format("{0}{1} <{2}>", Options.ArgumentPrefix, id, prop.Name);
		}

		#endregion

		#region Arg support class

		private class Arg
		{
			private readonly string mName;

			private readonly string[] mValues;

			public Arg(string name, string[] values)
			{
				mName = name;
				mValues = values;
			}

			public string Name
			{
				get { return mName; }
			}

			public string[] Values
			{
				get { return mValues; }
			}
		}

		#endregion
	}
}