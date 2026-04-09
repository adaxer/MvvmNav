using System;
using ADaxer.MvvmNav.Sample.Avalonia.Desktop.Views;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Sample.Avalonia.Desktop;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        var services = new ServiceCollection();

        services.AddMvvmNav()
            .WithShell<ShellWindow, ShellViewModel>()
            .WithStartupNavigation<HomeViewModel>()
            .RegisterCommonServices()
            .RegisterPlatformServices();

        var serviceProvider = services.BuildServiceProvider();

        var result = AppBuilder.Configure<App>(()=>new App { ServiceProvider = serviceProvider })
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

        return result;
    }
}
