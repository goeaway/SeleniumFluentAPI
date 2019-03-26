using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Components
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
                    _execution.Add(action.Action, action.Name);
                }

                return _execution;
            }
        }

        private void InnerAdd(Func<IWebDriver, bool> action, string actionName)
        {
            _actions.Add(new UtilityAction(actionName, driver =>
            {
                return action(driver);
            }));
        }

        public Utility(IExecution execution)
        {
            _execution = execution;
            _actions = new List<UtilityAction>();
        }

        public IUtility SetCookie(string cookieName, string value)
        {
            InnerAdd(driver =>
            {
                driver.Manage().Cookies.AddCookie(new Cookie(cookieName, value));

                return true;
            }, "Set Cookie");

            return this;
        }

        public IUtility SetWindowDimensions(int width, int height)
        {
            InnerAdd(driver =>
            {
                return false;
            }, "Maximise");

            return this;
        }

        public IUtility SetWindowMaximised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Maximize();
                return true;
            }, "Maximise");

            return this;
        }

        public IUtility SetWindowMinimised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Minimize();
                return true;
            }, "Minimise");

            return this;
        }
    }
}
