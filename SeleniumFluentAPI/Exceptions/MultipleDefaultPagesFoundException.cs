using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class MultipleDefaultPagesFoundException : SeleniumScriptException
    {
        public MultipleDefaultPagesFoundException(string message) : base(message) { }
    }
}
