using System;
using SeleniumScript.Abstractions;

namespace SeleniumScript.Domains
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
