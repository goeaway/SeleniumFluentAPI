using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class AssertionFailureException : SeleniumScriptException
    {
        public AssertionFailureException() : base("Assertion failed") { }

        public AssertionFailureException(string message) : base(message) { }

        public AssertionFailureException(string message, Exception innerException) : base(message, innerException) { }

        public AssertionFailureException(Exception innerException) : base("Assertion failed", innerException) { }
    }
}
