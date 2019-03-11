using OpenQA.Selenium;
using Polly;
using SeleniumFluentAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using SeleniumFluentAPI.Utilities;
using SeleniumFluentAPI.Exceptions;

namespace SeleniumFluentAPI.Executions
{
    public class Assertion : IAssertion
    {
        private readonly IExecutable _action;
        private readonly int _actionRetryCount;
        private readonly TimeSpan _actionRetryWaitPeriod;
        private readonly List<Func<bool>> _assertions;
        private readonly List<int> _assertionsToBeInverted;
        private readonly IWebDriver _driver;

        public Assertion(IExecutable action, IWebDriver driver, int actionRetryCount, TimeSpan actionRetryWaitPeriod)
        {
            if(actionRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(actionRetryCount));

            if(actionRetryWaitPeriod == null)
                throw new ArgumentNullException(nameof(actionRetryWaitPeriod));

            _action = action;
            _actionRetryCount = actionRetryCount;
            _actionRetryWaitPeriod = actionRetryWaitPeriod;
            _driver = driver;
            _assertions = new List<Func<bool>>();
            _assertionsToBeInverted = new List<int>();
        }

        public IAssertion ToBeOn(IPage page)
        {
            return ToBeOn(page.FullUri);
        }

        public IAssertion ToBeOn(Uri uri)
        {
            _assertions.Add(() =>
            {
                    try
                    {
                        return _driver.Url == uri.ToString();
                    }
                    catch (Exception)
                    {
                        return false;
                    }
            });

            return this;
        }

        public IAssertion ToBeTrue(Func<bool> assertion)
        {
            _assertions.Add(()=>
            {
                    return assertion();
            });

            return this;
        }

        public IAssertion ToBeAbleSeeElement(By by)
        {
            _assertions.Add(() =>
            {
                    var element = GetElement(by);
                    return element == null ? false : element.Displayed;
            });

            return this;
        }

        public IAssertion ToBeAbleToClickElement(By by)
        {
            _assertions.Add(() =>
            {
                    var element = GetElement(by);
                    return element == null ? false : element.Enabled;
            });

            return this;
        }

        public IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate)
        {
            _assertions.Add(() =>
            {
                    var element = GetElement(by);
                    return predicate(element);
            });

            return this;
        }

        public IAssertion CookieToExist(string cookieName)
        {
            _assertions.Add(() =>
            {
                    try
                    {
                        var cookie = _driver.Manage().Cookies.GetCookieNamed(cookieName);
                        return cookie != null;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
            });

            return this;
        }

        public IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate)
        {
            _assertions.Add(() =>
            {
                    try
                    {
                        var cookie = _driver.Manage().Cookies.GetCookieNamed(cookieName);

                        if (cookie != null)
                            return predicate(cookie);

                        return false;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
            });

            return this;
        }

        public IAssertion ElementToHaveClass(By by, string className)
        {
            _assertions.Add(() =>
            {
                var element = GetElement(by);
                return element.GetAttribute("class").Contains(className);
            });

            return this;
        }

        public IAssertion ElementToHaveAttr(By by, string attribute)
        {
            _assertions.Add(() =>
            {
                var element = GetElement(by);
                return !string.IsNullOrWhiteSpace(element.GetAttribute(attribute));
            });

            return this;
        }

        public IAssertion ElementToHaveAttr(By by, string attribute, string value)
        {
            _assertions.Add(() =>
            {
                var element = GetElement(by);
                var attr = element.GetAttribute(attribute);

                if (!string.IsNullOrWhiteSpace(attr))
                    return false;

                return attr == value;
            });

            return this;

                
        }

        public IExecutable Then
        {
            get
            {
                for (int i = 0; i < _assertions.Count; i++)
                {
                    var assertion = _assertions[i];
                    var invert = _assertionsToBeInverted.Contains(i);

                    _action.Add(() =>
                    {
                        try
                        {
                            bool result = false;

                            if (invert)
                                result = !assertion();
                            else
                                result = assertion();
                            if (!result)
                                throw new AssertionFailureException("Assertion failed");

                            return new ExecutionResult(result, _driver.Url, "Assertion");
                        }
                        catch (Exception e)
                        {
                            return new ExecutionResult(e, _driver.Url, "Assertion");
                        }
                    });
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

        private IWebElement GetElement(By by)
        {
            IWebElement element = null;

            Policy
                .Handle<WebDriverException>()
                .WaitAndRetry(_actionRetryCount, (tryNum) => _actionRetryWaitPeriod,
                    (exception, wait, tryNum, context) =>
                    {
                        element = _driver.FindElement(by);
                    });

            return element;
        }
    }
}
