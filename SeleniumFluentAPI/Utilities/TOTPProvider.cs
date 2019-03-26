using System;
using System.Collections.Generic;
using System.Text;
using OtpNet;
using SeleniumScript.Abstractions;

namespace SeleniumScript.Utilities
{
    public class TOTPProvider : ITOTPProvider
    {
        public string GetCode(string secretKey)
        {
            var computer = new Totp(Encoding.ASCII.GetBytes(secretKey));
            return computer.ComputeTotp();
        }
    }
}
