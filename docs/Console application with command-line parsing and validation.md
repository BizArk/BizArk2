# Simple command line class with two properties:

{{
using System;
using BizArk.Core;
using BizArk.Core.CmdLine;

private class SimpleCommandLineArguments : CmdLineObject
{
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public string Name { get; set; }

    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public int Age { get; set; }
}
}}

# Using command-line object with parsing, validation and display of usage
ConsoleApplication class does the following:
1. Parses the command line arguments from System.Environment.GetCommandLineArgs() according to definitions in the T class
2. Validates the arguments values against their types, mandatory or not, custom validations
3. If validation fails, Environment.ExitCode is set to **-1**
4. Executes the given method with these arguments
5. Execution is within try/catch block
6. If exception is thrown during the execution, Environment.ExitCode is set to **1**

{{
using System;
using BizArk.Core;
using BizArk.Core.CmdLine;

private static void Main(string[]() args)
{
    ConsoleApplication.RunProgram<SimpleCommandLineArguments>(RunMain);
}

private static void RunMain(SimpleCommandLineArguments args)
{
    Console.WriteLine("Name is {0}, Age is {1}", args.Name, args.Age);
    Console.ReadLine();
}

}}

# Command-line parsing with enum and arrays 

{{
using System;
using System.Drawing;
using BizArk.Core;
using BizArk.Core.CmdLine;

private class EnumCommandLineArguments : CmdLineObject
{
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public Color[]() Colors { get; set; }
        
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public Color FavoriteColor { get; set; }
}

}}


# Command-line parsing with custom argument prefix
Default prefix in BizArk is "/", it can be customized easily using  ArgumentPrefix

{{

[CmdLineOptions(ArgumentPrefix = "-")](CmdLineOptions(ArgumentPrefix-=-_-_))
private class CustomizedPrefixCommandLineArguments : CmdLineObject
{
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public string Name { get; set; }
}

}}

# Command-line parsing with custom assignment delimiter 
Default assignment delimiter in BizArk is space, it can be customized easily using  AssignmentDelimiter 

{{
[CmdLineOptions(AssignmentDelimiter = ':' )](CmdLineOptions(AssignmentDelimiter-=-'_'-))
private class CustomizedAssignmentDelimiterCommandLineArguments : CmdLineObject
{
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    public string Name { get; set; }
}
}}

# Customized usage of console application
Application description can be added on class level using CmdLineOptions
Argument description can be added on property using  System.ComponentModel.Description attribute

{{
[CmdLineOptions(Description = "This is the application")](CmdLineOptions(Description-=-_This-is-the-application_))
private class WithDescriptionCommandLineArguments : CmdLineObject
{
    [CmdLineArg(ShowInUsage = DefaultBoolean.True)](CmdLineArg(ShowInUsage-=-DefaultBoolean.True))
    [System.ComponentModel.Description("This is the name")](System.ComponentModel.Description(_This-is-the-name_))
    public string Name { get; set; }
}
}}

# Console application with waiting
Wait parameter can be helpful, when we need to specify to wait after the application has finished. WaitArgName specifies the argument name, that can be specified from the console.

{{
[CmdLineOptions(Wait=true, WaitArgName="PleaseWaitForMe")](CmdLineOptions(Wait=true,-WaitArgName=_PleaseWaitForMe_))
private class WithWaitCommandLineArguments : CmdLineObject
{
    [CmdLineArg()](CmdLineArg())
    public bool PleaseWaitForMe { get; set; } 
}

public class Program
{
  public static void Main()
  {
    ConsoleApplication.RunProgram<WithWaitCommandLineArguments>(MainWithArgs);
  }
  public static void MainWithArgs(WithWaitCommandLineArguments argsWithWait)
  {

  }
}
}}

# Validation of command line arguments

BizArk support a variety of validation techniques:

## Overriding Validate method
{{
public class SampleTestArgs : CmdLineObject
{
    [CmdLineArg](CmdLineArg)
    public string FilePath { get; set; }

    protected override string[]() Validate()
    {
        if (!File.Exists(FilePath))
        {
            return new[]() { string.Format("File {0} does not exist.", FilePath) };
        }

        return base.Validate();
    }
}
}}
## Using Validation attributes
{{

using System.ComponentModel.DataAnnotations;
using BizArk.Core;
using BizArk.Core.CmdLine;

public class SampleTestArgs : CmdLineObject
{
    [CmdLineArg](CmdLineArg)
    [EmailAddress](EmailAddress)
    public string CustomerEmail { get; set; }
}

}}

## Using custom validation attributes
{{
using System.ComponentModel.DataAnnotations;
using BizArk.Core;
using BizArk.Core.CmdLine;

public class SampleTestArgs : CmdLineObject
{
    [CmdLineArg](CmdLineArg)
    [CustomValidation(typeof(VersionValidator), "Validate")](CustomValidation(typeof(VersionValidator),-_Validate_))
    public string Version { get; set; }
}

public class VersionValidator
{
    // The method must be public and static
    public static ValidationResult Validate(string versionAsString)
    {
        Version version;
        if (!Version.TryParse(versionAsString, out version))
        {
            return new ValidationResult(string.Format("{0} is not valid version", versionAsString));
        }
        return ValidationResult.Success;
    }
}

}}