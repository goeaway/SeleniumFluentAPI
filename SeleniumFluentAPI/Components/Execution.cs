using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumScript.Abstractions;
using SeleniumScript.Enums;
using SeleniumScript.Exceptions;
using SeleniumScript.Utilities;

namespace SeleniumScript.Components
{
    /// <summary>
    /// Main actor for the SeleniumScript library. Implementation of the <see cref="IExecution"/> interface
    /// </summary>
    public class Execution : IExecution
    {
        private readonly List<ExecutionAction> _actions;
        private readonly ExecutionOptions _executionOptions;

        public Execution() : this (new ExecutionOptions()) { }

        /// <summary>
        /// Initialises a new <see cref="Execution"/> instance
        /// </summary>
        public Execution(ExecutionOptions options)
        {
            _actions = new List<ExecutionAction>();
            _executionOptions = options;
        }

        public IAssertion Expect => new Assertion(this, _executionOptions.ActionRetries, _executionOptions.ActionRetryWaitPeriod);
        public IWait Wait => new Wait(this, _executionOptions.ActionRetries, _executionOptions.ActionRetryWaitPeriod, _executionOptions.ThrowOnWaitFailure);
        public IUtility Utils => new Utility(this);

        private void InnerAddWithPolicy(Func<IWebDriver, bool> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    var result = Policy
                        .Handle<WebDriverException>()
                        .Or<LocatorFindException>()
                        .WaitAndRetry(_executionOptions.ActionRetries, (tryNum) => _executionOptions.ActionRetryWaitPeriod)
                        .Execute(() => action(driver));

                    return new ExecutionResult(result, driver.Url, actionName);
                }
                catch (ExecutionFailureException e)
                {
                    if(_executionOptions.ThrowOnExecutionFailure)
                    {
                        throw e;
                    }

                    throw new StopExecutionException();
                }
                catch (AssertionFailureException e)
                {
                    throw e;
                }
                catch (WaitFailureException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    if (_executionOptions.ThrowOnExecutionFailure)
                    {
                        throw new ExecutionFailureException($"{actionName} failed", e);
                    }

                    throw new StopExecutionException();
                }
            }));
        }
        private void InnerAdd(Func<IWebDriver, bool> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    var result = action(driver);

                    return new ExecutionResult(result, driver.Url, actionName);
                }
                catch (ExecutionFailureException e)
                {
                    if (_executionOptions.ThrowOnExecutionFailure)
                        throw e;

                    throw new StopExecutionException();
                }
                catch (AssertionFailureException e)
                {
                    throw e;
                }
                catch (WaitFailureException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    if (_executionOptions.ThrowOnExecutionFailure)
                    {
                        throw new ExecutionFailureException($"{actionName} failed", e);
                    }

                    throw new StopExecutionException();
                }
            }));
        }
        private IExecution InnerNavigateTo(Uri uri, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                driver.Url = uri.ToString();
                return true;
            }, actionName);

            return this;
        }
        
        public IExecution Click(Locator locator, string actionName = "Click")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                element.Click();

                if(_executionOptions.HighlightElementOnClick)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }

                return true;
            }, actionName);

            return this;
        }
        public IExecution Input(Locator locator, string textToInput, string actionName = "Click")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                element.SendKeys(textToInput);

                return true;
            }, actionName);

            return this;
        }
        public IExecution Select(Locator locator, int index, string actionName = "Select")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var select = new SelectElement(element);

                select.SelectByIndex(index);

                return true;
            }, actionName);

            return this;
        }
        public IExecution Select(Locator locator, string value, SelectionType selectionType, string actionName = "Select")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var select = new SelectElement(element);

                switch (selectionType)
                {
                    case SelectionType.Text:
                        select.SelectByText(value);
                        break;
                    case SelectionType.Value:
                        select.SelectByValue(value);
                        break;
                    default:
                        throw new NotSupportedException(selectionType.ToString());
                }

                return true;
            }, actionName);

            return this;
        }
        public IExecution Refresh(string actionName = "Refresh")
        {
            InnerAddWithPolicy(driver =>
            {
                driver.Navigate().Refresh();
                return true;
            }, actionName);

            return this;
        }
        public IExecution NavigateTo(Uri uri, string actionName = "NavigateTo") => InnerNavigateTo(uri, actionName);
        public IExecution Add(Func<IWebDriver, bool> component, string actionName = "Custom")
        {
            InnerAdd(driver =>
            {
                return component(driver);
            }, actionName);

            return this;
        }
        public IExecution ScrollTo(Locator locator, string actionName = "ScrollTo")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                driver.ExecuteJavaScript<string>("arguments[0].scrollIntoView(true);", element);

                return true;
            }, actionName);

            return this;
        }
        public IExecution Scroll(int pixels, bool up, string actionName = "Scroll")
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);

                var offset = up ? -pixels : pixels;

                actions.MoveByOffset(0, offset);
                actions.Perform();

                return true;
            }, actionName);

            return this;
        }
        public IExecution MoveMouseTo(Locator locator, string actionName = "MoveMouseTo")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var actions = new Actions(driver);
                actions.MoveToElement(element);
                actions.Perform();

                return true;
            }, actionName);

            return this;
        }
        public IExecution MoveMouseTo(int x, int y, string actionName = "MoveMouseTo")
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.MoveByOffset(x, y);
                actions.Perform();

                return true;
            }, actionName);

            return this;
        }
        public IExecution ClickAndHold(Locator locator, string actionName = "ClickAndHold")
        {
            InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var actions = new Actions(driver);
                actions.ClickAndHold(element);
                actions.Perform();

                return true;
            }, actionName);

            return this;
        }
        public IExecution ReleaseClick(string actionName = "ReleaseClick")
        {
            InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.Release();
                actions.Perform();

                return true;
            }, actionName);

            return this;
        }

        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <returns></returns>
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory)
            => Execute(webDriverFactory, driver => { }, (driver, context) => { }, driver => true);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart)
            => Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, driver => true);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart)
            => Execute(webDriverFactory, driver => { }, onActionStart, driver => true);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, 
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, driver => { }, (driver, context) => { }, onExecutionCompletion);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, onExecutionCompletion);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, driver => { }, onActionStart, onExecutionCompletion);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart)
            => Execute(webDriverFactory, onExecutionStart, onActionStart, driver => true);
        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart, 
            Func<IWebDriver, bool> onExecutionCompletion)
        {
            var results = new List<ExecutionResult>();

            var driver = webDriverFactory.CreateWebDriver();
            onExecutionStart(driver);

            try
            {
                foreach (var action in _actions)
                {
                    onActionStart(driver, ExecutionContext.GetContext(driver.Url, action.Name));

                    try
                    {
                        var result = action.Action(driver);
                        results.Add(result);
                    }
                    catch (StopExecutionException)
                    {
                        // capture which execution failed so users can debug, but don't allow anymore executions
                        results.Add(new ExecutionResult(false, driver.Url, action.Name));
                        break;
                    }
                }

                return results;
            }
            finally
            {
                if(onExecutionCompletion(driver))
                {
                    DriverQuitter.Quit(driver);
                }
            }
        }
    }
}
