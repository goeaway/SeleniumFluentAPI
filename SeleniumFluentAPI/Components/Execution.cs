using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Polly;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Attributes;
using SeleniumFluentAPI.Domains;
using SeleniumFluentAPI.Exceptions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Components
{
    public class Execution : IExecution
    {
        private readonly List<ExecutionAction> _actions;
        private int _actionRetries;
        private TimeSpan _actionRetryWaitPeriod;
        private bool _throwOnAssertionFailure;
        private bool _throwOnExecutionException;
        private bool _throwOnWaitException;

        public Execution()
        {
            _actionRetryWaitPeriod = TimeSpan.MinValue;
            _actions = new List<ExecutionAction>();
            _throwOnAssertionFailure = true;
            _throwOnExecutionException = true;
        }

        public IAssertion Expect => new Assertion(this, _actionRetries, _actionRetryWaitPeriod, _throwOnAssertionFailure);
        public IWait Wait => new Wait(this, _actionRetries, _actionRetryWaitPeriod, _throwOnWaitException);

        private void InnerAddWithPolicy(Func<IWebDriver, ExecutionResult> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    return Policy
                        .Handle<WebDriverException>()
                        .WaitAndRetry(_actionRetries, (tryNum) => _actionRetryWaitPeriod)
                        .Execute(() =>
                        {
                            return action(driver);
                        });
                }
                catch (Exception e)
                {
                    if(_throwOnExecutionException)
                        throw new ExecutionFailureException(e);

                    return new ExecutionResult(e, driver.Url, actionName);
                }
            }));
        }

        private void InnerAdd(Func<IWebDriver, ExecutionResult> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    return action(driver);
                }
                catch (Exception e)
                {
                    if (_throwOnExecutionException)
                        throw new ExecutionFailureException(e);

                    return new ExecutionResult(e, driver.Url, actionName);
                }
            }));
        }

        public IExecution Click(By by, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null)
                    return new ExecutionResult(false, driver.Url, actionName, "element could not be found");

                if (!element.Displayed)
                    return new ExecutionResult(false, driver.Url, actionName, "element was not visible");

                if (!element.Enabled)
                    return new ExecutionResult(false, driver.Url, actionName, "element was disabled");

                element.Click();

                return new ExecutionResult(true, driver.Url, actionName, $"click on element");
            }, actionName);

            return this;
        }

        public IExecution Click(By by)
        {
            return Click(by, "Click");
        }

        public IExecution Input(By by, string textToInput, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                if (element == null)
                    return new ExecutionResult(false, driver.Url, actionName, "element could not be found");

                if (!element.Displayed)
                    return new ExecutionResult(false, driver.Url, actionName, "element was not visible");

                if (!element.Enabled)
                    return new ExecutionResult(false, driver.Url, actionName, "element was disabled");

                element.SendKeys(textToInput);

                return new ExecutionResult(true, driver.Url, actionName, $"send '{textToInput}' to element");
            }, actionName);

            return this;
        }

        public IExecution Input(By by, string textToInput)
        {
            return Input(by, "Input");
        }

        public IExecution NavigateTo(IPage page, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var url = page.FullUri.ToString();
                driver.Url = url;
                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution NavigateTo(IPage page)
        {
            return NavigateTo(page, "Navigate");
        }

        public IExecution Access(IDomain domain, string actionName)
        {
            var pagesWithAttribute =
                domain.GetType()
                    .GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(DefaultPageAttribute)) &&
                                p.PropertyType.IsSubclassOf(typeof(Page)));

            if (pagesWithAttribute.Count() == 0)
                throw new DefaultPageNotFoundException("Default page could not be found when navigating to domain");

            if (pagesWithAttribute.Count() != 1)
                throw new MultipleDefaultPagesFoundException("Multiple default pages were found for domain when trying to navigate to it");

            var pageToInst = pagesWithAttribute.First().PropertyType;
            var ctor = pageToInst.GetConstructor(new Type[] { typeof(IDomain) });

            if (ctor == null)
                throw new InvalidOperationException();

            var page = (IPage)ctor.Invoke(new object[] { this });

            return NavigateTo(page, actionName);
        }

        public IExecution Access(IDomain domain)
        {
            return Access(domain, "Access");
        }

        public IExecution Add(Func<IWebDriver, ExecutionResult> component)
        {
            return Add(component, "Custom Execution");
        }

        public IExecution Add(Func<IWebDriver, ExecutionResult> component, string actionName)
        {
            InnerAdd(driver =>
            {
                return component(driver);
            }, actionName);

            return this;
        }

        public IExecution Complete()
        {
            return this;
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory)
        {
            var results = new List<ExecutionResult>();

            var driver = webDriverFactory.CreateWebDriver();

            foreach (var action in _actions)
            {
                var result = action.Action(driver);

                results.Add(result);
            }

            DriverQuitter.Quit(driver);

            return results;
        }

        public IExecution ScrollTo(By by, string actionName)
        {
            return MoveMouseTo(by, actionName);
        }

        public IExecution ScrollTo(By by)
        {
            return ScrollTo(by, "Scroll To");
        }

        public IExecution Scroll(int pixels, bool up, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);

                var offset = up ? -pixels : pixels;

                actions.MoveByOffset(0, offset);
                actions.Perform();

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution Scroll(int pixels, bool up)
        {
            return Scroll(pixels, up, "Scroll");
        }

        public IExecution MoveMouseTo(By by, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                var actions = new Actions(driver);
                actions.MoveToElement(element);
                actions.Perform();

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution MoveMouseTo(By by)
        {
            return MoveMouseTo(by, "Move Mouse To");
        }

        public IExecution MoveMouseTo(int x, int y, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.MoveByOffset(x, y);
                actions.Perform();

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution MoveMouseTo(int x, int y)
        {
            return MoveMouseTo(x, y, "Move Mouse To");
        }

        public IExecution ClickAndHold(By by)
        {
            return ClickAndHold(by, "Click And Hold");
        }

        public IExecution ClickAndHold(By by, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                var actions = new Actions(driver);
                actions.ClickAndHold(element);
                actions.Perform();

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution ReleaseClick()
        {
            return ReleaseClick("Release Click");
        }

        public IExecution ReleaseClick(string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.Release();
                actions.Perform();

                return new ExecutionResult(true, driver.Url, actionName);
            }, actionName);

            return this;
        }

        public IExecution RetryCount(int count, TimeSpan intervalWaitPeriod)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            _actionRetries = count;
            _actionRetryWaitPeriod = intervalWaitPeriod;

            return this;
        }

        public IExecution ExceptionOnAssertionFailure(bool throwException)
        {
            _throwOnAssertionFailure = throwException;
            return this;
        }

        public IExecution ExceptionOnExecutionFailure(bool throwException)
        {
            _throwOnExecutionException = throwException;
            return this;
        }

        public IExecution ExceptionOnWaitFailure(bool throwException)
        {
            _throwOnWaitException = throwException;
            return this;
        }

        public static Execution New()
        {
            return new Execution();
        }
    }
}
