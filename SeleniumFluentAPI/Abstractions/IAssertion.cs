using OpenQA.Selenium;
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
        /// <summary>
        /// Asserts that the browser is currently located at the specified <see cref="IPage"/>
        /// </summary>
        /// <param name="page">An <see cref="IPage"/> representation of a page the browser is expected to be on</param>
        /// <returns></returns>
        IAssertion ToBeOn(IPage page);
        /// <summary>
        /// Asserts that the browser is currently located at the specified <see cref="IPage"/>
        /// </summary>
        /// <param name="page">An <see cref="IPage"/> representation of a page the browser is expected to be on</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ToBeOn(IPage page, string actionName);
        /// <summary>
        /// Asserts that the browser is currently located at the specified <see cref="Uri"/>
        /// </summary>
        /// <param name="page">An <see cref="Uri"/> representation of a Uri the browser is expected to be on</param>
        /// <returns></returns>
        IAssertion ToBeOn(Uri uri);
        /// <summary>
        /// Asserts that the browser is currently located at the specified <see cref="Uri"/>
        /// </summary>
        /// <param name="page">An <see cref="Uri"/> representation of a Uri the browser is expected to be on</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ToBeOn(Uri uri, string actionName);
        /// <summary>
        /// Asserts that the provided <see cref="Func{IWebDriver, bool}"/> returns true
        /// </summary>
        /// <param name="assertion">A function that will take in a web driver and return a bool representing the result of an assertion</param>
        /// <returns></returns>
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion);
        /// <summary>
        /// Asserts that the provided <see cref="Func{IWebDriver, bool}"/> returns true
        /// </summary>
        /// <param name="assertion">A function that will take in a web driver and return a bool representing the result of an assertion</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ToBeTrue(Func<IWebDriver, bool> assertion, string actionName);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier is visible to the browser user
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <returns></returns>
        IAssertion ToBeAbleSeeElement(By by);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier is visible to the browser user
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ToBeAbleSeeElement(By by, string actionName);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier is clickable by the browser user
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <returns></returns>
        IAssertion ToBeAbleToClickElement(By by);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier is clickable by the browser user
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ToBeAbleToClickElement(By by, string actionName);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier has the specified CSS class
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="className">The CSS class the element is expected to have</param>
        /// <returns></returns>
        IAssertion ElementToHaveClass(By by, string className);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier has the specified CSS class
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="className">The CSS class the element is expected to have</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ElementToHaveClass(By by, string className, string actionName);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier has the specified attribute
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="attribute">The attribute name the element is expected to have</param>
        /// <returns></returns>
        IAssertion ElementToHaveAttr(By by, string attribute);
        /// <summary>
        /// Asserts that the element found by the provided <see cref="By"/> identifier has the specified attribute
        /// </summary>
        /// <param name="by">A <see cref="By"/> to locate a page element with</param>
        /// <param name="attribute">The attribute the element is expected to have</param>
        /// <param name="actionName">A string name for this Assertion to make it easier for humans to identify this component</param>
        /// <returns></returns>
        IAssertion ElementToHaveAttr(By by, string attribute, string actionName);
        IAssertion ElementToHaveAttrValue(By by, string attribute, string value);
        IAssertion ElementToHaveAttrValue(By by, string attribute, string value, string actionName);
        IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate);
        IAssertion ElementToPassThis(By by, Predicate<IWebElement> predicate, string actionName);
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
