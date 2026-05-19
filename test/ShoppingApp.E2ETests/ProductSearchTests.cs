using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NUnit.Framework;

namespace ShoppingApp.E2ETests;

public sealed class ProductSearchTests : E2ETestBase
{
    [Test]
    public async Task ProductSearch_Should_Filter_Products()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByRole(AriaRole.Link, new() { Name = "Shop Inventory" }).ClickAsync();

        var firstProductName = (await Page.GetByRole(AriaRole.Row).Nth(1).Locator("td").Nth(1).InnerTextAsync()).Trim();
        var searchTerm = firstProductName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];

        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Search products" }).FillAsync(searchTerm);

        var matchingProduct = Page.GetByRole(AriaRole.Cell, new() { NameRegex = new Regex(Regex.Escape(firstProductName)) });
        await Expect(matchingProduct).ToBeVisibleAsync();
    }
}
