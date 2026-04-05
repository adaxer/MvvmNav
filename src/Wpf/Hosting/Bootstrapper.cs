using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Wpf.Hosting;

/// <summary>
/// Provides helper methods for bootstrapping WPF applications that use MvvmNav.
/// </summary>
public static class Bootstrapper
{
    /// <summary>
    /// Builds a service provider for a WPF application that uses MvvmNav.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The shell view model type.
    /// </typeparam>
    /// <param name="configureServices">
    /// Optional application-specific service registrations.
    /// </param>
    public static IServiceProvider Build<TShellView, TShellViewModel>(
        Action<IServiceCollection>? configureServices = null)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        var builder = WpfNavigationHostBuilder<TShellView, TShellViewModel>
            .BuildDefault();

        if (configureServices is not null)
        {
            builder.WithServices(configureServices);
        }

        return builder.BuildServiceProvider();
    }


    /// <summary>
    /// Resolves and displays the application shell.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The shell view model type.
    /// </typeparam>
    /// <param name="services">
    /// The service provider used to resolve the shell.
    /// </param>
    public static (TShellView Shell, TShellViewModel ShellViewModel) Start<TShellView, TShellViewModel>(IServiceProvider services)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        ArgumentNullException.ThrowIfNull(services);

        var shell = services.GetRequiredService<TShellView>();
        var shellViewModel = services.GetRequiredService<TShellViewModel>();

        shell.DataContext = shellViewModel;
        shell.Show();

        return (shell, shellViewModel);
    }

    /// <summary>
    /// Builds the service provider and immediately starts the application shell.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The shell view type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The shell view model type.
    /// </typeparam>
    /// <param name="configureServices">
    /// Optional application-specific service registrations.
    /// </param>
    public static (IServiceProvider Services, TShellView Shell, TShellViewModel ShellViewModel) BuildAndStart<TShellView, TShellViewModel>(
           Action<IServiceCollection>? configureServices = null)
           where TShellView : class, IShellView
           where TShellViewModel : class, IShellViewModel
    {
        var builder = WpfNavigationHostBuilder<TShellView, TShellViewModel>
            .BuildDefault();

        if (configureServices is not null)
        {
            builder.WithServices(configureServices);
        }

        var services = builder.BuildServiceProvider();
        var (shell, shellViewModel) = Start<TShellView, TShellViewModel>(services);

        return (services, shell, shellViewModel);
    }
}
