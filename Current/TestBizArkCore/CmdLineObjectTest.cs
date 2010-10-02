using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizArk.Core.CmdLine;
using BizArk.Core.StringExt;
using BizArk.Core.WebExt;
using BizArk.Core.Util;
using System.IO;

namespace TestBizArkCore
{

    /// <summary>
    ///This is a test class for CmdLineObjectTest and is intended
    ///to contain all CmdLineObjectTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CmdLineObjectTest
    {

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void InitializeTest()
        {
            MyTestCmdLineObject target;
            string[] args;

            target = new MyTestCmdLineObject();
            args = new string[] { };
            target.InitializeFromCmdLine(args);
            Assert.IsNull(target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/H", "Hi Brian!" };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/h", "Hi Brian!" };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/h", "Hi Brian!", "/G", "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.AreEqual<string>("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "Hi Brian!", "/G", "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.AreEqual<string>("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject2();
            args = new string[] { "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
            Assert.IsNull(target.Hello);
            Assert.AreEqual<string>("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "TEST", "/h", "Hi Brian!", "/G", "Goodbye Christine.", "TEST" };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.AreEqual<string>("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/I" };
            target.InitializeFromCmdLine(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new string[] { "/I-" };
            target.InitializeFromCmdLine(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new string[] { "/I", "Yes" };
            target.InitializeFromCmdLine(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new string[] { "/I", "No" };
            target.InitializeFromCmdLine(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new string[] { "/S", "Cars", "Computers", "Food" };
            target.InitializeFromCmdLine(args);
            string[] expectedStuff = new string[] { "Cars", "Computers", "Food" };
            AssertEx.AreEqual(expectedStuff, target.StuffILike);
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void DefaultPropTest()
        {
            MyTestCmdLineObject3 target;
            string[] args;
            string[] expected;

            target = new MyTestCmdLineObject3();
            args = new string[] { "Brian", "Christine", "Abrian", "Brooke" };
            target.InitializeFromCmdLine(args);
            expected = new string[] { "Brian", "Christine", "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Family);

            target = new MyTestCmdLineObject3();
            args = new string[] { "Brian", "Christine", "Abrian", "Brooke", "/D", "Brian" };
            target.InitializeFromCmdLine(args);
            expected = new string[] { "Brian", "Christine", "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Family);
            Assert.AreEqual("Brian", target.Father);
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [TestMethod()]
        public void PartialNameTest()
        {
            MyTestCmdLineObject target;
            string[] args;

            target = new MyTestCmdLineObject();
            args = new string[] { "/Hell", "Hi Brian" };
            target.InitializeFromCmdLine(args);
            Assert.AreEqual("Hi Brian", target.Hello);
        }

        [TestMethod()]
        public void CmdLineDescriptionTest()
        {
            string test = "This is a test\ntest  test test";
            string[] lines = test.Lines();
            foreach (string line in lines)
                Debug.WriteLine(line);

            MyTestCmdLineObject target;
            target = new MyTestCmdLineObject();
            target.InitializeEmpty();
            Debug.WriteLine(target.GetHelpText(50));
        }

        [TestMethod()]
        public void SaveAndRestoreTest()
        {
            var settingsPath = @"C:\garb\Test.xml";
            if (!Directory.Exists(@"C:\garb"))
                Directory.CreateDirectory(@"C:\garb");
            var cmdLine = new MyTestCmdLineObject();
            var stuffILike = new string[] { "Cookies", "Candy", "Ice Cream" };
            Assert.AreNotEqual("Hi", cmdLine.Hello);
            cmdLine.InitializeFromCmdLine("/H", "Hi", "/S", "Cookies", "Candy", "Ice Cream");
            Assert.AreEqual("Hi", cmdLine.Hello);
            AssertEx.AreEqual(stuffILike, cmdLine.StuffILike);
            cmdLine.SaveToXml(settingsPath);
            Assert.IsTrue(System.IO.File.Exists(settingsPath));

            cmdLine = new MyTestCmdLineObject();
            Assert.AreNotEqual("Hi", cmdLine.Hello);
            cmdLine.RestoreFromXml(settingsPath);
            Assert.AreEqual("Hi", cmdLine.Hello);
            AssertEx.AreEqual(stuffILike, cmdLine.StuffILike);
        }

        [TestMethod]
        public void QueryStringTest()
        {
            MyTestCmdLineObject target;
            string args;

            target = new MyTestCmdLineObject();
            args = "";
            target.InitializeFromQueryString(args);
            Assert.IsNull(target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("H={0}", "Hi Brian!".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}", "Hi Brian!".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}&G={1}", "Hi Brian!".UrlEncode(), "Goodbye Christine.".UrlEncode());
            target.InitializeFromQueryString(args);
            Assert.AreEqual<string>("Hi Brian!", target.Hello);
            Assert.AreEqual<string>("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = "i=true";
            target.InitializeFromQueryString(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "I=false";
            target.InitializeFromQueryString(args);
            Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "i=Yes";
            target.InitializeFromQueryString(args);
            Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = "i=No";
            target.InitializeFromQueryString(args);
            Assert.IsFalse(target.DoesLikeIceCream);
        }

        [TestMethod]
        public void AliasTest()
        {
            // full alias
            var cmdLine = new MyTestCmdLineObject();
            cmdLine.InitializeFromCmdLine(new string[] { "/crap", "Christine" });
            Assert.AreEqual("Christine", cmdLine.StuffILike[0]);

            // partial alias
            cmdLine = new MyTestCmdLineObject();
            cmdLine.InitializeFromCmdLine(new string[] { "/c", "Christine" });
            Assert.AreEqual("Christine", cmdLine.StuffILike[0]);
        }

    }

    [CmdLineDefaultArg("Hello")]
    internal class MyTestCmdLineObject
        : CmdLineObject
    {
        [CmdLineArg(Alias = "H")]
        [System.ComponentModel.Description("Says hello to user.")]
        public string Hello { get; set; }

        [CmdLineArg(Alias = "G")]
        [System.ComponentModel.Description("Says goodbye to user.")]
        public string Goodbye { get; set; }

        [CmdLineArg(Alias = "I")]
        [System.ComponentModel.Description("Determines whether user likes ice cream or not.")]
        public bool DoesLikeIceCream { get; set; }

        [CmdLineArg(Aliases = new string[] { "S", "Stuff", "Crap" })]
        [System.ComponentModel.Description("List of things the user likes.")]
        public string[] StuffILike { get; set; }
    }

    [CmdLineDefaultArg("g")]
    internal class MyTestCmdLineObject2
        : MyTestCmdLineObject
    {
    }

    [CmdLineDefaultArg("F")]
    internal class MyTestCmdLineObject3
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }
}
