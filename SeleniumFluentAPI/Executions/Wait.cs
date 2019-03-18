using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using Polly;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Executions
{
    public class Wait : IWait
    {
        private readonly IExecution _execution;
        private readonly IWebDriver _driver;
        private readonly List<Func<ExecutionResult>> _waits;

        public Wait(IExecution execution, IWebDriver driver)
        {
            _execution = execution;
            _driver = driver;
            _waits = new List<Func<ExecutionResult>>();
        }

        public IWait ForElementToShow(By by, TimeSpan timeout)
        {
            _waits.Add(() =>
            {
                    IWebElement element = null;

                    Policy.Handle<WebDriverException>()
                        .WaitAndRetry(1, (tryNum) => timeout, (e, time, tryNum, context) =>
                        {
                            element = _driver.FindElement(by);

                            if (element == null || !element.Displayed)
                            {
                                throw new WebDriverException("Could not see or find element");
                            }
                        });

                    if (element == null || !element.Displayed)
                        return new ExecutionResult(false, _driver.Url, "Wait For Element To Show", "element did not show in time");

                    return new ExecutionResult(true, _driver.Url, "Wait For Element To Show", "waited for element to show");
            });

            return this;
        }

        public IWait For(TimeSpan timeSpan)
        {
            _waits.Add(() =>
            {
                    try
                    {
                        // might want to change to better imp
                        Thread.Sleep(timeSpan);
                        return new ExecutionResult(true, _driver.Url, "Wait For TimeSpan");
                    }
                    catch (Exception e)
                    {
                        return new ExecutionResult(e, _driver.Url, "Wait For TimeSpan");
                    }
            });

            return this;
        }

        public IWait ForElementToHide(By by, TimeSpan timeout)
        {
            _waits.Add(() =>
            {
                IWebElement element = null;

                Policy.Handle<WebDriverException>()
                    .WaitAndRetry(1, (tryNum) => timeout, (e, time, tryNum, context) =>
                    {
                        element = _driver.FindElement(by);

                        if (element == null || element.Displayed)
                        {
                            throw new WebDriverException("Searching for an element, or it being visible resulted in an exception");
                        }
                    });

                if (element == null || element.Displayed)
                    return new ExecutionResult(false, _driver.Url, "Wait For Element To Hide", "element did not hide in time");

                return new ExecutionResult(true, _driver.Url, "Wait For Element To Hide");
            });

            return this;
        }

        public IWait ForElementToBeEnabled(By by, TimeSpan timeout)
        {
            _waits.Add(() =>
            {
                IWebElement element = null;

                Policy.Handle<WebDriverException>()
                    .WaitAndRetry(1, (tryNum) => timeout, (e, time, tryNum, context) =>
                    {
                        element = _driver.FindElement(by);

                        if (element == null || !element.Enabled)
                        {
                            throw new WebDriverException("Searching for an element, or it being disabled resulted in an exception");
                        }
                    });

                if (element == null || !element.Enabled)
                    return new ExecutionResult(false, _driver.Url, "Wait For Element To Be Enabled", "element was not enabled in time");

                return new ExecutionResult(true, _driver.Url, "Wait For Element To Be Enabled", "waited for element to be enabled");
            });

            return this;
        }

        public IWait ForElementToBeDisabled(By by, TimeSpan timeout)
        {
            _waits.Add(() =>
            {
                IWebElement element = null;

                Policy.Handle<WebDriverException>()
                    .WaitAndRetry(1, (tryNum) => timeout, (e, time, tryNum, context) =>
                    {
                        element = _driver.FindElement(by);

                        if (element == null || element.Enabled)
                        {
                            throw new WebDriverException("Searching for an element, or it being enabled resulted in an exception");
                        }
                    });

                if (element == null || element.Enabled)
                    return new ExecutionResult(false, _driver.Url, "Wait For Element To Be Disabled", "element was not disabled in time");

                return new ExecutionResult(true, _driver.Url, "Wait For Element To Be Disabled", "waited for element to hide");
            });

            return this;
        }

        public IExecution Then
        {
            get
            {
                foreach (var wait in _waits)
                {
                    _execution.Add(wait);
                }

                return _execution;
            }
        }
    }
}
