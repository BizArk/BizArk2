using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BizArk.Web.Utils
{

    /// <summary>
    /// Generates ETags based on a stream.
    /// </summary>
    public interface IETagGenerator
    {

        /// <summary>
        /// Generates the ETag.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        string GenerateEtag(Stream s);

    }

    /// <summary>
    /// Base class for ETag generators that use System.Security.Cryptography.HashAlgorithm.
    /// </summary>
    public abstract class CryptoETagHGenerator : IETagGenerator
    {
        /// <summary>
        /// Generates the ETag.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public abstract string GenerateEtag(Stream s);

        /// <summary>
        /// Uses the HashAlgorithm to get the hash of the stream.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="hasher"></param>
        /// <returns></returns>
        protected string GetHash(Stream s, HashAlgorithm hasher)
        {
            var hash = hasher.ComputeHash(s);
            // = is a valueless character that is used for padding at the end (http://msdn.microsoft.com/en-us/library/system.convert.frombase64string.aspx).
            // We strip it off because the base64 string is not intended to be reversed.
            return System.Convert.ToBase64String(hash).TrimEnd('=');
        }

    }

    /// <summary>
    /// Creates an ETag based on an MD5 hash of the file contents.
    /// </summary>
    public class MD5ETagGenerator : CryptoETagHGenerator
    {
        /// <summary>
        /// Generates the ETag.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override string GenerateEtag(Stream s)
        {
            using (var md5 = new MD5CryptoServiceProvider())
                return GetHash(s, md5);
        }
    }

    /// <summary>
    /// Creates an ETag based on an SHA1 hash of the file contents.
    /// </summary>
    public class SHA1ETagGenerator : CryptoETagHGenerator
    {
        /// <summary>
        /// Generates the ETag.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public override string GenerateEtag(Stream s)
        {
            using (var md5 = new SHA1CryptoServiceProvider())
                return GetHash(s, md5);
        }
    }

    /// <summary>
    /// Provides ETag generators.
    /// </summary>
    public static class ETagGenerators
    {

        private static MD5ETagGenerator sMD5;
        /// <summary>
        /// Gets an instance of the MD5ETagGenerator.
        /// </summary>
        public static MD5ETagGenerator MD5
        {
            get
            {
                if (sMD5 == null)
                    sMD5 = new MD5ETagGenerator();
                return sMD5;
            }
        }

        private static SHA1ETagGenerator sSHA1;
        /// <summary>
        /// Gets an instance of the SHA1ETagGenerator.
        /// </summary>
        public static SHA1ETagGenerator SHA1
        {
            get
            {
                if (sSHA1 == null)
                    sSHA1 = new SHA1ETagGenerator();
                return sSHA1;
            }
        }

    }

}
