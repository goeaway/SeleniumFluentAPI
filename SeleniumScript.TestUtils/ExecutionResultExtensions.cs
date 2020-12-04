using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeleniumScript.Components;
using System;
using System.Linq;
using System.Text;

namespace SeleniumScript.TestUtils
{
    public static class ExecutionResultExtensions
    {
        /// <summary>
        /// Utility method that asserts whether an <see cref="ExecutionResult"/> is a success and provides output
        /// </summary>
        /// <param name="result"></param>
        public static void AssertSuccess(this ExecutionResult result)
        {
            if(!result.Success)
            {
                var output = new StringBuilder();
                output.AppendLine("Failed actions:");
                foreach(var act in result.ActionResults.Where(ar => !ar.Success))
                {
                    output.AppendLine($"Name: {act.Context.ActionName}");
                    output.AppendLine($"URL: {act.Context.Url}");
                    output.AppendLine($"Error: {act.InnerException.Message}");
                    output.AppendLine($"Stack: {act.InnerException.StackTrace}");
                    output.AppendLine();
                }

                Assert.Fail($"The execution was a failure\r\n{output}");
            }
        }
    }
}
