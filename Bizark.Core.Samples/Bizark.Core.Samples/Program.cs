using System;
using BizArk.Core;
using BizArk.Core.CmdLine;

namespace Bizark.Core.Samples
{
    static class Program
    {
        private static void Main(string[] args)
        {
            ConsoleApplication.RunProgram<SimpleCommandLineArguments>(RunMain);
        }

        private static void RunMain(SimpleCommandLineArguments args)
        {
            Console.WriteLine("Name is {0}, Age is {1}", args.Name, args.Age);
            Console.ReadLine();
        }

        private class SimpleCommandLineArguments : CmdLineObject
        {
            [CmdLineArg(ShowInUsage = DefaultBoolean.True)]
            public string Name { get; set; }

            [CmdLineArg(ShowInUsage = DefaultBoolean.True)]
            public int Age { get; set; }
        }
    }
}