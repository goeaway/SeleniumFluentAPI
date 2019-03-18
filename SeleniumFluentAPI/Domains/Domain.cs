using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Executions;
using SeleniumFluentAPI.Attributes;
using SeleniumFluentAPI.Exceptions;
using SeleniumFluentAPI.Utilities;

namespace SeleniumFluentAPI.Domains
{
    public class Domain : IDomain
    {
        public Uri BaseUri { get; }

        public Domain(Uri baseUri)
        {
            BaseUri = baseUri;
        }
    }
}
