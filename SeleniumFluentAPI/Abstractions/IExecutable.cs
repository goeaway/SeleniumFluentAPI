using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Executions;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IExecutable
    {
        IExecutable NavigateTo(IPage page);

        IExecutable Click(By by);
        //IExecutable Click(By by, string actionIdentifier);
        IExecutable Input(By by, string textToInput);

        // to add 

        IExecutable ScrollTo(By by);
        IExecutable Scroll(int pixels, bool up);
        IExecutable MoveMouseTo(By by);
        IExecutable MoveMouseTo(int x, int y);
        IExecutable ClickAndHold(By by);
        IExecutable ReleaseClick();

        IAssertion Expect { get; }
        IWait Wait { get; }

        IExecutable Add(Func<ExecutionResult> component);

        IExecutable Complete();

        IEnumerable<ExecutionResult> Execute(IWebDriverFactory webDriverFactory);
    }
}
