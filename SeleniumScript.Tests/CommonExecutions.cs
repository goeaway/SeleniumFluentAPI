using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using SeleniumScript.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Tests
{
    /// <summary>
    /// Provides common executions in an extension method format. Create a method for each common action that might be repeatedly used
    /// </summary>
    public static class CommonExecutions
    {
        public static IExecution Login(this IExecution execution, string user, string password)
        {
            // input data, then click, then wait for the first product link in the sso bar to be displayed
            return execution
                .Wait
                    .ForElementTo(
                        Locator.From(By.CssSelector("[placeholder='Email']")), 
                        element => element.Displayed,
                        TimeSpan.FromSeconds(10),
                        "Wait for email input to show")
                .Then
                .Input(Locator.From(By.CssSelector("[placeholder='Email']")), user, "Enter User")
                .Input(Locator.From(By.CssSelector("[placeholder='Password']")), password, "Enter Password")
                .Click(Locator.From(By.CssSelector("[type='submit']")))
                .Wait
                    .ForElementTo(
                        Locator.From(By.CssSelector(".productName"), 0), 
                        element => element.Displayed, 
                        TimeSpan.FromSeconds(10), 
                        "Wait for leads button in SSO")
                .Then;
        }
    }
}
