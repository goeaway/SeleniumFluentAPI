using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class DefaultPageNotFoundException : SeleniumScriptException
    {
        public DefaultPageNotFoundException(string message) : base(message) { }
    }
}
