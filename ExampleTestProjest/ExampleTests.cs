using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumScript.Enums;
using SeleniumScript.Components;
using SeleniumScript.Utilities;

namespace ExampleTestProjest
{
    [TestClass]
    public class ExampleTests
    {
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
                    .ForElementToBeHidden(domain.LoginPage.LoginButton, TimeSpan.FromSeconds(2))
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
