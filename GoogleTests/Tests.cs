using System;
using System.Linq;
using System.Threading;
using GoogleTests.Google;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Opera;
using SeleniumScript.Abstractions;
using SeleniumScript.Components;
using SeleniumScript.Enums;
using SeleniumScript.Utilities;
using WebDriverManager.DriverConfigs.Impl;

namespace GoogleTests
{
    [TestClass]
    public class Tests
    {
        private const bool LOCAL = true;
        private readonly static GoogleDomain domain = new GoogleDomain();

        private IWebDriverFactory GetFactory(Browser browser)
        {
            if (LOCAL)
            {
                return new ManagedWebDriverFactory(browser)
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
            }

            throw new NotSupportedException();
        }

        [TestMethod]
        [DataRow(Browser.Chrome)]
        [DataRow(Browser.Firefox)]
        //[DataRow(Browser.IE)]
        public void CanAccessGoogle(Browser browser)
        {
            var execution = Execution
                .New()
                .ExceptionOnAssertionFailure(false)
                .Access(domain)
                .Expect
                    .ToBeOn(domain.HomePage)
                .Then
                .Input(domain.HomePage.SearchInput, "my balls")
                .Click(domain.HomePage.SearchButton);

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

        [TestMethod]
        [DataRow(Browser.Chrome)]
        [DataRow(Browser.Firefox)]
        //[DataRow(Browser.IE)]
        public void CanClickXIndexLink(Browser browser)
        {
            var execution = Execution.New()
                .Access(domain)
                .Input(domain.HomePage.SearchInput, "search")
                .Click(domain.HomePage.SearchButton)
                .Click(Locator.From(domain.SearchPage.ResultsLinks, 0));

            var factory = GetFactory(browser);
            var result = execution.Execute(factory, onExecutionCompletion: d => { Thread.Sleep(2000); return true; });
        }

        [TestMethod]
        [DataRow(Browser.Chrome)]
        [DataRow(Browser.Firefox)]
        //[DataRow(Browser.IE)]
        public void CanScrollToElement(Browser browser)
        {
            var searchesRelatedText = Locator.From(By.CssSelector("div[class=\"e2BEnf U7izfe\"]"));

            var execution = Execution.New()
                .Access(domain)
                .RetryCount(2, TimeSpan.FromSeconds(5))
                .Input(domain.HomePage.SearchInput, "search")
                .Click(domain.HomePage.SearchButton, "click search")
                .ScrollTo(searchesRelatedText)
                .Expect
                .ToBeAbleSeeElement(searchesRelatedText)
                .Then;

            var factory = GetFactory(browser);
            var result = execution.Execute(factory, onExecutionCompletion: d => {
                Thread.Sleep(2000);
                return true;
            });
        }
    }
}
