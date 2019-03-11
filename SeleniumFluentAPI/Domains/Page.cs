using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Executions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Domains
{
    public abstract class Page : IPage
    {
        public IDomain Domain { get; }

        public Uri FullUri => new Uri(Domain.BaseUri, RelativeUri);

        public abstract string RelativeUri { get; }

        public Page(IDomain domain)
        {
            Domain = domain;
        }
    }
}
