using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Components
{
    internal sealed class ExecutionAction : ExecutionActionBase<ExecutionResult>
    {
        public ExecutionAction(string name, Func<IWebDriver, ExecutionResult> action) : base(name, action) { }
    }
}
