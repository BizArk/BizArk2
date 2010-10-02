using System;
using System.Net;
using System.Text;
using BizArk.Core.AttributeExt;
using BizArk.Core.WebExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// The UrlBuilder allows you to easily create a properly formatted URL including encoding of parameter values.
    /// </summary>
    public class UrlBuilder
    {
        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of UrlBuilder.
        /// </summary>
        public UrlBuilder()
        {
            Protocol = "http";
            Host = "";
            Port = -1;
            Path = new UrlSegmentList();
            Parameters = new UrlParamList();
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the protocol for the URL (e.g. http).
        /// </summary>
        public string Protocol { get; set; }

        /// <summary>
        /// Gets or sets the domain name or IP for the server.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the port number. Less than or equal to 0 prevents the port from appearing in the URL.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets the host:port (e.g. www.tourfactory.com:8080).
        /// </summary>
        public string Authority
        {
            get
            {
                if (string.IsNullOrEmpty(Host)) return "";
                if (Port <= 0) return Host;
                return Host + ":" + Port;
            }
        }

        /// <summary>
        /// Gets the path for the URL. This is represented as a list of names. Index 0 will be displayed first (e.g. /Path0/Path1/.../PathN).
        /// </summary>
        public UrlSegmentList Path { get; private set; }

        /// <summary>
        /// Gets the list of query string parameters.
        /// </summary>
        public UrlParamList Parameters { get; private set; }

        private string mAnchor;
        /// <summary>
        /// Gets or sets the anchor for the page.
        /// </summary>
        public string Anchor
        {
            get { return mAnchor; }
            set { mAnchor = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the URI for this URL.
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return new Uri(this.ToString(true));
        }

        /// <summary>
        /// Returns the properly formatted URL.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// Gets the URI for this URL.
        /// </summary>
        /// <param name="includeParams">Used by WebClient to not include the parameters in the url.</param>
        /// <returns></returns>
        protected Uri ToUri(bool includeParams)
        {
            return new Uri(this.ToString(includeParams));
        }

        /// <summary>
        /// Returns the properly formatted URL.
        /// </summary>
        /// <param name="includeParams">Used by WebClient to not include the parameters in the url.</param>
        /// <returns></returns>
        protected string ToString(bool includeParams)
        {
            if (string.IsNullOrEmpty(Host)) return "";

            var sb = new StringBuilder();
            sb.AppendFormat(@"{0}://{1}", Protocol, Host);
            if (Port > 0)
                sb.AppendFormat(":{0}", Port);

            foreach (string segment in Path)
                sb.AppendFormat(@"/{0}", segment);

            if (includeParams && Parameters.Count > 0)
                sb.Append("?" + Parameters.ToString());

            if (!string.IsNullOrEmpty(mAnchor))
                sb.Append("#" + mAnchor);

            return sb.ToString();
        }

        /// <summary>
        /// Submits the request to the server.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public string Submit(HttpMethod method)
        {
            WebRequest request;

            if (method == HttpMethod.Get)
                request = WebRequest.Create(ToUri(true));
            else
                request = WebRequest.Create(ToUri(false));
            request.Method = method.GetDescription();

            // If the method is Get, the params are included in the querystring, 
            // otherwise they should be put in the content of the request.
            if (method != HttpMethod.Get)
            {
                //todo: check to see if any of the parameters are of type FileInfo. 
                // If they are, use multi-part encoding and send files.
                // Will need to allow Objects in parameters. Probably better anyway 
                // so that we can easily send different data types and let the 
                // system convert to a string (using ConvertEx of course).

                var content = GetEncodedContent();
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = content.Length;
                using (var s = request.GetRequestStream())
                {
                    var contentData = UTF8Encoding.Default.GetBytes(content.ToString());
                    s.Write(contentData, 0, contentData.Length);
                }
            }

            var response = request.GetResponse() as HttpWebResponse;
            var result = response.GetContentString();
            response.Close();

            return result;
        }

        private string GetEncodedContent()
        {
            var content = new StringBuilder();
            var first = true;
            foreach (var p in Parameters)
            {
                if (!first)
                    content.Append("&");
                first = false;
                content.AppendFormat("{0}={1}", p.Name, p.EncodedValue);
            }
            return content.ToString();
        }

        #endregion
    }

}
