using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Web;

namespace Redwerb.BizArk.Core.WebExt
{
    /// <summary>
    /// Extension methods that are useful when working with web objects.
    /// </summary>
    public static class WebExt
    {

        /// <summary>
        /// Gets the content from a response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static string GetContentString(this WebResponse response)
        {
            using (var s = response.GetResponseStream())
            {
                var sr = new StreamReader(s);
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Encodes a string for safe HTML.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlEncode(this string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Decodes an encoded string.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string HtmlDecode(this string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        /// <summary>
        /// Can be used to encode a query string value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }

        /// <summary>
        /// Can be used to decode a url encoded value.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlDecode(this string str)
        {
            return HttpUtility.UrlDecode(str);
        }

    }
}
