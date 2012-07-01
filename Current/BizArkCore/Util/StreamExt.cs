using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace TourFactory.Core.Extensions.StreamExt
{

    /// <summary>
    /// Provides utility methods for handling streams.
    /// </summary>
    public static class StreamExt
    {

        /// <summary>
        /// Copies the stream to another stream.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        public static void Write(this Stream output, Stream input)
        {
            byte[] b = new byte[4096]; // copy 4k at a time.
            int r;
            while ((r = input.Read(b, 0, b.Length)) > 0)
                output.Write(b, 0, r);
        }

    }

}
