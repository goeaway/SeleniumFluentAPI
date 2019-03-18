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

namespace SeleniumFluentAPI.Executions
{
    public class Execution : IExecution
    {
        private readonly List<Func<ExecutionResult>> _actions;
        private int _actionRetries;
        private TimeSpan _actionRetryWaitPeriod;
        private IWebDriver _driver;
        private bool _throwOnAssertionFailure;

        public Execution()
        {
            _actionRetryWaitPeriod = TimeSpan.MinValue;
            _actions = new List<Func<ExecutionResult>>();
            _throwOnAssertionFailure = true;
        }

        public IAssertion Expect => new Assertion(this, _driver, _actionRetries, _actionRetryWaitPeriod, _throwOnAssertionFailure);
        public IWait Wait => new Wait(this, _driver);

        private IWebElement GetElement(By by)
        {
            IWebElement element = null;

            Policy
                .Handle<WebDriverException>()
                .WaitAndRetry(_actionRetries, (tryNum) => _actionRetryWaitPeriod, (exception, wait, tryNum, context) =>
                {
                    element = _driver.FindElement(by);
                });

            return element;
        }

        public IExecution Click(By by)
        {
            _actions.Add(() =>
            {
                var element = GetElement(by);

                if (element == null)
                    return new ExecutionResult(false, _driver.Url, "Click", "element could not be found");

                if (!element.Displayed)
                    return new ExecutionResult(false, _driver.Url, "Click", "element was not visible");

                if (!element.Enabled)
                    return new ExecutionResult(false, _driver.Url, "Click", "element was disabled");

                element.Click();

                return new ExecutionResult(true, _driver.Url, "Click", $"click on element");
            });

            return this;
        }

        public IExecution Input(By by, string textToInput)
        {
            _actions.Add(() =>
            {
                var element = GetElement(by);

                if (element == null)
                    return new ExecutionResult(false, _driver.Url, "Input", "element could not be found");

                if (!element.Displayed)
                    return new ExecutionResult(false, _driver.Url, "Input", "element was not visible");

                if (!element.Enabled)
                    return new ExecutionResult(false, _driver.Url, "Input", "element was disabled");

                element.SendKeys(textToInput);

                return new ExecutionResult(true, _driver.Url, "Input", $"send '{textToInput}' to element");
            });

            return this;
        }

        public IExecution NavigateTo(IPage page)
        {
            _actions.Add(() =>
            {
                try
                {
                    var url = page.FullUri.ToString();
                    _driver.Url = url;
                    return new ExecutionResult(true, _driver.Url, "Navigate");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Navigate");
                }
            });

            return this;
        }

        public IExecution Access(IDomain domain)
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

            return NavigateTo(page);
        }

        public IExecution Add(Func<ExecutionResult> component)
        {
            _actions.Add(() =>
            {
                return component();
            });

            return this;
        }

        public IExecution Complete()
        {
            return this;
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory)
        {
            var results = new List<ExecutionResult>();

            _driver = webDriverFactory.CreateWebDriver();

            foreach (var func in _actions)
            {
                var result = func();

                results.Add(result);
            }

            DriverQuitter.Quit(_driver);

            return results;
        }

        public IExecution ScrollTo(By by)
        {
            return MoveMouseTo(by);
        }

        public IExecution Scroll(int pixels, bool up)
        {
            _actions.Add(() =>
            {
                try
                {
                    var actions = new Actions(_driver);

                    var offset = up ? -pixels : pixels;

                    actions.MoveByOffset(0, offset);
                    actions.Perform();

                    return new ExecutionResult(true, _driver.Url, "Scroll by pixels");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Scroll by pixels");
                }
            });

            return this;
        }

        public IExecution MoveMouseTo(By by)
        {
            _actions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var actions = new Actions(_driver);
                    actions.MoveToElement(element);
                    actions.Perform();

                    return new ExecutionResult(true, _driver.Url, "Move Mouse");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Move Mouse");
                }
            });

            return this;
        }

        public IExecution MoveMouseTo(int x, int y)
        {
            _actions.Add(() =>
            {
                try
                {
                    var actions = new Actions(_driver);
                    actions.MoveByOffset(x, y);
                    actions.Perform();

                    return new ExecutionResult(true, _driver.Url, "Move Mouse");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Move Mouse");
                }
            });

            return this;
        }

        public IExecution ClickAndHold(By by)
        {
            _actions.Add(() =>
            {
                try
                {
                    var element = GetElement(by);
                    var actions = new Actions(_driver);
                    actions.ClickAndHold(element);
                    actions.Perform();

                    return new ExecutionResult(true, _driver.Url, "Click and Hold");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Click and Hold");
                }
            });

            return this;
        }

        public IExecution ReleaseClick()
        {
            _actions.Add(() =>
            {
                try
                {
                    var actions = new Actions(_driver);
                    actions.Release();
                    actions.Perform();

                    return new ExecutionResult(true, _driver.Url, "Release Click");
                }
                catch (Exception e)
                {
                    return new ExecutionResult(e, _driver.Url, "Release Click");
                }
            });

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

        public static Execution New()
        {
            return new Execution();
        }
    }
}
