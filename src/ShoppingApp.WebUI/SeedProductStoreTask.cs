using JetBrains.Annotations;
using ShoppingApp.Abstractions;
using ShoppingApp.Abstractions.Configuration;
using ShoppingApp.WebUI.Extensions;

namespace ShoppingApp.WebUI;

public sealed class SeedProductStoreTask(IGrainFactory grainFactory) : IStartupTask
{
    [UsedImplicitly]
	async Task IStartupTask.Execute(CancellationToken cancellationToken)
    {
        var globalStartupGrain = grainFactory.GetGrain<IGlobalStartupGrain>(nameof(IGlobalStartupGrain));
        if (await globalStartupGrain.IsProductStoreInitialized()) return;

        var faker = new ProductDetails().GetBogusFaker();

        foreach (var product in faker.GenerateLazy(1000))
        {
            var productGrain = grainFactory.GetGrain<IProductGrain>(product.Id);
            await productGrain.CreateOrUpdateProductAsync(product);
        }

        await globalStartupGrain.CompleteProductStoreInitialization();
    }
}