using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;
using SeleniumFluentAPI.Exceptions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Components
{
    public class Wait : IWait
    {
        private readonly IExecution _execution;
        private readonly List<UtilityAction> _waits;
        private readonly bool _throwOnException;
        private readonly int _retryCount;
        private readonly TimeSpan _retryWaitPeriod;

        public Wait(IExecution execution, int retryCount, TimeSpan retryWaitPeriod, bool throwOnException)
        {
            _execution = execution;
            _throwOnException = throwOnException;
            _retryCount = retryCount;
            _retryWaitPeriod = retryWaitPeriod;
            _waits = new List<UtilityAction>();
        }

        private void InnerAddWithPolicy(Func<IWebDriver, bool> action, string actionName)
        {
            _waits.Add(new UtilityAction(actionName, driver =>
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

                    return false;
                }
            }));
        }

        public IWait ForElementToExist(By by, TimeSpan timeout)
        {
            return ForElementToExist(by, timeout, "For Element To Exist");
        }

        public IWait ForElementToExist(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                var result = waiter.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(by));
                // if we got here the element exists
                return true;
            }, actionName);

            return this;
        }

        public IWait ForElementToNotExist(By by, TimeSpan timeout)
        {
            return ForElementToNotExist(by, timeout, "For Element To Exist");
        }

        public IWait ForElementToNotExist(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var start = DateTime.Now;
                IWebElement element = null;
                do
                {
                    try
                    {
                        element = driver.FindElement(by);
                    }
                    catch (NoSuchElementException)
                    {
                        element = null;
                    }
                } while (element == null || (DateTime.Now - start) > timeout);

                return element == null;
            }, actionName);

            return this;
        }

        public IWait ForElementToBeDisplayed(By by, TimeSpan timeout)
        {
            return ForElementToBeDisplayed(by, timeout, "For Element To Show");
        }

        public IWait ForElementToBeDisplayed(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => d.FindElement(by).Displayed);
            }, actionName);

            return this;
        }

        public IWait ForElementToBeHidden(By by, TimeSpan timeout)
        {
            return ForElementToBeHidden(by, timeout, "For Element To Show");
        }

        public IWait ForElementToBeHidden(By by, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => !d.FindElement(by).Displayed);
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
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => d.FindElement(by).Enabled);
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
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => !d.FindElement(by).Enabled);
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
