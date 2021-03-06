﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Exceptions
{
    public class SeleniumScriptException : Exception
    {
        public SeleniumScriptException(string message) : base(message) { }

        public SeleniumScriptException(string message, Exception innerException) : base(message, innerException) { }

    }
}
