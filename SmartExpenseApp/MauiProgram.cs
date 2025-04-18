using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SmartExpenseApp.Data;
using SmartExpenseApp.Utilities;
using SmartExpenseApp.Views;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;
using The49.Maui.BottomSheet;

namespace SmartExpenseApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .UseBottomSheet()
                // Initialize the .NET MAUI Community Toolkit
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<HomeScreen>();
            builder.Services.AddTransient<HomeScreen>();

            builder.Services.AddSingleton<AddTransactionPage>();
            builder.Services.AddTransient<AddTransactionPage>();

            builder.Services.AddSingleton<TransactionsListPage>();
            builder.Services.AddTransient<TransactionsListPage>();

            builder.Services.AddSingleton<GraphAndTrends>();
            builder.Services.AddTransient<GraphAndTrends>();

            builder.Services.AddSingleton<SmartExpenseAppDatabase>(provider =>
                new SmartExpenseAppDatabase(Constants.DatabasePath));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}