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
        private readonly TimeSpan _actionRetryWaitPeriod;
        private readonly List<UtilityAction> _assertions;
        private readonly List<int> _assertionsToBeInverted;
        private readonly bool _throwOnFailure;

        public Assertion(IExecution action, int actionRetryCount, TimeSpan actionRetryWaitPeriod, bool throwOnFailure)
        {
            if(actionRetryCount < 0)
                throw new ArgumentOutOfRangeException(nameof(actionRetryCount));

            if(actionRetryWaitPeriod == null)
                throw new ArgumentNullException(nameof(actionRetryWaitPeriod));

            _action = action;
            _actionRetryCount = actionRetryCount;
            _actionRetryWaitPeriod = actionRetryWaitPeriod;
            _throwOnFailure = throwOnFailure;
            _assertions = new List<UtilityAction>();
            _assertionsToBeInverted = new List<int>();
        }

        private void InnerAddWithPolicy(Func<IWebDriver, bool> func, string actionName)
        {
            _assertions.Add(new UtilityAction(actionName, driver =>
            {
                return Policy
                    .Handle<WebDriverException>()
                    .Or<LocatorFindException>()
                    .WaitAndRetry(_actionRetryCount, (tryNum) => _actionRetryWaitPeriod)
                    .Execute(() =>
                    {
                        return func(driver);
                    });
            }));
        }

        public IAssertion ToBeOn(IPage page)
        {
            return ToBeOn(page, "To Be On");
        }

        public IAssertion ToBeOn(IPage page, string actionName)
        {
            return ToBeOn(page.FullUri, actionName);
        }

        public IAssertion ToBeOn(Uri uri)
        {
            return ToBeOn(uri, "To Be On");
        }

        public IAssertion ToBeOn(Uri uri, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                return driver.Url == uri.ToString();
            }, actionName);

            return this;
        }

        public IAssertion ToBeTrue(Func<IWebDriver, bool> assertion)
        {
            return ToBeTrue(assertion, "Custom Assertion");
        }

        public IAssertion ToBeTrue(Func<IWebDriver, bool> assertion, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                return assertion(driver);
            }, actionName);

            return this;
        }

        public IAssertion ToBeAbleSeeElement(Locator locator)
        {
            return ToBeAbleSeeElement(locator, "To Be Able To See Element");
        }

        public IAssertion ToBeAbleSeeElement(Locator locator, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return element == null ? false : element.Displayed;
            }, actionName);

            return this;
        }

        public IAssertion ToBeAbleToClickElement(Locator locator)
        {
            return ToBeAbleToClickElement(locator, "To Be Able To Click Element");
        }

        public IAssertion ToBeAbleToClickElement(Locator locator, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return element == null ? false : element.Enabled;
            }, actionName);

            return this;
        }

        public IAssertion ElementToPassThis(Locator locator, Predicate<IWebElement> predicate)
        {
            return ElementToPassThis(locator, predicate, "Custom Element Assertion");
        }

        public IAssertion ElementToPassThis(Locator locator, Predicate<IWebElement> predicate, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return predicate(element);
            }, actionName);

            return this;
        }

        public IAssertion ElementsToPassThis(By by, Predicate<IReadOnlyCollection<IWebElement>> predicate)
        {
            return ElementsToPassThis(by, predicate, "Elements To Pass This");
        }

        public IAssertion ElementsToPassThis(By by, Predicate<IReadOnlyCollection<IWebElement>> predicate, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var elements = driver.FindElements(by);
                return predicate(elements);
            }, actionName);

            return this;
        }

        public IAssertion CookieToExist(string cookieName)
        {
            return CookieToExist(cookieName, "Cookie To Exist");
        }

        public IAssertion CookieToExist(string cookieName, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var cookie = driver.Manage().Cookies.GetCookieNamed(cookieName);
                return cookie != null;
            }, actionName);

            return this;
        }

        public IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate)
        {
            return CookieToPassThis(cookieName, predicate, "Cookie To Pass This");
        }

        public IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var cookie = driver.Manage().Cookies.GetCookieNamed(cookieName);
                return predicate(cookie);
            }, actionName);

            return this;
        }

        public IAssertion ElementToHaveClass(Locator locator, string className)
        {
            return ElementToHaveClass(locator, className, "Element To Have Class");
        }

        public IAssertion ElementToHaveClass(Locator locator, string className, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return element.GetAttribute("class").Contains(className);
            }, actionName);

            return this;
        }

        public IAssertion ElementToHaveAttr(Locator locator, string attribute)
        {
            return ElementToHaveAttr(locator, attribute, "Element To Have Attr");
        }

        public IAssertion ElementToHaveAttr(Locator locator, string attribute, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                return !string.IsNullOrWhiteSpace(element.GetAttribute(attribute));
            }, actionName);

            return this;
        }

        public IAssertion ElementToHaveAttrValue(Locator locator, string attribute, string value)
        {
            return ElementToHaveAttrValue(locator, attribute, value, "Element To Have Attr Value");
        }

        public IAssertion ElementToHaveAttrValue(Locator locator, string attribute, string value, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var attr = element.GetAttribute(attribute);

                return !string.IsNullOrWhiteSpace(attr) && attr == value;
            }, actionName);

            return this;
        }

        public IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate)
        {
            return CurrentPageNetworkEntriesPassThis(predicate, "Current Page Network Entries Pass This");
        }

        public IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var timings = driver.ExecuteJavaScript<string>("return window.performance.getEntries();");

                return predicate(timings);
            }, actionName);

            return this;
        }

        public IAssertion FileToBeDownloaded(string filename)
        {
            return FileToBeDownloaded(filename);
        }

        public IAssertion FileToBeDownloaded(string filename, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                throw new NotImplementedException();
            }, actionName);

            return this;
        }

        public IAssertion FileToBeDownloaded(Regex filenamePattern)
        {
            return FileToBeDownloaded(filenamePattern);
        }

        public IAssertion FileToBeDownloaded(Regex filenamePattern, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                throw new NotImplementedException();
            }, actionName);

            return this;
        }

        public IExecution Then
        {
            get
            {
                for (int i = 0; i < _assertions.Count; i++)
                {
                    var assertionAction = _assertions[i];
                    var invert = _assertionsToBeInverted.Contains(i);

                    _action.Add(driver =>
                    {
                        try
                        {
                            bool result = false;

                            if (invert)
                                result = !assertionAction.Action(driver);
                            else
                                result = assertionAction.Action(driver);

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
