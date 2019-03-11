using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Executions
{
    public class ExecutionResult
    {
        public bool Success { get; private set; }
        public string CurrentUrl { get; private set; }
        public string CurrentAction { get; private set; }
        public string Message { get; private set; }
        public Exception InnerException { get; private set; }

        public ExecutionResult(bool success, string currentUrl, string currentAction)
        {
            Success = success;
            CurrentUrl = currentUrl;
            CurrentAction = currentAction;
        }

        public ExecutionResult(bool success, string currentUrl, string currentAction, string message)
        {
            Success = success;
            CurrentUrl = currentUrl;
            CurrentAction = currentAction;
            Message = message;
        }

        public ExecutionResult(Exception e, string currentUrl, string currentAction)
        {
            InnerException = e;
            Message = e.Message;
            CurrentUrl = currentUrl;
            CurrentAction = currentAction;
        }
    }
}
