using System;
using System.Collections.Generic;
using System.Text;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Components
{
    public class ExecutionResult
    {
        public bool Success { get; private set; }
        public IExecutionContext Context { get; set; }
        public string Message { get; private set; }
        public Exception InnerException { get; private set; }

        public ExecutionResult(bool success, string currentUrl, ComponentType type, string currentAction)
        {
            Success = success;
            Context = ExecutionContext.GetContext(currentUrl, type, currentAction);
        }

        public ExecutionResult(bool success, string currentUrl, ComponentType type, string currentAction, string message)
        {
            Success = success;
            Context = ExecutionContext.GetContext(currentUrl, type, currentAction);
            Message = message;
        }

        public ExecutionResult(Exception e, string currentUrl, ComponentType type, string currentAction)
        {
            InnerException = e;
            Message = e.Message;
            Context = ExecutionContext.GetContext(currentUrl, type, currentAction);
        }
    }
}
