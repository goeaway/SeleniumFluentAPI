using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using SeleniumScript.Domains;
using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleTests.Google.Pages
{
    public class SearchPage : Page
    {
        public SearchPage(IDomain domain) : base(domain)
        {
        }

        public override string RelativeUri => "/search";

        public By ResultsLinks => By.CssSelector(".g .r a");
    }
}
