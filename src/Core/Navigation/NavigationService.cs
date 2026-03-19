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
    private readonly Stack<NavigationEntry> _backStack = new();

    private IShellViewModel? _shell;
    private NavigationEntry? _currentEntry;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="services">The application service provider.</param>
    /// <param name="dialogService">The dialog service.</param>
    /// <param name="logger">The logger instance.</param>
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

    /// <inheritdoc />
    public event EventHandler? NavigationStateChanged;

    private IShellViewModel Shell =>
        _shell ??= _services.GetRequiredService<IShellViewModel>();

    /// <inheritdoc />
    public bool CanGoBack() => _backStack.Count > 0;

    /// <inheritdoc />
    public Task NavigateAsync<TTarget>(
        NavigationParameters? context = null,
        NavigationOptions? options = null)
        where TTarget : class
        => NavigateAsync(typeof(TTarget), context, options);

    /// <inheritdoc />
    public async Task NavigateAsync(
        Type targetType,
        NavigationParameters? context = null,
        NavigationOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(targetType);

        context ??= NavigationParameters.Empty;
        options ??= NavigationOptions.Default;

        var navigationKey = BuildNavigationKey(targetType, context, options);

        _logger.LogDebug(
            "Navigation requested. Target={TargetType}, NavigationKey={NavigationKey}, ClearBackStack={ClearBackStack}, AddToBackStack={AddToBackStack}",
            targetType.FullName,
            navigationKey,
            options.ClearBackStack,
            options.AddToBackStack);

        var request = new NavigationRequest
        {
            TargetType = targetType,
            Parameters = context,
            NavigationKey = navigationKey,
            IsBackNavigation = false
        };

        var canLeave = await CanLeaveCurrentAsync(request);

        if (canLeave.IsConfirmed.HasValue == false)
        {
            _logger.LogInformation(
                "Navigation cancelled by guard. Target={TargetType}, NavigationKey={NavigationKey}",
                targetType.FullName,
                navigationKey);

            return;
        }

        var target = _services.GetRequiredService(targetType);

        if (options.ClearBackStack)
        {
            _logger.LogDebug("Clearing back stack before navigation.");
            _backStack.Clear();
        }

        if (options.AddToBackStack && _currentEntry is not null)
        {
            _logger.LogDebug(
                "Pushing current module onto back stack. CurrentType={CurrentType}, NavigationKey={NavigationKey}",
                _currentEntry.TargetType.FullName,
                _currentEntry.NavigationKey);

            _backStack.Push(_currentEntry);
        }

        await ActivateAsync(target, context, navigationKey);

        _logger.LogInformation(
            "Navigation completed. Target={TargetType}, NavigationKey={NavigationKey}",
            targetType.FullName,
            navigationKey);
    }

    /// <inheritdoc />
    public async Task GoBackAsync()
    {
        if (!CanGoBack())
        {
            _logger.LogDebug("GoBack requested, but back stack is empty.");
            return;
        }

        _logger.LogDebug("Back navigation requested.");

        var targetEntry = _backStack.Peek();

        var request = new NavigationRequest
        {
            TargetType = targetEntry.TargetType,
            Parameters = targetEntry.Parameters,
            NavigationKey = targetEntry.NavigationKey,
            IsBackNavigation = true
        };

        var canLeave = await CanLeaveCurrentAsync(request);
        if (canLeave.IsConfirmed.HasValue == false)
        {
            _logger.LogInformation(
                "Back navigation cancelled by guard. Target={TargetType}, NavigationKey={NavigationKey}",
                targetEntry.TargetType.FullName,
                targetEntry.NavigationKey);

            return;
        }

        targetEntry = _backStack.Pop();

        _logger.LogDebug(
            "Back navigation target resolved. TargetType={TargetType}, NavigationKey={NavigationKey}",
            targetEntry.TargetType.FullName,
            targetEntry.NavigationKey);

        await ActivateAsync(targetEntry.Target, targetEntry.Parameters, targetEntry.NavigationKey);

        _logger.LogInformation(
            "Back navigation completed. TargetType={TargetType}, NavigationKey={NavigationKey}",
            targetEntry.TargetType.FullName,
            targetEntry.NavigationKey);
    }

    /// <inheritdoc />
    public Task<DialogResult> ShowDialogAsync<TDialog>(NavigationParameters? context = null)
        where TDialog : class
        => ShowDialogAsync(typeof(TDialog), context);

    /// <inheritdoc />
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

    private async Task ActivateAsync(object target, NavigationParameters context, string navigationKey)
    {
        _logger.LogDebug(
            "Activating target. TargetType={TargetType}, NavigationKey={NavigationKey}",
            target.GetType().FullName,
            navigationKey);

        Shell.CurrentModule = target;

        _currentEntry = new NavigationEntry(
            target,
            target.GetType(),
            context,
            navigationKey);

        if (target is INavigationAware aware)
        {
            await aware.OnNavigatedToAsync(context);
        }

        NavigationStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private async Task<DialogResult> CanLeaveCurrentAsync(NavigationRequest request)
    {
        if (_currentEntry is not null &&
            _currentEntry.TargetType == request.TargetType &&
            string.Equals(_currentEntry.NavigationKey, request.NavigationKey, StringComparison.Ordinal))
        {
            _logger.LogDebug(
                "Cannot navigate to the same target. TargetType={TargetType}, NavigationKey={NavigationKey}",
                request.TargetType?.FullName,
                request.NavigationKey);

            return DialogResult.None;
        }

        if (Shell.CurrentModule is not ICanNavigateFrom guarded)
        {
            _logger.LogDebug("Current module has no navigation guard.");
            return DialogResult.True;
        }

        _logger.LogDebug(
            "Evaluating navigation guard for current module. CurrentType={CurrentType}, IsBackNavigation={IsBackNavigation}, TargetType={TargetType}, NavigationKey={NavigationKey}",
            Shell.CurrentModule?.GetType().FullName,
            request.IsBackNavigation,
            request.TargetType?.FullName,
            request.NavigationKey);

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

    private static string BuildNavigationKey(
        Type targetType,
        NavigationParameters parameters,
        NavigationOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.NavigationKey))
        {
            return options.NavigationKey;
        }

        var normalizedParameters = parameters.ToNormalizedString();
        var typeName = targetType.FullName ?? targetType.Name;

        return string.IsNullOrEmpty(normalizedParameters)
            ? typeName
            : $"{normalizedParameters}|{typeName}";
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

    private sealed class NavigationEntry
    {
        public NavigationEntry(
            object target,
            Type targetType,
            NavigationParameters parameters,
            string navigationKey)
        {
            Target = target;
            TargetType = targetType;
            Parameters = parameters;
            NavigationKey = navigationKey;
        }

        public object Target { get; }

        public Type TargetType { get; }

        public NavigationParameters Parameters { get; }

        public string NavigationKey { get; }
    }
}
