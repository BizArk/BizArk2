using BizArk.Core.FormatExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestBizArkCore
{
    
    
    /// <summary>
    ///This is a test class for FormatExtTest and is intended
    ///to contain all FormatExtTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FormatExtTest
    {
        
        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtIntTest()
        {
            Assert.AreEqual("1", 1.Fmt());
            Assert.AreEqual("1,000", 1000.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtNullIntTest()
        {
            int? i = null;
            Assert.AreEqual("", i.Fmt());
            i = 1;
            Assert.AreEqual("1", i.Fmt());
            i = 1000;
            Assert.AreEqual("1,000", i.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtDecimalTest()
        {
            Assert.AreEqual("1.00", 1M.Fmt());
            Assert.AreEqual("1,000.00", 1000M.Fmt());
            Assert.AreEqual("1,000", 1000M.Fmt(0));
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtNullDecimalTest()
        {
            decimal? i = null;
            Assert.AreEqual("", i.Fmt());
            i = 1;
            Assert.AreEqual("1.00", i.Fmt());
            i = 1000;
            Assert.AreEqual("1,000.00", i.Fmt());
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtCurrencyTest()
        {
            Assert.AreEqual("$1.00", 1M.FmtCurrency());
            Assert.AreEqual("$1,000.00", 1000M.FmtCurrency());
            Assert.AreEqual("$1,000", 1000M.FmtCurrency(0));
        }

        /// <summary>
        ///A test for Fmt
        ///</summary>
        [TestMethod()]
        public void FmtNullCurrencyTest()
        {
            decimal? i = null;
            Assert.AreEqual("", i.FmtCurrency());
            i = 1;
            Assert.AreEqual("$1.00", i.FmtCurrency());
            i = 1000;
            Assert.AreEqual("$1,000.00", i.FmtCurrency());
            Assert.AreEqual("$1,000", i.FmtCurrency(0));
        }

    }
}
