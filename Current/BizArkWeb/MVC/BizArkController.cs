using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using BizArk.Core.Util;
using BizArk.Web.MVC.ActionResults;

namespace BizArk.Web.MVC
{

    /// <summary>
    /// Provides additional functionality over the base MVC Controller for handling Http requests and responses.
    /// </summary>
    public abstract class BizArkController : Controller
    {

        /// <summary>
        /// Creates a FilePathResult object by using the path to the file. Gets the content type from MimeMap.GetMimeType based on the files extension.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public FileResult File(string fileName)
        {
            return base.File(fileName, MimeMap.GetMimeType(Path.GetExtension(fileName)));
        }

        /// <summary>
        /// Creates a FilePathExResult object for the file. This will handle cacheing and byte range requests.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="contentType"></param>
        /// <param name="fileDownloadName"></param>
        /// <returns></returns>
        protected override FilePathResult File(string fileName, string contentType, string fileDownloadName)
        {
            return new FilePathExResult(fileName, contentType) { FileDownloadName = fileDownloadName };
        }

    }
}
