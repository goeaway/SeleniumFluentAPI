using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IWait
    {
        IExecution Then { get; }
        IWait For(TimeSpan timeSpan);
        IWait For(TimeSpan timeSpan, string actionName);
        IWait ForElementToShow(By by, TimeSpan timeout);
        IWait ForElementToShow(By by, TimeSpan timeout, string actionName);
        IWait ForElementToHide(By by, TimeSpan timeout);
        IWait ForElementToHide(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeEnabled(By by, TimeSpan timeout);
        IWait ForElementToBeEnabled(By by, TimeSpan timeout, string actionName);
        IWait ForElementToBeDisabled(By by, TimeSpan timeout);
        IWait ForElementToBeDisabled(By by, TimeSpan timeout, string actionName);
    }
}
