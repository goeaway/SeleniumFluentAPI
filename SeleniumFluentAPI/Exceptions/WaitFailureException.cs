using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class WaitFailureException : Exception
    {
        public WaitFailureException() : base("Wait failed") { }

        public WaitFailureException(string message) : base(message) { }

        public WaitFailureException(string message, Exception innerException) : base(message, innerException) { }

        public WaitFailureException(Exception innerException) : base("Wait failed", innerException) { }
    }
}
