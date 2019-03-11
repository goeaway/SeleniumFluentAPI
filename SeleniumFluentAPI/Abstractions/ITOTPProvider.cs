using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumFluentAPI.Abstractions
{
    public interface ITOTPProvider
    {
        string GetCode(string secretKey);
    }
}
