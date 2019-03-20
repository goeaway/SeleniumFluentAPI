using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumFluentAPI.Enums;
using SeleniumFluentAPI.Components;
using SeleniumFluentAPI.Utilities;

namespace ExampleTestProjest
{
    [TestClass]
    public class ExampleTests
    {
        [TestMethod]
        public void Example()
        {
            var domain = new ExampleDomain();

            var execution = Execution
                .New()
                .ExceptionOnAssertionFailure(false)
                .RetryCount(3, TimeSpan.FromSeconds(2))
                .Access(domain)
                .Expect
                    .ToBeOn(domain.LoginPage)
                    .ToBeAbleSeeElement(domain.LoginPage.LoginButton)
                    .ToBeAbleToClickElement(domain.LoginPage.LoginButton)
                .Then
                .Click(domain.LoginPage.LoginButton)
                .Wait
                    .ForElementToBeDisabled(domain.LoginPage.LoginButton, TimeSpan.FromSeconds(3))
                    .ForElementToHide(domain.LoginPage.LoginButton, TimeSpan.FromSeconds(2))
                .Then
                .NavigateTo(domain.LoginPage)
                .Utils
                    .SetWindowMaximised()
                .Then
                .Complete();

            var factory = new ManagedWebDriverFactory(Browser.Chrome);
            var result = execution.Execute(factory);
        }
    }
}
