using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Wpf.Hosting;

/// <summary>
/// Builds WPF navigation hosts that use MvvmNav.
/// </summary>
public sealed class WpfNavigationHostBuilder
{
    private readonly List<Action<IServiceCollection>> _serviceConfigurations = [];
    private readonly List<Action<ILoggingBuilder>> _loggingConfigurations = [];

    private bool _useDefaultLogging;
    private DialogMode _dialogMode = DialogMode.Overlay;
    private Type? _shellViewType;
    private Type? _shellViewModelType;
    private Type? _startupNavigationType;
    private bool _overlayDialogModeConfigured;

    private WpfNavigationHostBuilder()
    {
    }

    /// <summary>
    /// Creates a builder with default debug logging configured.
    /// </summary>
    public static WpfNavigationHostBuilder Default()
    {
        return new WpfNavigationHostBuilder
        {
            _useDefaultLogging = true
        };
    }

    /// <summary>
    /// Configures the shell window and shell view model.
    /// </summary>
    /// <typeparam name="TShellView">
    /// The shell window type.
    /// </typeparam>
    /// <typeparam name="TShellViewModel">
    /// The shell view model type.
    /// </typeparam>
    public WpfNavigationHostBuilder WithShell<TShellView, TShellViewModel>()
        where TShellView : class
        where TShellViewModel : class, IModuleHost
    {
        _shellViewType = typeof(TShellView);
        _shellViewModelType = typeof(TShellViewModel);

        EnsureOverlayDialogHostSupported();

        return this;
    }

    /// <summary>
    /// Configures the view model that should be navigated to after the shell is shown.
    /// </summary>
    /// <typeparam name="TViewModel">
    /// The startup navigation target view model type.
    /// </typeparam>
    public WpfNavigationHostBuilder WithStartupNavigation<TViewModel>()
        where TViewModel : class
    {
        _startupNavigationType = typeof(TViewModel);
        return this;
    }

    /// <summary>
    /// Configures how dialogs are hosted in the WPF shell.
    /// </summary>
    /// <param name="dialogMode">
    /// The dialog hosting mode.
    /// </param>
    /// <returns>
    /// The current builder instance.
    /// </returns>
    public WpfNavigationHostBuilder WithDialogMode(DialogMode dialogMode)
    {
        _dialogMode = dialogMode;
        _overlayDialogModeConfigured = dialogMode == DialogMode.Overlay;

        if (dialogMode == DialogMode.Overlay)
        {
            EnsureOverlayDialogHostSupported();
        }

        return this;
    }

    /// <summary>
    /// Adds logging configuration to the host builder.
    /// </summary>
    /// <param name="configureLogging">
    /// The logging configuration callback.
    /// </param>
    public WpfNavigationHostBuilder WithLogging(
        Action<ILoggingBuilder> configureLogging)
    {
        ArgumentNullException.ThrowIfNull(configureLogging);

        _loggingConfigurations.Add(configureLogging);
        return this;
    }

    /// <summary>
    /// Adds service registrations to the host builder.
    /// </summary>
    /// <param name="configureServices">
    /// The service registration callback.
    /// </param>
    public WpfNavigationHostBuilder WithServices(
        Action<IServiceCollection> configureServices)
    {
        ArgumentNullException.ThrowIfNull(configureServices);

        _serviceConfigurations.Add(configureServices);
        return this;
    }

    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddMvvmNav();

        services.AddSingleton(new WpfDialogOptions
        {
            DialogMode = _dialogMode
        });

        if (_shellViewType is Type shellViewType)
        {
            services.AddSingleton(shellViewType);

            if (typeof(IShellView).IsAssignableFrom(shellViewType))
            {
                services.AddSingleton(typeof(IShellView), sp => sp.GetRequiredService(shellViewType));
            }

            if (typeof(IWpfShellView).IsAssignableFrom(shellViewType))
            {
                services.AddSingleton(typeof(IWpfShellView), sp => sp.GetRequiredService(shellViewType));
            }
        }

        if (_shellViewModelType is Type shellViewModelType)
        {
            services.AddSingleton(shellViewModelType);
            services.AddSingleton(typeof(IModuleHost), sp => sp.GetRequiredService(shellViewModelType));

            if (typeof(IShellViewModel).IsAssignableFrom(shellViewModelType))
            {
                services.AddSingleton(typeof(IShellViewModel), sp => sp.GetRequiredService(shellViewModelType));
            }

            if (typeof(IDialogHost).IsAssignableFrom(shellViewModelType))
            {
                services.AddSingleton(typeof(IDialogHost), sp => sp.GetRequiredService(shellViewModelType));
            }
        }

        if (_useDefaultLogging || _loggingConfigurations.Count > 0)
        {
            services.AddLogging(logging =>
            {
                if (_useDefaultLogging)
                {
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
    /// Builds the WPF navigation host.
    /// </summary>
    public WpfNavigationHost Build()
    {
        return new WpfNavigationHost(
            BuildServiceProvider,
            _shellViewType,
            _shellViewModelType,
            _startupNavigationType);
    }

    private void EnsureOverlayDialogHostSupported()
    {
        if (!_overlayDialogModeConfigured || _shellViewModelType is null)
        {
            return;
        }

        if (!typeof(IDialogHost).IsAssignableFrom(_shellViewModelType))
        {
            throw new InvalidOperationException(
                $"Overlay dialogs require the configured shell view model '{_shellViewModelType.FullName}' to implement {nameof(IDialogHost)}.");
        }
    }
}
