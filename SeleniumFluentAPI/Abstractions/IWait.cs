using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumScript.Abstractions
{
    public interface IWait
    {
        IExecution Then { get; }
        IWait ForElementToExist(By by, TimeSpan timeout);
        IWait ForElementToExist(By by, TimeSpan timeout, string actionName);
        IWait ForElementToNotExist(By by, TimeSpan timeout);
        IWait ForElementToNotExist(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeDisplayed(By by, TimeSpan timeout);
        IWait ForElementToBeDisplayed(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeHidden(By by, TimeSpan timeout);
        IWait ForElementToBeHidden(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeEnabled(By by, TimeSpan timeout);
        IWait ForElementToBeEnabled(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeDisabled(By by, TimeSpan timeout);
        IWait ForElementToBeDisabled(By by, TimeSpan timeout, string actionName);
    }
}
