using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Exceptions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Components
{
    public class Wait : IWait
    {
        private readonly IExecution _execution;
        private readonly List<ExecutionAction> _waits;
        private readonly bool _throwOnException;
        private readonly int _retryCount;
        private readonly TimeSpan _retryWaitPeriod;

        public Wait(IExecution execution, int retryCount, TimeSpan retryWaitPeriod, bool throwOnException)
        {
            _execution = execution;
            _throwOnException = throwOnException;
            _retryCount = retryCount;
            _retryWaitPeriod = retryWaitPeriod;
            _waits = new List<ExecutionAction>();
        }

        private void InnerAddWithPolicy(Func<IWebDriver, ExecutionResult> action, string actionName)
        {
            _waits.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    return Policy
                        .Handle<WebDriverException>()
                        .WaitAndRetry(_retryCount, (tryNum) => _retryWaitPeriod)
                        .Execute(() =>
                        {
                            return action(driver);
                        });
                }
                catch (Exception e)
                {
                    if (_throwOnException)
                        throw new WaitFailureException(e);

                    return new ExecutionResult(e, driver.Url, actionName);
                }
            }));
        }

        public IWait ForElementToShow(By by, TimeSpan timeout)
        {
            return ForElementToShow(by, timeout, "For Element To Show");
        }

        public IWait ForElementToShow(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null || !element.Displayed)
                    return new ExecutionResult(false, driver.Url, actionName, "element did not show in time");

                return new ExecutionResult(true, driver.Url, actionName, "waited for element to show");
            }, actionName);

            return this;
        }

        public IWait For(TimeSpan timeSpan)
        {
            return For(timeSpan, "For");
        }

        public IWait For(TimeSpan timeSpan, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                // might want to change to better imp
                Thread.Sleep(timeSpan);
                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IWait ForElementToHide(By by, TimeSpan timeout)
        {
            return ForElementToHide(by, timeout, "For Element To Show");
        }

        public IWait ForElementToHide(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null || element.Displayed)
                    return new ExecutionResult(false, driver.Url, actionName, "element did not hide in time");

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IWait ForElementToBeEnabled(By by, TimeSpan timeout)
        {
            return ForElementToBeEnabled(by, timeout, "For Element To Be Enabled");
        }

        public IWait ForElementToBeEnabled(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null || !element.Enabled)
                    return new ExecutionResult(false, driver.Url, actionName, "element was not enabled in time");

                return new ExecutionResult(true, driver.Url, actionName, "waited for element to be enabled");
            }, actionName);

            return this;
        }

        public IWait ForElementToBeDisabled(By by, TimeSpan timeout)
        {
            return ForElementToBeDisabled(by, timeout, "For Element To Be Disabled");
        }

        public IWait ForElementToBeDisabled(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null || element.Enabled)
                    return new ExecutionResult(false, driver.Url, actionName, "element was not disabled in time");

                return new ExecutionResult(true, driver.Url, actionName, "waited for element to hide");
            }, actionName);

            return this;
        }

        public IExecution Then
        {
            get
            {
                foreach (var wait in _waits)
                {
                    _execution.Add(wait.Action, wait.Name);
                }

                return _execution;
            }
        }
    }
}
