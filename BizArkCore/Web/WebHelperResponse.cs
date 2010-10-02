using System.Drawing;
using System.Net;
using System.Xml;
using BizArk.Core;

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
        public WebHelperResponse(object result, string contentType, HttpStatusCode statusCode, string contentEncoding, string charSet)
        {
            mResult = result;
            mContentType = contentType;
            mStatusCode = statusCode;
            mContentEncoding = contentEncoding;
            mCharacterSet = charSet;
        }

        #endregion

        #region Fields and Properties

        private object mResult;
        /// <summary>
        /// Gets the result. If not handled by the ProcessResponseStream event, this will be a byte[].
        /// </summary>
        public object Result
        {
            get { return mResult; }
        }

        private string mContentType;
        /// <summary>
        /// Gets the content type that describes the response.
        /// </summary>
        public string ContentType
        {
            get { return mContentType; }
        }

        private HttpStatusCode mStatusCode;
        /// <summary>
        /// Gets the status code for the response.
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return mStatusCode; }
        }

        private string mContentEncoding;
        /// <summary>
        /// Gets the encoding for the response.
        /// </summary>
        public string ContentEncoding
        {
            get { return mContentEncoding; }
        }

        private string mCharacterSet;
        /// <summary>
        /// Gets the character set for the response.
        /// </summary>
        public string CharacterSet
        {
            get { return mCharacterSet; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converts the result to an image.
        /// </summary>
        /// <returns></returns>
        public Image ResultToImage()
        {
            return ConvertEx.ChangeType<Image>(mResult);
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
        /// Converts the result to a string.
        /// </summary>
        /// <returns></returns>
        public string ResultToString()
        {
            return ConvertEx.ToString(mResult);
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

        #endregion

    }
}
