using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IWebDriverFactory
    {
        IWebDriver CreateWebDriver();
    }
}
