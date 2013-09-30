using System;
using System.Diagnostics;
using BizArk.Core.CmdLine;
using BizArk.Core.Extensions.StringExt;
using BizArk.Core.Extensions.WebExt;
using BizArk.Core.Util;
using System.IO;
using System.Linq;
using BizArk.Core;
using BizArk.Core.DataAnnotations;
using NUnit.Framework;

namespace TestBizArkCore
{

    /// <summary>
    ///This is a test class for CmdLineObjectTest and is intended
    ///to contain all CmdLineObjectTest Unit Tests
    ///</summary>
[TestFixture]
    public class CmdLineObjectTest
    {

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [Test]
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
            args = new[] { "/H", "Hi Brian!" };
            target.InitializeFromCmdLine(args);
             Assert.That(target.Hello, Is.EqualTo("Hi Brian!"));
             Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/h", "Hi Brian!" };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/h", "Hi Brian!", "/G", "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "Hi Brian!", "/G", "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject2();
            args = new string[] { "Goodbye Christine." };
            target.InitializeFromCmdLine(args);
             Assert.IsNull(target.Hello);
             Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "TEST", "/h", "Hi Brian!", "/G", "Goodbye Christine.", "TEST" };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.AreEqual("Goodbye Christine.", target.Goodbye);

            target = new MyTestCmdLineObject();
            args = new string[] { "/I" };
            target.InitializeFromCmdLine(args);
             Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] { "/I-" };
            target.InitializeFromCmdLine(args);
             Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] { "/I", "Yes" };
            target.InitializeFromCmdLine(args);
             Assert.IsTrue(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] { "/I", "No" };
            target.InitializeFromCmdLine(args);
             Assert.IsFalse(target.DoesLikeIceCream);

            target = new MyTestCmdLineObject();
            args = new[] { "/S", "Cars", "Computers", "Food" };
            target.InitializeFromCmdLine(args);
            string[] expectedStuff = new string[] { "Cars", "Computers", "Food" };
            AssertEx.AreEqual(expectedStuff, target.StuffILike);
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [Test]
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

        [Test]
        public void MultipleDefaultPropTest()
        {
            MyTestCmdLineObject4 target;
            string[] args;
            string[] expected;

            target = new MyTestCmdLineObject4();
            args = new string[] { "Brian", "Christine", "/c", "Abrian", "Brooke" };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Christine", target.Mother);
             Assert.AreEqual("Brian", target.Father);
            expected = new string[] { "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Children);

            target = new MyTestCmdLineObject4();
            args = new string[] { "Brian", "/c", "Abrian", "Brooke" };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual(null, target.Mother);
             Assert.AreEqual("Brian", target.Father);
            expected = new string[] { "Abrian", "Brooke" };
            AssertEx.AreEqual(expected, target.Children);

            Debug.WriteLine(target.GetHelpText(200));
        }

        /// <summary>
        ///A test for Initialize
        ///</summary>
        [Test]
        public void PartialNameTest()
        {
            MyTestCmdLineObject target;
            string[] args;

            target = new MyTestCmdLineObject();
            args = new string[] { "/Hell", "Hi Brian" };
            target.InitializeFromCmdLine(args);
             Assert.AreEqual("Hi Brian", target.Hello);
        }

        [Test]
        public void CmdLineDescriptionTest()
        {
            string test = "This is a test\ntest  test test";
            string[] lines = test.Lines();
            foreach (string line in lines)
                Debug.WriteLine(line);

            var args = new MyTestCmdLineObject();
            args.InitializeEmpty();
            Debug.WriteLine(args.GetHelpText(50));
        }

        [Test]
        public void UsageTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeEmpty();
            Debug.WriteLine(args.Options.Usage);
             Assert.AreEqual("TESTAPP [/H <Hello>] [/FavoriteCar <FavoriteCar>] [/?[-]]", args.Options.Usage);
        }

        [Test]
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

