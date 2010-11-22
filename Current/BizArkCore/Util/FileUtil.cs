using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using BizArk.Core.StringExt;

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

        /// <summary>
        /// Gets a directory structure based on a number. For example, if the number passed in is 12345, 00/00/00/01/23 is passed back.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string GetIDDir(int id)
        {
            var dir = id.ToString();
            dir = ("000000000000" + dir).Right(12).Left(10); // get a string with 10 chars in it with the number being at the end (remove the last two chars).
            return string.Format("{0}/{1}/{2}/{3}/{4}", dir.Substring(0, 2), dir.Substring(2, 2), dir.Substring(4, 2), dir.Substring(6, 2), dir.Substring(8, 2));
        }

    }
}
