# SeleniumScript
.NET Core 3.1 Selenium Fluent API framework to be used in .NET unit tests

Selenium is a great tool to automate website testing with, but the breadth of the API can quickly lead to difficult to read and hard to maintain test scripts. This nuget package attempts to hide away some of the implementation details and keep you writing tests in a structured, readable manner, so you can spend more time on the code you want to write!

## Features
- Readable - Allows you to build a test in a fluent API manner, with common sense names for methods that allow you and your team to get the jist of what a test is doing at a glance.
- Unopinionated - Although the library attempts to hide the details, We haven't added an obtuse abstraction layer between you and Selenium. All of the normal Selenium APIs, such as `IWebDriver`, `IWebElement`, `By` and more are available. Methods are included to allow you to get straight to the IWebDriver instance at any time, in case SeleniumScript doesn't offer an adequate method for your needs.
- Extendable - The Fluent style makes it easy to create your own extension methods to encapsulate common test steps you may use multiple times, such as logging into a website.
- Failure Resiliency - There's nothing more annoying than having a test fail because an element appeared .02 seconds after it was supposed to. SeleniumScript makes it easy to wait for elements and has resiliency built in to avoid these pesky issues by making use of [Polly](https://github.com/App-vNext/Polly)!

## Usage

SeleniumScript is best used in a test environment, where the results of the test can easily be asserted. The below code shows an example test 
```
[TestMethod]
public void MySeleniumScriptTest()
{
    // use the built in web driver factory so you don't have to manage the browser driver executables yourself
    // we use a factory to delay the creation of the web driver until the execution actually starts
    var factory = new ManagedWebDriverFactory(Browser.Chrome);

    // create an options object, where we can provide some required options (web driver factory) and some optional ones
    var options = new ExecutionOptions(factory);

    // create a new execution here
    // adding new actions with each method or property call
    var result = new Execution(options)
        .NavigateTo(new Uri("https://www.bbc.co.uk/")) // navigates the browser to the specified URL
        .Wait // some actions are grouped together, we can access methods to wait for things by using the .Wait property
            .ForElementTo(
                Locator.From(By.XPath("//*[text() = 'Welcome to the BBC']")), // define which element you want to wait for
                element => element.Displayed, // define how to wait for it, here we're waiting until the element is displayed
                TimeSpan.FromSeconds(5), // provide a timeout for the execution to give up waiting (will stop execution)
                "Wait for title to show" // It's best to provide unique names to each action you make, this makes it easier to see which actions have failed, if any
            )
        .Then // return back to the main Execution object for more options
        .Expect // .Expect property is used to assert things
            .ToBe(driver => driver.Url == "https://www.bbc.co.uk/", "Expect to be on the bbc home page") // here we are expecting the browser to be on a certain page
        .Then
        .Execute(); // call this to finally execute the test, don't forget to call this!

    Assert.IsTrue(result.Success); // success of the test is found in the result, along with information on which actions failed, if any
}
```

## Organisation

The SeleniumScript API usually accepts common .NET types as parameters, this means you don't have to learn too many new things, but can make reading a bit hard when you've built up a long test with lots of string selectors and urls. For this reason, it's recommended you organise DOM element references, urls and similar in groups to make it easier to read and find them. How you do this is really up to you, but a good start is to keep elements you'd find on the same page in the same static class, and then reference them in your test.

```
public static class HomePage {
    public static Uri URI = new Uri("https://mysite.com/home");

    public static Locator Title = Locator.From(By.XPath("//*[text() = 'my home page title']"));
    public static Locator CTAButton = Locator.From(By.CssSelector(".call-to-action"));
}
```

## Extending

As SeleniumScript uses a fluent API, it's really easy to extend the core functionality in any way you want. Perhaps there's a sequence of steps you always have to do to get to where you want to test, like logging in to your site. Create an extensions class like the below, and then call the methods like you would any other execution step.

```
public static class CommonExecutions
    {
        public static IExecution Login(this IExecution execution, string user, string password)
        {
            // input data, then click, then wait for the page to change
            return execution
                .Wait
                    .ForElementTo(
                        Locator.From(By.CssSelector("[placeholder='Email']")), 
                        element => element.Displayed,
                        TimeSpan.FromSeconds(10),
                        "Wait for email input to show")
                .Then
                .Input(Locator.From(By.CssSelector("[placeholder='Email']")), user, "Enter User")
                .Input(Locator.From(By.CssSelector("[placeholder='Password']")), password, "Enter Password")
                .Click(Locator.From(By.CssSelector("[type='submit']")))
                .Wait
                    .For(driver => driver.Url.Contains("/dashboard"), TimeSpan.FromSeconds(2), "Wait to be taken to dashboard")
                .Then;
        }
    }
```


```
[TestMethod]
public void Login()
{
    var factory = new ManagedWebDriverFactory(Browser.Chrome);

    var options = new ExecutionOptions(factory);

    var result = new Execution(options)
        .NavigateTo(new Uri("https://mysite.com/login"), "Go to log in page")
        .Login("example@mysite.com", Environment.GetEnvironmentVariable("UserPassword"))
        .Expect
            .ToBe(driver => driver.Url.Contain("/dashboard"), "Expect to be on the dashboard")
        .Then
        .Execute();
        
    Assert.IsTrue(result.Success);
}
```
