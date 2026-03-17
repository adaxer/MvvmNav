using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Wpf;

/// <summary>
/// Builds and starts WPF navigation hosts that use MvvmNav.
/// </summary>
public sealed class WpfNavigationHostBuilder<TShellView, TShellViewModel>
    where TShellView : class, IShellView
    where TShellViewModel : class, IShellViewModel
{
    private readonly List<Action<IServiceCollection>> _serviceConfigurations = [];
    private readonly List<Action<ILoggingBuilder>> _loggingConfigurations = [];

    private bool _useDefaultLogging;

    private WpfNavigationHostBuilder()
    {
    }

    /// <summary>
    /// Creates a builder with no default logging providers.
    /// </summary>
    public static WpfNavigationHostBuilder<TShellView, TShellViewModel> Build()
    {
        return new WpfNavigationHostBuilder<TShellView, TShellViewModel>();
    }

    /// <summary>
    /// Creates a builder with default logging configured.
    /// </summary>
    public static WpfNavigationHostBuilder<TShellView, TShellViewModel> BuildDefault()
    {
        return new WpfNavigationHostBuilder<TShellView, TShellViewModel>
        {
            _useDefaultLogging = true
        };
    }

    /// <summary>
    /// Adds logging configuration to the host builder.
    /// </summary>
    public WpfNavigationHostBuilder<TShellView, TShellViewModel> WithLogging(
        Action<ILoggingBuilder> configureLogging)
    {
        ArgumentNullException.ThrowIfNull(configureLogging);

        _loggingConfigurations.Add(configureLogging);
        return this;
    }

    /// <summary>
    /// Adds service registrations to the host builder.
    /// </summary>
    public WpfNavigationHostBuilder<TShellView, TShellViewModel> WithServices(
        Action<IServiceCollection> configureServices)
    {
        ArgumentNullException.ThrowIfNull(configureServices);

        _serviceConfigurations.Add(configureServices);
        return this;
    }

    /// <summary>
    /// Builds the service provider.
    /// </summary>
    public IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddMvvmNavWpf();

        services.AddSingleton<TShellView>();
        services.AddSingleton<IShellView>(sp => sp.GetRequiredService<TShellView>());

        services.AddSingleton<TShellViewModel>();
        services.AddSingleton<IShellViewModel>(sp => sp.GetRequiredService<TShellViewModel>());

        if (_useDefaultLogging || _loggingConfigurations.Count > 0)
        {
            services.AddLogging(logging =>
            {
                if (_useDefaultLogging)
                {
                    logging.AddDebug();
                    logging.SetMinimumLevel(LogLevel.Debug);
                }

                foreach (var configureLogging in _loggingConfigurations)
                {
                    configureLogging(logging);
                }
            });
        }

        foreach (var configureServices in _serviceConfigurations)
        {
            configureServices(services);
        }

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Builds the service provider, resolves the shell and starts the host.
    /// </summary>
    public WpfNavigationHost<TShellView, TShellViewModel> Start()
    {
        var services = BuildServiceProvider();

        var shell = services.GetRequiredService<TShellView>();
        var shellViewModel = services.GetRequiredService<TShellViewModel>();

        shell.DataContext = shellViewModel;
        shell.Show();

        return new WpfNavigationHost<TShellView, TShellViewModel>(
            services,
            shell,
            shellViewModel);
    }
}
