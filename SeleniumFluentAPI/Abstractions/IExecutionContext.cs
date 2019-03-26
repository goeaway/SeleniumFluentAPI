using System;
using System.Collections.Generic;
using System.Text;
using SeleniumScript.Enums;

namespace SeleniumScript.Abstractions
{
    public interface IExecutionContext
    {
        string CurrentUrl { get; }
        string ActionName { get; }
    }
}
