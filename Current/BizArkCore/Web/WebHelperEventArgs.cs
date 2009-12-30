using System;
using System.Net;
using System.IO;

namespace Redwerb.BizArk.Core.Web
{

    /// <summary>
    /// 
    /// </summary>
    public class RequestCompletedEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(WebHelperResponse response, object state, bool cancelled)
            : this(response, null, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(Exception ex, object state, bool cancelled)
            : this(null, ex, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(object state, bool cancelled)
            : this(null, null, state, cancelled)
        {
        }

        /// <summary>
        /// Creates an instance of RequestCompletedEventArgs.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ex"></param>
        /// <param name="state"></param>
        /// <param name="cancelled"></param>
        public RequestCompletedEventArgs(WebHelperResponse response, Exception ex, object state, bool cancelled)
        {
            mResponse = response;
            mError = ex;
            mState = state;
            mCancelled = cancelled;
        }

        #endregion

        #region Fields and Properties

        private WebHelperResponse mResponse;
        /// <summary>
        /// Gets the result.
        /// </summary>
        public WebHelperResponse Response
        {
            get { return mResponse; }
        }

        private Exception mError;
        /// <summary>
        /// Gets the error, if any, associated with the request.
        /// </summary>
        public Exception Error
        {
            get { return mError; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        private bool mCancelled = false;
        /// <summary>
        /// Gets a value that determines if the request was cancelled before it was completed.
        /// </summary>
        public bool Cancelled
        {
            get { return mCancelled; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class WebHelperProgressChangedEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of WebHelperProgressChangedEventArgs.
        /// </summary>
        /// <param name="bytesToSend"></param>
        /// <param name="bytesSent"></param>
        /// <param name="pctSent"></param>
        /// <param name="bytesToReceive"></param>
        /// <param name="bytesReceived"></param>
        /// <param name="pctReceived"></param>
        /// <param name="state"></param>
        public WebHelperProgressChangedEventArgs(long bytesToSend, long bytesSent, int pctSent, long bytesToReceive, long bytesReceived, int pctReceived, object state)
        {
            mBytesToSend = bytesToSend;
            mBytesSent = bytesSent;
            mSendProgressPercent = pctSent;
            mBytesToReceive = bytesToReceive;
            mBytesReceived = bytesReceived;
            mState = state;
        }

        #endregion

        #region Fields and Properties

        private long mBytesToSend;
        /// <summary>
        /// Gets the number of bytes to send.
        /// </summary>
        public long BytesToSend
        {
            get { return mBytesToSend; }
        }

        private long mBytesSent;
        /// <summary>
        /// Gets the number of bytes sent.
        /// </summary>
        public long BytesSent
        {
            get { return mBytesSent; }
        }

        private int mSendProgressPercent;
        /// <summary>
        /// Gets the progress for the request.
        /// </summary>
        public int SendProgressPercent
        {
            get { return mSendProgressPercent; }
        }

        private long mBytesReceived;
        /// <summary>
        /// Gets the number of bytes received.
        /// </summary>
        public long BytesReceived
        {
            get { return mBytesReceived; }
        }

        private long mBytesToReceive;
        /// <summary>
        /// Gets the number of bytes in the response. If the response hasn't been sent yet, this is an estimate based on WebHelper.EstimatedResponseLength.
        /// </summary>
        public long BytesToReceive
        {
            get { return mBytesToReceive; }
        }

        /// <summary>
        /// Gets the progress for the response.
        /// </summary>
        public int ResponseProgressPercent
        {
            get
            {
                if (mBytesReceived == 0) return 0;
                if (mBytesToReceive == 0) return 0;
                return (int)(((double)mBytesReceived / mBytesToReceive) * 100);
            }
        }

        /// <summary>
        /// Gets the progress for the entire request/response. If a response has not been sent yet, this is an estimate based on WebHelper.EstimatedResponseLength.
        /// </summary>
        public int TotalProgressPercent
        {
            get
            {
                var bytesMoved = mBytesSent + mBytesReceived;
                var bytesToMove = mBytesToSend + mBytesToReceive;
                return (int)(((double)bytesMoved / bytesToMove) * 100);
            }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class PrepareRequestEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of PrepareRequestEventArgs.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="state"></param>
        public PrepareRequestEventArgs(HttpWebRequest request, object state)
        {
            mRequest = request;
            mState = state;
        }

        #endregion

        #region Fields and Properties

        private HttpWebRequest mRequest;
        /// <summary>
        /// Gets the HttpWebRequest that can be modified prior to sending it to the server.
        /// </summary>
        public HttpWebRequest Request
        {
            get { return mRequest; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class ProcessResponseStreamEventArgs
        : EventArgs
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of ProcessResponseStreamEventArgs.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="response"></param>
        /// <param name="state"></param>
        public ProcessResponseStreamEventArgs(Stream s, HttpWebResponse response, object state)
        {
            mResponseStream = s;
            mResponse = response;
            mState = state;
        }

        #endregion

        #region Fields and Properties

        private Stream mResponseStream;
        /// <summary>
        /// Gets the stream associated with the response.
        /// </summary>
        public Stream ResponseStream
        {
            get { return mResponseStream; }
        }

        private HttpWebResponse mResponse;
        /// <summary>
        /// Gets the response.
        /// </summary>
        public HttpWebResponse Response
        {
            get { return mResponse; }
        }

        private bool mHandled = false;
        /// <summary>
        /// Gets or sets a value that determines if the stream has been processed. Prevents the helper from processing the stream.
        /// </summary>
        public bool Handled
        {
            get { return mHandled; }
            set { mHandled = value; }
        }

        private object mResult;
        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public object Result
        {
            get { return mResult; }
            set { mResult = value; }
        }

        private object mState;
        /// <summary>
        /// Gets the state associated with the request.
        /// </summary>
        public object State
        {
            get { return mState; }
        }

        #endregion

    }

}