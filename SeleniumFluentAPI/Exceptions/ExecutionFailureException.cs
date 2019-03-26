using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class ExecutionFailureException : SeleniumScriptException
    {
        public ExecutionFailureException() : base("Execution failed") { }

        public ExecutionFailureException(string message) : base(message) { }

        public ExecutionFailureException(string message, Exception innerException) : base(message, innerException) { }

        public ExecutionFailureException(Exception innerException) : base("Execution failed", innerException) { }
    }
}
