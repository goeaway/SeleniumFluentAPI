using SeleniumFluentAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Components
{
    public class ExecutionContext : IExecutionContext
    {
        public string CurrentUrl { get; private set; }
        public ComponentType Type { get; private set; }
        public string ActionName { get; private set; }

        private ExecutionContext()
        {

        }

        public static ExecutionContext GetContext(string currentUrl, ComponentType type, string actionName)
            => new ExecutionContext() { CurrentUrl = currentUrl, Type = type, ActionName = actionName};
    }
}
