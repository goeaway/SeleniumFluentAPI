using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumScript.Components;
using SeleniumScript.Enums;
using SeleniumScript.Exceptions;
using SeleniumScript.Utilities;

namespace SeleniumScript.Abstractions
{
    /// <summary>
    /// Defines the interface through which consumers can execute user actions which alter the state and trigger behaviour of a page or it's elements
    /// </summary>
    public interface IExecution
    {
        /// <summary>
        /// Sets how many times a failure to interact with the <see cref="IWebDriver"/> and or an <see cref="IWebElement"/> should be retried before throwing an <see cref="ExecutionFailureException"/>
        /// </summary>
        /// <param name="count">The number of times to retry an interaction</param>
        /// <param name="intervalWaitPeriod">The time period, represented by a <see cref="TimeSpan"/>, between each try</param>
        /// <returns></returns>
        IExecution RetryCount(int count, TimeSpan intervalWaitPeriod);
        /// <summary>
        /// Sets if an exception should be thrown if an <see cref="IAssertion"/> fails
        /// </summary>
        /// <param name="throwException"></param>
        /// <returns></returns>
        IExecution ExceptionOnAssertionFailure(bool throwException);
        /// <summary>
        /// Sets if an exception should be thrown if an <see cref="IWait"/> fails
        /// </summary>
        /// <param name="throwException"></param>
        /// <returns></returns>
        IExecution ExceptionOnWaitFailure(bool throwException);

        IExecution Refresh();
        IExecution Refresh(string actionName);

        IExecution NavigateTo(IPage page);
        IExecution NavigateTo(IPage page, string actionName);
        IExecution NavigateTo(IPage page, IDictionary<string, string> queryStringParameters);
        IExecution NavigateTo(IPage page, IDictionary<string, string> queryStringParameters, string actionName);
        IExecution NavigateTo(IPage page, IEnumerable<string> urlParameters);
        IExecution NavigateTo(IPage page, IEnumerable<string> urlParameters, string actionName);

        IExecution Access(IDomain domain);
        IExecution Access(IDomain domain, string actionName);

        IExecution Click(Locator locator);
        IExecution Click(Locator locator, string actionName);

        IExecution Input(Locator locator, string textToInput);
        IExecution Input(Locator locator, string textToInput, string actionName);

        IExecution Select(Locator locator, int index);
        IExecution Select(Locator locator, int index, string actionName);
        IExecution Select(Locator locator, string value, SelectionType selectionType);
        IExecution Select(Locator locator, string value, SelectionType selectionType, string actionName);

        IExecution ScrollTo(Locator locator);
        IExecution ScrollTo(Locator locator, string actionName);
        IExecution Scroll(int pixels, bool up);
        IExecution Scroll(int pixels, bool up, string actionName);

        IExecution MoveMouseTo(Locator locator);
        IExecution MoveMouseTo(Locator locator, string actionName);
        IExecution MoveMouseTo(Locator locator, int pixelOffset, PixelOffsetDirection direction);
        IExecution MoveMouseTo(Locator locator, int pixelOffset, PixelOffsetDirection direction, string actionName);
        IExecution MoveMouseTo(int x, int y);
        IExecution MoveMouseTo(int x, int y, string actionName);

        IExecution ClickAndHold(Locator locator);
        IExecution ClickAndHold(Locator locator, string actionName);

        IExecution ReleaseClick();
        IExecution ReleaseClick(string actionName);

        IAssertion Expect { get; }
        IUtility Utils { get; }
        IWait Wait { get; }

        IExecution Add(Func<IWebDriver, bool> component);
        IExecution Add(Func<IWebDriver, bool> component, string actionName);

        IExecution Complete();

        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onExecutionStart">An <see cref="Action"/> called after the <see cref="IWebDriver"/> is initialised, but before any execution components are executed</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory, 
            Action<IWebDriver> onExecutionStart);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onActionStart">An <see cref="Action"/> called before each execution component is executed</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory, 
            Action<IWebDriver, IExecutionContext> onActionStart);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onExecutionCompletion">A <see cref="Func{IWebDriver, bool}>"/> called after all execution components are executed, or after an unhandled exception occurs. Return true from the func if the <see cref="IWebDriver"/> should be quit automatically</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory, 
            Func<IWebDriver, bool> onExecutionCompletion);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onExecutionStart">An <see cref="Action"/> called after the <see cref="IWebDriver"/> is initialised, but before any execution components are executed</param>
        /// <param name="onExecutionCompletion">A <see cref="Func{IWebDriver, bool}>"/> called after all execution components are executed, or after an unhandled exception occurs. Return true from the func if the <see cref="IWebDriver"/> should be quit automatically</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Func<IWebDriver, bool> onExecutionCompletion);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onExecutionStart">An <see cref="Action"/> called after the <see cref="IWebDriver"/> is initialised, but before any execution components are executed</param>
        /// <param name="onActionStart">An <see cref="Action"/> called before each execution component is executed</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory,
            Action<IWebDriver> onExecutionStart,
            Action<IWebDriver, IExecutionContext> onActionStart);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onActionStart">An <see cref="Action"/> called before each execution component is executed</param>
        /// <param name="onExecutionCompletion">A <see cref="Func{IWebDriver, bool}>"/> called after all execution components are executed, or after an unhandled exception occurs. Return true from the func if the <see cref="IWebDriver"/> should be quit automatically</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory, 
            Action<IWebDriver, IExecutionContext> onActionStart, 
            Func<IWebDriver, bool> onExecutionCompletion);
        /// <summary>
        /// Executes each component of the <see cref="Execution"/> in the order they were added.
        /// </summary>
        /// <param name="webDriverFactory">A <see cref="IWebDriverFactory"/> to create an <see cref="IWebDriver"/></param>
        /// <param name="onExecutionStart">An <see cref="Action"/> called after the <see cref="IWebDriver"/> is initialised, but before any execution components are executed</param>
        /// <param name="onActionStart">An <see cref="Action"/> called before each execution component is executed</param>
        /// <param name="onExecutionCompletion">A <see cref="Func{IWebDriver, bool}>"/> called after all execution components are executed, or after an unhandled exception occurs. Return true from the func if the <see cref="IWebDriver"/> should be quit automatically</param>
        /// <returns></returns>
        IEnumerable<ExecutionResult> Execute(
            IWebDriverFactory webDriverFactory, 
            Action<IWebDriver> onExecutionStart, 
            Action<IWebDriver, IExecutionContext> onActionStart, 
            Func<IWebDriver, bool> onExecutionCompletion);
    }
}
