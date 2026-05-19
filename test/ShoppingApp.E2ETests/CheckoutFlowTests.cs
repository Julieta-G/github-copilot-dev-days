using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace ShoppingApp.E2ETests;

public sealed class CheckoutFlowTests : E2ETestBase
{
    [Test]
    public async Task CheckoutFlow_Should_Complete_Short_Happy_Path()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Cart" }).ClickAsync();

        var emptyCartButton = Page.GetByRole(AriaRole.Button, new() { Name = "Empty Cart" });
        if (await emptyCartButton.IsEnabledAsync())
        {
            await emptyCartButton.ClickAsync();
        }

        await Page.GetByRole(AriaRole.Link, new() { Name = "Shop Inventory" }).ClickAsync();

        var firstProductRow = Page.GetByRole(AriaRole.Row).Nth(1);
        var productName = (await firstProductRow.Locator("td").Nth(1).InnerTextAsync()).Trim();
        await firstProductRow.GetByRole(AriaRole.Button).ClickAsync();

        await Page.GetByRole(AriaRole.Link, new() { Name = "Cart" }).ClickAsync();

        await Expect(Page.GetByText(new Regex($"^1x {Regex.Escape(productName)}$"))).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Checkout" }).ClickAsync();
        await Expect(Page.GetByText("Payment successful.")).ToBeVisibleAsync();

        var screenshotPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "checkout-success.png");
        await Page.ScreenshotAsync(new PageScreenshotOptions { Path = screenshotPath, FullPage = true });
        TestContext.AddTestAttachment(screenshotPath, "Checkout success screenshot");
    }
}
