using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class DefaultPageAttribute : Attribute
    {

    }
}
