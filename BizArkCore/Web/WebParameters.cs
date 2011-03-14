using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.IO;
using BizArk.Core.ArrayExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Dynamic class that stores parameters to be sent with the web request.
    /// </summary>
    public class WebParameters : DynamicObject
    {

        #region Fields and Properties

        private List<WebParameter> mParameters = new List<WebParameter>();

        /// <summary>
        /// Gets the number of parameters in the collection.
        /// </summary>
        public int Count
        {
            get { return mParameters.Count; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the value of the named member.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var param = Find(binder.Name);
            result = param == null ? null : param.Value;
            return param != null;
        }

        /// <summary>
        /// Sets the value of the named member.
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var param = Find(binder.Name);
            if (param != null)
                mParameters.Remove(param); // parameters are immutable.
            param = WebParameter.CreateParameter(binder.Name, value);
            mParameters.Add(param);
            return true;
        }

        /// <summary>
        /// Finds a named WebParameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public WebParameter Find(string name)
        {
            return mParameters.Find((param) => { return param.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase); });
        }

        /// <summary>
        /// Gets all the file parameters.
        /// </summary>
        /// <returns></returns>
        public WebFileParameter[] GetFileParameters()
        {
            return mParameters.FindAll((param) => { return param.GetType() == typeof(WebFileParameter); }).ToArray().Convert<WebFileParameter>();
        }

        /// <summary>
        /// Gets all the binary parameters.
        /// </summary>
        /// <returns></returns>
        public WebBinaryParameter[] GetBinaryParameters()
        {
            return mParameters.FindAll((param) => { return param.GetType() == typeof(WebBinaryParameter); }).ToArray().Convert<WebBinaryParameter>();
        }

        /// <summary>
        /// Gets all the text parameters.
        /// </summary>
        /// <returns></returns>
        public WebTextParameter[] GetTextParameters()
        {
            return mParameters.FindAll((param) => { return param.GetType() == typeof(WebTextParameter); }).ToArray().Convert<WebTextParameter>();
        }

        /// <summary>
        /// Determines if the collection has one or more file parameters.
        /// </summary>
        /// <returns></returns>
        public bool HasFileParameters()
        {
            var p = mParameters.Find((param) => { return param.GetType() == typeof(WebFileParameter); });
            return p != null;
        }

        /// <summary>
        /// Determines if the collection has one or more binary parameters.
        /// </summary>
        /// <returns></returns>
        public bool HasBinaryParameters()
        {
            var p = mParameters.Find((param) => { return param.GetType() == typeof(WebBinaryParameter); });
            return p != null;
        }

        /// <summary>
        /// Determines if the collection has one or more text parameters.
        /// </summary>
        /// <returns></returns>
        public bool HasTextParameters()
        {
            var p = mParameters.Find((param) => { return param.GetType() == typeof(WebTextParameter); });
            return p != null;
        }

        #endregion

    }

    /// <summary>
    /// Base class for web parameters.
    /// </summary>
    public abstract class WebParameter
    {

        /// <summary>
        /// Creates an instance of WebParameter.
        /// </summary>
        /// <param name="name"></param>
        protected WebParameter(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Factory method to create the parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static WebParameter CreateParameter(string name, object value)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");

            var file = value as UploadFile;
            if (file != null) return new WebFileParameter(name, file);

            var fi = value as FileInfo;
            if (fi != null) return new WebFileParameter(name, new UploadFile("", fi.FullName));

            var data = value as byte[];
            if (data != null) return new WebBinaryParameter(name, data);

            var text = ConvertEx.ToString(value);
            return new WebTextParameter(name, text);
        }

    }

    /// <summary>
    /// Web parameter for files.
    /// </summary>
    public class WebFileParameter : WebParameter
    {

        internal WebFileParameter(string name, UploadFile file)
            : base(name)
        {
            Value = file;
        }

        /// <summary>
        /// Gets the file.
        /// </summary>
        public UploadFile File { get { return (UploadFile)Value; } }

    }

    /// <summary>
    /// Web parameter for byte arrays.
    /// </summary>
    public class WebBinaryParameter : WebParameter
    {

        internal WebBinaryParameter(string name, byte[] data)
            : base(name)
        {
            Value = data;
        }

        /// <summary>
        /// Gets the byte array.
        /// </summary>
        public byte[] Data { get { return (byte[])Value; } }

    }

    /// <summary>
    /// Web parameter for text.
    /// </summary>
    public class WebTextParameter : WebParameter
    {

        internal WebTextParameter(string name, string text)
            : base(name)
        {
            Value = text;
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        public string Text { get { return (string)Value; } }

    }

}
