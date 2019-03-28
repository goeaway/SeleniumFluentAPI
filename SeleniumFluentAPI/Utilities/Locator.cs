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

        private Locator(By by, int index)
        {
            By = by;
            Index = index;
        }

        private Locator(By by)
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
            catch (ArgumentOutOfRangeException)
            {
                throw new LocatorFindException($"Could not find element with Locator: {By.ToString()} {Index}");
            }
            catch (IndexOutOfRangeException)
            {
                throw new LocatorFindException($"Could not find element with Locator: {By.ToString()} {Index}");
            }
        }

        public static Locator From(By by)
        {
            if (by == null)
                throw new ArgumentNullException(nameof(by));

            return new Locator(by);
        }

        public static Locator From(By by, int index)
        {
            if (by == null)
                throw new ArgumentNullException(nameof(by));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            return new Locator(by, index);
        }
    }
}
