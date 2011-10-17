using System.Drawing;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System;
using System.Text;
using BizArk.Core.StringExt;
using BizArk.Core.FormatExt;
using System.Diagnostics;

namespace BizArk.Core.Web
{

    /// <summary>
    /// Contains the response from the request.
    /// </summary>
    public class WebHelperResponse
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperResponse.
        /// </summary>
        /// <param name="result"></param>
        /// <param name="contentType"></param>
        /// <param name="statusCode"></param>
        /// <param name="contentEncoding"></param>
        /// <param name="charSet"></param>
        public WebHelperResponse(object result, string contentType, HttpStatusCode statusCode, string contentEncoding, string charSet, WebHelperOptions options)
        {
            Result = result;
            ContentType = contentType;
            StatusCode = statusCode;
            ContentEncoding = contentEncoding;
            CharacterSet = charSet;
            Options = options;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets the result. If not handled by the ProcessResponseStream event, this will be a byte[].
        /// </summary>
        public object Result { get; private set; }

        /// <summary>
        /// Gets the content type that describes the response.
        /// </summary>
        public string ContentType { get; private set; }

        /// <summary>
        /// Gets the status code for the response.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the encoding for the response.
        /// </summary>
        public string ContentEncoding { get; private set; }

        /// <summary>
        /// Gets the character set for the response.
        /// </summary>
        public string CharacterSet { get; private set; }

        /// <summary>
        /// Gets the options for the request.
        /// </summary>
        public WebHelperOptions Options { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the result to an image.
        /// </summary>
        /// <returns></returns>
        public Image ResultToImage()
        {
            return ConvertEx.ChangeType<Image>(Result);
        }

        /// <summary>
        /// Converts the result to an xml document (the actual result should be an xml string).
        /// </summary>
        /// <returns></returns>
        public XmlDocument ResultToXml()
        {
            var xml = ResultToString();
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        /// <summary>
        /// Converts the result to an xml document (the actual result should be an xml string).
        /// </summary>
        /// <returns></returns>
        public XDocument ResultToXDoc()
        {
            var xml = ResultToString();
            return XDocument.Parse(xml);
        }

        /// <summary>
        /// Converts the result to a string.
        /// </summary>
        /// <returns></returns>
        public string ResultToString()
        {
            var encoding = GetEncoding();
            return encoding.GetString((byte[])Result);
        }

        /// <summary>
        /// Converts the result to any type of object you specify (assuming it can be converted by ConvertEx.ChangeType[T]).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ConvertResult<T>() where T : struct
        {
            var result = ResultToString();
            return ConvertEx.ChangeType<T>(result);
        }

        private const int cBytesToRead = 1024;
        /// <summary>
        /// Saves the result to a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="mode"></param>
        public void SaveFile(string fileName, FileMode mode)
        {
            var bytes = (byte[]) Result;
            using (var fs = new FileStream(fileName, mode))
            {
                for(int i = 0; i < bytes.Length; )
                {
                    fs.Write(bytes, i, Math.Min(bytes.Length - i, cBytesToRead));
                    i += cBytesToRead;
                }
                fs.Flush();
            }
        }

        /// <summary>
        /// Gets the character encoding for the response.
        /// </summary>
        /// <returns></returns>
        public Encoding GetEncoding()
        {
            try
            {
                if (!ContentEncoding.IsEmpty())
                    return Encoding.GetEncoding(ContentEncoding);
                if (!CharacterSet.IsEmpty())
                    return Encoding.GetEncoding(CharacterSet);

                var meta = Encoding.ASCII.GetString((byte[])Result).Trim();
                if (meta.StartsWith("<?xml"))
                {
                    var startPos = meta.IndexOf("encoding=\"");
                    if (startPos > 0)
                    {
                        var endPos = meta.IndexOf("\"", startPos + 1);
                        var charset = meta.Substring(startPos + 10, endPos - startPos + 1);
                        charset = charset.TrimEnd(new Char[] { '>', '"', '?' });
                        return Encoding.GetEncoding(charset);
                    }
                }
                else
                {
                    var startPos = meta.IndexOf("charset=");
                    if (startPos != -1)
                    {
                        var endPos = meta.IndexOf("\"", startPos);
                        if (endPos != -1)
                        {
                            var start = startPos + 8;
                            var charset = meta.Substring(start, endPos - start + 1);
                            charset = charset.TrimEnd(new Char[] { '>', '"' });
                            return Encoding.GetEncoding(charset);
                        }
                    }
                }
                return Options.ResponseEncoding;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error getting the encoding: {0}".Fmt(ex.Message));
                return Options.ResponseEncoding;
            }
        }

        #endregion

    }
}
