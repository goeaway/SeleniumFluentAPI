using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class AssertionFailureException : Exception
    {
        public AssertionFailureException(string message) : base(message) { }
    }
}
