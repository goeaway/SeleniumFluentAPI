using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class MultipleDefaultPagesFoundException : SeleniumScriptException
    {
        public MultipleDefaultPagesFoundException(string message) : base(message) { }
    }
}
