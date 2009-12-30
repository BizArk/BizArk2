/* This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. */

using Redwerb.BizArk.Core.ArrayExt;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestBizArkCore
{


    /// <summary>
    ///This is a test class for StringArrayExtTest and is intended
    ///to contain all StringArrayExtTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ArrayExtTest
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

        /// <summary>
        ///A test for Convert
        ///</summary>
        [TestMethod()]
        public void StandardConvertTest()
        {
            string[] arr = new string[] { "1", "2", "3" };
            Type elementType = typeof(int);
            int[] expected = new int[] { 1, 2, 3 };
            int[] actual;
            actual = (int[])ArrayExt.Convert(arr, elementType);
            AssertEx.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void GenericConvertTest()
        {
            string[] arr = new string[] { "1", "2", "3" };
            Type elementType = typeof(int);
            int[] expected = new int[] { 1, 2, 3 };
            int[] actual;
            actual = ArrayExt.Convert<int>(arr);
            AssertEx.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Shrink
        ///</summary>
        [TestMethod()]
        public void ShrinkTest()
        {
            string[] test;
            string[] expected;
            string[] actual;

            test = new string[] { };
            actual = ArrayExt.Shrink(test, 0, 0);
            expected = new string[] { };
            Assert.AreEqual(0, actual.Length);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 0, 1);
            expected = new string[] { "Hi", "Bye" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 0, 0);
            expected = new string[] { "Hi" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "Bye" };
            actual = ArrayExt.Shrink(test, 1, 1);
            expected = new string[] { "Bye" };
            AssertEx.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void RemoveEmptiesTest()
        {
            string[] test;
            string[] expected;
            string[] actual;

            test = new string[] { };
            actual = test.RemoveEmpties();
            expected = new string[] { };
            Assert.AreEqual(0, actual.Length);

            test = new string[] { "Hi", "Bye" };
            actual = test.RemoveEmpties();
            expected = new string[] { "Hi", "Bye" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { "Hi", "" };
            actual = test.RemoveEmpties();
            expected = new string[] { "Hi" };
            AssertEx.AreEqual(expected, actual);

            test = new string[] { null, "Bye" };
            actual = test.RemoveEmpties();
            expected = new string[] { "Bye" };
            AssertEx.AreEqual(expected, actual);

            var itest = new int[] { 1, 2, 3 };
            var iactual = itest.RemoveEmpties();
            var iexpected = new int[] { 1, 2, 3 };
            AssertEx.AreEqual(iexpected, iactual);

            itest = new int[] { 1, int.MinValue, 3 };
            iactual = itest.RemoveEmpties();
            iexpected = new int[] { 1, 3 };
            AssertEx.AreEqual(iexpected, iactual);

        }

    }
}
