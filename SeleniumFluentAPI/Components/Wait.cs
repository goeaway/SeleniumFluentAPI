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
        private readonly int _retryCount;
        private readonly ICollection<TimeSpan> _retryWaitPeriods;

        public Wait(IExecution execution, int retryCount, ICollection<TimeSpan> retryWaitPeriods)
        {
            _execution = execution;
            _retryCount = retryCount;
            _retryWaitPeriods = retryWaitPeriods;
            _waits = new List<UtilityAction>();
        }

        private void InnerAddWithPolicy(Func<IWebDriver, bool> action, string actionName)
        {
            _waits.Add(new UtilityAction(actionName, driver =>
            {
                return Policy
                    .Handle<WebDriverException>()
                    .Or<LocatorFindException>()
                    .WaitAndRetry(
                        _retryCount, 
                        (tryNum) => RetryWaitCalculator.GetTimeSpanForWait(tryNum, _retryWaitPeriods))
                    .Execute(() =>
                    {
                       return action(driver);
                    });
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
                    _execution.Custom(driver =>
                    {
                        var result = wait.Action(driver);


                    }, wait.Name);
                }

                return _execution;
            }
        }
    }
}
