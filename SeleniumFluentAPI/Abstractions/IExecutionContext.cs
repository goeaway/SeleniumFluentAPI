using System;
using System.Collections.Generic;
using System.Text;
using SeleniumFluentAPI.Enums;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IExecutionContext
    {
        string CurrentUrl { get; }
        ComponentType Type { get; }
        string ActionName { get; }
    }
}
