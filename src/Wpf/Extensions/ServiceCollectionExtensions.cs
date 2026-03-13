using ADaxer.MvvmNav.Abstractions;
using ADaxer.MvvmNav.Abstractions.Navigation;
using ADaxer.MvvmNav.Core;
using ADaxer.MvvmNav.Core.Navigation;
using ADaxer.MvvmNav.Wpf.Navigation;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for registering the MvvmNav services
/// required for WPF applications.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the MvvmNav navigation services for WPF applications.
    /// </summary>
    /// <param name="services">
    /// The service collection used to register services.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that additional
    /// calls can be chained.
    /// </returns>
    /// <remarks>
    /// This method registers:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <see cref="INavigationService"/> implemented by <see cref="NavigationService"/>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="INavigationHost"/> implemented by <see cref="ShellNavigationHost"/>
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <see cref="IDialogHost"/> implemented by <see cref="DialogHost"/>
    /// </description>
    /// </item>
    /// </list>
    /// 
    /// All services are registered with <see cref="ServiceLifetime.Singleton"/>.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="services"/> is <c>null</c>.
    /// </exception>
    public static IServiceCollection AddMvvmNavWpf(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(typeof(IFactory<>), typeof(GenericFactory<>));
        services.AddSingleton<IDialogService, WpfDialogService>();
        services.AddSingleton<INavigationService, NavigationService>();

        return services;
    }
}
