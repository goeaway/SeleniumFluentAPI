using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumScript.Components;
using SeleniumScript.Enums;
using SeleniumScript.Utilities;
using System;
using System.Threading;

namespace SeleniumScript.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow(Browser.Chrome)]
        public void TestMethod1(Browser browser)
        {
            var factory = new ManagedWebDriverFactory(browser);

            new Execution()
                .NavigateTo(new Uri("https://google.com"))
                .Wait.For(_ => {
                    Thread.Sleep(2000);
                    return true;
                }, TimeSpan.FromSeconds(2))
                .Then
                .Expect.ToBe(driver => driver.Url.Contains("google.smom"))
                .Then
                .Execute(factory);
        }
    }
}
