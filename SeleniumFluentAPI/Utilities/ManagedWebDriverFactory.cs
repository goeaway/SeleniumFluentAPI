using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace SeleniumFluentAPI.Utilities
{
    public class ManagedWebDriverFactory : IWebDriverFactory
    {
        private readonly Browser _browser;
        private readonly DriverManager _manager;

        public ManagedWebDriverFactory(Browser browser)
        {
            _browser = browser;
            _manager = new DriverManager();
        }

        public IWebDriver CreateWebDriver()
        {
            switch(_browser)
            {
                case Browser.Chrome:
                    _manager.SetUpDriver(new ChromeConfig());
                    return new ChromeDriver();
                case Browser.Firefox:
                    _manager.SetUpDriver(new FirefoxConfig());
                    return new FirefoxDriver();
                case Browser.Edge:
                    _manager.SetUpDriver(new EdgeConfig());
                    return new EdgeDriver();
                case Browser.IE:
                    _manager.SetUpDriver(new InternetExplorerConfig());
                    return new InternetExplorerDriver();
                default:
                    throw new NotSupportedException(_browser.ToString());
            }
        }
    }
}
