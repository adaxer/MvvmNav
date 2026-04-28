using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common.ViewModels;
using ADaxer.MvvmNav.Sample.Maui;
using Sample.Maui.Services;
using Sample.Maui.Views;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the core services for the sample app.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterPlatformServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<IShellViewModel>(sp => sp.GetRequiredService<ShellViewModel>());
        services.AddSingleton<IDialogHost>(sp => sp.GetRequiredService<ShellViewModel>());

        services.AddSingleton<ShellPage>();
        services.AddSingleton<IShellView>(sp => sp.GetRequiredService<ShellPage>());

        services.RegisterView<SettingsViewModel, SettingsView>();
        services.RegisterView<HomeViewModel, HomeView>();
        services.RegisterView<AboutViewModel, AboutView>();
        services.RegisterView<SettingsViewModel, SettingsView>();
        services.RegisterView<FeaturesViewModel, FeaturesView>();
        services.RegisterView<DetailsViewModel, DetailsView>();

        services.AddTransient<IFileService, MauiFileService>();
        services.AddTransient<IPlatformNameProvider, MauiPlatformNameProvider>();

        return services;
    }
}
