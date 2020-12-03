using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Components
{
    public class ExecutionOptions
    {
        /// <summary>
        /// Gets or sets if elements that are clicked should be highlighted in the browser for a period of time
        /// </summary>
        public bool HighlightElementOnClick { get; set; }
        /// <summary>
        /// Gets or sets how many times an execution should be retried before being considered a failure
        /// </summary>
        public int ActionRetries { get; set; }
        /// <summary>
        /// Gets or sets a collection of wait periods to be used between retries in order. 
        /// The collection can include 0 or more items. 
        /// If the collection is empty a default <see cref="TimeSpan.MinValue"/> is used.
        /// If the amount of retries is greater than the count of this collection, the collection is repeated.
        /// </summary>
        public ICollection<TimeSpan> ActionRetryWaitPeriods { get; set; }
            = new List<TimeSpan>();
    }
}
