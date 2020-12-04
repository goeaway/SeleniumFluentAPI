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
        IExecution Refresh(string actionName = "Refresh");

        IExecution NavigateTo(Uri uri, string actionName = "NavigateTo");

        IExecution SwitchToTab(int tabIndex, string actionName = "Switch To Tab");

        IExecution Click(Locator locator, string actionName = "Click");

        IExecution Input(Locator locator, string textToInput, string actionName = "Input");

        IExecution Select(Locator locator, int index, string actionName = "Select");
        IExecution Select(Locator locator, string value, SelectionType selectionType, string actionName = "Select");

        IExecution ScrollTo(Locator locator, string actionName = "ScrollTo");
        IExecution Scroll(int pixels, bool up, string actionName = "Scroll");

        IExecution MoveMouseTo(Locator locator, string actionName = "MoveMouseTo");
        IExecution MoveMouseTo(int x, int y, string actionName = "MoveMouseTo");

        IExecution ClickAndHold(Locator locator, string actionName = "ClickAndHold");

        IExecution ReleaseClick(string actionName = "ReleaseClick");

        IAssertion Expect { get; }
        IUtility Utils { get; }
        IWait Wait { get; }

        /// <summary>
        /// Add a custom execution action. Use the internal web driver instance to interact with the browser.
        /// Best used when other methods cannot provide required functionality.
        /// </summary>
        /// <param name="component">The action to interact with the browser in, via the web driver</param>
        /// <param name="actionName">An optional name for this action for easier debugging if a action fails</param>
        /// <returns></returns>
        IExecution Custom(Action<IWebDriver> component, string actionName = "Custom");

        /// <summary>
        /// Executes each component of the <see cref="IExecution"/> in the order they were added.
        /// </summary>
        /// <returns></returns>
        ExecutionResult Execute();
    }
}
