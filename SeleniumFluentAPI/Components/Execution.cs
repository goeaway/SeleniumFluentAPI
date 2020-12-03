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

        public IAssertion Expect => new Assertion(this, _executionOptions.ActionRetries, _executionOptions.ActionRetryWaitPeriods);
        public IWait Wait => new Wait(this, _executionOptions.ActionRetries, _executionOptions.ActionRetryWaitPeriods);
        public IUtility Utils => new Utility(this);

        private IExecution InnerAddWithPolicy(Action<IWebDriver> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                Policy
                    .Handle<WebDriverException>()
                    .Or<LocatorFindException>()
                    .WaitAndRetry(
                        _executionOptions.ActionRetries, 
                        (tryNum) => RetryWaitCalculator.GetTimeSpanForWait(tryNum, _executionOptions.ActionRetryWaitPeriods))
                    .Execute(() => action(driver));

                return new ActionResult 
                {
                    Context = new ExecutionContext 
                    { 
                        ActionName = actionName, 
                        Url = driver.Url
                    }
                };
            }));
            return this;
        }
        private IExecution InnerAdd(Action<IWebDriver> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                action(driver);

                return new ActionResult
                {
                    Context = new ExecutionContext
                    {
                        ActionName = actionName,
                        Url = driver.Url
                    }
                };
            }));
            return this;
        }
        public IExecution Click(Locator locator, string actionName = "Click")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                element.Click();

                if(_executionOptions.HighlightElementOnClick)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
            }, actionName);
        }
        public IExecution Input(Locator locator, string textToInput, string actionName = "Input")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                element.SendKeys(textToInput);
            }, actionName);
        }
        public IExecution Select(Locator locator, int index, string actionName = "Select")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var select = new SelectElement(element);

                select.SelectByIndex(index);
            }, actionName);
        }
        public IExecution Select(Locator locator, string value, SelectionType selectionType, string actionName = "Select")
        {
            return InnerAddWithPolicy(driver =>
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
            }, actionName);
        }
        public IExecution Refresh(string actionName = "Refresh")
        {
            return InnerAddWithPolicy(driver =>
            {
                driver.Navigate().Refresh();
            }, actionName);
        }
        public IExecution NavigateTo(Uri uri, string actionName = "NavigateTo")
        {
            return InnerAddWithPolicy(driver =>
            {
                driver.Url = uri.ToString();
            }, actionName);
        }

        public IExecution Custom(Action<IWebDriver> component, string actionName = "Custom")
        {
            return InnerAdd(driver => component(driver), actionName);
        }
        public IExecution ScrollTo(Locator locator, string actionName = "ScrollTo")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                driver.ExecuteJavaScript<string>("arguments[0].scrollIntoView(true);", element);
            }, actionName);
        }
        public IExecution Scroll(int pixels, bool up, string actionName = "Scroll")
        {
            return InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);

                var offset = up ? -pixels : pixels;

                actions.MoveByOffset(0, offset);
                actions.Perform();
            }, actionName);
        }
        public IExecution MoveMouseTo(Locator locator, string actionName = "MoveMouseTo")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var actions = new Actions(driver);
                actions.MoveToElement(element);
                actions.Perform();
            }, actionName);
        }
        public IExecution MoveMouseTo(int x, int y, string actionName = "MoveMouseTo")
        {
            return InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.MoveByOffset(x, y);
                actions.Perform();
            }, actionName);
        }
        public IExecution ClickAndHold(Locator locator, string actionName = "ClickAndHold")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var actions = new Actions(driver);
                actions.ClickAndHold(element);
                actions.Perform();
            }, actionName);
        }
        public IExecution ReleaseClick(string actionName = "ReleaseClick")
        {
            return InnerAddWithPolicy(driver =>
            {
                var actions = new Actions(driver);
                actions.Release();
                actions.Perform();
            }, actionName);
        }

        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <returns></returns>
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory)
            => Execute(webDriverFactory, driver => { }, (driver, context) => { }, driver => true);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart)
            => Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, driver => true);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart)
            => Execute(webDriverFactory, driver => { }, onActionStart, driver => true);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory, 
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, driver => { }, (driver, context) => { }, onExecutionCompletion);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, onExecutionCompletion);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
            => Execute(webDriverFactory, driver => { }, onActionStart, onExecutionCompletion);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart)
            => Execute(webDriverFactory, onExecutionStart, onActionStart, driver => true);
        
        public ExecutionResult Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart, 
            Func<IWebDriver, bool> onExecutionCompletion)
        {
            var exResult = new ExecutionResult();

            var driver = webDriverFactory.CreateWebDriver();
            onExecutionStart(driver);

            try
            {
                foreach (var action in _actions)
                {
                    onActionStart(driver, new ExecutionContext() { Url = driver.Url, ActionName = action.Name });

                    try
                    {
                        var result = action.Action(driver);
                        exResult.AddActionResult(result);
                    }
                    catch (Exception e)
                    {
                        // capture and store exception in result, break and return to user
                        // no more executions after this exception occurs will happen
                        exResult.AddActionResult(new ActionResult
                        {
                            InnerException = e,
                            Context = new ExecutionContext
                            {
                                ActionName = action.Name,
                                Url = driver.Url
                            }
                        });
                        break;
                    }
                }

                return exResult;
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
