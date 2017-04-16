# Command-line Parsing

Command-line parsing can be a tedious chore when you are building an application that accepts command-line arguments. But what if all you had to do was create a class with the properties that you want to accept? The BizArk framework offers a simple to use command-line parsing utility that allows you to do exactly that.

Command-line parsing in the BizArk framework has these key features:

* **Automatic initialization:** Class properties are automatically set based on the command-line arguments.
* **Default properties:** Send in a value without specifying the property name. 
* **Value conversion:** Uses the powerful ConvertEx class also included in BizArk to convert values to the proper type.
* **Data validation:** Uses DataAnnotations to validate command-line arguments.
* **Boolean flags:** Flags can be specified by simply using the argument (ex, /b for true and /b- for false) or by adding the value true/false, yes/no, etc.
* **Argument arrays:** Simply add multiple values after the command-line name to set a property that is defined as an array. Ex, /x 1 2 3 will populate x with the array { 1, 2, 3 } (assuming x is defined as an array of integers).
* **Argument aliases:** A property can support multiple aliases for it. For example, Help uses the alias ?.
* **Partial name recognition:** You don’t need to spell out the full name or alias, just spell enough for the parser to disambiguate the property/alias from the others.
* **Supports ClickOnce:** Can initialize properties even when they are specified as the query string in a URL for ClickOnce deployed applications. The command-line initialization method will detect if it is running as ClickOnce or not so your code doesn’t need to change when using it.
* **Automatically creates /? help:** Nicely formats the application name, description, usage information, what arguments are available, default values, possible values (for enums), etc.
* **Load/Save command-line arguments to a file:** This is especially useful if you have multiple large, complex sets of command-line arguments that you want to run multiple times.

So how do you use the command-line parsing? Start by creating a class that is derived from CmdLineObject. Add properties with data types that support conversion from a string using the BizArk ConvertEx class. In main, instantiate your class and call Initialize. That’s it.

If you want to validate your command-line or display help, you will need to check a couple of CmdLineObject properties. Other than that, you can just use your object as any other class.

Here’s an example custom command-line object:

{{
    [CmdLineOptions(DefaultArgName = "Message")](CmdLineOptions(DefaultArgName-=-_Message_))
    public class MyCmdLine
        : CmdLineObject
    {
        public MyCmdLine()
        {
            Message = "";
        }

        [CmdLineArg(Alias = "M", Usage = "Message")](CmdLineArg(Alias-=-_M_,-Usage-=-_Message_))
        [Description("Message to display.")](Description(_Message-to-display._))
        public string Message { get; set; }
    }
}}
This is the code to use it:

{{
    static void Main(string[]() args)
    {
        var cmdLine = new MyCmdLine();
        cmdLine.Initialize();
        // Validate only after checking to see if they requested help
        // in order to prevent displaying errors when they request help.
        if (cmdLine.Help || !cmdLine.IsValid())
        {
            Console.WriteLine(cmdLine.GetHelpText(Console.WindowWidth));
            return;
        }

        Console.WriteLine(cmdLine.Message);
    }
}}
And to display a message, call it like this: {{MyProgram /M "Hello World"}}

BizArk also provides a convenient method that you can call that will automatically initialize your command line arguments and handle common scenarios such as requesting help and error handling. {{BizArk.Core.CmdLine.ConsoleApplication.RunProgram}} takes a generic argument that should be your custom command-line object and a delegate for a parameter that is your main program. Your {{Main()}} function should just have a single line of code in it to call the {{ConsoleApplication.RunProgram}} method.

{{
    class Program
    {
        static void Main(string[]() args)
        {
            ConsoleApplication.RunProgram<MyCmdLine>(Start);
        }

        private static void Start(MyCmdLine args)
        {
            // Your code goes here
        }
    }
}}
[ConsoleApplication.RunProgram](http://bizark.codeplex.com/SourceControl/changeset/view/63721#1166313) will instantiate your custom CmdLineObject, initialize it, display help if necessary, display unhandled errors, and prompt the user to press a key to exit if {{CmdLineOptions.Wait}} is true. 