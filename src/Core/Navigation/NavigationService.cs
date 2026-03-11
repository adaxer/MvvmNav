using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationService"/>.
/// </summary>
/// <remarks>
/// The navigation service resolves target view models using the provided
/// <see cref="IServiceProvider"/> and delegates the actual presentation
/// to the configured <see cref="INavigationHost"/> or <see cref="IDialogHost"/>.
/// 
/// A simple in-memory stack is used to support back navigation.
/// </remarks>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly INavigationHost _host;
    private readonly IDialogHost _dialogHost;

    private readonly Stack<object> _backStack = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="services">
    /// The service provider used to resolve navigation targets and dialogs.
    /// </param>
    /// <param name="host">
    /// The navigation host responsible for presenting navigation targets.
    /// </param>
    /// <param name="dialogHost">
    /// The dialog host responsible for presenting dialogs.
    /// </param>
    public NavigationService(
        IServiceProvider services,
        INavigationHost host,
        IDialogHost dialogHost)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(dialogHost);
        
        _services = services;
        _host = host;
        _dialogHost = dialogHost;
    }

    /// <inheritdoc/>
    public bool CanGoBack()
    {
        return _backStack.Count > 0;
    }

    /// <inheritdoc/>
    public async Task NavigateAsync<TTarget>(
        NavigationContext? context = null,
        NavigationOptions? options = null)
        where TTarget : class
    {
        await NavigateAsync(typeof(TTarget), context, options);
    }

    /// <inheritdoc/>
    public async Task NavigateAsync(
        Type targetType,
        NavigationContext? context = null,
        NavigationOptions? options = null)
    {
        context ??= NavigationContext.Empty;
        options ??= NavigationOptions.Default;

        var target = _services.GetRequiredService(targetType);

        if (options.ClearBackStack)
            _backStack.Clear();

        if (options.AddToBackStack && _host.Current != null)
            _backStack.Push(_host.Current);

        await _host.ShowAsync(target, options);

        if (target is INavigationAware aware)
            await aware.OnNavigatedToAsync(context);
    }

    /// <inheritdoc/>
    public async Task GoBackAsync()
    {
        if (!CanGoBack())
            return;

        var target = _backStack.Pop();

        await _host.ShowAsync(target);

        if (target is INavigationAware aware)
            await aware.OnNavigatedToAsync(NavigationContext.Empty);
    }

    /// <inheritdoc/>
    public Task<DialogResult> ShowDialogAsync<TDialog>(
        NavigationContext? context = null)
        where TDialog : class
    {
        return ShowDialogAsync(typeof(TDialog), context);
    }

    /// <inheritdoc/>
    public async Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationContext? context = null)
        where TDialog : class
    {
        var dialog = _services.GetRequiredService<TDialog>();

        var result = await _dialogHost.ShowDialogAsync<TResult>(
            dialog!,
            context ?? NavigationContext.Empty);

        return result;
    }

    /// <summary>
    /// Shows a dialog using a runtime type.
    /// </summary>
    /// <param name="dialogType">
    /// The dialog type to resolve from the service provider.
    /// </param>
    /// <param name="context">
    /// Optional dialog parameters.
    /// </param>
    /// <returns>
    /// A <see cref="DialogResult"/> describing the dialog outcome.
    /// </returns>
    private async Task<DialogResult> ShowDialogAsync(
        Type dialogType,
        NavigationContext? context)
    {
        var dialog = _services.GetRequiredService(dialogType);

        return await _dialogHost.ShowDialogAsync(
            dialog!,
            context ?? NavigationContext.Empty);
    }
}
