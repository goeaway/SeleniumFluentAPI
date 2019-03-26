using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace SeleniumScript.Abstractions
{
    public interface IPage
    {
        IDomain Domain { get; }
        Uri FullUri { get; }
        string RelativeUri { get; }
    }
}
