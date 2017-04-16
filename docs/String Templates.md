# String Templates

I’ve never liked that String.Format requires you to provide indexed arguments. I’m a bit anal and like my arguments to increase from left to right so that if I add a new argument to the beginning of the string I end up renumbering all the subsequent arguments (hey DevEx, perhaps you can add this to Refactor!?). 

This isn’t too big of a deal for small strings, but it can make it difficult to work with large strings with a lot of arguments, especially when the text and arguments change frequently. To get around this, I often times resort to using named parameters and String.Replace (see the example below).

{{
var str = "{greeting} {name}!";
str = str.Replace("{greeting}", "Hello");
str = str.Replace("{name}", "World");
Debug.WriteLine(str);
}}
Surprisingly performance doesn’t seem to be a major problem with this approach. Iterating 1,000,000 times using string.Format("Hello {0}: {1,2}", "World", i) took ~650 milliseconds vs string.Replace which took about ~900 milliseconds. This might add up to a significant difference in extreme situations, but in most cases it is negligible.

However one major drawback to string.Replace is that you lose the ability to specify formatting in the string which can be very handy, especially when you are dealing with regional formatting issues.

The StringTemplate class in BizArk allows you to create a format string that combines the best of both worlds, named arguments along with specifying formatting in the string (see example below).

{{
var template = new StringTemplate("{greeting} {name} on {date:dddd, MMMM dd, yyyy}!");
template["greeting"](_greeting_) = "Hello";
template["name"](_name_) = "World";
template["date"](_date_) = DateTime.Now;
Debug.WriteLine(template.ToString());
}}
This will print out “Hello World on Thursday, August 06, 2009!”.

StringTemplate actually uses String.Format to format the string so any formatting that you can use in a String.Format argument you can use in a StringTemplate argument. The names are not case sensitive, but they must conform to the same standard as identifiers in C# (upper/lower case letters, numbers, and underscore).

There is also a generic version of StringTemplate. The generic version exposes a property that you can use to set the parameters. If the type has a default constructor, StringTemplate<T> will instantiate it for you (you can also set it if you prefer).  The name of the arguments should match with the name of the properties. It uses the TypeDescriptor class to get the properties, so you can implement ICustomTypeDescriptor if you want. This should make it fairly easy and painless to do mail-merge type functionality.