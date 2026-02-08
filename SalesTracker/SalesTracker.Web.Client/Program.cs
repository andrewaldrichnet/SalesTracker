using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;
using SalesTracker.Shared.Services;
using SalesTracker.Web.Client.Services;
using SalesTracker.Web.Client.Data;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

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
builder.Services.AddSingleton<DemoDataService>();

await builder.Build().RunAsync();
