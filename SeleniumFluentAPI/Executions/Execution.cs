using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using Polly;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Executions
{
    public class Execution : IExecutable
    {
        private readonly List<Func<ExecutionResult>> _actions;
        private readonly int _actionRetries;
        private readonly TimeSpan _actionRetryWaitPeriod;
        private IWebDriver _driver;

        public Execution(int actionRetries, TimeSpan actionRetryWaitPeriod)
        {
            if(actionRetries < 0)
                throw new ArgumentOutOfRangeException(nameof(actionRetries));

            if(actionRetryWaitPeriod == null)
                throw new ArgumentNullException(nameof(actionRetryWaitPeriod));

            _actionRetries = actionRetries;
            _actionRetryWaitPeriod = actionRetryWaitPeriod;
            _actions = new List<Func<ExecutionResult>>();
        }

        public IAssertion Expect => new Assertion(this, _driver, _actionRetries, _actionRetryWaitPeriod);
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

        public IExecutable Click(By by)
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
        
        public IExecutable Input(By by, string textToInput)
        {
            _actions.Add(() =>
            {
                    var element = GetElement(by);   

                    if(element == null)
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

        public IExecutable NavigateTo(IPage page)
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

        public IExecutable Add(Func<ExecutionResult> component)
        {
            _actions.Add(() =>
            {
                    return component();
            });

            return this;
        }

        public IExecutable Complete()
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

        public IExecutable ScrollTo(By by)
        {
            return MoveMouseTo(by);
        }

        public IExecutable Scroll(int pixels, bool up)
        {
            _actions.Add(() => {
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

        public IExecutable MoveMouseTo(By by)
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

        public IExecutable MoveMouseTo(int x, int y)
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

        public IExecutable ClickAndHold(By by)
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

        public IExecutable ReleaseClick()
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
    }
}
