using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumScript.Components
{
    internal class ExecutionActionBase<TResult>
    {
        public string Name { get; } 
        public Func<IWebDriver, TResult> Action { get; }

        public ExecutionActionBase(string name, Func<IWebDriver, TResult> action)
        {
            Name = name;
            Action = action;
        }
    }
}
