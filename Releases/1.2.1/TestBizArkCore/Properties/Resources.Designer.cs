﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.208
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TestBizArkCore.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TestBizArkCore.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        internal static System.Drawing.Bitmap TestImg {
            get {
                object obj = ResourceManager.GetObject("TestImg", resourceCulture);
                return ((System.Drawing.Bitmap)(obj));
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dear ${name},
        ///
        ///Thank you for purchasing a ${product}. We received your order on ${orderdate:g} and billed your credit card ${total} on ${billdate}. 
        ///
        ///The order ${shipped:has|hasn&apos;t} been shipped.
        ///
        ///Sincerely,
        ///${repname}.
        /// </summary>
        internal static string TextTemplateTest {
            get {
                return ResourceManager.GetString("TextTemplateTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;methodResponse&gt;
        ///  &lt;fault&gt;
        ///    &lt;value&gt;
        ///      &lt;struct&gt;
        ///        &lt;member&gt;
        ///          &lt;name&gt;faultCode&lt;/name&gt;
        ///          &lt;value&gt;
        ///            &lt;int&gt;1&lt;/int&gt;
        ///          &lt;/value&gt;
        ///        &lt;/member&gt;
        ///        &lt;member&gt;
        ///          &lt;name&gt;faultString&lt;/name&gt;
        ///          &lt;value&gt;
        ///            &lt;string&gt;ERROR&lt;/string&gt;
        ///          &lt;/value&gt;
        ///        &lt;/member&gt;
        ///      &lt;/struct&gt;
        ///    &lt;/value&gt;
        ///  &lt;/fault&gt;
        ///&lt;/methodResponse&gt;.
        /// </summary>
        internal static string ValidRpcFaultResponse {
            get {
                return ResourceManager.GetString("ValidRpcFaultResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;methodResponse&gt;
        ///  &lt;params&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;string&gt;Hello Christine&lt;/string&gt;
        ///      &lt;/value&gt;
        ///    &lt;/param&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;int&gt;4&lt;/int&gt;
        ///      &lt;/value&gt;
        ///    &lt;/param&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;boolean&gt;1&lt;/boolean&gt;
        ///      &lt;/value&gt;
        ///    &lt;/param&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;double&gt;1.3&lt;/double&gt;
        ///      &lt;/value&gt;
        ///    &lt;/param&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;dateTime.iso8601&gt;2010-01-01T04:00:00&lt;/dateTime.is [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string ValidRpcParamsResponse {
            get {
                return ResourceManager.GetString("ValidRpcParamsResponse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;methodResponse&gt;
        ///  &lt;params&gt;
        ///    &lt;param&gt;
        ///      &lt;value&gt;
        ///        &lt;string&gt;SUCCESS&lt;/string&gt;
        ///      &lt;/value&gt;
        ///    &lt;/param&gt;
        ///  &lt;/params&gt;
        ///&lt;/methodResponse&gt;.
        /// </summary>
        internal static string XmlRpcInvokeTest {
            get {
                return ResourceManager.GetString("XmlRpcInvokeTest", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
        ///&lt;methodCall&gt;
        /// &lt;methodName&gt;MyMethod&lt;/methodName&gt;
        /// &lt;params&gt;
        ///  &lt;param&gt;
        ///   &lt;value&gt;
        ///    &lt;string&gt;String Value&lt;/string&gt;
        ///   &lt;/value&gt;
        ///  &lt;/param&gt;
        ///   &lt;param&gt;
        ///     &lt;value&gt;
        ///       &lt;int&gt;1&lt;/int&gt;
        ///     &lt;/value&gt;
        ///   &lt;/param&gt;
        ///   &lt;param&gt;
        ///     &lt;value&gt;
        ///       &lt;i4&gt;2&lt;/i4&gt;
        ///     &lt;/value&gt;
        ///   &lt;/param&gt;
        ///   &lt;param&gt;
        ///     &lt;value&gt;
        ///       &lt;double&gt;3.1&lt;/double&gt;
        ///     &lt;/value&gt;
        ///   &lt;/param&gt;
        ///   &lt;param&gt;
        ///     &lt;value&gt;
        ///       &lt;dateTime.iso8601&gt;1998-07-17T14:08:55&lt;/dateTime.iso8601&gt;
        ///     &lt; [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string XmlRpcTest {
            get {
                return ResourceManager.GetString("XmlRpcTest", resourceCulture);
            }
        }
    }
}
