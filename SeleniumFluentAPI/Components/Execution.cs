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
        private bool _throwOnWaitException;

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
                    throw new ExecutionFailureException(e);
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
                    throw new ExecutionFailureException(e);
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

                element.SendKeys(textToInput);

                return new ExecutionResult(true, driver.Url, actionName, $"send '{textToInput}' to element");
            }, actionName);

            return this;
        }

        public IExecution Input(By by, string textToInput)
        {
            return Input(by, textToInput, "Input");
        }

        private IExecution InnerNavigateTo(Uri uri, string actionName)
        {
            InnerAddWithPolicy(driver =>
            {
                driver.Url = uri.ToString();
                return new ExecutionResult(true, driver.Url, actionName);
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

        public IExecution ExceptionOnWaitFailure(bool throwException)
        {
            _throwOnWaitException = throwException;
            return this;
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory)
        {
            return Execute(webDriverFactory, context => { }, driver => { });
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IExecutionContext> onActionStart)
        {
            return Execute(webDriverFactory, onActionStart, driver => { });
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, 
            Action<IWebDriver> onExecutionCompletion)
        {
            return Execute(webDriverFactory, context => { }, onExecutionCompletion);
        }

        public IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory,
            Action<IExecutionContext> onActionStart, Action<IWebDriver> onExecutionCompletion)
        {
            var results = new List<ExecutionResult>();

            var driver = webDriverFactory.CreateWebDriver();
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(3);
            try
            {
                foreach (var action in _actions)
                {
                    onActionStart(ExecutionContext.GetContext(driver.Url, action.Name));

                    var result = action.Action(driver);

                    results.Add(result);
                }


                return results;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                onExecutionCompletion(driver);
                DriverQuitter.Quit(driver);
            }
        }

        public static Execution New()
        {
            return new Execution();
        }
    }
}
