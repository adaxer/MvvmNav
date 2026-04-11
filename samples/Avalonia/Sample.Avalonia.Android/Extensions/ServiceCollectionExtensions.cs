using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Sample.Avalonia.Android.Services;
using ADaxer.MvvmNav.Sample.Common.Interfaces;
using ADaxer.MvvmNav.Sample.Common.ViewModels;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the Android specific services for the sample app.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAndroidServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IFileService,AndroidFileService>();

        return services;
    }
}
