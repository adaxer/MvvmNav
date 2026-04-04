using System.Windows;
using ADaxer.MvvmNav.Abstractions.Navigation;
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
    public static IServiceCollection AddMvvmNavWpf(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddMvvmNavCore();
        services.AddSingleton<IDialogService, WpfDialogService>();
        Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/MvvmNav.Wpf;component/MvvmNav.Wpf.Resources.xaml",
            UriKind.Absolute) });

        return services;
    }
}
