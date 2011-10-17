using BizArk.Core.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace TestBizArkCore
{
    
    
    /// <summary>
    ///This is a test class for UploadFileTest and is intended
    ///to contain all UploadFileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UploadFileTest
    {

        [TestMethod()]
        public void op_ExplicitTest()
        {
            var fi = new FileInfo(@"C:\Test.txt");
            var file = (UploadFile)fi;
            Assert.IsNotNull(file);
            Assert.AreEqual(fi.FullName, file.FilePath);
        }

    }
}
