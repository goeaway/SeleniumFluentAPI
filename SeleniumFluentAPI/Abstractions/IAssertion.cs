using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IAssertion
    {
        IAssertion ToBeOn(IPage page);
        IAssertion ToBeOn(Uri uri);
        IAssertion ToBeTrue(Func<bool> assertion);
        IAssertion ToBeAbleSeeElement(By by);
        IAssertion ToBeAbleToClickElement(By by);
        IAssertion ElementToHaveClass(By by, string className);
        IAssertion ElementToHaveAttr(By by, string attribute);
        IAssertion ElementToHaveAttr(By by, string attribute, string value);
        IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate);
        IAssertion CookieToExist(string cookieName);
        IAssertion CookieToPassThis(string cookieName, Predicate<Cookie> predicate);
        /// <summary>
        /// Inverts the succeeding assertion
        /// </summary>
        IAssertion Not { get; }
        IExecutable Then { get; }
    }
}
