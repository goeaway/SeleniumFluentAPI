using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Executions;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IExecution
    {
        IExecution RetryCount(int count, TimeSpan intervalWaitPeriod);
        IExecution ExceptionOnAssertionFailure(bool throwException);

        IExecution NavigateTo(IPage page);
        IExecution Access(IDomain domain);

        IExecution Click(By by);
        IExecution Input(By by, string textToInput);

        IExecution ScrollTo(By by);
        IExecution Scroll(int pixels, bool up);
        IExecution MoveMouseTo(By by);
        IExecution MoveMouseTo(int x, int y);
        IExecution ClickAndHold(By by);
        IExecution ReleaseClick();

        IAssertion Expect { get; }
        IWait Wait { get; }

        IExecution Add(Func<ExecutionResult> component);
        IExecution Complete();

        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory);
    }
}
