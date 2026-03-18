using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Wpf;
using ADaxer.MvvmNav.Sample.Wpf.Views;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common;

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
                services.AddTransient<HomeViewModel>();
                services.AddSingleton<SettingsViewModel>();
                services.AddTransient<AboutViewModel>();
                services.AddTransient<FeaturesViewModel>();
                services.AddTransient<DetailsViewModel>();
                services.AddSingleton<IFileService, FileService>();
            })
            .Start();

        _services = host.Services;
        var navigation = _services!.GetRequiredService<INavigationService>();
        await navigation.NavigateAsync<HomeViewModel>();
    }
}
