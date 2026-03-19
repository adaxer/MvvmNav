using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Wpf;
using ADaxer.MvvmNav.Sample.Wpf.Views;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Sample.Wpf;

public partial class App : Application
{
    private IServiceProvider? _services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var host = WpfNavigationHostBuilder<ShellWindow, ShellViewModel>
            .BuildDefault()
            .WithServices(services =>
            {
                services.RegisterCommonServices();
            })
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
            .Start();

        _services = host.Services;
        var navigation = _services!.GetRequiredService<INavigationService>();
        await navigation.NavigateAsync<HomeViewModel>();
    }
}
