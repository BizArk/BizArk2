# Data Type Conversions

The conversion library built into the BizArk Toolkit provides the conversion library that should have been built into .Net. It supports all the conversions that you would expect the .Net framework to support (but doesn't) plus many others. This library is used throughout the BizArk toolkit to do conversions (such as converting command-line parameters to their correct type, converting web parameters in WebHelper to strings, etc).

**Example of using ConvertEx:**
{{
var pt = ConvertEx.To<Point>("1,2");
}}

* **TypeConverter:** This is a class that you can implement to support conversions between any data types you wish to support. You can get a TypeConverter by calling TypeDescriptor.GetConverter(Type). TypeConverters are used in the binding support in WinForms as well as a few other places in the .Net framework.
* **ToXxx methods:** This is a common naming convention for converting an object into another type. The most common of these is the System.Object.ToString() method.
* **Parameterized constructors:**  This is a convention where a constructor for a class takes a typed parameter.
* **Static methods:** A static method is a common convention to convert to/from values (for example, Int32.Parse). Overloaded operators are also implemented as static methods.
* **Image to byte array and vice versa:** This is a specific type of conversion to convert an image to a byte array for storage or sending to a stream.
* **Enumerations:** Convert between enumerations and strings or numeric types.
* **To/from null:** Reference types can be set to null, but value types cannot. In those cases ConvertEx supports the concept of empty values. An empty value is determined based the following criteria:
	* Reference type: null
	* Char: 0 (that is \0)
	* Static Type.Empty property (if defined)
	* Static Type.MinValue property (if defined)
	* Enumerations get the first enumerated value (as opposed to the .Net default of 0)
	* Custom empty value. Register the empty value by calling ConvertEx.RegisterEmptyValue.

To use ConvertEx, simply call one of the ConvertEx methods. There are a number of helper methods to convert values to basic types such as integer, string, boolean, etc. However, you can also call ConvertEx.ChangeType or ConvertEx.To (an alias for ChangeType) to convert between any two types (as long as one of the types supports conversions from the other type).

ConvertEx also provides a plugin architecture if you want to extend the types that it can convert to/from. To use it, just create a class that implements the Redwerb.BizArk.Core.Convert.IConvertStrategy interface and register it by calling Redwerb.BizArk.Core.Convert.ConvertStrategyMgr.RegisterStrategy.