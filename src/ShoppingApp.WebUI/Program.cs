using MudBlazor.Services;
using ShoppingApp.Grains;
using ShoppingApp.WebUI;
using ShoppingApp.WebUI.Cart;
using ShoppingApp.WebUI.Products;
using ShoppingApp.WebUI.Services;
using ShoppingApp.WebUI.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ShoppingCartService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ComponentStateChangedObserver>();
builder.Services.AddScoped<ToastService>();
builder.Services.AddLocalStorageServices();

builder.Host.UseOrleans((context, siloBuilder) =>
{
    siloBuilder.UseLocalhostClustering()
        .AddMemoryGrainStorage(PersistentStorageConfig.AzureSqlName)
        .AddMemoryGrainStorage(PersistentStorageConfig.AzureStorageName);
    
    siloBuilder.AddStartupTask<SeedProductStoreTask>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this
    // for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();