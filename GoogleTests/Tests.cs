using System;
using System.Collections.Generic;
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
using SeleniumScript.Exceptions;
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

        [TestMethod]
        public void ExecutionStopsOnFailureWhenExceptionsAreCaught()
        {
            var execution = Execution
                .New()
                .Access(domain)
                .ExceptionOnExecutionFailure(false)
                .Click(Locator.From(By.Name("made-up-thing-this-wont-exist")))
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);
            var completed = false;

            var result = execution.Execute(factory, onExecutionCompletion: d =>
            {
                completed = true;
                return true;
            });

            Assert.IsTrue(completed);
            Assert.IsTrue(result.Count() == 2);
            Assert.IsTrue(!result.Last().Success);
        }

        [TestMethod]
        public void ThrowsWhenExeExceptionsAreNotCaught()
        {
            var execution = Execution
                .New()
                .Access(domain)
                .Click(Locator.From(By.Name("made-up-thing-this-wont-exist")))
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);

            Assert.ThrowsException<ExecutionFailureException>(() =>
            {
                var result = execution.Execute(factory);
            });
        }

        [TestMethod]
        public void ThrowsWhenWaitExceptionsAreNotCaught()
        {
            var execution = Execution
                .New()
                .Access(domain)
                .Wait
                .ForElementToExist(Locator.From(By.Name("asdjfkalsdfjasdfasdfasfasdfasdf")), TimeSpan.FromMilliseconds(200))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);

            Assert.ThrowsException<WaitFailureException>(() =>
            {
                var result = execution.Execute(factory);
            });
        }

        [TestMethod]
        public void ContinuesWhenWaitExceptionsAreCaught()
        {
            var execution = Execution
                .New()
                .ExceptionOnWaitFailure(false)
                .Access(domain)
                .Wait
                    .ForElementToExist(Locator.From(By.Name("asdjfkalsdfjasdfasdfasfasdfasdf")), TimeSpan.FromMilliseconds(200))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);
            var result = execution.Execute(factory);

            Assert.IsTrue(result.Last().Success);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void ThrowsWhenAssertionExceptionsAreNotCaught()
        {
            var execution = Execution
                .New()
                .Access(domain)
                .Expect
                    .ToBeOn(new Uri("http://example.com"))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);

            Assert.ThrowsException<AssertionFailureException>(() =>
            {
                var result = execution.Execute(factory);
            });
        }

        [TestMethod]
        public void ContinuesWhenAssertionExceptionsAreCaught()
        {
            var execution = Execution
                .New()
                .ExceptionOnAssertionFailure(false)
                .Access(domain)
                .Expect
                    .ToBeOn(new Uri("http://example.com"))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);
            var result = execution.Execute(factory);

            Assert.IsTrue(result.Last().Success);
            Assert.AreEqual(3, result.Count());
        }

        [TestMethod]
        public void ThrowsWhenAssertionExceptionOccursButExecutionExceptionsAreCaught()
        {
            var execution = Execution
                .New()
                .ExceptionOnExecutionFailure(false)
                .Access(domain)
                .Expect
                    .ToBeOn(new Uri("http://example.com"))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);

            Assert.ThrowsException<AssertionFailureException>(() =>
            {
                var result = execution.Execute(factory);
            });
        }

        [TestMethod]
        public void ThrowsWhenWaitExceptionOccursButExecutionExceptionsAreCaught()
        {
            var execution = Execution
                .New()
                .ExceptionOnExecutionFailure(false)
                .Access(domain)
                .Wait
                    .ForElementToExist(Locator.From(By.Name("asdjfkalsdfjasdfasdfasfasdfasdf")), TimeSpan.FromMilliseconds(200))
                .Then
                .Input(domain.HomePage.SearchInput, "search");

            var factory = GetFactory(Browser.Chrome);

            Assert.ThrowsException<WaitFailureException>(() =>
            {
                var result = execution.Execute(factory);
            });
        }

        [TestMethod]
        public void CanHighlightOnClick()
        {
            var execution = Execution
                .New()
                .ExceptionOnExecutionFailure(false)
                .HighlightElementOnClick(true)
                .Access(domain)
                .Click(Locator.From(By.TagName("body")));

            var factory = GetFactory(Browser.Chrome);

            var result = execution.Execute(
                factory, 
                onExecutionCompletion: (d) => { Thread.Sleep(20000); return true; });
        }
    }
}
