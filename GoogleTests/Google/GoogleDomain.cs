using System;
using System.Collections.Generic;
using System.Text;
using GoogleTests.Google.Pages;
using SeleniumFluentAPI.Attributes;
using SeleniumFluentAPI.Domains;

namespace GoogleTests.Google
{
    public class GoogleDomain : Domain
    {
        public GoogleDomain() : base(new Uri("https://www.google.co.uk"))
        {
        }

        [DefaultPage]
        public HomePage HomePage => new HomePage(this);
    }
}
