using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumScript.Utilities;

namespace SeleniumScript.Abstractions
{
    public interface IWait
    {
        IExecution Then { get; }
        IWait For(Func<IWebDriver, bool> predicate, TimeSpan timeout, string actionName = "For");
    }
}
