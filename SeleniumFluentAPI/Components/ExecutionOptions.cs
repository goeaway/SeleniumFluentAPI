using OpenQA.Selenium;
using SeleniumScript.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeleniumScript.Components
{
    public class ExecutionOptions
    {
        public ExecutionOptions(IWebDriverFactory webDriverFactory)
        {
            WebDriverFactory = webDriverFactory ?? throw new ArgumentNullException(nameof(webDriverFactory));
        }

        public ExecutionOptions(IWebDriver webDriver)
        {
            WebDriver = webDriver ?? throw new ArgumentNullException(nameof(webDriver));
        }

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
        /// <summary>
        /// Gets the <see cref="IWebDriverFactory"/> to be used to create the web driver when the execution should start. 
        /// The advantage of using this factory over just providing an <see cref="IWebDriver"/> is that you can delay the opening of a browser
        /// window until the execution is ready to use it.
        /// </summary>
        public IWebDriverFactory WebDriverFactory { get; }
        /// <summary>
        /// Gets the <see cref="IWebDriver"/> to be used in the execution to interact with a browser. It's reccommended
        /// to use <see cref="WebDriverFactory"/> instead to delay opening of the browser. <see cref="WebDriverFactory"/> takes precendant
        /// if both are assigned values
        /// </summary>
        public IWebDriver WebDriver { get; }
        /// <summary>
        /// Gets or sets a callback to be run by the execution when it starts.
        /// </summary>
        public Action<IWebDriver> OnExecutionStart { get; set; }
        /// <summary>
        /// Gets or sets a callback to be run by the execution before each action starts.
        /// </summary>
        public Action<IWebDriver, IExecutionContext> OnActionStart { get; set; }
        /// <summary>
        /// Gets or sets a callback to be run by the execution after it has finished. 
        /// Returning true from this signals to the execution that the <see cref="IWebDriver"/> should be disposed of, false
        /// will not. If this property is not set the <see cref="IWebDriver"/> will be disposed of by the execution.
        /// </summary>
        public Func<IWebDriver, bool> OnExecutionCompletion { get; set; }
    }
}
