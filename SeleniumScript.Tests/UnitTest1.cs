using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SeleniumScript.Components;
using SeleniumScript.Enums;
using SeleniumScript.TestUtils;
using SeleniumScript.Utilities;
using System;
using System.Linq;
using System.Threading;

namespace SeleniumScript.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        [DataRow(Browser.Chrome)]
        public void SwitchTabs(Browser browser)
        {
            var factory = new ManagedWebDriverFactory(browser);

            var options = new ExecutionOptions(factory)
            {
            };

            var result = new Execution(options)
                .NavigateTo(new Uri("https://google.com"))
                .Wait
                    .For(_ => {
                        Thread.Sleep(2000);
                        return true;
                    }, TimeSpan.FromSeconds(2))
                .Then
                .Utils.CreateNewTab()
                .Then
                .SwitchToTab(1)
                .NavigateTo(new Uri("https://youtube.com"))
                .Utils.CloseTab(0).Then
                .Execute();

            result.AssertSuccess();
        }

        [TestMethod]
        [DataRow(Browser.Chrome)]
        public void Login(Browser browser)
        {
            var factory = new ManagedWebDriverFactory(browser);

            var options = new ExecutionOptions(factory)
            {
                OnExecutionCompletion = driver =>
                {
                    Thread.Sleep(10000);
                    return true;
                }
            };

            var result = new Execution(options)
                .NavigateTo(new Uri("https://test.communigator.co.uk/login"), "Go to Test SSO")
                .Login("joe@communigator.co.uk", Environment.GetEnvironmentVariable("AutomatedTesterPassword"))
                .Click(Locator.From(By.CssSelector(".TopBarProduct.GatorPopup")))
                .Wait
                    .ForElementTo(
                        Locator.From(By.XPath("//*[text() = 'Popups']")), 
                        element => element.Displayed, 
                        TimeSpan.FromSeconds(30), "Wait for popup list to load")
                .Then
                .Click(Locator.From(By.CssSelector("[data-select='45']")), "Go to tester folder")
                .Click(Locator.From(By.XPath("//*[text() = 'Create PopUp']")), "Click create popup button")
                .Execute();

            result.AssertSuccess();
        }
    }
}
