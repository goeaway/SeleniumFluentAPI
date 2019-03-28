using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using SeleniumScript.Domains;
using SeleniumScript.Utilities;
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

        public Locator SearchInput => Locator.From(By.Name("q"));
        public Locator SearchButton => Locator.From(By.Name("btnK"));
    }
}
