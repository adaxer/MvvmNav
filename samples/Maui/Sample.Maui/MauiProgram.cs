using Microsoft.Extensions.Logging;

namespace Sample.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>(sp => new App(sp))
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services
            .AddMvvmNavMaui()
            .RegisterCommonServices()
            .RegisterPlatformServices();


#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
