using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Sample.Maui;
using Microsoft.Extensions.Logging;
using Sample.Maui.Views;

namespace Sample.Maui;

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

        builder.Services
            .AddMvvmNav()
            .WithShell<ShellPage, ShellViewModel>()
            .WithStartupNavigation<HomeViewModel>()
            .RegisterCommonServices()
            .RegisterPlatformServices();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
