using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace BizArk.Core.Template
{

    /// <summary>
    /// Provides a way to format a string using an argument object instead of positional parameters. Uses DataBinder.Eval to get the values from the object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StringTemplate<T>
    {

        #region Initialization and Destruction

        /// <summary>
        /// Creates an instance of StringTemplate.
        /// </summary>
        /// <param name="template"></param>
        public StringTemplate(string template)
        {
            mTemplate = new StringTemplate(template);
        }

        #endregion

        #region Fields and Properties

        private StringTemplate mTemplate;
        /// <summary>
        /// Gets the template string.
        /// </summary>
        public string Template
        {
            get { return mTemplate.Template; }
        }

        private T mArgs;
        /// <summary>
        /// Gets or sets the argument object. If not set, one will be automatically created.
        /// </summary>
        public T Args
        {
            get
            {
                if (mArgs == null)
                    mArgs = Activator.CreateInstance<T>();
                return mArgs;
            }
            set { mArgs = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the formatted string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // We don't require any change notification from the type so we 
            // must bind just before calling the template.
            foreach (var argName in mTemplate.ArgNames)
            {
                // set values.
                var val = DataBinder.Eval(mArgs, argName);
                mTemplate[argName] = val;
            }
            return mTemplate.ToString();
        }

        #endregion

    }

}