[Test]
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
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}", "Hi Brian!".UrlEncode());
            target.InitializeFromQueryString(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.IsNull(target.Goodbye);

            target = new MyTestCmdLineObject();
            args = string.Format("h={0}&G={1}", "Hi Brian!".UrlEncode(), "Goodbye Christine.".UrlEncode());
            target.InitializeFromQueryString(args);
             Assert.AreEqual("Hi Brian!", target.Hello);
             Assert.AreEqual("Goodbye Christine.", target.Goodbye);

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

[Test]
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

[Test]
        public void DuplicateArgumentsTest()
        {
            var cmdLine = new AliasesWithDifferentCaseTestCmdLineObject();
            AssertEx.Throws(typeof(CmdLineArgumentException), () => { cmdLine.InitializeEmpty(); });
        }

[Test]
        public void ArgumentValidationTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "2");
             Assert.AreEqual(2, args.NumberOfScoops);
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "chocolate");
             Assert.AreEqual(1, args.NumberOfScoops);
             Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteNumbers", "1", "2", "3");
             Assert.AreEqual(3, args.FavoriteNumbers.Length);
             Assert.AreEqual(1, args.FavoriteNumbers[0]);
             Assert.AreEqual(2, args.FavoriteNumbers[1]);
             Assert.AreEqual(3, args.FavoriteNumbers[2]);
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteNumbers", "Red", "Green", "Blue");
             Assert.AreEqual(1, args.NumberOfScoops);
             Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/FavoriteCar", "Ford");
             Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "5");
             Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.GetHelpText(200));
        }

[Test]
        public void OptionsAttTest()
        {
            var args = new MyTestCmdLineObject5();
             Assert.AreEqual("+", args.Options.ArgumentPrefix);
            args.InitializeFromCmdLine("+D", "Brian");
             Assert.AreEqual("Brian", args.Father);
        }

[Test]
        public void LongArgPrefixTest()
        {
            var args = new MyTestCmdLineObject6();
             Assert.AreEqual("--", args.Options.ArgumentPrefix);
            args.InitializeFromCmdLine("--D", "Brian");
             Assert.AreEqual("Brian", args.Father);
        }

[Test]
        public void OverrideValidationTest()
        {
            var args1 = new MyTestCmdLineObject7();
            args1.InitializeFromCmdLine("/v", "true");
             Assert.AreEqual(true, args1.IsValidProp);
             Assert.IsTrue(args1.IsValid());

            var args2 = new MyTestCmdLineObject7();
            args2.InitializeFromCmdLine("/v", "false");
             Assert.AreEqual(false, args2.IsValidProp);
             Assert.IsTrue(!args2.IsValid());
        }

[Test]
        public void ValidateSetTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "red");
             Assert.IsTrue(args.IsValid());
             Assert.AreEqual("red", args.SampleColor);

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "green");
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "blue");
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "Blue");
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor", "purple");
             Assert.IsFalse(args.IsValid());
            Debug.WriteLine(args.ErrorText);

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor2", "pink");
            args.Properties["SampleColor2"].Validators.Add(new SetValidator<string>("pink", "purple", "puce"));
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/SampleColor2", "pink");
            // Uses a custom equality comparer (a BizArk class).
            args.Properties["SampleColor2"].Validators.Add(new SetValidator<string>(new EqualityComparer((a, b) => { return a == b; }), "pink"));
             Assert.IsTrue(args.IsValid());

            args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine("/NumberOfScoops", "2");
            var vals = new int[] { 1, 2 };
            args.Properties["NumberOfScoops"].Validators.Add(new SetValidator<int>(vals));
            Debug.WriteLine(args.GetHelpText(50));
             Assert.IsTrue(args.IsValid());
        }

