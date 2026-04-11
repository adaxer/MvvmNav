using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Maui.Hosting;

/// <summary>
/// Creates the MAUI shell window and runs optional startup navigation.
/// </summary>
/// <remarks>
/// <para>
/// Window creation resolves the configured shell view model and shell view from dependency injection,
/// validates their expected MAUI roles, binds the shell view model, and caches the created
/// <see cref="Window"/>.
/// </para>
/// <para>
/// <see cref="StartAsync"/> ensures that window creation has happened and then performs startup
/// navigation only when <see cref="StartupOptions.StartupNavigationType"/> is configured.
/// </para>
/// </remarks>
public sealed class MvvmNavStarter : IMvvmNavStarter
{
    private readonly IServiceProvider _services;
    private readonly StartupOptions _options;

    private bool _started;
    private Window? _window;
    private Page? _shellPage;
    private IShellViewModel? _shellViewModel;

    /// <summary>
    /// Initializes the starter with the application service provider and startup options.
    /// </summary>
    /// <param name="services">
    /// The application service provider.
    /// </param>
    /// <param name="options">
    /// The configured shell and startup navigation options.
    /// </param>
    public MvvmNavStarter(IServiceProvider services, StartupOptions options)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    /// <summary>
    /// Creates or returns the cached application window for the configured shell.
    /// </summary>
    /// <param name="activationState">
    /// The MAUI activation state for the window creation request.
    /// </param>
    /// <returns>
    /// The created or cached application window.
    /// </returns>
    /// <remarks>
    /// The configured shell view model must implement <see cref="IShellViewModel"/> and
    /// <see cref="IDialogHost"/>. The configured shell view must either implement
    /// <see cref="IMauiShellView"/> or derive from <see cref="Page"/>.
    /// </remarks>
    public Window CreateWindow(IActivationState? activationState)
    {
        EnsureShellConfigured();

        if (_window is not null)
            return _window;

        LoadMvvmNavResources();

        var shellViewModelObject = _services.GetRequiredService(_options.ShellViewModelType!);
        if (shellViewModelObject is not IShellViewModel shellViewModel)
        {
            throw new InvalidOperationException(
                $"The configured shell view model '{_options.ShellViewModelType!.FullName}' must implement {nameof(IShellViewModel)}.");
        }

        if (shellViewModelObject is not IDialogHost)
        {
            throw new InvalidOperationException(
                $"The configured shell view model '{_options.ShellViewModelType!.FullName}' must implement {nameof(IDialogHost)} for MAUI dialog hosting.");
        }

        var shellObject = _services.GetRequiredService(_options.ShellViewType!);
        var shellPage = ResolveShellPage(shellObject, shellViewModel);

        _shellViewModel = shellViewModel;
        _shellPage = shellPage;
        _window = new Window(shellPage);

        return _window;
    }

    /// <summary>
    /// Runs the startup workflow once and performs configured startup navigation.
    /// </summary>
    /// <param name="cancellationToken">
    /// A token observed before startup navigation begins.
    /// </param>
    /// <returns>
    /// A task representing the startup operation.
    /// </returns>
    /// <remarks>
    /// If startup has already completed, the method returns immediately.
    /// </remarks>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_started)
            return;

        _started = true;

        EnsureShellConfigured();

        _ = _window ?? CreateWindow(null);

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

    private static Page ResolveShellPage(object shellObject, IShellViewModel shellViewModel)
    {
        if (shellObject is IMauiShellView mauiShellView)
        {
            mauiShellView.BindingContext = shellViewModel;

            if (shellObject is not Page pageFromInterface)
            {
                throw new InvalidOperationException(
                    $"The configured shell view '{shellObject.GetType().FullName}' implements {nameof(IMauiShellView)} but is not a MAUI {nameof(Page)}.");
            }

            return pageFromInterface;
        }

        if (shellObject is Page page)
        {
            page.BindingContext = shellViewModel;
            return page;
        }

        throw new InvalidOperationException(
            $"The configured shell view '{shellObject.GetType().FullName}' must either implement {nameof(IMauiShellView)} or derive from {nameof(Page)}.");
    }

    private static void LoadMvvmNavResources()
    {
        if (Application.Current?.Resources is null)
            return;

        var dictionaries = Application.Current.Resources.MergedDictionaries;
        if (dictionaries.OfType<MauiResources>().Any())
            return;

        dictionaries.Add(new MauiResources());
    }
}
