using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Abstractions
{
    public interface ITOTPProvider
    {
        string GetCode(string secretKey);
    }
}
