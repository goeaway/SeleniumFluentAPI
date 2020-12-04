using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class UtilityException : Exception
    {
        public UtilityException()
        {
        }

        public UtilityException(string message) : base(message)
        {
        }

        public UtilityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UtilityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
