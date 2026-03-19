using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common.Services;
using ADaxer.MvvmNav.Sample.Common.ViewModels;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the core services for the sample app.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterCommonServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddTransient<AboutViewModel>();
        services.AddTransient<FeaturesViewModel>();
        services.AddTransient<DetailsViewModel>();
        services.AddSingleton<FeatureService>();
        services.AddSingleton<IFileService, FileService>();

        return services;
    }
}
