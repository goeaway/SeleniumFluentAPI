using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeleniumScript.Utilities
{
    /// <summary>
    /// Tool to highlight a specified element on the screen for easier element locator debugging
    /// </summary>
    public static class Highlighter
    {
        private const int HIGHLIGHT_MILLISECONDS = 5000;

        private static string ConvertColorToRGBAString(Color color)
        {
            return $"rgba({color.R},{color.G},{color.B},{color.A})";
        }

        public static void Highlight(IWebDriver driver, IWebElement element)
        {
            Highlight(driver, element, Color.Yellow);
        }

        public static void Highlight(IWebDriver driver, IWebElement element, Color color)
        {
            var colorStr = ConvertColorToRGBAString(color);

            try
            {
                // get the existing value first so we can set it back after a period of time
                var existingValue = element.GetCssValue("background");
                // set the background to the color value
                driver.ExecuteJavaScript<string>($"arguments[0].style.background='{colorStr}'", element);

                // create a task that'll return the element's background to the original after a period of time
                Task.Run(() =>
                {
                     driver.ExecuteJavaScript<string>($"arguments[0].style.background='{existingValue}'", element);
                });
            }
            catch (WebDriverException)
            {

            }
        }
    }
}