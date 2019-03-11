using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Exceptions
{
    public class MultipleDefaultPagesFoundException : Exception
    {
        public MultipleDefaultPagesFoundException(string message) : base(message) { }
    }
}
