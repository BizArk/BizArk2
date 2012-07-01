using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.FormatExt;
using BizArk.Core.Util;
using BizArk.Web.Utils;

namespace BizArk.Web.MVC.ActionResults
{

    /// <summary>
    /// Sends the contents of the file to the response. Handles cacheing and byte ranges.
    /// </summary>
    public class FilePathExResult : FilePathResult
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of FilePathExResult.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        public FilePathExResult(string fileName, string contentType)
            : base(fileName, contentType)
        {
            // Copy the default options so that if somebody changes them, they don't change the default.
            Options = new FilePathExResultOptions(FilePathExResultOptions.Default);
        }

        #endregion

        #region Fields and Properties

        // Generated in HandleETag, used in HandleRange.
        private string mETag;

        /// <summary>
        /// Gets the options for handling files.
        /// </summary>
        public FilePathExResultOptions Options { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Checks to see if the file cached on the client is the same as on the server using ETags.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>True if the cached client file is the same as the file on the server.</returns>
        protected bool HandleETag(HttpResponseBase response)
        {
            if (Options.ETagGenerator == null) return false;

            using (var fs = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                mETag = Options.ETagGenerator.GenerateEtag(fs);
            if (string.IsNullOrEmpty(mETag)) return false;
            response.Cache.SetETag(mETag);

            var request = HttpContext.Current.Request;
            var lastETag = request.Headers["If-None-Match"];
            if (string.IsNullOrEmpty(lastETag)) return false;
            if (lastETag != mETag) return false;

            response.StatusCode = 304;
            response.StatusDescription = "Not Modified";
            return true;
        }

        /// <summary>
        /// Handles cacheing based on the last modified date of the file.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected bool HandleLastModified(HttpResponseBase response)
        {
            if (!Options.SupportLastModifiedCaching) return false;

            var lastFileMod = File.GetLastWriteTime(this.FileName);
            // Setting response.Cache.SetLastModified to a date in the future will throw an exception.
            // This can happen in certain edge conditions.
            if (lastFileMod < DateTime.Now) lastFileMod = DateTime.Now;

            // This date is sent back in the If-Modified-Since header.
            response.Cache.SetLastModified(lastFileMod);

            var request = HttpContext.Current.Request;
            var lastClientModStr = request.Headers["If-Modified-Since"];
            if (string.IsNullOrEmpty(lastClientModStr)) return false;

            DateTime lastClientMod;
            if (!DateTime.TryParseExact(lastClientModStr, "r", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out lastClientMod)) return false;
            if (lastFileMod != lastClientMod) return false;

            response.StatusCode = 304;
            response.StatusDescription = "Not Modified";
            return true;
        }

        /// <summary>
        /// If cacheing is supported, this will handle it.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual bool HandleCacheing(HttpResponseBase response)
        {
            response.Cache.SetCacheability(Options.Cacheability);

            if (HandleETag(response)) return true;
            if (HandleLastModified(response)) return true;
            if (Options.MaxAge.HasValue)
                response.Cache.SetMaxAge(Options.MaxAge.Value);
            
            return false;
        }

        /// <summary>
        /// If a range is specified, this will handle it.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected virtual bool HandleRange(HttpResponseBase response)
        {
            if (!Options.SupportByteRange) return false;

            response.AddHeader("Accept-Ranges", "bytes");

            var request = HttpContext.Current.Request;
            var range = ParseRange(request.Headers["Range"]);
            if (range == null) return false;

            var fi = new FileInfo(FileName);
            var end = Math.Min(range.End, fi.Length - 1); // last byte is at 1 less than the length (0 based);
            var length = end - range.Start + 1; // the start is 0 based and end is inclusive. Eg, a range of 0-0 would be 1 byte.

            var ifrange = request.Headers["If-Range"];
            if (ifrange.HasValue())
            {
                if (mETag != ifrange) return false;
            }

            response.StatusCode = 206;
            response.StatusDescription = "Partial Content";
            response.AddHeader("Content-Range", "bytes {0}-{1}/{2}".Fmt(range.Start, end, fi.Length));
            response.TransmitFile(FileName, range.Start, end);
            return true;
        }

        private Range<long> ParseRange(string rangeStr)
        {
            if (rangeStr.IsEmpty()) return null;
            if (!rangeStr.StartsWith("bytes=", StringComparison.InvariantCultureIgnoreCase)) return null;

            rangeStr = rangeStr.Substring("bytes=".Length);
            var i = rangeStr.IndexOf('-');
            if (i <= 0) return null;

            long start;
            long end;
            if (!long.TryParse(rangeStr.Substring(0, i), out start)) return null;
            if (!long.TryParse(rangeStr.Substring(i + 1), out end)) return null;
            if (start > end) return null;

            return new Range<long>(start, end);
        }

        /// <summary>
        /// Writes the range of the file specified in the request to the response if it's not cached by the client.
        /// </summary>
        /// <param name="response"></param>
        protected override void WriteFile(HttpResponseBase response)
        {
            if (HandleCacheing(response)) return;
            if (HandleRange(response)) return;
            base.WriteFile(response);
        }

        #endregion

    }

    /// <summary>
    /// Provides options for handling files in FilePathExResult.
    /// </summary>
    public class FilePathExResultOptions
    {

        /// <summary>
        /// Creates an instance of FilePathExResultOptions.
        /// </summary>
        public FilePathExResultOptions()
        {
            Cacheability = HttpCacheability.ServerAndPrivate;
            SupportLastModifiedCaching = false;
            SupportByteRange = true;
            ETagGenerator = ETagGenerators.MD5;
        }

        /// <summary>
        /// Creates an instance of FilePathExResultOptions.
        /// </summary>
        internal FilePathExResultOptions(FilePathExResultOptions options)
        {
            Cacheability = options.Cacheability;
            SupportLastModifiedCaching = options.SupportLastModifiedCaching;
            SupportByteRange = options.SupportByteRange;
            ETagGenerator = options.ETagGenerator;
        }

        /// <summary>
        /// Gets or sets the value for the Cache-Control header. Defaults to ServerAndPrivate.
        /// </summary>
        public HttpCacheability Cacheability { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if Last-Modified date caching is used. Defaults to false.
        /// </summary>
        /// <remarks>
        /// Using the last modified date is a quick and easy way to allow for client side
        /// caching. If the client supports the Last-Modified header and it has a copy
        /// cached, it will send the Last-Modified date back to the server to compare
        /// with the version on the server. If they are the same, the server will return
        /// an empty response with a http status code of 304.
        /// </remarks>
        public bool SupportLastModifiedCaching { get; set; }

        /// <summary>
        /// Gets or sets the generator for the ETag. Defaults to ETagGenerators.MD5. Set to null to disable ETags.
        /// </summary>
        /// <remarks>
        /// Using the ETag requires more processing on the server since the entire file
        /// must be read into memory in order to generate the ETag, but this method is
        /// very accurate and is not confused by updated last-modified dates if the file
        /// hasn't actually changed. 
        /// </remarks>
        public IETagGenerator ETagGenerator { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if byte range requests are supported. Defaults to false.
        /// </summary>
        public bool SupportByteRange { get; set; }

        /// <summary>
        /// Gets or sets the maximum age for the content.
        /// </summary>
        public TimeSpan? MaxAge { get; set; }

        static FilePathExResultOptions()
        {
            Default = new FilePathExResultOptions();
        }

        /// <summary>
        /// Gets or sets the default options for FilePathExResult.
        /// </summary>
        public static FilePathExResultOptions Default { get; set; }

    }

}
