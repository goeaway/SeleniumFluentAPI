using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using Polly;
using SeleniumScript.Abstractions;
using SeleniumScript.Attributes;
using SeleniumScript.Domains;
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
        private int _actionRetries;
        private TimeSpan _actionRetryWaitPeriod;
        private bool _throwOnAssertionFailure;
        private bool _throwOnWaitException;

        /// <summary>
        /// Initialises a new <see cref="Execution"/> instance
        /// </summary>
        public Execution()
        {
            _actionRetryWaitPeriod = TimeSpan.MinValue;
            _actions = new List<ExecutionAction>();
            _throwOnAssertionFailure = true;
            _throwOnWaitException = true;
        }

        public IAssertion Expect => new Assertion(this, _actionRetries, _actionRetryWaitPeriod, _throwOnAssertionFailure);
        public IWait Wait => new Wait(this, _actionRetries, _actionRetryWaitPeriod, _throwOnWaitException);
        public IUtility Utils => new Utility(this);

        private void InnerAddWithPolicy(Func<IWebDriver, bool> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                try
                {
                    var result = Policy
                        .Handle<WebDriverException>()
                        .WaitAndRetry(_actionRetries, (tryNum) => _actionRetryWaitPeriod)
                        .Execute(() =>
                        {
                            return action(driver);
                        });

                    return new ExecutionResult(result, driver.Url, actionName);
                }
                catch (Exception e)
                {
                    throw new ExecutionFailureException($"{actionName} failed", e);
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
                catch (Exception e)
                {
                    throw new ExecutionFailureException($"{actionName} failed", e);
                }
            }));
        }

        public IExecution Click(By by, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                element.Click();

                return true;
            }, actionName);

            return this;
        }

        public IExecution Click(By by)
        {
            return Click(by, "Click");
        }

        public IExecution Click(By by, int index)
        {
            return Click(by, index, "Click");
        }

        public IExecution Click(By by, int index, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                try
                {
                    var elements = driver.FindElements(by);
                    elements[index].Click();
                    return true;
                }
                catch (IndexOutOfRangeException)
                {
                    throw new ExecutionFailureException("Failed to find element with index: " + index);
                }
            }, actionName);

            return this;
        }

        public IExecution Input(By by, string textToInput, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);

                element.SendKeys(textToInput);

                return true;
            }, actionName);

            return this;
        }

        public IExecution Input(By by, string textToInput)
        {
            return Input(by, textToInput, "Input");
        }

        public IExecution Input(By by, int index, string textToInput)
        {
            return Input(by, index, textToInput, "Input");
        }

        public IExecution Input(By by, int index, string textToInput, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                try
                {
                    var elements = driver.FindElements(by);
                    elements[index].SendKeys(textToInput);
                    return true;
                }
                catch (IndexOutOfRangeException)
                { 
                    throw new ExecutionFailureException("Failed to find element with index: " + index);
                }
            }, actionName);

            return this;
        }

        public IExecution Select(By by, int index)
        {
            return Select(by, index, "Select By Index");
        }

        public IExecution Select(By by, int index, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                var select = new SelectElement(element);

                select.SelectByIndex(index);

                return true;
            }, actionName);

            return this;
        }

        public IExecution Select(By by, string value, SelectionType selectionType)
        {
            return Select(by, value, selectionType, "Select");
        }

        public IExecution Select(By by, string value, SelectionType selectionType, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
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

        private IExecution InnerNavigateTo(Uri uri, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                driver.Url = uri.ToString();
                return true;
            }, actionName);

            return this;
        }

        public IExecution Refresh()
        {
            return Refresh("Refresh");
        }

        public IExecution Refresh(string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                driver.Navigate().Refresh();
                return true;
            }, actionName);

            return this;
        }

        public IExecution NavigateTo(IPage page, string actionName)
        {
            return InnerNavigateTo(page.FullUri, actionName);
        }

        public IExecution NavigateTo(IPage page)
        {
            return NavigateTo(page, "Navigate");
        }

        public IExecution NavigateTo(IPage page, IDictionary<string, string> queryStringParameters)
        {
            return NavigateTo(page, queryStringParameters);
        }

        public IExecution NavigateTo(IPage page, IDictionary<string, string> queryStringParameters, string actionName)
        {
            var queryString = string.Join("&", queryStringParameters.Select(parameter => $"{parameter.Key}={parameter.Value}"));
            var uri = new Uri(page.FullUri, "?" + queryString);
            return InnerNavigateTo(uri, actionName);
        }

        public IExecution NavigateTo(IPage page, IEnumerable<string> urlParameters)
        {
            return NavigateTo(page, urlParameters, "Navigate");
        }

        public IExecution NavigateTo(IPage page, IEnumerable<string> urlParameters, string actionName)
        {
            var parameters = string.Join("/", urlParameters);
            var uri = new Uri(page.FullUri, "/" + parameters);
            return InnerNavigateTo(uri, actionName);
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

            var page = (IPage)ctor.Invoke(new object[] { domain });

            return NavigateTo(page, actionName);
        }

        public IExecution Access(IDomain domain)
        {
            return Access(domain, "Access");
        }

        public IExecution Add(Func<IWebDriver, bool> component)
        {
            return Add(component, "Custom Execution");
        }

        public IExecution Add(Func<IWebDriver, bool> component, string actionName)
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

        public IExecution ScrollTo(By by, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                driver.ExecuteJavaScript<string>("arguments[0].scrollIntoView(true);", element);

                return true;
            }, actionName);

            return this;
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

                return true;
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

                return true;
            }, actionName);

            return this;
        }

        public IExecution MoveMouseTo(By by)
        {
            return MoveMouseTo(by, "Move Mouse To");
        }

        public IExecution MoveMouseTo(By by, int pixelOffset, PixelOffsetDirection direction)
        {
            return MoveMouseTo(by, pixelOffset, direction);
        }

        public IExecution MoveMouseTo(By by, int pixelOffset, PixelOffsetDirection direction, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                var element = driver.FindElement(by);
                var actions = new Actions(driver);
                actions.MoveToElement(element);
                return true;
            }, actionName);

            return this;
        }

        public IExecution MoveMouseTo(int x, int y, string actionName)
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

                return true;
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

                return true;
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

        public IExecution ExceptionOnWaitFailure(bool throwException)
        {
            _throwOnWaitException = throwException;
            return this;
        }

        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <returns></returns>
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory)
        {
            return Execute(webDriverFactory, driver => { }, (driver, context) => { }, driver => true);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart)
        {
            return Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, driver => true);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart)
        {
            return Execute(webDriverFactory, driver => { }, onActionStart, driver => true);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, 
            Func<IWebDriver, bool> onExecutionCompletion)
        {
            return Execute(webDriverFactory, driver => { }, (driver, context) => { }, onExecutionCompletion);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
        {
            return Execute(webDriverFactory, onExecutionStart, (driver, context) => { }, onExecutionCompletion);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver, IExecutionContext> onActionStart,
            Func<IWebDriver, bool> onExecutionCompletion)
        {
            return Execute(webDriverFactory, driver => { }, onActionStart, onExecutionCompletion);
        }

        
        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart)
        {
            return Execute(webDriverFactory, onExecutionStart, onActionStart, driver => true);
        }

        
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

                    var result = action.Action(driver);

                    results.Add(result);
                }

                return results;
            }
            finally
            {
                var quit = onExecutionCompletion(driver);
                if(quit)
                    DriverQuitter.Quit(driver);
            }
        }

        /// <summary>
        /// Creates a new <see cref="Execution"/> 
        /// </summary>
        /// <returns></returns>
        public static Execution New()
        {
            return new Execution();
        }
    }
}
