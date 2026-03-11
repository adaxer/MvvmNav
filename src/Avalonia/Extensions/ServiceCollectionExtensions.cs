using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Avalonia.Navigation;
using ADaxer.MvvmNav.Core.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Avalonia;

/// <summary>
/// Provides extension methods for registering the MvvmNav services
/// for Avalonia-based applications.
/// </summary>
/// <remarks>
/// This extension registers the required navigation services and
/// Avalonia-specific host implementations with the dependency
/// injection container.
/// 
/// The following services are registered:
/// <list type="bullet">
/// <item>
/// <description>
/// <see cref="INavigationService"/> implemented by <see cref="NavigationService"/>
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="INavigationHost"/> implemented by <see cref="AvaloniaNavigationHost"/>
/// </description>
/// </item>
/// <item>
/// <description>
/// <see cref="IDialogHost"/> implemented by <see cref="AvaloniaDialogHost"/>
/// </description>
/// </item>
/// </list>
/// 
/// All services are registered with <see cref="ServiceLifetime.Singleton"/> lifetime.
/// </remarks>
/// <seealso cref="INavigationService"/>
/// <seealso cref="INavigationHost"/>
/// <seealso cref="IDialogHost"/>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the MvvmNav navigation services for Avalonia applications.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> used to register services.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that additional
    /// calls can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddMvvmNavAvalonia(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<INavigationHost, AvaloniaNavigationHost>();
        services.AddSingleton<IDialogHost, AvaloniaDialogHost>();
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }
}
