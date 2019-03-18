using System;
using System.Collections.Generic;
using System.Text;
using SeleniumFluentAPI.Abstractions;

namespace SeleniumFluentAPI.Executions
{
    public class ExecutionResult
    {
        public bool Success { get; private set; }
        public IExecutionContext Context { get; set; }
        public string Message { get; private set; }
        public Exception InnerException { get; private set; }

        public ExecutionResult(bool success, string currentUrl, string currentAction)
        {
            Success = success;
            Context = ExecutionContext.GetContext(currentUrl, currentAction);
        }

        public ExecutionResult(bool success, string currentUrl, string currentAction, string message)
        {
            Success = success;
            Context = ExecutionContext.GetContext(currentUrl, currentAction);
            Message = message;
        }

        public ExecutionResult(Exception e, string currentUrl, string currentAction)
        {
            InnerException = e;
            Message = e.Message;
            Context = ExecutionContext.GetContext(currentUrl, currentAction);
        }
    }
}
