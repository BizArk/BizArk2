using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizArk.DB
{

    /// <summary>
    /// Exception thrown if there is a problem with save.
    /// </summary>
    public class DatabaseSaveException : ApplicationException
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of DatabaseSaveException.
        /// </summary>
        /// <param name="message"></param>
        public DatabaseSaveException(string message)
            : base(message)
        {
        }

        #endregion

    }
}
