using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Components
{
    internal class AssertionAction : ExecutionActionBase<bool>
    {
        public AssertionAction(string name, Func<IWebDriver, bool> action) : base(name, action) { }
    }
}
