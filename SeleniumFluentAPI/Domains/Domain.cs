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

        public IExecutable Start()
        {
            return Start(0, TimeSpan.FromSeconds(1));
        }

        public IExecutable Start(int actionRetryCount, TimeSpan actionRetryWaitPeriod)
        {
            var pagesWithAttribute =
                GetType()
                    .GetProperties()
                    .Where(p => Attribute.IsDefined(p, typeof(DefaultPageAttribute)) &&
                                p.PropertyType.IsSubclassOf(typeof(Page)));

            if (pagesWithAttribute.Count() == 0)
                throw new DefaultPageNotFoundException("Default page could not be found when navigating to domain");

            if (pagesWithAttribute.Count() != 1)
                throw new MultipleDefaultPagesFoundException("Multiple default pages were found for domain when trying to navigate to it");

            var pageToInst = pagesWithAttribute.First().PropertyType;
            var ctor = pageToInst.GetConstructor(new Type[] {typeof(IDomain)});

            if(ctor == null) 
                throw new InvalidOperationException();

            var page = (IPage)ctor.Invoke(new object[] {this});
            
            // default if not set
            if (actionRetryWaitPeriod == null)
                actionRetryWaitPeriod = TimeSpan.FromSeconds(1);

            if (actionRetryCount < 0)
                actionRetryCount = 0;

            return new Execution(actionRetryCount, actionRetryWaitPeriod).NavigateTo(page);
        }
    }
}
