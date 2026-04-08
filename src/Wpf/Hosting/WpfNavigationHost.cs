using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Windows;

namespace ADaxer.MvvmNav.Wpf.Hosting;

/// <summary>
/// Represents a WPF navigation host.
/// </summary>
public sealed class WpfNavigationHost
{
    private static readonly Uri ResourcesUri = new(
        "pack://application:,,,/ADaxer.MvvmNav.Wpf;component/MvvmNav.Wpf.Resources.xaml",
        UriKind.Absolute);

    private readonly Func<IServiceProvider> _serviceProviderFactory;
    private readonly Type? _shellViewType;
    private readonly Type? _shellViewModelType;
    private readonly Type? _startupNavigationType;
    private int _started;

    internal WpfNavigationHost(
        Func<IServiceProvider> serviceProviderFactory,
        Type? shellViewType,
        Type? shellViewModelType,
        Type? startupNavigationType)
    {
        ArgumentNullException.ThrowIfNull(serviceProviderFactory);

        _serviceProviderFactory = serviceProviderFactory;
        _shellViewType = shellViewType;
        _shellViewModelType = shellViewModelType;
        _startupNavigationType = startupNavigationType;
    }

    /// <summary>
    /// Gets the service provider used by the host.
    /// </summary>
    public IServiceProvider? Services { get; private set; }

    /// <summary>
    /// Gets the resolved shell view.
    /// </summary>
    public Window? Shell { get; private set; }

    /// <summary>
    /// Gets the resolved shell view model.
    /// </summary>
    public IModuleHost? ShellViewModel { get; private set; }

    /// <summary>
    /// Gets the dialog hosting options used by the host.
    /// </summary>
    public WpfDialogOptions? DialogOptions { get; private set; }

    /// <summary>
    /// Builds the service provider, shows the shell, and performs startup navigation.
    /// </summary>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            throw new InvalidOperationException("The WPF navigation host has already been started.");
        }

        if (_shellViewType is null || _shellViewModelType is null)
        {
            throw new InvalidOperationException(
                $"Configure the shell using {nameof(WpfNavigationHostBuilder.WithShell)} before starting the WPF navigation host.");
        }

        var services = _serviceProviderFactory();
        Services = services;

        LoadMvvmNavResources();

        var shell = services.GetRequiredService(_shellViewType) as Window
            ?? throw new InvalidOperationException($"The configured shell '{_shellViewType.FullName}' must be a WPF window.");

        var shellViewModel = services.GetRequiredService(_shellViewModelType) as IModuleHost
            ?? throw new InvalidOperationException($"The configured shell view model '{_shellViewModelType.FullName}' must implement {nameof(IModuleHost)}.");

        DialogOptions = services.GetRequiredService<WpfDialogOptions>();
        Shell = shell;
        ShellViewModel = shellViewModel;

        shell.DataContext = shellViewModel;
        Application.Current.MainWindow = shell;

        if (_startupNavigationType is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var navigation = services.GetRequiredService<INavigationService>();
            await navigation.NavigateAsync(_startupNavigationType);
        }

        shell.Show();
    }

    internal static void LoadMvvmNavResources()
    {
        var resources = Application.Current.Resources.MergedDictionaries;

        if (resources.Any(resource => resource.Source == ResourcesUri))
        {
            return;
        }

        resources.Add(new ResourceDictionary { Source = ResourcesUri });
    }
}
