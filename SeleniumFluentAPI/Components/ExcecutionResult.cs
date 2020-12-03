using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SeleniumScript.Abstractions;
using SeleniumScript.Enums;

namespace SeleniumScript.Components
{
    /// <summary>
    /// Represents the result of an execution. Contains a collection of <see cref="ActionResult"/>
    /// </summary>
    public class ExecutionResult
    {
        public bool Success => ActionResults.Count == 0 || ActionResults.All(a => a.Success);

        private readonly List<ActionResult> _actionResults;
        public IReadOnlyCollection<ActionResult> ActionResults => _actionResults;

        internal ExecutionResult()
        {
            _actionResults = new List<ActionResult>();
        }

        internal void AddActionResult(ActionResult actionResult)
        {
            if(actionResult == null)
            {
                throw new ArgumentNullException(nameof(actionResult));
            }

            _actionResults.Add(actionResult);
        }
    }
}
