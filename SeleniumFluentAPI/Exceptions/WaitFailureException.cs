﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class WaitFailureException : SeleniumScriptException
    {
        public WaitFailureException() : base("Wait failed") { }

        public WaitFailureException(string message) : base(message) { }

        public WaitFailureException(string message, Exception innerException) : base(message, innerException) { }

        public WaitFailureException(Exception innerException) : base("Wait failed", innerException) { }
    }
}
