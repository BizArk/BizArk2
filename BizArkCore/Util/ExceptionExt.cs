/* Author: Brian Brewder
 * Website: http://redwerb.com
 * 
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. 
 */

using System;
using System.Text;

namespace Redwerb.BizArk.Core.ExceptionExt
{

    /// <summary>
    /// Extensions for classes within the Drawing namespace.
    /// </summary>
    public static class ExceptionExt
    {
        /// <summary>
        /// Gets the details of an exception suitable for display.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDetails(this Exception ex)
        {
            var details = new StringBuilder();

            while (ex != null)
            {
                details.AppendLine(ex.GetType().FullName);
                details.AppendLine(ex.Message);
                details.AppendLine(ex.StackTrace);

                ex = ex.InnerException;
                if (ex != null)
                {
                    details.AppendLine();
                    details.AppendLine(new string('#', 70));
                }
            }

            return details.ToString();
        }
    }
}