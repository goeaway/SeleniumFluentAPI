using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Components;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IExecution
    {
        IExecution RetryCount(int count, TimeSpan intervalWaitPeriod);
        IExecution ExceptionOnAssertionFailure(bool throwException);
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

        IExecution Click(By by);
        IExecution Click(By by, string actionName);
        IExecution Input(By by, string textToInput);
        IExecution Input(By by, string textToInput, string actionName);
        IExecution Select(By by, int index);
        IExecution Select(By by, int index, string actionName);
        IExecution Select(By by, string value, SelectionType selectionType);
        IExecution Select(By by, string value, SelectionType selectionType, string actionName);

        IExecution ScrollTo(By by);
        IExecution ScrollTo(By by, string actionName);
        IExecution Scroll(int pixels, bool up);
        IExecution Scroll(int pixels, bool up, string actionName);
        IExecution MoveMouseTo(By by);
        IExecution MoveMouseTo(By by, string actionName);
        IExecution MoveMouseTo(By by, int pixelOffset, PixelOffsetDirection direction);
        IExecution MoveMouseTo(By by, int pixelOffset, PixelOffsetDirection direction, string actionName);
        IExecution MoveMouseTo(int x, int y);
        IExecution MoveMouseTo(int x, int y, string actionName);
        IExecution ClickAndHold(By by);
        IExecution ClickAndHold(By by, string actionName);
        IExecution ReleaseClick();
        IExecution ReleaseClick(string actionName);

        IAssertion Expect { get; }
        IUtility Utils { get; }
        IWait Wait { get; }

        IExecution Add(Func<IWebDriver, ExecutionResult> component);
        IExecution Add(Func<IWebDriver, ExecutionResult> component, string actionName);
        IExecution Complete();

        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory);
        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, Action<IExecutionContext> onActionStart);
        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, Action<IWebDriver> onExecutionCompletion);
        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory, Action<IExecutionContext> onActionStart, Action<IWebDriver> onExecutionCompletion);
    }
}
