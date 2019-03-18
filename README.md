# SeleniumFluentAPI
.NET Standard Selenium Fluent API framework to be used in .NET unit tests

This Fluent API allows developers to test their websites by creating a test script, with site navigation, page interaction and assertions each running one after the other. Define a script without executing it straight away! Execute the script when ready and then 
read the results of each script component.

Write tests like the below:

```
[TestMethod]
public void Example()
{
    var domain = new ExampleDomain();

    var execution = Execution
        // create a new execution
        .New()
        // set that failed assertions should not throw an exception
        .ExceptionOnAssertionFailure(false)
        // set the amount of times an execution or assertion retries if a WebDriverException is thrown
        // and the time span to wait between each try
        .RetryCount(3, TimeSpan.FromSeconds(2))
        // access the domain, navigating to the default page of the domain
        .Access(domain)
        // start an IAssertion
        .Expect
            // expect to be on the domain login page
            .ToBeOn(domain.LoginPage)
            // expect to be able to see the login button
            .ToBeAbleSeeElement(domain.LoginPage.LoginButton)
            // expect to be able to click the login button
            .ToBeAbleToClickElement(domain.LoginPage.LoginButton)
        // return to the execution
        .Then
        // click the login button
        .Click(domain.LoginPage.LoginButton)
        // start an IWait
        .Wait
            // wait for the login button to be disabled, with a timeout of 3 seconds
            .ForElementToBeDisabled(domain.LoginPage.LoginButton, TimeSpan.FromSeconds(3))
            // wait for the login button to hide, with a timeout of 2 seconds
            .ForElementToHide(domain.LoginPage.LoginButton, TimeSpan.FromSeconds(2))
        // return to the execution
        .Then
        // navigate to the login page
        .NavigateTo(domain.LoginPage);

    // create a factory the execution can use to create a web driver
    var factory = new WebDriverFactory(Browser.Chrome);
    // execute the execution, this is where we actuall start the test, use the result to assert things
    var result = execution.Execute(factory);
}
```

### Setup

Start by creating a class that inherits from the `Domain` abstract class (or directly from the `IDomain` interface). This object should represent a domain you are going to test (this isn't necessarily tied directly to a website domain, but probably will be in most cases).

This should provide a base `Uri` object to the base constructor.

```
public class ExampleDomain : Domain
{
      public ExampleDomain() : base(new Uri("http://www.example.com"))
      {
      }
}
```

Then start adding classes that inherit from the `Page` abstract class (or directly from the `IPage` interface). These objects represent a single page within a domain, e.g. the index page of a site, or the login page.

The below is an example of a login page.

```
public class LoginPage : Page
{
    public LoginPage(IDomain domain) : base(domain)
    {
    }

    public override string RelativeUri => "/login.html";
        
    public By UsernameInput => By.Id("username");
    public By PasswordInput => By.Id("password");
    public By LoginButton => By.Id("login-button");
}
```

Here, we need to define a read only property `RelativeUri`, this will be used to navigate to this page when executing the test. We also define any web page element `By` identifiers that we might want to interact with on the page. In the example code above, we define `By` objects for the username and password inputs and the login button on the login page. These will be used in our tests to interact with those elements later.

Now return to `ExampleDomain` and add those `Page` objects. Make sure to add the `DefaultPage` attribute to one of the pages, this means we can easily tell which should be the first page to access when we first navigate to the domain.

```
public class ExampleDomain : Domain
{
      ...
      
      [DefaultPage]
      public LoginPage LoginPage => new LoginPage(this);
}
```

### Usage

Now instantiate your domain and use the `Execution.New()` to create a new `Execution` in a unit test, then access the domain via it's default page using the `Access` method:

```
[TestMethod]
public void Example()
{
    var domain = new ExampleDomain();

    var execution = Execution
        .New()
        .Access(domain);
}
```

Then add more `IExecution`, `IAssertion` and `IWait` components to test aspects of your domain. 

| Component | Description |
|---|---|
|`IExecution`| Main actor for the API, exposes many abilities such as clicking an element, typing in an input, navigating to a page or moving the mouse. Just about everything a real user might do. Also allows you to set execution options such as retry counts |
|`IAssertion`| Use these to make sure your site is doing what it should be, test element visibility and availability, browser location and cookie values |
|`IWait`| Exposes abilities to wait for elements to be visible or hidden, enabled or disabled, also allows for just waiting for a period of time |

Once a test script has been defined execute it by providing an `IWebDriverFactory`. Use the resulting collection of `ExcecutionResult` to assert if the test was a success.

```
var domain = new ExampleDomain();

var execution = Execution
    .New()
    .ExceptionOnAssertionFailure(false)
    .RetryCount(3, TimeSpan.FromSeconds(2))
    .Access(domain)
    .Expect
        .ToBeOn(domain.LoginPage)
    .Then
    .Click(domain.LoginPage.LoginButton)
    .Expect
        .Not.ToBeOn(domain.LoginPage)

// use the basic built in factory or create your own if more customisation is required
var factory = new WebDriverFactory(Browser.Chrome);
// use this return result to assert if this test was a success
var result = execution.Execute(factory);
```

### Extend

Build your own execution components to streamline your testing experience. Create extension methods for any of the three main types (`IExecution`, `IAssertion` or `IWait`) to provide reusable implementations of common domain specific actions, such as site login or accessing certain domain areas.

