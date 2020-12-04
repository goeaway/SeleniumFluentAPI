using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using SeleniumScript.Abstractions;
using SeleniumScript.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium.Opera;
using WebDriverManager;
using WebDriverManager.DriverConfigs;
using WebDriverManager.DriverConfigs.Impl;
using InternetExplorerOptions = OpenQA.Selenium.IE.InternetExplorerOptions;

namespace SeleniumScript.Utilities
{
    /// <summary>
    /// Provides functionality to create an <see cref="IWebDriver"/> based on provided browser options
    /// </summary>
    public class ManagedWebDriverFactory : IWebDriverFactory
    {
        private readonly Browser _browser;
        private readonly DriverManager _manager;
        private readonly IDictionary<Browser, IDriverConfig> _configs;
        private readonly IDictionary<Browser, DriverOptions> _options; 

        /// <summary>
        /// Initialise a <see cref="ManagedWebDriverFactory"/> with default options. Uses the latest version of the Chrome browser web driver
        /// </summary>
        public ManagedWebDriverFactory() : this (Browser.Chrome) { }
        /// <summary>
        /// Initialise a <see cref="ManagedWebDriverFactory"/> with a specified <see cref="Browser"/>
        /// </summary>
        /// <param name="browser">The browser to use</param>
        public ManagedWebDriverFactory(Browser browser) : this (browser, null, null) { }
        /// <summary>
        /// Initialise a <see cref="ManagedWebDriverFactory"/> with a specified <see cref="Browser"/> and <see cref="DriverOptions"/> for that browser
        /// </summary>
        /// <param name="browser">The browser to use</param>
        /// <param name="options">The options for the browser. You should provide a derived version of <see cref="DriverOptions"/> for the browser you want to use, e.g. <see cref="ChromeOptions"/></param>
        public ManagedWebDriverFactory(Browser browser, DriverOptions options) : this(browser, options, null) { }
        /// <summary>
        /// Initialise a <see cref="ManagedWebDriverFactory"/> with a specified <see cref="Browser"/> and <see cref="IDriverConfig"/> for that browser
        /// </summary>
        /// <param name="browser">The browser to use</param>
        /// <param name="config">The config for the web driver. You should provide an implementation of <see cref="IDriverConfig"/> for the browser you want to use, e.g. <see cref="ChromeConfig"/></param>
        public ManagedWebDriverFactory(Browser browser, IDriverConfig config) : this (browser,  null, config) { }
        /// <summary>
        /// Initialise a <see cref="ManagedWebDriverFactory"/> with a specified <see cref="Browser"/>, <see cref="DriverOptions"/> and <see cref="IDriverConfig"/> for that browser
        /// </summary>
        /// <param name="browser">The browser to use</param>
        /// <param name="options">The options for the browser. You should provide a derived version of <see cref="DriverOptions"/> for the browser you want to use, e.g. <see cref="ChromeOptions"/></param>
        /// <param name="config">The config for the web driver. You should provide an implementation of <see cref="IDriverConfig"/> for the browser you want to use, e.g. <see cref="ChromeConfig"/></param>
        public ManagedWebDriverFactory(Browser browser, DriverOptions options, IDriverConfig config)
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

            if(config != null)
            {
                try
                {
                    _configs[browser] = config;
                }
                catch (KeyNotFoundException)
                {
                    throw new NotSupportedException(browser.ToString());
                }
            }

            if(options != null)
            {
                try
                {
                    _options[browser] = options;
                }
                catch (KeyNotFoundException)
                {
                    throw new NotSupportedException(browser.ToString());
                }
            }
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
