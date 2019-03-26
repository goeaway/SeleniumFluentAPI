using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumScript.Enums;

namespace SeleniumScript.Abstractions
{
    public interface IWebDriverFactory
    {
        IWebDriver CreateWebDriver();
    }
}
