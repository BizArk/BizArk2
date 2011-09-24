using System;
using System.IO;
using System.Net;
using System.Text;
using BizArk.Core.MathExt;
using BizArk.Core.StringExt;
using BizArk.Core.Util;

namespace BizArk.Core.Web
{

    /// <summary>
    /// This is a helper class to easily make web requests. It is intended as a replacement for WebClient. It includes the ability to upload multiple files, post form values, set a timeout, run asynchrounously, and reports progress.
    /// </summary>
    public static class WebHelper2
    {

        #region Initialization and Destruction

        static WebHelper2()
        {
            DefaultOptions = new WebHelperOptions();
        }

        #endregion

        #region Fields and Properties

        private static WebHelperOptions DefaultOptions { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Downloads the content from the given address and saves it as a file.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="fileName">Path that the file should be saved to.</param>
        /// <param name="options">Request options.</param>
        public static void DownloadFile(string address, string fileName, WebHelperOptions options = null)
        {
            DownloadFile(new Uri(address), fileName, options);
        }

        /// <summary>
        /// Downloads the content from the given address and saves it as a file.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="fileName">Path that the file should be saved to.</param>
        /// <param name="options">Request options.</param>
        public static void DownloadFile(Uri address, string fileName, WebHelperOptions options = null)
        {
            options = options ?? DefaultOptions;
            var dir = System.IO.Path.GetDirectoryName(fileName);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (var fs = new FileStream(fileName, FileMode.CreateNew, FileAccess.Write))
                ProcessRequest(address, options, (buffer, read) => { fs.Write(buffer, 0, read); });
        }

        /// <summary>
        /// Downloads the request and converts it to a string.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="options">Request options.</param>
        /// <returns></returns>
        public static byte[] DownloadData(string address, WebHelperOptions options = null)
        {
            return DownloadData(new Uri(address), options);
        }

        /// <summary>
        /// Downloads the request and converts it to a string.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="options">Request options.</param>
        /// <returns></returns>
        public static byte[] DownloadData(Uri address, WebHelperOptions options = null)
        {
            options = options ?? DefaultOptions;
            using (var ms = new MemoryStream())
            {
                var response = ProcessRequest(address, options, (buffer, read) => { ms.Write(buffer, 0, read); });
                var http = response as HttpWebResponse;
                if (http != null)
                {
                    if (!http.CharacterSet.IsEmpty())
                        options.Encoding = Encoding.GetEncoding(http.CharacterSet);
                }
                ms.Flush();
                ms.Position = 0;
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Downloads the request and converts it to a string.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="options">Request options.</param>
        /// <returns></returns>
        public static string DownloadString(string address, WebHelperOptions options = null)
        {
            return DownloadString(new Uri(address), options);
        }

        /// <summary>
        /// Downloads the request and converts it to a string.
        /// </summary>
        /// <param name="address">URL to the file to download.</param>
        /// <param name="options">Request options.</param>
        /// <returns></returns>
        public static string DownloadString(Uri address, WebHelperOptions options = null)
        {
            options = options ?? DefaultOptions;
            var data = DownloadData(address, options);
            return options.Encoding.GetString(data);
        }

        private static WebRequest CreateRequest(Uri address, WebHelperOptions options)
        {
            var request = WebRequest.Create(address);
            var http = request as HttpWebRequest;
            request.Headers.Add(options.Headers);
            if (options.UseCompression)
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

            if (http != null)
            {
                http.Timeout = (int)options.Timeout.TotalMilliseconds;
                if (!string.IsNullOrEmpty(options.UserAgent)) http.UserAgent = options.UserAgent;
                http.KeepAlive = options.KeepAlive;
                http.AllowAutoRedirect = options.AllowAutoRedirect;
            }

            //todo: prepare request based on content type.

            if (options.PrepareRequest != null)
                options.PrepareRequest(request);

            return request;
        }

        private static WebResponse ProcessRequest(Uri address, WebHelperOptions options, Action<byte[], int> processBytes)
        {
            var request = CreateRequest(address, options);
            var requestTotal = new MemSize(request.ContentLength.Between(0, long.MaxValue));
            Report(options, RequestState.NotStarted, requestTotal, MemSize.Zero, MemSize.Zero, MemSize.Zero);

            var response = request.GetResponse();
            Report(options, RequestState.Receiving, requestTotal, MemSize.Zero, MemSize.Zero, MemSize.Zero);

            var stream = response.GetResponseStream();
            var responseTotal = new MemSize(response.ContentLength.Between(0, long.MaxValue));
            var buffer = new byte[4096];

            int totalRead = 0;
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalRead += read;
                processBytes(buffer, read);
                Report(options, RequestState.Receiving, requestTotal, requestTotal, responseTotal, new MemSize(totalRead));
            }
            responseTotal = new MemSize(totalRead); // make sure the final total is accurate.
            Report(options, RequestState.Complete, requestTotal, requestTotal, responseTotal, responseTotal);

            return response;
        }

        private static void Report(WebHelperOptions options, RequestState state, MemSize requestTotal, MemSize requestSent, MemSize responseTotal, MemSize responseReceived)
        {
            if (options.ReportProgress == null) return;
            options.ReportProgress(new WebHelperProgress(state, requestTotal, requestSent, responseTotal, responseReceived));
        }

        #endregion

    }

    /// <summary>
    /// Options that can be passed into WebHelper request methods.
    /// </summary>
    public class WebHelperOptions
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperOptions.
        /// </summary>
        public WebHelperOptions()
        {
            Headers = new WebHeaderCollection();
            UseCompression = false;
            Timeout = TimeSpan.FromMinutes(2);
            Encoding = Encoding.Default;
            AllowAutoRedirect = true;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets or sets the timeout for requests.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets the headers for the request.
        /// </summary>
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Gets or sets a value that determines if compression is used or not.
        /// </summary>
        public bool UseCompression { get; set; }

        /// <summary>
        /// Gets or sets the user agent to use for the request.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the KeepAlive header should be set.
        /// </summary>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if 302 redirects will be automatically followed.
        /// </summary>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets the character encoding to use for uploading strings. Will also be used for downloading strings if not specified in the response. This value is updated by WebHelper if specified in the response.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets or sets a method that can do additional preparation of a request before it is submitted.
        /// </summary>
        public Action<WebRequest> PrepareRequest { get; set; }

        /// <summary>
        /// Gets or sets a method that can do additional preparation of a request before it is submitted.
        /// </summary>
        public Action<WebHelperProgress> ReportProgress { get; set; }

        #endregion

    }

    /// <summary>
    /// Used to report progress during a WebHelper request.
    /// </summary>
    public class WebHelperProgress
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperProgress.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="requestTotal"></param>
        /// <param name="requestSent"></param>
        /// <param name="responseTotal"></param>
        /// <param name="responseReceived"></param>
        internal WebHelperProgress(RequestState state, MemSize requestTotal, MemSize requestSent, MemSize responseTotal, MemSize responseReceived)
        {
            State = state;
            RequestTotal = requestTotal;
            RequestSent = requestSent;
            ResponseTotal = responseTotal;
            ResponseReceived = responseReceived;
        }

        #endregion

        #region Fields and Properties

        /// <summary>
        /// Gets a value that determines what state the request is in.
        /// </summary>
        public RequestState State { get; private set; }

        /// <summary>
        /// Gets the number of bytes in the request.
        /// </summary>
        public MemSize RequestTotal { get; private set; }

        /// <summary>
        /// Gets the number of bytes that have been sent so far.
        /// </summary>
        public MemSize RequestSent { get; private set; }

        /// <summary>
        /// Gets the number of bytes expected in the response. This will be 0 until we receive the headers for the response.
        /// </summary>
        public MemSize ResponseTotal { get; private set; }

        /// <summary>
        /// Gets the number of bytes that have been received so far.
        /// </summary>
        public MemSize ResponseReceived { get; private set; }

        #endregion

    }

    /// <summary>
    /// Used during WebHelper progress reports to determine the current state of the request.
    /// </summary>
    public enum RequestState
    {
        /// <summary>The request has not yet started.</summary>
        NotStarted,
        /// <summary>The request is sending data to the server.</summary>
        Sending,
        /// <summary>The request is receiving data from the server.</summary>
        Receiving,
        /// <summary>The request has completed.</summary>
        Complete
    }

}
