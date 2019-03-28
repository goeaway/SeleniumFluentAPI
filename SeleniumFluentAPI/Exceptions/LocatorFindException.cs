using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class LocatorFindException : SeleniumScriptException
    {
        public LocatorFindException(string message) : base(message)
        {
        }
    }
}
