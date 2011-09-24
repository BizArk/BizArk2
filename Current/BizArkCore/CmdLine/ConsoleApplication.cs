using System;
using System.IO;
using System.Text.RegularExpressions;
using BizArk.Core.StringExt;

namespace BizArk.Core.CmdLine
{

    /// <summary>
    /// Provides helper methods for command-line applications.
    /// </summary>
    public static class ConsoleApplication
    {

        /// <summary>
        /// Convenient method for running an application. Provides command-line argument initialization, help text, error handling, and waiting for exit. This is typically the only line of code in Main.
        /// </summary>
        /// <typeparam name="TArgs">The type for the CmdLineObject to use. Must have a default constructor.</typeparam>
        /// <param name="run">The method to run once the arguments are initialized. Will not be called if the help flag is set or the args aren't valid.</param>
        public static void RunProgram<TArgs>(Run<TArgs> run) where TArgs : CmdLineObject
        {
            var args = Activator.CreateInstance<TArgs>();
            args.Initialize();

            try
            {
                // Validate only after checking to see if they requested help
                // in order to prevent displaying errors when they request help.
                if (args.Help || !args.IsValid())
                {
                    Console.WriteLine(args.GetHelpText(Console.WindowWidth));
                    return;
                }

                run(args);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
            finally
            {
                if (args.WaitForAnyKey())
                {
                    Console.WriteLine();
                    var prompt = "* Press any key to continue. *";
                    Console.WriteLine(new string('*', prompt.Length));
                    Console.WriteLine(prompt);
                    Console.WriteLine(new string('*', prompt.Length));
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Displays the exception to the console.
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteError(Exception ex)
        {
            var curEx = ex;
            if (curEx == null) return;
            Console.WriteLine("ERROR!!!");
            while (curEx != null)
            {
                Console.WriteLine(new string('*', Console.WindowWidth));
                Console.WriteLine(string.Format("{0} - {1}", curEx.GetType().FullName, curEx.Message));
                //Console.WriteLine(curEx.StackTrace);
                WriteLine(curEx.StackTrace, "   ");
                Console.WriteLine();
                curEx = curEx.InnerException;
            }
        }

        /// <summary>
        /// Writes a message to the console. Ensures that the message is wrapped at word boundaries and that indentation is preserved.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="prefix">Adds this string to the beginning of each line.</param>
        public static void WriteLine(string msg, string prefix = "")
        {
            var sr = new StringReader(msg);
            var line = sr.ReadLine();
            var re = new Regex(@"^(\s+)?.*");
            while(line != null)
            {
                var m = re.Match(line);
                var whitespace = "";
                if (m.Groups.Count > 1)
                    whitespace = m.Groups[1].Value;
                var newLine = line.Wrap(Console.WindowWidth, whitespace + prefix);
                Console.WriteLine(newLine);
                line = sr.ReadLine();
            }
        }

        /// <summary>
        /// Delegate method that is used to run a console application.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="args"></param>
        public delegate void Run<T>(T args) where T : CmdLineObject;

    }

}
