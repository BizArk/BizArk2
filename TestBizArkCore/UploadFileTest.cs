using BizArk.Core.Web;
using System.IO;
using NUnit.Framework;

namespace TestBizArkCore
{
	
	
	/// <summary>
	///This is a test class for UploadFileTest and is intended
	///to contain all UploadFileTest Unit Tests
	///</summary>
	[TestFixture]
	public class UploadFileTest
	{

		[Test]
		public void op_ExplicitTest()
		{
			var fi = new FileInfo(@"C:\Test.txt");
			var file = (UploadFile)fi;
			Assert.IsNotNull(file);
			Assert.AreEqual(fi.FullName, file.FilePath);
		}

	}
}
