# Welcome to the BizArk Toolkit Home Page

**UPDATE: BizArk v3 is now available! [Check it Out](https://github.com/BizArk/BizArk3)**

BizArk Toolkit is a container for different reusable things.  

**NOTE:** This repository was imported from CodePlex. The documentation is a bit rough, but should be adequate. Look in the docs folder for the full set of documentation that was imported from CodePlex.

## BizArkCore features

* Simple but powerful command-line argument parsing. See [Command-line Parsing](https://raw.githubusercontent.com/BizArk/BizArk2/master/docs/Command-line%20Parsing.md).
* Console application with command-line parsing [Console application with command-line parsing and validation](https://raw.githubusercontent.com/BizArk/BizArk2/master/docs/Console%20application%20with%20command-line%20parsing%20and%20validation.md)
* Robust data-type conversion class. Handles many types of conversions that the built-in .Net conversion class cannot. See [Data Type Conversions](https://raw.githubusercontent.com/BizArk/BizArk2/master/docs/Data%20Type%20Conversions.md).
* WebHelper class, similar to the WebClient, but supports multiple-file uploads, setting the timeout to something other than the default, and allows customization of the request. See [Web Helper](https://raw.githubusercontent.com/BizArk/BizArk2/master/docs/Web%20Helper.md).
* String templates allow you to used named arguments in a string instead of numeric place holders while still getting the formatting capabilities of String.Format. See [String Templates](https://raw.githubusercontent.com/BizArk/BizArk2/master/docs/String%20Templates.md).
* Data caching with built-in time-out and memory management.
* Simple class factory for replacing types with other types.
* Application object provides simplified access to the typical applicaion attributes, cache, and file utilities based on the application (temp directory, relative to app path, etc)
* A mime type map that can be used to get a mime type based on a file extension. This map defaults to the [Apache Mime.Types](http://svn.apache.org/viewvc/httpd/httpd/trunk/docs/conf/mime.types?view=markup) file.
* Use embedded fonts or fonts from a file with FontUtil.
* Manage your database access using the Database class. It can return strongly typed values with the ExecuteScalar<T> functions and you can use the Select<T> method to map an IDataReader to an object.
* Many extension methods (over 160):
    * Array - Shrink, Convert, IndexOf, Contains, Copy
    * PropertyDescriptor - GetAttribute
    * Type - GetAttribute, Implements (checks if type implements an interface), IsDerivedFrom, Instantiate (creates a new instance of the type)
    * Assembly - GetAttribute
    * Object - GetAttribute, Convert, GetValue (gets a property value using reflection)
    * Size - Resize (proportional resize given max width and height)
    * Exception - GetDetails (full log of exception)
    * Image - Open (opens Image in default application), GetExtension (gets the appropriate file extension for an Image), Resize (proportionally resizes image)
    * String - Wrap (wraps a string based on a max char count), Lines (returns an array of strings delimited by newline), Words (returns an array of strings delimited by white space), F
    * Xml - Easily get and set values in an XmlDocument.
    * Web - Encode/decode html and url strings.
    * Format - Convenient methods for formatting numeric values, a shortcut for string.Format, and a shortcut for [String Templates](/wikipage?title=String%20Templates&referringTitle=Home).
    * Data - Methods to get typed values from IDataReader, DataRow, and DataRowView. Uses ConvertEx to convert values if necessary.

* * *

## Available on nuget

Bizark.Core: contains the command-line parsing component

[http://www.nuget.org/packages/BizArk.Core/](http://www.nuget.org/packages/BizArk.Core/)
