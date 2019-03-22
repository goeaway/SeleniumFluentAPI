using OpenQA.Selenium;
using SeleniumFluentAPI.Abstractions;
using SeleniumFluentAPI.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleTests.Google.Pages
{
    public class HomePage : Page
    {
        public HomePage(IDomain domain) : base(domain)
        {
        }

        public override string RelativeUri => "/";

        public By SearchInput => By.Name("q");
    }
}
