using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using BizArk.Core.ExceptionExt;

namespace BizArk.Core.Web
{

    /// <summary>
    /// This is a helper class to easily make web requests. It is intended as a replacement for WebClient. It includes the ability to upload multiple files, post form values, set a timeout, run asynchrounously, and reports progress.
    /// </summary>
    public class WebHelper
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelper.
        /// </summary>
        public WebHelper(string url)
        {
            Parameters = new WebParameters();
            Url = url;
            Timeout = TimeSpan.FromSeconds(100);
            Method = HttpMethod.Get;
            Headers = new WebHeaderCollection();
            UserAgent = "";
            KeepAlive = true;
            AllowAutoRedirect = true;
            UseCompression = true;
        }

        #endregion

        #region Fields and Properties

        private AsyncOperation mAsyncOperation = null;
        private Thread mRequestThread = null;
        private long mRequestContentLength = 0;
        private long mResponseContentLength = 0;

        /// <summary>
        /// Gets or sets the url for the web request.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Gets or sets the method for the web request. The default is GET but might be different based on the content type.
        /// </summary>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets or sets the timeout for the web request. If null, uses the default value for HttpWebRequest (100 seconds). For no timeout, set to TimeSpan.MaxValue.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Gets or sets the content type for the request. If null, will determine the content type based on what needs to be sent. ContentTypes are a one-use thing. If set, it will need to be set for each call.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public ContentType ContentType { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the request. This can be an object with the properties as parameters (recommend anonymous object) or it can be a WebParameterDictionary. To upload files, use a UploadFile object.
        /// </summary>
        public dynamic Parameters { get; private set; }

        /// <summary>
        /// Gets the headers for the request.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public WebHeaderCollection Headers { get; private set; }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if http keep-alives are used. The default is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if redirects are followed. Default is false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets the estimated length of the response in number of bytes. This is used to estimate the total progress percent until we can get the actual length of the response. If set to null, upload/download will be split 50/50.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public long? EstimatedResponseLength { get; set; }

        /// <summary>
        /// Gets a value that determines if an asynchronous request is already in progress.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                if (mAsyncOperation == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary>
        /// Gets a value that determines if the current asynchronous request has been cancelled.
        /// </summary>
        public bool CancellationPending { get; private set; }

        /// <summary>
        /// Gets or sets the state object that will be sent through the events. The state object is not used internally.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object State { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if compression should be used. The default is true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool UseCompression { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Makes a request.
        /// </summary>
        /// <returns></returns>
        public WebHelperResponse MakeRequest()
        {
            if (IsBusy) throw new InvalidOperationException("A request has already been made. WebHelper only supports a single request at a time.");

            // Make sure the thread is dead. We don't want to make a request on the thread.
            if (mRequestThread != null)
            {
                try
                {
                    mRequestThread.Abort();
                }
                catch (Exception ex)
                {
                    // ignore exceptions.
                    Debug.WriteLine(ex.GetDetails());
                }
            }
            mRequestThread = null;

            return MakeRequest_Internal();
        }

        /// <summary>
        /// Makes the web request asynchronously.
        /// </summary>
        public void MakeRequestAsync()
        {
            MakeRequestAsync(null);
        }

        /// <summary>
        /// Makes the web request asynchronously.
        /// </summary>
        /// <param name="state"></param>
        public void MakeRequestAsync(object state)
        {
            if (IsBusy) throw new InvalidOperationException("An asynchronous request is already in progress. WebHelper only supports a single request at a time.");

            mAsyncOperation = AsyncOperationManager.CreateOperation(state);
            mRequestThread = new Thread(() =>
            {
                MakeRequest_Internal();
            });
            mRequestThread.Start();
        }

