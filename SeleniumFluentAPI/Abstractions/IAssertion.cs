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
        IAssertion ToBeOn(IPage page);
        IAssertion ToBeOn(IPage page, string actionName);
        IAssertion ToBeOn(Uri uri);
        IAssertion ToBeOn(Uri uri, string actionName);
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion);
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion, string actionName);
        IAssertion ToBeAbleSeeElement(Locator locator);
        IAssertion ToBeAbleSeeElement(Locator locator, string actionName);
        IAssertion ToBeAbleToClickElement(Locator locator);
        IAssertion ToBeAbleToClickElement(Locator locator, string actionName);
        IAssertion ElementToHaveClass(Locator locator, string className);
        IAssertion ElementToHaveClass(Locator locator, string className, string actionName);
        IAssertion ElementToHaveAttr(Locator locator, string attribute);
        IAssertion ElementToHaveAttr(Locator locator, string attribute, string actionName);
        IAssertion ElementToHaveAttrValue(Locator locator, string attribute, string value);
        IAssertion ElementToHaveAttrValue(Locator locator, string attribute, string value, string actionName);
        IAssertion ElementToPassThis(Locator locator, Predicate<IWebElement> predicate);
        IAssertion ElementToPassThis(Locator locator, Predicate<IWebElement> predicate, string actionName);
        IAssertion ElementsToPassThis(By by, Predicate<IReadOnlyCollection<IWebElement>> predicate);
        IAssertion ElementsToPassThis(By by, Predicate<IReadOnlyCollection<IWebElement>> predicate, string actionName);

        IAssertion CookieToExist(string cookieName);
        IAssertion CookieToExist(string cookieName, string actionName);
        IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate);
        IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate, string actionName);
        IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate);
        IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate, string actionName);
        IAssertion FileToBeDownloaded(string filename);
        IAssertion FileToBeDownloaded(string filename, string actionName);
        IAssertion FileToBeDownloaded(Regex filenamePattern);
        IAssertion FileToBeDownloaded(Regex filenamePattern, string actionName);

        /// <summary>
        /// Inverts the succeeding assertion
        /// </summary>
        IAssertion Not { get; }
        IExecution Then { get; }
    }
}
