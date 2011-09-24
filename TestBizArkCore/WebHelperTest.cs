using BizArk.Core.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Dynamic;
using System.Collections.Generic;
using BizArk.Core;
using Microsoft.CSharp.RuntimeBinder;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace TestBizArkCore
{


    /// <summary>
    ///This is a test class for NoContentTypeTest and is intended
    ///to contain all NoContentTypeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class WebHelperTest
    {

        //[TestMethod]
        //public void WebParameterTest()
        //{
        //    dynamic parameters = new WebParameters();
        //    parameters.Test = 5;
        //    Assert.AreEqual("5", parameters.Test);
        //    AssertEx.Throws(typeof(RuntimeBinderException), () => { var x = parameters.Test2; });

        //    var txtParam = parameters.Find("Test") as WebTextParameter;
        //    Assert.IsNotNull(txtParam);
        //    Assert.AreEqual(txtParam.Name, "Test");
        //    Assert.AreEqual(txtParam.Value, "5");

        //    parameters.MyFile = new FileInfo(@"C:\MyFile.txt");
        //    var fileParam = parameters.Find("MyFile") as WebFileParameter;
        //    Assert.IsNotNull(fileParam);
        //    Assert.AreEqual(@"C:\MyFile.txt", fileParam.File.FilePath);

        //    parameters.MyData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        //    var binParam = parameters.Find("MyData") as WebBinaryParameter;
        //    Assert.IsNotNull(binParam);
        //    Assert.AreEqual(10, binParam.Data.Length);
        //    Assert.AreEqual(0, binParam.Data[0]);
        //    Assert.AreEqual(9, binParam.Data[9]);
        //}

        //[TestMethod()]
        //public void NoContentTypeUrlTest()
        //{
        //    var contentType = new NoContentType(new WebParameters());
        //    var helper = new WebHelper("http://redwerb.com");
        //    var url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    dynamic parameters = new WebParameters();
        //    parameters.Test = "hello";
        //    contentType = ContentType.CreateContentType(HttpMethod.Get, parameters) as NoContentType;
        //    Assert.IsNotNull(contentType);
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?Test=hello", url);

        //    helper = new WebHelper("http://redwerb.com?hello=world");
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?hello=world&Test=hello", url);
        //}

        //[TestMethod()]
        //public void ContentTypeUrlTest()
        //{
        //    var helper = new WebHelper("http://redwerb.com");
        //    var contentType = new ApplicationUrlEncodedContentType(helper.Parameters);

        //    var url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    helper.Parameters.Test = "hello";
        //    contentType = ContentType.CreateContentType(HttpMethod.Post, helper.Parameters) as ApplicationUrlEncodedContentType;
        //    Assert.IsNotNull(contentType);
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com", url);

        //    helper = new WebHelper("http://redwerb.com?hello=world");
        //    url = contentType.GetUrl(helper);
        //    Assert.AreEqual("http://redwerb.com?hello=world", url);
        //}

        //[TestMethod]
        //public void CompressedResponseTest()
        //{
        //    // should test deflate compression.
        //    var helper = new WebHelper("http://redwerb.com");
        //    var response = helper.MakeRequest();
        //    var result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<h2>Tools, tips, and techniques for developers</h2>"));

        //    // should test gzip compression.
        //    helper = new WebHelper("http://bing.com");
        //    helper.Parameters.q = "redwerb";
        //    response = helper.MakeRequest();
        //    result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<title>redwerb - Bing</title>"));

        //    // should test no compression.
        //    helper = new WebHelper("http://redwerb.com");
        //    helper.UseCompression = false;
        //    response = helper.MakeRequest();
        //    result = response.ResultToString();
        //    Assert.IsTrue(result.Contains("<h2>Tools, tips, and techniques for developers</h2>"));
        //}

        //[TestMethod]
        //public void UploadFileTest()
        //{
        //    // This test only works when the test web project is running (not included in the BizArk test suite).
        //    if (false)
        //    {
        //        var helper = new WebHelper("http://localhost:49745/API/File/Upload");
        //        helper.Parameters.Name = "TestFile";
        //        helper.Parameters.Directory = "TestDir";
        //        helper.Parameters.File = new UploadFile("text/plain", @"D:\Test.txt");
        //        var response = helper.MakeRequest();
        //        var content = response.ResultToString();
        //        Assert.AreEqual("hello world!", content);
        //    }
        //}

        [TestMethod]
        public void UploadFileTest()
        {
            var str = WebHelper2.DownloadString("http://www.google.com");
            Assert.IsFalse(string.IsNullOrWhiteSpace(str));
        }

    }

}
