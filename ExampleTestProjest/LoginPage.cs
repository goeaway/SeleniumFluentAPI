using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using SeleniumScript.Domains;

namespace ExampleTestProjest
{
    public class LoginPage : Page
    {
        public LoginPage(IDomain domain) : base(domain)
        {
        }

        public override string RelativeUri => "/account/login";

        public By LoginButton => By.Id("login-button");
    }
}
