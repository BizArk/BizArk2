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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using Redwerb.BizArk.Core.DrawingExt;

namespace Redwerb.BizArk.Core.ImageExt
{
    /// <summary>
    /// Extension methods for images.
    /// </summary>
    public static class ImageExt
    {

        /// <summary>
        /// Saves the image to the temp directory and opens it in the default application.
        /// </summary>
        /// <param name="img"></param>
        public static bool Open(this Image img)
        {
            if (img == null) return false;

            string imgPath;
            if (img.RawFormat.Equals(ImageFormat.MemoryBmp))
            {
                // memory bitmaps cannot be saved, so convert it to a regular bitmap image.
                imgPath = GetUniqueFileName("bmp");
                img.Save(imgPath, ImageFormat.Bmp);
            }
            else
            {
                var ext = img.GetExtension();
                if (string.IsNullOrEmpty(ext)) return false;
                imgPath = GetUniqueFileName(ext);
                img.Save(imgPath);
            }

            Process.Start(imgPath);
            return true;
        }

        private static string GetUniqueFileName(string ext)
        {
            int i = 0;
            string path;
            string tempDir = Application.GetTempPath();
            do
            {
                i++;
                path = Path.Combine(tempDir, string.Format("{0}.{1}", i, ext));
            } while (File.Exists(path));
            return path;
        }

        /// <summary>
        /// Gets the default extension that can be used for the file name of the image.
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static string GetExtension(this Image img)
        {
            if (ImageFormat.Jpeg.Equals(img.RawFormat))
                return "jpg";
            else if (ImageFormat.Gif.Equals(img.RawFormat))
                return "gif";
            else if (ImageFormat.Bmp.Equals(img.RawFormat))
                return "bmp";
            else if (ImageFormat.Emf.Equals(img.RawFormat))
                return "emf";
            else if (ImageFormat.Exif.Equals(img.RawFormat))
                return "exif";
            else if (ImageFormat.Icon.Equals(img.RawFormat))
                return "ico";
            else if (ImageFormat.Png.Equals(img.RawFormat))
                return "png";
            else if (ImageFormat.Tiff.Equals(img.RawFormat))
                return "tif";
            else if (ImageFormat.Wmf.Equals(img.RawFormat))
                return "wmf";
            else
                return null;
        }

        /// <summary>
        /// Proportionally resizes an image.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Image Resize(this Image img, int maxWidth, int maxHeight)
        {
            var sz = img.Size.Resize(maxWidth, maxHeight);
            Bitmap result = new Bitmap(sz.Width, sz.Height);
            using (Graphics g = Graphics.FromImage((Image)result))
                g.DrawImage(img, 0, 0, sz.Width, sz.Height);
            return result;
        }

        /// <summary>
        /// Determines if the path is the path to an image file. Supports jpg, jpeg, gif, bmp, emf, exif, ico, png, tif, and wmf.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsImage(string path)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) return false;

            ext = ext.ToLower();
            switch (ext)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".bmp":
                case ".emf":
                case ".exif":
                case ".ico":
                case ".png":
                case ".tif":
                case ".wmf":
                    return true;
                default:
                    return false;
            }
        }
    }
}
