using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IPage
    {
        IDomain Domain { get; }
        Uri FullUri { get; }
        string RelativeUri { get; }
    }
}
