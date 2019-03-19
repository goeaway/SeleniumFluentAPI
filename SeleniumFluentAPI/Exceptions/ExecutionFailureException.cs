using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class ExecutionFailureException : Exception
    {
        public ExecutionFailureException() : base("Execution failed") { }

        public ExecutionFailureException(string message) : base(message) { }

        public ExecutionFailureException(string message, Exception innerException) : base(message, innerException) { }

        public ExecutionFailureException(Exception innerException) : base("Execution failed", innerException) { }
    }
}
