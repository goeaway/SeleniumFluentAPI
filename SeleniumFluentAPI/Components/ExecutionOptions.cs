using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Components
{
    public class ExecutionOptions
    {
        public bool ThrowOnWaitFailure { get; set; }
            = true;
        public bool ThrowOnExecutionFailure { get; set; }
            = true;
        public bool HighlightElementOnClick { get; set; }

        public int ActionRetries { get; set; }
        public TimeSpan ActionRetryWaitPeriod { get; set; }
            = TimeSpan.MinValue;
    }
}
