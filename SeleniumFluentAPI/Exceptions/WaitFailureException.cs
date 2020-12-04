using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class WaitFailureException : Exception
    {
        public WaitFailureException()
        {
        }

        public WaitFailureException(string message) : base(message)
        {
        }

        public WaitFailureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected WaitFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
