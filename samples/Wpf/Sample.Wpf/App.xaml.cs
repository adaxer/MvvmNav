using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Sample.Wpf.Views;
using Microsoft.Extensions.Logging;
using ADaxer.MvvmNav.Wpf.Hosting;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace ADaxer.MvvmNav.Sample.Wpf;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var host = WpfNavigationHostBuilder
            .Default()
            .WithServices(services =>
            {
                services.RegisterCommonServices();
                services.AddSingleton<IPlatformNameProvider, WpfPlatformNameProvider>();
            })
            .WithDialogMode(DialogMode.Overlay) // Set this to DialogMode.Window to have dialogs open in separate windows instead of as overlays on top of the current view
            .WithLogging(logging =>
            {
                // An example how to configure logging via code
                // To configure via appsettings.json, the wpf app would need its own host to have appsettings. 
                // It could then initialize services and add the MvvmNav goodness via the UseMvvmNav extension method,
                // which would then read the logging configuration from appsettings and apply it to the MvvmNav logging configuration.
                logging.AddFilter((category, level) =>
                {
                    if (category == typeof(DetailsViewModel).FullName)
                        return level >= LogLevel.Debug;

                    return level >= LogLevel.Information;
                });
            })
            .WithShell<ShellWindow, ShellViewModel>()
            .WithStartupNavigation<HomeViewModel>()
            .Build();

        await host.StartAsync();
    }
}
