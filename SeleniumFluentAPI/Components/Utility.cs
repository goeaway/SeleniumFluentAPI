using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using SeleniumScript.Enums;
using SeleniumScript.Exceptions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SeleniumScript.Components
{
    public class Utility : IUtility
    {
        private readonly IExecution _execution;
        private readonly IList<UtilityAction> _actions;

        public IExecution Then
        {
            get
            {
                foreach (var action in _actions)
                {
                    _execution.Custom(driver => action.Action(driver), action.Name);
                }

                return _execution;
            }
        }

        private IUtility InnerAdd(Func<IWebDriver, bool> action, string actionName)
        {
            _actions.Add(new UtilityAction(actionName, driver =>
            {
                return action(driver);
            }));
            return this;
        }

        public Utility(IExecution execution)
        {
            _execution = execution;
            _actions = new List<UtilityAction>();
        }

        public IUtility SetCookie(string cookieName, string value)
        {
            return InnerAdd(driver =>
            {
                driver.Manage().Cookies.AddCookie(new Cookie(cookieName, value));

                return true;
            }, "Set Cookie");
        }

        public IUtility SetWindowDimensions(int width, int height)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1)
                throw new ArgumentOutOfRangeException(nameof(height));

            return InnerAdd(driver =>
            {
                driver.Manage().Window.Size = new Size(width, height);
                return true;
            }, "Set Dimensions");
        }

        public IUtility SetWindowMaximised()
        {
            return InnerAdd(driver =>
            {
                driver.Manage().Window.Maximize();
                return true;
            }, "Maximise");
        }

        public IUtility SetWindowMinimised()
        {
            return InnerAdd(driver =>
            {
                driver.Manage().Window.Minimize();
                return true;
            }, "Minimise");
        }

        public IUtility CreateNewTab()
        {
            return InnerAdd(driver =>
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
                return true;
            }, "Create New Tab");
        }

        public IUtility CloseTab(int tabIndex)
        {
            return InnerAdd(driver =>
            {
                if(driver.WindowHandles.Count == 1)
                {
                    throw new UtilityException("Cannot close the last tab");
                }

                var tab = driver.WindowHandles[tabIndex];
                driver.SwitchTo().Window(tab);
                driver.Close();
                driver.SwitchTo().Window(driver.WindowHandles[0]);
                return true;
            }, "Close Tab");
        }
    }
}
