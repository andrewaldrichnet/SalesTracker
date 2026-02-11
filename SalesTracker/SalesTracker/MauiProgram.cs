using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using SalesTracker.Data.Sqlite;
using SalesTracker.Services;
using SalesTracker.Shared.Data;
using SalesTracker.Shared.Models;
using SalesTracker.Shared.Services;

namespace SalesTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the SalesTracker.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();
            builder.Services.AddSingleton<IImageCaptureService, MauiImageCaptureService>();
            builder.Services.AddSingleton<IImageStorageService, MauiImageStorageService>();
            builder.Services.AddSingleton<IImageProcessingService, MauiImageProcessingService>();

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "salestracker.db");
            builder.Services.AddDbContext<SalesTrackerDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));
            builder.Services.AddSingleton<IDataStore<Item>, SqliteDataStore<Item>>();
            builder.Services.AddSingleton<IDataStore<Order>, SqliteDataStore<Order>>();
            builder.Services.AddSingleton<IDataStore<ItemImage>, SqliteDataStore<ItemImage>>();
            builder.Services.AddSingleton<ItemService>();
            builder.Services.AddSingleton<OrderService>();
            builder.Services.AddSingleton<DashboardService>();
            builder.Services.AddSingleton<DemoDataService>();
            builder.Services.AddSingleton<DemoDataFlagService, MauiDemoDataFlagService>();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Initialize the database
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SalesTrackerDbContext>();
                dbContext.Database.EnsureCreated();
            }

            return app;
        }
    }
}
