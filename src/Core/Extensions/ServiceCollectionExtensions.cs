using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core;
using ADaxer.MvvmNav.Core.Navigation;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the core MvvmNav services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the platform-agnostic MvvmNav core services.
    /// </summary>
    public static IServiceCollection AddMvvmNavCore(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(typeof(IFactory<>), typeof(GenericFactory<>));
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }
}
