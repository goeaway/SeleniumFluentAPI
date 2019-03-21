using SeleniumFluentAPI.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Components
{
    public class Utility : IUtility
    {
        private readonly IExecution _execution;

        public IExecution Then => throw new NotImplementedException();

        public Utility(IExecution execution)
        {
            _execution = execution;
        }

        public IUtility SetCookie(string cookieName, string value)
        {
            throw new NotImplementedException();
        }

        public IUtility SetWindowDimensions(int width, int height)
        {
            throw new NotImplementedException();
        }

        public IUtility SetWindowMaximised()
        {
            throw new NotImplementedException();
        }

        public IUtility SetWindowMinimised()
        {
            throw new NotImplementedException();
        }
    }
}
