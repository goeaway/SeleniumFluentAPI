using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class DefaultPageNotFoundException : SeleniumScriptException
    {
        public DefaultPageNotFoundException(string message) : base(message) { }
    }
}
