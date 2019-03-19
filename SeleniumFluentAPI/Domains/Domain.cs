using System;
using SeleniumFluentAPI.Abstractions;

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
