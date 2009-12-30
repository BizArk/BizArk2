using System.Drawing;

namespace Redwerb.BizArk.Core.DrawingExt
{

    /// <summary>
    /// Extensions for classes within the Drawing namespace.
    /// </summary>
    public static class DrawingExt
    {
        /// <summary>
        /// Proportionally resizes a Size structure.
        /// </summary>
        /// <param name="sz"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        public static Size Resize(this Size sz, int maxWidth, int maxHeight)
        {
            int height = sz.Height;
            int width = sz.Width;

            double actualRatio = (double)width / (double)height;
            double maxRatio = (double)maxWidth / (double)maxHeight;
            double resizeRatio;

            if (actualRatio > maxRatio)
                // width is the determinate side.
                resizeRatio = (double)maxWidth / (double)width;
            else
                // height is the determinate side.
                resizeRatio = (double)maxHeight / (double)height;

            width = (int)(width * resizeRatio);
            height = (int)(height * resizeRatio);

            return new Size(width, height);
        }
    }
}