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
        private readonly IExecution _action;
        private readonly int _actionRetryCount;
        private readonly TimeSpan _actionRetryWaitPeriod;
        private readonly List<Func<bool>> _assertions;
        private readonly List<int> _assertionsToBeInverted;
        private readonly IWebDriver _driver;
        private readonly bool _throwOnFailure;

        public Assertion(IExecution action, IWebDriver driver, int actionRetryCount, TimeSpan actionRetryWaitPeriod, bool throwOnFailure)
        {
            if(actionRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(actionRetryCount));

            if(actionRetryWaitPeriod == null)
                throw new ArgumentNullException(nameof(actionRetryWaitPeriod));

            _action = action;
            _actionRetryCount = actionRetryCount;
            _actionRetryWaitPeriod = actionRetryWaitPeriod;
            _driver = driver;
            _throwOnFailure = throwOnFailure;
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
                    var result = _driver.Url == uri.ToString();

                    if(_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ToBeTrue(Func<bool> assertion)
        {
            _assertions.Add(()=>
            {
                try
                {
                    var result = assertion();

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ToBeAbleSeeElement(By by)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var result = element == null ? false : element.Displayed;
                    
                    if(_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if(_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ToBeAbleToClickElement(By by)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var result = element == null ? false : element.Enabled;

                    if(_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if(_throwOnFailure) 
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var result = predicate(element);

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
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
                    var result = cookie != null;

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

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

                    var result = false;

                    if (cookie != null)
                    {
                        result = predicate(cookie);
                    }

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ElementToHaveClass(By by, string className)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var result = element.GetAttribute("class").Contains(className);

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ElementToHaveAttr(By by, string attribute)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var result = !string.IsNullOrWhiteSpace(element.GetAttribute(attribute));

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IAssertion ElementToHaveAttr(By by, string attribute, string value)
        {
            _assertions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var attr = element.GetAttribute(attribute);

                    var result = !string.IsNullOrWhiteSpace(attr) && attr == value;

                    if (_throwOnFailure && !result)
                        throw new AssertionFailureException();

                    return result;
                }
                catch (Exception e)
                {
                    if (_throwOnFailure)
                        throw new AssertionFailureException(e);

                    return false;
                }
            });

            return this;
        }

        public IExecution Then
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
