using SeleniumScript.Domains;
using System;
using System.Collections.Generic;
using System.Text;
using SeleniumScript.Attributes;

namespace ExampleTestProjest
{
    public class ExampleDomain : Domain
    {
        public ExampleDomain() : base(new Uri("https://example.com"))
        {
        }

        [DefaultPage]
        public LoginPage LoginPage => new LoginPage(this);
    }
}
