using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace ShoppingApp.E2ETests;

public abstract class E2ETestBase : PageTest
{
    protected string BaseUrl { get; private set; } = null!;

    [SetUp]
    public void SetUpBaseUrl()
    {
        BaseUrl = Environment.GetEnvironmentVariable("BASE_URL") ?? "http://127.0.0.1:5169";
    }

    [TearDown]
    public async Task CaptureScreenshotOnFailureAsync()
    {
        if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
        {
            return;
        }

        var screenshotPath = Path.Combine(
            TestContext.CurrentContext.WorkDirectory,
            $"{TestContext.CurrentContext.Test.ID}.png");

        await Page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = screenshotPath,
            FullPage = true
        });

        TestContext.AddTestAttachment(screenshotPath, "Failure screenshot");
    }
}
