﻿using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Components
{
    internal class UtilityAction : ExecutionActionBase<bool>
    {
        public UtilityAction(string name, Func<IWebDriver, bool> action) : base(name, action) { }
    }
}
