using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;
using SalesTracker.Shared.Services;
using SalesTracker.Web.Client;
using SalesTracker.Web.Client.Data;
using SalesTracker.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<SalesTracker.Web.Client.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });


// Add device-specific services used by the SalesTracker.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddSingleton<IImageCaptureService, WebImageCaptureService>();
builder.Services.AddSingleton<IImageStorageService, WebAssemblyImageStorageService>();
builder.Services.AddSingleton<IImageProcessingService, WebAssemblyImageProcessingService>();
builder.Services.AddSingleton<IDataStore<Item>, IndexedDbDataStore<Item>>();
builder.Services.AddSingleton<IDataStore<Order>, IndexedDbDataStore<Order>>();
builder.Services.AddSingleton<IDataStore<ItemImage>, IndexedDbDataStore<ItemImage>>();
builder.Services.AddSingleton<ItemService>();
builder.Services.AddSingleton<OrderService>();
builder.Services.AddSingleton<DashboardService>();
builder.Services.AddSingleton<WebAssemblyDemoDataFlagService>();
builder.Services.AddSingleton<DemoDataFlagService>(sp => sp.GetRequiredService<WebAssemblyDemoDataFlagService>());
builder.Services.AddSingleton<DemoDataService>();

await builder.Build().RunAsync();
