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
        IWait ForElementToExist(Locator locator, TimeSpan timeout);
        IWait ForElementToExist(Locator locator, TimeSpan timeout, string actionName);
        IWait ForElementToNotExist(Locator locator, TimeSpan timeout);
        IWait ForElementToNotExist(Locator locator, TimeSpan timeout, string actionName);
        IWait ForElementToBeDisplayed(Locator locator, TimeSpan timeout);
        IWait ForElementToBeDisplayed(Locator locator, TimeSpan timeout, string actionName);
        IWait ForElementToBeHidden(Locator locator, TimeSpan timeout);
        IWait ForElementToBeHidden(Locator locator, TimeSpan timeout, string actionName);
        IWait ForElementToBeEnabled(Locator locator, TimeSpan timeout);
        IWait ForElementToBeEnabled(Locator locator, TimeSpan timeout, string actionName);
        IWait ForElementToBeDisabled(Locator locator, TimeSpan timeout);
        IWait ForElementToBeDisabled(Locator locator, TimeSpan timeout, string actionName);
    }
}
