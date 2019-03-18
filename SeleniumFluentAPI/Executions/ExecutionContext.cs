using SeleniumFluentAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Executions
{
    public class ExecutionContext : IExecutionContext
    {
        public string CurrentUrl { get; private set; }

        public string ActionName { get; private set; }

        private ExecutionContext()
        {

        }

        public static ExecutionContext GetContext(string currentUrl, string actionName)
            => new ExecutionContext() { CurrentUrl = currentUrl, ActionName = actionName};
    }
}
