using OpenQA.Selenium;
using Polly;
using SeleniumScript.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Support.Extensions;
using SeleniumScript.Utilities;
using SeleniumScript.Exceptions;
using System.IO;
using System.Text.RegularExpressions;
using SeleniumScript.Enums;

namespace SeleniumScript.Components
{
    public class Assertion : IAssertion
    {
        private readonly IExecution _action;
        private readonly int _actionRetryCount;
        private readonly ICollection<TimeSpan> _actionRetryWaitPeriods;
        private readonly List<UtilityAction> _assertions;
        private readonly List<int> _assertionsToBeInverted;

        public Assertion(IExecution action, int actionRetryCount, ICollection<TimeSpan> actionRetryWaitPeriods)
        {
            if(actionRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(actionRetryCount));

            if(actionRetryWaitPeriods == null)
                throw new ArgumentNullException(nameof(actionRetryWaitPeriods));

            _action = action;
            _actionRetryCount = actionRetryCount;
            _actionRetryWaitPeriods = actionRetryWaitPeriods;
            _assertions = new List<UtilityAction>();
            _assertionsToBeInverted = new List<int>();
        }

        private IAssertion InnerAddWithPolicy(Func<IWebDriver, bool> func, string actionName)
        {
            _assertions.Add(new UtilityAction(actionName, driver => Policy
                    .Handle<WebDriverException>()
                    .Or<LocatorFindException>()
                    .WaitAndRetry(
                        _actionRetryCount, 
                        (tryNum) => RetryWaitCalculator.GetTimeSpanForWait(tryNum, _actionRetryWaitPeriods))
                    .Execute(() => func(driver))));
            return this;
        }

        public IAssertion ToBe(Func<IWebDriver, bool> predicate, string actionName = "ToBe")
        {
            return InnerAddWithPolicy(predicate, actionName);
        }

        public IAssertion ElementToBe(Locator locator, Func<IWebElement, bool> predicate, string actionName = "ElementToBe")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return predicate(element);
            }, actionName);
        }

        public IExecution Then
        {
            get
            {
                for (int i = 0; i < _assertions.Count; i++)
                {
                    var assertionAction = _assertions[i];
                    var invert = _assertionsToBeInverted.Contains(i);

                    _action.Custom(driver =>
                    {
                        var result = assertionAction.Action(driver);

                        if (invert)
                        {
                            result = !result;
                        }

                        if(!result)
                        {
                            throw new AssertionFailureException();
                        }
                    }, assertionAction.Name);
                }

                return _action;
            }
        }
        public IAssertion Not
        {
            get
            {
                // flag the next assertion to be added for inversion
                _assertionsToBeInverted.Add(_assertions.Count);

                return this;
            }
        }
    }
}
