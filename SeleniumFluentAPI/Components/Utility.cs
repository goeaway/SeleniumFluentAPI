﻿using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
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

                return new ExecutionResult(true, driver.Url, ComponentType.Utility, "Set Cookie");
            }, "Set Cookie");

            return this;
        }

        public IUtility SetWindowDimensions(int width, int height)
        {
            if (width < 1)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height < 1)
                throw new ArgumentOutOfRangeException(nameof(height));

            InnerAdd(driver =>
            {
                driver.Manage().Window.Size = new Size(width, height);
                return new ExecutionResult(true, driver.Url, ComponentType.Utility, "Set Dimensions");
            }, "Set Dimensions");

            return this;
        }

        public IUtility SetWindowMaximised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Maximize();
                return new ExecutionResult(true, driver.Url, ComponentType.Utility, "Maximise");
            }, "Maximise");

            return this;
        }

        public IUtility SetWindowMinimised()
        {
            InnerAdd(driver =>
            {
                driver.Manage().Window.Minimize();
                return new ExecutionResult(true, driver.Url, ComponentType.Utility, "Minimise");
            }, "Minimise");

            return this;
        }
    }
}
