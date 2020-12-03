using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Components
{
    /// <summary>
    /// Represents an action performed in an execution. Contains information on whether it was a success and why if not
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// Gets if the action was a success (didn't cause an exception stored in <see cref="InnerException"/>)
        /// </summary>
        public bool Success => InnerException == null;
        /// <summary>
        /// Gets the inner exception that occurred during this action, if at all
        /// </summary>
        public Exception InnerException { get; internal set; }
        /// <summary>
        /// Gets the context associated with this context
        /// </summary>
        public ExecutionContext Context { get; internal set; }

        internal ActionResult() { }
    }
}
