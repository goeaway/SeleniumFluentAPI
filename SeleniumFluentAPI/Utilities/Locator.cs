using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using SeleniumScript.Exceptions;

namespace SeleniumScript.Utilities
{
    /// <summary>
    /// Used to identify a single web element by combining a By locator with an integer index 
    /// </summary>
    public struct Locator
    {
        public By By { get; }
        public int Index { get; }

        public Locator(By by, int index)
        {
            By = by;
            Index = index;
        }

        public Locator(By by)
        {
            By = by;
            Index = 0;
        }

        internal IWebElement FindElement(IWebDriver driver)
        {
            try
            {
                return driver.FindElements(By)[Index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new LocatorFindException($"Could not find element with Locator: {By.ToString()} {Index}");
            }
        }
    }
}
