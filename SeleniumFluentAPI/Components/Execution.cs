using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
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

        /// <summary>
        /// Initialises a new <see cref="Execution"/> instance
        /// <param name="options">Provide options for the execution</param>
        /// </summary>
        public Execution(ExecutionOptions options)
        {
            _executionOptions = options ?? throw new ArgumentNullException(nameof(options));
            _actions = new List<ExecutionAction>();
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

                if(_executionOptions.Debug)
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

                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
            }, actionName);
        }
        public IExecution Select(Locator locator, int index, string actionName = "Select")
        {
            return InnerAddWithPolicy(driver =>
            {
                var element = locator.FindElement(driver);
                var select = new SelectElement(element);

                select.SelectByIndex(index);

                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
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

                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
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

        public IExecution SwitchToTab(int tabIndex, string actionName = "Switch To Tab")
        {
            return InnerAddWithPolicy(driver =>
            {
                var tab = driver.WindowHandles[tabIndex];
                driver.SwitchTo().Window(tab);
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
                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
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

                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
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
                if (_executionOptions.Debug)
                {
                    Highlighter.Highlight(driver, element, Color.Yellow);
                }
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

        public ExecutionResult Execute()
        {
            var exResult = new ExecutionResult();

            var driver =
                _executionOptions.WebDriverFactory != null
                ? _executionOptions.WebDriverFactory.CreateWebDriver()
                : _executionOptions.WebDriver 
                    ?? throw new InvalidOperationException("A web driver or web driver factory is required");

            _executionOptions.OnExecutionStart?.Invoke(driver);

            try
            {
                foreach (var action in _actions)
                {
                    _executionOptions.OnActionStart?.Invoke(
                        driver,
                        new ExecutionContext()
                        {
                            Url = driver.Url,
                            ActionName = action.Name
                        });

                    try
                    {
                        var result = action.Action(driver);
                        exResult.AddActionResult(result);
                    }
                    catch (Exception e)
                    {
                        if(_executionOptions.Debug)
                        {
                            Thread.Sleep(10);
                        }
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
                var shouldDispose = _executionOptions.OnExecutionCompletion?.Invoke(driver);

                if(!shouldDispose.HasValue || shouldDispose.Value)
                {
                   DriverQuitter.Quit(driver);
                }
            }
        }
    }
}
