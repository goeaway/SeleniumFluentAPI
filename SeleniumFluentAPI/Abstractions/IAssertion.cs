using OpenQA.Selenium;
using SeleniumScript.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SeleniumScript.Abstractions
{
    /// <summary>
    /// Defines the interface through which consumers can assert the state or behaviour of a page or it's elements
    /// </summary>
    public interface IAssertion
    {
        IAssertion ToBe(Func<IWebDriver, bool> predicate, string actionName = "ToBe");
        IAssertion ElementToBe(Locator locator, Func<IWebElement, bool> predicate, string actionName = "Element To Be");
        
        /// <summary>
        /// Inverts the succeeding assertion
        /// </summary>
        IAssertion Not { get; }
        IExecution Then { get; }
    }
}
