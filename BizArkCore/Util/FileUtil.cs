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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Redwerb.BizArk.Core.Util
{

    /// <summary>
    /// Provides methods that are useful when working with files and directories.
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// Removes a directory as best as it can. Errors are ignored.
        /// </summary>
        /// <param name="dirPath"></param>
        public static void RemoveDirectory(string dirPath)
        {
            foreach (string childDirPath in Directory.GetDirectories(dirPath))
                RemoveDirectory(childDirPath);

            foreach (string filePath in Directory.GetFiles(dirPath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Unable to delete " + filePath + ": " + ex.Message);
                }
            }

            try
            {
                Directory.Delete(dirPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Unable to delete " + dirPath + ": " + ex.Message);
            }
        }
    }
}
