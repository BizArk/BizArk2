/* Author: Brian Brewder
 * Website: http://redwerb.com
 * 
 * This program is free software. It comes without any warranty, to
 * the extent permitted by applicable law. You can redistribute it
 * and/or modify it under the terms of the Do What The Fuck You Want
 * To Public License, Version 2, as published by Sam Hocevar. See
 * http://sam.zoy.org/wtfpl/COPYING for more details. 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redwerb.BizArk.Core
{
    /// <summary>
    /// Provides a tri-state boolean to allow something else to determine the value.
    /// </summary>
    public enum DefaultBoolean
    {
        /// <summary>Parent object determines value.</summary>
        Default,
        /// <summary>True</summary>
        True,
        /// <summary>False</summary>
        False
    }
}
