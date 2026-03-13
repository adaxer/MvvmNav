using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Wpf;
using ADaxer.MvvmNav.Sample.Wpf.Views;

namespace ADaxer.MvvmNav.Sample.Wpf;

public partial class App : Application
{
    private IServiceProvider? _services;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        (_services, _, _) = Bootstrapper.BuildAndStart<ShellWindow, ShellViewModel>(services =>
        {
            services.AddTransient<HomeViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<AboutViewModel>();
        });

        var navigation = _services.GetRequiredService<INavigationService>();
        await navigation.NavigateAsync<HomeViewModel>();
    }
}
