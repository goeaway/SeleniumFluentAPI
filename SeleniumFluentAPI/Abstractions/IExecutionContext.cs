using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Abstractions
{
    public interface IExecutionContext
    {
        string CurrentUrl { get; } 
        string ActionName { get; }
    }
}