[Test]
        public void HelpTextWrappingTest()
        {
            var args = new MyTestCmdLineObject();
            args.InitializeFromCmdLine();
            var vals = new int[] { 1, 2 };
            Debug.WriteLine(args.GetHelpText(40));
             Assert.IsTrue(args.IsValid());
        }

	    [Test]
	    public void InitializeFromFullCmdLine_AssignmentDelimiterSpace_InitializedFromArgs()
	    {
			// Arrange
		    var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

			// Act
			cmdLineObj.InitializeFromCmdLine(new[] { "/Name", "John"});

			//Assert
			Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
	    }

		[Test]
		public void InitializeFromFullCmdLine_AssignmentDelimiterColonOneArgument_InitializedFromArgs()
		{
			// Arrange
			var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

			// Act
			cmdLineObj.InitializeFromCmdLine(new[] { "/Name:John" });

			//Assert
			Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
		}

		[Test]
		public void InitializeFromFullCmdLine_AssignmentDelimiterColonTwoArguments_InitializedFromArgs()
		{
			// Arrange
			var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

			// Act
			cmdLineObj.InitializeFromCmdLine(new[] { "/Name:John", "/Count:3" });

			//Assert
			Assert.That(cmdLineObj.Name, Is.EqualTo("John"));
			Assert.That(cmdLineObj.Count, Is.EqualTo(3));
		}

		[Test]
		public void InitializeFromFullCmdLine_AssignmentDelimiterColonArrayArguments_InitializedFromArgs()
		{
			// Arrange
			var cmdLineObj = new ColonAssignmentDelimiterArrayArgCmndLineObject();

			// Act
            cmdLineObj.InitializeFromCmdLine(new[] { "/Names:John", "Maria" });

			//Assert
			Assert.That(cmdLineObj.Names, Is.EquivalentTo(new []{"John", "Maria"}));
		}

        [Test]
        public void InitializeFromFullCmdLine_AssignmentDelimiterColonArrayArgumentsValueContainsDoubleQuotes_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] { @"/Name:""John Smith""" });

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith") );
        }

        [Test]
        public void InitializeFromFullCmdLine_DefaultAssignmentDelimiterCmdLineObjectValueContainsDoubleQuotes_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] { @"/Name", @"John Smith" });

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith"));
        }

        [Test]
        public void InitializeFromCmdLine_DefaultAssignmentDelimiterCmdLineObjectValueContainsDoubleQuotes_InitializedFromArgs()
        {
            // Arrange
            var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

            // Act
            cmdLineObj.InitializeFromCmdLine(new[] { @"/Name", @"""John Smith""" });

            //Assert
            Assert.That(cmdLineObj.Name, Is.EqualTo("John Smith"));
        }


	    [Test]
		public void GetHelpText_DefaultAssignmentDelimiterOneArgument_UsageContainsSpace()
	    {
			// Arrange
			var cmdLineObj = new DefaultAssignmentDelimiterCmdLineObject();

			// Act
			cmdLineObj.InitializeFromFullCmdLine("prog.exe /Name John", new[] { "prog.exe", "/Name", "John" });

			//Assert
		    string helpText = cmdLineObj.GetHelpText(80);
			Assert.That(helpText, Is.EqualTo(@"Command-line options.

Usage:  [/Name <Name>] [/?[-]]

Name:     The name of the user
Help (?): Displays command-line usage information.
          Default Value: False
"));
	    }

		[Test]
		public void GetHelpText_ColonAssignmentDelimiterCmdLineObject_UsageContainsSpace()
		{
			// Arrange
			var cmdLineObj = new ColonAssignmentDelimiterCmdLineObject();

			// Act
			cmdLineObj.InitializeFromFullCmdLine("prog.exe /Name John", new[] { "prog.exe", "/Name", "John" });

			//Assert
			string helpText = cmdLineObj.GetHelpText(80);
			Assert.That(helpText, Is.EqualTo(@"Command-line options.

Usage:  [/Name:<Name>] [/?[-]]

Name:      The name of the user
Count:     
           Default Value: 0
Help (?):  Displays command-line usage information.
           Default Value: False
"));
		}
    }

    [CmdLineOptions(DefaultArgName = "Hello", ApplicationName = "TESTAPP")]
    internal class MyTestCmdLineObject
        : CmdLineObject
    {

        public MyTestCmdLineObject()
        {
            NumberOfScoops = 1;
            SampleColor = "blue";
        }

        [CmdLineArg(Alias = "H", ShowInUsage = DefaultBoolean.True)]
        [System.ComponentModel.Description("Says hello to user.")]
        public string Hello { get; set; }

        [CmdLineArg(Alias = "G")]
        [System.ComponentModel.Description("Says goodbye to user.")]
        public string Goodbye { get; set; }

        [CmdLineArg(Alias = "I")]
        [System.ComponentModel.Description("Determines whether user likes ice cream or not.")]
        public bool DoesLikeIceCream { get; set; }

        [CmdLineArg()]
        [System.ComponentModel.DataAnnotations.Range(1, 3)]
        public int NumberOfScoops { get; set; }

        [CmdLineArg()]
        public int[] FavoriteNumbers { get; set; }

        [CmdLineArg(ShowInUsage= DefaultBoolean.True)]
        public Car FavoriteCar { get; set; }

        [CmdLineArg(Aliases = new string[] { "S", "Stuff", "Crap" })]
        [System.ComponentModel.Description("List of things the user likes.")]
        public string[] StuffILike { get; set; }

        // ValidateSet: http://msdn.microsoft.com/en-us/library/windows/desktop/ms714432(v=vs.85).aspx
        [CmdLineArg()]
        [Set(true, "red", "green", "blue")]
        public string SampleColor { get; set; }

        [CmdLineArg()]
        public string SampleColor2 { get; set; }
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

    internal class MyTestCmdLineObject4
        : CmdLineObject
    {
        public MyTestCmdLineObject4()
        {
            Options.DefaultArgNames = new string[] { "F", "M" };
        }

        [CmdLineArg(Alias = "M")]
        public string Mother { get; set; }

        [CmdLineArg(Alias = "F")]
        public string Father { get; set; }

        [CmdLineArg(Alias = "C")]
        public string[] Children { get; set; }

    }

    [CmdLineOptions(DefaultArgName = "F", ArgumentPrefix = "+")]
    internal class MyTestCmdLineObject5
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }

    [CmdLineOptions(DefaultArgName = "F", ArgumentPrefix = "--")]
    internal class MyTestCmdLineObject6
        : CmdLineObject
    {
        [CmdLineArg(Alias = "F")]
        public string[] Family { get; set; }

        [CmdLineArg(Alias = "D")]
        public string Father { get; set; }
    }

    internal class MyTestCmdLineObject7
    : CmdLineObject
    {
        [CmdLineArg(Alias = "v")]
        public bool IsValidProp { get; set; }

        protected override string[] Validate()
        {
            var baseValidation = base.Validate().ToList();

            if (!IsValidProp)
                baseValidation.Add("Not IsValidProp!");

            return baseValidation.ToArray();
        }
    }

    internal class AliasesWithDifferentCaseTestCmdLineObject : CmdLineObject
    {
        [CmdLineArg(Aliases = new[] { "F", "f" })]
        public string Family { get; set; }
    }

    internal enum Car
    {
        Tesla,
        Ferrari,
        Lamborghini,
        Kia
    }

	internal class DefaultAssignmentDelimiterCmdLineObject : CmdLineObject
	{
		[CmdLineArg(ShowInUsage = DefaultBoolean.True)]
		[System.ComponentModel.Description("The name of the user")]
		public string Name { get; set; }
	}

	[CmdLineOptions(AssignmentDelimiter = ':')]
	internal class ColonAssignmentDelimiterCmdLineObject : CmdLineObject
	{
		[CmdLineArg(ShowInUsage = DefaultBoolean.True)]
		[System.ComponentModel.Description("The name of the user")]
		public string Name { get; set; }

		[CmdLineArg]
		public int Count { get; set; }
	}

	[CmdLineOptions(AssignmentDelimiter = ':')]
	internal class ColonAssignmentDelimiterArrayArgCmndLineObject : CmdLineObject
	{
		[CmdLineArg]
		public string[] Names { get; set; }
	}

}
