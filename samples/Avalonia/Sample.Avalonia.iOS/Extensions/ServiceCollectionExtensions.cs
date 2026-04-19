using ADaxer.MvvmNav.Sample.Avalonia.iOS.Services;
using ADaxer.MvvmNav.Sample.Common.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the iOS specific services for the sample app.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterIOSServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddTransient<IFileService,IOSFileService>();

        return services;
    }
}
