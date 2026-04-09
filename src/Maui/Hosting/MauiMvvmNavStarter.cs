using ADaxer.MvvmNav.Abstractions.Dialogs;
using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Maui.Hosting;

public sealed class MauiMvvmNavStarter : IMauiMvvmNavStarter
{
    private readonly IServiceProvider _services;
    private readonly MauiMvvmNavOptions _options;

    private bool _started;
    private Window? _window;
    private Page? _shellPage;
    private IShellViewModel? _shellViewModel;

    public MauiMvvmNavStarter(IServiceProvider services, MauiMvvmNavOptions options)
    {
        _services = services ?? throw new ArgumentNullException(nameof(services));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

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
