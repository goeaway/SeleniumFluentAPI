using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumScript.Abstractions;
using SeleniumScript.Enums;
using SeleniumScript.Exceptions;
using SeleniumScript.Utilities;

namespace SeleniumScript.Components
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

        public IWait ForElementToExist(Locator locator, TimeSpan timeout)
        {
            return ForElementToExist(locator, timeout, "For Element To Exist");
        }

        public IWait ForElementToExist(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d =>
                {
                    IWebElement element = null;
                    try
                    {
                        element = locator.FindElement(d);
                    }
                    catch(NoSuchElementException) { }
                    catch(LocatorFindException) { }

                    return element != null;
                });
            }, actionName);

            return this;
        }

        public IWait ForElementToNotExist(Locator locator, TimeSpan timeout)
        {
            return ForElementToNotExist(locator, timeout, "For Element To Exist");
        }

        public IWait ForElementToNotExist(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d =>
                {
                    IWebElement element = null;
                    try
                    {
                        element = locator.FindElement(d);
                    }
                    catch (NoSuchElementException)
                    {
                        element = null;
                    }

                    return element == null;
                });
            }, actionName);

            return this;
        }

        public IWait ForElementToBeDisplayed(Locator locator, TimeSpan timeout)
        {
            return ForElementToBeDisplayed(locator, timeout, "For Element To Show");
        }

        public IWait ForElementToBeDisplayed(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => locator.FindElement(d).Displayed);
            }, actionName);

            return this;
        }

        public IWait ForElementToBeHidden(Locator locator, TimeSpan timeout)
        {
            return ForElementToBeHidden(locator, timeout, "For Element To Show");
        }

        public IWait ForElementToBeHidden(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => !locator.FindElement(d).Displayed);
            }, actionName);

            return this;
        }

        public IWait ForElementToBeEnabled(Locator locator, TimeSpan timeout)
        {
            return ForElementToBeEnabled(locator, timeout, "For Element To Be Enabled");
        }

        public IWait ForElementToBeEnabled(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => locator.FindElement(d).Enabled);
            }, actionName);

            return this;
        }

        public IWait ForElementToBeDisabled(Locator locator, TimeSpan timeout)
        {
            return ForElementToBeDisabled(locator, timeout, "For Element To Be Disabled");
        }

        public IWait ForElementToBeDisabled(Locator locator, TimeSpan timeout, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var waiter = new WebDriverWait(driver, timeout);
                return waiter.Until(d => !locator.FindElement(d).Enabled);
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
