using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Represents a parameter in a URL.
    /// </summary>
    public class UrlParam
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlParam.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public UrlParam(string name, string value)
        {
            mName = name;
            mValue = value;
        }

        #endregion

        #region Fields and Properties

        private string mName = "";
        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        private string mValue = "";
        /// <summary>
        /// Gets the value of the parameter.
        /// </summary>
        public string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        /// <summary>
        /// Gets the properly encoded value of the parameter.
        /// </summary>
        public string EncodedValue
        {
            get { return HttpUtility.UrlEncode(mValue); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the URL encoded key=value.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}={1}", mName, EncodedValue);
        }

        #endregion

    }

    /// <summary>
    /// Contains a list of UrlParam objects. 
    /// </summary>
    public class UrlParamList
        : List<UrlParam>
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlParamList.
        /// </summary>
        public UrlParamList()
        {
        }

        /// <summary>
        /// Creates an instance of UrlParamList based on a query string.
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public UrlParamList(string queryStr)
        {
            AddRange(queryStr);
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets a url parameter based on it's name. 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Null if not found.</returns>
        public UrlParam this[string name]
        {
            get
            {
                foreach (UrlParam p in this)
                    if (p.Name == name) return p;
                return null;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a range of parameters based on a query string.
        /// </summary>
        /// <param name="queryStr"></param>
        /// <returns></returns>
        public UrlParam[] AddRange(string queryStr)
        {
            var paramList = new List<UrlParam>();

            var r = HttpUtility.ParseQueryString(queryStr);
            foreach (string key in r.Keys)
                paramList.Add(this.Add(key, r[key]));

            return paramList.ToArray();
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, string value)
        {
            var p = new UrlParam(name, value);
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, long value)
        {
            var p = new UrlParam(name, value.ToString());
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, ulong value)
        {
            var p = new UrlParam(name, value.ToString());
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Adds a UrlParam to the list.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public UrlParam Add(string name, Guid value)
        {
            var p = new UrlParam(name, value.ToString("B"));
            this.Add(p);
            return p;
        }

        /// <summary>
        /// Removes a parameter from the list.
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            this.Remove(this[name]);
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetString(string name)
        {
            return GetString(name, "");
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public string GetString(string name, string dflt)
        {
            var p = this[name];
            if (p == null) return dflt;
            return p.Value;
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetLong(string name)
        {
            return GetLong(name, int.MinValue);
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dflt"></param>
        /// <returns></returns>
        public long GetLong(string name, int dflt)
        {
            var p = this[name];
            if (p == null) return dflt;
            int val;
            if (int.TryParse(p.Value, out val)) return val;
            return dflt;
        }

        /// <summary>
        /// Returns the value of the given parameter.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Guid GetGuid(string name)
        {
            var p = this[name];
            if (p == null) return Guid.Empty;
            try
            {
                return new Guid(p.Value);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Returns the encoded query string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var parameters = new List<string>();
            foreach (UrlParam param in this)
                parameters.Add(param.ToString());
            return string.Join("&", parameters.ToArray());
        }

        #endregion

    }

    /// <summary>
    /// Holds parts of a url that will be combined to create the path.
    /// </summary>
    public class UrlSegmentList
        : List<string>
    {

        /// <summary>
        /// Adds a set of segments to the url.
        /// </summary>
        /// <param name="segments"></param>
        public void AddRange(params string[] segments)
        {
            base.AddRange(segments);
        }
    }

    /// <summary>
    /// The different HTTP methods supported by the UrlBuilder submit method.
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>Get methods</summary>
        [Description("GET")]
        Get,
        /// <summary>Post methods</summary>
        [Description("POST")]
        Post,
        /// <summary>Put methods</summary>
        [Description("PUT")]
        Put,
        /// <summary>Delete methods</summary>
        [Description("DELETE")]
        Delete
    }

}
