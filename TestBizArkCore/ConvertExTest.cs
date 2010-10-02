using BizArk.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Data;
namespace TestBizArkCore
{


    /// <summary>
    ///This is a test class for ConvertExTest and is intended
    ///to contain all ConvertExTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConvertExTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for IsEqual
        ///</summary>
        [TestMethod()]
        [DeploymentItem("BizArk.Core.dll")]
        public void IsEmptyTest()
        {
            object value = null;

            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = "test";
            Assert.IsFalse(ConvertEx.IsEmpty(value));

            value = "";
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = int.MinValue;
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = int.MaxValue;
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = 0;
            Assert.IsFalse(ConvertEx.IsEmpty(value));

            value = new char();
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = DateTime.MinValue;
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = DateTime.Parse("7/4/2008");
            Assert.IsFalse(ConvertEx.IsEmpty(value));

            value = String.Empty;
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = ConvertTest.Empty;
            Assert.IsTrue(ConvertEx.IsEmpty(value));

            value = new ConvertTest() { X = 5, Y = 10 };
            Assert.IsFalse(ConvertEx.IsEmpty(value));

            value = new ConvertTest() { X = 0, Y = 0 };
            Assert.IsTrue(ConvertEx.IsEmpty(value));
        }

        [TestMethod()]
        public void ChangeTypeTest()
        {
            object value;

            value = ConvertEx.ChangeType<DateTime>("7/4/2008");
            Assert.AreEqual(DateTime.Parse("7/4/2008"), value);

            var test = ConvertEx.ChangeType<ConvertTest>("1,2");
            Assert.AreEqual(1, test.X);
            Assert.AreEqual(2, test.Y);
        }

        [TestMethod()]
        public void GetDefaultEmptyTest()
        {
            object emptyValue;

            emptyValue = ConvertEx.GetDefaultEmptyValue<object>();
            Assert.IsNull(emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<string>();
            Assert.IsNull(emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<int>();
            Assert.AreEqual(int.MinValue, emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<char>();
            Assert.AreEqual('\0', emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<DateTime>();
            Assert.AreEqual(DateTime.MinValue, emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<ConvertTest>();
            Assert.IsNull(emptyValue);

            emptyValue = ConvertEx.GetDefaultEmptyValue<ConvertStructTest>();
            Assert.AreEqual(ConvertStructTest.Empty, emptyValue);

        }

        [TestMethod()]
        public void InheritanceConversionTest()
        {
            var test = new ConvertTest();
            var btest = ConvertEx.ChangeType<ConvertTestBase>(test);
            Assert.AreSame(test, btest);
        }

        [TestMethod()]
        public void TypeCtorTest()
        {
            var pt = new Point(5, 10);
            var test = ConvertEx.ChangeType<ConvertTest>(pt);
            Assert.AreEqual(pt.X, test.X);
            Assert.AreEqual(pt.Y, test.Y);
        }

        [TestMethod()]
        public void SqlDbTypeConversionTest()
        {
            var val = ConvertEx.ChangeType<SqlDbType>(typeof(int));
            Assert.AreEqual(SqlDbType.Int, val);

            val = ConvertEx.ChangeType<SqlDbType>(typeof(bool));
            Assert.AreEqual(SqlDbType.Bit, val);

            val = ConvertEx.ChangeType<SqlDbType>(typeof(byte[]));
            Assert.AreEqual(SqlDbType.Binary, val);
        }

        private class ConvertTestBase
        {
            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object obj)
            {
                var other = obj as ConvertTest;
                if (other == null) return false;
                if (this.X != other.X) return false;
                if (this.Y != other.Y) return false;
                return true;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        private class ConvertTest
            : ConvertTestBase
        {
            public ConvertTest()
            {
            }

            public ConvertTest(Point pt)
            {
                X = pt.X;
                Y = pt.Y;
            }

            public static ConvertTest Parse(string s)
            {
                var test = new ConvertTest();
                var vals = s.Split(',');
                test.X = ConvertEx.ChangeType<int>(vals[0]);
                test.Y = ConvertEx.ChangeType<int>(vals[1]);
                return test;
            }

            public static readonly ConvertTest Empty = new ConvertTest() { X = 0, Y = 0 };
        }

        private struct ConvertStructTest
        {
            public static readonly ConvertStructTest Empty = new ConvertStructTest() { X = 0, Y = 0 };

            public int X { get; set; }
            public int Y { get; set; }

            public override bool Equals(object obj)
            {
                var other = (ConvertStructTest)obj;
                if (this.X != other.X) return false;
                if (this.Y != other.Y) return false;
                return true;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
    }
}
