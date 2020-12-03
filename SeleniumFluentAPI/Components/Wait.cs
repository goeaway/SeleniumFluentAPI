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
                        .Or<LocatorFindException>()
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

        public IWait For(Func<IWebDriver, bool> predicate, TimeSpan timeout, string actionName = "For")
        {
            InnerAddWithPolicy(driver => new WebDriverWait(driver, timeout).Until(predicate), actionName);

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
