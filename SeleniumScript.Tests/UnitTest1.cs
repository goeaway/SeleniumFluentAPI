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
        public void SwitchTabs(Browser browser)
        {
            var factory = new ManagedWebDriverFactory(browser);

            var options = new ExecutionOptions
            {
                
            };

            var result = new Execution()
                .NavigateTo(new Uri("https://google.com"))
                .Custom(driver =>
                {
                    var handles = driver.WindowHandles;
                }, "Switch Tabs")
                .Execute(factory);

            foreach(var act in result.ActionResults)
            {
                Console.WriteLine($"{act.Context.ActionName} {act.Success} {act.InnerException?.Message}");
            }

            Console.WriteLine(result.ActionResults.Count);
            Assert.IsTrue(result.Success);
        }
    }
}
