using BizArk.Core.Util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestBizArkCore
{
    
    
    /// <summary>
    ///This is a test class for MimeMapTest and is intended
    ///to contain all MimeMapTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MimeMapTest
    {

        [TestMethod()]
        public void GetMimeTypeTest()
        {
            var mimeType = MimeMap.GetMimeType(".TxT");
            Assert.AreEqual("text/plain", mimeType);
        }

        [TestMethod()]
        public void InvalidMimeTypeTest()
        {
            var mimeType = MimeMap.GetMimeType(".!@#");
            Assert.AreEqual(null, mimeType);
        }

    }
}
