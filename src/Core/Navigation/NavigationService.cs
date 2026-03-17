using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationService"/>.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly IDialogService _dialogService;
    private readonly Stack<object> _backStack = new();
    private IShellViewModel _shell;

    public NavigationService(
        IServiceProvider services,
        IDialogService dialogService)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(dialogService);

        _services = services;
        _dialogService = dialogService;
    }

    IShellViewModel Shell => _shell??= _services.GetRequiredService<IShellViewModel>();

    ///// <inheritdoc/>
    //public object? Current { get; private set; }

    ///// <inheritdoc/>
    //public event EventHandler<object?>? CurrentChanged;

    /// <inheritdoc/>
    public bool CanGoBack() => _backStack.Count > 0;

    /// <inheritdoc/>
    public Task NavigateAsync<TTarget>(
        NavigationParameters? context = null,
        NavigationOptions? options = null)
        where TTarget : class
        => NavigateAsync(typeof(TTarget), context, options);

    /// <inheritdoc/>
    public async Task NavigateAsync(
        Type targetType,
        NavigationParameters? context = null,
        NavigationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        context ??= NavigationParameters.Empty;
        options ??= NavigationOptions.Default;

        var request = new NavigationRequest
        {
            TargetType = targetType,
            Parameters = context,
            IsBackNavigation = false
        };

        var canLeave = await CanLeaveCurrentAsync(request);
        if (canLeave.IsConfirmed.HasValue == false)
        {
            return;
        }

        var target = _services.GetRequiredService(targetType);

        if (options.ClearBackStack)
            _backStack.Clear();

        if (options.AddToBackStack && Shell.CurrentModule is not null)
            _backStack.Push(Shell.CurrentModule);

        await ActivateAsync(target, context);
    }

    /// <inheritdoc/>
    public async Task GoBackAsync()
    {
        if (!CanGoBack())
            return;

        var request = new NavigationRequest
        {
            TargetType = null,
            Parameters = NavigationParameters.Empty,
            IsBackNavigation = true
        };

        if ((await CanLeaveCurrentAsync(request)) != DialogResult.True)
            return;

        var target = _backStack.Pop();
        await ActivateAsync(target, NavigationParameters.Empty);
    }

    /// <inheritdoc/>
    public Task<DialogResult> ShowDialogAsync<TDialog>(
        NavigationParameters? context = null)
        where TDialog : class
        => ShowDialogAsync(typeof(TDialog), context);

    /// <inheritdoc/>
    public async Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationParameters? context = null)
        where TDialog : class
    {
        var dialog = _services.GetRequiredService<TDialog>();

        if (dialog is not IDialogAware dialogAware)
            throw new InvalidOperationException(
                $"Dialog type '{typeof(TDialog).FullName}' must implement IDialogAware.");

        return await _dialogService.ShowDialogAsync<TResult>(
            dialogAware,
            context ?? NavigationParameters.Empty);
    }

    private async Task<DialogResult> ShowDialogAsync(
        Type dialogType,
        NavigationParameters? context)
    {
        var dialog = _services.GetRequiredService(dialogType);

        if (dialog is not IDialogAware dialogAware)
            throw new InvalidOperationException(
                $"Dialog type '{dialogType.FullName}' must implement IDialogAware.");

        return await _dialogService.ShowDialogAsync(
            dialogAware,
            context ?? NavigationParameters.Empty);
    }

    private async Task ActivateAsync(
        object target,
        NavigationParameters context)
    {
        Shell.CurrentModule = target;

        if (target is INavigationAware aware)
            await aware.OnNavigatedToAsync(context);
    }

    private async Task<DialogResult> CanLeaveCurrentAsync(NavigationRequest request)
    {
        if (Shell.CurrentModule is not ICanNavigateFrom guarded)
            return DialogResult.True;

        var result = await guarded.CanNavigateFromAsync(request);

        return result.Decision switch
        {
            NavigationGuardDecision.Allow => DialogResult.True,
            NavigationGuardDecision.Disallow => DialogResult.False,
            NavigationGuardDecision.AskUser => await ConfirmNavigationAsync(result),
            _ => DialogResult.None
        };
    }

    private async Task<DialogResult> ConfirmNavigationAsync(NavigationGuardResult result)
    {
        if(result.Context is null)
        {
            throw new ArgumentNullException(nameof(result.Context), "Context can not be null, it is needed to show the ask user dialog");
        }
        var confirmation = await _dialogService.ConfirmAsync(result.Context);
        await result.ContinueAsync(confirmation, CancellationToken.None);
        return confirmation;
    }
}
