using ADaxer.MvvmNav.Abstractions.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ADaxer.MvvmNav.Core.Navigation;

/// <summary>
/// Default implementation of <see cref="INavigationService"/>.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _services;
    private readonly IDialogService _dialogService;
    private readonly ILogger<NavigationService> _logger;
    private readonly Stack<object> _backStack = new();

    private IShellViewModel? _shell;


    public NavigationService(
        IServiceProvider services,
        IDialogService dialogService,
        ILogger<NavigationService> logger)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(dialogService);
        ArgumentNullException.ThrowIfNull(logger);

        _services = services;
        _dialogService = dialogService;
        _logger = logger;
    }

    public event EventHandler? NavigationStateChanged;

    private IShellViewModel Shell =>
        _shell ??= _services.GetRequiredService<IShellViewModel>();

    public bool CanGoBack() => _backStack.Count > 0;

    public Task NavigateAsync<TTarget>(
        NavigationParameters? context = null,
        NavigationOptions? options = null)
        where TTarget : class
        => NavigateAsync(typeof(TTarget), context, options);

    public async Task NavigateAsync(
        Type targetType,
        NavigationParameters? context = null,
        NavigationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        context ??= NavigationParameters.Empty;
        options ??= NavigationOptions.Default;

        _logger.LogDebug(
            "Navigation requested. Target={TargetType}, ClearBackStack={ClearBackStack}, AddToBackStack={AddToBackStack}",
            targetType.FullName,
            options.ClearBackStack,
            options.AddToBackStack);

        var request = new NavigationRequest
        {
            TargetType = targetType,
            Parameters = context,
            IsBackNavigation = false
        };

        var canLeave = await CanLeaveCurrentAsync(request);

        if (canLeave.IsConfirmed.HasValue == false)
        {
            _logger.LogInformation(
                "Navigation cancelled by guard. Target={TargetType}",
                targetType.FullName);

            return;
        }

        var target = _services.GetRequiredService(targetType);

        if (options.ClearBackStack)
        {
            _logger.LogDebug("Clearing back stack before navigation.");
            _backStack.Clear();
        }

        if (options.AddToBackStack && Shell.CurrentModule is not null)
        {
            _logger.LogDebug(
                "Pushing current module onto back stack. CurrentType={CurrentType}",
                Shell.CurrentModule.GetType().FullName);

            _backStack.Push(Shell.CurrentModule);
        }

        await ActivateAsync(target, context);

        _logger.LogInformation(
            "Navigation completed. Target={TargetType}",
            targetType.FullName);
    }

    public async Task GoBackAsync()
    {
        if (!CanGoBack())
        {
            _logger.LogDebug("GoBack requested, but back stack is empty.");
            return;
        }

        _logger.LogDebug("Back navigation requested.");

        var request = new NavigationRequest
        {
            TargetType = _backStack.Peek().GetType(),
            Parameters = NavigationParameters.Empty,
            IsBackNavigation = true
        };

        var canLeave = await CanLeaveCurrentAsync(request);
        if (canLeave.IsConfirmed.HasValue == false)
        {
            _logger.LogInformation("Back navigation cancelled by guard.");
            return;
        }

        var target = _backStack.Pop();

        _logger.LogDebug(
            "Back navigation target resolved. TargetType={TargetType}",
            target.GetType().FullName);

        await ActivateAsync(target, NavigationParameters.Empty);

        _logger.LogInformation(
            "Back navigation completed. TargetType={TargetType}",
            target.GetType().FullName);
    }

    public Task<DialogResult> ShowDialogAsync<TDialog>(NavigationParameters? context = null)
        where TDialog : class
        => ShowDialogAsync(typeof(TDialog), context);

    public async Task<DialogResult<TResult>> ShowDialogAsync<TDialog, TResult>(
        NavigationParameters? context = null)
        where TDialog : class
    {
        _logger.LogDebug(
            "Dialog requested. DialogType={DialogType}",
            typeof(TDialog).FullName);

        var dialog = _services.GetRequiredService<TDialog>();

        if (dialog is not IDialogAware dialogAware)
        {
            _logger.LogError(
                "Resolved dialog does not implement IDialogAware. DialogType={DialogType}",
                typeof(TDialog).FullName);

            throw new InvalidOperationException(
                $"Dialog type '{typeof(TDialog).FullName}' must implement IDialogAware.");
        }

        var result = await _dialogService.ShowDialogAsync<TResult>(
            dialogAware,
            context ?? NavigationParameters.Empty);

        _logger.LogInformation(
            "Dialog completed. DialogType={DialogType}, Confirmed={Confirmed}",
            typeof(TDialog).FullName,
            result.IsConfirmed);

        return result;
    }

    private async Task<DialogResult> ShowDialogAsync(Type dialogType, NavigationParameters? context)
    {
        _logger.LogDebug(
            "Dialog requested. DialogType={DialogType}",
            dialogType.FullName);

        var dialog = _services.GetRequiredService(dialogType);

        if (dialog is not IDialogAware dialogAware)
        {
            _logger.LogError(
                "Resolved dialog does not implement IDialogAware. DialogType={DialogType}",
                dialogType.FullName);

            throw new InvalidOperationException(
                $"Dialog type '{dialogType.FullName}' must implement IDialogAware.");
        }

        var result = await _dialogService.ShowDialogAsync(
            dialogAware,
            context ?? NavigationParameters.Empty);

        _logger.LogInformation(
            "Dialog completed. DialogType={DialogType}, Confirmed={Confirmed}",
            dialogType.FullName,
            result.IsConfirmed);

        return result;
    }

    private async Task ActivateAsync(object target, NavigationParameters context)
    {
        _logger.LogDebug(
            "Activating target. TargetType={TargetType}",
            target.GetType().FullName);

        Shell.CurrentModule = target;

        if (target is INavigationAware aware)
        {
            await aware.OnNavigatedToAsync(context);
        }

        NavigationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task<DialogResult> CanLeaveCurrentAsync(NavigationRequest request)
    {
        if (Shell.CurrentModule is not ICanNavigateFrom guarded)
        {
            _logger.LogDebug("Current module has no navigation guard.");
            return DialogResult.True;
        }

        if (Shell.CurrentModule.GetType().Equals(request.TargetType))
        {
            _logger.LogDebug("Cannot navigate to the same type.");
            return DialogResult.None;
        }

        _logger.LogDebug(
            "Evaluating navigation guard for current module. CurrentType={CurrentType}, IsBackNavigation={IsBackNavigation}",
            Shell.CurrentModule.GetType().FullName,
            request.IsBackNavigation);

        var result = await guarded.CanNavigateFromAsync(request);

        _logger.LogDebug(
            "Navigation guard returned decision {Decision}.",
            result.Decision);

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
        if (result.Context is null)
        {
            _logger.LogError(
                "Navigation guard requested AskUser, but context was null.");

            throw new ArgumentNullException(
                nameof(result.Context),
                "Context can not be null, it is needed to show the ask user dialog");
        }

        _logger.LogDebug("Showing navigation confirmation dialog.");

        var confirmation = await _dialogService.ConfirmAsync(result.Context);

        await result.ContinueAsync(confirmation, CancellationToken.None);

        _logger.LogInformation(
            "Navigation confirmation completed. Confirmed={Confirmed}",
            confirmation.IsConfirmed);

        return confirmation;
    }
}
