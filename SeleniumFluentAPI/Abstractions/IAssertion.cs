using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IAssertion
    {
        IAssertion ToBeOn(IPage page);
        IAssertion ToBeOn(IPage page, string actionName);
        IAssertion ToBeOn(Uri uri);
        IAssertion ToBeOn(Uri uri, string actionName);
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion);
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion, string actionName);
        IAssertion ToBeAbleSeeElement(By by);
        IAssertion ToBeAbleSeeElement(By by, string actionName);
        IAssertion ToBeAbleToClickElement(By by);
        IAssertion ToBeAbleToClickElement(By by, string actionName);
        IAssertion ElementToHaveClass(By by, string className);
        IAssertion ElementToHaveClass(By by, string className, string actionName);
        IAssertion ElementToHaveAttr(By by, string attribute);
        IAssertion ElementToHaveAttr(By by, string attribute, string actionName);
        IAssertion ElementToHaveAttrValue(By by, string attribute, string value);
        IAssertion ElementToHaveAttrValue(By by, string attribute, string value, string actionName);
        IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate);
        IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate, string actionName);
        IAssertion CookieToExist(string cookieName);
        IAssertion CookieToExist(string cookieName, string actionName);
        IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate);
        IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate, string actionName);
        IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate);
        IAssertion CurrentPageNetworkEntriesPassThis(Predicate<string> predicate, string actionName);
        /// <summary>
        /// Inverts the succeeding assertion
        /// </summary>
        IAssertion Not { get; }
        IExecution Then { get; }
    }
}
