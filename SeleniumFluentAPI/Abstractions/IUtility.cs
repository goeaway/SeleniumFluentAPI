using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IUtility
    {
        IExecution Then { get; }

        IUtility SetCookie(string cookieName, string value);
    }
}
