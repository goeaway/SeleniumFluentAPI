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
using OpenQA.Selenium.Opera;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;
using InternetExplorerOptions = OpenQA.Selenium.IE.InternetExplorerOptions;

namespace SeleniumFluentAPI.Utilities
{
    public class LocalWebDriverFactory : IWebDriverFactory
    {
        private readonly Browser _browser;
        private readonly DriverManager _manager;
        private readonly IDictionary<Browser, IDriverConfig> _configs;
        private readonly IDictionary<Browser, DriverOptions> _options; 

        public LocalWebDriverFactory(Browser browser)
        {
            _browser = browser;
            _manager = new DriverManager();
            _configs = new Dictionary<Browser, IDriverConfig>
            {
                { Browser.Chrome, new ChromeConfig() },
                { Browser.Firefox, new FirefoxConfig() },
                { Browser.Opera, new OperaConfig() },
                { Browser.IE, new InternetExplorerConfig() },
                { Browser.Edge, new EdgeConfig() }
            };
            _options = new Dictionary<Browser, DriverOptions>
            {
                { Browser.Chrome, new ChromeOptions() },
                { Browser.Firefox, new FirefoxOptions() },
                { Browser.Opera, new OperaOptions() },
                { Browser.IE, new InternetExplorerOptions() },
                { Browser.Edge, new EdgeOptions() }
            };
        }

        public LocalWebDriverFactory SetConfig(Browser browser, IDriverConfig config)
        {
            try
            {
                _configs[browser] = config;
            }
            catch (KeyNotFoundException)
            {
                throw new NotSupportedException(browser.ToString());
            }

            return this;
        }

        public LocalWebDriverFactory SetDriverOptions(Browser browser, DriverOptions options)
        {
            try
            {
                _options[browser] = options;
            }
            catch (KeyNotFoundException)
            {
                throw new NotSupportedException(browser.ToString());
            }

            return this;
        }

        public IWebDriver CreateWebDriver()
        {
            try
            {
                var config = _configs[_browser];
                var options = _options[_browser];

                switch (_browser)
                {
                    case Browser.Chrome:
                        _manager.SetUpDriver(config);
                        return new ChromeDriver(options as ChromeOptions);
                    case Browser.Firefox:
                        _manager.SetUpDriver(config);
                        return new FirefoxDriver(options as FirefoxOptions);
                    case Browser.Edge:
                        _manager.SetUpDriver(config);
                        return new EdgeDriver(options as EdgeOptions);
                    case Browser.IE:
                        _manager.SetUpDriver(config);
                        return new InternetExplorerDriver(options as InternetExplorerOptions);
                    case Browser.Opera:
                        _manager.SetUpDriver(config);
                        return new OperaDriver(options as OperaOptions);
                    default:
                        throw new NotSupportedException(_browser.ToString());
                }
            }
            catch (NotSupportedException)
            {
                throw new NotSupportedException(_browser.ToString());
            }
            catch (KeyNotFoundException)
            {
                throw new NotSupportedException(_browser.ToString());
            }

        }
    }
}
