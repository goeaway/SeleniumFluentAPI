using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumScript.Utilities
{
    public static class DriverQuitter
    {
        public static void Quit(IWebDriver driver)
        {
            driver.Quit();
            
            // kill processes if they linger (only on local though!!!)
        }
    }
}
