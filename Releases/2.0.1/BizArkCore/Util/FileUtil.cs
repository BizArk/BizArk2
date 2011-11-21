using System;
using System.Diagnostics;
using System.IO;
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


        /// <summary>
        /// Creates a unique file name in the given directory. 
        /// </summary>
        /// <param name="dir">The path to the directory.</param>
        /// <param name="template">The template for the file name. Place a {0} where the counter should go (ex, MyPicture{0}.jpg).</param>
        /// <returns>The full path to the unique file name.</returns>
        public static string GetUniqueFileName(string dir, string template)
        {
            //todo: make sure the path is valid (fix the template)

            string path;
            path = Path.Combine(dir, string.Format(template, ""));
            var i = 2; // this will be the second version of the file name, the first version doesn't include a number but is effectively 1.
            while (File.Exists(path))
            {
                path = Path.Combine(dir, string.Format(template, i));
                i++;
            }
            return path;
        }

        /// <summary>
        /// Strips illegal characters from a potential file name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Legal file name.</returns>
        public static string GetLegalFileName(string name)
        {
            var newName = name;
            foreach (char ch in Path.GetInvalidFileNameChars())
                newName = newName.Replace(ch.ToString(), "");
            return newName;
        }

        /// <summary>
        /// Creates a unique file name in the temp directory.
        /// </summary>
        /// <param name="ext">The extension for the file.</param>
        /// <returns>The full path to the unique file name.</returns>
        public static string GetUniqueFileName(string ext)
        {
            var tempDir = Application.GetTempPath();
            string path;
            do
            {
                var id = Guid.NewGuid();
                path = Path.Combine(tempDir, string.Format("{0}.{1}", id, ext));
            } while (File.Exists(path));
            return path;
        }

        /// <summary>
        /// Creates a unique file name in the temp directory. The file will have an extension of .tmp.
        /// </summary>
        /// <returns>The full path to the unique file name.</returns>
        public static string GetUniqueFileName()
        {
            return GetUniqueFileName("tmp");
        }


        /// <summary>
        /// Deletes a file. If unable to delete will return false.
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            // Somebody wanted this file gone and it doesn't exist, done.
            if (!File.Exists(path)) return;

            var fi = new FileInfo(path);
            fi.Attributes = FileAttributes.Normal; // do what we can to delete the file.
            File.Delete(path);

            if (File.Exists(path))
                throw new ApplicationException(string.Format("Unable to delete {0}. No exceptions thrown but the file still exists.", path));
        }
    }
}