        private WebHelperResponse MakeRequest_Internal()
        {
            CancellationPending = false;

            WebHelperResponse whResponse = null;
            try
            {
                HttpWebRequest request = null;
                bool sent = false;

                using (var contentType = ContentType ?? ContentType.CreateContentType(Method, (WebParameters)Parameters))
                {
                    request = CreateRequest(contentType);
                    sent = contentType.SendRequest(this, request);
                }

                RequestCompletedEventArgs completedArgs = null;
                if (!sent)
                {
                    if (!CancellationPending) throw new InvalidOperationException("Unable to complete request. Unknown error.");
                    completedArgs = new RequestCompletedEventArgs(State, true);
                }
                else
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                        whResponse = ProcessResponse(response);

                    // even if a cancellation is pending, if we recieved the response, complete the request as normal.
                    if (whResponse != null)
                        completedArgs = new RequestCompletedEventArgs(whResponse, State, false);
                    else if (CancellationPending)
                        completedArgs = new RequestCompletedEventArgs(whResponse, State, true);
                    else
                        throw new InvalidOperationException("Unable to receive response. Unknown error.");
                }

                Post((arg) =>
                {
                    OnRequestCompleted((RequestCompletedEventArgs)arg);
                }, completedArgs);

            }
            catch (Exception ex)
            {
                var completedArgs = new RequestCompletedEventArgs(ex, State, false);
                Post((arg) =>
                {
                    // Regardless of the outcome, RequestCompleted is guaranteed to be raised.
                    // Caller is responsible for checking the status of the request object to
                    // see if it is successful or not.
                    OnRequestCompleted((RequestCompletedEventArgs)arg);
                }, completedArgs);

                if (mAsyncOperation == null)
                    // Don't throw the exception if running on a thread. It can't be caught
                    // and makes it difficult to debug (cannot continue in Visual Studio).
                    throw;
            }
            finally
            {
                if (mAsyncOperation != null)
                {
                    using (ManualResetEvent done = new ManualResetEvent(false))
                    {
                        // Use the ManualResetEvent to ensure that the operation completes before
                        // we exit the method. 
                        // This is used to ensure that the Wait method will not continue until 
                        // after the final operation has been completed.
                        mAsyncOperation.PostOperationCompleted((arg) =>
                        {
                            mAsyncOperation = null;
                            done.Set();
                        }, null);
                        done.WaitOne();
                    }
                }
            }
            return whResponse;
        }

        /// <summary>
        /// Cancels the request in progress.
        /// </summary>
        public void CancelRequestAsync()
        {
            CancellationPending = true;
        }

        /// <summary>
        /// Waits for the request to complete.
        /// </summary>
        public void Wait()
        {
            if (mRequestThread == null) return;
            mRequestThread.Join();
        }

        /// <summary>
        /// Waits for the request to complete or until the timeout, whichever comes first.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns>True if the request has completed, false if the timeout was reached.</returns>
        public bool Wait(int timeout)
        {
            if (mRequestThread == null) return true;
            return mRequestThread.Join(timeout);
        }

        /// <summary>
        /// Processes the response from the server.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual WebHelperResponse ProcessResponse(HttpWebResponse response)
        {
            //todo: check to see if this is set yet.
            mResponseContentLength = response.ContentLength;

            using (var responseStream = response.GetResponseStream())
            {
                var processArgs = new ProcessResponseStreamEventArgs(responseStream, response, State);
                OnProcessResponseStream(processArgs);
                if (processArgs.Handled)
                    return new WebHelperResponse(processArgs.Result, response.ContentType, response.StatusCode, response.ContentEncoding, response.CharacterSet);

                var ms = new MemoryStream();
                var buffer = new byte[8192];
                var bytesRead = 0;
                int read;
                Stream s;

                if (response.ContentEncoding.ToLower().Contains("deflate"))
                    s = new DeflateStream(responseStream, CompressionMode.Decompress);
                else if (response.ContentEncoding.ToLower().Contains("gzip"))
                    s = new GZipStream(responseStream, CompressionMode.Decompress);
                else
                    s = responseStream;

                while ((read = s.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (CancellationPending) return null;

                    ms.Write(buffer, 0, read);
                    bytesRead += read;
                    ReportResponseProgress(bytesRead);
                }

                return new WebHelperResponse(ms.ToArray(), response.ContentType, response.StatusCode, response.ContentEncoding, response.CharacterSet);
            }

        }

        /// <summary>
        /// Creates the request that will be used to contact the server.
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        protected virtual HttpWebRequest CreateRequest(ContentType contentType)
        {
            var request = (HttpWebRequest)WebRequest.Create(contentType.GetUrl(this));

            request.Timeout = (int)Timeout.TotalMilliseconds;
            request.Headers = Headers;
            if (UseCompression)
                request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            if (!string.IsNullOrEmpty(UserAgent)) request.UserAgent = UserAgent;
            request.KeepAlive = KeepAlive;
            request.AllowAutoRedirect = AllowAutoRedirect;

            // let the content type update the request.
            contentType.PrepareRequest(request, this);

            // let the request be customized.
            OnPrepareRequest(new PrepareRequestEventArgs(request, State));

            if (request.ContentLength > 0)
                // Used to determine progress
                mRequestContentLength = request.ContentLength;

            return request;
        }

        /// <summary>
        /// Ensures that certain methods or events are called on the primary thread when running asynchronously.
        /// </summary>
        /// <param name="call"></param>
        /// <param name="arg"></param>
        private void Post(SendOrPostCallback call, object arg)
        {
            if (mAsyncOperation == null)
                call(arg);
            else
            {
                mAsyncOperation.Post((p) =>
                {
                    call(p);
                }, arg);
            }
        }

        private int mLastReqPct = -1;
        /// <summary>
        /// Used by ContentType object to report progress during send.
        /// </summary>
        /// <param name="bytesSent"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReportRequestProgress(long bytesSent)
        {
            var pct = (int)(((double)bytesSent / mRequestContentLength) * 100);
            if (mLastReqPct != pct)
            {
                // Only raise the progress changed event if the progress percent changed. It can become a performance issue if you raise it every time you write a few bytes.
                var progressArgs = new WebHelperProgressChangedEventArgs(mRequestContentLength, bytesSent, pct, EstimatedResponseLength ?? mRequestContentLength, 0, 0, State);
                Post((arg) =>
                {
                    OnProgressChanged((WebHelperProgressChangedEventArgs)arg);
                }, progressArgs);
                mLastReqPct = pct;
            }
        }

        private int mLastResPct = -1;
        /// <summary>
        /// Used by ContentType object to report progress during send.
        /// </summary>
        /// <param name="bytesRead"></param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ReportResponseProgress(long bytesRead)
        {
            var pct = (int)(((double)bytesRead / mResponseContentLength) * 100);
            if (mLastResPct != pct)
            {
                // Only raise the progress changed event if the progress percent changed. It can become a performance issue if you raise it every time you write a few bytes.
                var progressArgs = new WebHelperProgressChangedEventArgs(mRequestContentLength, mRequestContentLength, 100, mResponseContentLength, bytesRead, pct, State);
                Post((arg) =>
                {
                    OnProgressChanged((WebHelperProgressChangedEventArgs)arg);
                }, progressArgs);
                mLastResPct = pct;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Delegate for PrepareRequest event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void PrepareRequestHandler(object sender, PrepareRequestEventArgs e);
        /// <summary>
        /// Event raised before the request is made. Allows for customization of the request object before the request is sent. This event is raised on the calling thread. It is recommended that you do not update the UI in this event handler.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PrepareRequestHandler PrepareRequest;
        /// <summary>
        /// Raises the PrepareRequest event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPrepareRequest(PrepareRequestEventArgs e)
        {
            if (PrepareRequest == null) return;
            PrepareRequest(this, e);
        }

        /// <summary>
        /// Delegate for ProgressChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProgressChangedHandler(object sender, WebHelperProgressChangedEventArgs e);
        /// <summary>
        /// Event raised when the progress changes. This event is raised on the thread that made the request.
        /// </summary>
        public event ProgressChangedHandler ProgressChanged;
        /// <summary>
        /// Raises the ProgressChanged event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProgressChanged(WebHelperProgressChangedEventArgs e)
        {
            if (ProgressChanged == null) return;
            ProgressChanged(this, e);
        }

        /// <summary>
        /// Delegate for ProcessResponseStream event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ProcessResponseStreamHandler(object sender, ProcessResponseStreamEventArgs e);
        /// <summary>
        /// Event raised to allow you to process the response. Allows custom handling of the response. This event is raised on the calling thread. It is recommended that you do not update the UI in this event handler.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public event ProcessResponseStreamHandler ProcessResponseStream;
        /// <summary>
        /// Raises the ProcessResponseStream event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProcessResponseStream(ProcessResponseStreamEventArgs e)
        {
            if (ProcessResponseStream == null) return;
            ProcessResponseStream(this, e);
        }

        /// <summary>
        /// Delegate for the RequestCompleted event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void RequestCompletedHandler(object sender, RequestCompletedEventArgs e);
        /// <summary>
        /// Event raised when the request has been completed. This event is raised on the thread that made the request.
        /// </summary>
        public event RequestCompletedHandler RequestCompleted;
        /// <summary>
        /// Raises the RequestCompleted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnRequestCompleted(RequestCompletedEventArgs e)
        {
            if (RequestCompleted == null) return;
            RequestCompleted(this, e);
        }

        #endregion

    }

}
