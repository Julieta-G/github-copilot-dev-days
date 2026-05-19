using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace ShoppingApp.E2ETests;

public sealed class CartFlowTests : E2ETestBase
{
    [Test]
    public async Task CartFlow_Should_Add_Product_To_Cart()
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

        var cartSummaryItem = Page.GetByText(new Regex($"^1x {Regex.Escape(productName)}$"));
        await Expect(cartSummaryItem).ToBeVisibleAsync();
        await Expect(Page.GetByText("Pretax Total:")).ToBeVisibleAsync();
    }
}
