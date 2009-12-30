/* This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestBizArkCore
{
    /// <summary>
    /// Additional assertions beyond what is provided by Assert.
    /// </summary>
    public static class AssertEx
    {
        public static void AreEqual(Array expected, Array actual)
        {
            AreEqual(expected, actual, "");
        }

        public static void AreEqual(Array expected, Array actual, string message)
        {
            if (expected == actual) return;
            if (expected == null) Assert.Fail(message == "" ? string.Format("Expected <null>. Actual array contained {0} elements.", actual.Length) : message);
            if (actual == null) Assert.Fail(message == "" ? string.Format("Expected {0} elements. Actual array was null.", expected.Length) : message);
            if (expected.GetType().GetElementType() != actual.GetType().GetElementType()) Assert.Fail(message == "" ? string.Format("Array element types differ. Expected elements of type {0}, actual array conains elements of type {1}.", expected.GetType().GetElementType().FullName, actual.GetType().GetElementType().FullName) : message);
            if (expected.Length != actual.Length) Assert.Fail(message == "" ? string.Format("Expected {0} elements. Actual array conains {1} elements.", expected.Length, actual.Length) : message);

            for (int i = 0; i < actual.Length; i++)
            {
                if (!expected.GetValue(i).Equals(actual.GetValue(i)))
                    Assert.Fail(message == "" ? string.Format("Arrays differ at element {0}. Expected '{1}', actual '{2}'.", i, expected.GetValue(i), actual.GetValue(i)) : message);
            }
        }

        //public static void NoError(FieldValidationResults res)
        //{
        //    if (res.Error == null) return;
        //    Assert.Fail("Validation resulted in error: ({0}) {1}", res.Error.MessageID, res.Error.Message);
        //}

        //public static void HasError(FieldValidationResults res, Enum msgID)
        //{
        //    if (res.Error == null)
        //        Assert.Fail("Validation succeeded. Expected error {0}", msgID);

        //    if (res.Error.MessageID.Equals(msgID))
        //        return;

        //    Assert.Fail("Expected error {0}. Validation resulted in unexpected error: ({1}) {2}", msgID, res.Error.MessageID, res.Error.Message);
        //}
    }
}
