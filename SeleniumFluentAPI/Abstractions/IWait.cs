using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IWait
    {
        IExecutable Then { get; }
        IWait For(TimeSpan timeSpan);
        IWait ForElementToShow(By by, TimeSpan timeout);
        IWait ForElementToHide(By by, TimeSpan timeout);
        IWait ForElementToBeEnabled(By by, TimeSpan timeout);
        IWait ForElementToBeDisabled(By by, TimeSpan timeout);
    }
}
