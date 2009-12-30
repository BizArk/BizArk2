using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace Redwerb.BizArk.Core.Util
{

    /// <summary>
    /// Web related helper methods.
    /// </summary>
    public class WebUtil
    {

        /// <summary>
        /// Essentially creates a query string.
        /// </summary>
        /// <param name="frmVals"></param>
        /// <returns></returns>
        public static string GetUrlEncodedData(NameValueCollection frmVals)
        {
            var sb = new StringBuilder();
            foreach (string key in frmVals.AllKeys)
            {
                var value = frmVals[key];
                if (sb.Length > 0) sb.Append("&");
                sb.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(value));
            }
            return sb.ToString();
        }

    }
}
