using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Wpf;

/// <summary>
/// Provides helper methods for bootstrapping WPF applications that use MvvmNav.
/// </summary>
public static class Bootstrapper
{
    /// <summary>
    /// Builds a service provider for a WPF application that uses MvvmNav.
    /// </summary>
    public static IServiceProvider Build<TShellView, TShellViewModel>(
        Action<IServiceCollection>? configureServices = null)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        var services = new ServiceCollection();

        services.AddMvvmNavWpf();

        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        services.AddSingleton<TShellView>();
        services.AddSingleton<IShellView>(sp => sp.GetRequiredService<TShellView>());

        services.AddSingleton<TShellViewModel>();
        services.AddSingleton<IShellViewModel>(sp => sp.GetRequiredService<TShellViewModel>());

        configureServices?.Invoke(services);

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Resolves and displays the application shell.
    /// </summary>
    public static (TShellView, TShellViewModel) Start<TShellView, TShellViewModel>(IServiceProvider services)
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
    public static (IServiceProvider, TShellView, TShellViewModel) BuildAndStart<TShellView, TShellViewModel>(
        Action<IServiceCollection>? configureServices = null)
        where TShellView : class, IShellView
        where TShellViewModel : class, IShellViewModel
    {
        var services = Build<TShellView, TShellViewModel>(configureServices);
        var (shell, shellViewModel) = Start<TShellView, TShellViewModel>(services);

        return (services, shell, shellViewModel);
    }
}
