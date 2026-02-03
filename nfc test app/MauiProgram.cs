using Microsoft.Extensions.Logging;
using nfc_test_app.Interfaces;
using nfc_test_app.Services;
using nfc_test_app.ViewModels;

namespace nfc_test_app
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register services
            builder.Services.AddSingleton<INFCService, Nfcservice>();

            // Register ViewModels
            builder.Services.AddSingleton<MainViewModel>();

            // Register Pages
            builder.Services.AddSingleton<MainPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
