# Web Helper

The primary purpose of the WebHelper class is to replace System.Web.WebClient. WebClient is a great class that makes it pretty easy to do simple web requests but unfortunately has some serious drawbacks that make it difficult to use in a production application.

WebClient only supports a small set of requests such as simple gets, form value posts, single file uploads, and a few others. If you need to upload multiple files in the same request, you are out of luck. If you have a request that might take more than 100 seconds, you are out of luck. If you need to do anything special at all, you are pretty much out of luck and will have to use the HttpRequest object to perform the request.

HttpRequest allows you to do a lot more, but you are stuck implementing the different protocols for the content body which can be difficult and time-consuming to get correct, not to mention that it doesn’t support progress reporting (you have to build that yourself).

The Redwerb.BizArk.Core.Web.WebHelper class is intended to provide you full access to making requests without bothering you with all the mundane details. It includes the following features:

* Supports no content type, multipart/form-data, and application/x-www-form-urlencoded. Will automatically pick the correct content type based on the properties you set. It can also support custom content types. Just inherit from Redwerb.BizArk.Core.Web.ContentType and implement the required methods.
* Can **set the timeout** for the request.
* Simple **multiple file upload** with form values (WebClient makes you send values in the query string when uploading files).
* Supports **in-memory file uploads**. The file doesn’t actually have to exist on disk.
* Handles **compressed responses** with a simple property set (on by default).
* Supports **progress**.
* Supports **asynchronous requests**.
* Simple static methods to handle most http requests.
* Event to modify the request before it is sent to the server. Provides you with full control over the request that is sent.
* Event to process the response instead of having the WebHelper handle it.
* The response is returned as a WebHelperResponse object. This object provides access to the result as well as status of the response. It also allows you to convert the response to another type other than a byte[]() (such as string, image, etc).

To use the WebHelper, just call one of the convenient static methods.

{{
    var response = WebHelper.MakeRequest("http://mydomain.com/Test");
    var result = response.ConvertResult<bool>();
}}

If you want to upload a file and some form variables, that’s easy too! Just set the dynamic Parameters property.

{{
    var response = WebHelper.MakeRequest("http://mydomain.com/test",  
        new 
        { 
            Test = "Hello", 
            File1 = new UploadFile(@"text\plain", "file1.txt", new MemoryStream(Encoding.UTF8.GetBytes("Hello World"))), 
            File2 = new FileInfo("MyFile.txt")
        });
    var result = response.ResultToString();  
}}
The Parameters property can understand UploadFile, FileInfo (will upload the file using the MimeType from the BizArk MimeMap), byte arrays, and any other object. If not a file or byte array, WebHelper will use ConvertEx.ToString() to get the value to upload.

Here's an example of using one of the asynchronous methods...

{{
    var done = WebHelper.MakeRequestAsync("http://www.google.com", new WebHelperOptions()
        {
            RequestComplete = (web, response, ex, cancelled) =>
            {
                if (ex != null)
                {
                    // handle error
                }
                if (cancelled) return;

                var result = response.ResultToString();
            }
        });
    done.WaitOne();
}}

_Special thanks goes to aspnetupload.com. WebHelper essentially started out as a copy of UploadHelper from this site (though I don’t think there is too much in common with it anymore)._