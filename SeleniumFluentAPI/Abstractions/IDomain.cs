using System;
using OpenQA.Selenium;
using SeleniumScript.Utilities;

namespace SeleniumScript.Abstractions
{
    public interface IDomain
    {
        Uri BaseUri { get; }
    }
}
