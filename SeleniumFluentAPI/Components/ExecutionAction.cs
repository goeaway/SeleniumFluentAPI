using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumScript.Components
{
    internal sealed class ExecutionAction : ExecutionActionBase<ActionResult>
    {
        public ExecutionAction(string name, Func<IWebDriver, ActionResult> action) : base(name, action) { }
    }
}
