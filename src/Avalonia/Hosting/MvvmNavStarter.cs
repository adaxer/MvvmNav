using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Platform;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Avalonia.Hosting;

/// <summary>
/// Default Avalonia implementation of <see cref="IMvvmNavStarter"/>.
/// </summary>
public sealed class MvvmNavStarter : IMvvmNavStarter
{
    private readonly IServiceProvider _services;
    private readonly MvvmNavOptions _options;

    private bool _initialized;
    private bool _started;
    private Application? _application;
    private object? _shellView;
    private IShellViewModel? _shellViewModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="MvvmNavStarter"/> class.
    /// </summary>
    /// <param name="services">The application service provider.</param>
    /// <param name="options">The configured Avalonia startup options.</param>
    public MvvmNavStarter(IServiceProvider services, MvvmNavOptions options)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <inheritdoc />
    public void Initialize(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);

        if (_initialized)
            throw new InvalidOperationException("MvvmNav has already been initialized for this Avalonia application.");

        EnsureShellConfigured();

        _application = application;
        _initialized = true;

        LoadMvvmNavResources(application);

        var shellViewModelObject = _services.GetRequiredService(_options.ShellViewModelType!);
        if (shellViewModelObject is not IShellViewModel shellViewModel)
        {
            throw new InvalidOperationException(
                $"The configured shell view model '{_options.ShellViewModelType!.FullName}' must implement {nameof(IShellViewModel)}.");
        }

        if (shellViewModelObject is not IDialogHost)
        {
            throw new InvalidOperationException(
                $"The configured shell view model '{_options.ShellViewModelType!.FullName}' must implement {nameof(IDialogHost)} for dialog hosting.");
        }

        var shellViewObject = _services.GetRequiredService(_options.ShellViewType!);
        BindShell(shellViewObject, shellViewModel);

        _shellView = shellViewObject;
        _shellViewModel = shellViewModel;

        WireLifetime(application, shellViewObject);
    }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started)
            return;

        if (!_initialized || _application is null)
        {
            throw new InvalidOperationException(
                "MvvmNav has not been initialized. Call Initialize(Application) before StartAsync().");
        }

        _started = true;

        if (_options.StartupNavigationType is not null)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var navigation = _services.GetRequiredService<INavigationService>();
            await navigation.NavigateAsync(_options.StartupNavigationType);
        }
    }

    private void EnsureShellConfigured()
    {
        if (_options.ShellViewType is null || _options.ShellViewModelType is null)
        {
            throw new InvalidOperationException(
                "No shell was configured. Call WithShell<TShellView, TShellViewModel>() before starting MvvmNav.");
        }
    }

    private static void BindShell(object shellViewObject, IShellViewModel shellViewModel)
    {
        switch (shellViewObject)
        {
            case StyledElement styledElement:
                styledElement.DataContext = shellViewModel;
                break;

            default:
                throw new InvalidOperationException(
                    $"The configured shell view '{shellViewObject.GetType().FullName}' must derive from {nameof(StyledElement)} in Avalonia.");
        }
    }

    private void WireLifetime(Application application, object shellViewObject)
    {
        switch (application.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktopLifetime:
                WireDesktopLifetime(desktopLifetime, shellViewObject);
                break;

            case ISingleViewApplicationLifetime singleViewLifetime:
                WireSingleViewLifetime(singleViewLifetime, shellViewObject);
                break;

            case IActivityApplicationLifetime activityLifetime:
                WireActivityLifetime(activityLifetime, shellViewObject);
                break;

            default:
                throw new InvalidOperationException(
                    $"The current Avalonia application lifetime '{application.ApplicationLifetime?.GetType().FullName ?? "null"}' is not supported by MvvmNav.");
        }
    }

    private static void WireDesktopLifetime(IClassicDesktopStyleApplicationLifetime lifetime, object shellViewObject)
    {
        switch (shellViewObject)
        {
            case Window window:
                lifetime.MainWindow = window;
                break;

            case Control control:
                lifetime.MainWindow = new AvaloniaShellWindow(control);
                break;

            default:
                throw new InvalidOperationException(
                    $"For desktop lifetime, the configured shell view '{shellViewObject.GetType().FullName}' must be a {nameof(Window)} or {nameof(Control)}.");
        }
    }

    private static void WireSingleViewLifetime(ISingleViewApplicationLifetime lifetime, object shellViewObject)
    {
        if (shellViewObject is not Control control)
        {
            throw new InvalidOperationException(
                $"For single-view lifetime, the configured shell view '{shellViewObject.GetType().FullName}' must derive from {nameof(Control)}.");
        }

        lifetime.MainView = control;
    }

    private void WireActivityLifetime(IActivityApplicationLifetime lifetime, object shellViewObject)
    {
        if (shellViewObject is Window)
        {
            throw new InvalidOperationException(
                $"For activity lifetime, the configured shell view '{shellViewObject.GetType().FullName}' must not be a {nameof(Window)}. Use a {nameof(Control)} instead.");
        }

        if (shellViewObject is not Control control)
        {
            throw new InvalidOperationException(
                $"For activity lifetime, the configured shell view '{shellViewObject.GetType().FullName}' must derive from {nameof(Control)}.");
        }

        lifetime.MainViewFactory = () =>
        {
            var shell = _services.GetRequiredService(_options.ShellViewType!);
            var vm = (IShellViewModel)_services.GetRequiredService(_options.ShellViewModelType!);

            BindShell(shell, vm);

            return (Control)shell;
        };
    }

    private static readonly Uri ResourcesUri =
        new("avares://ADaxer.MvvmNav.Avalonia/Resources/MvvmNav.Avalonia.Resources.axaml");

    private static readonly Uri StylesUri =
        new("avares://ADaxer.MvvmNav.Avalonia/Resources/MvvmNav.Avalonia.Styles.axaml");

    private static void LoadMvvmNavResources(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);

        application.Resources ??= new ResourceDictionary();

        if (!application.Resources.MergedDictionaries
            .OfType<ResourceInclude>()
            .Any(x => x.Source == ResourcesUri))
        {
            application.Resources.MergedDictionaries.Add(new ResourceInclude(ResourcesUri)
            {
                Source = ResourcesUri
            });
        }

        if (!application.Styles
            .OfType<StyleInclude>()
            .Any(x => x.Source == StylesUri))
        {
            application.Styles.Add(new StyleInclude(StylesUri)
            {
                Source = StylesUri
            });
        }
    }
}
