using System;
using System.Linq;
using GoogleTests.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Components;
using SeleniumFluentAPI.Enums;
using SeleniumFluentAPI.Utilities;
using WebDriverManager.DriverConfigs.Impl;

namespace GoogleTests
{
    [TestClass]
    public class Tests
    {
        private const bool LOCAL = true;

        // set protected mode on all zones in IE > internet options > security tab

        private IWebDriverFactory GetFactory(Browser browser)
        {
            if (LOCAL)
            {
                var factory = new LocalWebDriverFactory(browser)
                    .SetDriverOptions(
                        Browser.Firefox, 
                        new FirefoxOptions
                        {
                            BrowserExecutableLocation = @"C:\Program Files\Mozilla Firefox\firefox.exe"
                        })
                    .SetDriverOptions(
                        Browser.Edge,
                        new EdgeOptions
                        {
                            PageLoadStrategy = PageLoadStrategy.Eager,
                        })
                    .SetDriverOptions(
                        Browser.Opera,
                        new OperaOptions
                        {
                            BinaryLocation = @"C:\Users\Joe\AppData\Local\Programs\Opera\launcher.exe"
                        });

                return factory;
            }

            throw new NotSupportedException();
        }


        [TestMethod]
        [DataRow(Browser.Chrome)]
        //[DataRow(Browser.Edge)]
        //[DataRow(Browser.Firefox)]
        //[DataRow(Browser.IE)]
        public void CanAccessGoogle(Browser browser)
        {
            var domain = new GoogleDomain();

            var execution = Execution
                .New()
                .ExceptionOnAssertionFailure(false)
                .Access(domain)
                .Expect
                    .ToBeOn(domain.HomePage)
                .Then
                .Input(domain.HomePage.SearchInput, "my balls");

            var factory = GetFactory(browser);
            var results = execution.Execute(factory);

            var count = 1;
            foreach (var result in results)
            {
                Console.WriteLine($"{count++}. {result.Context.ActionName} ({result.Context.CurrentUrl}) - {(result.Success ? "PASS" : "FAIL")}");
                if(result.InnerException != null)
                    Console.WriteLine(result.InnerException.Message);
            }

            Assert.IsTrue(results.All(r => r.Success));
        }
    }
}
