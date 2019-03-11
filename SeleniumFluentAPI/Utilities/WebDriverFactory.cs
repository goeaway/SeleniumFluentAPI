using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Utilities
{
    public class WebDriverFactory : IWebDriverFactory
    {
        public WebDriverFactory(RunLocation runLocation, Browser browser)
        {

        }

        private readonly RunLocation _location;
        private readonly Browser _browser;

        public IWebDriver CreateWebDriver()
        {
            switch (_browser)
            {
                case Browser.Chrome:
                case Browser.Firefox:
                case Browser.Edge:
                case Browser.IE:
                case Browser.Safari:
                default: 
                    throw new NotSupportedException(_browser.ToString());
            }
        }
    }
}
