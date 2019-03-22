using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Components
{
    public class Utility : IUtility
    {
        private readonly IExecution _execution;
        private readonly IList<ExecutionAction> _actions;

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

        private void InnerAdd(Func<IWebDriver, ExecutionResult> action, string actionName)
        {
            _actions.Add(new ExecutionAction(actionName, driver =>
            {
                return action(driver);
            }));
        }

        public Utility(IExecution execution)
        {
            _execution = execution;
            _actions = new List<ExecutionAction>();
        }

        public IUtility SetCookie(string cookieName, string value)
        {
            InnerAdd(driver =>
            {
                driver.Manage().Cookies.AddCookie(new Cookie(cookieName, value));

                return new ExecutionResult(true, driver.Url, "Set Cookie");
            }, "Set Cookie");

            return this;
        }

        public IUtility SetWindowDimensions(int width, int height)
        {
            InnerAdd(driver =>
            {
                return new ExecutionResult(true, driver.Url, "Maximise");
            }, "Maximise");

            return this;
        }

        public IUtility SetWindowMaximised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Maximize();
                return new ExecutionResult(true, driver.Url, "Maximise");
            }, "Maximise");

            return this;
        }

        public IUtility SetWindowMinimised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Minimize();
                return new ExecutionResult(true, driver.Url, "Minimise");
            }, "Minimise");

            return this;
        }
    }
}
