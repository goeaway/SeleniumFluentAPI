using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SeleniumScript.Enums;
using SeleniumScript.Components;
using SeleniumScript.Utilities;

namespace ExampleTestProjest
{
    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public void Example()
        {
            var execution = new Execution()
                .ExceptionOnAssertionFailure(false)
                .RetryCount(3, TimeSpan.FromSeconds(2))
                .NavigateTo(new Uri("https://localhost"))
                .Expect
                    .ToBeOn(new Uri("https://localhost"))
                    .ToBeAbleToSeeElement(Locator.From(By.ClassName("login")))
                    .ToBeAbleToClickElement(Locator.From(By.ClassName("login")))
                .Then
                .Click(Locator.From(By.ClassName("login")))
                .Wait
                    .ForElementToBeDisabled(Locator.From(By.ClassName("login")), TimeSpan.FromSeconds(3))
                    .ForElementToBeHidden(Locator.From(By.ClassName("login")), TimeSpan.FromSeconds(2))
                .Then
                    .NavigateTo(new Uri("https://localhost"))
                .Utils
                    .SetWindowMaximised() 
                .Then
                .Complete();

            var factory = new ManagedWebDriverFactory(Browser.Chrome);
            var result = execution.Execute(factory);
        }
    }
}
