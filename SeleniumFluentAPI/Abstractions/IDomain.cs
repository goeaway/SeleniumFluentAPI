using System;
using OpenQA.Selenium;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IDomain
    {
        Uri BaseUri { get; }
    }
}
