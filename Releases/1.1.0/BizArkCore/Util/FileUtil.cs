using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace BizArk.Core.Util
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
