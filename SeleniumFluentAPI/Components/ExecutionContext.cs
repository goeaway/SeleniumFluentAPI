using SeleniumScript.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SeleniumScript.Enums;

namespace SeleniumScript.Components
{
    public class ExecutionContext : IExecutionContext
    {
        public string Url { get; set; }
        public string ActionName { get; set; }
    }
}
