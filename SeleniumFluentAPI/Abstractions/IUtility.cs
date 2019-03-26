using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Abstractions
{
    public interface IUtility
    {
        IExecution Then { get; }

        IUtility SetCookie(string cookieName, string value);
        IUtility SetWindowMaximised();
        IUtility SetWindowMinimised();
        IUtility SetWindowDimensions(int width, int height);

    }
}
